using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
using Robust.Shared.Reflection;


[CompilerGenerated]
public static class TypeFinderPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(ReflectionManager), "FindAllTypes", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = TypeFinderPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(TypeFinderPatch), "Postfix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Postfix);
	}
	
	private static void Postfix(ref IEnumerable<Type> __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		List<Type> list = __result.ToList<Type>();
		List<Type> list2 = list.Where((Type t) => !GlobalBlacklist.BlockedNames.Any((string blockedName) => t.Name.Contains(blockedName, StringComparison.OrdinalIgnoreCase))).ToList<Type>();
		__result = list2;
	}
}
