using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace TeaFramework
{
    [AddComponentMenu("Tea Framework/Tags/Decoration Tag")]
    public class DecorationTagAuthoring : MonoBehaviour
    {
        private class DecorationTagBaker : Baker<DecorationTagAuthoring>
        {
            public override void Bake(DecorationTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<DecorationITag>(entity);
            }
        }
    }
    
    public struct DecorationITag : IComponentData { }
    
#if UNITY_EDITOR

    [CustomEditor(typeof(DecorationTagAuthoring))]
    [CanEditMultipleObjects]
    public class DecorationTagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var tag = (DecorationTagAuthoring)target;
            EditorGUILayout.HelpBox("This component is used to label any entities that is a part of decoration", MessageType.Info);
            EditorGUILayout.HelpBox("Subject to external influence, such as physics", MessageType.Info);
        }
    }

#endif
}
