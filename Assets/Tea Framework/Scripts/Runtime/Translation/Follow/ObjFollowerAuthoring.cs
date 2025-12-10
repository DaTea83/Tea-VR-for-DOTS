using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework
{
    /// <summary>
    /// This authoring is for game object to follow this entity
    /// </summary>
    /// <remarks>
    /// For entity to follow entity see <see cref="TargetFollowerAuthoring"/>>
    /// </remarks>>
    [AddComponentMenu("Tea Framework/Translation/Unity Object Follower")]
    public class ObjFollowerAuthoring : MonoBehaviour
    {
        public Transform target;
        public float3 targetOffset;
        [Range(0f, 30f)]
        public float smoothFollowSpeed;
        
        internal class Baker : Baker<ObjFollowerAuthoring>
        {
            public override void Bake(ObjFollowerAuthoring authoring)
            {
                if (authoring.target is null) return;
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ObjTransformIData
                {
                    Transform = new UnityObjectRef<Transform>
                    {
                        Value = authoring.target
                    },
                    Offset = authoring.targetOffset,
                    SmoothFollowSpeed = authoring.smoothFollowSpeed
                });
            }
        }
    }
    
    public struct ObjTransformIData : IComponentData
    {
        public UnityObjectRef<Transform> Transform;
        public float3 Offset;
        public float SmoothFollowSpeed;
    }
    
    /// <summary>
    /// Put it in EffectSystemGroup, if put in translation the camera will fell janky 
    /// </summary>
    [UpdateInGroup(typeof(Tea_EffectSystemGroup), OrderFirst = true)]
    public partial struct ObjFollowerISystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            foreach (var (ltw, objTransformRef) 
                     in SystemAPI.Query<RefRO<LocalToWorld>, RefRW<ObjTransformIData>>())
            {
                var factor = objTransformRef.ValueRO.SmoothFollowSpeed > 0 ? objTransformRef.ValueRO.SmoothFollowSpeed * dt : 1;
                var obj = objTransformRef.ValueRW.Transform.Value;
                
                obj.position = math.lerp(obj.position, ltw.ValueRO.Position + objTransformRef.ValueRO.Offset, factor);
                obj.rotation = math.slerp(obj.rotation, ltw.ValueRO.Rotation, factor);
            }
        }
    }
}
