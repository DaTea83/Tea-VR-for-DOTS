using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace TeaFramework
{
    [UpdateInGroup(typeof(Tea_PhysicsSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct XRPlayerMovementISystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (move, rotate, 
                         crouch, lt) 
                     in SystemAPI.Query<RefRW<PlayerMovementIData>,RefRW<PlayerRotationIData>, 
                         RefRW<PlayerCrouchIData>, RefRW<LocalTransform>>())
            {
                if(move.ValueRO.BodyEntity == Entity.Null) continue;
                var hInput = SystemAPI.GetComponentRO<HeadInputIData>(move.ValueRO.Player);
                var lInput = SystemAPI.GetComponentRO<LHandInputIData>(move.ValueRO.Player);
                var rInput = SystemAPI.GetComponentRO<RHandInputIData>(move.ValueRO.Player);
                if (lInput.ValueRO.IsTracked == 0 && rInput.ValueRO.IsTracked == 0) continue;
                
                move.ValueRW.PreviousInput = move.ValueRO.CurrentInput;
                rotate.ValueRW.PreviousInput = rotate.ValueRO.CurrentInput;
                crouch.ValueRW.PreviousInput = crouch.ValueRO.CurrentInput;

                move.ValueRW.CurrentInput = move.ValueRO.JoystickInput.GetInputType(lInput, rInput, hInput);
                rotate.ValueRW.CurrentInput = rotate.ValueRO.JoystickInput.GetInputType(lInput, rInput);
                crouch.ValueRW.CurrentInput = crouch.ValueRO.JoystickInput.GetInputType(lInput, rInput);
                
                
            }
        }
    }
}
