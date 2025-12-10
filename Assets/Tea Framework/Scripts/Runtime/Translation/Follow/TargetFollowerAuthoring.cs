using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework
{
    /// <summary>
    /// This authoring is for an Entity in subscene to follow this entity
    /// </summary>
    /// <remarks>
    /// For game object to follow an entity refer to <see cref="ObjFollowerAuthoring"/>>
    /// </remarks>>
    [AddComponentMenu("Tea Framework/Translation/Entity to you Follower")]
    [DisallowMultipleComponent]
    public class TargetFollowerAuthoring : MonoBehaviour
    {
        public GameObject target;
        [Range(0f, 30f)]
        public float smoothFollowSpeed;
        public bool ignoreX;
        public bool ignoreY;
        public bool ignoreZ;
        public bool ignoreRotation;
        
        internal class Baker : Baker<TargetFollowerAuthoring>
        {
            public override void Bake(TargetFollowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var target = GetEntity(authoring.target, TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new FollowTargetIEnableable
                {
                    Target = target,
                    SmoothFollowSpeed = authoring.smoothFollowSpeed,
                    IgnoreX = authoring.ignoreX,
                    IgnoreY = authoring.ignoreY,
                    IgnoreZ = authoring.ignoreZ,
                    IgnoreRotation = authoring.ignoreRotation
                });
            }
        }
    }

    public struct FollowTargetIEnableable : IComponentData, IEnableableComponent
    {
        public Entity Target;
        public float SmoothFollowSpeed;
        public bool IgnoreX;
        public bool IgnoreY;
        public bool IgnoreZ;
        public bool IgnoreRotation;
    }

    /// <summary>
    /// 
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    public partial struct FollowTargetISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            foreach (var (ltw, follow) 
                     in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<FollowTargetIEnableable>>())
            {
                var targetLt = SystemAPI.GetComponent<LocalTransform>(follow.ValueRO.Target);
                var factor = follow.ValueRO.SmoothFollowSpeed > 0 ? follow.ValueRO.SmoothFollowSpeed * dt : 1f;
                
                var posX = follow.ValueRO.IgnoreX ? targetLt.Position.x : ltw.ValueRO.Position.x;
                var posY = follow.ValueRO.IgnoreY ? targetLt.Position.y : ltw.ValueRO.Position.y;
                var posZ = follow.ValueRO.IgnoreZ ? targetLt.Position.z : ltw.ValueRO.Position.z;
                var newRot = follow.ValueRO.IgnoreRotation ? targetLt.Rotation : ltw.ValueRO.Rotation;

                targetLt.Position = math.lerp(targetLt.Position, new float3(posX, posY, posZ), factor);
                targetLt.Rotation = math.slerp(targetLt.Rotation, newRot, factor);
                
                SystemAPI.SetComponent(follow.ValueRO.Target, targetLt);
            }
        }
    }
}
