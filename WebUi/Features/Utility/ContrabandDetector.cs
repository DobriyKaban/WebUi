using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Contraband;
using Content.Shared.Examine;
using Content.Shared.Roles;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;


[CompilerGenerated]
public sealed class ContrabandDetector : EntitySystem
{
	public override void Initialize()
	{
		ComponentRegistration componentRegistration;
		this._hasContrabandComp = this._componentFactory.TryGetRegistration("Contraband", out componentRegistration, false);
		bool hasCompReg = this._hasContrabandComp;
		if (hasCompReg)
		{
			this._contrabandCompType = ((componentRegistration != null) ? componentRegistration.Type : null);
		}
		base.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddContrabandCheckVerb), null, null);
	}
	private void AddContrabandCheckVerb(GetVerbsEvent<Verb> args)
	{
		bool flag = !args.CanInteract || args.Hands == null;
		if (!flag)
		{
			ContainerManagerComponent containerManagerComponent;
			bool flag2 = !base.TryComp<ContainerManagerComponent>(args.Target, out containerManagerComponent) || containerManagerComponent.Containers.Count == 0 || (this._hasContrabandComp && this._entityManager.HasComponent(args.Target, this._contrabandCompType)) || (!this.HasContraband(args.Target) && !this.HasImplants(args.Target));
			if (!flag2)
			{
				AlternativeVerb alternativeVerb = new AlternativeVerb
				{
					Act = delegate
					{
						FormattedMessage examineText = this.GetExamineText(args.Target);
						this._examineSystem.SendExamineTooltip(args.User, args.Target, examineText, false, false);
					},
					Text = "Information",
					ClientExclusive = true
				};
				args.Verbs.Add(alternativeVerb);
			}
		}
	}
	public bool HasContraband(EntityUid target)
	{
		bool hasCompReg = this._hasContrabandComp;
		bool flag3;
		if (hasCompReg)
		{
			ContainerManagerComponent containerManagerComponent;
			bool flag = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
			if (flag)
			{
				foreach (BaseContainer baseContainer in containerManagerComponent.Containers.Values)
				{
					foreach (EntityUid entityUid in baseContainer.ContainedEntities)
					{
						bool flag2 = this.IsContraband(entityUid, target);
						if (flag2)
						{
							return true;
						}
					}
				}
			}
			flag3 = false;
		}
		else
		{
			ContainerManagerComponent containerManagerComponent2;
			bool flag4 = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent2);
			if (flag4)
			{
				foreach (BaseContainer baseContainer2 in containerManagerComponent2.Containers.Values)
				{
					foreach (EntityUid entityUid2 in baseContainer2.ContainedEntities)
					{
						EntityPrototype entityPrototype = base.MetaData(entityUid2).EntityPrototype;
						string text = ((entityPrototype != null) ? entityPrototype.ID : null);
						bool flag5 = text != null && this._contrabandItems.Contains(text);
						if (flag5)
						{
							return true;
						}
					}
				}
			}
			flag3 = false;
		}
		return flag3;
	}
	public bool HasImplants(EntityUid target)
	{
		BaseContainer baseContainer;
		return this._containerSystem.TryGetContainer(target, "implant", out baseContainer, null) && baseContainer.ContainedEntities.Count > 0;
	}
	public bool HasWeapons(EntityUid target)
	{
		ContainerManagerComponent containerManagerComponent;
		bool flag = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
		if (flag)
		{
			foreach (BaseContainer baseContainer in containerManagerComponent.Containers.Values)
			{
				foreach (EntityUid entityUid in baseContainer.ContainedEntities)
				{
					EntityPrototype entityPrototype = base.MetaData(entityUid).EntityPrototype;
					string text = ((entityPrototype != null) ? entityPrototype.ID : null);
					bool flag2 = text != null && this._weaponItems.Contains(text);
					if (flag2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	private bool IsContraband(EntityUid target, EntityUid idCardOwner)
	{
		bool flag = !this._hasContrabandComp;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			ContrabandComponent contrabandComponent;
			bool flag3 = !base.TryComp<ContrabandComponent>(target, out contrabandComponent);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				List<ProtoId<DepartmentPrototype>> list = null;
				Entity<IdCardComponent> entity;
				bool flag4 = this._idCardSystem.TryFindIdCard(idCardOwner, out entity);
				if (flag4)
				{
					list = entity.Comp.JobDepartments;
				}
				flag2 = contrabandComponent.AllowedDepartments == null || list == null || !list.Intersect(contrabandComponent.AllowedDepartments).Any<ProtoId<DepartmentPrototype>>();
			}
		}
		return flag2;
	}
	private FormattedMessage GetExamineText(EntityUid target)
	{
		FormattedMessage formattedMessage = new FormattedMessage();
		ContainerManagerComponent containerManagerComponent;
		bool flag = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
		if (flag)
		{
			bool flag2 = this.HasImplants(target);
			if (flag2)
			{
				List<string> implants = this.GetImplants(target);
				formattedMessage.AddMarkup("[color=red]Detected implants:[/color]\n");
				foreach (string text in implants)
				{
					formattedMessage.AddMarkup("- " + text + "\n");
					formattedMessage.PushNewline();
				}
			}
			bool flag3 = this.HasContraband(target);
			if (flag3)
			{
				bool hasCompReg = this._hasContrabandComp;
				if (hasCompReg)
				{
					Dictionary<string, List<string>> contrabandItems = this.GetContrabandItems(target);
					formattedMessage.AddMarkup("[color=red]Detected contraband:[/color]\n");
					foreach (KeyValuePair<string, List<string>> keyValuePair in contrabandItems)
					{
						string text2;
						List<string> list;
						keyValuePair.Deconstruct(out text2, out list);
						string text3 = text2;
						List<string> list2 = list;
						formattedMessage.AddMarkup(text3 + "\n");
						foreach (string text4 in list2)
						{
							formattedMessage.AddMarkup("- " + text4 + "\n");
						}
					}
				}
				else
				{
					List<string> contrabandItemsWithoutComponent = this.GetContrabandItemsWithoutComponent(target);
					formattedMessage.AddMarkup("[color=red]Detected contraband:[/color]\n");
					foreach (string text5 in contrabandItemsWithoutComponent)
					{
						formattedMessage.AddMarkup("- " + text5 + "\n");
					}
				}
			}
			bool flag4 = this.HasWeapons(target);
			if (flag4)
			{
				List<string> weaponItems = this.GetWeaponItems(target);
				formattedMessage.AddMarkup("[color=red]Detected weapon:[/color]\n");
				foreach (string text6 in weaponItems)
				{
					formattedMessage.AddMarkup("- " + text6 + "\n");
					formattedMessage.PushNewline();
				}
			}
		}
		return formattedMessage;
	}
	private Dictionary<string, List<string>> GetContrabandItems(EntityUid target)
	{
		bool flag = !this._hasContrabandComp;
		Dictionary<string, List<string>> dictionary;
		if (flag)
		{
			dictionary = new Dictionary<string, List<string>>();
		}
		else
		{
			Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
			ContainerManagerComponent containerManagerComponent;
			bool flag2 = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
			if (flag2)
			{
				foreach (BaseContainer baseContainer in containerManagerComponent.Containers.Values)
				{
					foreach (EntityUid entityUid in baseContainer.ContainedEntities)
					{
						bool flag3 = !this.IsContraband(entityUid, target);
						if (!flag3)
						{
							ContrabandSeverityPrototype contrabandSeverityPrototype = this._prototypeManager.Index<ContrabandSeverityPrototype>(base.Comp<ContrabandComponent>(entityUid).Severity);
							string text = contrabandSeverityPrototype.ExamineText.ToString();
							string text3;
							string text2 = (this._contrabandTextMap.TryGetValue(text, out text3) ? text3 : text);
							bool flag4 = !dictionary2.ContainsKey(text2);
							if (flag4)
							{
								dictionary2[text2] = new List<string>();
							}
							dictionary2[text2].Add(base.Name(entityUid, null));
						}
					}
				}
			}
			dictionary = dictionary2;
		}
		return dictionary;
	}
	private List<string> GetImplants(EntityUid target)
	{
		List<string> list = new List<string>();
		BaseContainer baseContainer;
		bool flag = this._containerSystem.TryGetContainer(target, "implant", out baseContainer, null);
		if (flag)
		{
			list.AddRange(baseContainer.ContainedEntities.Select(delegate(EntityUid e)
			{
				MetaDataComponent metaDataComponent = base.Comp<MetaDataComponent>(e);
				EntityPrototype entityPrototype = metaDataComponent.EntityPrototype;
				return string.IsNullOrEmpty((entityPrototype != null) ? entityPrototype.EditorSuffix : null) ? metaDataComponent.EntityName : metaDataComponent.EntityPrototype.EditorSuffix;
			}));
		}
		return list;
	}
	private List<string> GetWeaponItems(EntityUid target)
	{
		List<string> list = new List<string>();
		ContainerManagerComponent containerManagerComponent;
		bool flag = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
		if (flag)
		{
			foreach (BaseContainer baseContainer in containerManagerComponent.Containers.Values)
			{
				foreach (EntityUid entityUid in baseContainer.ContainedEntities)
				{
					HashSet<string> weaponSet = this._weaponItems;
					EntityPrototype entityPrototype = base.MetaData(entityUid).EntityPrototype;
					bool flag2 = weaponSet.Contains(((entityPrototype != null) ? entityPrototype.ID : null) ?? string.Empty);
					if (flag2)
					{
						list.Add(base.Name(entityUid, null));
					}
				}
			}
		}
		return list;
	}
	private List<string> GetContrabandItemsWithoutComponent(EntityUid target)
	{
		List<string> list = new List<string>();
		ContainerManagerComponent containerManagerComponent;
		bool flag = base.TryComp<ContainerManagerComponent>(target, out containerManagerComponent);
		if (flag)
		{
			foreach (BaseContainer baseContainer in containerManagerComponent.Containers.Values)
			{
				foreach (EntityUid entityUid in baseContainer.ContainedEntities)
				{
					HashSet<string> contrabandSet = this._contrabandItems;
					EntityPrototype entityPrototype = base.MetaData(entityUid).EntityPrototype;
					bool flag2 = contrabandSet.Contains(((entityPrototype != null) ? entityPrototype.ID : null) ?? string.Empty);
					if (flag2)
					{
						list.Add(base.Name(entityUid, null));
					}
				}
			}
		}
		return list;
	}
	
	[Robust.Shared.IoC.Dependency] private readonly IComponentFactory _componentFactory = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPrototypeManager _prototypeManager = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedContainerSystem _containerSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly ExamineSystemShared _examineSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly SharedIdCardSystem _idCardSystem = null;
	private readonly ComponentManager _compManager = new ComponentManager();
	private readonly Dictionary<string, string> _contrabandTextMap = new Dictionary<string, string>
	{
		{ "contraband-examine-text-Minor", "[color=yellow]Этот предмет считается мелкой контрабандой.[/color]" },
		{ "contraband-examine-text-Restricted", "[color=yellow]Этот предмет ограничен отделом.[/color]" },
		{ "contraband-examine-text-Restricted-department", "[color=yellow]Этот предмет ограничен для {departments}, и может считаться контрабандой.[/color]" },
		{ "contraband-examine-text-Major", "[color=red]Этот предмет считается крупной контрабандой.[/color]" },
		{ "contraband-examine-text-GrandTheft", "[color=red]Этот предмет является очень ценной целью для агентов Синдиката![/color]" },
		{ "contraband-examine-text-Syndicate", "[color=crimson]Этот предмет является крайне незаконной контрабандой Синдиката![/color]" },
		{ "contraband-examine-text-avoid-carrying-around", "[color=red][italic]Вам, вероятно, не стоит носить его с собой без веской причины.[/italic][/color]" },
		{ "contraband-examine-text-in-the-clear", "[color=green][italic]Вы должны быть чисты, чтобы носить этот предмет на виду.[/italic][/color]" }
	};
	private readonly HashSet<string> _contrabandItems = new HashSet<string>
	{
		"Emag", "ClothingHandsGlovesNorthStar", "Telecrystal1", "BibleNecronomicon", "HotPotato", "SupermatterGrenade", "WhiteholeGrenade", "EmpGrenade", "C4", "SyndieMiniBomb",
		"SyndiHypo", "EnergyDagger", "PenExploding", "Hypopen", "HolyHandGrenade", "TrashBananaPeelExplosive", "RubberStampSyndicate", "SoapSyndie", "SlipocalypseClusterSoap", "NocturineChemistryBottle",
		"Stimpack", "StimpackMini", "ExGrenade", "ChameleonProjector", "ClothingHandsGlovesBoxingRigged"
	};
	private readonly HashSet<string> _weaponItems = new HashSet<string>
	{
		"WeaponRifleAk", "WeaponLaserSvalinn", "WeaponLaserGun", "WeaponMakeshiftLaser", "WeaponTeslaGun", "WeaponLaserCarbine", "WeaponLaserCarbinePractice", "WeaponPulsePistol", "WeaponPulseCarbine", "WeaponPulseRifle",
		"WeaponLaserCannon", "WeaponParticleDecelerator", "WeaponXrayCannon", "WeaponDisabler", "WeaponDisablerSMG", "WeaponDisablerPractice", "WeaponTaser", "WeaponAntiqueLaser", "WeaponAdvancedLaser", "WeaponPistolCHIMP",
		"WeaponPistolCHIMPUpgraded", "WeaponBehonkerLaser", "BaseWeaponHeavyMachineGun", "WeaponMinigun", "BaseWeaponLauncher", "WeaponLauncherChinaLake", "WeaponLauncherRocket", "WeaponLauncherMultipleRocket", "WeaponLauncherPirateCannon", "WeaponTetherGun",
		"WeaponForceGun", "WeaponGrapplingGun", "WeaponTetherGunAdmin", "WeaponForceGunAdmin", "WeaponLauncherAdmemeMeteorLarge", "WeaponLauncherAdmemeImmovableRodSlow", "BaseWeaponLightMachineGun", "WeaponLightMachineGunL6", "WeaponLightMachineGunL6C", "BaseWeaponPistol",
		"WeaponPistolViper", "WeaponPistolCobra", "WeaponPistolMk58", "WeaponPistolN1984", "WeaponRifleAk", "WeaponRifleM90GrenadeLauncher", "WeaponRifleLecter", "WeaponRifleAk", "WeaponRifleM90GrenadeLauncher", "WeaponRifleLecter",
		"WeaponShotgunBulldog", "WeaponShotgunDoubleBarreled", "WeaponShotgunDoubleBarreledRubber", "WeaponShotgunEnforcer", "WeaponShotgunEnforcerRubber", "WeaponShotgunKammerer", "WeaponShotgunSawn", "WeaponShotgunSawnEmpty", "WeaponShotgunHandmade", "WeaponShotgunBlunderbuss",
		"WeaponShotgunImprovised", "WeaponShotgunImprovisedLoaded", "BaseWeaponRevolver", "WeaponRevolverDeckard", "WeaponRevolverInspector", "WeaponRevolverMateba", "WeaponRevolverPython", "WeaponRevolverPythonAP", "WeaponRevolverPirate", "WeaponSubMachineGunAtreides",
		"WeaponSubMachineGunC20r", "WeaponSubMachineGunDrozd", "WeaponSubMachineGunVector", "WeaponSubMachineGunWt550", "WeaponSniperMosin", "WeaponSniperHristov", "Musket", "WeaponPistolFlintlock", "EnergySword", "EnergySwordDouble"
	};
	private bool _hasContrabandComp;
	
	private Type _contrabandCompType;
}
