using System;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Weapons.Melee;
using Content.Shared.CombatMode;
using Content.Shared.Weapons.Melee;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;


[CompilerGenerated]
public sealed class MeleeHelperSystem : EntitySystem
{
	public override void Update(float frameTime)
	{
		bool flag = !CerberusConfig.MeleeHelper.Enabled;
		if (!flag)
		{
			EntityUid? localEntity = this._playerManager.LocalEntity;
			CombatModeComponent combatModeComponent;
			bool flag2 = localEntity == null || !base.TryComp<CombatModeComponent>(localEntity.Value, out combatModeComponent) || !combatModeComponent.IsInCombatMode;
			if (!flag2)
			{
				EntityUid entityUid;
				MeleeWeaponComponent meleeWeaponComponent;
				bool flag3 = !this._meleeWeaponSystem.TryGetWeapon(localEntity.Value, out entityUid,out meleeWeaponComponent);
				if (!flag3)
				{
					bool attack = CerberusConfig.MeleeHelper.Attack360;
					if (attack)
					{
						meleeWeaponComponent.Angle = 360f;
					}
					bool autoAttack = CerberusConfig.MeleeHelper.AutoAttack;
					if (autoAttack)
					{
						meleeWeaponComponent.AutoAttack = true;
					}
				}
			}
		}
	}
	
	[Robust.Shared.IoC.Dependency] private readonly MeleeWeaponSystem _meleeWeaponSystem = null;
	
	[Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = null;
}
