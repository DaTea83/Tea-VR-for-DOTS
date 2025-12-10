using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
// ReSharper disable GrammarMistakeInComment

namespace TeaFramework
{
    public struct InitializePhysicsMassDataITag : IComponentData { }
    
    /// <summary>
    /// Stores the data cause changing to kinematic will set both data to zero
    /// </summary>
    /// <remarks>
    /// The relationship between mass and inverse mass is : IM = 1 / (M * 100)
    /// </remarks>
    public struct PhysicsMassIData : IComponentData
    {
        public float InverseMass;
        public float3 InverseInertia;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Tea_InitializationSystemGroup))]
    public partial struct InitializePhysicsMassISystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (mass, entity) 
                     in SystemAPI.Query<RefRO<PhysicsMass>>().WithAll<InitializePhysicsMassDataITag>()
                         .WithNone<PhysicsMassIData>().WithEntityAccess())
            {
                ecb.AddComponent(entity, new PhysicsMassIData
                {
                    InverseMass = mass.ValueRO.InverseMass,
                    InverseInertia = mass.ValueRO.InverseInertia
                });
                ecb.RemoveComponent<InitializePhysicsMassDataITag>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
    
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_InitializationSystemGroup))]
    [UpdateBefore(typeof(InitializePhysicsMassISystem))]
    public partial struct InitializeGravityFactorISystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (mass, entity) 
                     in SystemAPI.Query<RefRO<PhysicsMass>>().WithNone<PhysicsGravityFactor>().WithEntityAccess())
            {
                ecb.AddComponent(entity, new PhysicsGravityFactor
                {
                    Value = 1
                });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
