using Unity.Entities;
using Unity.Mathematics;

namespace TeaFramework {
    //For Interactable

    public struct InteractableGrabIData : IComponentData {
        public float SmoothFollowSpeed;
    }

    public struct InteractableNonPhysicsITag : IComponentData { }

    public struct InteractGrabFollowIEnableableTag : IComponentData, IEnableableComponent { }

    /// <summary>
    /// 2 just simply we have two hands
    /// In any scenario where more is required just increase the number
    /// </summary>
    [InternalBufferCapacity(2)]
    public struct InteractGrabIBuffer : IBufferElementData {
        public Entity GrabberEntity;
        public float3 Offset;
    }

    //For Grabber

    [InternalBufferCapacity(8)]
    public struct GrabberEntitiesIBuffer : IBufferElementData {
        public Entity Interactable;
        public float DisSqr;
        public float Dot;
    }

    public struct GrabberTriggerIData : IComponentData {
        public Entity Player;
        public EButtonType GrabButton;
        public EButtonInteract InteractType;
        public float Threshold;
    }

    public struct GrabberActiveIData : IComponentData {
        public Entity InteractEntity;
        public float CurrentInput;
        public float PreviousInput;
    }
}