using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[CompilerGenerated]
internal static class Win32Imports
{
	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr WindowFromDC(IntPtr hdc);
	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
	private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	[DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
	private static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
	{
		return (IntPtr.Size == 8) ? Win32Imports.SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : Win32Imports.SetWindowLong32(hWnd, nIndex, dwNewLong);
	}
	[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
	public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
	[DllImport("user32.dll")]
	public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
	[DllImport("user32.dll")]
	public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
	
	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern IntPtr LoadLibrary(string lpFileName);
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool FreeLibrary(IntPtr hModule);
	public const int GWLP_WNDPROC = -4;
}
