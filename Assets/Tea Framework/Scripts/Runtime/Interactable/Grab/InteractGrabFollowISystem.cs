using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TeaFramework {
    [BurstCompile]
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup))]
    public partial struct InteractGrabFollowISystem : ISystem {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (iBuffer, data, lt)
                     in SystemAPI
                         .Query<DynamicBuffer<InteractGrabIBuffer>, RefRO<InteractableGrabIData>,
                             RefRW<LocalTransform>>()
                         .WithAll<InteractGrabFollowIEnableableTag>()) {
                var lwLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
                var factor = data.ValueRO.SmoothFollowSpeed >= 0 ? data.ValueRO.SmoothFollowSpeed * dt : 1f;
                var totalOffset = float3.zero;

                for (var i = 0; i < iBuffer.Length; i++) {
                    var grabber = iBuffer[i].GrabberEntity;
                    var grabLt = lwLookup[grabber];
                    totalOffset += iBuffer[i].Offset + grabLt.Position;
                }

                totalOffset /= iBuffer.Length;
                lt.ValueRW.Rotation = math.slerp(lt.ValueRO.Rotation, quaternion.EulerXYZ(float3.zero), factor);
                lt.ValueRW.Position = math.lerp(lt.ValueRO.Position, totalOffset, factor);
            }
        }
    }
}