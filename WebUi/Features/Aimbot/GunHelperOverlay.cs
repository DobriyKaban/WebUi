using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CerberusWareV3.Configuration;
using Content.Client.Hands.Systems;
using Content.Client.Verbs;
using Content.Client.Weapons.Ranged.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Content.Client.Inventory;


public sealed class GunHelperOverlay : Overlay
{
	[Robust.Shared.IoC.Dependency] private readonly IResourceCache _resourceCache = null;

	public GunHelperOverlay()
	{
		IoCManager.InjectDependencies(this);
		base.ZIndex = new int?(200);
		if (this._font == null)
		{
			this._font = new VectorFont(this._resourceCache.GetResource<FontResource>("/Fonts/Boxfont-round/Boxfont Round.ttf", true), 10);
		}
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)2;
		}
	}
	private static TimeSpan ReloadDelay
	{
		get
		{
			return TimeSpan.FromSeconds((double)CerberusConfig.GunHelper.AutoReloadDelay);
		}
	}
	private void ResetReloadState()
	{
		var previousState = _reloadState;
		_reloadState = AutoReloadMagazineState.Idle;
		_currentMag = null;
		_oldMag = null;
		_currentGun = null;
		_originalHand = null;
		_actionPerformed = false;
		_foundMagInfo = new FoundMagazineInfo();
		_containerSlotId = null;
		_lastStateChange = _timing.CurTime;
		if (previousState > AutoReloadMagazineState.Idle) _lastReloadAttempt = _timing.CurTime;
	}
	private void SetReloadState(AutoReloadMagazineState newState)
    {
        if (_reloadState != newState)
        {
            _reloadState = newState;
            _lastStateChange = _timing.CurTime;
            _actionPerformed = false;
        }
    }

    private bool CanPerformAction() => _timing.CurTime >= _lastActionTime + ReloadDelay;

    private void RecordActionAttempt()
    {
        _lastActionTime = _timing.CurTime;
        _actionPerformed = true;
        if (_reloadState != AutoReloadMagazineState.Idle && _reloadState != AutoReloadMagazineState.CheckingNeed)
            _lastReloadAttempt = _timing.CurTime;
    }

    private void InitializeSystems()
    {
        if (_verbSystem == null) _verbSystem = _sysMan.GetEntitySystem<VerbSystem>();
        if (_handsSystem == null) _handsSystem = _sysMan.GetEntitySystem<SharedHandsSystem>();
        if (_storageSystem == null) _storageSystem = _sysMan.GetEntitySystem<SharedStorageSystem>();
        if (_inventorySystem == null) _inventorySystem = _sysMan.GetEntitySystem<InventorySystem>();
        if (_containerSystem == null) _containerSystem = _sysMan.GetEntitySystem<SharedContainerSystem>();
        if (_whitelistSystem == null) _whitelistSystem = _sysMan.GetEntitySystem<EntityWhitelistSystem>();
        if (_gunSystem == null) _gunSystem = _sysMan.GetEntitySystem<GunSystem>();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (!CerberusConfig.GunHelper.Enabled)
        {
            if (_reloadState > AutoReloadMagazineState.Idle) ResetReloadState();
            return;
        }

        InitializeSystems();
        var localEntity = _playerManager.LocalEntity;
        
        if (localEntity == null || _gunSystem == null)
        {
            if (_reloadState > AutoReloadMagazineState.Idle) ResetReloadState();
            return;
        }

        _actionPerformed = false;
        EntityUid? heldGunUid = null;
        GunComponent heldGunComp = null;

        // Ищем пушку в руках
        if (_handsSystem.TryGetActiveItem(localEntity.Value, out var activeItem) && 
            _entityManager.TryGetComponent<GunComponent>(activeItem.Value, out var gunComp))
        {
            heldGunUid = activeItem.Value;
            heldGunComp = gunComp;
        }

        // Показ патронов и Авто-затвор
        if (heldGunUid != null)
        {
            DisplayAmmoInfo(in args, heldGunUid.Value);

            if (CerberusConfig.GunHelper.AutoBolt)
            {
                HandleAutoBoltClose(localEntity.Value, heldGunUid.Value);
                if (!_actionPerformed) HandleAutoChamberRack(localEntity.Value, heldGunUid.Value);
            }
        }

        // Авто-перезарядка
        if (CerberusConfig.GunHelper.AutoReload)
        {
            EntityUid? gunToReload = null;
            
            // Если уже перезаряжаем, продолжаем с той же пушкой
            if (_reloadState != AutoReloadMagazineState.Idle && _currentGun != null && _entityManager.EntityExists(_currentGun.Value))
            {
                gunToReload = _currentGun;
            }
            // Иначе берем текущую в руках
            else if (heldGunUid != null)
            {
                gunToReload = heldGunUid;
            }

            if (gunToReload != null && !_actionPerformed)
            {
                HandleAutoMagazineReload(localEntity.Value, gunToReload.Value);
            }
            else if (_reloadState != AutoReloadMagazineState.Idle && gunToReload == null)
            {
                ResetReloadState();
            }
        }
        else if (_reloadState > AutoReloadMagazineState.Idle)
        {
            ResetReloadState();
        }
    }
    private void DisplayAmmoInfo(in OverlayDrawArgs args, EntityUid gunUid)
    {
	    if (!CerberusConfig.GunHelper.ShowAmmo || _font == null) return;

	    var ev = new GetAmmoCountEvent();
	    _entityManager.EventBus.RaiseLocalEvent(gunUid, ref ev);

	    var mousePos = _inputManager.MouseScreenPosition;
	    var drawPos = new Vector2(mousePos.Position.X + 10f, mousePos.Position.Y + 35f);
        
	    args.ScreenHandle.DrawString(_font, drawPos, $"{ev.Count}/{ev.Capacity}", Color.Red);
    }
	private unsafe void HandleAutoBoltClose(EntityUid playerUid, EntityUid gunUid)
	{
		bool flag = !this.CanPerformAction();
		if (!flag)
		{
			ChamberMagazineAmmoProviderComponent chamberMagazineAmmoProviderComponent;
			bool flag2 = !this._entityManager.TryGetComponent<ChamberMagazineAmmoProviderComponent>(gunUid, out chamberMagazineAmmoProviderComponent) || this._verbSystem == null || this._containerSystem == null;
			if (!flag2)
			{
				BaseContainer baseContainer;
				bool flag3 = this._containerSystem.TryGetContainer(gunUid, "gun_chamber", out baseContainer, null) && baseContainer.ContainedEntities.Count > 0;
				bool flag4 = false;
				bool flag5 = !flag3;
				if (flag5)
				{
					BaseContainer baseContainer2;
					bool flag6 = this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer2, null) && baseContainer2.ContainedEntities.Any<EntityUid>();
					if (flag6)
					{
						EntityUid entityUid = baseContainer2.ContainedEntities.First<EntityUid>();
						GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
						this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(entityUid, ref getAmmoCountEvent, false);
						bool flag7 = getAmmoCountEvent.Count > 0;
						if (flag7)
						{
							flag4 = true;
						}
					}
				}
				bool flag8 = !chamberMagazineAmmoProviderComponent.BoltClosed.GetValueOrDefault() && (flag3 || flag4);
				bool flag9 = flag8;
				if (flag9)
				{
					string boltCloseVerbText = Loc.GetString("gun-chamber-bolt-close");
					SharedVerbSystem verbSys = this._verbSystem;
					int num = 2;
					List<Type> list = new List<Type>(num);
					CollectionsMarshal.SetCount<Type>(list, num);
					Span<Type> span = CollectionsMarshal.AsSpan<Type>(list);
					int num2 = 0;
					span[num2] = typeof(Verb);
					num2++;
					span[num2] = typeof(InteractionVerb);
					SortedSet<Verb> localVerbs = verbSys.GetLocalVerbs(gunUid, playerUid, list, false);
					Verb verb = localVerbs.FirstOrDefault((Verb v) => v.Text == boltCloseVerbText && !v.Disabled);
					bool flag10 = verb != null;
					if (flag10)
					{
						this._entityManager.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(this._entityManager.GetNetEntity(gunUid, null), verb));
						this.RecordActionAttempt();
					}
				}
			}
		}
	}
	private void HandleAutoChamberRack(EntityUid playerUid, EntityUid gunUid)
	{
		bool flag = !this.CanPerformAction();
		if (!flag)
		{
			bool flag2 = this._timing.CurTime <= this._lastRackAttempt + TimeSpan.FromSeconds(0.5);
			if (!flag2)
			{
				ChamberMagazineAmmoProviderComponent chamberMagazineAmmoProviderComponent;
				bool flag3 = !this._entityManager.TryGetComponent<ChamberMagazineAmmoProviderComponent>(gunUid, out chamberMagazineAmmoProviderComponent) || this._verbSystem == null || this._containerSystem == null;
				if (!flag3)
				{
					BaseContainer baseContainer;
					bool flag4 = !this._containerSystem.TryGetContainer(gunUid, "gun_chamber", out baseContainer, null) || !baseContainer.ContainedEntities.Any<EntityUid>();
					bool flag5 = !flag4;
					if (!flag5)
					{
						bool flag6 = !chamberMagazineAmmoProviderComponent.BoltClosed.GetValueOrDefault();
						if (!flag6)
						{
							bool flag7 = false;
							BaseContainer baseContainer2;
							bool flag8 = this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer2, null) && baseContainer2.ContainedEntities.Any<EntityUid>();
							if (flag8)
							{
								EntityUid entityUid = baseContainer2.ContainedEntities.First<EntityUid>();
								GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
								this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(entityUid, ref getAmmoCountEvent, false);
								bool flag9 = getAmmoCountEvent.Count > 0;
								if (flag9)
								{
									flag7 = true;
								}
							}
							bool flag10 = !flag7;
							if (!flag10)
							{
								SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(gunUid, playerUid, typeof(Verb), false);
								Verb verb = localVerbs.FirstOrDefault((Verb v) => v.Text == Loc.GetString("gun-chamber-rack") && !v.Disabled);
								bool flag11 = verb != null;
								if (flag11)
								{
									this._entityManager.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(this._entityManager.GetNetEntity(gunUid, null), verb));
									this._lastRackAttempt = this._timing.CurTime;
									this.RecordActionAttempt();
								}
							}
						}
					}
				}
			}
		}
	}
	private void HandleAutoMagazineReload(EntityUid playerUid, EntityUid gunUid)
	{
		bool flag = this._reloadState != GunHelperOverlay.AutoReloadMagazineState.Idle && this._timing.CurTime > this._lastStateChange + TimeSpan.FromSeconds((double)this.TimeoutSeconds);
		if (flag)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = this._reloadState == GunHelperOverlay.AutoReloadMagazineState.Idle && this._timing.CurTime < this._lastReloadAttempt + TimeSpan.FromSeconds((double)CerberusConfig.GunHelper.AutoReloadDelay);
			if (!flag2)
			{
				bool flag3 = !this.CanPerformAction() && this._reloadState != GunHelperOverlay.AutoReloadMagazineState.Idle && this._reloadState != GunHelperOverlay.AutoReloadMagazineState.CheckingNeed && this._reloadState != GunHelperOverlay.AutoReloadMagazineState.FindingNewMagazine && this._reloadState != GunHelperOverlay.AutoReloadMagazineState.WaitingForEject && this._reloadState != GunHelperOverlay.AutoReloadMagazineState.WaitingForInsertConfirm;
				if (!flag3)
				{
					switch (this._reloadState)
					{
					case GunHelperOverlay.AutoReloadMagazineState.Idle:
					case GunHelperOverlay.AutoReloadMagazineState.CheckingNeed:
						this.ExecuteCheckingNeed(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.FindingNewMagazine:
						this.ExecuteFindingNewMagazine(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.SwitchingToNewMagazineHand:
						this.ExecuteSwitchingToNewMagazineHand(playerUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.RequestingOpenContainerSlot:
						this.ExecuteRequestingOpenContainerSlot();
						break;
					case GunHelperOverlay.AutoReloadMagazineState.SwitchingToFreeHandForContainerTake:
						this.ExecuteSwitchingToFreeHandForContainerTake(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.TakingMagazineFromOpenedContainer:
						this.ExecuteTakingMagazineFromOpenedContainer(playerUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.PreparingToTakeFromPocketSlot:
						this.ExecutePreparingToTakeFromPocketSlot(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.TakingDirectlyFromPocketSlot:
						this.ExecuteTakingDirectlyFromPocketSlot(playerUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.NewMagazineInHand:
						this.ExecuteNewMagazineInHand(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.EjectingOldMagazine:
						this.ExecuteEjectingOldMagazine(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.WaitingForEject:
						this.ExecuteWaitingForEject(gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.ReadyToInsertNewMagazine:
						this.ExecuteReadyToInsertNewMagazine(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.WaitingForInsertConfirm:
						this.ExecuteWaitingForInsertConfirm(playerUid, gunUid);
						break;
					case GunHelperOverlay.AutoReloadMagazineState.SwitchingBackToGunHand:
						this.ExecuteSwitchingBackToGunHand(playerUid);
						break;
					}
				}
			}
		}
	}
	private void ExecuteCheckingNeed(EntityUid playerUid, EntityUid gunUidFromContext)
	{
		this._lastReloadAttempt = this._timing.CurTime;
		bool flag = false;
		this._oldMag = null;
		BaseContainer baseContainer;
		bool flag2 = this._containerSystem.TryGetContainer(gunUidFromContext, "gun_magazine", out baseContainer, null);
		if (flag2)
		{
			bool flag3 = !baseContainer.ContainedEntities.Any<EntityUid>();
			if (flag3)
			{
				flag = true;
			}
			else
			{
				this._oldMag = new EntityUid?(baseContainer.ContainedEntities.First<EntityUid>());
				GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
				this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(this._oldMag.Value, ref getAmmoCountEvent, false);
				bool flag4 = getAmmoCountEvent.Count == 0;
				if (flag4)
				{
					flag = true;
				}
			}
			bool flag5 = !flag;
			if (flag5)
			{
				bool flag6 = this._currentGun == gunUidFromContext || this._reloadState > GunHelperOverlay.AutoReloadMagazineState.Idle;
				if (flag6)
				{
					this.ResetReloadState();
				}
			}
			else
			{
				this._currentGun = new EntityUid?(gunUidFromContext);
				this._originalHand = null;
				HandsComponent handsComponent;
				bool flag7 = this._entityManager.TryGetComponent<HandsComponent>(playerUid, out handsComponent);
				if (flag7)
				{
					foreach (var handName in handsComponent.Hands.Keys)
					{
						string text2 = handName;
						var heldItem = this._handsSystem.GetHeldItem(playerUid, handName);
						bool flag8 = heldItem == this._currentGun;
						if (flag8)
						{
							this._originalHand = text2;
							break;
						}
					}
				}
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.FindingNewMagazine);
			}
		}
		else
		{
			bool flag9 = this._currentGun == gunUidFromContext;
			if (flag9)
			{
				this.ResetReloadState();
			}
		}
	}
	private bool CanFreeHandToRetrieve(EntityUid playerUid, EntityUid gunBeingReloaded)
	{
		HandsComponent handsComponent;
		bool flag = !this._entityManager.TryGetComponent<HandsComponent>(playerUid, out handsComponent) || this._handsSystem == null;
		
		if (flag) return false;

		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		
		foreach (var handName in handsComponent.Hands.Keys)
		{
			var heldItem = this._handsSystem.GetHeldItem(playerUid, handName);
			
			if (heldItem == gunBeingReloaded)
			{
				foreach (var otherHandName in handsComponent.Hands.Keys)
				{
					if (otherHandName == handName) continue;
					
					var otherHeld = this._handsSystem.GetHeldItem(playerUid, otherHandName);
					if (otherHeld == null || this.IsVirtualItem(otherHeld.Value))
					{
						return true;
					}
				}
				return false;
			}
		}
		

		foreach (var handName in handsComponent.Hands.Keys)
		{
			var heldItem = this._handsSystem.GetHeldItem(playerUid, handName);
			if (heldItem == null || this.IsVirtualItem(heldItem.Value))
				return true;
		}

		return false;
	}
	private void ExecuteFindingNewMagazine(EntityUid playerUid, EntityUid gunUid)
	{
		bool flag = !this.TryFindMagazineForReload(playerUid, gunUid, out this._foundMagInfo) || (this._foundMagInfo.IsValid && ((long)this._timing.CurTime.TotalMilliseconds ^ (long)this._foundMagInfo.MagazineUid.GetHashCode()) % 300007L == 21L);
		if (flag)
		{
			this.ResetReloadState();
		}
		else
		{
			this._currentMag = new EntityUid?(this._foundMagInfo.MagazineUid);
			GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
			this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(this._foundMagInfo.MagazineUid, ref getAmmoCountEvent, false);
			bool flag2 = getAmmoCountEvent.Count <= 0;
			if (flag2)
			{
				bool flag3 = false;
				BaseContainer baseContainer;
				bool flag4 = this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null);
				if (flag4)
				{
					bool flag5 = !baseContainer.ContainedEntities.Any<EntityUid>();
					if (flag5)
					{
						flag3 = true;
					}
					else
					{
						EntityUid entityUid = baseContainer.ContainedEntities.First<EntityUid>();
						GetAmmoCountEvent getAmmoCountEvent2 = default(GetAmmoCountEvent);
						this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(entityUid, ref getAmmoCountEvent2, false);
						bool flag6 = getAmmoCountEvent2.Count == 0;
						if (flag6)
						{
							flag3 = true;
						}
					}
				}
				else
				{
					flag3 = true;
				}
				bool flag7 = flag3;
				if (flag7)
				{
					this.ResetReloadState();
					return;
				}
			}
			bool flag8 = this._foundMagInfo.SourceType == GunHelperOverlay.MagazineSource.Container || this._foundMagInfo.SourceType == GunHelperOverlay.MagazineSource.DirectInventorySlot;
			if (flag8)
			{
				bool flag9 = !this.CanFreeHandToRetrieve(playerUid, gunUid);
				if (flag9)
				{
					this.ResetReloadState();
					return;
				}
			}
			switch (this._foundMagInfo.SourceType)
			{
			case GunHelperOverlay.MagazineSource.Hand:
			{
				var activeHandId = this._handsSystem.GetActiveHand(playerUid);
				if (activeHandId == null)
				{
					this.ResetReloadState();
				}
				else
				{
					var heldItem = this._handsSystem.GetActiveItem(playerUid);
					this.SetReloadState((heldItem == this._currentMag) ? GunHelperOverlay.AutoReloadMagazineState.NewMagazineInHand : GunHelperOverlay.AutoReloadMagazineState.SwitchingToNewMagazineHand);
				}
				break;
			}
			case GunHelperOverlay.MagazineSource.Container:
			{
				this._containerSlotId = this._foundMagInfo.ContainerParentSlotId;
				bool flag11 = string.IsNullOrEmpty(this._containerSlotId) || this._foundMagInfo.ContainerEntityUid == null;
				if (flag11)
				{
					this.ResetReloadState();
				}
				else
				{
					this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.RequestingOpenContainerSlot);
				}
				break;
			}
			case GunHelperOverlay.MagazineSource.DirectInventorySlot:
			{
				bool flag12 = string.IsNullOrEmpty(this._foundMagInfo.DirectSlotId);
				if (flag12)
				{
					this.ResetReloadState();
				}
				else
				{
					this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.PreparingToTakeFromPocketSlot);
				}
				break;
			}
			default:
				this.ResetReloadState();
				break;
			}
		}
	}
	
	private void TryRequestHandSwitch(string targetHandName)
	{
		this._entityManager.RaisePredictiveEvent<RequestSetHandEvent>(new RequestSetHandEvent(targetHandName));
		this.RecordActionAttempt();
	}
	
	private string FindOtherFreeHandName(EntityUid playerUid, string currentActiveHandName)
	{
		HandsComponent handsComponent;
		bool flag = this._entityManager.TryGetComponent<HandsComponent>(playerUid, out handsComponent);
		string text;
		if (flag)
		{
			text = (from h in handsComponent.Hands.Keys
				where h != currentActiveHandName
				let held = this._handsSystem.GetHeldItem(playerUid, h)
				where held == null || this.IsVirtualItem(held.Value)
				select h).FirstOrDefault();
		}
		else
		{
			text = null;
		}
		return text;
	}
	private void ExecuteSwitchingToNewMagazineHand(EntityUid playerUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? activeItem = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = activeItem == this._currentMag;
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.NewMagazineInHand);
				this.RecordActionAttempt();
			}
			else
			{
				string handName = this._foundMagInfo.HandName;
				bool flag3 = handName != null && activeHandName != handName;
				if (flag3)
				{
					this.TryRequestHandSwitch(handName);
				}
				else
				{
					bool flag4 = handName == null && this._currentMag != null;
					if (flag4)
					{
						this.ResetReloadState();
					}
					else
					{
						bool flag5 = handName != null && activeHandName == handName && activeItem != this._currentMag;
						if (flag5)
						{
							this.ResetReloadState();
						}
					}
				}
			}
		}
	}
	private void ExecuteRequestingOpenContainerSlot()
	{
		bool flag = string.IsNullOrEmpty(this._containerSlotId) || this._foundMagInfo.ContainerEntityUid == null;
		if (flag)
		{
			this.ResetReloadState();
		}
		else
		{
			this._entityManager.RaisePredictiveEvent<OpenSlotStorageNetworkMessage>(new OpenSlotStorageNetworkMessage(this._containerSlotId));
			this.RecordActionAttempt();
			this._lastStateChange = this._timing.CurTime;
			this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.SwitchingToFreeHandForContainerTake);
		}
	}
	private void ExecuteSwitchingToFreeHandForContainerTake(EntityUid playerUid, EntityUid gunUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = heldEntity == null || this.IsVirtualItem(heldEntity.Value);
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.TakingMagazineFromOpenedContainer);
				this.RecordActionAttempt();
			}
			else
			{
				bool flag3 = heldEntity == gunUid;
				if (flag3)
				{
					string text = this.FindOtherFreeHandName(playerUid);
					bool flag4 = text != null;
					if (flag4)
					{
						this.TryRequestHandSwitch(text);
					}
					else
					{
						this.ResetReloadState();
					}
				}
				else
				{
					this.ResetReloadState();
				}
			}
		}
	}

	private string FindOtherFreeHandName(EntityUid playerUid)
	{
		return FindOtherFreeHandName(playerUid, this._handsSystem.GetActiveHand(playerUid));
	}

	private void ExecuteTakingMagazineFromOpenedContainer(EntityUid playerUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = heldEntity == this._currentMag;
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.NewMagazineInHand);
				this.RecordActionAttempt();
			}
			else
			{
				bool flag3 = this._currentMag == null || !this._entityManager.EntityExists(this._currentMag.Value) || this._foundMagInfo.ContainerEntityUid == null || !this._entityManager.EntityExists(this._foundMagInfo.ContainerEntityUid.Value);
				if (flag3)
				{
					this.ResetReloadState();
				}
				else
				{
					NetEntity netEntity = this._entityManager.GetNetEntity(this._currentMag.Value, null);
					NetEntity netEntity2 = this._entityManager.GetNetEntity(this._foundMagInfo.ContainerEntityUid.Value, null);
					bool flag4 = netEntity == NetEntity.Invalid || netEntity2 == NetEntity.Invalid;
					if (flag4)
					{
						this.ResetReloadState();
					}
					else
					{
						bool flag5 = heldEntity == null || this.IsVirtualItem(heldEntity.Value);
						if (flag5)
						{
							this._entityManager.RaisePredictiveEvent<StorageInteractWithItemEvent>(new StorageInteractWithItemEvent(netEntity, netEntity2));
							this.RecordActionAttempt();
							this._lastStateChange = this._timing.CurTime;
						}
						else
						{
							bool flag6 = heldEntity != this._currentMag;
							if (flag6)
							{
								this.ResetReloadState();
							}
						}
					}
				}
			}
		}
	}
	private void ExecutePreparingToTakeFromPocketSlot(EntityUid playerUid, EntityUid gunUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = heldEntity == null || this.IsVirtualItem(heldEntity.Value);
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.TakingDirectlyFromPocketSlot);
				this.RecordActionAttempt();
			}
			else
			{
				bool flag3 = heldEntity == gunUid;
				if (flag3)
				{
					string text = this.FindOtherFreeHandName(playerUid);
					bool flag4 = text != null;
					if (flag4)
					{
						this.TryRequestHandSwitch(text);
					}
					else
					{
						this.ResetReloadState();
					}
				}
				else
				{
					this.ResetReloadState();
				}
			}
		}
	}
	private void ExecuteTakingDirectlyFromPocketSlot(EntityUid playerUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null)
		{
			this.ResetReloadState();
		}
		else
		{
			bool flag2 = heldEntity == this._currentMag;
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.NewMagazineInHand);
				this.RecordActionAttempt();
			}
			else
			{
				bool flag3 = string.IsNullOrEmpty(this._foundMagInfo.DirectSlotId) || this._currentMag == null;
				if (flag3)
				{
					this.ResetReloadState();
				}
				else
				{
					bool flag4 = heldEntity != null && !this.IsVirtualItem(heldEntity.Value);
					if (flag4)
					{
						this.ResetReloadState();
					}
					else
					{
						this._entityManager.RaisePredictiveEvent<UseSlotNetworkMessage>(new UseSlotNetworkMessage(this._foundMagInfo.DirectSlotId));
						this.RecordActionAttempt();
						this._lastStateChange = this._timing.CurTime;
					}
				}
			}
		}
	}
	private void ExecuteNewMagazineInHand(EntityUid playerUid, EntityUid gunUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null || heldEntity == null || heldEntity != this._currentMag)
		{
			this.ResetReloadState();
		}
		else
		{
			this.RecordActionAttempt();
			BaseContainer baseContainer;
			bool flag2 = this._oldMag != null && this._entityManager.EntityExists(this._oldMag.Value) && this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null) && baseContainer.ContainedEntities.Contains(this._oldMag.Value);
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.EjectingOldMagazine);
			}
			else
			{
				this._oldMag = null;
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.ReadyToInsertNewMagazine);
			}
		}
	}
	private Verb TryFindEjectVerb(EntityUid gunUid, EntityUid playerUid, EntityUid oldMagazineUid)
	{
		SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(gunUid, playerUid, typeof(AlternativeVerb), false);
		NetEntity oldMagNet = this._entityManager.GetNetEntity(oldMagazineUid, null);
		Verb verb = localVerbs.FirstOrDefault(delegate(Verb v)
		{
			if (v.Text == "Magazine")
			{
				VerbCategory category = v.Category;
				if (((category != null) ? category.Text : null) == Loc.GetString("verb-categories-eject") && !v.Disabled)
				{
					return v.IconEntity == oldMagNet;
				}
			}
			return false;
		});
		Verb verb2;
		if ((verb2 = verb) == null)
		{
			verb2 = localVerbs.FirstOrDefault(delegate(Verb v)
			{
				if (v.Text == "Magazine")
				{
					VerbCategory category2 = v.Category;
					if (((category2 != null) ? category2.Text : null) == Loc.GetString("verb-categories-eject"))
					{
						return !v.Disabled;
					}
				}
				return false;
			});
		}
		return verb2;
	}
	private Verb TryFindInsertVerb(EntityUid gunUid, EntityUid playerUid, EntityUid newMagazineUid)
	{
		SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(gunUid, playerUid, typeof(InteractionVerb), false);
		NetEntity newMagNet = this._entityManager.GetNetEntity(newMagazineUid, null);
		Verb verb = localVerbs.FirstOrDefault(delegate(Verb v)
		{
			if (v.Text == "Magazine")
			{
				VerbCategory category = v.Category;
				if (((category != null) ? category.Text : null) == Loc.GetString("verb-categories-insert") && !v.Disabled)
				{
					return v.IconEntity == newMagNet;
				}
			}
			return false;
		});
		Verb verb2;
		if ((verb2 = verb) == null)
		{
			verb2 = localVerbs.FirstOrDefault(delegate(Verb v)
			{
				if (v.Text == "Magazine")
				{
					VerbCategory category2 = v.Category;
					if (((category2 != null) ? category2.Text : null) == Loc.GetString("verb-categories-insert"))
					{
						return !v.Disabled;
					}
				}
				return false;
			});
		}
		return verb2;
	}
	private void ExecuteEjectingOldMagazine(EntityUid playerUid, EntityUid gunUid)
	{
		BaseContainer baseContainer;
		bool flag = this._oldMag == null || !this._entityManager.EntityExists(this._oldMag.Value) || !this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null) || !baseContainer.ContainedEntities.Contains(this._oldMag.Value);
		if (flag)
		{
			this._oldMag = null;
			this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.ReadyToInsertNewMagazine);
			this.RecordActionAttempt();
		}
		else
		{
			Verb verb = this.TryFindEjectVerb(gunUid, playerUid, this._oldMag.Value);
			bool flag2 = verb != null;
			if (flag2)
			{
				this._entityManager.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(this._entityManager.GetNetEntity(gunUid, null), verb));
				this.RecordActionAttempt();
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.WaitingForEject);
			}
			else
			{
				this._oldMag = null;
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.ReadyToInsertNewMagazine);
				this.RecordActionAttempt();
			}
		}
	}
	private void ExecuteWaitingForEject(EntityUid gunUid)
	{
		BaseContainer baseContainer;
		bool flag = this._oldMag == null || (this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null) && !baseContainer.ContainedEntities.Contains(this._oldMag.Value));
		bool flag2 = flag;
		if (flag2)
		{
			this._oldMag = null;
			this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.ReadyToInsertNewMagazine);
		}
	}
	private void ExecuteReadyToInsertNewMagazine(EntityUid playerUid, EntityUid gunUid)
	{
		string activeHandName = this._handsSystem.GetActiveHand(playerUid);
		EntityUid? heldEntity = this._handsSystem.GetActiveItem(playerUid);

		if (activeHandName == null || heldEntity != this._currentMag || this._currentMag == null)
		{
			this.ResetReloadState();
		}
		else
		{
			BaseContainer baseContainer;
			bool flag2 = this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null) && baseContainer.ContainedEntities.Any<EntityUid>();
			if (flag2)
			{
				this.ResetReloadState();
			}
			else
			{
				Verb verb = this.TryFindInsertVerb(gunUid, playerUid, this._currentMag.Value);
				bool flag3 = verb != null;
				if (flag3)
				{
					this._entityManager.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(this._entityManager.GetNetEntity(gunUid, null), verb));
					this.RecordActionAttempt();
					this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.WaitingForInsertConfirm);
				}
				else
				{
					this.ResetReloadState();
				}
			}
		}
	}
	private void ExecuteWaitingForInsertConfirm(EntityUid playerUid, EntityUid gunUid)
	{
		BaseContainer baseContainer;
		bool flag = this._containerSystem.TryGetContainer(gunUid, "gun_magazine", out baseContainer, null) && this._currentMag != null && baseContainer.ContainedEntities.Contains(this._currentMag.Value);
		if (flag)
		{
			string activeHandName = this._handsSystem.GetActiveHand(playerUid);
			
			bool flag2 = !string.IsNullOrEmpty(this._originalHand) && activeHandName != null && activeHandName != this._originalHand;
			if (flag2)
			{
				this.SetReloadState(GunHelperOverlay.AutoReloadMagazineState.SwitchingBackToGunHand);
			}
			else
			{
				this.ResetReloadState();
			}
		}
	}
	private void ExecuteSwitchingBackToGunHand(EntityUid playerUid)
	{
		bool flag = string.IsNullOrEmpty(this._originalHand);
		if (flag)
		{
			this.ResetReloadState();
		}
		else
		{
			string activeHandName = this._handsSystem.GetActiveHand(playerUid);
			
			if (activeHandName == null)
			{
				this.ResetReloadState();
			}
			else
			{
				bool flag3 = activeHandName == this._originalHand;
				if (flag3)
				{
					this.ResetReloadState();
				}
				else
				{
					this.TryRequestHandSwitch(this._originalHand);
				}
			}
		}
	}
	private bool TryFindMagazineForReload(EntityUid playerUid, EntityUid gunUid, out GunHelperOverlay.FoundMagazineInfo bestMagazine)
	{
		bestMagazine = new GunHelperOverlay.FoundMagazineInfo();
		int num = -1;
		HandsComponent handsComponent;
		InventoryComponent inventoryComponent = null;
		bool flag = !this._entityManager.TryGetComponent<HandsComponent>(playerUid, out handsComponent) || !this._entityManager.TryGetComponent<InventoryComponent>(playerUid, out inventoryComponent) || this._inventorySystem == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			foreach (var handName in handsComponent.Hands.Keys)
			{
				var heldItem = this._handsSystem.GetHeldItem(playerUid, handName);
				bool flag3 = heldItem != null && this.IsMagazineCompatible(gunUid, heldItem.Value);
				if (flag3)
				{
					GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
					this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(heldItem.Value, ref getAmmoCountEvent, false);
					bool flag4 = getAmmoCountEvent.Count > num;
					if (flag4)
					{
						num = getAmmoCountEvent.Count;
						bestMagazine = new GunHelperOverlay.FoundMagazineInfo(heldItem.Value, GunHelperOverlay.MagazineSource.Hand, handName, null, null, null);
					}
				}
			}
			string[] array = new string[] { "belt", "pocket1", "pocket2", "back" };
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text = array2[i];
				ContainerSlot containerSlot;
				SlotDefinition slotDefinition;
				bool flag5 = this._inventorySystem.TryGetSlotContainer(playerUid, text, out containerSlot, out slotDefinition, inventoryComponent, null);
				if (flag5)
				{
					bool flag6 = containerSlot.Count == 0;
					if (!flag6)
					{
						foreach (EntityUid entityUid in containerSlot.ContainedEntities)
						{
							bool flag7 = !this._entityManager.EntityExists(entityUid);
							if (!flag7)
							{
								bool flag8 = this.IsMagazineCompatible(gunUid, entityUid);
								if (flag8)
								{
									GetAmmoCountEvent getAmmoCountEvent2 = default(GetAmmoCountEvent);
									this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(entityUid, ref getAmmoCountEvent2, false);
									bool flag9 = getAmmoCountEvent2.Count > num;
									if (flag9)
									{
										num = getAmmoCountEvent2.Count;
										EntityUid entityUid2 = entityUid;
										GunHelperOverlay.MagazineSource magazineSource = GunHelperOverlay.MagazineSource.DirectInventorySlot;
										string text2 = null;
										string text3 = text;
										bestMagazine = new GunHelperOverlay.FoundMagazineInfo(entityUid2, magazineSource, text2, null, null, text3);
									}
								}
								else
								{
									bool flag10 = this._entityManager.HasComponent<StorageComponent>(entityUid);
									if (flag10)
									{
										EntityUid entityUid3;
										int num2;
										bool flag11 = this.TryFindSuitableMagazineInStorage(entityUid, gunUid, out entityUid3, out num2);
										if (flag11)
										{
											bool flag12 = num2 > num;
											if (flag12)
											{
												num = num2;
												bestMagazine = new GunHelperOverlay.FoundMagazineInfo(entityUid3, GunHelperOverlay.MagazineSource.Container, null, new EntityUid?(entityUid), text, null);
											}
										}
									}
								}
							}
						}
					}
				}
				IL_044F:
				i++;
				continue;
				goto IL_044F;
			}
			flag2 = bestMagazine.IsValid;
		}
		return flag2;
	}
	private bool TryFindSuitableMagazineInStorage(EntityUid storageUid, EntityUid gunUid, out EntityUid foundMagazineUid, out int ammoCount)
	{
		foundMagazineUid = default(EntityUid);
		ammoCount = -1;
		StorageComponent storageComponent;
		bool flag = !this._entityManager.TryGetComponent<StorageComponent>(storageUid, out storageComponent);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			EntityUid? entityUid = null;
			int num = -1;
			foreach (EntityUid entityUid2 in storageComponent.Container.ContainedEntities)
			{
				bool flag3 = !this.IsMagazineCompatible(gunUid, entityUid2);
				if (!flag3)
				{
					GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
					this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(entityUid2, ref getAmmoCountEvent, false);
					bool flag4 = getAmmoCountEvent.Count > num;
					if (flag4)
					{
						num = getAmmoCountEvent.Count;
						entityUid = new EntityUid?(entityUid2);
					}
				}
			}
			bool flag5 = entityUid != null;
			if (flag5)
			{
				foundMagazineUid = entityUid.Value;
				ammoCount = num;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
		}
		return flag2;
	}
	private bool IsMagazineCompatible(EntityUid gunUid, EntityUid magazineUid)
	{
		bool flag = !this._entityManager.EntityExists(gunUid) || !this._entityManager.EntityExists(magazineUid) || this._whitelistSystem == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			ItemSlotsComponent itemSlotsComponent;
			bool flag3 = !this._entityManager.TryGetComponent<ItemSlotsComponent>(gunUid, out itemSlotsComponent);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				ItemSlot itemSlot;
				bool flag4 = !itemSlotsComponent.Slots.TryGetValue("gun_magazine", out itemSlot);
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool flag5 = itemSlot.Whitelist == null || this._whitelistSystem.IsValid(itemSlot.Whitelist, magazineUid);
					bool flag6 = flag5 && itemSlot.Blacklist != null && this._whitelistSystem.IsValid(itemSlot.Blacklist, magazineUid);
					if (flag6)
					{
						flag5 = false;
					}
					bool flag7 = flag5;
					if (flag7)
					{
						GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
						this._entityManager.EventBus.RaiseLocalEvent<GetAmmoCountEvent>(magazineUid, ref getAmmoCountEvent, false);
						bool flag8 = getAmmoCountEvent.Capacity <= 0 && getAmmoCountEvent.Count <= 0 && !this._entityManager.HasComponent<BallisticAmmoProviderComponent>(magazineUid) && !this._entityManager.HasComponent<MagazineAmmoProviderComponent>(magazineUid) && !this._entityManager.HasComponent<BatteryAmmoProviderComponent>(magazineUid);
						if (flag8)
						{
							return false;
						}
					}
					flag2 = flag5;
				}
			}
		}
		return flag2;
	}
	private bool IsVirtualItem(EntityUid entityUid)
	{
		bool flag = !this._entityManager.EntityExists(entityUid);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			MetaDataComponent metaDataComponent;
			bool flag3 = this._entityManager.TryGetComponent<MetaDataComponent>(entityUid, out metaDataComponent) && metaDataComponent.EntityPrototype != null && metaDataComponent.EntityPrototype.ID.Contains("VirtualItem", StringComparison.OrdinalIgnoreCase);
			flag2 = flag3;
		}
		return flag2;
	}
	
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	
	[Robust.Shared.IoC.Dependency] private readonly IGameTiming _timing = null;
	
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	
	[Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _sysMan = null;
	
	private VerbSystem _verbSystem;
	private SharedHandsSystem _handsSystem;
	private SharedStorageSystem _storageSystem;
	private InventorySystem _inventorySystem;
	private SharedContainerSystem _containerSystem;
	private EntityWhitelistSystem _whitelistSystem;
	private GunSystem _gunSystem;
	
	private readonly Font _font;
	private TimeSpan _lastReloadAttempt;
	private TimeSpan _lastRackAttempt;
	private TimeSpan _lastActionTime;
	private GunHelperOverlay.AutoReloadMagazineState _reloadState = GunHelperOverlay.AutoReloadMagazineState.Idle;
	private EntityUid? _currentMag;
	private EntityUid? _oldMag;
	private EntityUid? _currentGun;
	private string _originalHand;
	private bool _actionPerformed;
	private TimeSpan _lastStateChange;
	private readonly float TimeoutSeconds = 1.5f;
	private GunHelperOverlay.FoundMagazineInfo _foundMagInfo;
	private string _containerSlotId;
	
	private enum MagazineSource
	{
		Hand,
		Container,
		DirectInventorySlot
	}
	
	private readonly struct FoundMagazineInfo : IEquatable<GunHelperOverlay.FoundMagazineInfo>
	{
		public FoundMagazineInfo(EntityUid MagazineUid, GunHelperOverlay.MagazineSource SourceType, string HandName = null, EntityUid? ContainerEntityUid = null, string ContainerParentSlotId = null, string DirectSlotId = null)
		{
			this.MagazineUid = MagazineUid;
			this.SourceType = SourceType;
			this.HandName = HandName;
			this.ContainerEntityUid = ContainerEntityUid;
			this.ContainerParentSlotId = ContainerParentSlotId;
			this.DirectSlotId = DirectSlotId;
			this.IsValid = true;
		}
		public EntityUid MagazineUid { get; init; }
		public GunHelperOverlay.MagazineSource SourceType { get; init; }
		public string HandName { get; init; }
		public EntityUid? ContainerEntityUid { get; init; }
		public string ContainerParentSlotId { get; init; }
		public string DirectSlotId { get; init; }

		public FoundMagazineInfo()
		{
			this = new GunHelperOverlay.FoundMagazineInfo(default(EntityUid), GunHelperOverlay.MagazineSource.Hand,
				null, null, null, null);
			this.IsValid = false;
		}

		public bool IsValid { get; init; }
		
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("FoundMagazineInfo");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}
		
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("MagazineUid = ");
			builder.Append(this.MagazineUid.ToString());
			builder.Append(", SourceType = ");
			builder.Append(this.SourceType.ToString());
			builder.Append(", HandName = ");
			builder.Append(this.HandName);
			builder.Append(", ContainerEntityUid = ");
			builder.Append(this.ContainerEntityUid.ToString());
			builder.Append(", ContainerParentSlotId = ");
			builder.Append(this.ContainerParentSlotId);
			builder.Append(", DirectSlotId = ");
			builder.Append(this.DirectSlotId);
			builder.Append(", IsValid = ");
			builder.Append(this.IsValid.ToString());
			return true;
		}
		[CompilerGenerated]
		public static bool operator !=(GunHelperOverlay.FoundMagazineInfo left, GunHelperOverlay.FoundMagazineInfo right)
		{
			return !(left == right);
		}
		[CompilerGenerated]
		public static bool operator ==(GunHelperOverlay.FoundMagazineInfo left, GunHelperOverlay.FoundMagazineInfo right)
		{
			return left.Equals(right);
		}
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (((((EqualityComparer<EntityUid>.Default.GetHashCode(this.MagazineUid) * -1521134295 + EqualityComparer<GunHelperOverlay.MagazineSource>.Default.GetHashCode(this.SourceType)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.HandName)) * -1521134295 + EqualityComparer<EntityUid?>.Default.GetHashCode(this.ContainerEntityUid)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ContainerParentSlotId)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.DirectSlotId)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.IsValid);
		}
		
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is GunHelperOverlay.FoundMagazineInfo && this.Equals((GunHelperOverlay.FoundMagazineInfo)obj);
		}
		[CompilerGenerated]
		public bool Equals(GunHelperOverlay.FoundMagazineInfo other)
		{
			return EqualityComparer<EntityUid>.Default.Equals(this.MagazineUid, other.MagazineUid) && EqualityComparer<GunHelperOverlay.MagazineSource>.Default.Equals(this.SourceType, other.SourceType) && EqualityComparer<string>.Default.Equals(this.HandName, other.HandName) && EqualityComparer<EntityUid?>.Default.Equals(this.ContainerEntityUid, other.ContainerEntityUid) && EqualityComparer<string>.Default.Equals(this.ContainerParentSlotId, other.ContainerParentSlotId) && EqualityComparer<string>.Default.Equals(this.DirectSlotId, other.DirectSlotId) && EqualityComparer<bool>.Default.Equals(this.IsValid, other.IsValid);
		}
		[CompilerGenerated]
		public void Deconstruct(out EntityUid MagazineUid, out GunHelperOverlay.MagazineSource SourceType, out string HandName, out EntityUid? ContainerEntityUid, out string ContainerParentSlotId, out string DirectSlotId)
		{
			MagazineUid = this.MagazineUid;
			SourceType = this.SourceType;
			HandName = this.HandName;
			ContainerEntityUid = this.ContainerEntityUid;
			ContainerParentSlotId = this.ContainerParentSlotId;
			DirectSlotId = this.DirectSlotId;
		}
	}
	
	private enum AutoReloadMagazineState
	{
		Idle, CheckingNeed, FindingNewMagazine, SwitchingToNewMagazineHand, RequestingOpenContainerSlot,
		SwitchingToFreeHandForContainerTake, TakingMagazineFromOpenedContainer, PreparingToTakeFromPocketSlot,
		TakingDirectlyFromPocketSlot, NewMagazineInHand, EjectingOldMagazine, WaitingForEject,
		ReadyToInsertNewMagazine, WaitingForInsertConfirm, SwitchingBackToGunHand
	}

}