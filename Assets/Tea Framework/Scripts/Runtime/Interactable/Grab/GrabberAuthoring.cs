using System;
using Unity.Entities;
using UnityEngine;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Tags/Grabber Tag")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    public sealed class GrabberAuthoring : MonoBehaviour {
        [SerializeField] private PlayerTagAuthoring player;
        [SerializeField] private EButtonType grabButton;
        [Range(0f, 1f)] [SerializeField] private float grabThreshold = 0.5f;
        [SerializeField] private EButtonInteract interactType = EButtonInteract.Hold;

        private SphereCollider _collider;

        private void OnValidate() {
            _collider = GetComponent<SphereCollider>();
            if (!_collider.isTrigger) _collider.isTrigger = true;
        }

        internal class Baker : Baker<GrabberAuthoring> {
            public override void Bake(GrabberAuthoring authoring) {
                DependsOn(authoring.player);
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var p = GetEntity(authoring.player, TransformUsageFlags.Dynamic);

                AddBuffer<GrabberEntitiesIBuffer>(entity);
                AddComponent<GrabberActiveIData>(entity);

                AddComponent(entity, new GrabberTriggerIData {
                    GrabButton = authoring.grabButton,
                    InteractType = authoring.interactType,
                    Threshold = authoring.grabThreshold,
                    Player = p
                });
            }
        }
    }
}