using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.UserInterface.Systems.Chat;
using HarmonyLib;
[CompilerGenerated]
public static class ForceSayPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(ChatUIController), "OnDamageForceSay", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = ForceSayPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(ForceSayPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	private static bool Prefix()
	{
		return !CerberusConfig.Settings.DamageForcePatch;
	}
}
