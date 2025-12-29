using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Camera;
using HarmonyLib;
[CompilerGenerated]
public class NoRecoilPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(CameraRecoilSystem), "KickCamera", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = NoRecoilPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(NoRecoilPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	private static bool Prefix()
	{
		return !CerberusConfig.Settings.NoCameraKickPatch;
	}
}
