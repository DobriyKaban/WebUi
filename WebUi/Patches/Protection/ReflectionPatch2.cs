using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
[CompilerGenerated]
public static class ReflectionPatch2
{
	
	private static MethodInfo TargetMethod()
	{
		Type type = AccessTools.TypeByName("Robust.Shared.Reflection.ReflectionManager");
		return AccessTools.PropertyGetter(type, "Assemblies");
	}
	public static void Patch()
	{
		MethodInfo methodInfo = ReflectionPatch2.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(ReflectionPatch2), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	
	private static bool Prefix(object __instance, ref IReadOnlyList<Assembly> __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		FieldInfo fieldInfo = AccessTools.Field(__instance.GetType(), "assemblies");
		List<Assembly> list = ((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) as List<Assembly>;
		bool flag = list == null;
		bool flag2;
		if (flag)
		{
			__result = Array.Empty<Assembly>();
			flag2 = false;
		}
		else
		{
			ReadOnlyCollection<Assembly> readOnlyCollection = list.Where(delegate(Assembly a)
			{
				string name = a.GetName().Name;
				return name != null && !GlobalBlacklist.AllowedNamespaces.Contains(name);
			}).ToList<Assembly>().AsReadOnly();
			__result = readOnlyCollection;
			flag2 = false;
		}
		return flag2;
	}
}
