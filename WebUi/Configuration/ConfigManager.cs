using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CerberusWareV3.Configuration
{
	public class ConfigManager
	{
		public static void SaveConfig(string name)
		{
			JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = 
				{
					new ConfigManager.Vector4Converter()
				}
			};
			string text = JsonSerializer.Serialize<ConfigManagerData>(ConfigManager.GatherData(), jsonSerializerOptions);
			if (!Directory.Exists(ConfigManager.configDir))
			{
				Directory.CreateDirectory(ConfigManager.configDir);
			}
			File.WriteAllText(Path.Combine(ConfigManager.configDir, name + ".json"), text);
		}
		public static ConfigManagerData LoadConfig(string name)
		{
			string text = Path.Combine(ConfigManager.configDir, name + ".json");
			if (!File.Exists(text))
			{
				return new ConfigManagerData();
			}
			JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
			{
				Converters = 
				{
					new ConfigManager.Vector4Converter()
				}
			};
			return JsonSerializer.Deserialize<ConfigManagerData>(File.ReadAllText(text), jsonSerializerOptions);
		}
		public static ConfigManagerData GatherData()
		{
			return new ConfigManagerData
			{
				GunAimBotData = 
				{
					Enabled = CerberusConfig.GunAimBot.Enabled,
					HotKey = CerberusConfig.GunAimBot.HotKey,
					TargetCritical = CerberusConfig.GunAimBot.TargetCritical,
					MinSpread = CerberusConfig.GunAimBot.MinSpread,
					HitScan = CerberusConfig.GunAimBot.HitScan,
					AutoPredict = CerberusConfig.GunAimBot.AutoPredict,
					PredictEnabled = CerberusConfig.GunAimBot.PredictEnabled,
					PredictCorrection = CerberusConfig.GunAimBot.PredictCorrection,
					ShowCircle = CerberusConfig.GunAimBot.ShowCircle,
					ShowLine = CerberusConfig.GunAimBot.ShowLine,
					CircleRadius = CerberusConfig.GunAimBot.CircleRadius,
					Color = CerberusConfig.GunAimBot.Color,
					TargetPriority = CerberusConfig.GunAimBot.TargetPriority,
					OnlyPriority = CerberusConfig.GunAimBot.OnlyPriority
				},
				MeleeAimBotData = 
				{
					Enabled = CerberusConfig.MeleeAimBot.Enabled,
					LightHotKey = CerberusConfig.MeleeAimBot.LightHotKey,
					HeavyHotKey = CerberusConfig.MeleeAimBot.HeavyHotKey,
					TargetCritical = CerberusConfig.MeleeAimBot.TargetCritical,
					ShowCircle = CerberusConfig.MeleeAimBot.ShowCircle,
					ShowLine = CerberusConfig.MeleeAimBot.ShowLine,
					CircleRadius = CerberusConfig.MeleeAimBot.CircleRadius,
					Color = CerberusConfig.MeleeAimBot.Color,
					TargetPriority = CerberusConfig.MeleeAimBot.TargetPriority,
					OnlyPriority = CerberusConfig.MeleeAimBot.OnlyPriority,
					FixNetworkDelay = CerberusConfig.MeleeAimBot.FixNetworkDelay,
					FixDelay = CerberusConfig.MeleeAimBot.FixDelay
				},
				GunHelperData = 
				{
					Enabled = CerberusConfig.GunHelper.Enabled,
					ShowAmmo = CerberusConfig.GunHelper.ShowAmmo,
					AutoBolt = CerberusConfig.GunHelper.AutoBolt,
					AutoReload = CerberusConfig.GunHelper.AutoReload,
					AutoReloadDelay = CerberusConfig.GunHelper.AutoReloadDelay
				},
				MeleeHelperData = 
				{
					Enabled = CerberusConfig.MeleeHelper.Enabled,
					Attack360 = CerberusConfig.MeleeHelper.Attack360,
					AutoAttack = CerberusConfig.MeleeHelper.AutoAttack,
					RotateToTarget = CerberusConfig.MeleeHelper.RotateToTarget
				},
				EspData = 
				{
					Enabled = CerberusConfig.Esp.Enabled,
					ShowName = CerberusConfig.Esp.ShowName,
					ShowCKey = CerberusConfig.Esp.ShowCKey,
					ShowAntag = CerberusConfig.Esp.ShowAntag,
					ShowFriend = CerberusConfig.Esp.ShowFriend,
					ShowPriority = CerberusConfig.Esp.ShowPriority,
					ShowCombatMode = CerberusConfig.Esp.ShowCombatMode,
					ShowImplants = CerberusConfig.Esp.ShowImplants,
					ShowContraband = CerberusConfig.Esp.ShowContraband,
					ShowWeapon = CerberusConfig.Esp.ShowWeapon,
					ShowNoSlip = CerberusConfig.Esp.ShowNoSlip,
					NameColor = CerberusConfig.Esp.NameColor,
					CKeyColor = CerberusConfig.Esp.CKeyColor,
					AntagColor = CerberusConfig.Esp.AntagColor,
					FriendColor = CerberusConfig.Esp.FriendColor,
					PriorityColor = CerberusConfig.Esp.PriorityColor,
					CombatModeColor = CerberusConfig.Esp.CombatModeColor,
					ImplantsColor = CerberusConfig.Esp.ImplantsColor,
					ContrabandColor = CerberusConfig.Esp.ContrabandColor,
					WeaponColor = CerberusConfig.Esp.WeaponColor,
					NoSlipColor = CerberusConfig.Esp.NoSlipColor,
					MainFontPath = CerberusConfig.Esp.MainFontPath,
					MainFontIndex = CerberusConfig.Esp.MainFontIndex,
					MainFontSize = CerberusConfig.Esp.MainFontSize,
					OtherFontPath = CerberusConfig.Esp.OtherFontPath,
					OtherFontIndex = CerberusConfig.Esp.OtherFontIndex,
					OtherFontSize = CerberusConfig.Esp.OtherFontSize,
					FontInterval = CerberusConfig.Esp.FontInterval
				},
				EyeData = 
				{
					FovEnabled = CerberusConfig.Eye.FovEnabled,
					FullBrightEnabled = CerberusConfig.Eye.FullBrightEnabled,
					Zoom = CerberusConfig.Eye.Zoom,
					SuperFastZoom = CerberusConfig.Eye.SuperFastZoom,
					FovHotKey = CerberusConfig.Eye.FovHotKey,
					FullBrightHotKey = CerberusConfig.Eye.FullBrightHotKey,
					ZoomUpHotKey = CerberusConfig.Eye.ZoomUpHotKey,
					ZoomDownHotKey = CerberusConfig.Eye.ZoomDownHotKey
				},
				HudData = 
				{
					ShowHealth = CerberusConfig.Hud.ShowHealth,
					ShowAntag = CerberusConfig.Hud.ShowAntag,
					ShowJobIcons = CerberusConfig.Hud.ShowJobIcons,
					ShowMindShieldIcons = CerberusConfig.Hud.ShowMindShieldIcons,
					ShowCriminalRecordIcons = CerberusConfig.Hud.ShowCriminalRecordIcons,
					ShowSyndicateIcons = CerberusConfig.Hud.ShowSyndicateIcons,
					ChemicalAnalysis = CerberusConfig.Hud.ChemicalAnalysis,
					ShowElectrocution = CerberusConfig.Hud.ShowElectrocution,
					ShowStamina = CerberusConfig.Hud.ShowStamina,
					StaminaColor = CerberusConfig.Hud.StaminaColor
				},
				StorageViewerData = 
				{
					Enabled = CerberusConfig.StorageViewer.Enabled,
					Color = CerberusConfig.StorageViewer.Color
				},
				SpammerData = 
				{
					ChatEnabled = CerberusConfig.Spammer.ChatEnabled,
					ChatText = CerberusConfig.Spammer.ChatText,
					ChatDelay = CerberusConfig.Spammer.ChatDelay,
					ProtectTextEnabled = CerberusConfig.Spammer.ProtectTextEnabled,
					ProtectRandomLength = CerberusConfig.Spammer.ProtectRandomLength,
					ProtectLength = CerberusConfig.Spammer.ProtectLength,
					AHelpEnabled = CerberusConfig.Spammer.AHelpEnabled,
					AHelpText = CerberusConfig.Spammer.AHelpText,
					AHelpDelay = CerberusConfig.Spammer.AHelpDelay,
					Channels = CerberusConfig.Spammer.Channels
				},
				MiscData = 
				{
					TrashTalkEnabled = CerberusConfig.Misc.TrashTalkEnabled,
					DamageOverlayEnabled = CerberusConfig.Misc.DamageOverlayEnabled,
					AntiSoapEnabled = CerberusConfig.Misc.AntiSoapEnabled,
					AntiAfkEnabled = CerberusConfig.Misc.AntiAfkEnabled,
					AntiAimEnabled = CerberusConfig.Misc.AntiAimEnabled,
					AutoRotateSpeed = CerberusConfig.Misc.AutoRotateSpeed,
					ItemSearcherEnabled = CerberusConfig.Misc.ItemSearcherEnabled,
					ItemSearchEntries = CerberusConfig.Misc.ItemSearchEntries,
					ItemSearcherShowName = CerberusConfig.Misc.ItemSearcherShowName,
					ShowExplosive = CerberusConfig.Misc.ShowExplosive,
					ShowTrajectory = CerberusConfig.Misc.ShowTrajectory
				},
				FunData = 
				{
					Enabled = CerberusConfig.Fun.Enabled,
					RainbowEnabled = CerberusConfig.Fun.RainbowEnabled,
					RotationEnabled = CerberusConfig.Fun.RotationEnabled,
					TrailsEnabled = CerberusConfig.Fun.TrailsEnabled,
					JumpEnabled = CerberusConfig.Fun.JumpEnabled,
					ShakeEnabled = CerberusConfig.Fun.ShakeEnabled,
					RotationSpeed = CerberusConfig.Fun.RotationSpeed,
					Color = CerberusConfig.Fun.Color,
					ScaleX = CerberusConfig.Fun.ScaleX,
					ScaleY = CerberusConfig.Fun.ScaleY,
					RainbowSpeed = CerberusConfig.Fun.RainbowSpeed,
					AffectPlayer = CerberusConfig.Fun.AffectPlayer,
					AffectMobs = CerberusConfig.Fun.AffectMobs,
					AffectOthers = CerberusConfig.Fun.AffectOthers
				},
				TextureData = 
				{
					Enabled = CerberusConfig.Texture.Enabled,
					Size = CerberusConfig.Texture.Size,
					MakeEntitiesInvisible = CerberusConfig.Texture.MakeEntitiesInvisible
				},
				SettingsData = 
				{
					UiCustomizable = CerberusConfig.Settings.UiCustomizable,
					ShowMenu = CerberusConfig.Settings.ShowMenu,
					ShowMenuHotKey = CerberusConfig.Settings.ShowMenuHotKey,
					ShowDebugConsole = CerberusConfig.Settings.ShowDebugConsole,
					CurrentLanguage = CerberusConfig.Settings.CurrentLanguage,
					ClydePatch = CerberusConfig.Settings.ClydePatch,
					OverlaysPatch = CerberusConfig.Settings.OverlaysPatch,
					SmokePatch = CerberusConfig.Settings.SmokePatch,
					AdminPatch = CerberusConfig.Settings.AdminPatch,
					DamageForcePatch = CerberusConfig.Settings.DamageForcePatch,
					NoDmgFriendPatch = CerberusConfig.Settings.NoDmgFriendPatch,
					AntiScreenGrubPatch = CerberusConfig.Settings.AntiScreenGrubPatch,
					TranslateChatPatch = CerberusConfig.Settings.TranslateChatPatch,
					TranslateChatLang = CerberusConfig.Settings.TranslateChatLang,
					TranslateMePatch = CerberusConfig.Settings.TranslateMePatch,
					TranslateMeLang = CerberusConfig.Settings.TranslateMeLang,
					NoCameraKickPatch = CerberusConfig.Settings.NoCameraKickPatch
				},
				NotificationsData = 
				{
					Enabled = CerberusConfig.Notifications.Enabled,
					MaxNotifications = CerberusConfig.Notifications.MaxNotifications,
					FontSize = CerberusConfig.Notifications.FontSize,
					AnchorPosition = CerberusConfig.Notifications.AnchorPosition,
					IgnoreSizeCheck = CerberusConfig.Notifications.IgnoreSizeCheck
				},
				NoSavedConfigData = 
				{
					HasTarget = CerberusConfig.NoSavedConfig.HasTarget,
					HasAntiCheat = CerberusConfig.NoSavedConfig.HasAntiCheat,
					Version = CerberusConfig.NoSavedConfig.Version
				}
			};
		}
		public static void ApplyConfig(ConfigManagerData config)
		{
			CerberusConfig.GunAimBot.Enabled = config.GunAimBotData.Enabled;
			CerberusConfig.GunAimBot.HotKey = config.GunAimBotData.HotKey;
			CerberusConfig.GunAimBot.TargetCritical = config.GunAimBotData.TargetCritical;
			CerberusConfig.GunAimBot.MinSpread = config.GunAimBotData.MinSpread;
			CerberusConfig.GunAimBot.HitScan = config.GunAimBotData.HitScan;
			CerberusConfig.GunAimBot.AutoPredict = config.GunAimBotData.AutoPredict;
			CerberusConfig.GunAimBot.PredictEnabled = config.GunAimBotData.PredictEnabled;
			CerberusConfig.GunAimBot.PredictCorrection = config.GunAimBotData.PredictCorrection;
			CerberusConfig.GunAimBot.ShowCircle = config.GunAimBotData.ShowCircle;
			CerberusConfig.GunAimBot.ShowLine = config.GunAimBotData.ShowLine;
			CerberusConfig.GunAimBot.CircleRadius = config.GunAimBotData.CircleRadius;
			CerberusConfig.GunAimBot.Color = config.GunAimBotData.Color;
			CerberusConfig.GunAimBot.TargetPriority = config.GunAimBotData.TargetPriority;
			CerberusConfig.GunAimBot.OnlyPriority = config.GunAimBotData.OnlyPriority;
			CerberusConfig.MeleeAimBot.Enabled = config.MeleeAimBotData.Enabled;
			CerberusConfig.MeleeAimBot.LightHotKey = config.MeleeAimBotData.LightHotKey;
			CerberusConfig.MeleeAimBot.HeavyHotKey = config.MeleeAimBotData.HeavyHotKey;
			CerberusConfig.MeleeAimBot.TargetCritical = config.MeleeAimBotData.TargetCritical;
			CerberusConfig.MeleeAimBot.ShowCircle = config.MeleeAimBotData.ShowCircle;
			CerberusConfig.MeleeAimBot.ShowLine = config.MeleeAimBotData.ShowLine;
			CerberusConfig.MeleeAimBot.CircleRadius = config.MeleeAimBotData.CircleRadius;
			CerberusConfig.MeleeAimBot.Color = config.MeleeAimBotData.Color;
			CerberusConfig.MeleeAimBot.TargetPriority = config.MeleeAimBotData.TargetPriority;
			CerberusConfig.MeleeAimBot.OnlyPriority = config.MeleeAimBotData.OnlyPriority;
			CerberusConfig.MeleeAimBot.FixNetworkDelay = config.MeleeAimBotData.FixNetworkDelay;
			CerberusConfig.MeleeAimBot.FixDelay = config.MeleeAimBotData.FixDelay;
			CerberusConfig.GunHelper.Enabled = config.GunHelperData.Enabled;
			CerberusConfig.GunHelper.ShowAmmo = config.GunHelperData.ShowAmmo;
			CerberusConfig.GunHelper.AutoBolt = config.GunHelperData.AutoBolt;
			CerberusConfig.GunHelper.AutoReload = config.GunHelperData.AutoReload;
			CerberusConfig.GunHelper.AutoReloadDelay = config.GunHelperData.AutoReloadDelay;
			CerberusConfig.MeleeHelper.Enabled = config.MeleeHelperData.Enabled;
			CerberusConfig.MeleeHelper.Attack360 = config.MeleeHelperData.Attack360;
			CerberusConfig.MeleeHelper.AutoAttack = config.MeleeHelperData.AutoAttack;
			CerberusConfig.MeleeHelper.RotateToTarget = config.MeleeHelperData.RotateToTarget;
			CerberusConfig.Esp.Enabled = config.EspData.Enabled;
			CerberusConfig.Esp.ShowName = config.EspData.ShowName;
			CerberusConfig.Esp.ShowCKey = config.EspData.ShowCKey;
			CerberusConfig.Esp.ShowAntag = config.EspData.ShowAntag;
			CerberusConfig.Esp.ShowFriend = config.EspData.ShowFriend;
			CerberusConfig.Esp.ShowPriority = config.EspData.ShowPriority;
			CerberusConfig.Esp.ShowCombatMode = config.EspData.ShowCombatMode;
			CerberusConfig.Esp.ShowImplants = config.EspData.ShowImplants;
			CerberusConfig.Esp.ShowContraband = config.EspData.ShowContraband;
			CerberusConfig.Esp.ShowWeapon = config.EspData.ShowWeapon;
			CerberusConfig.Esp.ShowNoSlip = config.EspData.ShowNoSlip;
			CerberusConfig.Esp.NameColor = config.EspData.NameColor;
			CerberusConfig.Esp.CKeyColor = config.EspData.CKeyColor;
			CerberusConfig.Esp.AntagColor = config.EspData.AntagColor;
			CerberusConfig.Esp.FriendColor = config.EspData.FriendColor;
			CerberusConfig.Esp.PriorityColor = config.EspData.PriorityColor;
			CerberusConfig.Esp.CombatModeColor = config.EspData.CombatModeColor;
			CerberusConfig.Esp.ImplantsColor = config.EspData.ImplantsColor;
			CerberusConfig.Esp.ContrabandColor = config.EspData.ContrabandColor;
			CerberusConfig.Esp.WeaponColor = config.EspData.WeaponColor;
			CerberusConfig.Esp.NoSlipColor = config.EspData.NoSlipColor;
			CerberusConfig.Esp.MainFontPath = config.EspData.MainFontPath;
			CerberusConfig.Esp.MainFontIndex = config.EspData.MainFontIndex;
			CerberusConfig.Esp.MainFontSize = config.EspData.MainFontSize;
			CerberusConfig.Esp.OtherFontPath = config.EspData.OtherFontPath;
			CerberusConfig.Esp.OtherFontIndex = config.EspData.OtherFontIndex;
			CerberusConfig.Esp.OtherFontSize = config.EspData.OtherFontSize;
			CerberusConfig.Esp.FontInterval = config.EspData.FontInterval;
			CerberusConfig.Eye.FovEnabled = config.EyeData.FovEnabled;
			CerberusConfig.Eye.FullBrightEnabled = config.EyeData.FullBrightEnabled;
			CerberusConfig.Eye.Zoom = config.EyeData.Zoom;
			CerberusConfig.Eye.SuperFastZoom = config.EyeData.SuperFastZoom;
			CerberusConfig.Eye.FovHotKey = config.EyeData.FovHotKey;
			CerberusConfig.Eye.FullBrightHotKey = config.EyeData.FullBrightHotKey;
			CerberusConfig.Eye.ZoomUpHotKey = config.EyeData.ZoomUpHotKey;
			CerberusConfig.Eye.ZoomDownHotKey = config.EyeData.ZoomDownHotKey;
			CerberusConfig.Hud.ShowHealth = config.HudData.ShowHealth;
			CerberusConfig.Hud.ShowAntag = config.HudData.ShowAntag;
			CerberusConfig.Hud.ShowJobIcons = config.HudData.ShowJobIcons;
			CerberusConfig.Hud.ShowMindShieldIcons = config.HudData.ShowMindShieldIcons;
			CerberusConfig.Hud.ShowCriminalRecordIcons = config.HudData.ShowCriminalRecordIcons;
			CerberusConfig.Hud.ShowSyndicateIcons = config.HudData.ShowSyndicateIcons;
			CerberusConfig.Hud.ChemicalAnalysis = config.HudData.ChemicalAnalysis;
			CerberusConfig.Hud.ShowElectrocution = config.HudData.ShowElectrocution;
			CerberusConfig.Hud.ShowStamina = config.HudData.ShowStamina;
			CerberusConfig.Hud.StaminaColor = config.HudData.StaminaColor;
			CerberusConfig.StorageViewer.Enabled = config.StorageViewerData.Enabled;
			CerberusConfig.StorageViewer.Color = config.StorageViewerData.Color;
			CerberusConfig.Spammer.ChatEnabled = config.SpammerData.ChatEnabled;
			CerberusConfig.Spammer.ChatText = config.SpammerData.ChatText;
			CerberusConfig.Spammer.ChatDelay = config.SpammerData.ChatDelay;
			CerberusConfig.Spammer.ProtectTextEnabled = config.SpammerData.ProtectTextEnabled;
			CerberusConfig.Spammer.ProtectRandomLength = config.SpammerData.ProtectRandomLength;
			CerberusConfig.Spammer.ProtectLength = config.SpammerData.ProtectLength;
			CerberusConfig.Spammer.AHelpEnabled = config.SpammerData.AHelpEnabled;
			CerberusConfig.Spammer.AHelpText = config.SpammerData.AHelpText;
			CerberusConfig.Spammer.AHelpDelay = config.SpammerData.AHelpDelay;
			CerberusConfig.Spammer.Channels = config.SpammerData.Channels;
			CerberusConfig.Misc.TrashTalkEnabled = config.MiscData.TrashTalkEnabled;
			CerberusConfig.Misc.DamageOverlayEnabled = config.MiscData.DamageOverlayEnabled;
			CerberusConfig.Misc.AntiSoapEnabled = config.MiscData.AntiSoapEnabled;
			CerberusConfig.Misc.AntiAfkEnabled = config.MiscData.AntiAfkEnabled;
			CerberusConfig.Misc.AntiAimEnabled = config.MiscData.AntiAimEnabled;
			CerberusConfig.Misc.AutoRotateSpeed = config.MiscData.AutoRotateSpeed;
			CerberusConfig.Misc.ItemSearcherEnabled = config.MiscData.ItemSearcherEnabled;
			CerberusConfig.Misc.ItemSearchEntries = config.MiscData.ItemSearchEntries;
			CerberusConfig.Misc.ItemSearcherShowName = config.MiscData.ItemSearcherShowName;
			CerberusConfig.Misc.ShowExplosive = config.MiscData.ShowExplosive;
			CerberusConfig.Misc.ShowTrajectory = config.MiscData.ShowTrajectory;
			CerberusConfig.Fun.Enabled = config.FunData.Enabled;
			CerberusConfig.Fun.RainbowEnabled = config.FunData.RainbowEnabled;
			CerberusConfig.Fun.RotationEnabled = config.FunData.RotationEnabled;
			CerberusConfig.Fun.TrailsEnabled = config.FunData.TrailsEnabled;
			CerberusConfig.Fun.JumpEnabled = config.FunData.JumpEnabled;
			CerberusConfig.Fun.ShakeEnabled = config.FunData.ShakeEnabled;
			CerberusConfig.Fun.RotationSpeed = config.FunData.RotationSpeed;
			CerberusConfig.Fun.Color = config.FunData.Color;
			CerberusConfig.Fun.ScaleX = config.FunData.ScaleX;
			CerberusConfig.Fun.ScaleY = config.FunData.ScaleY;
			CerberusConfig.Fun.RainbowSpeed = config.FunData.RainbowSpeed;
			CerberusConfig.Fun.AffectPlayer = config.FunData.AffectPlayer;
			CerberusConfig.Fun.AffectMobs = config.FunData.AffectMobs;
			CerberusConfig.Fun.AffectOthers = config.FunData.AffectOthers;
			CerberusConfig.Texture.Enabled = config.TextureData.Enabled;
			CerberusConfig.Texture.Size = config.TextureData.Size;
			CerberusConfig.Texture.MakeEntitiesInvisible = config.TextureData.MakeEntitiesInvisible;
			CerberusConfig.Settings.UiCustomizable = config.SettingsData.UiCustomizable;
			CerberusConfig.Settings.ShowMenu = config.SettingsData.ShowMenu;
			CerberusConfig.Settings.ShowMenuHotKey = config.SettingsData.ShowMenuHotKey;
			CerberusConfig.Settings.ShowDebugConsole = config.SettingsData.ShowDebugConsole;
			CerberusConfig.Settings.CurrentLanguage = config.SettingsData.CurrentLanguage;
			CerberusConfig.Settings.ClydePatch = config.SettingsData.ClydePatch;
			CerberusConfig.Settings.OverlaysPatch = config.SettingsData.OverlaysPatch;
			CerberusConfig.Settings.SmokePatch = config.SettingsData.SmokePatch;
			CerberusConfig.Settings.AdminPatch = config.SettingsData.AdminPatch;
			CerberusConfig.Settings.DamageForcePatch = config.SettingsData.DamageForcePatch;
			CerberusConfig.Settings.NoDmgFriendPatch = config.SettingsData.NoDmgFriendPatch;
			CerberusConfig.Settings.AntiScreenGrubPatch = config.SettingsData.AntiScreenGrubPatch;
			CerberusConfig.Settings.TranslateChatPatch = config.SettingsData.TranslateChatPatch;
			CerberusConfig.Settings.TranslateChatLang = config.SettingsData.TranslateChatLang;
			CerberusConfig.Settings.TranslateMePatch = config.SettingsData.TranslateMePatch;
			CerberusConfig.Settings.TranslateMeLang = config.SettingsData.TranslateMeLang;
			CerberusConfig.Settings.NoCameraKickPatch = config.SettingsData.NoCameraKickPatch;
			CerberusConfig.Notifications.Enabled = config.NotificationsData.Enabled;
			CerberusConfig.Notifications.MaxNotifications = config.NotificationsData.MaxNotifications;
			CerberusConfig.Notifications.FontSize = config.NotificationsData.FontSize;
			CerberusConfig.Notifications.AnchorPosition = config.NotificationsData.AnchorPosition;
			CerberusConfig.Notifications.IgnoreSizeCheck = config.NotificationsData.IgnoreSizeCheck;
			CerberusConfig.NoSavedConfig.HasTarget = config.NoSavedConfigData.HasTarget;
			CerberusConfig.NoSavedConfig.HasAntiCheat = config.NoSavedConfigData.HasAntiCheat;
			CerberusConfig.NoSavedConfig.Version = config.NoSavedConfigData.Version;
		}
		public static string ConfigName = "";
		public static int SelectedConfigIndex = -1;
		public static List<string> ConfigFiles = new List<string>();
		public static string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CerberusWare", "Config");
		public class Vector4Converter : JsonConverter<Vector4>
		{
			public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.StartArray)
				{
					throw new JsonException();
				}
				reader.Read();
				float single = reader.GetSingle();
				reader.Read();
				float single2 = reader.GetSingle();
				reader.Read();
				float single3 = reader.GetSingle();
				reader.Read();
				float single4 = reader.GetSingle();
				reader.Read();
				return new Vector4(single, single2, single3, single4);
			}
			public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
			{
				writer.WriteStartArray();
				writer.WriteNumberValue(value.X);
				writer.WriteNumberValue(value.Y);
				writer.WriteNumberValue(value.Z);
				writer.WriteNumberValue(value.W);
				writer.WriteEndArray();
			}
		}
	}
}
