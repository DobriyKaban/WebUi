using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;


[CompilerGenerated]
public class FunOverlay : Overlay
{
	public FunOverlay()
	{
		this.ShakeIntensity = 0.1f;
		this.JumpHeight = 0.5f;
		
		IoCManager.InjectDependencies<FunOverlay>(this);
		base.ZIndex = new int?(200);
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)16;
		}
	}
	public float ShakeIntensity { get; set; }
	public float JumpHeight { get; set; }
	protected override void Draw(in OverlayDrawArgs args)
	{
		bool flag = !CerberusConfig.Fun.Enabled || !CerberusConfig.Fun.TrailsEnabled;
		if (!flag)
		{
			foreach (KeyValuePair<EntityUid, ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>> keyValuePair in this._entityData)
			{
				EntityUid entityUid;
				ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple;
				keyValuePair.Deconstruct(out entityUid, out valueTuple);
				EntityUid entityUid2 = entityUid;
				ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple2 = valueTuple;
				List<Vector2> item = valueTuple2.Item4;
				bool flag2 = item.Count < 2 || !this._entityManager.EntityExists(entityUid2);
				if (!flag2)
				{
					for (int i = 1; i < item.Count; i++)
					{
						//Color color = (CerberusConfig.Fun.RainbowEnabled ? Color.FromHsv(new Vector4((this._rainbowHue + (float)i * 360f / 30f) % 360f / 360f, 1f, 1f, 1f)) : new Color(ref CerberusConfig.Fun.Color));
						//args.WorldHandle.DrawLine(item[i - 1], item[i], color);
					}
				}
			}
		}
	}
	protected override void FrameUpdate(FrameEventArgs args)
	{
		bool flag = !CerberusConfig.Fun.Enabled;
		if (!flag)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple;
			bool flag2 = localEntity != null && !CerberusConfig.Fun.AffectPlayer && this._entityData.TryGetValue(localEntity.Value, out valueTuple);
			if (flag2)
			{
				SpriteComponent spriteComponent;
				bool flag3 = this._entityManager.TryGetComponent<SpriteComponent>(localEntity.Value, out spriteComponent);
				if (flag3)
				{
					valueTuple.Item1 = default;
					spriteComponent.Color = valueTuple.Item1;
					valueTuple.Item2 = default;
					spriteComponent.Rotation = valueTuple.Item2;
					valueTuple.Item3 = default;
					spriteComponent.Scale = valueTuple.Item3;
					valueTuple.Item6 = default;
					spriteComponent.Offset -= valueTuple.Item6;
				}
				this._entityData.Remove(localEntity.Value);
			}
			Box2 worldViewport = this._eyeManager.GetWorldViewport();
			HashSet<EntityUid> entitiesIntersecting = this._entityManager.System<EntityLookupSystem>().GetEntitiesIntersecting(this._eyeManager.CurrentMap, worldViewport);
			this._time += args.DeltaSeconds;
			foreach (EntityUid entityUid in entitiesIntersecting)
			{
				bool flag4;
				if (!CerberusConfig.Fun.AffectPlayer)
				{
					EntityUid entityUid2 = entityUid;
					EntityUid? entityUid3 = localEntity;
					flag4 = entityUid2 == entityUid3;
				}
				else
				{
					flag4 = false;
				}
				bool flag5 = flag4;
				if (!flag5)
				{
					SpriteComponent spriteComponent2;
					TransformComponent transformComponent = null;
					bool flag6 = !this._entityManager.TryGetComponent<SpriteComponent>(entityUid, out spriteComponent2) || !this._entityManager.TryGetComponent<TransformComponent>(entityUid, out transformComponent);
					if (!flag6)
					{
						bool flag7;
						if ((!CerberusConfig.Fun.AffectMobs || !this._entityManager.HasComponent<MobStateComponent>(entityUid)) && !CerberusConfig.Fun.AffectOthers)
						{
							if (CerberusConfig.Fun.AffectPlayer)
							{
								EntityUid entityUid2 = entityUid;
								EntityUid? entityUid3 = localEntity;
								flag7 = !(entityUid2 == entityUid3);
							}
							else
							{
								flag7 = true;
							}
						}
						else
						{
							flag7 = false;
						}
						bool flag8 = flag7;
						if (!flag8)
						{
							bool flag9 = !this._entityData.ContainsKey(entityUid);
							if (flag9)
							{
								this._entityData[entityUid] = new ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>(spriteComponent2.Color, spriteComponent2.Rotation, spriteComponent2.Scale, new List<Vector2>(), 0f, Vector2.Zero);
							}
							ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple2 = this._entityData[entityUid];
							Color item = valueTuple2.Item1;
							Angle item2 = valueTuple2.Item2;
							Vector2 item3 = valueTuple2.Item3;
							List<Vector2> item4 = valueTuple2.Item4;
							float item5 = valueTuple2.Item5;
							Vector2 item6 = valueTuple2.Item6;
							bool trailsEnabled = CerberusConfig.Fun.TrailsEnabled;
							if (trailsEnabled)
							{
								item4.Add(transformComponent.WorldPosition);
								bool flag10 = item4.Count > 30;
								if (flag10)
								{
									item4.RemoveAt(0);
								}
							}
							spriteComponent2.Scale = new Vector2(CerberusConfig.Fun.ScaleX, CerberusConfig.Fun.ScaleY);
							bool rainbowEnabled = CerberusConfig.Fun.RainbowEnabled;
							if (rainbowEnabled)
							{
								this._rainbowHue += CerberusConfig.Fun.RainbowSpeed * args.DeltaSeconds * 360f;
								this._rainbowHue %= 360f;
								//spriteComponent2.Color = Color.FromHsv(new Vector4(this._rainbowHue / 360f, 1f, 1f, 1f));
							}
							else
							{
								spriteComponent2.Color = item;
							}
							bool rotationEnabled = CerberusConfig.Fun.RotationEnabled;
							if (rotationEnabled)
							{
								spriteComponent2.Rotation += Angle.FromDegrees((double)(CerberusConfig.Fun.RotationSpeed * args.DeltaSeconds));
							}
							bool jumpEnabled = CerberusConfig.Fun.JumpEnabled;
							if (jumpEnabled)
							{
								float num = item5 + this._time * (float)(entityUid.GetHashCode() % 100) * 0.1f;
								num += this.JumpHeight * 16f * args.DeltaSeconds;
								bool flag11 = num > 3.1415927f;
								if (flag11)
								{
									num -= 6.2831855f;
								}
								spriteComponent2.Offset = new Vector2(0f, (float)Math.Abs(Math.Sin((double)num)) * this.JumpHeight);
							}
							bool shakeEnabled = CerberusConfig.Fun.ShakeEnabled;
							if (shakeEnabled)
							{
								spriteComponent2.Offset -= item6;
								item6 = new Vector2(this._random.NextFloat(-this.ShakeIntensity, this.ShakeIntensity), this._random.NextFloat(-this.ShakeIntensity, this.ShakeIntensity));
								spriteComponent2.Offset += item6;
							}
							this._entityData[entityUid] = new ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>(item, item2, item3, item4, item5, item6);
						}
					}
				}
			}
			List<EntityUid> list = new List<EntityUid>();
			foreach (KeyValuePair<EntityUid, ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>> keyValuePair in this._entityData)
			{
				EntityUid entityUid2;
				ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple3;
				keyValuePair.Deconstruct(out entityUid2, out valueTuple3);
				EntityUid entityUid4 = entityUid2;
				bool flag12 = !this._entityManager.EntityExists(entityUid4);
				if (flag12)
				{
					list.Add(entityUid4);
				}
			}
			foreach (EntityUid entityUid5 in list)
			{
				this._entityData.Remove(entityUid5);
			}
		}
	}
	public void ResetSpriteValues()
	{
		foreach (KeyValuePair<EntityUid, ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>> keyValuePair in this._entityData)
		{
			EntityUid entityUid;
			ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple;
			keyValuePair.Deconstruct(out entityUid, out valueTuple);
			EntityUid entityUid2 = entityUid;
			ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2> valueTuple2 = valueTuple;
			SpriteComponent spriteComponent;
			bool flag = this._entityManager.TryGetComponent<SpriteComponent>(entityUid2, out spriteComponent);
			if (flag)
			{
				spriteComponent.Color = valueTuple2.Item1;
				spriteComponent.Rotation = valueTuple2.Item2;
				spriteComponent.Scale = valueTuple2.Item3;
				spriteComponent.Offset -= valueTuple2.Item6;
				spriteComponent.Offset = Vector2.Zero;
			}
		}
		this._entityData.Clear();
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	private readonly IRobustRandom _random = null;
	private readonly Dictionary<EntityUid, ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>> _entityData = new Dictionary<EntityUid, ValueTuple<Color, Angle, Vector2, List<Vector2>, float, Vector2>>();
	private const int TrailLength = 30;
	private float _time;
	private float _rainbowHue;
}
