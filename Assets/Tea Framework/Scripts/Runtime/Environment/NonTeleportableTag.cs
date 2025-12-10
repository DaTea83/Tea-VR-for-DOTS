using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Tags/No Teleport Tag")]
    public class NonTeleportableTag : MonoBehaviour
    {
        private class NonTeleportTagBaker : Baker<NonTeleportableTag>
        {
            public override void Bake(NonTeleportableTag authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<NonTeleportableITag>(entity);
            }
        }
    }
    
    public struct NonTeleportableITag : IComponentData { }
}
