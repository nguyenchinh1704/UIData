using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonRandomLayout)), CanEditMultipleObjects]
    public class FlexalonRandomLayoutEditor : FlexalonComponentEditor
    {
        [MenuItem("GameObject/Flexalon/Random Layout")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonRandomLayout>("Random Layout");
        }

        private bool _showPosition = true;
        private bool _showRotation = true;
        private bool _showSize = true;

        private void CreateItem(string label, string enableName, string minName, string maxName, string alignName = null)
        {
            var enableProp = serializedObject.FindProperty(enableName);
            EditorGUILayout.BeginHorizontal();
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30.0f;
            EditorGUILayout.PropertyField(enableProp, new GUIContent(label), new GUILayoutOption[] { GUILayout.Width(40.0f) });
            if (enableProp.boolValue)
            {
                EditorGUIUtility.labelWidth = 50.0f;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(minName), new GUIContent("Min"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(maxName), new GUIContent("Max"));
            }
            else if (alignName != null)
            {
                EditorGUIUtility.labelWidth = 50.0f;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(alignName), new GUIContent("Align"));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = labelWidth;
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_randomSeed"), true);
            _showPosition = EditorGUILayout.Foldout(_showPosition, "Position");
            if (_showPosition)
            {
                EditorGUI.indentLevel++;
                CreateItem("X", "_randomizePositionX", "_positionMinX", "_positionMaxX", "_horizontalAlign");
                CreateItem("Y", "_randomizePositionY", "_positionMinY", "_positionMaxY", "_verticalAlign");
                CreateItem("Z", "_randomizePositionZ", "_positionMinZ", "_positionMaxZ", "_depthAlign");
                EditorGUI.indentLevel--;
            }

            _showRotation = EditorGUILayout.Foldout(_showRotation, "Rotation");
            if (_showRotation)
            {
                EditorGUI.indentLevel++;
                CreateItem("X", "_randomizeRotationX", "_rotationMinX", "_rotationMaxX");
                CreateItem("Y", "_randomizeRotationY", "_rotationMinY", "_rotationMaxY");
                CreateItem("Z", "_randomizeRotationZ", "_rotationMinZ", "_rotationMaxZ");
                EditorGUI.indentLevel--;
            }

            _showSize = EditorGUILayout.Foldout(_showSize, "Size");
            if (_showSize)
            {
                EditorGUI.indentLevel++;
                CreateItem("X", "_randomizeSizeX", "_sizeMinX", "_sizeMaxX");
                CreateItem("Y", "_randomizeSizeY", "_sizeMinY", "_sizeMaxY");
                CreateItem("Z", "_randomizeSizeZ", "_sizeMinZ", "_sizeMaxZ");
                EditorGUI.indentLevel--;
            }

            ApplyModifiedProperties();
        }
    }
}