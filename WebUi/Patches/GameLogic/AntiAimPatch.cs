using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.MouseRotator;
using HarmonyLib;
[CompilerGenerated]
public class AntiAimPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(MouseRotatorSystem), "Update", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = AntiAimPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(AntiAimPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	public static bool Prefix()
	{
		bool antiAimEnabled = CerberusConfig.Misc.AntiAimEnabled;
		return !antiAimEnabled && (!CerberusConfig.MeleeHelper.RotateToTarget || !CerberusConfig.NoSavedConfig.HasTarget);
	}
}
