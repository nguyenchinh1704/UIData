using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, DisallowMultipleComponent, HelpURL("https://www.flexalon.com/docs/constraints")]
    public class FlexalonConstraint : FlexalonComponent
    {
        [SerializeField]
        private GameObject _target;
        public GameObject Target
        {
            get { return _target; }
            set { _target = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalAlign = Align.Center;
        public Align HorizontalAlign
        {
            get { return _horizontalAlign; }
            set { _horizontalAlign = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        public Align VerticalAlign
        {
            get { return _verticalAlign; }
            set { _verticalAlign = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _depthAlign = Align.Center;
        public Align DepthAlign
        {
            get { return _depthAlign; }
            set { _depthAlign = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalPivot = Align.Center;
        public Align HorizontalPivot
        {
            get { return _horizontalPivot; }
            set { _horizontalPivot = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalPivot = Align.Center;
        public Align VerticalPivot
        {
            get { return _verticalPivot; }
            set { _verticalPivot = value; MarkDirty(); }
        }

        [SerializeField]
        private Align _depthPivot = Align.Center;
        public Align DepthPivot
        {
            get { return _depthPivot; }
            set { _depthPivot = value; MarkDirty(); }
        }

        private Vector3 _lastTargetPosition;
        private Quaternion _lastTargetRotation;
        private Vector3 _lastTargetScale;

        protected override void ResetProperties()
        {
            UpdateTarget(null);
        }

        public override void DoUpdate()
        {
            if (_target)
            {
                if (_lastTargetPosition != _target.transform.position ||
                    _lastTargetRotation != _target.transform.rotation ||
                    _lastTargetScale != _target.transform.lossyScale)
                {
                    MarkDirty();
                }
            }
        }

        protected override void UpdateProperties()
        {
            UpdateTarget(_target);
        }

        private void UpdateTarget(GameObject target)
        {
            if (target)
            {
                var targetNode = Flexalon.GetOrCreateNode(target);
                _node.SetConstraint(this, targetNode);

                if (!targetNode.HasResult)
                {
                    targetNode.MarkDirty();
                }

                _lastTargetPosition = target.transform.position;
                _lastTargetRotation = target.transform.rotation;
                _lastTargetScale = target.transform.lossyScale;
            }
            else
            {
                _node.SetConstraint(null, null);
            }
        }

        private float GetAlignPosition(Axis axis, Vector3 size, Align align, FlexalonNode targetNode)
        {
            float alignPos = 0;
            if (align == Align.Start)
            {
                var targetSize = size[(int)axis];
                alignPos = -targetSize * 0.5f;
            }
            else if (align == Align.Center)
            {
                alignPos = 0;
            }
            else if (align == Align.End)
            {
                var targetSize = size[(int)axis];
                alignPos = targetSize * 0.5f;
            }

            return alignPos;
        }

        private float GetPivotPosition(int axis, Vector3 size, Align pivot, FlexalonNode node)
        {
            var directions = Math.GetDirectionsFromAxis(axis);

            float pivotPos = 0;
            if (pivot == Align.Start)
            {
                pivotPos = -size[axis] * 0.5f;
            }
            else if (pivot == Align.Center)
            {
                pivotPos = 0;
            }
            else if (pivot == Align.End)
            {
                pivotPos = size[axis] * 0.5f;
            }

            return pivotPos;
        }

        public void Constrain(FlexalonNode node)
        {
            if (_target)
            {
                var targetNode = Flexalon.GetOrCreateNode(_target);
                var targetSize = targetNode.Result.AdapterBounds.size + targetNode.Margin.Size;
                var bounds = node.Result.RotatedAndScaledBounds;
                bounds.center += node.Margin.Center;
                bounds.size += node.Margin.Size;

                var alignPosition = new Vector3(
                    GetAlignPosition(Axis.X, targetSize, _horizontalAlign, targetNode),
                    GetAlignPosition(Axis.Y, targetSize, _verticalAlign, targetNode),
                    GetAlignPosition(Axis.Z, targetSize, _depthAlign, targetNode));

                FlexalonLog.Log("Constrain:AlignPosition [Initial]", node, alignPosition);

                alignPosition += targetNode.Result.AdapterBounds.center;
                alignPosition += targetNode.Margin.Center;
                FlexalonLog.Log("Constrain:AlignPosition [Centered]", node, alignPosition);

                alignPosition.Scale(targetNode.GetWorldBoxScale(true));
                FlexalonLog.Log("Constrain:AlignPosition [Scaled]", node, alignPosition);

                var pivotPosition = new Vector3(
                    GetPivotPosition(0, bounds.size, _horizontalPivot, node),
                    GetPivotPosition(1, bounds.size, _verticalPivot, node),
                    GetPivotPosition(2, bounds.size, _depthPivot, node));

                FlexalonLog.Log("Constrain:PivotPosition", node, pivotPosition);

                var worldRotation = _target.transform.rotation;
                var localRotation = Quaternion.Inverse(transform.parent?.rotation ?? Quaternion.identity) * worldRotation;

                var position = alignPosition - pivotPosition - bounds.center + node.Offset;
                var worldPosition = worldRotation * (position) + _target.transform.position;
                FlexalonLog.Log("Constrain:WorldPosition", node, worldPosition);

                var localPosition = transform.parent?.worldToLocalMatrix.MultiplyPoint(worldPosition) ?? worldPosition;
                FlexalonLog.Log("Constrain:LocalPosition", node, localPosition);

                node.SetPositionResult(localPosition);
                node.SetRotationResult(localRotation);
            }
        }
    }
}