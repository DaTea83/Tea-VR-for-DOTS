using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TeaFramework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EntityFollowerAuthoring))]
    public sealed class DistanceInteractAuthoring : MonoBehaviour
    {
        [SerializeField] private PlayerTagAuthoring player;
        [SerializeField] private GameObject target;
        [SerializeField] private EButtonType inputType;
        [SerializeField][Min(0.001f)] private float threshold = 0.1f;
        [SerializeField] private float3 initialPos;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, threshold);
        }

        public class Baker : Baker<DistanceInteractAuthoring>
        {
            public override void Bake(DistanceInteractAuthoring authoring)
            {
                DependsOn(authoring.target);
                DependsOn(authoring.player);
                
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var t = GetEntity(authoring.target, TransformUsageFlags.Dynamic);
                var p = GetEntity(authoring.player, TransformUsageFlags.Dynamic);
                
                AddComponent(e, new DistanceInteractIData
                {
                    TrackingEntity = t,
                    InputType = authoring.inputType,
                    Threshold = authoring.threshold,
                    InitialPosition = authoring.initialPos,
                    Player = p
                });
            }
        }
    }
    
    public struct DistanceInteractIData : IComponentData
    {
        public Entity Player;
        public Entity TrackingEntity;
        public EButtonType InputType;
        public float Threshold;
        public float3 InitialPosition;
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    [UpdateBefore(typeof(TargetFollowISystem))]
    public partial struct DistanceInteractISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            
            foreach (var (interact, lt, 
                         ltw, entity) 
                     in SystemAPI.Query<RefRO<DistanceInteractIData>, RefRW<LocalTransform>, 
                             RefRO<LocalToWorld>>().WithEntityAccess())
            {
                var lInput = SystemAPI.GetComponentRO<LHandInputIData>(interact.ValueRO.Player);
                var rInput = SystemAPI.GetComponentRO<RHandInputIData>(interact.ValueRO.Player);
                if (lInput.ValueRO.IsTracked == 0 && rInput.ValueRO.IsTracked == 0) continue;
                var input = interact.ValueRO.InputType.GetButtonValues(lInput, rInput);
                
                var tLtw = SystemAPI.GetComponent<LocalToWorld>(interact.ValueRO.TrackingEntity);
                var tLt = SystemAPI.GetComponent<LocalTransform>(interact.ValueRO.TrackingEntity);
                var dis = math.length(ltw.ValueRO.GetDir(tLtw));

                if (input > 0.5f)
                {
                    if(dis > interact.ValueRO.Threshold) continue;
                    if (SystemAPI.IsComponentEnabled<TargetFollowerIEnableable>(entity)) continue;
                    SystemAPI.SetComponentEnabled<TargetFollowerIEnableable>(entity, true);
                    var follow = SystemAPI.GetComponent<TargetFollowerIEnableable>(entity);
                    follow.Offset = lt.ValueRO.GetDir(tLt);
                    SystemAPI.SetComponent(entity, follow);
                } 
                else
                {
                    if(SystemAPI.IsComponentEnabled<TargetFollowerIEnableable>(entity))
                        SystemAPI.SetComponentEnabled<TargetFollowerIEnableable>(entity, false);

                    lt.ValueRW.Position = math.lerp(lt.ValueRO.Position, interact.ValueRO.InitialPosition, dt * 10);
                }
            }
        }
    }
}
