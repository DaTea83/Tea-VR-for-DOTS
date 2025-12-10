using Unity.Entities;
using Unity.Mathematics;

namespace TeaFramework
{
    public static partial class HelperCollection
    {
        internal static void GetInputType(EJoystickValue joystick, RefRO<LHandInputIData> lInput, RefRO<RHandInputIData> rInput, out float value)
        {
            value = joystick switch
            {
                EJoystickValue.LeftJoystickUpDown => lInput.ValueRO.LHandJoystick.y,
                EJoystickValue.LeftJoystickLeftRight => lInput.ValueRO.LHandJoystick.x,
                EJoystickValue.RightJoystickUpDown => rInput.ValueRO.RHandJoystick.y,
                EJoystickValue.RightJoystickLeftRight => rInput.ValueRO.RHandJoystick.x,
                _ => 0f
            };
        }
        
        internal static float GetInputType(this EJoystickValue joystick, RefRO<LHandInputIData> lInput, RefRO<RHandInputIData> rInput)
        {
            return joystick switch
            {
                EJoystickValue.LeftJoystickUpDown => lInput.ValueRO.LHandJoystick.y,
                EJoystickValue.LeftJoystickLeftRight => lInput.ValueRO.LHandJoystick.x,
                EJoystickValue.RightJoystickUpDown => rInput.ValueRO.RHandJoystick.y,
                EJoystickValue.RightJoystickLeftRight => rInput.ValueRO.RHandJoystick.x,
                _ => 0f
            };
        }
        
        internal static void GetInputType(EDeviceType inputType, RefRO<LHandInputIData> lInput, 
            RefRO<RHandInputIData> rInput, RefRO<HeadInputIData> hInput, out float2 value)
        {
            value = inputType switch
            {
                EDeviceType.HMount => new float2(hInput.ValueRO.HeadPos.x, hInput.ValueRO.HeadPos.z),
                EDeviceType.LController => lInput.ValueRO.LHandJoystick,
                EDeviceType.RController => rInput.ValueRO.RHandJoystick,
                _ => float2.zero
            };
        }
        
        internal static float2 GetInputType(this EDeviceType inputType, RefRO<LHandInputIData> lInput, 
            RefRO<RHandInputIData> rInput, RefRO<HeadInputIData> hInput)
        {
            return inputType switch
            {
                EDeviceType.HMount => new float2(hInput.ValueRO.HeadPos.x, hInput.ValueRO.HeadPos.z),
                EDeviceType.LController => lInput.ValueRO.LHandJoystick,
                EDeviceType.RController => rInput.ValueRO.RHandJoystick,
                _ => float2.zero
            };
            
        }

        public static float GetButtonValues(this EButtonType buttonType, RefRO<LHandInputIData> lInput, RefRO<RHandInputIData> rInput)
        {
            var value = buttonType switch
            {
                EButtonType.LeftHandGrip => lInput.ValueRO.LHandGrip,
                EButtonType.LeftHandTrigger => lInput.ValueRO.LHandTrigger,
                EButtonType.LeftHandPrimary => lInput.ValueRO.LHandPrimary,
                EButtonType.LeftHandSecondary => lInput.ValueRO.LHandSecondary,
                EButtonType.Menu => lInput.ValueRO.Menu,
                EButtonType.RightHandGrip => rInput.ValueRO.RHandGrip,
                EButtonType.RightHandTrigger => rInput.ValueRO.RHandTrigger,
                EButtonType.RightHandPrimary => rInput.ValueRO.RHandPrimary,
                EButtonType.RightHandSecondary => rInput.ValueRO.RHandSecondary,
                _ => 0f
            };
            
            return value;
        }
    }
}
