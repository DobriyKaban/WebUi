using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
[CompilerGenerated]
public static class ClydePatch
{
	
	private static MethodInfo TargetMethod()
	{
		Type type = AccessTools.TypeByName("Robust.Client.Graphics.Clyde.Clyde");
		return (type != null) ? AccessTools.Method(type, "DrawOcclusionDepth", null, null) : null;
	}
	public static void Patch()
	{
		MethodInfo methodInfo = ClydePatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(ClydePatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	private static bool Prefix()
	{
		return !CerberusConfig.Settings.ClydePatch;
	}
}
