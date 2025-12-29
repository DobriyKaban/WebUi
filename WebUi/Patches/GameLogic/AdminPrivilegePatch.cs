using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using HarmonyLib;

public static class AdminPrivilegePatch
{
    private static IEnumerable<MethodInfo> TargetMethods()
    {
        var adminManagerType = AccessTools.TypeByName("Content.Client.Administration.Managers.ClientAdminManager");
        
        if (adminManagerType != null)
        {
            var hasFlagMethod = AccessTools.Method(adminManagerType, "HasFlag"); 
            if (hasFlagMethod != null)
            {
                yield return hasFlagMethod;
            }
            
            var isActiveProp = AccessTools.PropertyGetter(adminManagerType, "IsActive");
            if (isActiveProp != null)
            {
                yield return isActiveProp;
            }
        }
        
        var sharedAdminType = AccessTools.TypeByName("Content.Shared.Administration.Managers.SharedAdminManager");
        if (sharedAdminType != null)
        {
             var hasFlagMethod = AccessTools.Method(sharedAdminType, "HasFlag");
             if (hasFlagMethod != null)
             {
                 yield return hasFlagMethod;
             }
        }
    }

    public static void Patch()
    {
        IEnumerable<MethodInfo> enumerable = AdminPrivilegePatch.TargetMethods();
        MethodInfo patch = Patcher.GetMethod(typeof(AdminPrivilegePatch), "Postfix", null);
        
        foreach (MethodInfo methodInfo in enumerable)
        {
            if (methodInfo != null)
            {
                Patcher.PatchMethod(methodInfo, patch, HarmonyPatchType.Postfix);
            }
        }
    }

    private static void Postfix(ref bool __result)
    {
        bool enabled = CerberusConfig.Settings.AdminPatch;
        if (enabled)
        {
            __result = true;
        }
    }
}