using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Player/XR Origin Controller")]
    [DisallowMultipleComponent]
    public sealed class XROriginController : GenericSingleton<XROriginController> {
        [Header("Tracking Settings")] public TrackingOriginModeFlags trackingOrigin = TrackingOriginModeFlags.Floor;

        [Header("Joystick Settings")] [Range(0.005f, 0.1f)]
        public float deadZoneX = 0.01f;

        [Range(0.005f, 0.1f)] public float deadZoneY = 0.01f;

        private bool _isSetOrigin;
        private readonly List<XRInputSubsystem> _subsystems = new();

        public event Action OnNoneSubsystemFound;

        private void Update() {
            if (_isSetOrigin) return;
            if (trackingOrigin == TrackingOriginModeFlags.Unknown) return;
            _subsystems.Clear();
            SubsystemManager.GetSubsystems(_subsystems);

            if (_subsystems.Count == 0) {
#if UNITY_EDITOR
                Debug.LogWarning("No XR Input Subsystems found");
#endif
                OnNoneSubsystemFound?.Invoke();
                return;
            }

            bool isThisFrameHaveSubsystems = false;
            foreach (var sub in _subsystems) {
                var supportedModes = sub.GetSupportedTrackingOriginModes();
                bool supportsModes = (supportedModes & trackingOrigin) != 0;

                if (!supportsModes) continue;
                if (!sub.running) {
#if UNITY_EDITOR
                    Debug.LogWarning("No XR Input Subsystems running");
#endif
                    OnNoneSubsystemFound?.Invoke();
                    continue;
                }

                if (sub.TrySetTrackingOriginMode(trackingOrigin)) {
                    isThisFrameHaveSubsystems = true;
                    break;
                }
            }

            if (isThisFrameHaveSubsystems)
                _isSetOrigin = true;
        }
    }
}