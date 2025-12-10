using Unity.Entities;

namespace TeaFramework
{
    public static partial class BakerCollection
    {
        public static void AddSetIEnableableComponent<T>(this IBaker baker, Entity entity, bool initializeState)
            where T : unmanaged, IComponentData, IEnableableComponent
        {
            baker.AddComponent<T>(entity);
            baker.SetComponentEnabled<T>(entity, initializeState);
        }
    }
}
