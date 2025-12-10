using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Destroy/Instant Destroy Tag")]
    [DisallowMultipleComponent]
    public class DestroyEntityAuthoring : MonoBehaviour
    {
        internal class DestroyEntityBaker : Baker<DestroyEntityAuthoring>
        {
            public override void Bake(DestroyEntityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                this.AddSetIEnableableComponent<DestroyEntityIEnableableTag>(entity, false);
            }
        }
    }
    
    public struct DestroyEntityIEnableableTag : IComponentData, IEnableableComponent { }

    [UpdateInGroup(typeof(Tea_DestroySystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct DestroyEntityISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var endFrameECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, entity) 
                     in SystemAPI.Query<RefRO<DestroyEntityIEnableableTag>>()
                         .WithAll<DestroyEntityIEnableableTag>().WithEntityAccess())
            {
                endFrameECB.DestroyEntity(entity);
            }
        }
    }
}
