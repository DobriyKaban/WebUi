using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MinHook;


[CompilerGenerated]
internal sealed class HookManager : IDisposable
{
	public GLImports.WglSwapBuffersFunc OriginalSwapBuffers { get; private set; }
	public void Install()
	{
		bool flag = this._hookEngine != null;
		if (!flag)
		{
			this._hookEngine = new HookEngine();
			IntPtr intPtr = GLImports.wglGetProcAddress("wglSwapBuffers");
			bool flag2 = intPtr == IntPtr.Zero;
			if (flag2)
			{
				IntPtr moduleHandle = GLImports.GetModuleHandle("opengl32.dll");
				bool flag3 = moduleHandle != IntPtr.Zero;
				if (flag3)
				{
					intPtr = GLImports.GetProcAddress(moduleHandle, "wglSwapBuffers");
				}
				bool flag4 = intPtr == IntPtr.Zero;
				if (flag4)
				{
					this._hookEngine.Dispose();
					this._hookEngine = null;
					throw new EntryPointNotFoundException("Could not find wglSwapBuffers entry point via wglGetProcAddress or GetProcAddress(opengl32.dll).");
				}
			}
			GLImports.WglSwapBuffersFunc wglSwapBuffersFunc = new GLImports.WglSwapBuffersFunc(this.WglSwapBuffersDetour);
			this._hookHandle = new GCHandle?(GCHandle.Alloc(wglSwapBuffersFunc));
			try
			{
				this.OriginalSwapBuffers = this._hookEngine.CreateHook<GLImports.WglSwapBuffersFunc>(intPtr, wglSwapBuffersFunc);
				this._hookEngine.EnableHooks();
			}
			catch (Exception)
			{
				this.Uninstall();
				throw;
			}
		}
	}
	public void Uninstall()
	{
		bool flag = this._hookEngine != null;
		if (flag)
		{
			try
			{
				this._hookEngine.DisableHooks();
			}
			catch
			{
			}
			this._hookEngine.Dispose();
			this._hookEngine = null;
			this.OriginalSwapBuffers = null;
		}
		bool flag2 = this._hookHandle != null;
		if (flag2)
		{
			bool isAllocated = this._hookHandle.Value.IsAllocated;
			if (isAllocated)
			{
				this._hookHandle.Value.Free();
			}
			this._hookHandle = null;
		}
	}
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}
	public event GLImports.WglSwapBuffersFunc OnSwapBuffers
	{
		[CompilerGenerated]
		add
		{
			GLImports.WglSwapBuffersFunc wglSwapBuffersFunc = this.SwapBuffersEvent;
			GLImports.WglSwapBuffersFunc wglSwapBuffersFunc2;
			do
			{
				wglSwapBuffersFunc2 = wglSwapBuffersFunc;
				GLImports.WglSwapBuffersFunc wglSwapBuffersFunc3 = (GLImports.WglSwapBuffersFunc)Delegate.Combine(wglSwapBuffersFunc2, value);
				wglSwapBuffersFunc = Interlocked.CompareExchange<GLImports.WglSwapBuffersFunc>(ref this.SwapBuffersEvent, wglSwapBuffersFunc3, wglSwapBuffersFunc2);
			}
			while (wglSwapBuffersFunc != wglSwapBuffersFunc2);
		}
		[CompilerGenerated]
		remove
		{
			GLImports.WglSwapBuffersFunc wglSwapBuffersFunc = this.SwapBuffersEvent;
			GLImports.WglSwapBuffersFunc wglSwapBuffersFunc2;
			do
			{
				wglSwapBuffersFunc2 = wglSwapBuffersFunc;
				GLImports.WglSwapBuffersFunc wglSwapBuffersFunc3 = (GLImports.WglSwapBuffersFunc)Delegate.Remove(wglSwapBuffersFunc2, value);
				wglSwapBuffersFunc = Interlocked.CompareExchange<GLImports.WglSwapBuffersFunc>(ref this.SwapBuffersEvent, wglSwapBuffersFunc3, wglSwapBuffersFunc2);
			}
			while (wglSwapBuffersFunc != wglSwapBuffersFunc2);
		}
	}
	private bool WglSwapBuffersDetour(IntPtr hdc)
	{
		bool flag = this.SwapBuffersEvent != null;
		if (flag)
		{
			try
			{
				return this.SwapBuffersEvent(hdc);
			}
			catch (Exception)
			{
				return false;
			}
		}
		bool flag2 = this.OriginalSwapBuffers != null;
		return flag2 && this.OriginalSwapBuffers(hdc);
	}
	private void Dispose(bool disposing)
	{
		bool flag = !this._disposed;
		if (flag)
		{
			if (disposing)
			{
				this.Uninstall();
			}
			this._disposed = true;
		}
	}
	~HookManager()
	{
		this.Dispose(false);
	}
	private GCHandle? _hookHandle;
	private HookEngine _hookEngine;
	private bool _disposed;
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private GLImports.WglSwapBuffersFunc SwapBuffersEvent;
}
