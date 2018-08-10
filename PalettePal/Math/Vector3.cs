using System;

namespace PalettePal
{
    public struct Vector3
    {
        public static Vector3 Zero { get; } = new Vector3(0);
        public static Vector3 One { get; } = new Vector3(1);

        public int Length => 3;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(float x)
        {
            X = x;
            Y = x;
            Z = x;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public static Vector3 operator +(Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v0.X + v1.X,
                v0.Y + v1.Y,
                v0.Z + v1.Z
            );
        }

        public static Vector3 operator -(Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v0.X - v1.X,
                v0.Y - v1.Y,
                v0.Z - v1.Z
            );
        }

        public static Vector3 operator *(Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v0.X * v1.X,
                v0.Y * v1.Y,
                v0.Z * v1.Z
            );
        }

        public static Vector3 operator /(Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v0.X / v1.X,
                v0.Y / v1.Y,
                v0.Z / v1.Z
            );
        }

        public static Vector3 operator +(Vector3 v0, float x)
        {
            return new Vector3(
                v0.X + x,
                v0.Y + x,
                v0.Z + x
            );
        }

        public static Vector3 operator -(Vector3 v0, float x)
        {
            return new Vector3(
                v0.X - x,
                v0.Y - x,
                v0.Z - x
            );
        }

        public static Vector3 operator *(Vector3 v0, float x)
        {
            return new Vector3(
                v0.X * x,
                v0.Y * x,
                v0.Z * x
            );
        }

        public static Vector3 operator /(Vector3 v0, float x)
        {
            return new Vector3(
                v0.X / x,
                v0.Y / x,
                v0.Z / x
            );
        }

        public static Vector3 operator +(Vector3 v0, Vector3i v1) =>
            v0 + (Vector3)v1;

        public static Vector3 operator -(Vector3 v0, Vector3i v1) =>
            v0 - (Vector3)v1;

        public static Vector3 operator *(Vector3 v0, Vector3i v1) =>
            v0 * (Vector3)v1;

        public static Vector3 operator /(Vector3 v0, Vector3i v1) =>
            v0 / (Vector3)v1;

        public static explicit operator Vector3(Vector3i other)
        {
            return new Vector3(other.X, other.Y, other.Z);
        }

        public static Vector3 Lerp(Vector3 v0, Vector3 v1, float n)
        {
            return v0 * (1 - n) + v1 * n;
        }

        public float Sum()
        {
            return X + Y + Z;
        }

        public float Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }

        public float Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }
    }
}
