using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace TeaFramework
{
    public struct PlayerITag : IComponentData { }
    
    public struct PlayerMovementIData : IComponentData
    {
        public EDeviceType JoystickInput;
        public ETranslationType TranslationType;
        public EMovementState MovementType;
        public Entity Player;
        public Entity BodyEntity;
        public float2 CurrentInput;
        public float2 PreviousInput;
        public float InputThreshold;
    }

    [Serializable]
    public struct PlayerMovementSerialize
    {
        public EDeviceType joystickInput;
        public ETranslationType translationType;
        public EMovementState movementType;
        public PlayerTagAuthoring player;
        public GameObject bodyObj;
        [Range(0.01f, 0.9f)] public float inputThreshold;
    }

    public struct PlayerRotationIData : IComponentData
    {
        public EJoystickValue JoystickInput;
        public EMotionTransition MotionType;
        public float CurrentInput;
        public float PreviousInput;
        public float InputThreshold;
    }

    [Serializable]
    public struct PlayerRotationSerialize
    {
        public EJoystickValue joystickInput;
        public EMotionTransition motionType;
        [Range(0.01f, 0.9f)] public float inputThreshold;
    }

    public struct PlayerCrouchIData : IComponentData
    {
        public EJoystickValue JoystickInput;
        public EMotionTransition MotionType;
        public float CurrentInput;
        public float PreviousInput;
        public float InputThreshold;
        public float DefaultHeight;
    }

    [Serializable]
    public struct PlayerCrouchSerialize
    {
        public EJoystickValue joystickInput;
        public EMotionTransition motionType;
        [Range(0.01f, 0.9f)] public float inputThreshold;
        public float defaultHeight;
    }
}
