using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonCircleLayout)), CanEditMultipleObjects]
    public class FlexalonCircleLayoutEditor : FlexalonComponentEditor
    {
        [MenuItem("GameObject/Flexalon/Circle Layout")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonCircleLayout>("Circle Layout");
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();

            SerializedObject so = serializedObject;
            if (!(target as FlexalonCircleLayout).UseWidth)
            {
                EditorGUILayout.PropertyField(so.FindProperty("_radius"), true);
            }

            EditorGUILayout.PropertyField(so.FindProperty("_useWidth"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_spiral"), true);

            if ((target as FlexalonCircleLayout).Spiral)
            {
                EditorGUILayout.PropertyField(so.FindProperty("_spiralSpacing"), true);
            }

            EditorGUILayout.PropertyField(so.FindProperty("_spacingType"), true);

            if ((target as FlexalonCircleLayout).SpacingType == FlexalonCircleLayout.SpacingOptions.Fixed)
            {
                EditorGUILayout.PropertyField(so.FindProperty("_spacingDegrees"), true);
            }

            EditorGUILayout.PropertyField(so.FindProperty("_startAtDegrees"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_rotate"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_verticalAlign"), true);

            ApplyModifiedProperties();
        }
    }
}