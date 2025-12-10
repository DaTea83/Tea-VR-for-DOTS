using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [DisallowMultipleComponent]
    public class XRPlayerMovementAuthoring : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private PlayerMovementSerialize movement;
        [Header("Rotation")] [SerializeField] private PlayerRotationSerialize rotation;
        [Header("Crouch")] [SerializeField] private PlayerCrouchSerialize crouch;
        
        internal class Baker : Baker<XRPlayerMovementAuthoring>
        {
            public override void Bake(XRPlayerMovementAuthoring authoring)
            {
                DependsOn(authoring.movement.player);
                DependsOn(authoring.movement.bodyObj);
                
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var body = GetEntity(authoring.movement.bodyObj, TransformUsageFlags.Dynamic);
                var p = GetEntity(authoring.movement.player, TransformUsageFlags.Dynamic);
                
                AddComponent(e, new PlayerMovementIData
                {
                    JoystickInput = authoring.movement.joystickInput,
                    TranslationType = authoring.movement.translationType,
                    MovementType = authoring.movement.movementType,
                    InputThreshold = authoring.movement.inputThreshold,
                    BodyEntity = body,
                    Player = p
                });
                AddComponent(e, new PlayerRotationIData
                {
                    JoystickInput = authoring.rotation.joystickInput,
                    MotionType = authoring.rotation.motionType,
                    InputThreshold = authoring.rotation.inputThreshold
                });
                AddComponent(e,  new PlayerCrouchIData
                {
                    JoystickInput = authoring.crouch.joystickInput,
                    MotionType = authoring.crouch.motionType,
                    InputThreshold = authoring.crouch.inputThreshold,
                    DefaultHeight = authoring.crouch.defaultHeight
                });
            }
        }
    }
}
