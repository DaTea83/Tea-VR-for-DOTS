using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Physics/Rigid Body Data")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsMassInitialDataAuthoring : MonoBehaviour
    {
        private class PhysicsInitialDataBaker : Baker<PhysicsMassInitialDataAuthoring>
        {
            public override void Bake(PhysicsMassInitialDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<InitializePhysicsMassDataITag>(entity);
            }
        }
    }
}
