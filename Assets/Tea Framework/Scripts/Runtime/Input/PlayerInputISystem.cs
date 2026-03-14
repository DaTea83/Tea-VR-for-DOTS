using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TeaFramework {
    /// <summary>
    /// System that only RW the values from an input system 
    /// </summary>
    [UpdateInGroup(typeof(Tea_InitializationSystemGroup), OrderLast = true)]
    public partial struct PlayerInputISystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            if (XRInputController.Instance is null || XROriginController.Instance is null) return;
            if (XRInputController.Instance.Input is null) return;

            var input = XRInputController.Instance.Input;
            var deadX = XROriginController.Instance.deadZoneX;
            var deadY = XROriginController.Instance.deadZoneY;

            foreach (var (_, hInput, lInput,
                         rInput, uInput, entity)
                     in SystemAPI.Query<RefRO<PlayerITag>, RefRW<HeadInputIData>,
                             RefRW<LHandInputIData>, RefRW<RHandInputIData>, RefRW<UIInputIData>>()
                         .WithEntityAccess()) {
                hInput.ValueRW.IsTracked = input.XRHead.IsTracked.ReadValue<float>();
                hInput.ValueRW.TrackingState = input.XRHead.TrackingState.ReadValue<int>();
                hInput.ValueRW.HeadPos = input.XRHead.Position.ReadValue<Vector3>();
                hInput.ValueRW.HeadRot = input.XRHead.Rotation.ReadValue<Vector3>();

                lInput.ValueRW.IsTracked = input.XRLHand.IsTracked.ReadValue<float>();
                lInput.ValueRW.TrackingState = input.XRLHand.TrackingState.ReadValue<int>();
                lInput.ValueRW.LHandPos = input.XRLHand.HandPosition.ReadValue<Vector3>();
                lInput.ValueRW.LHandRot = input.XRLHand.HandRotation.ReadValue<Quaternion>();
                lInput.ValueRW.LHandGrip = input.XRLHand.GripSelect.ReadValue<float>();
                lInput.ValueRW.LHandTrigger = input.XRLHand.TriggerSelect.ReadValue<float>();
                lInput.ValueRW.LHandJoystick = input.XRLHand.Joystick.ReadValue<Vector2>()
                    .SetDeadZone(deadX, deadY);
                lInput.ValueRW.LHandPrimary = input.XRLHand.Primary.ReadValue<float>();
                lInput.ValueRW.LHandSecondary = input.XRLHand.Secondary.ReadValue<float>();
                lInput.ValueRW.Menu = input.XRLHand.Menu.ReadValue<float>();

                rInput.ValueRW.IsTracked = input.XRRHand.IsTracked.ReadValue<float>();
                rInput.ValueRW.TrackingState = input.XRRHand.TrackingState.ReadValue<int>();
                rInput.ValueRW.RHandPos = input.XRRHand.HandPosition.ReadValue<Vector3>();
                rInput.ValueRW.RHandRot = input.XRRHand.HandRotation.ReadValue<Quaternion>();
                rInput.ValueRW.RHandGrip = input.XRRHand.GripSelect.ReadValue<float>();
                rInput.ValueRW.RHandTrigger = input.XRRHand.TriggerSelect.ReadValue<float>();
                rInput.ValueRW.RHandJoystick = input.XRRHand.Joystick.ReadValue<Vector2>()
                    .SetDeadZone(deadX, deadY);
                rInput.ValueRW.RHandPrimary = input.XRRHand.Primary.ReadValue<float>();
                rInput.ValueRW.RHandSecondary = input.XRRHand.Secondary.ReadValue<float>();

                uInput.ValueRW.PointerPos = input.UI.PointerPosition.ReadValue<Vector3>();
                uInput.ValueRW.PointerRot = input.UI.PointerRotation.ReadValue<Quaternion>();
                uInput.ValueRW.Click = input.UI.Click.ReadValue<float>();
                uInput.ValueRW.Navigate = input.UI.Navigate.ReadValue<Vector2>();
                uInput.ValueRW.Submit = input.UI.Submit.ReadValue<float>();
                uInput.ValueRW.Cancel = input.UI.Cancel.ReadValue<float>();
                uInput.ValueRW.Point = input.UI.Point.ReadValue<Vector2>();
                uInput.ValueRW.DevicePos = input.UI.DevicePosition.ReadValue<Vector3>();
                uInput.ValueRW.DeviceRot = input.UI.DeviceRotation.ReadValue<Quaternion>();
                uInput.ValueRW.ScrollWheel = input.UI.ScrollWheel.ReadValue<Vector2>();
                uInput.ValueRW.MiddleClick = input.UI.MiddleClick.ReadValue<float>();
                uInput.ValueRW.RightClick = input.UI.RightClick.ReadValue<float>();
            }
        }
    }

    public static partial class HelperCollection {
        internal static float2 SetDeadZone(this Vector2 input, float deadZoneX, float deadZoneY) {
            if (math.abs(input.x) < deadZoneX) input.x = 0;
            if (math.abs(input.y) < deadZoneY) input.y = 0;
            return input;
        }
    }
}