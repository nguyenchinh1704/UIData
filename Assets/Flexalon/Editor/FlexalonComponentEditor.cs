using UnityEditor;
using UnityEngine;

namespace Flexalon.Editor
{
    [CustomEditor(typeof(FlexalonComponent)), CanEditMultipleObjects]
    public class FlexalonComponentEditor : UnityEditor.Editor
    {
        public static void Create<T>(string name) where T : MonoBehaviour
        {
            FlexalonEditor.Create();
            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            go.AddComponent<T>();
            Selection.activeGameObject = go;
        }

        protected void ForceUpdateButton()
        {
            if (GUILayout.Button("Force Update"))
            {
                foreach (var target in targets)
                {
                    ForceUpdate(target as FlexalonComponent);
                }
            }
        }

        protected void ApplyModifiedProperties()
        {
            if (serializedObject.ApplyModifiedProperties())
            {
                foreach (var target in targets)
                {
                    Record(target as FlexalonComponent);
                    MarkDirty(target as FlexalonComponent);
                }
            }
        }

        protected void Record(FlexalonComponent script)
        {
            Undo.RecordObject(script, "Record Component Edit");
            PrefabUtility.RecordPrefabInstancePropertyModifications(script);
            Undo.RecordObject(script.Node.Result, "Record Component Edit");
            PrefabUtility.RecordPrefabInstancePropertyModifications(script.Node.Result);
            Flexalon.GetOrCreate().RecordFrameChanges = true;
        }

        protected void MarkDirty(FlexalonComponent script)
        {
            script.MarkDirty();
            Flexalon.GetOrCreate().UpdateDirtyNodes();
        }

        protected void ForceUpdate(FlexalonComponent script)
        {
            Record(script);
            script.ForceUpdate();
        }
    }
}