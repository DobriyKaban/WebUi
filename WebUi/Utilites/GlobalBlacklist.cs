using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


[CompilerGenerated]
public static class GlobalBlacklist
{
	public static readonly HashSet<string> AllowedNamespaces = new HashSet<string> { "Content.Client", "Content.Shared", "Content.Server", "Robust.Client", "Robust.Shared", "Robust.Server" };
	public static readonly HashSet<string> BlockedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"MarseyPatch", "SubverterPatch", "0Harmony", "HarmonyLib", "Harmony", "Marsey", "Ware", "Cerberus", "Sedition", "Cheat",
		"CerberusWare"
	};
}
