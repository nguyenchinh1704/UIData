using UnityEngine;
using System.Collections.Generic;

namespace Flexalon
{
    [ExecuteAlways, HelpURL("https://www.flexalon.com/docs/flexibleLayout")]
    public class FlexalonFlexibleLayout : LayoutBase
    {
        [SerializeField]
        private Direction _direction = Direction.PositiveX;
        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private bool _wrap;
        public bool Wrap
        {
            get { return _wrap; }
            set { _wrap = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Direction _wrapDirection = Direction.NegativeY;
        public Direction WrapDirection
        {
            get { return _wrapDirection; }
            set { _wrapDirection = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _horizontalAlign = Align.Center;
        public Align HorizontalAlign
        {
            get { return _horizontalAlign; }
            set { _horizontalAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        public Align VerticalAlign
        {
            get { return _verticalAlign; }
            set { _verticalAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthAlign = Align.Center;
        public Align DepthAlign
        {
            get { return _depthAlign; }
            set { _depthAlign = value; _node.MarkDirty(); }
        }

            [SerializeField]
        private Align _horizontalInnerAlign = Align.Center;
        public Align HorizontalInnerAlign
        {
            get { return _horizontalInnerAlign; }
            set { _horizontalInnerAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalInnerAlign = Align.Center;
        public Align VerticalInnerAlign
        {
            get { return _verticalInnerAlign; }
            set { _verticalInnerAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _depthInnerAlign = Align.Center;
        public Align DepthInnerAlign
        {
            get { return _depthInnerAlign; }
            set { _depthInnerAlign = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _gap;
        public float Gap
        {
            get { return _gap; }
            set { _gap = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _wrapGap;
        public float WrapGap
        {
            get { return _wrapGap; }
            set { _wrapGap = value; _node.MarkDirty(); }
        }

        class Line
        {
            public Vector3 Size = Vector3.zero;
            public Vector3 Position = Vector3.zero;
            public List<FlexalonNode> Children = new List<FlexalonNode>();
            public List<Vector3> ChildSizes = new List<Vector3>();
            public List<Vector3> ChildFillSizes = new List<Vector3>();
            public List<Vector3> ChildPositions = new List<Vector3>();
            public float WrapPercent = 0;
            public bool Fill = false;
        }

        private List<Line> _lines = new List<Line>();

        private void CreateLines(FlexalonNode node, int flexAxis, int wrapAxis, int thirdAxis, bool wrap, Vector3 size)
        {
            _lines.Clear();

            // Divide children into lines considering: size, child sizes.
            var line = new Line();
            _lines.Add(line);
            bool addGap = false;
            int i = 0;
            foreach (var child in node.Children)
            {
                var gap = (addGap ? _gap : 0);
                var childSize = child.GetMeasureSize();
                if (line.ChildSizes.Count > 0 && wrap &&
                    line.Size[flexAxis] + childSize[flexAxis] + gap > size[flexAxis])
                {
                    line = new Line();
                    _lines.Add(line);
                    addGap = false;
                    gap = 0;
                    i++;
                }

                FlexalonLog.Log("FlexMeasure | Add child to line", child, i);
                FlexalonLog.Log("FlexMeasure | Child Size", child, childSize);
                line.ChildSizes.Add(childSize);
                line.ChildFillSizes.Add(childSize);
                line.Size[flexAxis] += childSize[flexAxis] + gap;
                line.Size[wrapAxis] = Mathf.Max(line.Size[wrapAxis], childSize[wrapAxis]);
                line.Size[thirdAxis] = Mathf.Max(line.Size[thirdAxis], childSize[thirdAxis]);
                line.Children.Add(child);
                addGap = true;
            }
        }

        private Vector3 MeasureTotalLineSize(bool wrap, int flexAxis, int wrapAxis, int thirdAxis)
        {
            Vector3 layoutSize = Vector3.zero;
            foreach (var line in _lines)
            {
                if (wrap)
                {
                    layoutSize[flexAxis] = Mathf.Max(layoutSize[flexAxis], line.Size[flexAxis]);
                    layoutSize[wrapAxis] += line.Size[wrapAxis];
                    layoutSize[thirdAxis] = Mathf.Max(layoutSize[thirdAxis], line.Size[thirdAxis]);
                }
                else
                {
                    for (int axis = 0; axis < 3; axis++)
                    {
                        layoutSize[axis] = Mathf.Max(layoutSize[axis], line.Size[axis]);
                    }
                }
            }

            if (wrap)
            {
                layoutSize[wrapAxis] += _wrapGap * (_lines.Count - 1);
            }

            return layoutSize;
        }

        private Vector3 MeasureLayoutSize(FlexalonNode node, bool wrap, int flexAxis, int wrapAxis, int thirdAxis, Vector3 size)
        {
            Vector3 totalSize = MeasureTotalLineSize(wrap, flexAxis, wrapAxis, thirdAxis);

            for (int axis = 0; axis < 3; axis++)
            {
                if (node.GetSizeType((Axis)axis) != SizeType.Layout)
                {
                    totalSize[axis] = size[axis];
                }
            }

            return totalSize;
        }

        private bool UpdatePercentChildSizes(Vector3 size, int flexAxis, int wrapAxis, int thirdAxis)
        {
            // Update sizes of % children to fill the remainder of their lines
            float wrapPercentTotal = 0;
            bool changedSomething = false;
            float wrapRemainingSpace = size[wrapAxis] - (_lines.Count - 1 ) * _wrapGap;
            foreach (var line in _lines)
            {
                line.Fill = true;
                float remainingSpace = size[flexAxis] - line.Size[flexAxis];
                float flexPercentTotal = 0;
                for (int i = 0; i < line.Children.Count; i++)
                {
                    var child = line.Children[i];
                    if (child.GetSizeType(flexAxis) == SizeType.Fill)
                    {
                        flexPercentTotal += child.SizeOfParent[flexAxis];
                    }

                    if (child.GetSizeType(wrapAxis) == SizeType.Fill)
                    {
                        line.WrapPercent = Mathf.Max(line.WrapPercent, child.SizeOfParent[wrapAxis]);
                    }
                    else
                    {
                        line.Fill = false;
                    }

                    if (child.GetSizeType(thirdAxis) == SizeType.Fill)
                    {
                        var childSize = line.ChildSizes[i];
                        var childFillSize = line.ChildFillSizes[i];
                        childSize[thirdAxis] = size[thirdAxis] * child.SizeOfParent[thirdAxis];
                        childFillSize[thirdAxis] = size[thirdAxis];
                        line.Size[thirdAxis] = Mathf.Max(line.Size[thirdAxis], childSize[thirdAxis]);
                        changedSomething = true;
                        FlexalonLog.Log("FlexMeasure | Update Third Axis", child, childSize);
                        line.ChildSizes[i] = childSize;
                        line.ChildFillSizes[i] = childFillSize;
                    }
                }

                if (flexPercentTotal > 0)
                {
                    var factor = flexPercentTotal > 1 ? (1 / flexPercentTotal) : 1;
                    for (int i = 0; i < line.Children.Count; i++)
                    {
                        var child = line.Children[i];
                        var childSize = line.ChildSizes[i];
                        var childFillSize = line.ChildFillSizes[i];
                        if (child.GetSizeType(flexAxis) == SizeType.Fill)
                        {
                            childSize[flexAxis] = remainingSpace * child.SizeOfParent[flexAxis] * factor;
                            childFillSize[flexAxis] = remainingSpace * factor;
                            line.Size[flexAxis] += childSize[flexAxis];
                            changedSomething = true;
                            FlexalonLog.Log("FlexMeasure | Update Flex Axis", child, line.ChildSizes[i]);
                            FlexalonLog.Log("FlexMeasure | New Line Size", child, line.Size);
                            line.ChildSizes[i] = childSize;
                            line.ChildFillSizes[i] = childFillSize;
                        }
                    }
                }

                if (line.Fill)
                {
                    wrapPercentTotal += line.WrapPercent;
                }
                else
                {
                    wrapRemainingSpace -= line.Size[wrapAxis];
                }
            }

            var lineFillFactor = wrapPercentTotal > 1 ? (1 / wrapPercentTotal) : wrapPercentTotal;
            foreach (var line in _lines)
            {
                if (line.Fill)
                {
                    line.Size[wrapAxis] = wrapRemainingSpace * line.WrapPercent * lineFillFactor;
                    for (int i = 0; i < line.Children.Count; i++)
                    {
                        var child = line.Children[i];
                        if (child.GetSizeType(wrapAxis) == SizeType.Fill)
                        {
                            var childSize = line.ChildSizes[i];
                            var childFillSize = line.ChildFillSizes[i];

                            childSize[wrapAxis] = wrapRemainingSpace * child.SizeOfParent[wrapAxis] * lineFillFactor;
                            childFillSize[wrapAxis] = wrapRemainingSpace * lineFillFactor;
                            changedSomething = true;

                            FlexalonLog.Log("FlexMeasure | Update Wrap Axis (fill remaining)", child, childSize);

                            line.ChildSizes[i] = childSize;
                            line.ChildFillSizes[i] = childFillSize;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < line.Children.Count; i++)
                    {
                        var child = line.Children[i];
                        if (child.GetSizeType(wrapAxis) == SizeType.Fill)
                        {
                            var childSize = line.ChildSizes[i];
                            var childFillSize = line.ChildFillSizes[i];

                            childSize[wrapAxis] = line.Size[wrapAxis] * child.SizeOfParent[wrapAxis];
                            childFillSize[wrapAxis] = line.Size[wrapAxis];
                            changedSomething = true;

                            FlexalonLog.Log("FlexMeasure | Update Wrap Axis (fill line)", child, childSize);
                            line.ChildSizes[i] = childSize;
                            line.ChildFillSizes[i] = childFillSize;
                        }
                    }
                }
            }

            return changedSomething;
        }

        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            FlexalonLog.Log("FlexMeasure | Size", node, size);

            // Gather useful data
            var flexAxis = (int) Math.GetAxisFromDirection(_direction);
            var otherAxes = Math.GetOtherAxes(flexAxis);
            bool childrenSizeFlexAxis = node.GetSizeType(flexAxis) == SizeType.Layout;
            var wrapAxis = (int) Math.GetAxisFromDirection(_wrapDirection);
            if (wrapAxis == flexAxis)
            {
                wrapAxis = otherAxes.Item1;
            }

            var thirdAxis = (wrapAxis == otherAxes.Item1 ? otherAxes.Item2 : otherAxes.Item1);
            bool wrap = !childrenSizeFlexAxis && (flexAxis != wrapAxis) && _wrap;

            FlexalonLog.Log("FlexMeasure | Flex Axis", node,  flexAxis);
            FlexalonLog.Log("FlexMeasure | Wrap Axis", node,  wrapAxis);
            FlexalonLog.Log("FlexMeasure | Third Axis", node,  thirdAxis);
            FlexalonLog.Log("FlexMeasure | Wrap", node, wrap);

            CreateLines(node, flexAxis, wrapAxis, thirdAxis, wrap, size);
            for (int i = 0; i < _lines.Count; i++)
            {
                FlexalonLog.Log("FlexMeasure | Line size " + i + " " + _lines[i].Size);
            }

            Vector3 totalSize = MeasureLayoutSize(node, wrap, flexAxis, wrapAxis, thirdAxis, size);
            FlexalonLog.Log("FlexMeasure | Total Size", node, totalSize);

            if (UpdatePercentChildSizes(totalSize, flexAxis, wrapAxis, thirdAxis))
            {
                totalSize = MeasureLayoutSize(node, wrap, flexAxis, wrapAxis, thirdAxis, size);
                FlexalonLog.Log("FlexMeasure | Update TotalSize", node, totalSize);
            }

            foreach (var line in _lines)
            {
                for (int i = 0; i < line.Children.Count; i++)
                {
                    line.Children[i].SetFillSize(line.ChildFillSizes[i]);
                }
            }

            return new Bounds(Vector3.zero, totalSize);
        }

        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("FlexArrange | LayoutSize", node, layoutSize);

            // Gather useful data
            var flexAxis = (int) Math.GetAxisFromDirection(_direction);
            var otherAxes = Math.GetOtherAxes(flexAxis);
            var wrapAxis = (int) Math.GetAxisFromDirection(_wrapDirection);
            if (wrapAxis == flexAxis)
            {
                wrapAxis = otherAxes.Item1;
            }

            var thirdAxis = (wrapAxis == otherAxes.Item1 ? otherAxes.Item2 : otherAxes.Item1);
            bool wrap = (flexAxis != wrapAxis) && _wrap;
            var flexDirection = Math.GetPositiveFromDirection(_direction);
            var wrapDirection = Math.GetPositiveFromDirection(_wrapDirection);
            var align = new Align[] { _horizontalAlign, _verticalAlign, _depthAlign };
            var innerAlign = new Align[] { _horizontalInnerAlign, _verticalInnerAlign, _depthInnerAlign };

            FlexalonLog.Log("FlexArrange | Flex Axis", node,  flexAxis);
            FlexalonLog.Log("FlexArrange | Wrap Axis", node,  wrapAxis);
            FlexalonLog.Log("FlexArrange | Third Axis", node,  thirdAxis);
            FlexalonLog.Log("FlexArrange | Wrap", node, wrap);

            // Position children within _lines. Consider: line size, child size, flexInnerAlign
            {
                foreach (var line in _lines)
                {
                    float nextChildPosition = flexDirection * -line.Size[flexAxis] / 2;
                    foreach (var childSize in line.ChildSizes)
                    {
                        Vector3 childPosition = Vector3.zero;
                        childPosition[flexAxis] = nextChildPosition + flexDirection * childSize[flexAxis] / 2;
                        childPosition[otherAxes.Item1] = Math.Align(
                            childSize, line.Size, otherAxes.Item1, innerAlign[otherAxes.Item1]);
                        childPosition[otherAxes.Item2] = Math.Align(
                            childSize, line.Size, otherAxes.Item2, innerAlign[otherAxes.Item2]);
                        line.ChildPositions.Add(childPosition);
                        nextChildPosition += flexDirection * (childSize[flexAxis] + _gap);
                    }
                }
            }

            for (int i = 0; i < _lines.Count; i++)
            {
                for (int j = 0; j < _lines[i].ChildPositions.Count; j++)
                {
                    FlexalonLog.Log("FlexArrange | Child Size", _lines[i].Children[j], _lines[i].ChildSizes[j]);
                    FlexalonLog.Log("FlexArrange | Child Position", _lines[i].Children[j], _lines[i].ChildPositions[j]);
                }
            }

            Vector3 totalLineSize = MeasureTotalLineSize(wrap, flexAxis, wrapAxis, thirdAxis);
            FlexalonLog.Log("FlexArrange | totalLineSize", node, totalLineSize);

            // Position lines in total line size, consider: totalLineSize, innerAlign
            {
                if (wrap)
                {
                    float nextLinePosition = wrapDirection * -totalLineSize[wrapAxis] / 2;
                    foreach (var line in _lines)
                    {
                        line.Position[wrapAxis] = nextLinePosition + wrapDirection * line.Size[wrapAxis] / 2;
                        line.Position[flexAxis] = Math.Align(
                            line.Size, totalLineSize, flexAxis, innerAlign[flexAxis]);
                        line.Position[thirdAxis] = Math.Align(
                            line.Size, totalLineSize, thirdAxis, innerAlign[thirdAxis]);
                        nextLinePosition += wrapDirection * line.Size[wrapAxis] + _wrapGap * wrapDirection;
                    }
                }
                else
                {
                    for (int axis = 0; axis < 3; axis++)
                    {
                        _lines[0].Position[axis] = Math.Align(
                            _lines[0].Size, totalLineSize, axis, innerAlign[axis]);
                    }
                }
            }

            for (int i = 0; i < _lines.Count; i++)
            {
                FlexalonLog.Log("FlexArrange | Line position " + i + " " + _lines[i].Position);
            }

            // Align the total line size within the size
            Vector3 alignOffset = Vector3.zero;
            for (int axis = 0; axis < 3; axis++)
            {
                alignOffset[axis] = Math.Align(totalLineSize, layoutSize, axis, align[axis]);
            }

            FlexalonLog.Log("FlexArrange | alignOffset", node, alignOffset);

            // Assign final child positions
            int childIndex = 0;
            foreach (var line in _lines)
            {
                foreach (var childPosition in line.ChildPositions)
                {
                    var child = node.Children[childIndex];
                    var result = alignOffset + line.Position + childPosition;
                    child.SetPositionResult(result);
                    child.SetRotationResult(Quaternion.identity);
                    FlexalonLog.Log("FlexArrange | ChildPosition", child, result);
                    childIndex++;
                }
            }

            _lines.Clear();
        }
    }
}