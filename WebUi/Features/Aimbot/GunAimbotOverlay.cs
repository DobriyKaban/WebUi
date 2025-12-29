using System;
using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Ranged.Components;
using HarmonyLib;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;


public sealed class GunAimbotOverlay : Overlay
{
	public GunAimbotOverlay()
	{
		IoCManager.InjectDependencies<GunAimbotOverlay>(this);
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)4;
		}
	}
	protected override void FrameUpdate(FrameEventArgs args)
	{
		bool flag = !CerberusConfig.GunAimBot.Enabled;
		if (flag)
		{
			bool flag2 = this._targetUid != null;
			if (flag2)
			{
				this._targetUid = null;
				this.PredictedPos = null;
			}
		}
		else
		{
			if (this._gunTargetSystem == null)
			{
				this._gunTargetSystem = this._sysMan.GetEntitySystem<GunTargetSystem>();
			}
			bool flag3 = this._gunTargetSystem == null;
			if (!flag3)
			{
				EntityUid? localEntity = this._playerManager.LocalEntity;
				bool flag4 = localEntity == null;
				if (flag4)
				{
					this._targetUid = null;
					this.PredictedPos = null;
				}
				else
				{
					CombatModeComponent combatModeComponent;
					bool flag5 = !this._entityManager.TryGetComponent<CombatModeComponent>(localEntity.Value, out combatModeComponent) || !combatModeComponent.IsInCombatMode;
					if (flag5)
					{
						this._targetUid = null;
						this.PredictedPos = null;
					}
					else
					{
						GunSystem entitySystem = this._sysMan.GetEntitySystem<GunSystem>();
						GunComponent gunComponent;
						bool flag6 = !entitySystem.TryGetGun(localEntity.Value, out this._gunUid, out gunComponent);
						if (flag6)
						{
							this._targetUid = null;
							this.PredictedPos = null;
						}
						else
						{
							bool flag7 = !this._entityManager.HasComponent<GunComponent>(this._gunUid);
							if (!flag7)
							{
								MapCoordinates mapCoordinates = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
								bool flag8 = mapCoordinates.MapId == MapId.Nullspace;
								if (flag8)
								{
									this._targetUid = null;
									this.PredictedPos = null;
								}
								else
								{
									this.MouseWorldPos = mapCoordinates.Position;
									EntityUid? prevTarget = this._targetUid;
									this._targetUid = this.GetTarget(localEntity.Value, this.MouseWorldPos);
									bool flag9 = this._targetUid != null;
									if (flag9)
									{
										double totalSeconds = (this._timing.CurTime - gunComponent.NextFire).TotalSeconds;
										this._currentAngle = new Angle(MathHelper.Clamp(gunComponent.CurrentAngle.Theta - gunComponent.AngleDecayModified.Theta * totalSeconds, gunComponent.MinAngleModified.Theta, gunComponent.MaxAngleModified.Theta));
										bool autoPredict = CerberusConfig.GunAimBot.AutoPredict;
										if (autoPredict)
										{
											bool predictEnabled = CerberusConfig.GunAimBot.PredictEnabled;
											bool flag10 = default;
											if (flag10)
											{
												CerberusConfig.GunAimBot.PredictEnabled = false;
											}
											else
											{
												CerberusConfig.GunAimBot.PredictEnabled = true;
											}
										}
										TransformComponent transformComponent = null;
										PhysicsComponent physicsComponent;
										bool flag11 = CerberusConfig.GunAimBot.PredictEnabled && this._entityManager.TryGetComponent<TransformComponent>(this._targetUid.Value, out transformComponent) && this._entityManager.TryGetComponent<PhysicsComponent>(this._targetUid.Value, out physicsComponent);
										if (flag11)
										{
											float projectileSpeedModified = this._entityManager.GetComponent<GunComponent>(this._gunUid).ProjectileSpeedModified;
											Vector2 predictedWorldShootPosition = PredictionUtils.GetPredictedWorldShootPosition(localEntity.Value, this._targetUid.Value, projectileSpeedModified, 5);
											EntityUid entityUid = transformComponent.MapUid ?? transformComponent.ParentUid;
											TransformComponent transformComponent2 = null;
											bool flag12 = entityUid.IsValid() && this._entityManager.TryGetComponent<TransformComponent>(entityUid, out transformComponent2);
											if (flag12)
											{
												SharedTransformSystem entitySystem2 = this._sysMan.GetEntitySystem<SharedTransformSystem>();
												Vector2 vector = Vector2.Transform(predictedWorldShootPosition, entitySystem2.GetInvWorldMatrix(transformComponent2));
												this.PredictedPos = new EntityCoordinates?(new EntityCoordinates(entityUid, vector));
											}
											else
											{
												bool flag13 = transformComponent.MapUid != null;
												if (flag13)
												{
													SharedTransformSystem entitySystem3 = this._sysMan.GetEntitySystem<SharedTransformSystem>();
													Vector2 vector2 = Vector2.Transform(predictedWorldShootPosition, entitySystem3.GetInvWorldMatrix(transformComponent.MapUid.Value));
													this.PredictedPos = new EntityCoordinates?(new EntityCoordinates(transformComponent.MapUid.Value, vector2));
												}
												else
												{
													this.PredictedPos = null;
												}
											}
										}
										else
										{
											this.PredictedPos = null;
										}
									}
									else
									{
										this.PredictedPos = null;
									}
									bool flag14 = this._targetUid != null;
									if (flag14)
									{
										this.HandleShooting();
									}
								}
							}
						}
					}
				}
			}
		}
	}
	protected override void Draw(in OverlayDrawArgs args)
	{
		bool flag = !PredictionUtils._isInitialized;
		if (flag)
		{
			PredictionUtils.InitializeDependencies(this._netMan, this._timing, this._cfg, this._entityManager);
		}
		bool flag2 = !CerberusConfig.GunAimBot.Enabled;
		if (!flag2)
		{
			bool flag3 = this._gunTargetSystem == null;
			if (!flag3)
			{
				EntityUid? localEntity = this._playerManager.LocalEntity;
				bool flag4 = localEntity == null || args.MapId == MapId.Nullspace;
				if (!flag4)
				{
					bool flag5 = !this._entityManager.HasComponent<GunComponent>(this._gunUid) || !this._entityManager.EntityExists(this._gunUid);
					if (!flag5)
					{
						SharedTransformSystem entitySystem = this._sysMan.GetEntitySystem<SharedTransformSystem>();
						TransformComponent component = this._entityManager.GetComponent<TransformComponent>(localEntity.Value);
						CombatModeComponent combatModeComponent;
						bool flag6 = this._entityManager.TryGetComponent<CombatModeComponent>(localEntity.Value, out combatModeComponent) && combatModeComponent.IsInCombatMode;
						if (flag6)
						{
							this.DrawCircleAndLine(args, this.MouseWorldPos, component, entitySystem);
						}
					}
				}
			}
		}
	}
	private void DrawCircleAndLine(OverlayDrawArgs args, Vector2 circleCenter, TransformComponent playerXform, SharedTransformSystem transformSystem)
	{
		bool showCircle = CerberusConfig.GunAimBot.ShowCircle;
		if (showCircle)
		{
			args.WorldHandle.DrawCircle(circleCenter, CerberusConfig.GunAimBot.CircleRadius, new Color(ref CerberusConfig.GunAimBot.Color), false);
		}
		bool flag = this._targetUid == null || !this._entityManager.EntityExists(this._targetUid.Value);
		if (!flag)
		{
			TransformComponent transformComponent;
			bool flag2 = !this._entityManager.TryGetComponent<TransformComponent>(this._targetUid.Value, out transformComponent);
			if (!flag2)
			{
				bool showLine = CerberusConfig.GunAimBot.ShowLine;
				if (showLine)
				{
					Vector2 worldPosition = transformSystem.GetWorldPosition(playerXform);
					Vector2 worldPosition2 = transformSystem.GetWorldPosition(transformComponent);
					args.WorldHandle.DrawLine(worldPosition, worldPosition2, new Color(ref CerberusConfig.GunAimBot.Color));
					bool flag3 = CerberusConfig.GunAimBot.PredictEnabled && this.PredictedPos != null;
					if (flag3)
					{
						bool flag4 = transformSystem.GetMapId(this.PredictedPos.Value.EntityId) != args.MapId;
						if (!flag4)
						{
							bool flag5 = this._gunTargetSystem.CanHitTargetWithHitScan(this._playerManager.LocalEntity.Value, this._targetUid.Value, new EntityCoordinates?(this.PredictedPos.Value));
							Vector2 position = transformSystem.ToMapCoordinates(this.PredictedPos.Value, true).Position;
							args.WorldHandle.DrawLine(worldPosition2, position, flag5 ? Color.Aqua : new Color(ref CerberusConfig.GunAimBot.Color));
						}
					}
				}
			}
		}
	}
	private void HandleShooting()
	{
		if (this._targetUid == null || !this._entityManager.EntityExists(this._targetUid.Value) || !this._entityManager.EntityExists(this._gunUid)) return;

		// ЗАМЕНА: Используем InputUtils вместо ImGuiWidgets
		if (!InputUtils.IsKeyDown(CerberusConfig.GunAimBot.HotKey)) return;

		if (!this._entityManager.TryGetComponent<GunComponent>(this._gunUid, out var gunComponent)) return;

		if (gunComponent.NextFire > this._timing.CurTime) return;

		if (!CerberusConfig.GunAimBot.MinSpread || this._currentAngle <= gunComponent.MinAngleModified + Angle.FromDegrees(1.0))
		{
			if (!this._entityManager.TryGetComponent<TransformComponent>(this._targetUid.Value, out var transformComponent)) return;

			EntityCoordinates entityCoordinates;
			if (CerberusConfig.GunAimBot.PredictEnabled && this.PredictedPos != null)
			{
				if (this._playerManager.LocalEntity == null || !this._entityManager.EntityExists(this._targetUid.Value)) return;
				
				bool canHit = this._gunTargetSystem.CanHitTargetWithHitScan(this._playerManager.LocalEntity.Value, this._targetUid.Value, this.PredictedPos.Value);
				entityCoordinates = canHit ? this.PredictedPos.Value : transformComponent.Coordinates;
			}
			else
			{
				entityCoordinates = transformComponent.Coordinates;
			}

			EntityEventArgs entityEventArgs = this.CreateShootEvent(entityCoordinates, this._gunUid, this._targetUid.Value);
			this._entityManager.RaisePredictiveEvent(entityEventArgs);
		}
	}
	private EntityUid? GetTarget(EntityUid playerEnt, Vector2 circleCenter)
	{
		EntityUid? entityUid = null;
		bool flag = this._gunTargetSystem == null;
		EntityUid? entityUid2;
		if (flag)
		{
			entityUid2 = null;
		}
		else
		{
			switch (CerberusConfig.GunAimBot.TargetPriority)
			{
			case 0:
				entityUid = this._gunTargetSystem.GetClosestTargetToPlayer(playerEnt, circleCenter, CerberusConfig.GunAimBot.CircleRadius, CerberusConfig.GunAimBot.TargetCritical, this);
				break;
			case 1:
				entityUid = this._gunTargetSystem.GetClosestTargetToMouse(circleCenter, CerberusConfig.GunAimBot.CircleRadius, CerberusConfig.GunAimBot.TargetCritical, this);
				break;
			case 2:
				entityUid = this._gunTargetSystem.GetHighestHealthTarget(circleCenter, CerberusConfig.GunAimBot.CircleRadius, CerberusConfig.GunAimBot.TargetCritical, this);
				break;
			}
			entityUid2 = entityUid;
		}
		return entityUid2;
	}
	private EntityEventArgs CreateShootEvent(EntityCoordinates coords, EntityUid gunUid, EntityUid targetUid)
	{
		Type type = AccessTools.TypeByName("Content.Shared.Weapons.Ranged.Events.RequestShootEvent");
		EntityEventArgs entityEventArgs = (EntityEventArgs)AccessTools.CreateInstance(type);
		AccessTools.Field(type, "Gun").SetValue(entityEventArgs, this._entityManager.GetNetEntity(gunUid, null));
		AccessTools.Field(type, "Coordinates").SetValue(entityEventArgs, this._entityManager.GetNetCoordinates(coords, null));
		FieldInfo fieldInfo = AccessTools.Field(type, "Target");
		bool flag = fieldInfo != null;
		if (flag)
		{
			fieldInfo.SetValue(entityEventArgs, this._entityManager.GetNetEntity(targetUid, null));
		}
		else
		{
			FieldInfo fieldInfo2 = AccessTools.Field(type, "Targets");
			bool flag2 = fieldInfo2 != null;
			if (flag2)
			{
				object value = fieldInfo2.GetValue(entityEventArgs);
				IList list = value as IList;
				bool flag3 = list != null;
				if (flag3)
				{
					list.Add(this._entityManager.GetNetEntity(targetUid, null));
				}
			}
		}
		return entityEventArgs;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _sysMan = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IConfigurationManager _cfg = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IClientNetManager _netMan = null;
	
	private GunTargetSystem _gunTargetSystem;
	private Angle _currentAngle;
	private EntityUid _gunUid;
	private EntityUid? _targetUid;
	public EntityCoordinates? PredictedPos;
	public Vector2 MouseWorldPos;
}
