using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;


[CompilerGenerated]
public sealed class NativeLibraryLoader : IDisposable
{
	public NativeLibraryLoader()
	{
		try
		{
			this._tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(this._tempPath);
		}
		catch (Exception)
		{
			this._tempPath = null;
		}
	}
	public void Dispose()
	{
		using (this._lock.EnterScope())
		{
			foreach (KeyValuePair<string, ValueTuple<string, IntPtr>> keyValuePair in this._loadedLibraries.Reverse<KeyValuePair<string, ValueTuple<string, IntPtr>>>())
			{
				string key = keyValuePair.Key;
				ValueTuple<string, IntPtr> value = keyValuePair.Value;
				string item = value.Item1;
				IntPtr item2 = value.Item2;
				bool flag = !Win32Imports.FreeLibrary(item2);
				if (flag)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
				}
				this.TryDeleteFile(item);
			}
			this._loadedLibraries.Clear();
		}
		bool flag2 = string.IsNullOrEmpty(this._tempPath) || !Directory.Exists(this._tempPath);
		if (!flag2)
		{
			try
			{
				Directory.Delete(this._tempPath, true);
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			catch (Exception)
			{
			}
			finally
			{
				this._tempPath = null;
			}
		}
	}
	public IntPtr LoadLibraryFromResource(string libraryFileName, Assembly callingAssembly)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(libraryFileName, "libraryFileName");
		ArgumentNullException.ThrowIfNull(callingAssembly, "callingAssembly");
		bool flag = string.IsNullOrEmpty(this._tempPath);
		IntPtr intPtr;
		if (flag)
		{
			intPtr = IntPtr.Zero;
		}
		else
		{
			using (this._lock.EnterScope())
			{
				ValueTuple<string, IntPtr> valueTuple;
				bool flag2 = this._loadedLibraries.TryGetValue(libraryFileName, out valueTuple);
				if (flag2)
				{
					intPtr = valueTuple.Item2;
				}
				else
				{
					string text = callingAssembly.GetManifestResourceNames().FirstOrDefault((string name) => name.EndsWith(libraryFileName, StringComparison.OrdinalIgnoreCase));
					bool flag3 = string.IsNullOrEmpty(text);
					if (flag3)
					{
						intPtr = IntPtr.Zero;
					}
					else
					{
						string text2 = Path.Combine(this._tempPath, libraryFileName);
						try
						{
							using (Stream manifestResourceStream = callingAssembly.GetManifestResourceStream(text))
							{
								bool flag4 = manifestResourceStream == null;
								if (flag4)
								{
									return IntPtr.Zero;
								}
								using (FileStream fileStream = new FileStream(text2, FileMode.Create, FileAccess.Write, FileShare.None))
								{
									manifestResourceStream.CopyTo(fileStream);
								}
							}
						}
						catch (Exception)
						{
							this.TryDeleteFile(text2);
							return IntPtr.Zero;
						}
						IntPtr intPtr2 = Win32Imports.LoadLibrary(text2);
						bool flag5 = intPtr2 == IntPtr.Zero;
						if (flag5)
						{
							int lastWin32Error = Marshal.GetLastWin32Error();
							this.TryDeleteFile(text2);
							intPtr = IntPtr.Zero;
						}
						else
						{
							this._loadedLibraries[libraryFileName] = new ValueTuple<string, IntPtr>(text2, intPtr2);
							intPtr = intPtr2;
						}
					}
				}
			}
		}
		return intPtr;
	}
	private void TryDeleteFile(string filePath)
	{
		bool flag = !File.Exists(filePath);
		if (!flag)
		{
			try
			{
				File.Delete(filePath);
			}
			catch (Exception)
			{
			}
		}
	}
	private readonly Dictionary<string, ValueTuple<string, IntPtr>> _loadedLibraries = new Dictionary<string, ValueTuple<string, IntPtr>>();
	private readonly Lock _lock = new Lock();
	
	private string _tempPath;
}
