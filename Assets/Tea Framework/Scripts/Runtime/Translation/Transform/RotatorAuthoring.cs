using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using RotationOrder = Unity.Mathematics.math.RotationOrder;

namespace TeaFramework {
    [DisallowMultipleComponent]
    [AddComponentMenu("Tea Framework/Translation/Rotator")]
    public class RotatorAuthoring : MonoBehaviour {
        public RotationOrder rotationOrder;
        public float3 rotationSpeed;

        private class RotatorBaker : Baker<RotatorAuthoring> {
            public override void Bake(RotatorAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RotationIEnableable {
                    RotationType = authoring.rotationOrder,
                    RotationSpeed = authoring.rotationSpeed,
                });
            }
        }
    }

    public struct RotationIEnableable : IComponentData, IEnableableComponent {
        public RotationOrder RotationType;
        public float3 RotationSpeed;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    public partial struct RotationISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var (lt, data)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationIEnableable>>()
                         .WithAll<RotationIEnableable>()) {
                var currentRotation = quaternion.Euler(data.ValueRO.RotationSpeed * dt, data.ValueRO.RotationType);
                lt.ValueRW = lt.ValueRW.Rotate(currentRotation);
            }
        }
    }
}