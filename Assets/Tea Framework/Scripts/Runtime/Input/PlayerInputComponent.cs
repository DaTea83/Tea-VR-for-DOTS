using Unity.Entities;
using Unity.Mathematics;

namespace TeaFramework {
    public struct HeadInputIData : IComponentData {
        public float IsTracked;
        public int TrackingState;
        public float3 HeadPos;
        public float3 HeadRot;
    }

    public struct LHandInputIData : IComponentData {
        public float IsTracked;
        public int TrackingState;
        public float3 LHandPos;
        public quaternion LHandRot;
        public float LHandGrip;
        public float LHandTrigger;
        public float2 LHandJoystick;
        public float LHandPrimary;
        public float LHandSecondary;
        public float Menu;
    }

    public struct RHandInputIData : IComponentData {
        public float IsTracked;
        public int TrackingState;
        public float3 RHandPos;
        public quaternion RHandRot;
        public float RHandGrip;
        public float RHandTrigger;
        public float2 RHandJoystick;
        public float RHandPrimary;
        public float RHandSecondary;
    }

    public struct UIInputIData : IComponentData {
        public float2 Navigate;
        public float Click;
        public float Submit;
        public float Cancel;
        public float2 Point;
        public float3 PointerPos;
        public quaternion PointerRot;
        public float3 DevicePos;
        public quaternion DeviceRot;
        public float2 ScrollWheel;
        public float MiddleClick;
        public float RightClick;
    }
}