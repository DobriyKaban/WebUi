using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Examine;
using Content.Shared.Movement.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


[CompilerGenerated]
public class CameraSystem : EntitySystem
{
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = default!;
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = default!;
	
	private const float DefaultZoom = 0.5f;
	private const float ZoomStep = 0.5f;
	private EntityUid? _localEntity;
	
	public override void Update(float frameTime)
	{
		this._localEntity = this._playerManager.LocalEntity;
		bool flag = this._localEntity == null;
		if (!flag)
		{
			ContentEyeComponent contentEyeComponent;
			EyeComponent eyeComponent = null;
			ExaminerComponent examinerComponent = null;
			bool flag2 = !this._entityManager.TryGetComponent<ContentEyeComponent>(this._localEntity, out contentEyeComponent) || !this._entityManager.TryGetComponent<EyeComponent>(this._localEntity, out eyeComponent) || !this._entityManager.TryGetComponent<ExaminerComponent>(this._localEntity, out examinerComponent);
			if (!flag2)
			{
				this.DisableNetSync(contentEyeComponent, eyeComponent, examinerComponent);
				this.ApplyVisualConfigurations(eyeComponent, examinerComponent);
				this.UpdateZoom(contentEyeComponent, eyeComponent);
			}
		}
	}
	private void DisableNetSync(ContentEyeComponent zoomComponent, EyeComponent eyeComponent, ExaminerComponent examinerComponent)
	{
		zoomComponent.NetSyncEnabled = false;
		eyeComponent.NetSyncEnabled = false;
		examinerComponent.NetSyncEnabled = false;
	}
	private void ApplyVisualConfigurations(EyeComponent eyeComponent, ExaminerComponent examinerComponent)
	{
		eyeComponent.Eye.DrawFov = !CerberusConfig.Eye.FovEnabled;
		eyeComponent.Eye.DrawLight = !CerberusConfig.Eye.FullBrightEnabled;
		examinerComponent.CheckInRangeUnOccluded = !CerberusConfig.Eye.FovEnabled;
		examinerComponent.SkipChecks = CerberusConfig.Eye.FovEnabled;
	}
	private void UpdateZoom(ContentEyeComponent zoomComponent, EyeComponent eyeComponent)
	{
		float zoom = CerberusConfig.Eye.Zoom;
		zoomComponent.TargetZoom = new Vector2(zoom);
		base.Dirty(this._localEntity.Value, eyeComponent, null);
	}
	
}
