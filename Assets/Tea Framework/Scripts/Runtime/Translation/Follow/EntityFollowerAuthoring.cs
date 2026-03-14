using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Translation/you to Entity Follower")]
    [DisallowMultipleComponent]
    public class EntityFollowerAuthoring : MonoBehaviour {
        public GameObject target;
        [Range(0f, 30f)] public float smoothFollowSpeed;
        public bool ignoreX;
        public bool ignoreY;
        public bool ignoreZ;
        public bool ignoreRotation;

        private class Baker : Baker<EntityFollowerAuthoring> {
            public override void Bake(EntityFollowerAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var target = GetEntity(authoring.target, TransformUsageFlags.Dynamic);

                AddComponent(entity, new TargetFollowerIEnableable {
                    Target = target,
                    SmoothFollowSpeed = authoring.smoothFollowSpeed,
                    IgnoreX = authoring.ignoreX,
                    IgnoreY = authoring.ignoreY,
                    IgnoreZ = authoring.ignoreZ,
                    IgnoreRotation = authoring.ignoreRotation
                });
                SetComponentEnabled<TargetFollowerIEnableable>(entity, false);
            }
        }
    }

    public struct TargetFollowerIEnableable : IComponentData, IEnableableComponent {
        public Entity Target;
        public float SmoothFollowSpeed;
        public bool IgnoreX;
        public bool IgnoreY;
        public bool IgnoreZ;
        public bool IgnoreRotation;
        public float3 Offset;
    }

    /// <summary>
    /// 
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    public partial struct TargetFollowISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (follow, lt)
                     in SystemAPI.Query<RefRO<TargetFollowerIEnableable>, RefRW<LocalTransform>>()
                         .WithAll<TargetFollowerIEnableable>()) {
                var targetLt = SystemAPI.GetComponent<LocalTransform>(follow.ValueRO.Target);
                var factor = follow.ValueRO.SmoothFollowSpeed > 0 ? follow.ValueRO.SmoothFollowSpeed * dt : 1f;

                var posX = follow.ValueRO.IgnoreX ? lt.ValueRO.Position.x : targetLt.Position.x;
                var posY = follow.ValueRO.IgnoreY ? lt.ValueRO.Position.y : targetLt.Position.y;
                var posZ = follow.ValueRO.IgnoreZ ? lt.ValueRO.Position.z : targetLt.Position.z;
                var newRot = follow.ValueRO.IgnoreRotation ? lt.ValueRO.Rotation : targetLt.Rotation;

                var offX = follow.ValueRO.IgnoreX ? 0f : follow.ValueRO.Offset.x;
                var offY = follow.ValueRO.IgnoreY ? 0f : follow.ValueRO.Offset.y;
                var offZ = follow.ValueRO.IgnoreZ ? 0f : follow.ValueRO.Offset.z;
                var offset = new float3(offX, offY, offZ);

                lt.ValueRW.Position = math.lerp(lt.ValueRO.Position, new float3(posX, posY, posZ) + offset, factor);
                lt.ValueRW.Rotation = math.slerp(lt.ValueRO.Rotation, newRot, factor);
            }
        }
    }
}