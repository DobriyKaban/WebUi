using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Popups;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Timing;


public class DamageNumbersSystem : EntitySystem
{
	public override void Initialize()
	{
		//base.SubscribeLocalEvent<TransformComponent, DamageChangedEvent>(new ComponentEventHandler<TransformComponent, DamageChangedEvent>(this.OnDamageChange), null, null);
	}
	private void OnDamageChange(EntityUid uid, TransformComponent transform, DamageChangedEvent args)
	{
		bool flag = !CerberusConfig.Misc.DamageOverlayEnabled;
		if (!flag)
		{
			FieldInfo field = args.Damageable.GetType().GetField("TotalDamage");
			object value = null;
			bool flag2;
			if (!(field == null))
			{
				value = field.GetValue(args.Damageable);
				flag2 = value == null;
			}
			else
			{
				flag2 = true;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				this._lastDamageValues.Remove(uid);
			}
			else
			{
				FixedPoint2 dmgVal = FixedPoint2.FromObject(value);
				FixedPoint2 thresholdVal;
				bool flag4 = !this._lastDamageValues.TryGetValue(uid, out thresholdVal);
				if (flag4)
				{
					this._lastDamageValues[uid] = dmgVal;
				}
				else
				{
					FixedPoint2 critThreshold = dmgVal - thresholdVal;
					bool flag5 = critThreshold == FixedPoint2.Zero;
					if (!flag5)
					{
						this._lastDamageValues[uid] = dmgVal;
						TimeSpan curTime = this._timing.CurTime;
						TimeSpan timeSpan;
						bool flag6 = this._lastPopupTimes.TryGetValue(uid, out timeSpan) && (curTime - timeSpan).TotalSeconds < 1.0;
						if (!flag6)
						{
							this._lastPopupTimes[uid] = curTime;
							string text = ((critThreshold.RawValue > 0) ? "-" : "+");
							string text2 = FixedPoint2.Abs(critThreshold).ToString();
							EntityCoordinates entityCoordinates = this.GenerateRandomCoordinates(transform.Coordinates);
							this._popupSystem.PopupCoordinates(text + text2, entityCoordinates, 0);
						}
					}
				}
			}
		}
	}
	private EntityCoordinates GenerateRandomCoordinates(EntityCoordinates center)
	{
		double num = this._random.NextDouble() * 2.0 * 3.141592653589793;
		double num2 = this._random.NextDouble() * 0.5;
		float num3 = (float)(Math.Cos(num) * num2);
		float num4 = (float)(Math.Sin(num) * num2);
		Vector2 vector = new Vector2(center.Position.X + num3, center.Position.Y + num4);
		return new EntityCoordinates(center.EntityId, vector);
	}
	
	private readonly PopupSystem _popupSystem = null;
	
	private readonly IRobustRandom _random = null;
	
	private readonly IGameTiming _timing = null;
	private readonly Dictionary<EntityUid, FixedPoint2> _lastDamageValues = new Dictionary<EntityUid, FixedPoint2>();
	private readonly Dictionary<EntityUid, TimeSpan> _lastPopupTimes = new Dictionary<EntityUid, TimeSpan>();
	private const double PopupDelay = 1.0;
}
