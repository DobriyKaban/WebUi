using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hexa.NET.ImGui;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


[CompilerGenerated]
public static class TextureLoader
{
	public static bool AddImage(string key,  byte[] imageData, TextureLoader.ImageFilterMode filterMode = TextureLoader.ImageFilterMode.Linear)
	{
		bool flag = string.IsNullOrEmpty(key) || imageData == null || imageData.Length == 0;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			ImTextureID imTextureID;
			bool flag3 = TextureLoader._textures.TryGetValue(key, out imTextureID);
			if (flag3)
			{
				uint num = (uint)((UIntPtr)imTextureID.Handle);
				bool flag4 = num > 0U;
				if (flag4)
				{
					try
					{
						GLImports.glDeleteTextures(1, ref num);
					}
					catch
					{
					}
				}
				TextureLoader._textures.Remove(key);
			}
			ImTextureID imTextureID2 = TextureLoader.LoadImageAsTexture(imageData, filterMode);
			bool flag5 = !imTextureID2.Equals(default(ImTextureID));
			if (flag5)
			{
				TextureLoader._textures[key] = imTextureID2;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
		}
		return flag2;
	}
	public static ImTextureID GetImage(string key)
	{
		ImTextureID imTextureID;
		return TextureLoader._textures.TryGetValue(key, out imTextureID) ? imTextureID : default(ImTextureID);
	}
	public static void DisposeAll()
	{
		foreach (ImTextureID imTextureID in TextureLoader._textures.Values)
		{
			uint num = (uint)((UIntPtr)imTextureID.Handle);
			bool flag = num > 0U;
			if (flag)
			{
				try
				{
					GLImports.glDeleteTextures(1, ref num);
				}
				catch
				{
				}
			}
		}
		TextureLoader._textures.Clear();
	}
	private static ImTextureID LoadImageAsTexture(byte[] imageData, TextureLoader.ImageFilterMode filterMode)
	{
		uint num = 0U;
		GCHandle gchandle = default(GCHandle);
		ImTextureID imTextureID;
		try
		{
			using (Image<Rgba32> image = Image.Load<Rgba32>(imageData))
			{
				int width = image.Width;
				int height = image.Height;
				byte[] array = new byte[width * height * 4];
				image.CopyPixelDataTo(array);
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				IntPtr intPtr = gchandle.AddrOfPinnedObject();
				bool flag = intPtr == IntPtr.Zero;
				if (flag)
				{
					imTextureID = default(ImTextureID);
				}
				else
				{
					GLImports.glGenTextures(1, out num);
					bool flag2 = num == 0U;
					if (flag2)
					{
						imTextureID = default(ImTextureID);
					}
					else
					{
						GLImports.glBindTexture(3553U, num);
						int num2;
						int num3;
						if (filterMode != TextureLoader.ImageFilterMode.Linear)
						{
							if (filterMode == TextureLoader.ImageFilterMode.Nearest)
							{
								num2 = 9728;
								num3 = 9728;
								goto IL_0166;
							}
						}
						num2 = 9729;
						num3 = 9729;
						IL_0166:
						GLImports.glTexParameteri(3553U, 10241U, num2);
						GLImports.glTexParameteri(3553U, 10240U, num3);
						GLImports.glTexParameteri(3553U, 10242U, 33071);
						GLImports.glTexParameteri(3553U, 10243U, 33071);
						GLImports.glTexImage2D(3553U, 0, 32856, width, height, 0, 6408U, 5121U, intPtr);
						bool flag3 = GLImports.glGetError() > 0U;
						if (flag3)
						{
							GLImports.glDeleteTextures(1, ref num);
							imTextureID = default(ImTextureID);
						}
						else
						{
							GLImports.glBindTexture(3553U, 0U);
							imTextureID = new ImTextureID((ulong)num);
						}
					}
				}
			}
		}
		catch
		{
			bool flag4 = num == 0U;
			if (flag4)
			{
				imTextureID = default(ImTextureID);
			}
			else
			{
				try
				{
					GLImports.glDeleteTextures(1, ref num);
				}
				catch
				{
				}
				imTextureID = default(ImTextureID);
			}
		}
		finally
		{
			bool isAllocated = gchandle.IsAllocated;
			if (isAllocated)
			{
				gchandle.Free();
			}
		}
		return imTextureID;
	}
	private static readonly Dictionary<string, ImTextureID> _textures = new Dictionary<string, ImTextureID>();
	
	public enum ImageFilterMode
	{
		Linear,
		Nearest
	}
}
