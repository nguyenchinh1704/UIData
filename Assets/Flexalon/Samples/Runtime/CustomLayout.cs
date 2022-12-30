using System.Collections.Generic;
using UnityEngine;

namespace Flexalon.Samples
{
    // This is an example of how to implement your own Layout.
    // The layout strategy is to place the children on after
    // the other diagonally ascending.
    // See also CustomLayoutEditor.
    [ExecuteAlways]
    public class CustomLayout : LayoutBase
    {
        [SerializeField]
        private Vector3 _gap = Vector3.zero;

        public static Vector3 AggregateLayoutSizes(IReadOnlyList<FlexalonNode> nodes)
        {
            var totalSize = Vector3.zero;
            foreach (var child in nodes)
            {
                // Note: this GetMeasureSize will be 0 for any child axis using SizeType.Fill.
                totalSize += child.GetMeasureSize();
            }

            return totalSize;
        }

        public static void FillRemainingSpace(IReadOnlyList<FlexalonNode> nodes, Vector3 space)
        {
            space = Vector3.Max(space, Vector3.zero);

            // If child percentages add up to more than 100%, we need to distribute
            // the remaining space between them proportionally. For example, if two
            // children have WidthOfParent == 1, then each get 50% of the remaining space.
            var percentTotal = Vector3.zero;
            foreach (var child in nodes)
            {
                for (int axis = 0; axis < 3; axis++)
                {
                    if (child.GetSizeType(axis) == SizeType.Fill)
                    {
                        percentTotal[axis] += child.SizeOfParent[axis];
                    }
                }
            }

            foreach (var child in nodes)
            {
                var fillSize = Vector3.zero;
                for (int axis = 0; axis < 3; axis++)
                {
                    if (child.GetSizeType(axis) == SizeType.Fill)
                    {
                        var percent = percentTotal[axis] <= 1 ?
                            child.SizeOfParent[axis] :
                            (child.SizeOfParent[axis] / percentTotal[axis]);
                        fillSize[axis] = percent * space[axis];
                    }
                }

                child.SetFillSize(fillSize);
            }
        }

        // Measure update the size of this node by accounting
        // for any axes which are assigned SizeType.Layout. This method
        // should also determine the sizes of any children using SizeType.Fill
        // by calling SetFillSize.
        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            // The layout size should be the sum of all child sizes.
            var aggregateSize = AggregateLayoutSizes(node.Children);

            // Make sure to add the gaps between children.
            aggregateSize += _gap * (node.Children.Count - 1);

            // Children using SizeType.Fill should take up the remaining space.
            FillRemainingSpace(node.Children, size - aggregateSize);

            // Adjust the size for axes which are SizeType.Layout.
            for (int axis = 0; axis < 3; axis++)
            {
                if (node.GetSizeType(axis) == SizeType.Layout)
                {
                    size[axis] = aggregateSize[axis];
                }
            }

            return new Bounds(Vector3.zero, size);
        }

        // Arrange the children in a diagonal pattern.
        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            var nextPosition = -layoutSize / 2;
            foreach (var child in node.Children)
            {
                var childSize = child.GetArrangeSize();
                child.SetPositionResult(nextPosition + childSize / 2);
                child.SetRotationResult(Quaternion.identity);
                nextPosition += childSize + _gap;
            }
        }
    }
}