using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Utilities;


[CompilerGenerated]
public static class FontManager
{
	public static void AddFont(string key, byte[] font, float size = 12f)
	{
		bool flag = !FontManager._fonts.ContainsKey(key);
		if (flag)
		{
			FontManager._fonts.Add(key, FontManager.LoadFontFromBytes(font, size));
		}
	}
	public static ImFontPtr GetFont(string key)
	{
		ImFontPtr imFontPtr;
		return FontManager._fonts.TryGetValue(key, out imFontPtr) ? imFontPtr : ImFontPtr.Null;
	}
	private unsafe static ImFontPtr LoadFontFromBytes(byte[] font, float size)
	{
		ImFontPtr imFontPtr = ImFontPtr.Null;
		try
		{
			ImGuiFontBuilder imGuiFontBuilder = new ImGuiFontBuilder();
			FontBlob fontBlob = new FontBlob(font);
			uint* glyphRangesCyrillic = ImGui.GetIO().Fonts.GetGlyphRangesCyrillic();
			imGuiFontBuilder.AddFontFromMemoryTTF(fontBlob.Data, fontBlob.Length, size, glyphRangesCyrillic);
			imFontPtr = imGuiFontBuilder.Build();
		}
		catch
		{
		}
		return imFontPtr;
	}
	private static readonly Dictionary<string, ImFontPtr> _fonts = new Dictionary<string, ImFontPtr>();
}
