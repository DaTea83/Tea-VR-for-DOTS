using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.XR;

namespace TeaFramework
{
    [UpdateInGroup(typeof(Tea_PreTransformSystemGroup), OrderFirst = true)]
    public partial class XRDeviceTrackerSystemBase : SystemBase
    {
        private static readonly List<InputDevice> Devices = new();

        protected override void OnUpdate()
        {
            InputDevices.GetDevices(Devices);
            
            foreach (var (deviceTrack, lt) 
                     in SystemAPI.Query<RefRO<XRDeviceTrackerIData>, RefRW<LocalTransform>>())
            {
                var device = deviceTrack.ValueRO.DeviceType switch
                {
                    EDeviceType.HMount => GetHeadDevice(),
                    EDeviceType.LController => GetController(InputDeviceCharacteristics.Left),
                    EDeviceType.RController => GetController(InputDeviceCharacteristics.Right),
                    _ => default
                };

                if (!device.isValid) continue;
                
                lt.ValueRW.Position = GetDevicePosition(device);
                lt.ValueRW.Rotation = GetDeviceRotation(device);
            }
        }
        
        private InputDevice GetHeadDevice()
        {
            InputDevices.GetDevices(Devices);
            
            var deviceList = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted, deviceList);
            
            return deviceList.FirstOrDefault();
        }
        
        private InputDevice GetController(InputDeviceCharacteristics side)
        {
            InputDevices.GetDevices(Devices);
            
            var deviceList = new List<InputDevice>();
            var controller = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand | side;
            InputDevices.GetDevicesWithCharacteristics(controller, deviceList);
            
            if(deviceList.Count > 0)
                return deviceList.First();

            var hand = InputDeviceCharacteristics.HandTracking | side;
            InputDevices.GetDevicesWithCharacteristics(hand, deviceList);
            return deviceList.FirstOrDefault();
        }
        
        private float3 GetDevicePosition(InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.devicePosition, out var value);
            return value;
        }
        
        private quaternion GetDeviceRotation(InputDevice device)
        {
            device.TryGetFeatureValue(CommonUsages.deviceRotation, out var value);
            return value;
        }
    }
}
