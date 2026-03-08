using UnityEditor;
using VRForDOTS.Common.Environment;
// ReSharper disable CheckNamespace

namespace VRFordots.Editor
{
    [CustomEditor(typeof(EnvironmentTagAuthoring))]
    [CanEditMultipleObjects]
    public class EnvironmentTagEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var tag = (EnvironmentTagAuthoring)target;
            EditorGUILayout.HelpBox("This component is used to label any entities that is a part of environment", MessageType.Info);
            EditorGUILayout.HelpBox("Non moveable in runtime, basically a static object", MessageType.Info);
        }
    }
}