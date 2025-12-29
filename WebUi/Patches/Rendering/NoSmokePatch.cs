using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Chemistry.Visualizers;
using HarmonyLib;
using Robust.Client.GameObjects;
[CompilerGenerated]
public static class NoSmokePatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(SmokeVisualizerSystem), "OnAppearanceChange", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = NoSmokePatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(NoSmokePatch), "Postfix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Postfix);
	}
	public static void Postfix(ref AppearanceChangeEvent args)
	{
		bool flag = !CerberusConfig.Settings.SmokePatch;
		if (!flag)
		{
			bool flag2 = args.Sprite == null;
			if (!flag2)
			{
				args.Sprite.Color = args.Sprite.Color.WithAlpha(0.2f);
				args.Sprite.DrawDepth = 1;
			}
		}
	}
}
