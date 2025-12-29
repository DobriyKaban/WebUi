using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using CerberusWareV3.MyImGui;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.OpenGL3;
using Hexa.NET.ImGui.Backends.Win32;
using Robust.Shared.IoC;
using Robust.Shared.Log;


[CompilerGenerated]
public sealed class RenderManager : IDisposable
{
	public static RenderManager Instance { get; } = new RenderManager();
	
	public event RenderManager.EventHandler RenderEvent
	{
		
		[CompilerGenerated]
		add
		{
			RenderManager.EventHandler eventHandler = this.OnRender;
			RenderManager.EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				RenderManager.EventHandler eventHandler3 = (RenderManager.EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange<RenderManager.EventHandler>(ref this.OnRender, eventHandler3, eventHandler2);
			}
			while (eventHandler != eventHandler2);
		}
		
		[CompilerGenerated]
		remove
		{
			RenderManager.EventHandler eventHandler = this.OnRender;
			RenderManager.EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				RenderManager.EventHandler eventHandler3 = (RenderManager.EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange<RenderManager.EventHandler>(ref this.OnRender, eventHandler3, eventHandler2);
			}
			while (eventHandler != eventHandler2);
		}
	}
	public RenderManager()
	{
		IoCManager.InjectDependencies<RenderManager>(this);
		this._libraryLoader = new NativeLibraryLoader();
	}
	public void RegisterRender(IOverlay overlay)
	{
		ArgumentNullException.ThrowIfNull(overlay, "overlay");
		using (this._lock.EnterScope())
		{
			bool flag = this._overlays.Contains(overlay);
			if (!flag)
			{
				this._overlays.Add(overlay);
			}
		}
	}
	public void UnregisterRender(IOverlay overlay)
	{
		ArgumentNullException.ThrowIfNull(overlay, "overlay");
		using (this._lock.EnterScope())
		{
			this._overlays.Remove(overlay);
		}
	}
	public bool Initialize()
	{
		bool flag = !this.LoadNativeLibraries();
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			try
			{
				this._hookManager = new HookManager();
				this._hookManager.OnSwapBuffers += this.SwapBuffersHooked;
				this._hookManager.Install();
			}
			catch (Exception ex)
			{
				this._libraryLoader.Dispose();
				return false;
			}
			flag2 = true;
		}
		return flag2;
	}
	public void Dispose()
	{
		using (this._lock.EnterScope())
		{
			this._overlays.Clear();
		}
		this.RestoreWndProc();
		bool flag = this._hookManager != null;
		if (flag)
		{
			this._hookManager.OnSwapBuffers -= this.SwapBuffersHooked;
			this._hookManager.Uninstall();
			this._hookManager = null;
		}
		bool initFlag = this._initialized;
		if (initFlag)
		{
			bool flag2 = this._imGuiContext != ImGuiContextPtr.Null;
			if (flag2)
			{
				ImGui.SetCurrentContext(this._imGuiContext);
				ImGuiImplOpenGL3.Shutdown();
				ImGuiImplWin32.Shutdown();
			}
		}
		bool flag3 = this._imGuiContext != ImGuiContextPtr.Null;
		if (flag3)
		{
			ImGui.DestroyContext(this._imGuiContext);
			this._imGuiContext = default(ImGuiContextPtr);
		}
		this._libraryLoader.Dispose();
		this._initialized = false;
	}
	private bool SwapBuffersHooked(IntPtr hdc)
	{
		bool flag = !this._initialized;
		bool? flag4;
		if (flag)
		{
			bool flag2 = !this.InitializeHook(hdc);
			if (flag2)
			{
				HookManager hookMgr = this._hookManager;
				bool? flag3;
				if (hookMgr == null)
				{
					flag3 = null;
				}
				else
				{
					GLImports.WglSwapBuffersFunc original = hookMgr.OriginalSwapBuffers;
					flag3 = ((original != null) ? new bool?(original(hdc)) : null);
				}
				flag4 = flag3;
				return flag4.GetValueOrDefault(true);
			}
			this._initialized = true;
		}
		try
		{
			ImGuiImplOpenGL3.NewFrame();
			ImGuiImplWin32.NewFrame();
			ImGui.NewFrame();
			List<IOverlay> list;
			using (this._lock.EnterScope())
			{
				list = this._overlays.ToList<IOverlay>();
			}
			foreach (IOverlay overlay in list)
			{
				try
				{
					overlay.Render();
				}
				catch (Exception ex)
				{
					this.UnregisterRender(overlay);
				}
			}
			ImGui.Render();
			GLImports.glDisable(36281U);
			ImGuiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());
			GLImports.glEnable(36281U);
		}
		catch (Exception ex2)
		{
		}
		HookManager hookMgr2 = this._hookManager;
		bool? flag5;
		if (hookMgr2 == null)
		{
			flag5 = null;
		}
		else
		{
			GLImports.WglSwapBuffersFunc original2 = hookMgr2.OriginalSwapBuffers;
			flag5 = ((original2 != null) ? new bool?(original2(hdc)) : null);
		}
		flag4 = flag5;
		return flag4.GetValueOrDefault(true);
	}
	private bool InitializeHook(IntPtr hdc)
	{
		bool flag2;
		try
		{
			this._imGuiContext = ImGui.CreateContext();
			bool flag = this._imGuiContext == ImGuiContextPtr.Null;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this._windowHandle = Win32Imports.WindowFromDC(hdc);
				bool flag3 = this._windowHandle == IntPtr.Zero;
				if (flag3)
				{
					ImGui.DestroyContext(this._imGuiContext);
					this._imGuiContext = default(ImGuiContextPtr);
					flag2 = false;
				}
				else
				{
					bool flag4 = !this.SubclassWndProc();
					if (flag4)
					{
						ImGui.DestroyContext(this._imGuiContext);
						this._imGuiContext = default(ImGuiContextPtr);
						flag2 = false;
					}
					else
					{
						ImGui.SetCurrentContext(this._imGuiContext);
						RenderManager.EventHandler renderHandler = this.OnRender;
						if (renderHandler != null)
						{
							renderHandler();
						}
						ImGuiImplWin32.SetCurrentContext(this._imGuiContext);
						bool flag5 = !ImGuiImplWin32.InitForOpenGL(this._windowHandle);
						if (flag5)
						{
							this.RestoreWndProc();
							ImGui.DestroyContext(this._imGuiContext);
							this._imGuiContext = default(ImGuiContextPtr);
							flag2 = false;
						}
						else
						{
							ImGuiImplOpenGL3.SetCurrentContext(this._imGuiContext);
							bool flag6 = !ImGuiImplOpenGL3.Init("#version 330 core");
							if (flag6)
							{
								this.RestoreWndProc();
								ImGuiImplWin32.Shutdown();
								ImGui.DestroyContext(this._imGuiContext);
								this._imGuiContext = default(ImGuiContextPtr);
								flag2 = false;
							}
							else
							{
								flag2 = true;
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			this.RestoreWndProc();
			bool flag7 = this._imGuiContext != ImGuiContextPtr.Null;
			if (flag7)
			{
				ImGui.SetCurrentContext(this._imGuiContext);
				ImGuiImplOpenGL3.Shutdown();
				ImGuiImplWin32.Shutdown();
				ImGui.DestroyContext(this._imGuiContext);
				this._imGuiContext = default(ImGuiContextPtr);
			}
			flag2 = false;
		}
		return flag2;
	}
	private bool SubclassWndProc()
	{
		bool flag = this._windowHandle == IntPtr.Zero;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = this._originalWndProc != IntPtr.Zero;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				this._wndProcDelegate = new RenderManager.WndProcDelegate(this.NewWndProc);
				IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<RenderManager.WndProcDelegate>(this._wndProcDelegate);
				this._originalWndProc = Win32Imports.SetWindowLongPtr(this._windowHandle, -4, functionPointerForDelegate);
				bool flag4 = this._originalWndProc == IntPtr.Zero;
				if (flag4)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					this._wndProcDelegate = null;
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
			}
		}
		return flag2;
	}
	private void RestoreWndProc()
	{
		bool flag = this._windowHandle != IntPtr.Zero && this._originalWndProc != IntPtr.Zero;
		if (flag)
		{
			IntPtr windowLongPtr = Win32Imports.GetWindowLongPtr(this._windowHandle, -4);
			IntPtr intPtr = ((this._wndProcDelegate != null) ? Marshal.GetFunctionPointerForDelegate<RenderManager.WndProcDelegate>(this._wndProcDelegate) : IntPtr.Zero);
			bool flag2 = windowLongPtr == intPtr;
			if (flag2)
			{
				Win32Imports.SetWindowLongPtr(this._windowHandle, -4, this._originalWndProc);
			}
			this._originalWndProc = IntPtr.Zero;
			this._wndProcDelegate = null;
		}
	}
	private unsafe IntPtr NewWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		IntPtr intPtr = ImGuiImplWin32.WndProcHandler(hWnd, msg, (UIntPtr)wParam, lParam);
		ImGuiIOPtr io = ImGui.GetIO();
		bool flag = io.WantCaptureMouse || io.WantCaptureKeyboard;
		bool flag2 = !flag;
		if (flag2)
		{
			flag = ImGui.IsAnyItemActive() || ImGui.IsAnyItemFocused();
		}
		bool flag3 = flag;
		if (flag3)
		{
			if (msg - 256U <= 6U || msg - 512U <= 10U)
			{
				return IntPtr.Zero;
			}
		}
		bool flag4 = intPtr != IntPtr.Zero;
		IntPtr intPtr2;
		if (flag4)
		{
			intPtr2 = IntPtr.Zero;
		}
		else
		{
			intPtr2 = ((this._originalWndProc != IntPtr.Zero) ? Win32Imports.CallWindowProc(this._originalWndProc, hWnd, msg, wParam, lParam) : Win32Imports.DefWindowProc(hWnd, msg, wParam, lParam));
		}
		return intPtr2;
	}
	private bool LoadNativeLibraries()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		this._cimguiHandle = this._libraryLoader.LoadLibraryFromResource("cimgui.dll", executingAssembly);
		bool flag = this._cimguiHandle == IntPtr.Zero;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			this._imguiImplHandle = this._libraryLoader.LoadLibraryFromResource("ImGuiImpl.dll", executingAssembly);
			bool flag3 = this._imguiImplHandle == IntPtr.Zero;
			if (flag3)
			{
				this._libraryLoader.Dispose();
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
		}
		return flag2;
	}
	private IntPtr _originalWndProc = IntPtr.Zero;
	
	private RenderManager.WndProcDelegate _wndProcDelegate;
	private readonly NativeLibraryLoader _libraryLoader;
	private readonly List<IOverlay> _overlays = new List<IOverlay>();
	private readonly Lock _lock = new Lock();
	
	private HookManager _hookManager;
	private ImGuiContextPtr _imGuiContext;
	private bool _initialized;
	private IntPtr _windowHandle = IntPtr.Zero;
	
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private RenderManager.EventHandler OnRender;
	private IntPtr _cimguiHandle = IntPtr.Zero;
	private IntPtr _imguiImplHandle = IntPtr.Zero;
	private ISawmill _logger = Logger.GetSawmill("imgui-manager");
	
	private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
	
	public delegate bool EventHandler();
}
