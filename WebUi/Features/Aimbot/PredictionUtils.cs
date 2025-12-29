using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;


[CompilerGenerated]
public static class PredictionUtils
{
	public static bool _isInitialized { get; private set; }
	
	public static void InitializeDependencies(IClientNetManager netManager, IGameTiming gameTiming, IConfigurationManager cfgManager, IEntityManager entityManager)
	{
		PredictionUtils._entityManager = entityManager;
		PredictionUtils._transformSystem = PredictionUtils._entityManager.System<SharedTransformSystem>();
		PredictionUtils._physicsSystem = PredictionUtils._entityManager.System<SharedPhysicsSystem>();
		PredictionUtils._isInitialized = true;
	}
	public static Vector2 GetPredictedWorldShootPosition(EntityUid shooterUid, EntityUid targetUid, float projectileSpeed, int maxIterations = 5)
	{
		bool flag = !PredictionUtils._isInitialized || PredictionUtils._entityManager == null || PredictionUtils._transformSystem == null || PredictionUtils._physicsSystem == null;
		Vector2 vector;
		if (flag)
		{
			vector = Vector2.Zero;
		}
		else
		{
			TransformComponent transformComponent;
			TransformComponent transformComponent2 = null;
			PhysicsComponent physicsComponent = null;
			bool flag2 = !PredictionUtils._entityManager.TryGetComponent<TransformComponent>(shooterUid, out transformComponent) || !PredictionUtils._entityManager.TryGetComponent<TransformComponent>(targetUid, out transformComponent2) || !PredictionUtils._entityManager.TryGetComponent<PhysicsComponent>(targetUid, out physicsComponent);
			if (flag2)
			{
				vector = Vector2.Zero;
			}
			else
			{
				Vector2 worldPosition = PredictionUtils._transformSystem.GetWorldPosition(transformComponent);
				Vector2 worldPosition2 = PredictionUtils._transformSystem.GetWorldPosition(transformComponent2);
				Vector2 mapLinearVelocity = PredictionUtils._physicsSystem.GetMapLinearVelocity(targetUid, physicsComponent, transformComponent2);
				bool flag3 = projectileSpeed <= 0.001f;
				if (flag3)
				{
					vector = worldPosition2;
				}
				else
				{
					float num = 0f;
					Vector2 vector2 = worldPosition2;
					for (int i = 0; i < maxIterations; i++)
					{
						float num2 = (vector2 - worldPosition).Length();
						bool flag4 = num2 < 0.001f;
						if (flag4)
						{
							vector2 = worldPosition2;
							break;
						}
						float num3 = num2 / projectileSpeed;
						bool flag5 = i > 0 && Math.Abs(num3 - num) < 0.001f;
						if (flag5)
						{
							break;
						}
						num = num3;
						vector2 = worldPosition2 + mapLinearVelocity * num;
					}
					vector = vector2;
				}
			}
		}
		return vector;
	}
	[Robust.Shared.IoC.Dependency] private static IEntityManager _entityManager;
	private static SharedTransformSystem _transformSystem;
	private static SharedPhysicsSystem _physicsSystem;
}
