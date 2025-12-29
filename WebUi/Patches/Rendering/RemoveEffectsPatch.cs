using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
[CompilerGenerated]
public static class RemoveEffectsPatch
{
	private static MethodInfo GetOverlayDraw(string typeName)
	{
		Type type = AccessTools.TypeByName(typeName);
		return (type != null) ? AccessTools.Method(type, "Draw", null, null) : null;
	}
	private static IEnumerable<MethodInfo> TargetMethods()
	{
		List<MethodInfo> list = new List<MethodInfo>
		{
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Drunk.DrunkOverlay"),
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Drugs.RainbowOverlay"),
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Eye.Blinding.BlurryVisionOverlay"),
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Eye.Blinding.BlindOverlay"),
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Flash.FlashOverlay"),
			RemoveEffectsPatch.GetOverlayDraw("Content.Client.Silicons.StationAi.StationAiOverlay")
		};
		return from m in list
			where m != null
			select (m);
	}
	public static void Patch()
	{
		IEnumerable<MethodInfo> enumerable = RemoveEffectsPatch.TargetMethods();
		MethodInfo method = Patcher.GetMethod(typeof(RemoveEffectsPatch), "Prefix", null);
		foreach (MethodInfo methodInfo in enumerable)
		{
			Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
		}
	}
	private static bool Prefix()
	{
		return !CerberusConfig.Settings.OverlaysPatch;
	}
}
