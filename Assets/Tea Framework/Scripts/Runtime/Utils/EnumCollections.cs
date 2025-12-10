// ReSharper disable InconsistentNaming

namespace TeaFramework
{
    public enum EJoystickValue : byte 
    {
        LeftJoystickUpDown = 1 << 0,
        LeftJoystickLeftRight = 1 << 1,
        RightJoystickUpDown = 1 << 2,
        RightJoystickLeftRight = 1 << 3,
    }
    
    public enum EDeviceType : byte
    {
        HMount = 1 << 0,
        LController = 1 << 1,
        RController = 1 << 2
    }

    public enum EMovementState : byte
    {
        SmoothWalking = 0,
        Teleporting = 1 << 1,
        None = byte.MaxValue,
    }

    public enum EMotionTransition : byte
    {
        Snap = 1 << 1,
        Smooth = 1 << 2,
        None = byte.MaxValue, 
    }

    public enum EButtonInteract : byte
    {
        Hold = 0,
        Toggle = 1 << 0,
    }

    public enum EGrabType : byte
    {
        Snap = 0,
        Accurate = 1 << 0,
        Hover =  1 << 1,
        Inherit = 1 << 2,
    }

    public enum ETranslationType : byte
    {
        LocalTransform = 0,
        Physics = 1 << 0,
    }

    public enum EEventFireType : byte
    {
        Discrete = 0,
        Continuous = 1 << 0,
    }
    
    public enum EButtonType : byte
    {
        LeftHandGrip,
        LeftHandTrigger,
        LeftHandPrimary,
        LeftHandSecondary,
        Menu,
        RightHandGrip,
        RightHandTrigger,
        RightHandPrimary,
        RightHandSecondary,
    }
    
    public enum EFadeType : byte
    {
        FadeIn,
        FadeOut,
    }
}
