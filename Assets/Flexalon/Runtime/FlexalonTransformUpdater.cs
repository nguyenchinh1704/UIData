using UnityEngine;

namespace Flexalon
{
    public interface TransformUpdater
    {
        bool UpdatePosition(FlexalonNode node, Vector3 position);
        bool UpdateRotation(FlexalonNode node, Quaternion rotation);
        bool UpdateScale(FlexalonNode node, Vector3 scale);
    }

    internal class DefaultTransformUpdater : TransformUpdater
    {
        private void RecordEdit(FlexalonNode node)
        {
#if UNITY_EDITOR
            if (Flexalon.GetOrCreate().RecordFrameChanges)
            {
                UnityEditor.Undo.RecordObject(node.GameObject.transform, "Flexalon transform change");
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(node.GameObject.transform);
            }
#endif
        }

        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            RecordEdit(node);
            node.GameObject.transform.localPosition = position;
            return true;
        }

        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            RecordEdit(node);
            node.GameObject.transform.localRotation = rotation;
            return true;
        }

        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            RecordEdit(node);
            node.GameObject.transform.localScale = scale;
            return true;
        }
    }
}