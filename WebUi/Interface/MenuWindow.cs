using System;
using System.Drawing;
using System.Windows.Forms; 
using Microsoft.Web.WebView2.Core;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using WebUi; 

public class MenuWindow : Form
{
    private CoreWebView2Controller _controller;
    private CoreWebView2 _coreWebView2;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();
    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    public MenuWindow()
    {
        try
        {
            this.Text = "Cerberus V3";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true; 
            this.ShowInTaskbar = false;
            
            // Убираем рамку полностью
            this.FormBorderStyle = FormBorderStyle.None;
            // Делаем фон темным (чтобы при загрузке не было белой вспышки)
            this.BackColor = Color.FromArgb(20, 20, 20);

            this.Resize += MenuWindow_Resize;
            this.Load += async (s, e) => await InitializeAsync();
        }
        catch (Exception ex)
        {
            Log.Error($"MenuWindow ctor error: {ex}");
        }
    }

    private async Task InitializeAsync()
    {
        try
        {
            Log.Info("Initializing WebView2...");
            var env = await CoreWebView2Environment.CreateAsync(null, System.IO.Path.GetTempPath());
            
            _controller = await env.CreateCoreWebView2ControllerAsync(this.Handle);
            _coreWebView2 = _controller.CoreWebView2;

            _controller.DefaultBackgroundColor = Color.Transparent; 
            _coreWebView2.Settings.IsZoomControlEnabled = false; 
            _coreWebView2.Settings.AreDefaultContextMenusEnabled = false; 

            _controller.Bounds = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            _controller.IsVisible = true;

            _coreWebView2.WebMessageReceived += OnWebMessageReceived;

            Log.Info("Navigating to local server...");
            _coreWebView2.Navigate("http://localhost:4649/");
        }
        catch (Exception ex)
        {
            Log.Error($"WebView2 Init Failed: {ex}");
            MessageBox.Show($"WebView2 Error: {ex.Message}\nCheck WebView2Loader.dll!");
        }
    }

    private void OnWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            string message = e.TryGetWebMessageAsString();
            
            if (message == "drag_window")
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
            else if (message == "close_window")
            {
                // По нажатию крестика в HTML просто скрываем окно
                this.Hide(); 
                if (_controller != null) _controller.IsVisible = false;
            }
        }
        catch { }
    }

    private void MenuWindow_Resize(object sender, EventArgs e)
    {
        if (_controller != null)
        {
            _controller.Bounds = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
            if (_controller != null) _controller.IsVisible = false;
        }
        else
        {
            _controller?.Close();
            _controller = null;
            base.OnFormClosing(e);
        }
    }
    
    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (_controller != null) _controller.IsVisible = this.Visible;
    }

    protected override void SetVisibleCore(bool value)
    {
        if (!this.IsHandleCreated)
        {
            CreateHandle();
            value = false; 
        }
        base.SetVisibleCore(value);
    }
}