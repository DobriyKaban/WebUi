using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Weapons.Melee;
using HarmonyLib;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;


[CompilerGenerated]
public sealed class MeleeTargetSystem : EntitySystem
{
	private IEnumerable<EntityUid> FindPotentialTargets(Vector2 circleCenter, float circleRadius, MeleeWeaponComponent melee, bool targetCritical)
	{
		EntityUid value = this._playerManager.LocalEntity.Value;
		MapId mapId = this._transformSystem.GetMapId(value);
		return from entity in this._entityLookup.GetEntitiesInRange(mapId, circleCenter, circleRadius)
			where this.IsValidTarget(entity, melee, circleCenter, circleRadius, targetCritical)
			select entity;
	}
	private EntityUid? SelectTarget(IEnumerable<EntityUid> targets, Func<EntityUid, EntityUid, int> comparison)
	{
		List<EntityUid> list = targets.ToList<EntityUid>();
		list.Sort((EntityUid a, EntityUid b) => this.CompareTargets(a, b, comparison));
		bool onlyPriority = CerberusConfig.MeleeAimBot.OnlyPriority;
		EntityUid? entityUid;
		if (onlyPriority)
		{
			entityUid = new EntityUid?(list.FirstOrDefault((EntityUid t) => this._prioritySystem.IsPriority(t)));
		}
		else
		{
			entityUid = new EntityUid?(list.FirstOrDefault<EntityUid>());
		}
		return entityUid;
	}
	private int CompareTargets(EntityUid a, EntityUid b, Func<EntityUid, EntityUid, int> customCompare)
	{
		bool flag = this._prioritySystem.IsPriority(a);
		bool flag2 = this._prioritySystem.IsPriority(b);
		if (!true)
		{
		}
		int num;
		if (flag)
		{
			if (!flag2)
			{
				num = -1;
				goto IL_0093;
			}
		}
		else if (flag2)
		{
			num = 1;
			goto IL_0093;
		}
		num = customCompare(a, b);
		IL_0093:
		if (!true)
		{
		}
		return num;
	}
	private bool IsValidTarget(EntityUid entity, MeleeWeaponComponent melee, Vector2 circleCenter, float circleRadius, bool targetCritical)
	{
		return this.IsNotSelf(entity) && this.HasValidMobState(entity, targetCritical) && this.IsNotFriendly(entity) && this.IsInRange(entity, melee.Range) && this.IsWithinCircle(entity, circleCenter, circleRadius);
	}
	private bool IsNotSelf(EntityUid entity)
	{
		return entity != this._playerManager.LocalEntity.Value;
	}
	private bool HasValidMobState(EntityUid entity, bool targetCritical)
	{
		MobStateComponent mobStateComponent;
		bool flag = !this._entityManager.TryGetComponent<MobStateComponent>(entity, out mobStateComponent);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			MobState currentState = mobStateComponent.CurrentState;
			if (!true)
			{
			}
			bool flag3;
			if (currentState != (MobState)2)
			{
				if (currentState == (MobState)3)
				{
					flag3 = false;
					goto IL_00C2;
				}
			}
			else if (!targetCritical)
			{
				flag3 = false;
				goto IL_00C2;
			}
			flag3 = !this.HasBetterCriticalTarget(entity, targetCritical);
			IL_00C2:
			if (!true)
			{
			}
			flag2 = flag3;
		}
		return flag2;
	}
	private bool HasBetterCriticalTarget(EntityUid entity, bool targetCritical)
	{
		MobStateComponent mobStateComponent;
		bool flag = !targetCritical || !this._entityManager.TryGetComponent<MobStateComponent>(entity, out mobStateComponent) || mobStateComponent.CurrentState != (MobState)2;
		return !flag && this._entityLookup.GetEntitiesInRange(this._transformSystem.GetMapId(entity), this._transformSystem.GetWorldPosition(entity), 5f).Any((EntityUid e) => e != entity && this._entityManager.HasComponent<MobStateComponent>(e));
	}
	private bool IsNotFriendly(EntityUid entity)
	{
		return !this._friendSystem.IsFriend(entity);
	}
	private bool IsInRange(EntityUid entity, float maxRange)
	{
		Vector2 worldPosition = this._transformSystem.GetWorldPosition(this._playerManager.LocalEntity.Value);
		Vector2 worldPosition2 = this._transformSystem.GetWorldPosition(entity);
		return (worldPosition2 - worldPosition).Length() <= maxRange;
	}
	private bool IsWithinCircle(EntityUid entity, Vector2 center, float radius)
	{
		Vector2 worldPosition = this._transformSystem.GetWorldPosition(entity);
		return (worldPosition - center).LengthSquared() <= radius * radius;
	}
	public EntityUid? GetClosestTargetToPlayer(MeleeWeaponComponent melee, Vector2 circleCenter, float circleRadius, bool targetCritical)
	{
		return this.SelectTarget(this.FindPotentialTargets(circleCenter, circleRadius, melee, targetCritical), (EntityUid a, EntityUid b) => this.CompareByDistanceToPlayer(a, b));
	}
	public EntityUid? GetClosestTargetToMouse(MeleeWeaponComponent melee, Vector2 circleCenter, float circleRadius, bool targetCritical)
	{
		return this.SelectTarget(this.FindPotentialTargets(circleCenter, circleRadius, melee, targetCritical), (EntityUid a, EntityUid b) => this.CompareByDistanceToPoint(a, b, circleCenter));
	}
	public EntityUid? GetHighestHealthTarget(MeleeWeaponComponent melee, Vector2 circleCenter, float circleRadius, bool targetCritical)
	{
		IEnumerable<EntityUid> enumerable = from e in this.FindPotentialTargets(circleCenter, circleRadius, melee, targetCritical)
			where this._entityManager.HasComponent<DamageableComponent>(e)
			select e;
		return this.SelectTarget(enumerable, (EntityUid a, EntityUid b) => this.CompareByHealth(a, b, circleCenter));
	}
	private int CompareByDistanceToPlayer(EntityUid a, EntityUid b)
	{
		Vector2 worldPosition = this._transformSystem.GetWorldPosition(this._playerManager.LocalEntity.Value);
		return this.DistanceTo(a, worldPosition).CompareTo(this.DistanceTo(b, worldPosition));
	}
	private int CompareByDistanceToPoint(EntityUid a, EntityUid b, Vector2 point)
	{
		return this.DistanceTo(a, point).CompareTo(this.DistanceTo(b, point));
	}
	private int CompareByHealth(EntityUid a, EntityUid b, Vector2 referencePoint)
	{
		FixedPoint2? totalDamageUniversal = this.GetTotalDamageUniversal(a);
		FixedPoint2? totalDamageUniversal2 = this.GetTotalDamageUniversal(b);
		bool flag = totalDamageUniversal == null || totalDamageUniversal2 == null;
		int num;
		if (flag)
		{
			bool flag2 = totalDamageUniversal != null;
			if (flag2)
			{
				num = -1;
			}
			else
			{
				bool flag3 = totalDamageUniversal2 != null;
				if (flag3)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
		}
		else
		{
			int num2 = totalDamageUniversal.Value.CompareTo(totalDamageUniversal2.Value);
			num = ((num2 != 0) ? num2 : this.CompareByDistanceToPoint(a, b, referencePoint));
		}
		return num;
	}
	private float DistanceTo(EntityUid entity, Vector2 point)
	{
		return (this._transformSystem.GetWorldPosition(entity) - point).Length();
	}
	private FixedPoint2? GetTotalDamageUniversal(EntityUid entity)
	{
		DamageableComponent damageableComponent;
		bool flag = !this._entityManager.TryGetComponent<DamageableComponent>(entity, out damageableComponent);
		FixedPoint2? dmgVal;
		if (flag)
		{
			dmgVal = null;
		}
		else
		{
			FieldInfo fieldInfo = AccessTools.Field(damageableComponent.GetType(), "TotalDamage");
			bool flag2 = fieldInfo == null;
			if (flag2)
			{
				dmgVal = null;
			}
			else
			{
				object value = fieldInfo.GetValue(damageableComponent);
				bool flag3 = value == null;
				if (flag3)
				{
					dmgVal = null;
				}
				else
				{
					try
					{
						dmgVal = new FixedPoint2?(FixedPoint2.FromObject(value));
					}
					catch
					{
						dmgVal = null;
					}
				}
			}
		}
		return dmgVal;
	}
	public HashSet<EntityUid> ArcRayCast(Vector2 position, Angle angle, Angle arcWidth, float range, MapId mapId, EntityUid ignore)
	{
		int collisionMask = 31;
		int rays = 1 + 35 * (int)Math.Ceiling(arcWidth.Theta / 6.283185307179586);
		double angleStep = arcWidth.Theta / rays;
		Angle startAngle = angle - arcWidth / 2.0;
		HashSet<EntityUid> hashSet = new HashSet<EntityUid>();
		for (int i = 0; i < rays; i++)
		{
			Angle angle3 = default;
			Angle rayAngle = new Angle(startAngle.Theta + angleStep * i);
			List<RayCastResults> list = this._physicsSystem.IntersectRay(mapId, new CollisionRay(position, angle3.ToWorldVec(), collisionMask), range, new EntityUid?(ignore), false).ToList<RayCastResults>();
			bool flag = list.Count == 0;
			if (!flag)
			{
				EntityUid hitEntity = list[0].HitEntity;
				bool noDmgFriendPatch = CerberusConfig.Settings.NoDmgFriendPatch;
				if (noDmgFriendPatch)
				{
					bool flag2 = !this._friendSystem.IsFriend(hitEntity);
					if (flag2)
					{
						hashSet.Add(hitEntity);
					}
				}
				else
				{
					hashSet.Add(hitEntity);
				}
			}
		}
		return hashSet;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly PrioritySystem _prioritySystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly EntityLookupSystem _entityLookup = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedPhysicsSystem _physicsSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly FriendSystem _friendSystem = null;
	
	[Robust.Shared.IoC.Dependency]  private readonly SharedTransformSystem _transformSystem = null;
}
