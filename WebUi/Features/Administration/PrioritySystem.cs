using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Mobs.Components;
using Content.Shared.Verbs;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;


[CompilerGenerated]
public sealed class PrioritySystem : EntitySystem
{
	public override void Initialize()
	{
		base.SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(new EntityEventHandler<GetVerbsEvent<AlternativeVerb>>(this.AddPriorityVerb), null, null);
	}
	public bool IsPriority(EntityUid entity)
	{
		ICommonSession localSession = this._playerManager.LocalSession;
		bool flag = localSession == null;
		HashSet<EntityUid> hashSet;
		return !flag && this._priorityPlayers.TryGetValue(localSession, out hashSet) && hashSet.Contains(entity);
	}
	private void AddPriorityVerb(GetVerbsEvent<AlternativeVerb> ev)
	{
		bool flag = ev.User == ev.Target;
		if (!flag)
		{
			bool flag2 = !base.HasComp<MobStateComponent>(ev.Target);
			if (!flag2)
			{
				bool flag3 = this._playerManager.LocalEntity == null;
				if (!flag3)
				{
					bool flag4 = ev.User != this._playerManager.LocalEntity.Value;
					if (!flag4)
					{
						ICommonSession localSession = this._playerManager.LocalSession;
						bool flag5 = localSession == null;
						if (!flag5)
						{
							HashSet<EntityUid> priorities;
							bool flag6 = !this._priorityPlayers.TryGetValue(localSession, out priorities);
							if (flag6)
							{
								priorities = new HashSet<EntityUid>();
								this._priorityPlayers[localSession] = priorities;
							}
							string text = (priorities.Contains(ev.Target) ? "Delete priority" : "Make priority");
							AlternativeVerb alternativeVerb = new AlternativeVerb
							{
								Act = delegate
								{
									bool flag7 = !priorities.Add(ev.Target);
									if (flag7)
									{
										priorities.Remove(ev.Target);
									}
								},
								Text = text,
								Priority = 200
							};
							ev.Verbs.Add(alternativeVerb);
						}
					}
				}
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	private readonly Dictionary<ICommonSession, HashSet<EntityUid>> _priorityPlayers = new Dictionary<ICommonSession, HashSet<EntityUid>>();
}
