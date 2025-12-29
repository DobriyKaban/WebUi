using System;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.IoC;

[CompilerGenerated]
public class HudManager : EntitySystem
{
    // Используем стандартные зависимости
    [Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager = default!;
    [Robust.Shared.IoC.Dependency] private readonly IPlayerManager _playerManager = default!;
    private readonly ComponentManager _componentManager; // Оставляем ваш хелпер, если он работает

    public HudManager()
    {
        // Инициализируем хелпер. Если он требует IoC, он сам их подтянет
        _componentManager = new ComponentManager();
    }

    public override void Initialize()
    {
        base.Initialize();
        // Подписываемся на прикрепление игрока, чтобы сразу включить иконки
        SubscribeLocalEvent<LocalPlayerAttachedEvent>(OnLocalPlayerAttached);
    }

    private void OnLocalPlayerAttached(LocalPlayerAttachedEvent ev)
    {
        // При спавне сразу применяем настройки
        UpdateHudComponents(ev.Entity);
    }

    public override void Update(float frameTime)
    {
        // Каждый кадр (или реже, если оптимизировать) проверяем настройки
        // И применяем их к локальному игроку
        if (_playerManager.LocalEntity != null)
        {
            UpdateHudComponents(_playerManager.LocalEntity.Value);
        }
    }

    private void UpdateHudComponents(EntityUid entity)
    {
        // Проходимся по всем галочкам HUD из конфига
        // И добавляем/удаляем соответствующие компоненты
        
        SyncComponent(entity, "ShowAntagIcons", CerberusConfig.Hud.ShowAntag);
        SyncComponent(entity, "ShowJobIcons", CerberusConfig.Hud.ShowJobIcons);
        SyncComponent(entity, "ShowMindShieldIcons", CerberusConfig.Hud.ShowMindShieldIcons);
        SyncComponent(entity, "ShowCriminalRecordIcons", CerberusConfig.Hud.ShowCriminalRecordIcons);
        SyncComponent(entity, "ShowSyndicateIcons", CerberusConfig.Hud.ShowSyndicateIcons);
        
        // Chemical Analysis (SolutionScanner и ShowElectrocutionHUD часто идут вместе)
        SyncComponent(entity, "SolutionScanner", CerberusConfig.Hud.ChemicalAnalysis);
        SyncComponent(entity, "ShowElectrocutionHUD", CerberusConfig.Hud.ShowElectrocution);
        
        // ShowHealth и ShowStamina — это оверлеи, они работают отдельно в своих классах
        // Им компоненты на игроке не нужны (обычно), они читают конфиг сами.
    }

    private void SyncComponent(EntityUid uid, string componentName, bool isEnabled)
    {
        // Если галочка включена, а компонента нет -> добавляем
        if (isEnabled && !_componentManager.HasComponent(componentName, uid))
        {
            _componentManager.AddComponent(componentName, uid);
        }
        // Если галочка выключена, а компонент есть -> удаляем
        else if (!isEnabled && _componentManager.HasComponent(componentName, uid))
        {
            _componentManager.RemoveComponent(componentName, uid);
        }
    }
}