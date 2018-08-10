using System;

namespace PalettePal
{
    public static class MathEx
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException();
            return Math.Min(Math.Max(value, min), max);
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException();
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
