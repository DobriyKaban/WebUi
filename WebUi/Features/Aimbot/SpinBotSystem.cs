using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.MouseRotator;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

[CompilerGenerated]
public class SpinBotSystem : EntitySystem
{
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = default!;
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = default!;
	[Robust.Shared.IoC.Dependency] private readonly SharedTransformSystem _transformSystem = default!;
	private float _rotationAngle;
	
	public override void Update(float frameTime)
	{
		bool flag = !this._timing.IsFirstTimePredicted || !CerberusConfig.Misc.AntiAimEnabled;
		if (!flag)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			MouseRotatorComponent mouseRotatorComponent;
			bool flag2 = localEntity == null || !base.TryComp<MouseRotatorComponent>(localEntity, out mouseRotatorComponent);
			if (!flag2)
			{
				this.RotatePlayer(frameTime);
			}
		}
	}
	public void RotateToTarget(Vector2 targetPosition)
	{
		bool flag = !this._timing.IsFirstTimePredicted;
		if (!flag)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			MouseRotatorComponent mouseRotatorComponent;
			bool flag2 = localEntity == null || !base.TryComp<MouseRotatorComponent>(localEntity, out mouseRotatorComponent);
			if (!flag2)
			{
				Vector2 worldPosition = this._transformSystem.GetWorldPosition(localEntity.Value);
				Angle worldRotation = this._transformSystem.GetWorldRotation(localEntity.Value);
				Angle angle = DirectionExtensions.ToWorldAngle(targetPosition - worldPosition);
				this.SendRotationEvent(angle);
			}
		}
	}
	private void RotatePlayer(float frameTime)
	{
		float autoRotateSpeed = CerberusConfig.Misc.AutoRotateSpeed;
		bool flag = this._rotationAngle + autoRotateSpeed * frameTime > 360f;
		if (flag)
		{
			this._rotationAngle = 0f;
		}
		this._rotationAngle += autoRotateSpeed * frameTime;
		Angle angle = Angle.FromDegrees((double)this._rotationAngle);
		this.SendRotationEvent(angle);
	}
	private void SendRotationEvent(Angle targetRotation)
	{
		base.RaisePredictiveEvent<RequestMouseRotatorRotationEvent>(new RequestMouseRotatorRotationEvent
		{
			Rotation = targetRotation
		});
	}
	

}
