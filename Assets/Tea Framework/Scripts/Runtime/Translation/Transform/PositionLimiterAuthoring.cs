using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework {
    [DisallowMultipleComponent]
    public class PositionLimiterAuthoring : MonoBehaviour {
        [SerializeField] private float3 limit;
        [SerializeField] private float3 offset;

        private class Baker : Baker<PositionLimiterAuthoring> {
            public override void Bake(PositionLimiterAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(e, new PositionLimiterIData {
                    Value = authoring.limit,
                    Offset = authoring.offset
                });
            }
        }
    }

    public struct PositionLimiterIData : IComponentData {
        public float3 Value;
        public float3 Offset;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup), OrderLast = true)]
    public partial struct PositionLimiterISystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (limit, lt)
                     in SystemAPI.Query<RefRO<PositionLimiterIData>, RefRW<LocalTransform>>()) {
                var posX = math.clamp(lt.ValueRO.Position.x, -limit.ValueRO.Value.x, limit.ValueRO.Value.x);
                var posY = math.clamp(lt.ValueRO.Position.y, -limit.ValueRO.Value.y, limit.ValueRO.Value.y);
                var posZ = math.clamp(lt.ValueRO.Position.z, -limit.ValueRO.Value.z, limit.ValueRO.Value.z);

                lt.ValueRW.Position = new float3(posX, posY, posZ) + limit.ValueRO.Offset;
            }
        }
    }
}