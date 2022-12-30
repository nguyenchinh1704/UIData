using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonObject)), CanEditMultipleObjects]
    public class FlexalonObjectEditor : FlexalonComponentEditor
    {
        private bool _marginToggle;
        private bool _paddingToggle;

        [MenuItem("GameObject/Flexalon/Empty Object")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonObject>("Empty Object");
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();

            CreateSizeProperty("_width", "Width");
            CreateSizeProperty("_height", "Height");
            CreateSizeProperty("_depth", "Depth");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_offset"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rotation"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_scale"), true);

            _marginToggle = EditorGUILayout.Foldout(_marginToggle, "Margins");
            if (_marginToggle)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginLeft"), new GUIContent("Left"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginRight"), new GUIContent("Right"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginTop"), new GUIContent("Top"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginBottom"), new GUIContent("Bottom"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginFront"), new GUIContent("Front"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_marginBack"), new GUIContent("Back"), true);
            }

            _paddingToggle = EditorGUILayout.Foldout(_paddingToggle, "Paddings");
            if (_paddingToggle)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingLeft"), new GUIContent("Left"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingRight"), new GUIContent("Right"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingTop"), new GUIContent("Top"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingBottom"), new GUIContent("Bottom"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingFront"), new GUIContent("Front"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_paddingBack"), new GUIContent("Back"), true);
            }

            ApplyModifiedProperties();
        }

        private void CreateSizeProperty(string name, string label)
        {
            var typeProperty = serializedObject.FindProperty(name + "Type");
            EditorGUILayout.BeginHorizontal();
            bool showLabel = true;
            var labelContent = new GUIContent(label);
            if (typeProperty.enumValueIndex == (int)SizeType.Fixed)
            {
                showLabel = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name), labelContent, true);
            }
            else if (typeProperty.enumValueIndex == (int)SizeType.Fill)
            {
                showLabel = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(name + "OfParent"), labelContent, true);
            }

            EditorGUILayout.PropertyField(typeProperty, showLabel ? labelContent : GUIContent.none, true);
            EditorGUILayout.EndHorizontal();
        }

        void OnSceneGUI()
        {
            // Draw a box at the transforms position
            var script = target as FlexalonObject;
            var node = Flexalon.GetNode(script.gameObject);
            if (node == null || node.Result == null)
            {
                return;
            }

            var r = node.Result;

            if (node.Parent != null)
            {
                // Box used to layout this object, plus margins.
                Handles.color = new Color(1f, 1f, .2f, 1.0f);
                var layoutBoxScale = node.GetWorldBoxScale(false);
                var layoutRotation = script.transform.parent != null ? script.transform.parent.rotation * r.LayoutRotation : r.LayoutRotation;
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center + node.Margin.Center, r.RotatedAndScaledBounds.size + node.Margin.Size);

                // Box used to layout this object.
                Handles.color = new Color(.2f, 1f, .5f, 1.0f);
                Handles.matrix = Matrix4x4.TRS(script.transform.position, layoutRotation, layoutBoxScale);
                Handles.DrawWireCube(r.RotatedAndScaledBounds.center, r.RotatedAndScaledBounds.size);
            }

            // Box in which children are layed out. This is the box with handles on it.
            Handles.color = Color.cyan;
            var worldBoxScale = node.GetWorldBoxScale(true);
            Handles.matrix = Matrix4x4.TRS(node.GetWorldBoxPosition(r, worldBoxScale, false), script.transform.rotation, worldBoxScale);
            Handles.DrawWireCube(Vector3.zero, r.AdapterBounds.size);

            var id = 0;
            float result;
            if (script.WidthType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 0, script, out result))
                {
                    Record(script);
                    script.Width = result;
                    MarkDirty(script);
                }
            }

            if (script.HeightType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 1, script, out result))
                {
                    Record(script);
                    script.Height = result;
                    MarkDirty(script);
                }
            }

            if (script.DepthType == SizeType.Fixed)
            {
                if (CreateSizeHandles(id++, id++, r.AdapterBounds.size, 2, script, out result))
                {
                    Record(script);
                    script.Depth = result;
                    MarkDirty(script);
                }
            }
        }

        private bool CreateSizeHandles(int id1, int id2, Vector3 size, int axis, FlexalonObject script, out float result)
        {
            bool changed = false;
            result = 0;

            if (CreateSizeHandleOnSide(id1, size, axis, 1, script, out float r1))
            {
                result = r1;
                changed = true;
            }

            if (CreateSizeHandleOnSide(id2, size, axis, -1, script, out float r2))
            {
                result = r2;
                changed = true;
            }

            return changed;
        }

        private bool CreateSizeHandleOnSide(int id, Vector3 size, int axis, int positive, FlexalonObject script, out float result)
        {
            var cid = GUIUtility.GetControlID(id, FocusType.Passive);
            var p = new Vector3();
            p[axis] = size[axis] / 2 * positive;
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.FreeMoveHandle(cid, p, Quaternion.identity, HandleUtility.GetHandleSize(p) * 0.2f, Vector3.one * 0.1f, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                result = newPos[axis] * 2 * positive;
                return true;
            }

            result = 0;
            return false;
        }
    }
}