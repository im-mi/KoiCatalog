using System;

namespace PalettePal
{
    public static class ColorConversion
    {
        /// <summary>
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns>
        /// h = [0,360], s = [0,1], v = [0,1]
        /// if s == 0, then h = 0 (undefined)
        /// </returns>
        public static Vector3 RgbToHsv(Vector3 rgb)
        {
            // Based on https://www.cs.rit.edu/~ncs/color/t_convert.html

            var hsv = new Vector3();

            var min = rgb.Min();
            var max = rgb.Max();
            hsv[2] = max;

            var delta = max - min;

            if (max != 0)
            {
                hsv[1] = delta / max;
            }
            else
            {
                // r = g = b = 0
                // s = 0, v is undefined
                hsv[0] = 0;
                hsv[1] = 0;
                return hsv;
            }

            if (rgb[0] == max)
                hsv[0] = (rgb[1] - rgb[2]) / delta; // Between yellow and magenta
            else if (rgb[1] == max)
                hsv[0] = 2 + (rgb[2] - rgb[0]) / delta; // Between cyan and yellow
            else
                hsv[0] = 4 + (rgb[0] - rgb[1]) / delta; // Between magenta and cyan

            hsv[0] *= 60;
            if (hsv[0] < 0)
                hsv[0] += 360;

            return hsv;
        }

        public static Vector3 HsvToRgb(Vector3 hsv)
        {
            // Based on https://www.cs.rit.edu/~ncs/color/t_convert.html

            if (hsv[1] == 0)
            {
                // achromatic (grey)
                return new Vector3(hsv[2]);
            }

            hsv[0] /= 60; // sector 0 to 5
            var i = (int)Math.Floor(hsv[0]);
            var f = hsv[0] - i;
            var p = hsv[2] * (1 - hsv[1]);
            var q = hsv[2] * (1 - hsv[1] * f);
            var t = hsv[2] * (1 - hsv[1] * (1 - f));

            switch (i)
            {
                case 0: return new Vector3(hsv[2], t, p);
                case 1: return new Vector3(q, hsv[2], p);
                case 2: return new Vector3(p, hsv[2], t);
                case 3: return new Vector3(p, q, hsv[2]);
                case 4: return new Vector3(t, p, hsv[2]);
                default:
                case 5: return new Vector3(hsv[2], p, q);
            }
        }

        public static Vector3 RgbToXyz(Vector3 rgb)
        {
            // Based on http://www.brucelindbloom.com
            // Using sRGB (D65)
            
            for (var i = 0; i < rgb.Length; i++)
            {
                if (rgb[i] <= 0.04045f)
                    rgb[i] /= 12;
                else
                    rgb[i] = (float)Math.Pow((rgb[i] + 0.055f) / 1.055f, 2.4f);
            }

            return new Vector3(
                (rgb * new Vector3(0.436052025f, 0.385081593f, 0.143087414f)).Sum(),
                (rgb * new Vector3(0.222491598f, 0.716886060f, 0.060621486f)).Sum(),
                (rgb * new Vector3(0.013929122f, 0.097097002f, 0.714185470f)).Sum()
            );
        }

        public static Vector3i XyzToLab(Vector3 xyz)
        {
            // Based on http://www.brucelindbloom.com

            const float episilon = 216f / 24389f;
            const float k = 24389f / 27f;

            // Reference white (D50)
            var white = new Vector3(0.964221f, 1, 0.825211f);

            var p = xyz / white;
            var f = new Vector3();

            for (var i = 0; i < f.Length; i++)
            {
                if (p[i] > episilon)
                    f[i] = (float)Math.Pow(p[i], 1f / 3f);
                else
                    f[i] = (k * p[i] + 16.0f) / 116.0f;
            }

            var ls = 116 * f.Y - 16;
            var fas = 500 * (f.X - f.Y);
            var fbs = 200 * (f.Y - f.Z);
            var result = new Vector3(2.55f * ls, fas, fbs) + 0.5f;

            return (Vector3i)result;
        }

        public static Vector3 ColorToRgb(Color color)
        {
            return new Vector3(color.R, color.G, color.B) / byte.MaxValue;
        }

        public static Color RgbToColor(Vector3 rgb)
        {
            return Color.FromArgb(
                0xFF,
                (byte)MathEx.Clamp((int)(rgb[0] * byte.MaxValue), byte.MinValue, byte.MaxValue),
                (byte)MathEx.Clamp((int)(rgb[1] * byte.MaxValue), byte.MinValue, byte.MaxValue),
                (byte)MathEx.Clamp((int)(rgb[2] * byte.MaxValue), byte.MinValue, byte.MaxValue)
            );
        }

        /// <summary>
        /// Converts RGB to a luminance value using a typical luminance equation.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static float RgbToLum(Vector3 rgb)
        {
            // https://www.w3.org/TR/WCAG20/#relativeluminancedef
            for (var i = 0; i < rgb.Length; i++)
                rgb[i] = rgb[i] <= .03928f ? rgb[i] / 12.92f : (float)Math.Pow((rgb[i] + .055f) / 1.055f, 2.4f);
            var lumFactor = new Vector3(.2126f, .7152f, .0722f);
            return (rgb * lumFactor).Sum();
        }
    }
}
