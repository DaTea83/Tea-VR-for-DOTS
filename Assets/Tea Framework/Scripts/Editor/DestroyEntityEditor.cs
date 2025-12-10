using UnityEditor;
using UnityEngine;

namespace TeaFramework
{
#if UNITY_EDITOR    
    
    [CustomEditor(typeof(DestroyEntityAuthoring))]
    public class DestroyEntityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Component will be disabled at start, after activation will instant despawn the entity", MessageType.Info);
        }
    }
    
#endif    
}
