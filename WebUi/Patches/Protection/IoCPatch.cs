using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
[CompilerGenerated]
public static class IoCPatch
{
	
	private static MethodInfo TargetMethod()
	{
		Type type = AccessTools.TypeByName("Robust.Shared.IoC.DependencyCollection");
		return (type == null) ? null : AccessTools.Method(type, "GetRegisteredTypes", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = IoCPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(IoCPatch), "Postfix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Postfix);
	}
	
	private static void Postfix(ref IEnumerable<Type> __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		List<Type> list = __result.ToList<Type>();
		List<Type> list2 = list.Where((Type t) => t.Namespace != null && !GlobalBlacklist.AllowedNamespaces.Any((string ns) => t.Namespace.StartsWith(ns))).ToList<Type>();
		__result = list2;
	}
}
