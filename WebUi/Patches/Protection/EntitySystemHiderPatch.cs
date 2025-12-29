using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
using Robust.Shared.GameObjects;
[CompilerGenerated]
public static class EntitySystemHiderPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(EntitySystemManager), "GetEntitySystemTypes", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = EntitySystemHiderPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(EntitySystemHiderPatch), "Postfix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Postfix);
	}
	
	private static void Postfix(ref IEnumerable<Type> __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		List<Type> list = __result.ToList<Type>();
		List<Type> list2 = list.Where((Type t) => t.Namespace != null && GlobalBlacklist.AllowedNamespaces.Any((string ns) => t.Namespace.StartsWith(ns))).ToList<Type>();
		__result = list2;
	}
}
