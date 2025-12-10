using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework
{
    [DisallowMultipleComponent]
    public class TransformTaskAuthoring : MonoBehaviour
    {
        [SerializeField] private float3 position;
        [SerializeField] private float3 rotation;
        [SerializeField][Min(0.001f)] private float scale = 1f;
        [SerializeField] private float duration;
        
        private class Baker : Baker<TransformTaskAuthoring>
        {
            public override void Bake(TransformTaskAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TransformTaskIEnableable
                {
                    Position = authoring.position,
                    Rotation = authoring.rotation,
                    Scale = authoring.scale,
                    TotalTime = authoring.duration
                });
            }
        }
    }

    public struct TransformTaskIEnableable : IComponentData, IEnableableComponent
    {
        public float3 Position;
        public float3 Rotation;
        public float Scale;
        public float TotalTime;
        public float CurrentTime;
    }

    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup), OrderLast = true)]
    public partial struct TransformTaskISystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var (transformData, lt, entity) 
                     in SystemAPI.Query<RefRW<TransformTaskIEnableable>, RefRW<LocalTransform>>()
                         .WithEntityAccess().WithAll<TransformTaskIEnableable>())
            {
                if (transformData.ValueRO.CurrentTime < transformData.ValueRO.TotalTime)
                {
                    transformData.ValueRW.CurrentTime += dt.SmoothFactor();
                    lt.ValueRW.Position = math.lerp(lt.ValueRO.Position, transformData.ValueRO.Position, transformData.ValueRO.CurrentTime / transformData.ValueRO.TotalTime);
                    lt.ValueRW.Rotation = math.slerp(lt.ValueRO.Rotation, quaternion.Euler(transformData.ValueRO.Rotation), transformData.ValueRO.CurrentTime / transformData.ValueRO.TotalTime);
                    lt.ValueRW.Scale = math.lerp(lt.ValueRO.Scale, transformData.ValueRO.Scale, transformData.ValueRO.CurrentTime / transformData.ValueRO.TotalTime);
                }
                else
                    SystemAPI.SetComponentEnabled<TransformTaskIEnableable>(entity, false);
            }
        }
    }
}
