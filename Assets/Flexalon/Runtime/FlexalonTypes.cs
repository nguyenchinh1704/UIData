using UnityEngine;

namespace Flexalon
{
    public enum Direction
    {
        PositiveX = 0,
        NegativeX = 1,
        PositiveY = 2,
        NegativeY = 3,
        PositiveZ = 4,
        NegativeZ = 5
    };

    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    };

    public enum Align
    {
        Start = 0,
        Center = 1,
        End = 2
    };

    public enum SizeType
    {
        Fixed = 0,
        Fill = 1,
        Component = 2,
        Layout = 3
    };

    [System.Serializable]
    public struct Directions
    {
        public static Directions zero => new Directions(new float[]{ 0, 0, 0, 0, 0, 0 });

        public float[] Values;

        public Directions(float[] values)
        {
            Values = values;
        }

        public float this[int key]
        {
            get => Values[key];
            set => Values[key] = value;
        }

        public float this[Direction key]
        {
            get => Values[(int)key];
            set => Values[(int)key] = value;
        }

        public Vector3 Size => new Vector3(
            Values[0] + Values[1], Values[2] + Values[3], Values[4] + Values[5]);

        public Vector3 Center => new Vector3(
            (Values[0] - Values[1]) * 0.5f, (Values[2] - Values[3]) * 0.5f, (Values[4] - Values[5]) * 0.5f);
    }
}