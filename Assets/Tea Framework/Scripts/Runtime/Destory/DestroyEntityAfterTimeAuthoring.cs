using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Destroy/Destroy After Time Tag")]
    [RequireComponent(typeof(DestroyEntityAuthoring))]
    [DisallowMultipleComponent]
    public class DestroyEntityAfterTimeAuthoring : MonoBehaviour
    {
        public float duration = 1f;

        internal class DestroyTimeBaker : Baker<DestroyEntityAfterTimeAuthoring>
        {
            public override void Bake(DestroyEntityAfterTimeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DestroyTimeIData { Value = authoring.duration});
            }
        }
    }

    public struct DestroyTimeIData : IComponentData
    {
        public float Value;
    }
    
    [UpdateInGroup(typeof(Tea_DestroySystemGroup))]
    [UpdateBefore(typeof(DestroyEntityISystem))]
    [BurstCompile]
    public partial struct DestroyAfterTimeISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            foreach (var (destroyTime, entity) 
                     in SystemAPI.Query<RefRW<DestroyTimeIData>>().WithEntityAccess())
            {
                destroyTime.ValueRW.Value -= dt;
                
                if (destroyTime.ValueRW.Value > 0) continue;
                SystemAPI.SetComponentEnabled<DestroyEntityIEnableableTag>(entity, true);
            }
        }
    }
}
