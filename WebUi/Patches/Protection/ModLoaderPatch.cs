using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
[CompilerGenerated]
public static class ModLoaderPatch
{
	
	private static MethodInfo TargetMethod()
	{
		Type type = AccessTools.TypeByName("Robust.Shared.ContentPack.BaseModLoader");
		return AccessTools.PropertyGetter(type, "LoadedModules");
	}
	public static void Patch()
	{
		MethodInfo methodInfo = ModLoaderPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(ModLoaderPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	
	private static bool Prefix(object __instance, ref IEnumerable<Assembly> __result)
	{
		FieldInfo fieldInfo = AccessTools.Field(__instance.GetType(), "Mods");
		IEnumerable<object> enumerable = fieldInfo.GetValue(__instance) as IEnumerable<object>;
		bool flag = enumerable == null;
		bool flag2;
		if (flag)
		{
			__result = Array.Empty<Assembly>();
			flag2 = false;
		}
		else
		{
			List<object> list = enumerable.ToList<object>();
			bool flag3 = list.Count == 0;
			if (flag3)
			{
				__result = Array.Empty<Assembly>();
				flag2 = false;
			}
			else
			{
				PropertyInfo gameAssemblyProp = AccessTools.Property(list[0].GetType(), "GameAssembly");
				__result = (from m in list
					select gameAssemblyProp.GetValue(m) as Assembly into asm
					where asm != null && GlobalBlacklist.AllowedNamespaces.Contains(asm.GetName().Name)
					select asm).Cast<Assembly>();
				flag2 = false;
			}
		}
		return flag2;
	}
}
