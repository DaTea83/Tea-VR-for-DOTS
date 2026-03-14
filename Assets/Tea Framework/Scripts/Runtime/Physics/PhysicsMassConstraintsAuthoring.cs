using Unity.Entities;
using UnityEngine;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Physics/Rigid Body Constraints")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PhysicsMassInitialDataAuthoring))]
    public class PhysicsMassConstraintsAuthoring : MonoBehaviour {
        public bool lockX;
        public bool lockY;
        public bool lockZ;

        private class PlayerPhysicsBaker : Baker<PhysicsMassConstraintsAuthoring> {
            public override void Bake(PhysicsMassConstraintsAuthoring massConstraintsAuthoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new InitializeLockRotationIData {
                    X = massConstraintsAuthoring.lockX,
                    Y = massConstraintsAuthoring.lockY,
                    Z = massConstraintsAuthoring.lockZ,
                });
            }
        }
    }
}