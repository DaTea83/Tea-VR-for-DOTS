using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

namespace TeaFramework
{
    /// <summary>
    /// Add this if you find the parent component wasn't automatically added in children entity
    /// </summary>
    [AddComponentMenu("Tea Framework/Translation/Parent Component")]
    [DisallowMultipleComponent]
    public class FollowParentAuthoring : MonoBehaviour
    {
        public GameObject followTarget;

        private void OnValidate()
        {
            followTarget = transform.parent.gameObject;
        }

        public class FollowParentAuthoringBaker : Baker<FollowParentAuthoring>
        {
            public override void Bake(FollowParentAuthoring authoring)
            {
                if (authoring.followTarget is null) return;
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var parent = GetEntity(authoring.followTarget ,TransformUsageFlags.Dynamic);
                AddComponent(entity, new Parent
                {
                    Value = parent,
                });
            }
        }
    }
    
#if UNITY_EDITOR

    [CustomEditor(typeof(FollowParentAuthoring))]
    public class FollowParentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var authoring = (FollowParentAuthoring)target;
            EditorGUILayout.HelpBox("When you add rigidbody to an entity, usually it will automatically remove the parent component", MessageType.Info);
            EditorGUILayout.HelpBox("This component is to add back if you still want the parent component", MessageType.Info);
            
            authoring.followTarget  = (GameObject)EditorGUILayout.ObjectField("Follow Target", authoring.followTarget, typeof(GameObject), true);
        }
    }
    
#endif    
}
