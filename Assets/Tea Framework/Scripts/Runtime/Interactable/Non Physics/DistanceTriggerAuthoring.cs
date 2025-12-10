using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework
{
    [DisallowMultipleComponent]
    public class DistanceTriggerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField][Min(0.001f)] private float threshold = 0.1f;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, threshold);
        }

        private class Baker : Baker<DistanceTriggerAuthoring>
        {
            public override void Bake(DistanceTriggerAuthoring authoring)
            {
                DependsOn(authoring.target);
                
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var t = GetEntity(authoring.target, TransformUsageFlags.Dynamic);
                
                AddComponent(e, new DistanceTriggerIData
                {
                    TrackingEntity = t,
                    Threshold = authoring.threshold
                });
            }
        }
    }

    public struct DistanceTriggerIData : IComponentData
    {
        public Entity TrackingEntity;
        public float Threshold;
        public bool InRange;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    public partial struct DistanceTriggerISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (trigger, ltw) 
                     in SystemAPI.Query<RefRW<DistanceTriggerIData>, RefRO<LocalToWorld>>())
            {
                var tLtw = SystemAPI.GetComponent<LocalToWorld>(trigger.ValueRO.TrackingEntity);
                var dis = math.length(ltw.ValueRO.GetDir(tLtw));
                trigger.ValueRW.InRange = dis <= trigger.ValueRO.Threshold;
            }
        }
    }
}
