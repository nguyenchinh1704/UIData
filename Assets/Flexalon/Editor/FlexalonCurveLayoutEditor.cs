using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    // A tiny custom editor for ExampleScript component
    [CustomEditor(typeof(FlexalonCurveLayout)), CanEditMultipleObjects]
    public class FlexalonCurveLayoutEditor : FlexalonComponentEditor
    {
        [MenuItem("GameObject/Flexalon/Curve Layout")]
        public static void Create()
        {
            FlexalonComponentEditor.Create<FlexalonCurveLayout>("Curve Layout");
        }

        public override void OnInspectorGUI()
        {
            ForceUpdateButton();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_lockTangents"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_lockPositions"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_points"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spacingType"), true);

            if ((target as FlexalonCurveLayout).SpacingType == FlexalonCurveLayout.SpacingOptions.Fixed)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_spacing"), true);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_startAt"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_rotation"), true);
            ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            var curveLayout = target as FlexalonCurveLayout;
            var points = curveLayout.Points;
            var transform = curveLayout.gameObject.transform;

            if (points != null)
            {
                Handles.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var point1 = points[i];
                    var point2 = points[i + 1];

                    Handles.color = new Color(1, 1, 0, 0.5f);
                    Handles.DrawLine(point1.Position, point1.Position + point1.Tangent);
                    Handles.DrawLine(point2.Position, point2.Position - point2.Tangent);
                }

                Handles.color = new Color(1, 1, 1, 0.5f);
                for (int i = 1; i < curveLayout.CurvePositions.Count; i++)
                {
                    Handles.DrawLine(curveLayout.CurvePositions[i - 1], curveLayout.CurvePositions[i]);
                }

                for (int i = 0; i < points.Count; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    var p = points[i].Position;

                    if (!curveLayout.LockPositions)
                    {
                        Vector3 newPos = Handles.PositionHandle(p, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Record(curveLayout);
                            curveLayout.ReplacePoint(i, new FlexalonCurveLayout.CurvePoint { Position = newPos, Tangent = points[i].Tangent });
                            MarkDirty(curveLayout);
                        }
                    }

                    if (!curveLayout.LockTangents)
                    {
                        if (i < points.Count - 1)
                        {
                            EditorGUI.BeginChangeCheck();
                            p = points[i].Position + points[i].Tangent;
                            Vector3 newTan1 = Handles.PositionHandle(p, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Record(curveLayout);
                                curveLayout.ReplacePoint(i, new FlexalonCurveLayout.CurvePoint { Position = points[i].Position, Tangent = newTan1 - points[i].Position });
                                MarkDirty(curveLayout);
                            }
                        }

                        if (i > 0)
                        {
                            EditorGUI.BeginChangeCheck();
                            p = points[i].Position - points[i].Tangent;
                            Vector3 newTan2 = Handles.PositionHandle(p, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Record(curveLayout);
                                curveLayout.ReplacePoint(i, new FlexalonCurveLayout.CurvePoint { Position = points[i].Position, Tangent = points[i].Position - newTan2 });
                                MarkDirty(curveLayout);
                            }
                        }
                    }
                }
            }
        }
    }
}