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
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using HarmonyLib;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;


[CompilerGenerated]
public sealed class GunTargetSystem : EntitySystem
{
	public EntityUid? GetClosestTargetToPlayer(EntityUid player, Vector2 circleCenter, float circleRadius, bool targetCritical, GunAimbotOverlay overlay)
	{
		List<EntityUid> list = new List<EntityUid>();
		MapId mapId = this._transformSystem.GetMapId(player);
		foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInRange(mapId, circleCenter, circleRadius))
		{
			bool flag = this.IsValidTarget(entityUid, targetCritical, overlay);
			if (flag)
			{
				bool flag2 = this._transformSystem.GetMapId(entityUid) == this._transformSystem.GetMapId(player);
				if (flag2)
				{
					list.Add(entityUid);
				}
			}
		}
		list.Sort(delegate(EntityUid a, EntityUid b)
		{
			bool flag3 = this._prioritySystem.IsPriority(a);
			bool flag4 = this._prioritySystem.IsPriority(b);
			bool flag5 = flag3 && !flag4;
			int num;
			if (flag5)
			{
				num = -1;
			}
			else
			{
				bool flag6 = !flag3 && flag4;
				if (flag6)
				{
					num = 1;
				}
				else
				{
					float num2 = (this._transformSystem.GetWorldPosition(a) - this._transformSystem.GetWorldPosition(player)).Length();
					float num3 = (this._transformSystem.GetWorldPosition(b) - this._transformSystem.GetWorldPosition(player)).Length();
					num = num2.CompareTo(num3);
				}
			}
			return num;
		});
		bool onlyPriority = CerberusConfig.GunAimBot.OnlyPriority;
		EntityUid? entityUid2;
		if (onlyPriority)
		{
			entityUid2 = new EntityUid?(list.FirstOrDefault((EntityUid t) => this._prioritySystem.IsPriority(t)));
		}
		else
		{
			entityUid2 = new EntityUid?(list.FirstOrDefault<EntityUid>());
		}
		return entityUid2;
	}
	public EntityUid? GetClosestTargetToMouse(Vector2 circleCenter, float circleRadius, bool targetCritical, GunAimbotOverlay overlay)
	{
		List<EntityUid> list = new List<EntityUid>();
		EntityUid value = this._playerManager.LocalEntity.Value;
		MapId mapId = this._transformSystem.GetMapId(value);
		foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInRange(mapId, circleCenter, circleRadius))
		{
			bool flag = this.IsValidTarget(entityUid, targetCritical, overlay);
			if (flag)
			{
				float num = (this._transformSystem.GetWorldPosition(entityUid) - circleCenter).LengthSquared();
				bool flag2 = num <= circleRadius * circleRadius;
				if (flag2)
				{
					list.Add(entityUid);
				}
			}
		}
		list.Sort(delegate(EntityUid a, EntityUid b)
		{
			bool flag3 = this._prioritySystem.IsPriority(a);
			bool flag4 = this._prioritySystem.IsPriority(b);
			bool flag5 = flag3 && !flag4;
			int num2;
			if (flag5)
			{
				num2 = -1;
			}
			else
			{
				bool flag6 = !flag3 && flag4;
				if (flag6)
				{
					num2 = 1;
				}
				else
				{
					float num3 = (this._transformSystem.GetWorldPosition(a) - circleCenter).Length();
					float num4 = (this._transformSystem.GetWorldPosition(b) - circleCenter).Length();
					num2 = num3.CompareTo(num4);
				}
			}
			return num2;
		});
		bool onlyPriority = CerberusConfig.GunAimBot.OnlyPriority;
		EntityUid? entityUid2;
		if (onlyPriority)
		{
			entityUid2 = new EntityUid?(list.FirstOrDefault((EntityUid t) => this._prioritySystem.IsPriority(t)));
		}
		else
		{
			entityUid2 = new EntityUid?(list.FirstOrDefault<EntityUid>());
		}
		return entityUid2;
	}
	private bool IsValidTarget(EntityUid entity, bool targetCritical, GunAimbotOverlay overlay)
    {
        // 1. Не стреляем в себя
        if (entity == this._playerManager.LocalEntity) return false;

        // 2. Проверяем наличие компонента состояния моба
        MobStateComponent mobStateComponent;
        if (!this._entityManager.TryGetComponent<MobStateComponent>(entity, out mobStateComponent)) return false;

        // 3. Не стреляем в друзей (система друзей)
        if (this._friendSystem.IsFriend(entity)) return false;

        // 4. Логика состояния (Живой / Крит / Мертвый)
        // Мертвых игнорируем всегда
        if (mobStateComponent.CurrentState == MobState.Dead) return false;

        // Логика галочки "Target Critical" (бить лежачих)
        if (mobStateComponent.CurrentState == MobState.Critical)
        {
            // Если цель в крите, стреляем ТОЛЬКО если включена опция TargetCritical
            if (!targetCritical) return false;
        }
        // Если цель жива (Alive), стреляем всегда (при условии прохождения остальных проверок)

        // 5. Проверка видимости / HitScan / Предикта
        // Если предикт включен и есть позиция -> проверяем её. Иначе проверяем текущую позицию.
        if (CerberusConfig.GunAimBot.PredictEnabled && overlay.PredictedPos != null)
        {
            return this.CanHitTargetWithHitScan(this._playerManager.LocalEntity.Value, entity, new EntityCoordinates?(overlay.PredictedPos.Value));
        }
        
        return this.CanHitTargetWithHitScan(this._playerManager.LocalEntity.Value, entity, null);
    }

    public EntityUid? GetHighestHealthTarget(Vector2 circleCenter, float circleRadius, bool targetCritical, GunAimbotOverlay overlay)
    {
        List<EntityUid> list = new List<EntityUid>();
        EntityUid localPlayer = this._playerManager.LocalEntity.Value;
        MapId mapId = this._transformSystem.GetMapId(localPlayer);

        // Собираем всех кандидатов в радиусе
        foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInRange(mapId, circleCenter, circleRadius))
        {
            // Проверяем валидность цели и наличие компонента урона
            if (this._entityManager.HasComponent<DamageableComponent>(entityUid) && 
                this.IsValidTarget(entityUid, targetCritical, overlay))
            {
                list.Add(entityUid);
            }
        }

        // Сортируем список
        list.Sort(delegate(EntityUid a, EntityUid b)
        {
            // 1. Приоритет (ручной выбор игрока)
            bool isPriorityA = this._prioritySystem.IsPriority(a);
            bool isPriorityB = this._prioritySystem.IsPriority(b);
            
            if (isPriorityA && !isPriorityB) return -1; // A выше
            if (!isPriorityA && isPriorityB) return 1;  // B выше

            // 2. Сравнение по здоровью (Lowest Health = Highest Damage)
            DamageableComponent dmgA = EntityManagerExt.GetComponentOrNull<DamageableComponent>(this._entityManager, a);
            DamageableComponent dmgB = EntityManagerExt.GetComponentOrNull<DamageableComponent>(this._entityManager, b);

            if (dmgA != null && dmgB != null)
            {
                // Получаем TotalDamage (урон). Чем больше урон, тем меньше здоровья.
                // Мы хотим сначала тех, у кого МАЛО здоровья (МНОГО урона).
                // Sort descending by Damage: B.CompareTo(A)
                
                // Используем рефлексию для надежности, если поле приватное, или свойство TotalDamage
                // Предполагаем, что TotalDamage доступно как свойство
                
                Content.Shared.FixedPoint.FixedPoint2 damageA = dmgA.TotalDamage;
                Content.Shared.FixedPoint.FixedPoint2 damageB = dmgB.TotalDamage;

                int damageComparison = damageB.CompareTo(damageA); // Убывание урона
                if (damageComparison != 0) return damageComparison;
            }

            // 3. Если здоровье одинаковое (или не удалось получить), выбираем ближайшего к прицелу
            float distA = (this._transformSystem.GetWorldPosition(a) - circleCenter).LengthSquared();
            float distB = (this._transformSystem.GetWorldPosition(b) - circleCenter).LengthSquared();
            
            return distA.CompareTo(distB); // Возрастание дистанции
        });

        // Возвращаем результат с учетом настройки "Только приоритетные"
        bool onlyPriority = CerberusConfig.GunAimBot.OnlyPriority;
        if (onlyPriority)
        {
            return list.FirstOrDefault((EntityUid t) => this._prioritySystem.IsPriority(t));
        }
        
        return list.FirstOrDefault();
    }
	public bool CanHitTargetWithHitScan(EntityUid userUid, EntityUid targetUid, EntityCoordinates? targetCoords = null)
	{
		bool flag = !CerberusConfig.GunAimBot.HitScan;
		bool flag2 = false;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			MapCoordinates mapCoordinates = this._transformSystem.GetMapCoordinates(userUid, null);
			Vector2 vector = ((targetCoords != null) ? this._transformSystem.ToMapCoordinates(targetCoords.Value, true).Position : this._transformSystem.GetMapCoordinates(targetUid, null).Position);
			EntityUid entityUid;
			GunComponent gunComponent;
			bool flag3 = !this._gunSystem.TryGetGun(userUid, out entityUid, out gunComponent);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				// bool flag4 = this._entityManager.HasComponent<>(entityUid);
				// int num;
				// if (flag4)
				// {
				// 	num = 1;
				// }
				// else
				// {
				// 	num = 64;
				// }
				// Vector2 vector2 = vector - mapCoordinates.Position;
				// CollisionRay collisionRay;
				// collisionRay(mapCoordinates.Position, Vector2Helpers.Normalized(vector2), num);
				// List<RayCastResults> list = this._physics.IntersectRay(mapCoordinates.MapId, collisionRay, vector2.Length(), new EntityUid?(userUid), true).ToList<RayCastResults>();
				// bool flag5 = !list.Any<RayCastResults>();
			}
		}
		return flag2;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly EntityLookupSystem _entityLookup = null;
	
	[Robust.Shared.IoC.Dependency] private readonly PrioritySystem _prioritySystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly FriendSystem _friendSystem = null;
	
	[Robust.Shared.IoC.Dependency]  private readonly SharedGunSystem _gunSystem = null;
	
	private readonly SharedPhysicsSystem _physics = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedTransformSystem _transformSystem = null;
}
