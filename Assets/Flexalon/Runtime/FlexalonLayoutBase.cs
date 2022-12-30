using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, DisallowMultipleComponent]
    public abstract class LayoutBase : FlexalonComponent, Layout
    {
        protected override void DoOnEnable()
        {
            if (GetComponent<FlexalonObject>() == null)
            {
                var obj = gameObject.AddComponent<FlexalonObject>();
                obj.WidthType = SizeType.Layout;
                obj.HeightType = SizeType.Layout;
                obj.DepthType = SizeType.Layout;
            }

            _node.DetachAllChildren();
            foreach (Transform child in transform)
            {
                _node.AddChild(Flexalon.GetOrCreateNode(child.gameObject));
            }
        }

        protected override void ResetProperties()
        {
            _node.SetMethod(null);
            _node.DetachAllChildren();
        }

        protected override void UpdateProperties()
        {
            _node.SetMethod(this);
        }

        // This function is complicated because it's working around two issues.
        // First, OnTransformChildrenChanged doesn't always run on 2019.4 due to a bug.
        // See https://issuetracker.unity3d.com/issues/ontransformchildrenchanged-doesnt-get-called-in-the-edit-mode-when-dragging-a-prefab-from-the-project-window-to-the-hierarchy
        // Second, we need to deal with undo/redo. The strategy here is to do nothing on undo/redo except fix
        // the node.Children list, since it isn't serialzed. To detect undo/redo, we check if the Parent or SiblingIndex
        // values change in the serialized FlexalonResult matches the transform children.
        public override void DoUpdate()
        {
            // Check if any old children changed parents. They need to be marked dirty
            // since their size may change after leaving the layout.
            for (int i = 0; i < _node.Children.Count; i++)
            {
                var childNode = _node.Children[i];
                if (childNode.GameObject && !childNode.GameObject.transform.IsChildOf(transform))
                {
                    i--;
                    childNode.Detach();
                    if (childNode.Result.Parent == transform)
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.RecordObject(childNode.Result, "Parent change");
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(childNode.Result);
                        Flexalon.GetOrCreate().RecordFrameChanges = true;
#endif
                        childNode.Result.Parent = childNode.GameObject.transform.parent;
                        childNode.Result.SiblingIndex = childNode.GameObject.transform.GetSiblingIndex();
                        childNode.MarkDirty();
                    }
                }
            }

            // Check if we have any new or out of order children.
            foreach (Transform child in transform)
            {
                var childNode = Flexalon.GetOrCreateNode(child.gameObject);
                var index = child.GetSiblingIndex();
                _node.InsertChild(childNode, index);
                if (childNode.Result.Parent != transform || childNode.Result.SiblingIndex != index)
                {
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(childNode.Result, "Parent change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(childNode.Result);
                    Flexalon.GetOrCreate().RecordFrameChanges = true;
#endif
                    childNode.Result.Parent = transform;
                    childNode.Result.SiblingIndex = child.GetSiblingIndex();
                    childNode.ApplyScaleAndRotation();
                    MarkDirty();
                }
            }
        }

        public virtual Bounds Measure(FlexalonNode node, Vector3 size)
        {
            foreach (var child in node.Children)
            {
                child.SetFillSize(size);
            }

            return new Bounds(Vector3.zero, size);
        }

        public virtual void Arrange(FlexalonNode node, Vector3 layoutSize) {}
    }
}