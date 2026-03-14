using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Scenes;

// Execution order from top to bottom
namespace TeaFramework {
    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
    public partial class Tea_InitializationSystemGroup : ComponentSystemGroup { }

    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    public partial class Tea_PhysicsSystemGroup : ComponentSystemGroup { }

    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class Tea_PreTransformSystemGroup : ComponentSystemGroup { }

    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class Tea_PostTransformSystemGroup : ComponentSystemGroup { }

    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(Tea_DestroySystemGroup))]
    public partial class Tea_EffectSystemGroup : ComponentSystemGroup { }

    /// <summary>
    /// 
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class Tea_DestroySystemGroup : ComponentSystemGroup { }
}