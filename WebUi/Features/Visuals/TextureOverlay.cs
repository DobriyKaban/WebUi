using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Mobs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;


[CompilerGenerated]
public class TextureOverlay : Overlay
{
	public TextureOverlay()
	{
		IoCManager.InjectDependencies<TextureOverlay>(this);
		base.ZIndex = new int?(200);
	}
	public override OverlaySpace Space
	{
		get
		{
			return (OverlaySpace)16;
		}
	}
	private void LoadTextureFromAppData()
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string text = Path.Combine(folderPath, "CerberusWare", "image.png");
		bool flag = File.Exists(text);
		if (flag)
		{
			try
			{
				using (FileStream fileStream = File.OpenRead(text))
				{
					this._customTexture = Texture.LoadFromPNGStream(fileStream, "CustomTexture", null);
				}
			}
			catch (Exception)
			{
				NotificationManager.ShowNotification("Failed to load texture from", 5f, 0.3f, 0.5f, null, false);
			}
		}
		else
		{
			NotificationManager.ShowNotification("Custom texture not found", 5f, 0.3f, 0.5f, null, false);
		}
	}
	protected override void Draw(in OverlayDrawArgs args)
	{
		bool flag = !CerberusConfig.Texture.Enabled;
		if (!flag)
		{
			if (this._entityLookup == null)
			{
				this._entityLookup = this._entityManager.System<EntityLookupSystem>();
			}
			if (this._transformSystem == null)
			{
				this._transformSystem = this._entityManager.System<SharedTransformSystem>();
			}
			bool flag2 = this._entityLookup == null || this._transformSystem == null;
			if (!flag2)
			{
				bool flag3 = !this._isTextureLoaded;
				if (flag3)
				{
					this.LoadTextureFromAppData();
					this._isTextureLoaded = true;
				}
				bool flag4 = this._customTexture == null;
				if (!flag4)
				{
					HashSet<EntityUid> entitiesIntersecting = this._entityLookup.GetEntitiesIntersecting(this._eyeManager.CurrentMap, this._eyeManager.GetWorldViewport());
					foreach (EntityUid entityUid in entitiesIntersecting)
					{
						TransformComponent transformComponent;
						SpriteComponent spriteComponent = null;
						bool flag5 = !this._entityManager.TryGetComponent<TransformComponent>(entityUid, out transformComponent) || !this._entityManager.TryGetComponent<SpriteComponent>(entityUid, out spriteComponent) || !this._entityManager.HasComponent<MobStateComponent>(entityUid);
						if (!flag5)
						{
							bool makeEntitiesInvisible = CerberusConfig.Texture.MakeEntitiesInvisible;
							if (makeEntitiesInvisible)
							{
								bool flag6 = !this._originalColors.ContainsKey(entityUid);
								if (flag6)
								{
									this._originalColors[entityUid] = spriteComponent.Color;
								}
								spriteComponent.Color = new Color(0, 0, 0, 0);
							}
							else
							{
								bool flag7 = this._originalColors.ContainsKey(entityUid);
								if (flag7)
								{
									spriteComponent.Color = this._originalColors[entityUid];
									this._originalColors.Remove(entityUid);
								}
							}
							Vector2 worldPosition = this._transformSystem.GetWorldPosition(entityUid);
							Angle worldRotation = this._transformSystem.GetWorldRotation(entityUid);
							Box2 box = Box2.CenteredAround(worldPosition, new Vector2(CerberusConfig.Texture.Size));
							Box2Rotated box2Rotated = default;
							args.WorldHandle.DrawTextureRect(this._customTexture, box2Rotated, null);
						}
					}
				}
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	private readonly IEyeManager _eyeManager = null;
	
	private EntityLookupSystem _entityLookup;
	
	private SharedTransformSystem _transformSystem;
	private readonly Dictionary<EntityUid, Color> _originalColors = new Dictionary<EntityUid, Color>();
	private bool _isTextureLoaded;
	
	private Texture _customTexture;
}
