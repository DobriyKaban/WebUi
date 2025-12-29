using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using CerberusWareV3.Features.AimBot;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;


[CompilerGenerated]
public sealed class MeleeAimbotOverlay : Overlay
{
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)4;
		}
	}
	public MeleeAimbotOverlay()
	{
		IoCManager.InjectDependencies<MeleeAimbotOverlay>(this);
	}
	private void InitializeSystems()
	{
		if (this._meleeSystem == null)
		{
			this._meleeSystem = this._systemManager.GetEntitySystem<SharedMeleeWeaponSystem>();
		}
		if (this._transformSystem == null)
		{
			this._transformSystem = this._systemManager.GetEntitySystem<SharedTransformSystem>();
		}
		if (this._targetSystem == null)
		{
			this._targetSystem = this._systemManager.GetEntitySystem<MeleeTargetSystem>();
		}
		if (this._spinBotSystem == null)
		{
			this._spinBotSystem = this._systemManager.GetEntitySystem<SpinBotSystem>();
		}
	}
	protected override void Draw(in OverlayDrawArgs args)
	{
		this.InitializeSystems();
		bool flag = !this.IsAimbotActive(args.MapId);
		if (!flag)
		{
			Vector2? mouseWorldPosition = this.GetMouseWorldPosition(args);
			bool flag2 = mouseWorldPosition == null;
			if (!flag2)
			{
				MeleeWeaponComponent meleeWeaponComponent;
				bool flag3 = !this.TryGetWeapon(this._playerManager.LocalEntity.Value, out meleeWeaponComponent);
				if (!flag3)
				{
					this.DrawAimBotElements(args, mouseWorldPosition.Value);
					this.ProcessAttackLogic(mouseWorldPosition.Value, meleeWeaponComponent);
				}
			}
		}
	}
	private bool IsAimbotActive(MapId mapId)
	{
		return CerberusConfig.MeleeAimBot.Enabled && mapId != MapId.Nullspace && this.IsPlayerInCombatMode();
	}
	private bool IsPlayerInCombatMode()
	{
		EntityUid entityUid;
		CombatModeComponent combatModeComponent;
		return this.TryGetPlayerComponents(out entityUid, out combatModeComponent) && combatModeComponent.IsInCombatMode;
	}
	
	private bool TryGetWeapon(EntityUid uid, [NotNullWhen(true)] out MeleeWeaponComponent weaponComp)
	{
		EntityUid entityUid;
		bool flag = !this._meleeSystem.TryGetWeapon(uid, out entityUid, out weaponComp);
		GunComponent gunComponent;
		return !flag && (!this._entityManager.TryGetComponent<GunComponent>(entityUid, out gunComponent) || !gunComponent.UseKey);
	}
	private bool Exists(EntityUid uid)
	{
		return this._entityManager.EntityExists(uid) && !this._entityManager.Deleted(uid);
	}
	private bool IsKeyDown(BoundKeyFunction key)
	{
		IKeyBinding keyBinding;
		bool flag = !this._inputManager.TryGetKeyBinding(key, out keyBinding);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = !this._inputManager.IsKeyDown(keyBinding.BaseKey);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				bool flag4 = !CerberusConfig.MeleeAimBot.FixNetworkDelay;
				if (flag4)
				{
					flag2 = true;
				}
				else
				{
					bool flag5 = this._timing.CurTime < this._lastCheckTime + TimeSpan.FromSeconds((double)CerberusConfig.MeleeAimBot.FixDelay);
					if (flag5)
					{
						flag2 = false;
					}
					else
					{
						this._lastCheckTime = this._timing.CurTime;
						flag2 = true;
					}
				}
			}
		}
		return flag2;
	}
	private Vector2? GetMouseWorldPosition(OverlayDrawArgs args)
	{
		MapCoordinates mapCoordinates = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
		return (mapCoordinates.MapId == args.MapId) ? new Vector2?(mapCoordinates.Position) : null;
	}
	
	private bool TryGetPlayerComponents(out EntityUid player, [NotNullWhen(true)] out CombatModeComponent combatMode)
	{
		player = this._playerManager.LocalEntity.GetValueOrDefault();
		return this._entityManager.TryGetComponent<CombatModeComponent>(player, out combatMode);
	}
	private void DrawAimBotElements(OverlayDrawArgs args, Vector2 mousePos)
	{
		bool showCircle = CerberusConfig.MeleeAimBot.ShowCircle;
		if (showCircle)
		{
			args.WorldHandle.DrawCircle(mousePos, CerberusConfig.MeleeAimBot.CircleRadius, new Color(ref CerberusConfig.MeleeAimBot.Color), false);
		}
		EntityUid? entityUid;
		bool flag = this.TryGetTarget(mousePos, out entityUid);
		if (flag)
		{
			this.DrawTargetLine(args, mousePos, entityUid.Value);
		}
	}
	private void DrawTargetLine(OverlayDrawArgs args, Vector2 startPos, EntityUid target)
	{
		bool flag = !CerberusConfig.MeleeAimBot.ShowLine;
		if (!flag)
		{
			Vector2 worldPosition = this._transformSystem.GetWorldPosition(target);
			args.WorldHandle.DrawLine(startPos, worldPosition, new Color(ref CerberusConfig.MeleeAimBot.Color));
		}
	}
	private bool TryGetTarget(Vector2 mousePos, [NotNullWhen(true)] out EntityUid? target)
	{
		EntityUid entityUid;
		MeleeWeaponComponent meleeWeaponComponent;
		bool flag = !this._meleeSystem.TryGetWeapon(this._playerManager.LocalEntity.Value, out entityUid, out meleeWeaponComponent) || this._entityManager.HasComponent<GunComponent>(entityUid);
		bool flag2;
		if (flag)
		{
			target = null;
			flag2 = false;
		}
		else
		{
			TargetPriority targetPriority = (TargetPriority)CerberusConfig.MeleeAimBot.TargetPriority;
			if (!true)
			{
			}
			EntityUid? entityUid2;
			switch (targetPriority)
			{
			case TargetPriority.DistanceToPlayer:
				entityUid2 = this._targetSystem.GetClosestTargetToPlayer(meleeWeaponComponent, mousePos, CerberusConfig.MeleeAimBot.CircleRadius, CerberusConfig.MeleeAimBot.TargetCritical);
				break;
			case TargetPriority.DistanceToMouse:
				entityUid2 = this._targetSystem.GetClosestTargetToMouse(meleeWeaponComponent, mousePos, CerberusConfig.MeleeAimBot.CircleRadius, CerberusConfig.MeleeAimBot.TargetCritical);
				break;
			case TargetPriority.LowestHealth:
				entityUid2 = this._targetSystem.GetHighestHealthTarget(meleeWeaponComponent, mousePos, CerberusConfig.MeleeAimBot.CircleRadius, CerberusConfig.MeleeAimBot.TargetCritical);
				break;
			default:
				entityUid2 = null;
				break;
			}
			if (!true)
			{
			}
			target = entityUid2;
			bool flag3 = target != null && this.Exists(target.Value);
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				target = null;
				flag2 = false;
			}
		}
		return flag2;
	}
	private void ProcessAttackLogic(Vector2 mousePos, MeleeWeaponComponent weaponComp)
	{
		bool flag = weaponComp.NextAttack > this._timing.CurTime || weaponComp.Attacking;
		if (!flag)
		{
			EntityUid? entityUid;
			bool flag2 = !this.TryGetTarget(mousePos, out entityUid);
			if (flag2)
			{
				CerberusConfig.NoSavedConfig.HasTarget = false;
			}
			else
			{
				CerberusConfig.NoSavedConfig.HasTarget = true;
				bool rotateToTarget = CerberusConfig.MeleeHelper.RotateToTarget;
				if (rotateToTarget)
				{
					this._spinBotSystem.RotateToTarget(this._transformSystem.GetWorldPosition(entityUid.Value));
				}
				this.HandleLightAttack(weaponComp, entityUid.Value);
				this.HandleHeavyAttack(weaponComp, entityUid.Value);
			}
		}
	}
	private void HandleLightAttack(MeleeWeaponComponent weapon, EntityUid target)
	{
        // ИЗМЕНЕНИЕ: Используем InputUtils вместо ImGuiWidgets
		if (InputUtils.IsKeyDown(CerberusConfig.MeleeAimBot.LightHotKey))
		{
			EntityCoordinates entityCoordinates = this._transformSystem.ToCoordinates(target, this._transformSystem.GetMapCoordinates(target, null));
			this._entityManager.RaisePredictiveEvent(new LightAttackEvent(
                this._entityManager.GetNetEntity(target), 
                this._entityManager.GetNetEntity(weapon.Owner), 
                this._entityManager.GetNetCoordinates(entityCoordinates)
            ));
		}
	}

	private void HandleHeavyAttack(MeleeWeaponComponent weapon, EntityUid target)
	{
        // ИЗМЕНЕНИЕ: Используем InputUtils вместо ImGuiWidgets
		if (InputUtils.IsKeyDown(CerberusConfig.MeleeAimBot.HeavyHotKey))
		{
			Vector2 worldPosition = this._transformSystem.GetWorldPosition(this._playerManager.LocalEntity.Value);
			Vector2 worldPosition2 = this._transformSystem.GetWorldPosition(target);
			Vector2 vector = Vector2Helpers.Normalized(worldPosition2 - worldPosition);
			Vector2 vector2 = worldPosition + vector * weapon.Range;
			EntityCoordinates entityCoordinates = this._transformSystem.ToCoordinates(this._transformSystem.ToMapCoordinates(new EntityCoordinates(this._playerManager.LocalEntity.Value, vector2), true));
			
			if (weapon.AltDisarm && weapon.Owner == this._playerManager.LocalEntity)
			{
				this._entityManager.RaisePredictiveEvent(new DisarmAttackEvent(
                    this._entityManager.GetNetEntity(target), 
                    this._entityManager.GetNetCoordinates(entityCoordinates)
                ));
			}
			else
			{
				HashSet<EntityUid> hashSet = this._targetSystem.ArcRayCast(worldPosition, DirectionExtensions.ToWorldAngle(vector), weapon.Angle, weapon.Range, this._transformSystem.GetMapId(this._playerManager.LocalEntity.Value), this._playerManager.LocalEntity.Value);
				
                this._entityManager.RaisePredictiveEvent(new HeavyAttackEvent(
                    this._entityManager.GetNetEntity(weapon.Owner), 
                    hashSet.Select(e => this._entityManager.GetNetEntity(e)).ToList(), 
                    this._entityManager.GetNetCoordinates(entityCoordinates)
                ));
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _systemManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	private SharedMeleeWeaponSystem _meleeSystem;
	
	private SharedTransformSystem _transformSystem;
	
	private MeleeTargetSystem _targetSystem;
	
	private SpinBotSystem _spinBotSystem;
	private TimeSpan _lastCheckTime = TimeSpan.Zero;
}
