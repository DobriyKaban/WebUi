using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using CerberusWareV3.Configuration;
using Content.Client.StatusIcon;
using Content.Client.UserInterface.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusIcon.Components;
using HarmonyLib;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using EntityManagerExt = Robust.Shared.GameObjects.EntityManagerExt;

// Убедитесь, что используете правильное пространство имен для FixedPoint2
// Если FixedPoint2.cs в глобальном пространстве, то using не нужен.

public class HealthBarOverlay : Overlay
{
    [Robust.Shared.IoC.Dependency] private readonly IEntityManager _entityManager;
    private readonly SharedTransformSystem _transformSystem;
    private readonly MobStateSystem _mobStateSystem;
    private readonly MobThresholdSystem _mobThresholdSystem;
    private readonly StatusIconSystem _statusIconSystem;
    private readonly ProgressColorSystem _progressColorSystem;

    public HealthBarOverlay(IEntityManager entity)
    {
        IoCManager.InjectDependencies(this);
        this._entityManager = entity;
        this._transformSystem = this._entityManager.System<SharedTransformSystem>();
        this._mobStateSystem = this._entityManager.System<MobStateSystem>();
        this._mobThresholdSystem = this._entityManager.System<MobThresholdSystem>();
        this._statusIconSystem = this._entityManager.System<StatusIconSystem>();
        this._progressColorSystem = this._entityManager.System<ProgressColorSystem>();
    }

    public override OverlaySpace Space => (OverlaySpace)8; // ScreenSpace

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (!CerberusConfig.Hud.ShowHealth) return;

        DrawingHandleWorld worldHandle = args.WorldHandle;
        IEye eye = args.Viewport.Eye;
        Angle angle = (eye != null) ? eye.Rotation : Angle.Zero;

        EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
        Vector2 scale = new Vector2(1f, 1f);
        Matrix3x2 scaleMatrix = Matrix3Helpers.CreateScale(ref scale);
        Matrix3x2 rotationMatrix = Matrix3Helpers.CreateRotation(-angle);

        // Используем более общий запрос, чтобы точно найти всех мобов
        var query = this._entityManager.AllEntityQueryEnumerator<MobStateComponent, DamageableComponent, SpriteComponent>();

        while (query.MoveNext(out EntityUid entityUid, out var mobStateComponent, out var damageableComponent, out var spriteComponent))
        {
            try
            {
                if (!entityQuery.TryGetComponent(entityUid, out var transformComponent) || transformComponent.MapID != args.MapId)
                    continue;

                // Пропускаем мертвых, если не хотим захламлять экран
                if (mobStateComponent.CurrentState == MobState.Dead) continue;

                // Получаем пороги (опционально)
                _entityManager.TryGetComponent(entityUid, out MobThresholdsComponent thresholds);

                var progressInfo = this.CalcProgress(entityUid, mobStateComponent, damageableComponent, thresholds);

                if (progressInfo != null)
                {
                    var (ratio, inCrit) = progressInfo.Value;

                    StatusIconComponent statusIconComponent = EntityManagerExt.GetComponentOrNull<StatusIconComponent>(this._entityManager, entityUid);
                    Box2 box = (statusIconComponent?.Bounds) ?? spriteComponent.Bounds;

                    Vector2 worldPos = this._transformSystem.GetWorldPosition(transformComponent);
                    Matrix3x2 translationMatrix = Matrix3Helpers.CreateTranslation(worldPos);
                    Matrix3x2 transformMatrix = Matrix3x2.Multiply(scaleMatrix, translationMatrix);
                    transformMatrix = Matrix3x2.Multiply(rotationMatrix, transformMatrix);

                    worldHandle.SetTransform(ref transformMatrix);

                    // Размеры полоски
                    float height = box.Height * 32f / 2f - 3f;
                    float width = Math.Max(box.Width * 32f, 32f); // Минимальная ширина 32px
                    Vector2 baseOffset = new Vector2(-width / 32f / 2f, height / 32f);

                    Color progressColor = this.GetProgressColor(ratio, inCrit);

                    float barWidth = width - 8f;
                    float filledWidth = barWidth * ratio + 8f;
                    
                    // Фон (черный)
                    Box2 bgBox = new Box2(new Vector2(8f, 0f) / 32f, new Vector2(barWidth + 8f, 4f) / 32f);
                    bgBox = bgBox.Translated(baseOffset);
                    worldHandle.DrawRect(bgBox, Color.Black.WithAlpha(192), true);
                    
                    // Заполнение (цветное)
                    if (filledWidth > 8f) // Рисуем только если есть здоровье
                    {
                        Box2 fgBox = new Box2(new Vector2(8f, 0f) / 32f, new Vector2(filledWidth, 4f) / 32f);
                        fgBox = fgBox.Translated(baseOffset);
                        worldHandle.DrawRect(fgBox, progressColor, true);
                    }
                    
                    // Тень (для объема)
                    Box2 shadowBox = new Box2(new Vector2(8f, 3f) / 32f, new Vector2(filledWidth, 4f) / 32f);
                    shadowBox = shadowBox.Translated(baseOffset);
                    worldHandle.DrawRect(shadowBox, Color.Black.WithAlpha(128), true);
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки отрисовки для конкретной сущности, чтобы не крашить весь оверлей
            }
        }

        Matrix3x2 identity = Matrix3x2.Identity;
        worldHandle.SetTransform(ref identity);
    }

