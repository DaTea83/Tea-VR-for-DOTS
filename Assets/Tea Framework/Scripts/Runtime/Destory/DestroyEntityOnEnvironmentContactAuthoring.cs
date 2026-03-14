using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Destroy/Destroy On Contact Tag")]
    [RequireComponent(typeof(DestroyEntityAuthoring))]
    [DisallowMultipleComponent]
    public class DestroyEntityOnEnvironmentContactAuthoring : MonoBehaviour {
        internal class DestroyEnvironmentBaker : Baker<DestroyEntityOnEnvironmentContactAuthoring> {
            public override void Bake(DestroyEntityOnEnvironmentContactAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DestroyEnvironmentITag>(entity);
            }
        }
    }

    public struct DestroyEnvironmentITag : IComponentData { }

    [UpdateInGroup(typeof(Tea_DestroySystemGroup))]
    [UpdateBefore(typeof(DestroyEntityISystem))]
    [BurstCompile]
    public partial struct DestroyOnEnvironmentContactISystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { state.RequireForUpdate<SimulationSingleton>(); }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var sim = SystemAPI.GetSingleton<SimulationSingleton>();
            var job = new CheckCollisionIJob {
                EnvironmentLookup = SystemAPI.GetComponentLookup<EnvironmentITag>(true),
                DestroyLookup = SystemAPI.GetComponentLookup<DestroyEnvironmentITag>(true),
                DestroyEntityLookup = SystemAPI.GetComponentLookup<DestroyEntityIEnableableTag>()
            };
            state.Dependency = job.Schedule(sim, state.Dependency);
        }

        [BurstCompile]
        private struct CheckCollisionIJob : ICollisionEventsJob {
            [ReadOnly] public ComponentLookup<EnvironmentITag> EnvironmentLookup;
            [ReadOnly] public ComponentLookup<DestroyEnvironmentITag> DestroyLookup;

            public ComponentLookup<DestroyEntityIEnableableTag> DestroyEntityLookup;

            public void Execute(CollisionEvent collisionEvent) {
                var (entity, environment) = collisionEvent.GetSimulationEntities(EnvironmentLookup, DestroyLookup);
                if (entity == Entity.Null || environment == Entity.Null) return;

                DestroyEntityLookup.SetComponentEnabled(entity, true);
            }
        }
    }
}