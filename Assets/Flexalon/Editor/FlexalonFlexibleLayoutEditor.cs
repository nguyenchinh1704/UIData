using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonFlexibleLayout)), CanEditMultipleObjects]
    public class FlexalonFlexibleLayoutEditor : FlexalonComponentEditor
    {
        [MenuItem("GameObject/Flexalon/Flexible Layout")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonFlexibleLayout>("Flexible Layout");
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();
            SerializedObject so = serializedObject;
            EditorGUILayout.PropertyField(so.FindProperty("_direction"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_wrap"), true);
            if ((target as FlexalonFlexibleLayout).Wrap)
            {
                EditorGUILayout.PropertyField(so.FindProperty("_wrapDirection"), true);
            }

            EditorGUILayout.PropertyField(so.FindProperty("_horizontalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_depthAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_horizontalInnerAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalInnerAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_depthInnerAlign"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_gap"), true);
            if ((target as FlexalonFlexibleLayout).Wrap)
            {
                EditorGUILayout.PropertyField(so.FindProperty("_wrapGap"), true);
            }
            ApplyModifiedProperties();
        }
    }
}