using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


public sealed class OverlayRegistrationSystem : EntitySystem
{
	public override void Initialize()
	{
		 this._healthBar = new HealthBarOverlay(this._entityManager);
		this._overlayManager.AddOverlay(this._healthBar);
		this._overlayManager.AddOverlay(this._gunAimbot);
		this._overlayManager.AddOverlay(this._gunHelper);
		this._overlayManager.AddOverlay(this._meleeAimbot);
		this._overlayManager.AddOverlay(this._esp);
		this._overlayManager.AddOverlay(this._stamina);
		//this._overlayManager.AddOverlay(this._fun);
		this._overlayManager.AddOverlay(this._texture);
		this._overlayManager.AddOverlay(this._storageViewer);
		this._overlayManager.AddOverlay(this._itemSearch);
		this._overlayManager.AddOverlay(this._placeholder);
		this._overlayManager.AddOverlay(this._trajectory);
	}
	public override void Shutdown()
	{
		this._overlayManager.RemoveOverlay(this._healthBar);
		this._overlayManager.RemoveOverlay(this._gunAimbot);
		this._overlayManager.RemoveOverlay(this._gunHelper);
		this._overlayManager.RemoveOverlay(this._meleeAimbot);
		this._overlayManager.RemoveOverlay(this._esp);
		this._overlayManager.RemoveOverlay(this._stamina);
		//this._overlayManager.RemoveOverlay(this._fun);
		this._overlayManager.RemoveOverlay(this._texture);
		this._overlayManager.RemoveOverlay(this._storageViewer);
		this._overlayManager.RemoveOverlay(this._itemSearch);
		this._overlayManager.RemoveOverlay(this._placeholder);
		this._overlayManager.RemoveOverlay(this._trajectory);
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IOverlayManager _overlayManager;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager;
	 private readonly GunAimbotOverlay _gunAimbot = new GunAimbotOverlay();
	 private readonly GunHelperOverlay _gunHelper = new GunHelperOverlay();
	 private readonly MeleeAimbotOverlay _meleeAimbot = new MeleeAimbotOverlay();
	 private readonly EspOverlay _esp = new EspOverlay();
	 private readonly StaminaOverlay _stamina = new StaminaOverlay();
	 //private readonly FunOverlay _fun = new FunOverlay();
 private readonly TextureOverlay _texture = new TextureOverlay();
private readonly StorageViewerOverlay _storageViewer = new StorageViewerOverlay();
private readonly ItemSearchOverlay _itemSearch = new ItemSearchOverlay();
private readonly PlaceholderOverlay _placeholder = new PlaceholderOverlay();
private readonly TrajectoryOverlay _trajectory = new TrajectoryOverlay();
 private HealthBarOverlay _healthBar = null;
}
