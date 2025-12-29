using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;


[CompilerGenerated]
public sealed class PlayerTrackerSystem : EntitySystem
{
	public event Action<ICommonSession> OnPlayerJoined
	{
		[CompilerGenerated]
		add
		{
			Action<ICommonSession> action = this.PlayerJoinedEvent;
			Action<ICommonSession> action2;
			do
			{
				action2 = action;
				Action<ICommonSession> action3 = (Action<ICommonSession>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<ICommonSession>>(ref this.PlayerJoinedEvent, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<ICommonSession> action = this.PlayerJoinedEvent;
			Action<ICommonSession> action2;
			do
			{
				action2 = action;
				Action<ICommonSession> action3 = (Action<ICommonSession>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<ICommonSession>>(ref this.PlayerJoinedEvent, action3, action2);
			}
			while (action != action2);
		}
	}
	public event Action<ICommonSession> OnPlayerLeft
	{
		[CompilerGenerated]
		add
		{
			Action<ICommonSession> action = this.PlayerLeftEvent;
			Action<ICommonSession> action2;
			do
			{
				action2 = action;
				Action<ICommonSession> action3 = (Action<ICommonSession>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<ICommonSession>>(ref this.PlayerLeftEvent, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<ICommonSession> action = this.PlayerLeftEvent;
			Action<ICommonSession> action2;
			do
			{
				action2 = action;
				Action<ICommonSession> action3 = (Action<ICommonSession>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<ICommonSession>>(ref this.PlayerLeftEvent, action3, action2);
			}
			while (action != action2);
		}
	}
	public Dictionary<NetUserId, PlayerData> AllPlayerSessions { get; }
	public override void Initialize()
	{
		this._playerManager.PlayerListUpdated += this.OnPlayerListUpdated;
	}
	public override void Shutdown()
	{
		this._playerManager.PlayerListUpdated -= this.OnPlayerListUpdated;
	}
	public override void Update(float frameTime)
	{
		this.UpdateSessionDetails();
	}
	private void UpdateSessionDetails()
	{
		foreach (PlayerData playerData in this.AllPlayerSessions.Values)
		{
			bool flag = playerData.Status == "Offline";
			if (!flag)
			{
				EntityUid? attachedEntity = playerData.Session.AttachedEntity;
				playerData.AttachedEntity = attachedEntity;
				bool flag2 = attachedEntity == null;
				if (flag2)
				{
					playerData.EntityName = "Unknown";
					playerData.Job = "Unknown";
					playerData.LastKnownPosition = Vector2.Zero;
				}
				else
				{
					EntityUid value = attachedEntity.Value;
					TransformComponent transformComponent;
					MetaDataComponent metaDataComponent = null;
					bool flag3 = !base.TryComp(value, out transformComponent) || !base.TryComp(value, out metaDataComponent);
					if (flag3)
					{
						playerData.EntityName = "Unknown";
						playerData.Job = "Unknown";
						playerData.LastKnownPosition = Vector2.Zero;
					}
					else
					{
						bool flag4 = transformComponent.LocalPosition == Vector2.Zero;
						if (flag4)
						{
							playerData.IsVisible = false;
						}
						else
						{
							playerData.IsVisible = true;
							playerData.EntityName = metaDataComponent.EntityName;
							playerData.LastKnownPosition = transformComponent.LocalPosition;
							Entity<IdCardComponent> entity;
							bool flag5 = this._idCardSystem.TryFindIdCard(value, out entity);
							if (flag5)
							{
								string text;
								if ((text = entity.Comp.LocalizedJobTitle) == null)
								{
									text = Regex.Match(base.MetaData(entity).EntityName ?? "", "\\(([^)]*)\\)").Groups[1].Value;
								}
								string text2 = text;
								playerData.Job = ((!string.IsNullOrEmpty(text2)) ? text2 : "Unknown");
							}
							else
							{
								playerData.Job = "Unknown";
							}
						}
					}
				}
			}
		}
	}
	private void OnPlayerListUpdated()
	{
		IReadOnlyDictionary<NetUserId, ICommonSession> sessionsDict = this._playerManager.SessionsDict;
		Dictionary<NetUserId, ICommonSession> dictionary = new Dictionary<NetUserId, ICommonSession>(sessionsDict);
		bool flag = this._cachedSessions == null;
		if (flag)
		{
			foreach (KeyValuePair<NetUserId, ICommonSession> keyValuePair in dictionary)
			{
				NetUserId netUserId;
				ICommonSession commonSession;
				keyValuePair.Deconstruct(out netUserId, out commonSession);
				NetUserId netUserId2 = netUserId;
				ICommonSession commonSession2 = commonSession;
				bool flag2 = !this.AllPlayerSessions.ContainsKey(netUserId2);
				if (flag2)
				{
					this.AllPlayerSessions.Add(netUserId2, new PlayerData(commonSession2));
				}
			}
			this._cachedSessions = dictionary;
			this.UpdateSessionDetails();
		}
		else
		{
			List<NetUserId> list = this._cachedSessions.Keys.Except(dictionary.Keys).ToList<NetUserId>();
			foreach (NetUserId netUserId3 in list)
			{
				ICommonSession commonSession3;
				bool flag3 = this._cachedSessions.TryGetValue(netUserId3, out commonSession3);
				if (flag3)
				{
					Action<ICommonSession> onLeave = this.PlayerLeftEvent;
					if (onLeave != null)
					{
						onLeave(commonSession3);
					}
				}
				PlayerData playerData;
				bool flag4 = this.AllPlayerSessions.TryGetValue(netUserId3, out playerData);
				if (flag4)
				{
					playerData.Status = "Offline";
				}
			}
			List<NetUserId> list2 = dictionary.Keys.Where(delegate(NetUserId k)
			{
				PlayerData data;
				return !this._cachedSessions.ContainsKey(k) || (this.AllPlayerSessions.TryGetValue(k, out data) && data.Status == "Offline");
			}).ToList<NetUserId>();
			foreach (NetUserId netUserId4 in list2)
			{
				ICommonSession commonSession4;
				bool flag5 = dictionary.TryGetValue(netUserId4, out commonSession4);
				if (flag5)
				{
					bool flag6 = false;
					PlayerData existingData;
					bool flag7 = this.AllPlayerSessions.TryGetValue(netUserId4, out existingData);
					if (flag7)
					{
						flag6 = existingData.Status == "Offline";
						existingData.Session = commonSession4;
						existingData.Status = "Online";
					}
					else
					{
						this.AllPlayerSessions.Add(netUserId4, new PlayerData(commonSession4));
					}
					bool flag8 = flag6 || !this._cachedSessions.ContainsKey(netUserId4);
					if (flag8)
					{
						Action<ICommonSession> onJoin = this.PlayerJoinedEvent;
						if (onJoin != null)
						{
							onJoin(commonSession4);
						}
					}
				}
			}
			this._cachedSessions = dictionary;
		}
	}
	public PlayerTrackerSystem()
	{
		this.AllPlayerSessions = new Dictionary<NetUserId, PlayerData>();
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedIdCardSystem _idCardSystem;
	private IReadOnlyDictionary<NetUserId, ICommonSession> _cachedSessions;
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<ICommonSession> PlayerJoinedEvent;
	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Action<ICommonSession> PlayerLeftEvent;
}
