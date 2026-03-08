using UnityEditor;
using VRForDOTS.Common.Environment;
// ReSharper disable CheckNamespace

namespace VRFordots.Editor
{
    [CustomEditor(typeof(DecorationTagAuthoring))]
	[CanEditMultipleObjects]
    public class DecorationTagEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var tag = (DecorationTagAuthoring)target;
            EditorGUILayout.HelpBox("This component is used to label any entities that is a part of decoration", MessageType.Info);
            EditorGUILayout.HelpBox("Subject to external influence, such as physics", MessageType.Info);
        }
    }
}