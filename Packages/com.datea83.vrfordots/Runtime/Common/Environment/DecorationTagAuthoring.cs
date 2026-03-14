using Unity.Entities;
using UnityEngine;

namespace VRForDOTS.Common.Environment {
    [AddComponentMenu("VRForDOTS/Tags/Decoration Tag")]
    [DisallowMultipleComponent]
    public class DecorationTagAuthoring : MonoBehaviour {
        private class DecorationTagAuthoringBaker : Baker<DecorationTagAuthoring> {
            public override void Bake(DecorationTagAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<DecorationITag>(e);
            }
        }
    }

    public struct DecorationITag : IComponentData { }
}