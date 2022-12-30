using UnityEngine;

namespace Flexalon
{
    public static class Math
    {
        public static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.PositiveX: return Direction.NegativeX;
                case Direction.NegativeX: return Direction.PositiveX;
                case Direction.PositiveY: return Direction.NegativeY;
                case Direction.NegativeY: return Direction.PositiveY;
                case Direction.PositiveZ: return Direction.NegativeZ;
                case Direction.NegativeZ: return Direction.PositiveZ;
                default: return Direction.PositiveX;
            }
        }

        public static Direction GetOppositeDirection(int direction)
        {
            return GetOppositeDirection((Direction)direction);
        }

        public static Axis GetAxisFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.PositiveX: return Axis.X;
                case Direction.NegativeX: return Axis.X;
                case Direction.PositiveY: return Axis.Y;
                case Direction.NegativeY: return Axis.Y;
                case Direction.PositiveZ: return Axis.Z;
                case Direction.NegativeZ: return Axis.Z;
                default: return Axis.X;
            }
        }

        public static Axis GetAxisFromDirection(int direction)
        {
            return GetAxisFromDirection((Direction)direction);
        }

        public static (Direction, Direction) GetDirectionsFromAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return (Direction.PositiveX, Direction.NegativeX);
                case Axis.Y: return (Direction.PositiveY, Direction.NegativeY);
                case Axis.Z: return (Direction.PositiveZ, Direction.NegativeZ);
                default: return (Direction.PositiveX, Direction.NegativeX);
            }
        }

        public static (Direction, Direction) GetDirectionsFromAxis(int axis)
        {
            return GetDirectionsFromAxis((Axis)axis);
        }

        public static float GetPositiveFromDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.PositiveX:
                case Direction.PositiveY:
                case Direction.PositiveZ:
                    return 1;
                default:
                    return -1;
            }
        }

        public static float GetPositiveFromDirection(int direction)
        {
            return GetPositiveFromDirection((Direction)direction);
        }

        public static (Axis, Axis) GetOtherAxes(Axis axis)
        {
            switch (axis)
            {
                case Axis.X: return (Axis.Y, Axis.Z);
                case Axis.Y: return (Axis.X, Axis.Z);
                default: return (Axis.X, Axis.Y);
            }
        }

        public static (int, int) GetOtherAxes(int axis)
        {
            var other = GetOtherAxes((Axis)axis);
            return ((int)other.Item1, (int)other.Item2);
        }

        public static Axis GetThirdAxis(Axis axis1, Axis axis2)
        {
            var otherAxes = GetOtherAxes(axis1);
            return (otherAxes.Item1 == axis2) ? otherAxes.Item2 : otherAxes.Item1;
        }

        public static int GetThirdAxis(int axis1, int axis2)
        {
            return (int) GetThirdAxis((Axis)axis1, (Axis)axis2);
        }

        public static Vector3 Mul(Vector3 a, Vector3 b)
        {
            a.x *= b.x;
            a.y *= b.y;
            a.z *= b.z;
            return a;
        }

        public static Vector3 Div(Vector3 a, Vector3 b)
        {
            a.x /= b.x;
            a.y /= b.y;
            a.z /= b.z;
            return a;
        }

        public static float Min(Vector3 a)
        {
            return Mathf.Min(a.x, a.y, a.z);
        }

        public static Bounds RotateBounds(Bounds bounds, Quaternion rotation)
        {
            if (rotation == Quaternion.identity) return bounds;

            var rotatedCenter = rotation * bounds.center;
            var p1 = rotation * bounds.max;
            var p2 = rotation * new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            var p3 = rotation * new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            var p4 = rotation * new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            var p5 = rotation * new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            var p6 = rotation * new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            var p7 = rotation * new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            var p8 = rotation * bounds.min;

            var rotatedBounds = new Bounds(rotatedCenter, Vector3.zero);
            rotatedBounds.Encapsulate(p1);
            rotatedBounds.Encapsulate(p2);
            rotatedBounds.Encapsulate(p3);
            rotatedBounds.Encapsulate(p4);
            rotatedBounds.Encapsulate(p5);
            rotatedBounds.Encapsulate(p6);
            rotatedBounds.Encapsulate(p7);
            rotatedBounds.Encapsulate(p8);
            return rotatedBounds;
        }

        public static Bounds CreateRotatedBounds(Vector3 center, Vector3 size, Quaternion rotation)
        {
            if (rotation == Quaternion.identity) return new Bounds(center, size);
            var bounds = RotateBounds(new Bounds(Vector3.zero, size), rotation);
            bounds.center = center;
            return bounds;
        }

        public static Bounds ScaleBounds(Bounds bounds, Vector3 scale)
        {
            bounds.center = Math.Mul(bounds.center, scale);
            bounds.size = Math.Mul(bounds.size, scale);
            return bounds;
        }

        public static float Align(float childSize, float parentSize, Align align)
        {
            switch (align)
            {
                case global::Flexalon.Align.Start:
                    return -parentSize / 2 + childSize / 2;
                case global::Flexalon.Align.Center:
                    return 0;
                case global::Flexalon.Align.End:
                    return parentSize / 2 - childSize / 2;
                default: return 0;
            }
        }

        public static float Align(Vector3 childSize, Vector3 parentSize, int axis, Align align)
        {
            return Align(childSize[axis], parentSize[axis], align);
        }

        public static float Align(Vector3 childSize, Vector3 parentSize, Axis axis, Align align)
        {
            return Align(childSize, parentSize, (int)axis, align);
        }

        public static Vector3 Align(Vector3 childSize, Vector3 parentSize, Align horizontal, Align vertical, Align depth)
        {
            return new Vector3(Align(childSize, parentSize, 0, horizontal),
                Align(childSize, parentSize, 1, vertical),
                Align(childSize, parentSize, 2, depth));
        }

        public static Bounds MeasureComponentBounds(Bounds componentBounds, FlexalonNode node, Vector3 size)
        {
            componentBounds.size = Vector3.Max(componentBounds.size, Vector3.one * 0.0001f);
            var bounds = componentBounds;

            bool componentX = node.GetSizeType(Axis.X) == SizeType.Component;
            bool componentY = node.GetSizeType(Axis.Y) == SizeType.Component;
            bool componentZ = node.GetSizeType(Axis.Z) == SizeType.Component;

            var scale = Math.Div(size, bounds.size);
            float minScale = (componentX && componentY && componentZ) ? 1 : float.MaxValue;
            if (!componentX)
            {
                minScale = Mathf.Min(minScale, scale.x);
            }

            if (!componentY)
            {
                minScale = Mathf.Min(minScale, scale.y);
            }

            if (!componentZ)
            {
                minScale = Mathf.Min(minScale, scale.z);
            }

            scale = Vector3.one * minScale;

            bounds.size = new Vector3(
                componentX ? bounds.size.x * scale.x : size.x,
                componentY ? bounds.size.y * scale.y : size.y,
                componentZ ? bounds.size.z * scale.z : size.z);

            bounds.center = Math.Mul(bounds.center, Math.Div(bounds.size, componentBounds.size));

            return bounds;
        }

        public static Bounds MeasureComponentBounds2D(Bounds componentBounds, FlexalonNode node, Vector3 size)
        {
            componentBounds.size = Vector3.Max(componentBounds.size, Vector3.one * 0.0001f);
            var bounds = componentBounds;

            bool componentX = node.GetSizeType(Axis.X) == SizeType.Component;
            bool componentY = node.GetSizeType(Axis.Y) == SizeType.Component;
            bool componentZ = node.GetSizeType(Axis.Z) == SizeType.Component;

            var scale = Math.Div(size, bounds.size);
            float minScale = (componentX && componentY) ? 1 : float.MaxValue;
            if (!componentX)
            {
                minScale = Mathf.Min(minScale, scale.x);
            }

            if (!componentY)
            {
                minScale = Mathf.Min(minScale, scale.y);
            }

            scale = Vector3.one * minScale;

            bounds.size = new Vector3(
                componentX ? bounds.size.x * scale.x : size.x,
                componentY ? bounds.size.y * scale.y : size.y,
                componentZ ? 0 : size.z);

            bounds.center = Math.Mul(bounds.center, Math.Div(bounds.size, componentBounds.size));

            return bounds;
        }

        public static Vector3 Abs(Vector3 v)
        {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
            return v;
        }
    }
}