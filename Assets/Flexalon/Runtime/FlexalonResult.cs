using UnityEngine;

namespace Flexalon
{
    [DisallowMultipleComponent]
    public class FlexalonResult : MonoBehaviour
    {
        // Parent layout
        public Transform Parent;

        // Index in layout
        public int SiblingIndex;

        // Arranged position in parent layout space.
        public Vector3 LayoutPosition = Vector3.zero;

        // Arranged rotation in parent layout space.
        public Quaternion LayoutRotation = Quaternion.identity;

        // Bounds deteremined by Adapter.Measure function.
        public Bounds AdapterBounds = new Bounds();

        // Combined bounds of Layout.Measure function and Adapter.Measure functions.
        public Bounds LayoutBounds = new Bounds();

        // Bounds after layout, scale and rotation used the the parent layout.
        public Bounds RotatedAndScaledBounds = new Bounds();

        // What the component updater thinks the scale should be in layout space.
        public Vector3 ComponentScale = Vector3.one;

        // Expected local transform set by the layout system.
        public Vector3 TargetPosition = Vector3.zero;
        public Quaternion TargetRotation = Quaternion.identity;
        public Vector3 TargetScale = Vector3.one;

        // Last transform set by transform updater. Used to detect unexpected changes.
        public Vector3 TransformPosition = Vector3.zero;
        public Quaternion TransformRotation = Quaternion.identity;
        public Vector3 TransformScale = Vector3.one;
    };
}