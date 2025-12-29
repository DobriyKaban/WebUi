using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
using Robust.Shared.Reflection;
[CompilerGenerated]
public static class AssemblyHiderPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.PropertyGetter(typeof(ReflectionManager), "Assemblies");
	}
	public static void Patch()
	{
		MethodInfo methodInfo = AssemblyHiderPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(AssemblyHiderPatch), "Postfix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Postfix);
	}
	
	private static void Postfix(ref IReadOnlyList<Assembly> __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		ReadOnlyCollection<Assembly> readOnlyCollection = __result.Where((Assembly a) => a.GetName().Name != null && GlobalBlacklist.AllowedNamespaces.Contains(a.GetName().Name)).ToList<Assembly>().AsReadOnly();
		__result = readOnlyCollection;
	}
}
