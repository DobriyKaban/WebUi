using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


public class AntagDetector : EntitySystem
{
	public bool IsAgent(EntityUid target)
	{
		bool flag = !this.AntagExists("StoreDiscount");
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			BaseContainer baseContainer;
			bool flag3 = !this._containerSystem.TryGetContainer(target, "id", out baseContainer, null);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				foreach (EntityUid entityUid in baseContainer.ContainedEntities)
				{
					bool flag4 = this._componentUtils.HasComponent("StoreDiscount", new EntityUid?(entityUid));
					if (flag4)
					{
						return true;
					}
				}
				flag2 = false;
			}
		}
		return flag2;
	}
	public bool IsHeretic(EntityUid target)
	{
		bool flag = !this.AntagExists("Heretic");
		return !flag && this._componentUtils.HasComponent("Heretic", new EntityUid?(target));
	}
	public bool IsVampire(EntityUid target)
	{
		bool flag = !this.AntagExists("Vampire");
		return !flag && this._componentUtils.HasComponent("Vampire", new EntityUid?(target));
	}
	public bool IsFleshCultist(EntityUid target)
	{
		bool flag = !this.AntagExists("FleshCultist");
		return !flag && this._componentUtils.HasComponent("FleshCultist", new EntityUid?(target));
	}
	public bool IsZeroZombie(EntityUid target)
	{
		bool flag = !this.AntagExists("PendingZombie");
		return !flag && this._componentUtils.HasComponent("PendingZombie", new EntityUid?(target));
	}
	public bool IsChangeling(EntityUid target)
	{
		bool flag = !this.AntagExists("Changeling");
		return !flag && this._componentUtils.HasComponent("Changeling", new EntityUid?(target));
	}
	public bool IsCosmicCult(EntityUid target)
	{
		bool flag = !this.AntagExists("CosmicCult");
		return !flag && this._componentUtils.HasComponent("CosmicCult", new EntityUid?(target));
	}
	public bool IsDevil(EntityUid target)
	{
		bool flag = !this.AntagExists("CheatDeath");
		return !flag && this._componentUtils.HasComponent("CheatDeath", new EntityUid?(target));
	}
	public bool IsBlob(EntityUid target)
	{
		bool flag = !this.AntagExists("BlobCarrier");
		return !flag && this._componentUtils.HasComponent("BlobCarrier", new EntityUid?(target));
	}
	public bool IsThief(EntityUid target)
	{
		ContainerManagerComponent containerManagerComponent;
		bool flag = !base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			foreach (BaseContainer baseContainer in this._containerSystem.GetAllContainers(target, containerManagerComponent))
			{
				bool flag3 = this.ContainsThief(baseContainer);
				if (flag3)
				{
					return true;
				}
			}
			flag2 = false;
		}
		return flag2;
	}
	private bool AntagExists(string name)
	{
		return this._componentUtils.ComponentExists(name);
	}
	
	private bool ContainsThief(BaseContainer container)
	{
		bool flag = container == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			foreach (EntityUid entityUid in container.ContainedEntities)
			{
				MetaDataComponent metaDataComponent;
				bool flag3 = base.TryComp<MetaDataComponent>(entityUid, out metaDataComponent) && metaDataComponent.EntityPrototype != null && metaDataComponent.EntityPrototype.ID.Contains("Thief");
				if (flag3)
				{
					return true;
				}
				ContainerManagerComponent containerManagerComponent;
				bool flag4 = !base.TryComp<ContainerManagerComponent>(entityUid, out containerManagerComponent);
				if (!flag4)
				{
					foreach (BaseContainer baseContainer in this._containerSystem.GetAllContainers(entityUid, containerManagerComponent))
					{
						bool flag5 = this.ContainsThief(baseContainer);
						if (flag5)
						{
							return true;
						}
					}
				}
			}
			flag2 = false;
		}
		return flag2;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly SharedContainerSystem _containerSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly ComponentUtils _componentUtils = null;
}
