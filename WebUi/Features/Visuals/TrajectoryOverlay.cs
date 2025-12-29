using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;


[CompilerGenerated]
public sealed class TrajectoryOverlay : Overlay
{
	public TrajectoryOverlay()
	{
		IoCManager.InjectDependencies<TrajectoryOverlay>(this);
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)4;
		}
	}
	protected override void Draw(in OverlayDrawArgs args)
	{
		bool flag = !CerberusConfig.Misc.ShowTrajectory;
		if (!flag)
		{
			if (this._physics == null)
			{
				this._physics = this._entityManager.System<SharedPhysicsSystem>();
			}
			if (this._xform == null)
			{
				this._xform = this._entityManager.System<SharedTransformSystem>();
			}
			EntityUid? localEntity = this._playerManager.LocalEntity;
			MapCoordinates mapCoordinates = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
			bool flag2 = localEntity == null || mapCoordinates.MapId != this._xform.GetMapId(localEntity.Value);
			if (!flag2)
			{
				Vector2 vector = this.RayCast(localEntity.Value, mapCoordinates);
				args.WorldHandle.DrawLine(this._xform.GetWorldPosition(localEntity.Value), vector, (vector == mapCoordinates.Position) ? Color.Green : Color.Red);
			}
		}
	}
	private Vector2 RayCast(EntityUid player, MapCoordinates targetPos)
	{
		MapCoordinates mapCoordinates = this._xform.GetMapCoordinates(player, null);
		Vector2 position = targetPos.Position;
		int num = 64;
		Vector2 vector = position - mapCoordinates.Position;
		CollisionRay collisionRay = default;
		List<RayCastResults> list = this._physics.IntersectRay(mapCoordinates.MapId, collisionRay, vector.Length(), new EntityUid?(player), true).ToList<RayCastResults>();
		bool flag = list.Count > 0;
		Vector2 vector3;
		if (flag)
		{
			float num2 = float.MaxValue;
			Vector2 vector2 = default(Vector2);
			foreach (RayCastResults rayCastResults in list)
			{
				float num3 = (rayCastResults.HitPos - mapCoordinates.Position).Length();
				bool flag2 = num3 < num2;
				if (flag2)
				{
					num2 = num3;
					vector2 = rayCastResults.HitPos;
				}
			}
			vector3 = vector2;
		}
		else
		{
			vector3 = position;
		}
		return vector3;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	private SharedPhysicsSystem _physics;
	
	private SharedTransformSystem _xform;
}
