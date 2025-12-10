using Unity.Entities;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Player/XR Device Tracker")]
    [DisallowMultipleComponent]
    public class XRDeviceTrackerAuthoring : MonoBehaviour
    {
        public EDeviceType deviceType;

        private class XRDeviceTrackerAuthoringBaker : Baker<XRDeviceTrackerAuthoring>
        {
            public override void Bake(XRDeviceTrackerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new XRDeviceTrackerIData
                {
                    DeviceType = authoring.deviceType,
                });
            }
        }
    }

    public struct XRDeviceTrackerIData : IComponentData
    {
        public EDeviceType DeviceType;
    }
}
