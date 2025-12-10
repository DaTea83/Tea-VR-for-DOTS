using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Tags/Environment Tag")]
    public class EnvironmentTagAuthoring : MonoBehaviour
    {
        private class EnvironmentTagBaker : Baker<EnvironmentTagAuthoring>
        {
            public override void Bake(EnvironmentTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<EnvironmentITag>(entity);
            }
        }
    }
    
    public struct EnvironmentITag : IComponentData { }
    
#if UNITY_EDITOR
    
    [CustomEditor(typeof(EnvironmentTagAuthoring))]
    [CanEditMultipleObjects]
    public class EnvironmentTagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var tag = (EnvironmentTagAuthoring)target;
            EditorGUILayout.HelpBox("This component is used to label any entities that is a part of environment", MessageType.Info);
            EditorGUILayout.HelpBox("Non moveable in runtime, basically a static object", MessageType.Info);
        }
    }
    
#endif    
}
