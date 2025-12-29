using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.UserInterface.Systems.DamageOverlays.Overlays;
using HarmonyLib;


[CompilerGenerated]
public class DamageVisualLimitPatch
{
	
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(DamageOverlay), "Draw", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = DamageVisualLimitPatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(DamageVisualLimitPatch), "Prefix", null);
		MethodInfo method2 = Patcher.GetMethod(typeof(DamageVisualLimitPatch), "Transpiler", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
		Patcher.PatchMethod(methodInfo, method2, HarmonyPatchType.Transpiler);
	}
	private static void Prefix(DamageOverlay __instance)
	{
		bool flag = !CerberusConfig.Settings.OverlaysPatch;
		if (!flag)
		{
			DamageVisualLimitPatch.LimitFieldValue(__instance, "BruteLevel", 0.2f);
			DamageVisualLimitPatch.LimitFieldValue(__instance, "OxygenLevel", 0.2f);
			DamageVisualLimitPatch.LimitFieldValue(__instance, "CritLevel", 0.2f);
		}
	}
	private static void LimitFieldValue(DamageOverlay instance, string fieldName, float maxValue)
	{
		FieldInfo fieldInfo = AccessTools.Field(typeof(DamageOverlay), fieldName);
		bool flag = fieldInfo == null;
		if (!flag)
		{
			try
			{
				object value = fieldInfo.GetValue(instance);
				bool flag2 = value == null;
				if (!flag2)
				{
					float num = (float)value;
					float num2 = Math.Min(num, maxValue);
					fieldInfo.SetValue(instance, num2);
				}
			}
			catch
			{
			}
		}
	}
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		bool flag = !CerberusConfig.Settings.OverlaysPatch;
		IEnumerable<CodeInstruction> enumerable;
		if (flag)
		{
			enumerable = instructions;
		}
		else
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			int i = 0;
			while (i < list.Count - 3)
			{
				if (!(list[i].opcode == OpCodes.Ldfld))
				{
					goto IL_018E;
				}
				FieldInfo fieldInfo = list[i].operand as FieldInfo;
				if (fieldInfo == null || !(fieldInfo.Name == "State") || !(list[i + 1].opcode == OpCodes.Ldc_I4_2) || (!(list[i + 2].opcode == OpCodes.Bne_Un) && !(list[i + 2].opcode == OpCodes.Bne_Un_S)) || !(list[i + 3].opcode == OpCodes.Ldc_R4))
				{
					goto IL_018E;
				}
				object operand = list[i + 3].operand;
				if (!(operand is float))
				{
					goto IL_018E;
				}
				float num = (float)operand;
				bool flag2 = Math.Abs(num - 1f) < 0.0001f;
				IL_0193:
				bool flag3 = flag2;
				if (flag3)
				{
					list[i + 3].operand = 0f;
				}
				i++;
				continue;
				IL_018E:
				flag2 = false;
				goto IL_0193;
			}
			enumerable = list;
		}
		return enumerable;
	}
}
