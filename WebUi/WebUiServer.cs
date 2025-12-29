using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.Json;
using System.IO;
using System.Reflection;
using CerberusWareV3.Configuration;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using CerberusWareV3.Localization;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.IoC;
using Robust.Shared.GameObjects;
using Robust.Shared.Timing;

namespace WebUi
{
    public class WebUiServer : IDisposable
    {
        public void SetPlayerTracker(PlayerTrackerSystem tracker)
        {
            _playerTracker = tracker;
        }
        
        private PlayerTrackerSystem _playerTracker;
        private HttpListener _httpListener;
        private CancellationTokenSource _cts;
        private ConcurrentDictionary<string, WebSocket> _clients = new ConcurrentDictionary<string, WebSocket>();
        private readonly Assembly _assembly;
        // ВАЖНО: Проверьте этот путь! Должен быть Namespace.Папка.
        private readonly string _resourcePrefix = "WebUi.Resources.Web."; 

        public WebUiServer(string uriPrefix = "http://localhost:4649/")
        {
            _assembly = Assembly.GetExecutingAssembly();
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(uriPrefix);
        }

        public void Start()
        {
            try
            {
                _httpListener.Start();
                _cts = new CancellationTokenSource();
                Task.Run(() => ListenLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                Log.Error($"[WebUI] Start Error: {ex.Message}");
            }
        }

        private async Task ListenLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var context = await _httpListener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        ProcessWebSocketRequest(context);
                    }
                    else
                    {
                        ServeStaticContent(context);
                    }
                }
                catch { break; }
            }
        }

        private void ServeStaticContent(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                if (path == "/" || path == "/index.html") path = "index.html";

                string resourceName = _resourcePrefix + path.TrimStart('/').Replace('/', '.');

                using (Stream stream = _assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        context.Response.StatusCode = 200;
                        if (path.EndsWith(".html")) context.Response.ContentType = "text/html; charset=utf-8";
                        else if (path.EndsWith(".js")) context.Response.ContentType = "application/javascript";
                        else if (path.EndsWith(".css")) context.Response.ContentType = "text/css";
                        
                        stream.CopyTo(context.Response.OutputStream);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                    }
                }
                context.Response.Close();
            }
            catch { context.Response.Close(); }
        }

        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            WebSocketContext wsContext = null;
            try
            {
                wsContext = await context.AcceptWebSocketAsync(null);
                string clientId = Guid.NewGuid().ToString();
                _clients.TryAdd(clientId, wsContext.WebSocket);
                
                await SendConfig(wsContext.WebSocket);
                await ReceiveLoop(wsContext.WebSocket, clientId);
            }
            catch 
            {
                if(wsContext != null) wsContext.WebSocket.Dispose();
            }
        }

        private async Task ReceiveLoop(WebSocket ws, string clientId)
        {
            var buffer = new byte[65536];
            while (ws.State == WebSocketState.Open)
            {
                try
                {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        // Вызываем обработку сообщения
                        await HandleMessage(message, ws);
                    }
                }
                catch { break; }
            }
            _clients.TryRemove(clientId, out _);
        }

        private async Task HandleMessage(string json, WebSocket ws)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    string type = root.GetProperty("type").GetString();

                    if (type == "update_feature")
                    {
                        string id = root.GetProperty("id").GetString();
                        JsonElement value = root.GetProperty("value");
                        ApplyFeatureUpdate(id, value);
                    }
                    else if (type == "update_keybind") // НОВОЕ: Обработка биндов
                    {
                        string id = root.GetProperty("id").GetString();
                        string keyName = root.GetProperty("value").GetString();
                        ApplyKeybindUpdate(id, keyName);
                    }
                    else if (type == "get_players")
                    {
                        await SendPlayerList(ws);
                    }
                    // Обработка кнопок (Reset Zoom, Switch Lang, Configs)
                    else if (type == "button_click")
                    {
                        string id = root.GetProperty("id").GetString();
                        HandleButtonClick(id, root);
                    }
                }
            }
            catch (Exception e)
            {
                // Логируем ошибку, но не крашим поток
                Log.Error($"WebUI HandleMessage Error: {e.Message}"); 
            }
        }

        private void HandleButtonClick(string id, JsonElement root)
        {
            try
            {
                switch (id)
                {
                    case "y_reset": 
                        CerberusConfig.Eye.Zoom = 1.0f;
                        break;
                    case "st_ru": 
                        // Исправляем переключение языка
                        if (CerberusConfig.Settings.CurrentLanguage == Language.Ru)
                            CerberusConfig.Settings.CurrentLanguage = Language.En;
                        else
                            CerberusConfig.Settings.CurrentLanguage = Language.Ru;
                        
                        // ВАЖНО: После смены языка нужно обновить локализацию в UI (если она динамическая)
                        // Но ваше меню на React, там тексты захардкожены в data.ts.
                        // Если вы хотите менять язык МЕНЮ, вам нужно слать новый JSON с data.ts на клиент.
                        // Если речь про игровые строки (LocalizationManager), то это сработает для ESP и т.д.
                        break;
                    
                    case "cfg_save":
                        // Используем CerberusConfig.ConfigName, который обновляется из инпута
                        string saveName = CerberusConfig.ConfigName;
                        if (string.IsNullOrEmpty(saveName)) saveName = "default";
                        ConfigManager.SaveConfig(saveName);
                        break;
                        
                    case "cfg_load": // Добавим загрузку
                         string loadName = CerberusConfig.ConfigName;
                         if (string.IsNullOrEmpty(loadName)) loadName = "default";
                         var cfg = ConfigManager.LoadConfig(loadName);
                         if (cfg != null) ConfigManager.ApplyConfig(cfg);
                         // Тут хорошо бы отправить config_sync обратно клиенту
                         // await SendConfig(ws); // Но у нас тут нет доступа к ws в этом методе легко.
                        break;

                    case "cfg_open":
                        System.Diagnostics.Process.Start("explorer.exe", ConfigManager.configDir);
                        break;
                    case "st_unload":
                        MainController.Instance.Unload();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Button Click Error ({id}): {ex.Message}");
            }
        }

        private void ApplyKeybindUpdate(string id, string keyName)
        {
            try
            {
                if (!Enum.TryParse<Hexa.NET.ImGui.ImGuiKey>(keyName, true, out var key))
                {
                    key = Hexa.NET.ImGui.ImGuiKey.None;
                }

                switch (id)
                {
                    // Gun
                    case "g_enabled": // Если в меню бинд висит на этом тоггле
                        CerberusConfig.GunAimBot.HotKey = key; 
                        break;

                    // Melee - разделяем на Light и Heavy
                    // В меню (data.txt) вам нужно добавить отдельные пункты для биндинга, 
                    // либо обрабатывать специальные ID, которые вы пошлете из модалки.
                    
                    // ЕСЛИ в меню вы используете 'm_enabled' для Light Attack:
                    case "m_enabled": 
                        CerberusConfig.MeleeAimBot.LightHotKey = key; 
                        break;
                    
                    // А для Heavy Attack нужно добавить кнопку в меню или хардкодить второй ID.
                    // Допустим, вы добавите в data.txt кнопку "Heavy Bind" с id "m_heavy_bind"
                    case "m_heavy_bind":
                        CerberusConfig.MeleeAimBot.HeavyHotKey = key;
                        break;
                    
                    // Eye
                    case "y_fov": CerberusConfig.Eye.FovHotKey = key; break;
                    case "y_fullbright": CerberusConfig.Eye.FullBrightHotKey = key; break;
                    case "y_zoom": CerberusConfig.Eye.ZoomUpHotKey = key; break; // Up
                    // Zoom Down тоже нужен. Рекомендую в меню сделать 2 настройки для зума.
                    
                    case "st_menu": CerberusConfig.Settings.ShowMenuHotKey = key; break;
                    case "y_st_en": CerberusConfig.StorageViewer.HotKey = key; break;
                }
            }
            catch (Exception ex) { Log.Error($"Keybind Error: {ex.Message}"); }
        }
        

        private void ApplyFeatureUpdate(string id, JsonElement value)
        {
            try 
            {
                switch(id)
                {
                    // --- GUN AIMBOT ---
                    case "g_enabled": CerberusConfig.GunAimBot.Enabled = value.GetBoolean(); break;
                    case "g_radius": CerberusConfig.GunAimBot.CircleRadius = value.GetSingle(); break;
                    case "g_priority": 
                        string p = value.GetString();
                        if(p == "Distance") CerberusConfig.GunAimBot.TargetPriority = 0;
                        else if(p == "Mouse") CerberusConfig.GunAimBot.TargetPriority = 1;
                        else if(p == "Health") CerberusConfig.GunAimBot.TargetPriority = 2;
                        else if(p == "Role") CerberusConfig.GunAimBot.TargetPriority = 0; // Fallback
                        break;
                    case "g_only_prio": CerberusConfig.GunAimBot.OnlyPriority = value.GetBoolean(); break;
                    case "g_crit": CerberusConfig.GunAimBot.TargetCritical = value.GetBoolean(); break;
                    
                    case "g_hitscan": CerberusConfig.GunAimBot.HitScan = value.GetBoolean(); break;
                    case "g_spread": CerberusConfig.GunAimBot.MinSpread = value.GetBoolean(); break;
                    case "g_autopred": CerberusConfig.GunAimBot.AutoPredict = value.GetBoolean(); break;
                    case "g_predict": CerberusConfig.GunAimBot.PredictEnabled = value.GetBoolean(); break;
                    case "g_pred_corr": CerberusConfig.GunAimBot.PredictCorrection = value.GetSingle(); break;
                    
                    // --- GUN HELPER ---
                    case "g_ammo": CerberusConfig.GunHelper.ShowAmmo = value.GetBoolean(); if (value.GetBoolean()) CerberusConfig.GunHelper.Enabled = true; break;
                    case "g_bolt": CerberusConfig.GunHelper.AutoBolt = value.GetBoolean(); if (value.GetBoolean()) CerberusConfig.GunHelper.Enabled = true; break;
                    case "g_reload": CerberusConfig.GunHelper.AutoReload = value.GetBoolean(); if (value.GetBoolean()) CerberusConfig.GunHelper.Enabled = true; break;
                    case "g_reload_delay": CerberusConfig.GunHelper.AutoReloadDelay = value.GetSingle(); break;

                    // --- MELEE ---
                    case "m_enabled": CerberusConfig.MeleeAimBot.Enabled = value.GetBoolean(); break;
                    case "m_radius": CerberusConfig.MeleeAimBot.CircleRadius = value.GetSingle(); break;
                    case "m_priority":
                         string pm = value.GetString();
                        if(pm == "Distance") CerberusConfig.MeleeAimBot.TargetPriority = 0;
                        else if(pm == "Mouse") CerberusConfig.MeleeAimBot.TargetPriority = 1;
                        else if(pm == "Health") CerberusConfig.MeleeAimBot.TargetPriority = 2;
                        break;
                    case "m_only_prio": CerberusConfig.MeleeAimBot.OnlyPriority = value.GetBoolean(); break;
                    case "m_crit": CerberusConfig.MeleeAimBot.TargetCritical = value.GetBoolean(); break;
                    
                    case "m_rotate": CerberusConfig.MeleeHelper.RotateToTarget = value.GetBoolean(); break;
                    case "m_net": CerberusConfig.MeleeAimBot.FixNetworkDelay = value.GetBoolean(); break;
                    
                    case "m_circle": CerberusConfig.MeleeAimBot.ShowCircle = value.GetBoolean(); break;
                    case "m_line": CerberusConfig.MeleeAimBot.ShowLine = value.GetBoolean(); break;
                    case "m_color": CerberusConfig.MeleeAimBot.Color = ParseColor(value.GetString()); break;
                    
                    case "m_auto_atk": CerberusConfig.MeleeHelper.AutoAttack = value.GetBoolean(); if (value.GetBoolean()) CerberusConfig.MeleeHelper.Enabled = true; break;
                    case "m_360": CerberusConfig.MeleeHelper.Attack360 = value.GetBoolean(); if (value.GetBoolean()) CerberusConfig.MeleeHelper.Enabled = true; break;

                    // --- ESP ---
                    case "e_enabled": CerberusConfig.Esp.Enabled = value.GetBoolean(); break;
                    case "e_name": CerberusConfig.Esp.ShowName = value.GetBoolean(); break;
                    case "e_ckey": CerberusConfig.Esp.ShowCKey = value.GetBoolean(); break;
                    case "e_antag": CerberusConfig.Esp.ShowAntag = value.GetBoolean(); break;
                    case "e_friend": CerberusConfig.Esp.ShowFriend = value.GetBoolean(); break;
                    case "e_prio": CerberusConfig.Esp.ShowPriority = value.GetBoolean(); break;
                    case "e_combat": CerberusConfig.Esp.ShowCombatMode = value.GetBoolean(); break;
                    case "e_implants": CerberusConfig.Esp.ShowImplants = value.GetBoolean(); break;
                    case "e_contra": CerberusConfig.Esp.ShowContraband = value.GetBoolean(); break;
                    case "e_wep": CerberusConfig.Esp.ShowWeapon = value.GetBoolean(); break;
                    case "e_slip": CerberusConfig.Esp.ShowNoSlip = value.GetBoolean(); break;

                    // ESP Colors
                    case "e_c_name": CerberusConfig.Esp.NameColor = ParseColor(value.GetString()); break;
                    case "e_c_ckey": CerberusConfig.Esp.CKeyColor = ParseColor(value.GetString()); break;
                    case "e_c_antag": CerberusConfig.Esp.AntagColor = ParseColor(value.GetString()); break;
                    case "e_c_friend": CerberusConfig.Esp.FriendColor = ParseColor(value.GetString()); break;
                    case "e_c_prio": CerberusConfig.Esp.PriorityColor = ParseColor(value.GetString()); break;
                    case "e_c_combat": CerberusConfig.Esp.CombatModeColor = ParseColor(value.GetString()); break;
                    case "e_c_impl": CerberusConfig.Esp.ImplantsColor = ParseColor(value.GetString()); break;
                    case "e_c_cont": CerberusConfig.Esp.ContrabandColor = ParseColor(value.GetString()); break;
                    case "e_c_wep": CerberusConfig.Esp.WeaponColor = ParseColor(value.GetString()); break;
                    case "e_c_slip": CerberusConfig.Esp.NoSlipColor = ParseColor(value.GetString()); break;

                    case "e_interval": CerberusConfig.Esp.FontInterval = value.GetInt32(); break;
                    case "e_size": CerberusConfig.Esp.MainFontSize = value.GetInt32(); break;

                    // --- EYE & HUD ---
                    case "y_fov": CerberusConfig.Eye.FovEnabled = value.GetBoolean(); break;
                    case "y_fullbright": CerberusConfig.Eye.FullBrightEnabled = value.GetBoolean(); break;
                    case "y_zoom": CerberusConfig.Eye.Zoom = value.GetSingle(); break;
                    
                    case "y_health": CerberusConfig.Hud.ShowHealth = value.GetBoolean(); break;
                    case "y_stam": CerberusConfig.Hud.ShowStamina = value.GetBoolean(); break;
                    case "y_hud_col": CerberusConfig.Hud.StaminaColor = ParseColor(value.GetString()); break;
                    case "y_antag_i": CerberusConfig.Hud.ShowAntag = value.GetBoolean(); break;
                    case "y_job_i": CerberusConfig.Hud.ShowJobIcons = value.GetBoolean(); break;
                    case "y_mind_i": CerberusConfig.Hud.ShowMindShieldIcons = value.GetBoolean(); break;
                    case "y_rec_i": CerberusConfig.Hud.ShowCriminalRecordIcons = value.GetBoolean(); break;
                    case "y_syn_i": CerberusConfig.Hud.ShowSyndicateIcons = value.GetBoolean(); break;
                    case "y_chem": CerberusConfig.Hud.ChemicalAnalysis = value.GetBoolean(); break;
                    case "y_shock": CerberusConfig.Hud.ShowElectrocution = value.GetBoolean(); break;

                    case "y_clyde": CerberusConfig.Settings.ClydePatch = value.GetBoolean(); break;
                    case "y_smoke": CerberusConfig.Settings.SmokePatch = value.GetBoolean(); break;
                    case "y_overlay": CerberusConfig.Settings.OverlaysPatch = value.GetBoolean(); break;
                    case "y_recoil": CerberusConfig.Settings.NoCameraKickPatch = value.GetBoolean(); break;

                    case "y_st_en": CerberusConfig.StorageViewer.Enabled = value.GetBoolean(); break;
                    case "y_st_col": CerberusConfig.StorageViewer.Color = ParseColor(value.GetString()); break;

                    // --- FUN ---
                    case "f_rainbow": CerberusConfig.Fun.RainbowEnabled = value.GetBoolean(); break;
                    case "f_spin": CerberusConfig.Fun.RotationEnabled = value.GetBoolean(); break;

                    // --- MISC ---
                    case "mi_soap": CerberusConfig.Misc.AntiSoapEnabled = value.GetBoolean(); break;
                    case "mi_afk": CerberusConfig.Misc.AntiAfkEnabled = value.GetBoolean(); break;
                    case "mi_trash": CerberusConfig.Misc.TrashTalkEnabled = value.GetBoolean(); break;
                    case "mi_antiaim": CerberusConfig.Misc.AntiAimEnabled = value.GetBoolean(); break;
                    case "mi_speed": CerberusConfig.Misc.AutoRotateSpeed = value.GetSingle(); break;

                    case "mi_exp": CerberusConfig.Misc.ShowExplosive = value.GetBoolean(); break;
                    case "mi_traj": CerberusConfig.Misc.ShowTrajectory = value.GetBoolean(); break;
                    case "mi_dmg": CerberusConfig.Misc.DamageOverlayEnabled = value.GetBoolean(); break;

                    // --- SPAMMER ---
                    case "s_c_prot": CerberusConfig.Spammer.ProtectTextEnabled = value.GetBoolean(); break;
                    case "s_ch_en": CerberusConfig.Spammer.ChatEnabled = value.GetBoolean(); break;
                    case "s_ch_del": CerberusConfig.Spammer.ChatDelay = value.GetInt32(); break;
                    case "s_ch_txt": CerberusConfig.Spammer.ChatText = value.GetString(); break;

                    case "s_ah_en": CerberusConfig.Spammer.AHelpEnabled = value.GetBoolean(); break;
                    case "s_ah_del": CerberusConfig.Spammer.AHelpDelay = value.GetInt32(); break;
                    case "s_ah_txt": CerberusConfig.Spammer.AHelpText = value.GetString(); break;

                    // --- UTILITY ---
                    case "s_tr_en": CerberusConfig.Settings.TranslateChatPatch = value.GetBoolean(); break;
                    case "s_tr_lang": CerberusConfig.Settings.TranslateChatLang = value.GetString(); break;

                    case "s_item_en": CerberusConfig.Misc.ItemSearcherEnabled = value.GetBoolean(); break;
                    case "s_item_name": CerberusConfig.Misc.ItemSearcherShowName = value.GetBoolean(); break;
                    
                    case "s_item_list":
                        var items = JsonSerializer.Deserialize<List<ItemDto>>(value.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        CerberusConfig.Misc.ItemSearchEntries.Clear();
                        if (items != null)
                        {
                            foreach(var item in items)
                            {
                                CerberusConfig.Misc.ItemSearchEntries.Add(new ItemSearchEntry 
                                { 
                                    ItemName = item.Name, 
                                    Color = ParseColor(item.Color) 
                                });
                            }
                        }
                        break;

                    // --- SETTINGS ---
                    case "st_menu": CerberusConfig.Settings.ShowMenu = value.GetBoolean(); break;
                    case "st_notif": CerberusConfig.Notifications.Enabled = value.GetBoolean(); break;
                    
                    case "st_admin": CerberusConfig.Settings.AdminPatch = value.GetBoolean(); break;
                    case "st_dmg_friend": CerberusConfig.Settings.NoDmgFriendPatch = value.GetBoolean(); break;
                    case "st_dmg_fs": CerberusConfig.Settings.DamageForcePatch = value.GetBoolean(); break;
                    case "st_grab": CerberusConfig.Settings.AntiScreenGrubPatch = value.GetBoolean(); break;

                    case "cfg_name": CerberusConfig.ConfigName = value.GetString(); break;
                }
            }
            catch(Exception ex)
            {
                Log.Error($"Error apply feature {id}: {ex.Message}");
            }
        }

        private async Task SendPlayerList(WebSocket ws)
        {
            try
            {
                // Используем сохраненную ссылку вместо IoCManager.Resolve
                if (_playerTracker == null)
                {
                    // Попытка получить через IoC как запасной вариант (но лучше передать явно)
                    try 
                    {
                        var sysMan = IoCManager.Resolve<IEntitySystemManager>();
                        sysMan.TryGetEntitySystem(out _playerTracker);
                    }
                    catch 
                    {
                        // Если и тут не вышло, значит зависимостей нет совсем
                        return; 
                    }
                }

                if (_playerTracker == null) return;

                var playersList = new List<object>();

                // Блокируем или безопасно копируем данные
                // PlayerData - ссылочный тип, но словарь AllPlayerSessions может меняться
                // Попробуем сделать копию значений. Если словарь модифицируется в этот момент, может быть исключение.
                // Лучший вариант без блокировок в движке - try-catch.
                
                IEnumerable<PlayerData> sessionsSnapshot = null;
                try
                {
                    if (_playerTracker.AllPlayerSessions != null)
                        sessionsSnapshot = _playerTracker.AllPlayerSessions.Values.ToArray();
                }
                catch { /* Игнорируем ошибки конкурентного доступа */ }

                if (sessionsSnapshot != null)
                {
                    foreach (var p in sessionsSnapshot)
                    {
                        if (p == null) continue;

                        playersList.Add(new 
                        {
                            name = p.Session?.Name ?? "Unknown",
                            charName = p.EntityName ?? "Unknown", 
                            entity = p.AttachedEntity?.ToString() ?? "None",
                            status = p.Status ?? "Offline",
                            job = p.Job ?? "Unknown"
                        });
                    }
                }

                var payload = new { type = "player_list", data = playersList };
                var json = JsonSerializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);

                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"SendPlayerList Error: {ex.Message}");
            }
        }

        private async Task SendConfig(WebSocket ws)
        {
             try 
            {
                var configData = ConfigManager.GatherData();
                var payload = new { type = "config_sync", data = configData };
                var json = JsonSerializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch { }
        }

        private Vector4 ParseColor(string hex)
        {
            if (string.IsNullOrEmpty(hex)) return Vector4.One;
            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
                float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
                float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;
                return new Vector4(r, g, b, 1.0f);
            }
            return Vector4.One;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _httpListener?.Stop();
            foreach(var client in _clients.Values) client.Dispose();
        }
        
        private class ItemDto
        {
            public string Name { get; set; }
            public string Color { get; set; }
        }
        
        [Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	
        [Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	
        [Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	
        [Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	
        [Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _sysMan = null;
    }
    
}