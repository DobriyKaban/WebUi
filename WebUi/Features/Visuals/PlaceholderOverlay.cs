using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Explosion.Components;
using Content.Shared.Trigger.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;


public class PlaceholderOverlay : Overlay
{
	public PlaceholderOverlay()
	{
		IoCManager.InjectDependencies<PlaceholderOverlay>(this);
		if (this._font == null)
		{
			this._font = new VectorFont(this._resourceCache.GetResource<FontResource>("/Fonts/Boxfont-round/Boxfont Round.ttf", true), 12);
		}
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)2;
		}
	}
	protected override void Draw(in OverlayDrawArgs args)
	{
		bool flag = !CerberusConfig.Misc.ShowExplosive;
		if (!flag)
		{
			if (this._transformSystem == null)
			{
				this._transformSystem = this._entityManager.System<SharedTransformSystem>();
			}
			if (this._containerSystem == null)
			{
				this._containerSystem = this._entityManager.System<SharedContainerSystem>();
			}
			EntityQueryEnumerator<ActiveTimerTriggerComponent> entityQueryEnumerator = this._entityManager.EntityQueryEnumerator<ActiveTimerTriggerComponent>();
			for (;;)
			{
				EntityUid entityUid;
				ActiveTimerTriggerComponent activeTimerTriggerComponent;
				bool flag2 = entityQueryEnumerator.MoveNext(out entityUid, out activeTimerTriggerComponent);
				if (!flag2)
				{
					break;
				}
				Vector2 vector = this._eyeManager.WorldToScreen(this._transformSystem.GetWorldPosition(entityUid));
				Angle worldRotation = this._transformSystem.GetWorldRotation(entityUid);
				SpriteComponent spriteComponent;
				bool flag3 = !this._entityManager.TryGetComponent<SpriteComponent>(entityUid, out spriteComponent) || spriteComponent.Icon == null;
				if (flag3)
				{
					break;
				}
				args.ScreenHandle.DrawString(this._font, vector + new Vector2(-35f, 20f), "Danger", Color.Red);
				bool flag4 = !this._containerSystem.IsEntityInContainer(entityUid, null);
				if (flag4)
				{
					break;
				}
				args.ScreenHandle.DrawEntity(entityUid, vector, new Vector2(3f), new Angle?(worldRotation.GetDir().ToAngle()), this._eyeManager.CurrentEye.Rotation, null, null, null, null);
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IResourceCache _resourceCache = null;
	private readonly Font _font;
	
	private SharedTransformSystem _transformSystem;
	
	private SharedContainerSystem _containerSystem;
}
