using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace TeaFramework
{
    /// <summary>
    /// This system is for check grabber input and has grabber overlap with interactable,
    /// if yes add it into the buffer
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PostTransformSystemGroup))]
    public partial struct OverlapGrabInteractISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<LHandInputIData>();
            state.RequireForUpdate<RHandInputIData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var lInput = SystemAPI.GetSingleton<LHandInputIData>();
            var rInput = SystemAPI.GetSingleton<RHandInputIData>();
            var sim   = SystemAPI.GetSingleton<SimulationSingleton>();
            
            if (lInput.IsTracked == 0 && rInput.IsTracked == 0) return;

            foreach (var (buffer, data, active)
                     in SystemAPI.Query<DynamicBuffer<GrabberEntitiesIBuffer>, RefRO<GrabberTriggerIData>, 
                         RefRW<GrabberActiveIData>>())
            {
                buffer.Clear();
            }
            
            state.Dependency = new TriggerEventsJob
            {
                GrabberLookup = SystemAPI.GetComponentLookup<GrabberTriggerIData>(true),
                ActiveLookup = SystemAPI.GetComponentLookup<GrabberActiveIData>(true),
                InteractableLookup = SystemAPI.GetComponentLookup<InteractableGrabIData>(true),
                LtwLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
                BufferLookup = SystemAPI.GetBufferLookup<GrabberEntitiesIBuffer>()
                
            }.Schedule(sim, state.Dependency);
        }

        [BurstCompile]
        private struct TriggerEventsJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentLookup<GrabberTriggerIData> GrabberLookup;
            [ReadOnly] public ComponentLookup<GrabberActiveIData> ActiveLookup;
            [ReadOnly] public ComponentLookup<InteractableGrabIData> InteractableLookup;
            [ReadOnly] public ComponentLookup<LocalToWorld> LtwLookup;
            
            public BufferLookup<GrabberEntitiesIBuffer> BufferLookup;
            
            public void Execute(TriggerEvent triggerEvent)
            {
                var (grabber, interactable) = triggerEvent.GetSimulationEntities(GrabberLookup, InteractableLookup);
                if (grabber == Entity.Null || interactable == Entity.Null) return;

                if(ActiveLookup[grabber].CurrentInput < GrabberLookup[grabber].Threshold) return;
                MathCollection.GetDistanceAndDot(LtwLookup[grabber], LtwLookup[interactable],  
                    out var distance, out var dot);
                
                BufferLookup[grabber].Add(new GrabberEntitiesIBuffer
                {
                    Interactable = interactable,
                    DisSqr = distance,
                    Dot = dot
                });
            }
        }
    }
}
