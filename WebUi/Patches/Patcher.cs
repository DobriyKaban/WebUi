using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;


[CompilerGenerated]
public static class Patcher
{
	public static void PatchAll()
	{
		ReflectionPatch2.Patch();
		ModLoaderPatch.Patch();
		EntitySystemHiderPatch.Patch();
		IoCPatch.Patch();
		ModLoaderPatch.Patch();
		AssemblyHiderPatch.Patch();
		TypeFinderPatch.Patch();
		TypeBlockerPatch.Patch();
		AdminPrivilegePatch.Patch();
		ClydePatch.Patch();
		ForceSayPatch.Patch();
		AntiAimPatch.Patch();
		FriendlyFirePatch.Patch();
		RemoveEffectsPatch.Patch();
		DamageVisualLimitPatch.Patch();
		NoSmokePatch.Patch();
		ChatTranslationPatch.Patch();
		NoRecoilPatch.Patch();
	}
	
	public static void PatchMethod(MethodInfo method, MethodInfo patch, HarmonyPatchType type)
	{
		bool flag = method == null;
		if (!flag)
		{
			bool flag2 = patch == null;
			if (!flag2)
			{
				switch (type)
				{
				case HarmonyPatchType.Prefix:
					Patcher.Prefix(method, patch);
					break;
				case HarmonyPatchType.Postfix:
					Patcher.Postfix(method, patch);
					break;
				case HarmonyPatchType.Transpiler:
					Patcher.Transpiler(method, patch);
					break;
				}
			}
		}
	}
	private static void Prefix(MethodBase method, MethodInfo prefix)
	{
		Patcher._harmony.Patch(method, prefix, null, null, null);
	}
	private static void Postfix(MethodBase method, MethodInfo postfix)
	{
		Patcher._harmony.Patch(method, null, postfix, null, null);
	}
	private static void Transpiler(MethodBase method, MethodInfo transpiler)
	{
		Patcher._harmony.Patch(method, null, null, transpiler, null);
	}
	public static MethodInfo GetMethod(string fqtn, string methodName, Type[] parameters = null)
	{
		Type type = Patcher.TypeFromQualifiedName(fqtn);
		bool flag = type != null;
		MethodInfo methodInfo;
		if (flag)
		{
			methodInfo = Patcher.GetMethod(type, methodName, parameters);
		}
		else
		{
			methodInfo = null;
		}
		return methodInfo;
	}
	
	public static MethodInfo GetMethod(Type type,  string methodName, Type[] parameters = null)
	{
		return AccessTools.Method(type, methodName, parameters, null);
	}
	private static Type TypeFromQualifiedName(string fqtn)
	{
		return AccessTools.TypeByName(fqtn);
	}
	private static readonly Harmony _harmony = new Harmony("CerberusWareV3");
}
