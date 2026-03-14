using Unity.Burst;
using Unity.Entities;

namespace TeaFramework {
    partial struct OverlapClimbISystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) { }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}