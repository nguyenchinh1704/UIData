using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(Flexalon))]
    public class FlexalonEditor : UnityEditor.Editor
    {
        public static void Create()
        {
            if (FindObjectOfType<Flexalon>() == null)
            {
                var flexalon = Flexalon.GetOrCreate();
                Undo.RegisterCreatedObjectUndo(flexalon.gameObject, "Create Flexalon");
            }
        }

        public override void OnInspectorGUI()
        {
            SerializedObject so = serializedObject;
            EditorGUILayout.PropertyField(so.FindProperty("_updateInEditMode"), true);
            EditorGUILayout.PropertyField(so.FindProperty("_updateInPlayMode"), true);

            if (so.ApplyModifiedProperties())
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }

            if ((Application.isPlaying && !(target as Flexalon).UpdateInPlayMode) ||
                (!Application.isPlaying && !(target as Flexalon).UpdateInEditMode))
            {
                if (GUILayout.Button("Update"))
                {
                    // TODO: We probably need to record all dirty objects
                    Undo.RecordObject(target, "Update");
                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                    var flexalon = (target as Flexalon);
                    flexalon.RecordFrameChanges = true;
                    flexalon.UpdateDirtyNodes();
                }
            }

            if (GUILayout.Button("Force Update"))
            {
                // TODO: We probably need to record all objects
                Undo.RecordObject(target, "Force Update");
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                var flexalon = (target as Flexalon);
                flexalon.RecordFrameChanges = true;
                flexalon.ForceUpdate();
            }

            EditorGUILayout.HelpBox("You should only have one Flexalon component in the scene. If you create a new one, disable and re-enable all flexalon components or restart Unity.", MessageType.Info);
        }
    }
}