using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Weapons.Melee;
using HarmonyLib;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;


[CompilerGenerated]
public static class FriendlyFirePatch
{
	private static MethodInfo TargetMethod()
	{
		return AccessTools.Method(typeof(SharedMeleeWeaponSystem), "ArcRayCast", null, null);
	}
	public static void Patch()
	{
		MethodInfo methodInfo = FriendlyFirePatch.TargetMethod();
		MethodInfo method = Patcher.GetMethod(typeof(FriendlyFirePatch), "Prefix", null);
		Patcher.PatchMethod(methodInfo, method, HarmonyPatchType.Prefix);
	}
	
	public static bool Prefix(SharedMeleeWeaponSystem __instance, Vector2 position, Angle angle, Angle arcWidth, float range, MapId mapId, EntityUid ignore, ref HashSet<EntityUid> __result)
	{
		bool flag = !CerberusConfig.Settings.NoDmgFriendPatch;
		bool flag2;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			if (FriendlyFirePatch._friendSystem == null)
			{
				FriendlyFirePatch._friendSystem = IoCManager.Resolve<IEntityManager>().System<FriendSystem>();
			}
			if (FriendlyFirePatch._physicsSystem == null)
			{
				FriendlyFirePatch._physicsSystem = IoCManager.Resolve<IEntityManager>().System<SharedPhysicsSystem>();
			}
			int num = 1 + 35 * (int)Math.Ceiling(arcWidth.Theta / 6.283185307179586);
			num = Math.Max(1, num);
			double num2 = arcWidth / (double)num;
			Angle angle2 = angle - arcWidth / 2.0;
			HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
			for (int i = 0; i < num; i++)
			{
				Angle angle3 = angle2 + num2 * (double)i;
				List<RayCastResults> list = FriendlyFirePatch._physicsSystem.IntersectRay(mapId, new CollisionRay(position, angle3.ToWorldVec(), 31), range, new EntityUid?(ignore), false).ToList<RayCastResults>();
				bool flag3 = list.Count != 0;
				if (flag3)
				{
					EntityUid hitEntity = list[0].HitEntity;
					bool flag4 = !FriendlyFirePatch._friendSystem.IsFriend(hitEntity);
					if (flag4)
					{
						hashSet.Add(hitEntity);
					}
				}
			}
			__result = hashSet;
			flag2 = false;
		}
		return flag2;
	}
	private static FriendSystem _friendSystem;
	private static SharedPhysicsSystem _physicsSystem;
}
