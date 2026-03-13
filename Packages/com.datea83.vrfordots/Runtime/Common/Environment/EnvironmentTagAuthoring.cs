using Unity.Entities;
using UnityEngine;

namespace VRForDOTS.Common.Environment
{
    [AddComponentMenu("VRForDOTS/Tags/Environment Tag")]
    [DisallowMultipleComponent]
    public class EnvironmentTagAuthoring : MonoBehaviour
    {
        private class EnvironmentTagAuthoringBaker : Baker<EnvironmentTagAuthoring>
        {
            public override void Bake(EnvironmentTagAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent<EnvironmentITag>(e);
            }
        }
    }
    
    public struct EnvironmentITag : IComponentData { }
}