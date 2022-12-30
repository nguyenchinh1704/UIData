using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonConstraint)), CanEditMultipleObjects]
    public class FlexalonConstraintEditor : FlexalonComponentEditor
    {
        public override void OnInspectorGUI()
        {
            ForceUpdateButton();
            SerializedObject so = serializedObject;
            EditorGUILayout.PropertyField(so.FindProperty("_target"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_horizontalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_depthAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_horizontalPivot"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalPivot"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_depthPivot"), true);
            ApplyModifiedProperties();
        }
    }
}