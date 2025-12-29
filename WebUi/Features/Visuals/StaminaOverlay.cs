using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CerberusWareV3.Configuration;
using Content.Client.Damage.Systems;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using EntityManagerExt = Robust.Shared.GameObjects.EntityManagerExt;


[CompilerGenerated]
public sealed class StaminaOverlay : Overlay
{
    public StaminaOverlay()
    {
        IoCManager.InjectDependencies<StaminaOverlay>(this);
    }
    public override OverlaySpace Space
    {
        get
        {
            return (OverlaySpace)8;
        }
    }
    protected override void Draw(in OverlayDrawArgs args)
    {
        bool flag = !CerberusConfig.Hud.ShowStamina;
        if (!flag)
        {
            if (this._transformSystem == null)
            {
                this._transformSystem = this._entityManager.System<SharedTransformSystem>();
            }
            if (this._staminaSystem == null)
            {
                this._staminaSystem = this._entityManager.System<StaminaSystem>();
            }
            bool flag2 = this._transformSystem == null || this._staminaSystem == null;
            if (!flag2)
            {
                DrawingHandleWorld worldHandle = args.WorldHandle;
                IEye eye = args.Viewport.Eye;
                Angle angle = ((eye != null) ? eye.Rotation : Angle.Zero);
                EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
                Vector2 vector = new Vector2(1f, 1f);
                Matrix3x2 matrix3x = Matrix3Helpers.CreateScale(ref vector);
                Matrix3x2 matrix3x2 = Matrix3Helpers.CreateRotation(-angle);
                AllEntityQueryEnumerator<StaminaComponent, MobStateComponent, SpriteComponent> allEntityQueryEnumerator = this._entityManager.AllEntityQueryEnumerator<StaminaComponent, MobStateComponent, SpriteComponent>();
                for (; ; )
                {
                    EntityUid entityUid;
                    StaminaComponent staminaComponent;
                    MobStateComponent mobStateComponent;
                    SpriteComponent spriteComponent;
                    bool flag3 = allEntityQueryEnumerator.MoveNext(out entityUid, out staminaComponent, out mobStateComponent, out spriteComponent);
                    if (!flag3)
                    {
                        break;
                    }
                    TransformComponent transformComponent;
                    bool flag4 = !entityQuery.TryGetComponent(entityUid, out transformComponent) || transformComponent.MapID != args.MapId;
                    if (!flag4)
                    {
                        bool flag5 = mobStateComponent.CurrentState == (MobState)3; // Dead
                        if (!flag5)
                        {
                            StatusIconComponent componentOrNull = EntityManagerExt.GetComponentOrNull<StatusIconComponent>(this._entityManager, entityUid);
                            Box2 box = ((componentOrNull != null) ? componentOrNull.Bounds : null) ?? spriteComponent.Bounds;
                            Vector2 worldPosition = this._transformSystem.GetWorldPosition(transformComponent, entityQuery);
                            
                            Box2 worldAABB = args.WorldAABB;
                            bool isVisible = box.Translated(worldPosition).Intersects(ref worldAABB);

                            if (isVisible)
                            {
                                ValueTuple<float, bool>? valueTuple = this.CalcProgress(entityUid, this._staminaSystem, staminaComponent);
                                
                                if (valueTuple != null)
                                {
                                    var progressInfo = valueTuple.Value;
                                    float staminaRatio = progressInfo.Item1;
                                    // bool isCrit = progressInfo.Item2;

                                    Vector2 worldPosition2 = this._transformSystem.GetWorldPosition(transformComponent);
                                    Matrix3x2 matrix3x3 = Matrix3Helpers.CreateTranslation(worldPosition2);
                                    Matrix3x2 matrix3x4 = Matrix3x2.Multiply(matrix3x, matrix3x3);
                                    Matrix3x2 matrix3x5 = Matrix3x2.Multiply(matrix3x2, matrix3x4);
                                    worldHandle.SetTransform(ref matrix3x5);
                                    
                                    float num2 = box.Height * 32f / 2f;
                                    float num3 = box.Width * 32f;
                                    
                                    Vector2 vector2 = new Vector2(-num3 / 32f / 2f, num2 / 32f); 
                                    
                                    float barWidth = num3 - 8f;
                                    
                                    float filledWidth = barWidth * staminaRatio + 8f; 
                                    
                                    Box2 box2 = new Box2(new Vector2(8f, 3f) / 32f, new Vector2(barWidth + 8f, 6f) / 32f); 
                                    box2 = box2.Translated(vector2);
                                    worldHandle.DrawRect(box2, Color.Black.WithAlpha(192), true);
                                    
                                    Box2 box3 = new Box2(new Vector2(8f, 3f) / 32f, new Vector2(filledWidth, 6f) / 32f);
                                    box3 = box3.Translated(vector2);
                                    worldHandle.DrawRect(box3, new Color(ref CerberusConfig.Hud.StaminaColor), true);
                                    
                                    Box2 box4 = new Box2(new Vector2(8f, 5f) / 32f, new Vector2(filledWidth, 6f) / 32f);
                                    box4 = box4.Translated(vector2);
                                    worldHandle.DrawRect(box4, Color.Black.WithAlpha(128), true);
                                }
                            }
                        }
                    }
                }
                DrawingHandleBase drawingHandleBase = worldHandle;
                Matrix3x2 identity = Matrix3x2.Identity;
                drawingHandleBase.SetTransform(ref identity);
            }
        }
    }
    private ValueTuple<float, bool>? CalcProgress(EntityUid uid, StaminaSystem staminaSystem, StaminaComponent stam)
    {
        bool flag = stam.CritThreshold <= 0f;
        ValueTuple<float, bool>? valueTuple;
        if (flag)
        {
            valueTuple = null;
        }
        else
        {
            float staminaDamage = staminaSystem.GetStaminaDamage(uid, stam);
            float num = 1f - staminaDamage / stam.CritThreshold;
            valueTuple = new ValueTuple<float, bool>?(new ValueTuple<float, bool>(Math.Clamp(num, 0f, 1f), stam.Critical));
        }
        return valueTuple;
    }
    
    [Robust.Shared.IoC.Dependency]  private readonly IEntityManager _entityManager = null;
    
    private SharedTransformSystem _transformSystem;
    
    private StaminaSystem _staminaSystem;
}