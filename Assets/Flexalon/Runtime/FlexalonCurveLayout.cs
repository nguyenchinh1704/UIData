using System.Collections.Generic;
using UnityEngine;

namespace Flexalon
{
    [ExecuteAlways, HelpURL("https://www.flexalon.com/docs/curveLayout")]
    public class FlexalonCurveLayout : LayoutBase
    {
        [System.Serializable]
        public struct CurvePoint
        {
            public Vector3 Position;
            public Vector3 Tangent;
        }

        [SerializeField]
        private List<CurvePoint> _points = new List<CurvePoint>() {
            new CurvePoint() { Position = Vector3.left, Tangent = Vector3.zero },
            new CurvePoint() { Position = Vector3.zero, Tangent = Vector3.zero },
            new CurvePoint() { Position = Vector3.right, Tangent = Vector3.zero },
        };

        public IReadOnlyList<CurvePoint> Points => _points;

        public enum SpacingOptions
        {
            Fixed,
            Evenly,
        }

        [SerializeField]
        private bool _lockTangents = false;
        public bool LockTangents => _lockTangents;

        [SerializeField]
        private bool _lockPositions = false;
        public bool LockPositions => _lockPositions;

        [SerializeField]
        private SpacingOptions _spacingType;
        public SpacingOptions SpacingType
        {
            get { return _spacingType; }
            set { _spacingType = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _spacing = 0.5f;
        public float Spacing
        {
            get { return _spacing; }
            set { _spacing = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _startAt = 0.0f;
        public float StartAt
        {
            get { return _startAt; }
            set { _startAt = value; _node.MarkDirty(); }
        }

        public enum RotationOptions
        {
            None,
            In,
            Out,
            InWithRoll,
            OutWithRoll,
            Forward,
            Backward
        }

        [SerializeField]
        private RotationOptions _rotation;
        public RotationOptions Rotation
        {
            get { return _rotation; }
            set { _rotation = value; _node.MarkDirty(); }
        }

        public void AddPoint(CurvePoint point)
        {
            _points.Add(point);
            MarkDirty();
        }

        public void AddPoint(Vector3 position, Vector3 tangent)
        {
            AddPoint(new CurvePoint{ Position = position, Tangent = tangent });
        }


        public void InsertPoint(int index, CurvePoint point)
        {
            _points.Insert(index, point);
            MarkDirty();
        }

        public void InsertPoint(int index, Vector3 position, Vector3 tangent)
        {
            InsertPoint(index, new CurvePoint{ Position = position, Tangent = tangent });
        }

        public void ReplacePoint(int index, CurvePoint point)
        {
            _points.RemoveAt(index);
            InsertPoint(index, point);
        }

        public void ReplacePoint(int index, Vector3 position, Vector3 tangent)
        {
            ReplacePoint(index, new CurvePoint{ Position = position, Tangent = tangent });
        }

        public void RemovePoint(int index)
        {
            _points.RemoveAt(index);
            MarkDirty();
        }

        [SerializeField] // Saved for editor visual
        private List<Vector3> _curvePositions = new List<Vector3>();
        private List<float> _curveLengths = new List<float>();
        private float _curveLength = 0;
        private Bounds _curveBounds;
        private List<CurvePoint> _computedPoints = new List<CurvePoint>();

        public IReadOnlyList<Vector3> CurvePositions => _curvePositions;

        private void UpdateCurvePositions()
        {
            if (_computedPoints.Count == _points.Count)
            {
                bool curveChanged = false;
                for (int i = 0; i < _points.Count; i++)
                {
                    if (_computedPoints[i].Position != _points[i].Position || _computedPoints[i].Tangent != _points[i].Tangent)
                    {
                        curveChanged = true;
                        break;
                    }
                }

                if (!curveChanged)
                {
                    return;
                }
            }

            _curvePositions.Clear();
            _curveLengths.Clear();
            _curveLength = 0;
            _computedPoints.Clear();
            _computedPoints.AddRange(_points);

            if (_points.Count == 0)
            {
                _curveBounds = new Bounds();
                return;
            }

            _curvePositions.Add(_points[0].Position);
            _curveLengths.Add(0);
            _curveBounds = new Bounds(_points[0].Position, Vector3.zero);
            var prev = _points[0].Position;
            for (int i = 1; i < _points.Count; i++)
            {
                for (int j = 1; j <= 100; j++)
                {
                    var pos = ComputePositionOnBezierCurve(_points[i - 1], _points[i], (float) j / 100);
                    var len = Vector3.Distance(prev, pos);
                    _curvePositions.Add(pos);
                    _curveLength += len;
                    _curveLengths.Add(_curveLength);
                    _curveBounds.Encapsulate(pos);
                    prev = pos;
                }
            }
        }

        private (Vector3, Vector3) GetCurvePositionAndForwardAtDistance(float distance)
        {
            // Assumes > 1 points
            distance = Mathf.Clamp(distance, 0, _curveLength);

            int s = 0;
            int e = _curvePositions.Count - 1;

            int i = 0;
            while (s != e && i < 100)
            {
                i++;
                var m = s + (e - s) / 2;
                if (_curveLengths[m] <= distance)
                {
                    s = m + 1;
                }
                else
                {
                    e = m;
                }
            }

            // We should be at the next position after distance.
            var t = (distance -_curveLengths[e - 1]) / (_curveLengths[e] - _curveLengths[e - 1]);
            var p = Vector3.Lerp(_curvePositions[e - 1], _curvePositions[e], t);
            var f = _curvePositions[e] - _curvePositions[e - 1];
            return (p, f.normalized);
        }

        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            FlexalonLog.Log("CurveMeasure | Size", node, size);

            UpdateCurvePositions();

            Vector3 center = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                if (node.GetSizeType(i) == SizeType.Layout)
                {
                    center[i] = _curveBounds.center[i];
                    size[i] = _curveBounds.size[i];
                }
            }

            var childFillSizeXZ = _curveLength / node.Children.Count;
            var childFillSize = new Vector3(childFillSizeXZ, size.y, childFillSizeXZ);
            base.Measure(node, childFillSize);

            return new Bounds(center, size);
        }

        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("CurveArrange | LayoutSize", node, layoutSize);

            if (node.Children.Count == 0 || _points == null || _points.Count < 2)
            {
                return;
            }

            var spacing = _spacing;
            var startAt = _startAt;
            if (_spacingType == SpacingOptions.Evenly)
            {
                spacing = _curveLength / node.Children.Count;
                startAt = _startAt + spacing / 2;
            }

            var d = startAt;
            for (int i = 0; i < node.Children.Count; i++)
            {
                var (position, forward) = GetCurvePositionAndForwardAtDistance(d);
                node.Children[i].SetPositionResult(position);
                d += spacing;

                var rotation = Quaternion.identity;
                var inDirection = Vector3.Cross(forward, Vector3.up).normalized;
                switch (_rotation)
                {
                    case RotationOptions.In:
                        rotation = Quaternion.LookRotation(inDirection);
                        break;
                    case RotationOptions.Out:
                        rotation = Quaternion.LookRotation(-inDirection);
                        break;
                    case RotationOptions.InWithRoll:
                        rotation = Quaternion.LookRotation(inDirection, Vector3.Cross(inDirection, forward));
                        break;
                    case RotationOptions.OutWithRoll:
                        rotation = Quaternion.LookRotation(-inDirection, Vector3.Cross(inDirection, forward));
                        break;
                    case RotationOptions.Forward:
                        rotation = Quaternion.LookRotation(forward);
                        break;
                    case RotationOptions.Backward:
                        rotation = Quaternion.LookRotation(-forward);
                        break;
                }

                node.Children[i].SetRotationResult(rotation);
            }
        }

        public static Vector3 ComputePositionOnBezierCurve(CurvePoint point1, CurvePoint point2, float t)
        {
            Vector3 p1 = point1.Position;
            Vector3 p2 = point1.Position + point1.Tangent;
            Vector3 p3 = point2.Position - point2.Tangent;
            Vector3 p4 = point2.Position;

            float a = Mathf.Pow(1 - t, 3);
            float b = 3 * Mathf.Pow(1 - t, 2) * t;
            float c = 3 * (1 - t) * Mathf.Pow(t, 2);
            float d = Mathf.Pow(t, 3);
            return p1 * a + p2 * b + p3 * c + p4 * d;
        }
    }
}