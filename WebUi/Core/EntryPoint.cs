using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC; // Важно добавить!
using Robust.Shared.GameObjects; // Важно добавить!
using WebUi; 

[CompilerGenerated]
public class EntryPoint : GameClient
{
    private WebUiServer _webServer;
    private Thread _guiThread;
    private MenuWindow _menuWindow;
    private bool _isRunning = true;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    private const int VK_INSERT = 0x2D;

    public override void PreInit()
    {
        // Log.Info("[Cerberus] PreInit..."); // Если Log статический, ок. Если нет, используйте IoC.
        Patcher.PatchAll();
    }

    public override void Init()
    {
        // Log.Info("[Cerberus] Init...");
        
        // 1. Инициализация RenderManager
        try 
        {
            RenderManager.Instance.Initialize();
            RenderManager.Instance.RenderEvent += this.LoadFontAtlas; 
        }
        catch (Exception e)
        {
            // Log.Error($"RenderManager init error: {e}");
        }
        
        // 2. Запуск веб-сервера
        try 
        {
            // Создаем сервер
            _webServer = new WebUiServer("http://localhost:4649/"); 
            
            // Пытаемся получить зависимости. Если не выйдет - не страшно, сервер все равно запустится.
            try 
            {
                var sysMan = IoCManager.Resolve<IEntitySystemManager>();
                if (sysMan.TryGetEntitySystem<PlayerTrackerSystem>(out var tracker))
                {
                    _webServer.SetPlayerTracker(tracker);
                }
            }
            catch 
            {
                // Игнорируем ошибки IoC на этапе Init, если они возникают.
                // PlayerTrackerSystem можно будет подцепить позже, если нужно, 
                // но обычно к моменту Init клиента системы уже созданы.
            }

            _webServer.Start();
            // Log.Info("WebUI Server started.");
        }
        catch (Exception e)
        {
            // Log.Error($"WebUI Server failed: {e}");
        }

        // 3. Запуск GUI (в отдельном потоке, это важно для Windows Forms)
        _guiThread = new Thread(GuiThreadLoop);
        _guiThread.SetApartmentState(ApartmentState.STA);
        _guiThread.IsBackground = true; 
        _guiThread.Start();

        // 4. Input Loop
        Task.Run(InputLoop);
    }

    private void GuiThreadLoop()
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _menuWindow = new MenuWindow();
            Application.Run(_menuWindow); 
        }
        catch (Exception ex)
        {
            // Log.Error($"GUI Thread Crashed: {ex}");
        }
    }

    private async Task InputLoop()
    {
        while (_isRunning)
        {
            try 
            {
                if ((GetAsyncKeyState(VK_INSERT) & 0x8000) != 0)
                {
                    if (_menuWindow != null && !_menuWindow.IsDisposed && _menuWindow.IsHandleCreated)
                    {
                        _menuWindow.BeginInvoke(new Action(() => 
                        {
                            if (_menuWindow.Visible)
                                _menuWindow.Hide();
                            else
                            {
                                _menuWindow.Show();
                                _menuWindow.Activate();
                            }
                        }));
                    }
                    await Task.Delay(300); 
                }
            }
            catch { }
            await Task.Delay(10);
        }
    }
    
    public override void Shutdown()
    {
        _isRunning = false;
        RenderManager.Instance.RenderEvent -= this.LoadFontAtlas;
        RenderManager.Instance.Dispose();
        _webServer?.Dispose();
        
        if (_menuWindow != null && !_menuWindow.IsDisposed && _menuWindow.IsHandleCreated)
        {
            try
            {
                _menuWindow.BeginInvoke(new Action(() => 
                {
                    _menuWindow.Dispose();
                    Application.Exit();
                }));
            }
            catch { }
        }
    }

    // ... (LoadFontAtlas) ...
    private bool LoadFontAtlas()
    {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string text = "WebUi.Resources.Font1.Font.ttf";
        bool flag2;
        using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text))
        {
            bool flag = manifestResourceStream == null;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                byte[] array;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    manifestResourceStream.CopyTo(memoryStream);
                    array = memoryStream.ToArray();
                }
                FontManager.AddFont("global-micro", array, 12f);
                FontManager.AddFont("global-small", array, 24f);
                FontManager.AddFont("global", array, 32f);
                FontManager.AddFont("global-large", array, 48f);
                flag2 = true;
            }
        }
        return flag2;
    }
}