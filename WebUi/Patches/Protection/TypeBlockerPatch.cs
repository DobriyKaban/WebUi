using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;
[CompilerGenerated]
public class TypeBlockerPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(Type), "GetType", new Type[] { typeof(string) }, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = TypeBlockerPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(TypeBlockerPatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	
	private static bool Prefix(ref string typeName,  ref Type __result)
	{
		CerberusConfig.NoSavedConfig.HasAntiCheat = true;
		string typeNameCheck = typeName;
		bool flag = !string.IsNullOrEmpty(typeName) && GlobalBlacklist.BlockedNames.Any((string blockedName) => typeNameCheck.Contains(blockedName, StringComparison.OrdinalIgnoreCase));
		bool flag2;
		if (flag)
		{
			__result = null;
			flag2 = false;
		}
		else
		{
			flag2 = true;
		}
		return flag2;
	}
}
