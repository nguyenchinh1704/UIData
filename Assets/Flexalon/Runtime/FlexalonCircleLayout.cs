using System.Collections.Generic;
using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, HelpURL("https://www.flexalon.com/docs/circleLayout")]
    public class FlexalonCircleLayout : LayoutBase
    {
        [SerializeField]
        private float _radius = 1;
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private bool _useWidth = false;
        public bool UseWidth
        {
            get { return _useWidth; }
            set { _useWidth = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private bool _spiral = false;
        public bool Spiral
        {
            get { return _spiral; }
            set { _spiral = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _spiralSpacing = 0;
        public float SpiralSpacing
        {
            get { return _spiralSpacing; }
            set { _spiralSpacing = value; _node.MarkDirty(); }
        }

        private float _spiralHeight = 0;

        public enum SpacingOptions
        {
            Fixed,
            Evenly,
        }

        [SerializeField]
        private SpacingOptions _spacingType;
        public SpacingOptions SpacingType
        {
            get { return _spacingType; }
            set { _spacingType = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _spacingDegrees = 30.0f;
        public float SpacingDegrees
        {
            get { return _spacingDegrees; }
            set { _spacingDegrees = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _startAtDegrees = 0.0f;
        public float StartAtDegrees
        {
            get { return _startAtDegrees; }
            set { _startAtDegrees = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        public Align VerticalAlign
        {
            get { return _verticalAlign; }
            set { _verticalAlign = value; _node.MarkDirty(); }
        }

        public enum RotateOptions
        {
            None,
            Out,
            In,
            Forward,
            Backwards
        }

        [SerializeField]
        private RotateOptions _rotate = RotateOptions.None;
        public RotateOptions Rotate
        {
            get { return _rotate; }
            set { _rotate = value; _node.MarkDirty(); }
        }

        private float GetSpacing(FlexalonNode node)
        {
            var spacing = _spacingDegrees * Mathf.PI / 180;
            if (_spacingType == SpacingOptions.Evenly)
            {
                if (node.Children.Count < 2)
                {
                    spacing = 0;
                }
                else
                {
                    spacing = 2 * Mathf.PI / node.Children.Count;
                }
            }

            return spacing;
        }

        private float GetRadius(Vector3 layoutSize)
        {
            return _useWidth ? layoutSize.x / 2 : _radius;
        }

        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            var spacing = GetSpacing(node);

            var diameter = _radius * 2;
            if (_useWidth)
            {
                float maxChildWidth = 0;
                foreach (var child in node.Children)
                {
                    maxChildWidth = Mathf.Max(maxChildWidth, child.GetMeasureSize().x);
                }

                diameter = maxChildWidth / Mathf.Tan(spacing / 2);
            }
            else
            {
                diameter = _radius * 2;
            }

            if (node.GetSizeType(Axis.X) == SizeType.Layout)
            {
                size.x = diameter;
            }

            if (_spiral)
            {
                _spiralHeight = _spiralSpacing * (node.Children.Count - 1);
                foreach (var child in node.Children)
                {
                    // Note: this GetMeasureSize will be 0 for any child axis using SizeType.Fill.
                    _spiralHeight += child.GetMeasureSize().y;
                }
            }

            if (node.GetSizeType(Axis.Y) == SizeType.Layout)
            {
                if (_spiral)
                {

                    size.y = _spiralHeight;
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        size.y = Mathf.Max(size.y, child.GetMeasureSize().y);
                    }
                }
            }

            if (node.GetSizeType(Axis.Z) == SizeType.Layout)
            {
                size.z = diameter;
            }

            float percentTotal = 0;
            foreach (var child in node.Children)
            {
                if (child.GetSizeType(Axis.Y) == SizeType.Fill)
                {
                    percentTotal += child.SizeOfParent[1];
                }
            }

            float remainingHeight = Mathf.Max(0, size.y - _spiralHeight);

            var childAvailableWidth = (node.Children.Count <= 2 && _spacingType == SpacingOptions.Evenly) ? 1 :
                diameter * Mathf.Tan(spacing / 2);

            foreach (var child in node.Children)
            {
                float childAvailableHeight = size.y;
                if (_spiral)
                {
                    var percent = percentTotal <= 1 ?
                        child.SizeOfParent[1] :
                        (child.SizeOfParent[1] / percentTotal);
                    childAvailableHeight = percent * remainingHeight;
                }

                child.SetFillSize(new Vector3(childAvailableWidth, childAvailableHeight, childAvailableWidth));
            }

            return new Bounds(Vector3.zero, size);
        }

        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("CircleArrange | LayoutSize", node, layoutSize);

            var startAt = _startAtDegrees * Mathf.PI / 180;
            var spacing = GetSpacing(node);
            var radius = GetRadius(layoutSize);

            _spiralHeight = _spiralSpacing * (node.Children.Count - 1);
            foreach (var child in node.Children)
            {
                _spiralHeight += child.GetArrangeSize().y;
            }

            float spiralPos = Math.Align(_spiralHeight, layoutSize.y, _verticalAlign) - _spiralHeight / 2;

            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                var childSize = child.GetArrangeSize();
                var pos = new Vector3(
                    radius * Mathf.Cos(i * spacing + startAt),
                    0,
                    radius * Mathf.Sin(i * spacing + startAt));

                if (_spiral)
                {
                    pos.y = spiralPos + childSize.y * 0.5f;
                    spiralPos += childSize.y + _spiralSpacing;
                }
                else
                {
                    pos.y = Math.Align(childSize, layoutSize, Axis.Y, _verticalAlign);
                }

                child.SetPositionResult(pos);

                float rotation = -i * spacing - startAt;
                switch (_rotate)
                {
                    case RotateOptions.None:
                        rotation = 0;
                        break;
                    case RotateOptions.Forward:
                        break;
                    case RotateOptions.Backwards:
                        rotation += Mathf.PI;
                        break;
                    case RotateOptions.In:
                        rotation += Mathf.PI * 0.5f;
                        break;
                    case RotateOptions.Out:
                        rotation -= Mathf.PI * 0.5f;
                        break;
                }

                var q = Quaternion.AngleAxis(rotation * 180.0f / Mathf.PI, Vector3.up);
                child.SetRotationResult(q);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_node != null)
            {
                // Draw a semitransparent circle at the transforms position
                Gizmos.color = new Color(1, 1, 0, 0.5f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                int segments = 30;

                var scale = _node.GetWorldBoxScale(true);

                var radius = _radius;
                if (_useWidth)
                {
                    radius = _node.Result.AdapterBounds.size.x * scale.x / 2;
                }

                for (int i = 0; i < segments; i++)
                {
                    var a1 = Mathf.PI * 2 * (i / (float)segments);
                    var a2 = Mathf.PI * 2 * ((i + 1) / (float)segments);
                    var p1 = new Vector3(radius * Mathf.Cos(a1), 0, radius * Mathf.Sin(a1));
                    var p2 = new Vector3(radius * Mathf.Cos(a2), 0, radius * Mathf.Sin(a2));
                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
    }
}