using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.Storage;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Containers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;


[CompilerGenerated]
public sealed class StorageViewerOverlay : Overlay
{
	public StorageViewerOverlay()
	{
		IoCManager.InjectDependencies<StorageViewerOverlay>(this);
		base.ZIndex = new int?(200);
		this._font = new VectorFont(this._resourceCache.GetResource<FontResource>("/Fonts/Boxfont-round/Boxfont Round.ttf", true), 8);
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
		bool flag = !CerberusConfig.StorageViewer.Enabled;
		if (!flag)
		{
			if (this._entityLookup == null)
			{
				this._entityLookup = this._systemManager.GetEntitySystem<EntityLookupSystem>();
			}
			bool flag2 = this._entityLookup == null;
			if (!flag2)
			{
				this.HandleTargetLocking(in args);
				EntityUid? targetEntity = this.GetTargetEntity(in args);
				bool flag3 = targetEntity == null;
				if (!flag3)
				{
					this.DisplayTargetContents(in args, targetEntity.Value);
				}
			}
		}
	}
	private void HandleTargetLocking(in OverlayDrawArgs args)
	{
		// ИЗМЕНЕНИЕ: Используем InputUtils
		if (InputUtils.IsKeyDown(CerberusConfig.StorageViewer.HotKey))
		{
			EntityUid? entityUnderCursor = this.GetEntityUnderCursor(in args);
			if (entityUnderCursor != null)
			{
				if (this._lockedEntity == entityUnderCursor)
				{
					this._lockedEntity = null;
				}
				else
				{
					this._lockedEntity = entityUnderCursor;
				}
			}
		}
	}
	private EntityUid? GetTargetEntity(in OverlayDrawArgs args)
	{
		bool flag = this._lockedEntity != null && this._entityManager.EntityExists(this._lockedEntity.Value);
		EntityUid? entityUid;
		if (flag)
		{
			entityUid = this._lockedEntity;
		}
		else
		{
			entityUid = this.GetEntityUnderCursor(in args);
		}
		return entityUid;
	}
	private EntityUid? GetEntityUnderCursor(in OverlayDrawArgs args)
	{
		EntityUid? localEntity = this._playerManager.LocalEntity;
		bool flag = localEntity == null || !this._entityManager.HasComponent<TransformComponent>(localEntity.Value);
		EntityUid? entityUid;
		if (flag)
		{
			entityUid = null;
		}
		else
		{
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			MapCoordinates mapPosition = this._entityManager.GetComponent<TransformComponent>(localEntity.Value).MapPosition;
			bool flag2 = mapPosition.MapId != mapCoordinates.MapId;
			if (flag2)
			{
				entityUid = null;
			}
			else
			{
				Box2 box = Box2.CenteredAround(mapCoordinates.Position, new Vector2(0.2f, 0.2f));
				entityUid = new EntityUid?(this._entityLookup.GetEntitiesIntersecting(mapCoordinates.MapId, box).FirstOrDefault((EntityUid uid) => this._entityManager.HasComponent<TransformComponent>(uid) && (this._entityManager.HasComponent<StorageComponent>(uid) || this._entityManager.HasComponent<ContainerManagerComponent>(uid))));
			}
		}
		return entityUid;
	}
	private void DisplayTargetContents(in OverlayDrawArgs args, EntityUid targetEntity)
	{
		TransformComponent transformComponent;
		bool flag = !this._entityManager.TryGetComponent<TransformComponent>(targetEntity, out transformComponent);
		if (!flag)
		{
			Vector2 vector = this._eyeManager.WorldToScreen(transformComponent.WorldPosition);
			Vector2 vector2 = vector - new Vector2(230f, 30f);
			bool flag2 = false;
			StorageComponent storageComponent;
			bool flag3 = this._entityManager.TryGetComponent<StorageComponent>(targetEntity, out storageComponent) && storageComponent.Container.ContainedEntities.Count > 0;
			if (flag3)
			{
				this.DisplayContainerContents(in args, ref vector2, storageComponent.Container, ref flag2, 1);
			}
			else
			{
				ContainerManagerComponent containerManagerComponent;
				bool flag4 = this._entityManager.TryGetComponent<ContainerManagerComponent>(targetEntity, out containerManagerComponent);
				if (flag4)
				{
					foreach (ValueTuple<string, string> valueTuple in this._slotMap)
					{
						string item = valueTuple.Item1;
						string item2 = valueTuple.Item2;
						BaseContainer baseContainer;
						bool flag5 = !containerManagerComponent.TryGetContainer(item, out baseContainer) || !this.HasContainerContents(baseContainer);
						if (!flag5)
						{
							this.DisplayContainerName(in args, ref vector2, item2, ref flag2);
							this.DisplayContainerContents(in args, ref vector2, baseContainer, ref flag2, 1);
						}
					}
					BaseContainer baseContainer2;
					bool flag6 = containerManagerComponent.TryGetContainer(this.ImplantContainerId, out baseContainer2);
					if (flag6)
					{
						foreach (EntityUid entityUid in baseContainer2.ContainedEntities)
						{
							StorageComponent storageComponent2;
							bool flag7 = this._entityManager.TryGetComponent<StorageComponent>(entityUid, out storageComponent2) && storageComponent2.Container.ContainedEntities.Count > 0;
							if (flag7)
							{
								string entityName = this._entityManager.GetComponent<MetaDataComponent>(entityUid).EntityName;
								this.DisplayContainerName(in args, ref vector2, entityName, ref flag2);
								this.DisplayContainerContents(in args, ref vector2, storageComponent2.Container, ref flag2, 2);
							}
						}
					}
				}
			}
		}
	}
	private void DisplayContainerName(in OverlayDrawArgs args, ref Vector2 textPosition, string containerName, ref bool hasContents)
	{
		bool flag = !hasContents;
		if (flag)
		{
			args.ScreenHandle.DrawString(this._font, textPosition, "Хранится:", new Color(ref CerberusConfig.StorageViewer.Color));
			textPosition.Y += 12f;
			hasContents = true;
		}
		args.ScreenHandle.DrawString(this._font, textPosition, " " + containerName + ":", new Color(ref CerberusConfig.StorageViewer.Color));
		textPosition.Y += 12f;
	}
	private void DisplayContainerContents(in OverlayDrawArgs args, ref Vector2 textPosition, BaseContainer container, ref bool hasContents, int nestingLevel = 1)
	{
		IEnumerable<ValueTuple<string, int>> enumerable = from entity in container.ContainedEntities
			group entity by this._entityManager.GetComponent<MetaDataComponent>(entity).EntityName into @group
			select new ValueTuple<string, int>(@group.Key, @group.Count<EntityUid>());
		foreach (ValueTuple<string, int> valueTuple in enumerable)
		{
			string item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			string text = new string(' ', nestingLevel * 2);
			string text2;
			if (item2 <= 1)
			{
				text2 = item;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted(item);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(item2);
				defaultInterpolatedStringHandler.AppendLiteral("x");
				text2 = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			string text3 = text2;
			args.ScreenHandle.DrawString(this._font, textPosition, text + "- " + text3, new Color(ref CerberusConfig.StorageViewer.Color));
			textPosition.Y += 12f;
		}
		foreach (EntityUid entityUid in container.ContainedEntities)
		{
			StorageComponent storageComponent;
			bool flag = this._entityManager.TryGetComponent<StorageComponent>(entityUid, out storageComponent) && storageComponent.Container.ContainedEntities.Count > 0;
			if (flag)
			{
				string text4 = new string(' ', nestingLevel * 2);
				args.ScreenHandle.DrawString(this._font, textPosition, text4 + "- " + this._entityManager.GetComponent<MetaDataComponent>(storageComponent.Owner).EntityName + ":", new Color(ref CerberusConfig.StorageViewer.Color));
				textPosition.Y += 12f;
				this.DisplayContainerContents(in args, ref textPosition, storageComponent.Container, ref hasContents, nestingLevel + 1);
			}
		}
	}
	private bool HasContainerContents(BaseContainer container)
	{
		bool flag = container.ContainedEntities.Count > 0;
		bool flag2;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			foreach (EntityUid entityUid in container.ContainedEntities)
			{
				StorageComponent storageComponent;
				bool flag3 = this._entityManager.TryGetComponent<StorageComponent>(entityUid, out storageComponent) && storageComponent.Container.ContainedEntities.Count > 0;
				if (flag3)
				{
					return true;
				}
			}
			flag2 = false;
		}
		return flag2;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IInputManager _inputManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IResourceCache _resourceCache = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _systemManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	private EntityLookupSystem _entityLookup;
	private readonly ValueTuple<string, string>[] _slotMap = new ValueTuple<string, string>[]
	{
		new ValueTuple<string, string>("pocket1", "Карман 1"),
		new ValueTuple<string, string>("pocket2", "Карман 2"),
		new ValueTuple<string, string>("back", "Спина"),
		new ValueTuple<string, string>("belt", "Пояс"),
		new ValueTuple<string, string>("body_part_slot_right_hand", "Правая рука"),
		new ValueTuple<string, string>("body_part_slot_left_hand", "Левая рука"),
		new ValueTuple<string, string>("entity_storage", "Контейнер"),
		new ValueTuple<string, string>("disposals", "Мусорка")
	};
	private readonly Font _font;
	private readonly string ImplantContainerId = "implant";
	private EntityUid? _lockedEntity;
}
