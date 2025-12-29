using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Shared.CombatMode;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;


[CompilerGenerated]
public sealed class EspOverlay : Overlay
{
	public EspOverlay()
	{
		IoCManager.InjectDependencies<EspOverlay>(this);
		base.ZIndex = new int?(200);
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
		bool flag = !CerberusConfig.Esp.Enabled;
		if (!flag)
		{
			this.UpdateValidSessions();
			Font font = new VectorFont(this._resourceCache.GetResource<FontResource>(CerberusConfig.Esp.MainFontPath, true), CerberusConfig.Esp.MainFontSize);
			Font font2 = new VectorFont(this._resourceCache.GetResource<FontResource>(CerberusConfig.Esp.OtherFontPath, true), CerberusConfig.Esp.OtherFontSize);
			if (this._friendSystem == null)
			{
				this._friendSystem = this._systemManager.GetEntitySystem<FriendSystem>();
			}
			if (this._prioritySystem == null)
			{
				this._prioritySystem = this._systemManager.GetEntitySystem<PrioritySystem>();
			}
			if (this._contrabandDetector == null)
			{
				this._contrabandDetector = this._systemManager.GetEntitySystem<ContrabandDetector>();
			}
			if (this._entityLookup == null)
			{
				this._entityLookup = this._systemManager.GetEntitySystem<EntityLookupSystem>();
			}
			if (this._antagDetector == null)
			{
				this._antagDetector = this._systemManager.GetEntitySystem<AntagDetector>();
			}
			if (this._noSlipSystem == null)
			{
				this._noSlipSystem = this._systemManager.GetEntitySystem<NoSlipSystem>();
			}
			foreach (ICommonSession commonSession in this._validSessions)
			{
				EntityUid? attachedEntity = commonSession.AttachedEntity;
				if (attachedEntity == null)
				{
					continue;
				}

				EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
				if (!this._entityManager.EntityExists(valueOrDefault))
				{
					continue;
				}

				if (this._entityManager.GetComponent<TransformComponent>(valueOrDefault).MapID != this._eyeManager.CurrentMap)
				{
					continue;
				}
				MetaDataComponent component = this._entityManager.GetComponent<MetaDataComponent>(valueOrDefault);
				Box2 worldAABB = this._entityLookup.GetWorldAABB(valueOrDefault, null);
				Box2 worldAABB_copy = args.WorldAABB; 
				IEyeManager eyeManagerRef = this._eyeManager;
				Vector2 center = worldAABB.Center;
				Angle angle = new Angle(-this._eyeManager.CurrentEye.Rotation);
				Vector2 vector = worldAABB.TopRight - worldAABB.Center;
				Vector2 vector2 = eyeManagerRef.WorldToScreen(center + angle.RotateVec(ref vector)) + new Vector2(1f, 7f);
				Vector2 vector3 = vector2;
				Vector2 vector4 = new Vector2(0f, (float)CerberusConfig.Esp.FontInterval);
				bool flag4 = !worldAABB.Intersects(ref worldAABB_copy);
				if (flag4)
				{
					continue;
				}
				string name = commonSession.Name;
				ICommonSession localSession = this._playerManager.LocalSession;
				bool flag5 = name == ((localSession != null) ? localSession.Name : null);
				if (flag5)
				{
					continue;
				}
				bool showName = CerberusConfig.Esp.ShowName;
				if (showName)
				{
					args.ScreenHandle.DrawString(font, vector3, component.EntityName, (commonSession.Status == (SessionStatus)4) ? Color.White : new Color(ref CerberusConfig.Esp.NameColor));
					vector3 += vector4;
				}
				bool showCKey = CerberusConfig.Esp.ShowCKey;
				if (showCKey)
				{
					args.ScreenHandle.DrawString(font, vector3, commonSession.Name, (commonSession.Status == (SessionStatus)4) ? Color.White : new Color(ref CerberusConfig.Esp.CKeyColor));
					vector3 += vector4;
				}
				bool showAntag = CerberusConfig.Esp.ShowAntag;
				if (showAntag)
				{
					bool flag6 = this._antagDetector.IsAgent(valueOrDefault) && CerberusConfig.Esp.ShowAntag;
					if (flag6)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Agent"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag7 = this._antagDetector.IsHeretic(valueOrDefault);
					if (flag7)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Heretic"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag8 = this._antagDetector.IsVampire(valueOrDefault);
					if (flag8)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Vampire"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag9 = this._antagDetector.IsFleshCultist(valueOrDefault);
					if (flag9)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_FleshCult"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag10 = this._antagDetector.IsZeroZombie(valueOrDefault);
					if (flag10)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_ZeroZombie"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag11 = this._antagDetector.IsChangeling(valueOrDefault);
					if (flag11)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Changeling"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag12 = this._antagDetector.IsCosmicCult(valueOrDefault);
					if (flag12)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_CosmicCult"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag13 = this._antagDetector.IsDevil(valueOrDefault);
					if (flag13)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Devil"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag14 = this._antagDetector.IsBlob(valueOrDefault);
					if (flag14)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Blob"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
					bool flag15 = this._antagDetector.IsThief(valueOrDefault);
					if (flag15)
					{
						args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Thief"), new Color(ref CerberusConfig.Esp.AntagColor));
						vector3 += vector4;
					}
				}
				bool flag16 = CerberusConfig.Esp.ShowFriend && this._friendSystem.IsFriend(valueOrDefault);
				if (flag16)
				{
					args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Friend"), new Color(ref CerberusConfig.Esp.FriendColor));
					vector3 += vector4;
				}
				bool flag17 = CerberusConfig.Esp.ShowPriority && this._prioritySystem.IsPriority(valueOrDefault);
				if (flag17)
				{
					args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_Priority"), new Color(ref CerberusConfig.Esp.PriorityColor));
					vector3 += vector4;
				}
				bool showCombatMode = CerberusConfig.Esp.ShowCombatMode;
				if (showCombatMode)
				{
					CombatModeComponent combatModeComponent;
					bool flag18 = this._entityManager.TryGetComponent<CombatModeComponent>(valueOrDefault, out combatModeComponent);
					if (flag18)
					{
						bool isInCombatMode = combatModeComponent.IsInCombatMode;
						if (isInCombatMode)
						{
							args.ScreenHandle.DrawString(font, vector3, LocalizationManager.GetString("ESP_CombatMode"), new Color(ref CerberusConfig.Esp.CombatModeColor));
							vector3 += vector4;
						}
					}
				}
				bool flag19 = this._contrabandDetector.HasContraband(valueOrDefault) && CerberusConfig.Esp.ShowContraband;
				if (flag19)
				{
					args.ScreenHandle.DrawString(font2, vector3, LocalizationManager.GetString("ESP_Contraband"), new Color(ref CerberusConfig.Esp.ContrabandColor));
					vector3 += vector4;
				}
				bool flag20 = this._contrabandDetector.HasImplants(valueOrDefault) && CerberusConfig.Esp.ShowImplants;
				if (flag20)
				{
					args.ScreenHandle.DrawString(font2, vector3, LocalizationManager.GetString("ESP_Implants"), new Color(ref CerberusConfig.Esp.ImplantsColor));
					vector3 += vector4;
				}
				bool flag21 = this._contrabandDetector.HasWeapons(valueOrDefault) && CerberusConfig.Esp.ShowWeapon;
				if (flag21)
				{
					args.ScreenHandle.DrawString(font2, vector3, LocalizationManager.GetString("ESP_Weapon"), new Color(ref CerberusConfig.Esp.WeaponColor));
					vector3 += vector4;
				}
				bool flag22 = !this._noSlipSystem.CanSlip(valueOrDefault) && CerberusConfig.Esp.ShowNoSlip;
				if (flag22)
				{
					args.ScreenHandle.DrawString(font2, vector3, LocalizationManager.GetString("ESP_NoSlip"), new Color(ref CerberusConfig.Esp.NoSlipColor));
					vector3 += vector4;
				}
				continue;
			}
		}
	}
	private void UpdateValidSessions()
	{
		List<ICommonSession> list = this._playerManager.Sessions.ToList<ICommonSession>();
		foreach (ICommonSession commonSession in this._validSessions.ToList<ICommonSession>())
		{
			bool flag = !list.Contains(commonSession);
			if (flag)
			{
				this._playerManager.SetStatus(commonSession, (SessionStatus)4);
			}
		}
		foreach (ICommonSession commonSession2 in list)
		{
			bool flag2 = !this._validSessions.Contains(commonSession2);
			if (flag2)
			{
				this._validSessions.Add(commonSession2);
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEyeManager _eyeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IResourceCache _resourceCache = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntitySystemManager _systemManager = null;
	
	private FriendSystem _friendSystem;
	
	private PrioritySystem _prioritySystem;
	
	private ContrabandDetector _contrabandDetector;
	
	private EntityLookupSystem _entityLookup;
	
	private AntagDetector _antagDetector;
	
	private NoSlipSystem _noSlipSystem;
	private readonly List<ICommonSession> _validSessions = new List<ICommonSession>();
}
