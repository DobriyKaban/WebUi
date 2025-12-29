using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;


[CompilerGenerated]
public sealed class ItemSearchOverlay : Overlay
{
	public ItemSearchOverlay()
	{
		IoCManager.InjectDependencies<ItemSearchOverlay>(this);
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
	protected override void Draw(in OverlayDrawArgs args)
	{
		EntityUid valueOrDefault = default;
		bool flag;
		if (CerberusConfig.Misc.ItemSearcherEnabled && CerberusConfig.Misc.ItemSearchEntries.Count != 0)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			int num;
			if (localEntity != null)
			{
				valueOrDefault = localEntity.GetValueOrDefault();
				num = 1;
			}
			else
			{
				num = 0;
			}
			flag = num == 0;
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		if (!flag2)
		{
			if (this._entityLookup == null)
			{
				this._entityLookup = this._entityManager.System<EntityLookupSystem>();
			}
			TransformComponent component = this._entityManager.GetComponent<TransformComponent>(valueOrDefault);
			MapId mapID = component.MapID;
			Box2 worldViewport = this._eyeManager.GetWorldViewport();
			HashSet<EntityUid> entitiesIntersecting = this._entityLookup.GetEntitiesIntersecting(mapID, worldViewport);
			foreach (EntityUid entityUid in entitiesIntersecting)
			{
				MetaDataComponent metaDataComponent;
				TransformComponent transformComponent = null;
				bool flag3 = !this._entityManager.TryGetComponent<MetaDataComponent>(entityUid, out metaDataComponent) || !this._entityManager.TryGetComponent<TransformComponent>(entityUid, out transformComponent);
				if (!flag3)
				{
					foreach (ItemSearchEntry searchEntry in CerberusConfig.Misc.ItemSearchEntries)
					{
						bool flag4 = string.IsNullOrWhiteSpace(searchEntry.ItemName);
						if (!flag4)
						{
							bool flag5 = metaDataComponent.EntityName.Contains(searchEntry.ItemName, StringComparison.OrdinalIgnoreCase);
							if (flag5)
							{
								Vector2 vector = this._eyeManager.WorldToScreen(component.WorldPosition);
								Vector2 vector2 = this._eyeManager.WorldToScreen(transformComponent.WorldPosition);
								args.ScreenHandle.DrawLine(vector, vector2, new Color(ref searchEntry.Color));
								bool itemSearcherShowName = CerberusConfig.Misc.ItemSearcherShowName;
								if (itemSearcherShowName)
								{
									args.ScreenHandle.DrawString(this._font, vector2 - new Vector2(0f, 10f), metaDataComponent.EntityName, new Color(ref searchEntry.Color));
								}
								break;
							}
						}
					}
				}
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IResourceCache _resourceCache = null;
	private readonly Font _font;
	
	private EntityLookupSystem _entityLookup;
}
