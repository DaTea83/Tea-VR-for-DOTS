using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TeaFramework {
    [AddComponentMenu("Tea Framework/Camera Controller")]
    [DisallowMultipleComponent]
    public class CameraController : GenericSingleton<CameraController> {
        [SerializeField] private Image blackScreenImg;
        [SerializeField] private float initialFadeOutTime = 5f;

        private async void OnEnable() {
            try {
                await Awaitable.WaitForSecondsAsync(3f);
                await RunFadeScreen(EFadeType.FadeOut, initialFadeOutTime);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public async Task RunFadeScreen(EFadeType fadeType, float duration) {
            try {
                await Awaitable.EndOfFrameAsync();
                await blackScreenImg.FadeScreenAsync(fadeType, duration, Time.deltaTime);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(CameraController))]
    public class CameraTrackerEditor : Editor {
        public override void OnInspectorGUI() {
            var instance = (CameraController)target;
            EditorGUILayout.HelpBox(
                "Currently in ECS you can't just attach the camera to a subscene entity and called it a day",
                MessageType.Info);
            EditorGUILayout.HelpBox("That's where this singleton comes into play", MessageType.Info);
            EditorGUILayout.HelpBox("Attach this to the camera object", MessageType.Info);
            EditorGUILayout.HelpBox("Don't put the camera in the subscene, put it in normal hierarchy",
                MessageType.Warning);
            EditorGUILayout.HelpBox(
                "Keep Singleton true or not doesn't matter, if true it will just override the other scene's camera",
                MessageType.Info);

            base.OnInspectorGUI();
        }
    }

#endif
}