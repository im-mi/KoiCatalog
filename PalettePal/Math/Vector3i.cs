using System;

namespace PalettePal
{
    public struct Vector3i
    {
        public int Length => 3;

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int this[int index]
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

        public static int GetDistanceSquared(Vector3i v0, Vector3i v1)
        {
            var d = v1 - v0;
            d *= d;
            return d.Sum();
        }

        public static Vector3i operator +(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(
                v0.X + v1.X,
                v0.Y + v1.Y,
                v0.Z + v1.Z
            );
        }

        public static Vector3i operator -(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(
                v0.X - v1.X,
                v0.Y - v1.Y,
                v0.Z - v1.Z
            );
        }

        public static Vector3i operator *(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(
                v0.X * v1.X,
                v0.Y * v1.Y,
                v0.Z * v1.Z
            );
        }

        public static Vector3i operator /(Vector3i v0, Vector3i v1)
        {
            return new Vector3i(
                v0.X / v1.X,
                v0.Y / v1.Y,
                v0.Z / v1.Z
            );
        }

        public static Vector3i operator +(Vector3i v0, int x)
        {
            return new Vector3i(
                v0.X + x,
                v0.Y + x,
                v0.Z + x
            );
        }

        public static Vector3i operator -(Vector3i v0, int x)
        {
            return new Vector3i(
                v0.X - x,
                v0.Y - x,
                v0.Z - x
            );
        }

        public static Vector3i operator *(Vector3i v0, int x)
        {
            return new Vector3i(
                v0.X * x,
                v0.Y * x,
                v0.Z * x
            );
        }

        public static Vector3i operator /(Vector3i v0, int x)
        {
            return new Vector3i(
                v0.X / x,
                v0.Y / x,
                v0.Z / x
            );
        }

        public static bool operator ==(Vector3i v0, Vector3i v1)
        {
            return v0.X == v1.X && v0.Y == v1.Y && v0.Z == v1.Z;
        }

        public static bool operator !=(Vector3i v0, Vector3i v1)
        {
            return !(v0 == v1);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Vector3i vector3i)
                return Equals(vector3i);
            return base.Equals(obj);
        }

        public bool Equals(Vector3i other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Z;
                return hashCode;
            }
        }

        public static explicit operator Vector3i(Vector3 src)
        {
            return new Vector3i((int)src.X, (int)src.Y, (int)src.Z);
        }

        public int Sum()
        {
            return X + Y + Z;
        }

        public int Min()
        {
            return Math.Min(Math.Min(X, Y), Z);
        }

        public int Max()
        {
            return Math.Max(Math.Max(X, Y), Z);
        }
    }
}
