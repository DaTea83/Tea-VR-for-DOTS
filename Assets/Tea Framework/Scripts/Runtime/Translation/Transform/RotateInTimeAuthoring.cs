using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TeaFramework {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RotatorAuthoring))]
    public class RotateInTimeAuthoring : MonoBehaviour {
        public float duration = 1f;

        internal class Baker : Baker<RotateInTimeAuthoring> {
            public override void Bake(RotateInTimeAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RotateInTimeIData { Value = authoring.duration });
            }
        }
    }

    public struct RotateInTimeIData : IComponentData {
        public float Value;
    }

    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    [UpdateBefore(typeof(RotationISystem))]
    public partial struct RotateTaskISystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var (spinTime, entity)
                     in SystemAPI.Query<RefRW<RotateInTimeIData>>().WithEntityAccess()) {
                switch (spinTime.ValueRW.Value) {
                    case > 0:
                        spinTime.ValueRW.Value -= dt;
                        if (!SystemAPI.IsComponentEnabled<RotationIEnableable>(entity))
                            SystemAPI.SetComponentEnabled<RotationIEnableable>(entity, true);
                        break;
                    case <= 0:
                        SystemAPI.SetComponentEnabled<RotationIEnableable>(entity, false);
                        break;
                }
            }
        }
    }
}