    private (float ratio, bool inCrit)? CalcProgress(EntityUid uid, MobStateComponent comp, DamageableComponent dmg, MobThresholdsComponent thresholds)
    {
        // 1. Получаем текущий урон (TotalDamage)
        float currentDamage = 0f;
        try 
        {
            // Пытаемся получить свойство TotalDamage
            PropertyInfo prop = dmg.GetType().GetProperty("TotalDamage");
            if (prop != null)
            {
                var val = prop.GetValue(dmg);
                currentDamage = FixedPoint2.FromObject(val).ToFloat();
            }
            else
            {
                // Если свойства нет, ищем поле
                FieldInfo field = dmg.GetType().GetField("TotalDamage");
                if (field != null)
                {
                    var val = field.GetValue(dmg);
                    currentDamage = FixedPoint2.FromObject(val).ToFloat();
                }
                else
                {
                    return null; 
                }
            }
        }
        catch { return null; }

        // 2. Получаем пороги (Critical и Dead)
        float critThreshold = 0f;
        float deadThreshold = 0f;

        if (thresholds != null)
        {
            try 
            {
                FieldInfo dictField = AccessTools.Field(typeof(MobThresholdsComponent), "_thresholds");
                if (dictField != null)
                {
                    var dict = dictField.GetValue(thresholds) as System.Collections.IDictionary;
                    if (dict != null)
                    {
                        foreach (System.Collections.DictionaryEntry entry in dict)
                        {
                            var state = (MobState)entry.Value;
                            var val = FixedPoint2.FromObject(entry.Key).ToFloat();
                            
                            if (state == MobState.Critical) critThreshold = val;
                            if (state == MobState.Dead) deadThreshold = val;
                        }
                    }
                }
            }
            catch { /* Ignore */ }
        }

        // --- ФОЛЛБЭК ЗНАЧЕНИЯ ---
        // Если пороги не найдены (0), ставим дефолтные для человека
        if (deadThreshold <= 0.1f) deadThreshold = 100f; // Обычно 100 или 200 урона = смерть
        if (critThreshold <= 0.1f) critThreshold = deadThreshold; // Если нет крита, то сразу смерть

        // 3. Расчет
        if (_mobStateSystem.IsAlive(uid, comp))
        {
            // Жив: Полоска от 0 до CritThreshold
            // Если порог крита == порогу смерти, то до смерти.
            float maxHP = critThreshold;
            
            // Защита от деления на 0
            if (maxHP <= 0.1f) maxHP = 100f; 

            float ratio = 1f - (currentDamage / maxHP);
            return (Math.Clamp(ratio, 0f, 1f), false);
        }
        else if (_mobStateSystem.IsCritical(uid, comp))
        {
            // В крите: Полоска от CritThreshold до DeadThreshold
            float damageInCrit = currentDamage - critThreshold;
            float critRange = deadThreshold - critThreshold;

            if (critRange <= 0.1f) return (0f, true); // Почти мертв

            float ratio = 1f - (damageInCrit / critRange);
            return (Math.Clamp(ratio, 0f, 1f), true);
        }
        
        return null;
    }

    public Color GetProgressColor(float progress, bool crit)
    {
        if (crit)
        {
            // Мигающий красный или просто красный для крита
            return Color.Red; 
        }
        
        // Градиент от Зеленого (1.0) к Красному (0.0)
        if (progress > 0.5f)
        {
            // От 0.5 до 1.0 -> Желтый к Зеленому
            // Lerp(Yellow, Green, (p - 0.5) * 2)
            return Color.InterpolateBetween(Color.Yellow, Color.LimeGreen, (progress - 0.5f) * 2f);
        }
        else
        {
            // От 0.0 до 0.5 -> Красный к Желтому
            // Lerp(Red, Yellow, p * 2)
            return Color.InterpolateBetween(Color.Red, Color.Yellow, progress * 2f);
        }
    }
}