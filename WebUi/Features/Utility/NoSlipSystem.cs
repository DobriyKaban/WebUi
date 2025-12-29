using System;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Movement.Components;
using Content.Shared.Slippery;
using Content.Shared.StepTrigger.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;


[CompilerGenerated]
public sealed class NoSlipSystem : EntitySystem
{
	public override void FrameUpdate(float frameTime)
	{
		bool flag = !CerberusConfig.Misc.AntiSoapEnabled;
		if (!flag)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			bool flag2 = localEntity == null;
			if (!flag2)
			{
				bool flag3 = this.ShouldPressWalk(localEntity.Value);
				this._wasPressingWalk = flag3 != this._lastWalkState;
				this._lastWalkState = flag3;
				bool changedState = this._wasPressingWalk;
				if (changedState)
				{
					this.PressWalk((BoundKeyState)(this._lastWalkState ? 1 : 0));
				}
			}
		}
	}
	private void PressWalk(BoundKeyState state)
	{
		bool flag = this._playerManager.LocalEntity == null;
		if (!flag)
		{
			EntityUid value = this._playerManager.LocalEntity.Value;
			bool flag2 = !this.CanSlip(value);
			if (!flag2)
			{
				EntityCoordinates moverCoordinates = this._transformSystem.GetMoverCoordinates(value);
				ScreenCoordinates screenCoordinates = this._eyeManager.CoordinatesToScreen(this._transformSystem.GetMoverCoordinates(value));
				KeyFunctionId keyFunctionId = this._inputManager.NetworkBindMap.KeyFunctionID(EngineKeyFunctions.Walk);
				ClientFullInputCmdMessage clientFullInputCmdMessage = new ClientFullInputCmdMessage(this._timing.CurTick, this._timing.TickFraction, keyFunctionId)
				{
					State = state,
					Coordinates = moverCoordinates,
					ScreenCoordinates = screenCoordinates,
					Uid = EntityUid.Invalid
				};
				this._inputSystem.HandleInputCommand(this._playerManager.LocalSession, EngineKeyFunctions.Walk, clientFullInputCmdMessage, false);
			}
		}
	}
	private bool ShouldPressWalk(EntityUid player)
	{
		BaseContainer baseContainer;
		bool flag = this._containerSystem.TryGetContainer(player, "shoes", out baseContainer, null) && baseContainer.ContainedEntities.Count > 0;
		foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInRange(player, 1f))
		{
			StepTriggerComponent stepTriggerComponent;
			bool flag2 = !base.TryComp<StepTriggerComponent>(entityUid, out stepTriggerComponent) || !stepTriggerComponent.Active;
			if (!flag2)
			{
				ValueTuple<float, float> playerSpeed = this.GetPlayerSpeed(player);
				float item = playerSpeed.Item1;
				float item2 = playerSpeed.Item2;
				bool flag3 = item2 < stepTriggerComponent.RequiredTriggeredSpeed || item > stepTriggerComponent.RequiredTriggeredSpeed;
				if (!flag3)
				{
					bool flag4 = base.HasComp<SlipperyComponent>(entityUid);
					if (flag4)
					{
						MetaDataComponent metaDataComponent;
						bool flag5;
						if (base.TryComp<MetaDataComponent>(entityUid, out metaDataComponent))
						{
							EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
							flag5 = entityPrototype != null && entityPrototype.ID.Contains("ShardGlass");
						}
						else
						{
							flag5 = false;
						}
						bool flag6 = flag5;
						if (!flag6)
						{
							return true;
						}
						bool flag7 = !flag;
						if (flag7)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}
	
	private ValueTuple<float, float> GetPlayerSpeed(EntityUid player)
	{
		MovementSpeedModifierComponent movementSpeedModifierComponent;
		base.TryComp<MovementSpeedModifierComponent>(player, out movementSpeedModifierComponent);
		return new ValueTuple<float, float>((movementSpeedModifierComponent != null) ? movementSpeedModifierComponent.CurrentWalkSpeed : 2.5f, (movementSpeedModifierComponent != null) ? movementSpeedModifierComponent.CurrentSprintSpeed : 4.5f);
	}
	public bool CanSlip(EntityUid target)
	{
		bool flag = base.HasComp<NoSlipComponent>(target);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			BaseContainer baseContainer;
			bool flag3 = !this._containerSystem.TryGetContainer(target, "shoes", out baseContainer, null) || baseContainer.ContainedEntities.Count <= 0;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				EntityUid entityUid = baseContainer.ContainedEntities[0];
				flag2 = !base.HasComp<NoSlipComponent>(entityUid);
			}
		}
		return flag2;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly InputSystem _inputSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly EntityLookupSystem _entityLookup = null;
	
	[Robust.Shared.IoC.Dependency] private readonly ContainerSystem _containerSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedTransformSystem _transformSystem = null;
	private bool _wasPressingWalk;
	private bool _lastWalkState;
}
