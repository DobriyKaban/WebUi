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
public sealed class FriendSystem : EntitySystem
{
	public override void Initialize()
	{
		base.SubscribeLocalEvent<GetVerbsEvent<AlternativeVerb>>(new EntityEventHandler<GetVerbsEvent<AlternativeVerb>>(this.AddFriendVerb), null, null);
	}
	public bool IsFriend(EntityUid entity)
	{
		ICommonSession localSession = this._playerManager.LocalSession;
		bool flag = localSession == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			HashSet<EntityUid> hashSet;
			bool flag3 = !this._friendPlayers.TryGetValue(localSession, out hashSet);
			flag2 = !flag3 && hashSet.Contains(entity);
		}
		return flag2;
	}
	private void AddFriendVerb(GetVerbsEvent<AlternativeVerb> ev)
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
							HashSet<EntityUid> friends;
							bool flag6 = !this._friendPlayers.TryGetValue(localSession, out friends);
							if (flag6)
							{
								friends = new HashSet<EntityUid>();
								this._friendPlayers[localSession] = friends;
							}
							string text = (friends.Contains(ev.Target) ? "Delete friend" : "Make friend");
							AlternativeVerb alternativeVerb = new AlternativeVerb
							{
								Act = delegate
								{
									bool flag7 = !friends.Add(ev.Target);
									if (flag7)
									{
										friends.Remove(ev.Target);
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
	private readonly Dictionary<ICommonSession, HashSet<EntityUid>> _friendPlayers = new Dictionary<ICommonSession, HashSet<EntityUid>>();
}
