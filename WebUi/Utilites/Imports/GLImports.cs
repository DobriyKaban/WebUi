using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
[CompilerGenerated]
internal static class GLImports
{
	[DllImport("opengl32.dll")]
	public static extern void glEnable(uint cap);
	[DllImport("opengl32.dll")]
	public static extern void glDisable(uint cap);
	
	[DllImport("opengl32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	internal static extern IntPtr wglGetProcAddress(string procName);
	
	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern IntPtr GetModuleHandle(string lpModuleName);
	
	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	[DllImport("opengl32.dll")]
	public static extern void glGenTextures(int n, out uint textures);
	[DllImport("opengl32.dll")]
	public static extern void glBindTexture(uint target, uint texture);
	[DllImport("opengl32.dll")]
	public static extern void glDeleteTextures(int n, ref uint textures);
	[DllImport("opengl32.dll")]
	public static extern void glTexParameteri(uint target, uint pname, int param);
	[DllImport("opengl32.dll")]
	public unsafe static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, void* data);
	[DllImport("opengl32.dll")]
	public static extern void glTexImage2D(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr data);
	[DllImport("opengl32.dll")]
	public static extern uint glGetError();
	public const uint GL_TEXTURE_2D_BINDING = 36281U;
	public const uint GL_TEXTURE_2D = 3553U;
	public const uint GL_RGBA = 6408U;
	public const uint GL_UNSIGNED_BYTE = 5121U;
	public const uint GL_LINEAR = 9729U;
	public const uint GL_NEAREST = 9728U;
	public const uint GL_CLAMP_TO_EDGE = 33071U;
	public const uint GL_TEXTURE_WRAP_S = 10242U;
	public const uint GL_TEXTURE_WRAP_T = 10243U;
	public const uint GL_TEXTURE_MIN_FILTER = 10241U;
	public const uint GL_TEXTURE_MAG_FILTER = 10240U;
	public const int GL_RGBA8 = 32856;
	[UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
	public delegate bool WglSwapBuffersFunc(IntPtr hdc);
}
