using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework {
    [DisallowMultipleComponent]
    public class LookTowardsAuthoring : MonoBehaviour {
        public Transform target;
        public float3 offset;
        public bool ignoreX;
        public bool ignoreY;
        public bool ignoreZ;

        public class Baker : Baker<LookTowardsAuthoring> {
            public override void Bake(LookTowardsAuthoring authoring) {
                DependsOn(authoring.target);
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var t = GetEntity(authoring.transform, TransformUsageFlags.Dynamic);
                AddComponent(e, new LookTowardsIData {
                    Target = t,
                    Offset = authoring.offset,
                    IgnoreX = authoring.ignoreX,
                    IgnoreY = authoring.ignoreY,
                    IgnoreZ = authoring.ignoreZ,
                });
            }
        }
    }

    public struct LookTowardsIData : IComponentData {
        public Entity Target;
        public float3 Offset;
        public bool IgnoreX;
        public bool IgnoreY;
        public bool IgnoreZ;
    }

    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup), OrderLast = true)]
    public partial struct LookTowardsISystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            foreach (var (look, lt)
                     in SystemAPI.Query<RefRO<LookTowardsIData>, RefRW<LocalTransform>>()) {
                var tLtw = SystemAPI.GetComponent<LocalToWorld>(look.ValueRO.Target);
                float3 fwd = math.normalizesafe(lt.ValueRO.Position - tLtw.Position, new float3(0, 0, 1));
                float3 up = new float3(0, 1, 0);
                var newEuler = math.Euler(quaternion.LookRotationSafe(fwd, up)) + look.ValueRO.Offset;
                var newRad = new float3(look.ValueRO.IgnoreX ? lt.ValueRO.Rotation.GetEuler().x : newEuler.x,
                    look.ValueRO.IgnoreY ? lt.ValueRO.Rotation.GetEuler().y : newEuler.y,
                    look.ValueRO.IgnoreZ ? lt.ValueRO.Rotation.GetEuler().z : newEuler.z);
                var newRot = quaternion.Euler(newRad);

                lt.ValueRW.Rotation = newRot;
            }
        }
    }
}