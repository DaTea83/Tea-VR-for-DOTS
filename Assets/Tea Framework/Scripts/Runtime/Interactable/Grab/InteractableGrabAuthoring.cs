using System;
using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Tags/Grab Interact Tag")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class InteractableGrabAuthoring : MonoBehaviour
    {
        [SerializeField][Range(0f, 30f)] private float smoothFollowSpeed;
        [SerializeField] private ETranslationType objectType;

        private Rigidbody _rb;

        private void OnValidate()
        {
            _rb = GetComponent<Rigidbody>();
            objectType = _rb.isKinematic ? ETranslationType.LocalTransform : ETranslationType.Physics;
        }

        internal class Baker : Baker<InteractableGrabAuthoring>
        {
            public override void Bake(InteractableGrabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                
                switch (authoring.objectType)
                {
                    case ETranslationType.LocalTransform:
                        AddComponent<InteractableNonPhysicsITag>(entity);
                        break;
                    case ETranslationType.Physics:
                        AddComponent<InitializePhysicsMassDataITag>(entity);
                        break;
                }
                
                AddComponent(entity, new InteractableGrabIData
                {
                    SmoothFollowSpeed = authoring.smoothFollowSpeed
                });
                AddBuffer<InteractGrabIBuffer>(entity);
                this.AddSetIEnableableComponent<InteractGrabFollowIEnableableTag>(entity, false);
            }
        }
    }
}
