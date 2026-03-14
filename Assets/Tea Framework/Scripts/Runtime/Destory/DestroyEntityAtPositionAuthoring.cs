using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Destroy/Destroy At Position Tag")]
    [RequireComponent(typeof(DestroyEntityAuthoring))]
    [DisallowMultipleComponent]
    public class DestroyEntityAtPositionAuthoring : MonoBehaviour {
        public float3 position;
        [Range(0.01f, 10f)] public float tolerance = 0.2f;

        internal class DestroyPositionBaker : Baker<DestroyEntityAtPositionAuthoring> {
            public override void Bake(DestroyEntityAtPositionAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyPositionIData {
                    Position = authoring.position,
                    Tolerance = authoring.tolerance,
                    LastDistanceSq = float.MaxValue
                });
            }
        }
    }

    public struct DestroyPositionIData : IComponentData {
        public float3 Position;
        public float Tolerance;
        public float LastDistanceSq;
    }

    [UpdateInGroup(typeof(Tea_DestroySystemGroup))]
    [UpdateBefore(typeof(DestroyEntityISystem))]
    [BurstCompile]
    public partial struct DestroyAtPositionISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (destroyPos, ltw, entity)
                     in SystemAPI.Query<RefRW<DestroyPositionIData>, RefRO<LocalToWorld>>().WithEntityAccess()) {
                var dis = math.lengthsq(destroyPos.ValueRO.Position - ltw.ValueRO.Position);

                if (dis < destroyPos.ValueRO.Tolerance && dis > destroyPos.ValueRO.LastDistanceSq)
                    SystemAPI.SetComponentEnabled<DestroyEntityIEnableableTag>(entity, true);
                else
                    destroyPos.ValueRW.LastDistanceSq = dis;
            }
        }
    }
}