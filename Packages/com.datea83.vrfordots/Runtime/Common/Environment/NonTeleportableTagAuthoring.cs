using Unity.Entities;
using UnityEngine;

namespace VRForDOTS.Common.Environment {
    [AddComponentMenu("VRForDOTS/Tags/Non-Teleport Tag")]
    [DisallowMultipleComponent]
    public class NonTeleportableTagAuthoring : MonoBehaviour {
        private class NonTeleportableTagAuthoringBaker : Baker<NonTeleportableTagAuthoring> {
            public override void Bake(NonTeleportableTagAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent<NonTeleportableITag>(e);
            }
        }
    }

    public struct NonTeleportableITag : IComponentData { }
}