using Unity.Entities;
using UnityEngine;

namespace TeaFramework {
    [DisallowMultipleComponent]
    public sealed class PlayerTagAuthoring : MonoBehaviour {
        public class Baker : Baker<PlayerTagAuthoring> {
            public override void Bake(PlayerTagAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerITag>(e);
            }
        }
    }

    [UpdateInGroup(typeof(Tea_InitializationSystemGroup))]
    public partial struct InitializedPlayerInputISystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (_, entity) in SystemAPI.Query<RefRO<PlayerITag>>()
                         .WithNone<HeadInputIData, LHandInputIData, RHandInputIData>().WithEntityAccess()) {
                ecb.AddComponent(entity, new HeadInputIData());
                ecb.AddComponent(entity, new LHandInputIData());
                ecb.AddComponent(entity, new RHandInputIData());
                ecb.AddComponent(entity, new UIInputIData());
            }

            ecb.Playback(state.EntityManager);
        }
    }
}