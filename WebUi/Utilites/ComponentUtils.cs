using System;
using System.Runtime.CompilerServices;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


[CompilerGenerated]
public class ComponentUtils : EntitySystem
{
	public void AddComponent(string componentName, EntityUid? uid = null)
	{
		bool flag = !this.ComponentExists(componentName);
		if (!flag)
		{
			bool flag2 = uid == null;
			if (flag2)
			{
				bool flag3 = !this.GetPlayer(out uid);
				if (flag3)
				{
					return;
				}
			}
			bool flag4 = !this._entityManager.HasComponent(uid, this._entityManager.ComponentFactory.GetRegistration(componentName, false).Type);
			if (flag4)
			{
				IComponent component = this._entityManager.ComponentFactory.GetComponent(componentName, false);
				component.NetSyncEnabled = false;
				this._entityManager.AddComponent<IComponent>(uid.Value, component, false, null);
			}
		}
	}
	public void RemoveComponent(string componentName, EntityUid? uid = null)
	{
		bool flag = !this.ComponentExists(componentName);
		if (!flag)
		{
			bool flag2 = uid == null;
			if (flag2)
			{
				bool flag3 = !this.GetPlayer(out uid);
				if (flag3)
				{
					return;
				}
			}
			bool flag4 = this._entityManager.HasComponent(uid, this._entityManager.ComponentFactory.GetRegistration(componentName, false).Type);
			if (flag4)
			{
				this._entityManager.RemoveComponent(uid.Value, this._entityManager.ComponentFactory.GetRegistration(componentName, false).Type, null);
			}
		}
	}
	public bool HasComponent(string componentName, EntityUid? uid = null)
	{
		bool flag = !this.ComponentExists(componentName);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = uid == null;
			if (flag3)
			{
				bool flag4 = !this.GetPlayer(out uid);
				if (flag4)
				{
					return false;
				}
			}
			flag2 = this._entityManager.HasComponent(uid.Value, this._entityManager.ComponentFactory.GetRegistration(componentName, false).Type);
		}
		return flag2;
	}
	public bool ComponentExists(string componentName)
	{
		ComponentRegistration componentRegistration;
		return this._entityManager.ComponentFactory.TryGetRegistration(componentName, out componentRegistration, false);
	}
	private bool GetPlayer(out EntityUid? player)
	{
		player = null;
		bool flag = this._playerManager.LocalEntity == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			player = this._playerManager.LocalEntity;
			flag2 = true;
		}
		return flag2;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
}
