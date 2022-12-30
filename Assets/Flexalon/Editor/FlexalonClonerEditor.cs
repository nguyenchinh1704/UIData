using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonCloner)), CanEditMultipleObjects]
    public class FlexalonClonerEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Flexalon/Cloner")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonCloner>("Cloner");
        }


        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_objects"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_cloneType"), true);

            if ((target as FlexalonCloner).DataSource == null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_count"), true);
            }

            if ((target as FlexalonCloner).CloneType == FlexalonCloner.CloneTypes.Random)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_randomSeed"), true);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_dataSource"), true);

            if (serializedObject.ApplyModifiedProperties())
            {
                if (Application.isPlaying)
                {
                    foreach (var target in targets)
                    {
                        (target as FlexalonCloner).MarkDirty();
                    }
                }
            }
        }
    }
}