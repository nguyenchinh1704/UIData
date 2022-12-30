using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonGridLayout)), CanEditMultipleObjects]
    public class FlexalonGridLayoutEditor : FlexalonComponentEditor
    {
        [MenuItem("GameObject/Flexalon/Grid Layout")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonGridLayout>("Grid Layout");
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();
            SerializedObject so = serializedObject;
            EditorGUILayout.PropertyField(so.FindProperty("_cellType"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_columns"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_rows"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_columnDirection"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_rowDirection"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_columnSpacing"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_rowSpacing"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_horizontalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_depthAlign"), true);
            ApplyModifiedProperties();
        }
    }
}