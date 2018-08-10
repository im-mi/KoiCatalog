using System;
using System.Globalization;

namespace PalettePal
{
    [Serializable]
    public struct Color : IComparable<Color>, IComparable
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static Color FromUint(uint value)
        {
            return new Color
            {
                A = (byte)(value >> 24),
                R = (byte)(value >> 16),
                G = (byte)(value >> 8),
                B = (byte)(value >> 0),
            };
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color
            {
                A = a,
                R = r,
                G = g,
                B = b,
            };
        }

        public override bool Equals(object obj)
        {
            var color = obj as Color?;
            if (color == null) return false;
            return Equals(color.Value);
        }

        public static bool TryParse(string str, out Color color)
        {
            if (str == null)
            {
                color = default(Color);
                return false;
            }

            str = str.Trim();
            if (str.Length == 0)
            {
                color = default(Color);
                return false;
            }

            // Todo: Normalize the string first to avoid having to account for character width, etc..
            if (str.StartsWith("#") || str.StartsWith("＃"))
            {
                str = str.Substring(1);
            }
            else
            {
                color = default(Color);
                return false;
            }

            if (!uint.TryParse(str, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var colorCode))
            {
                color = default(Color);
                return false;
            }

            // If no alpha was specified, then assume FF.
            if (str.Length <= 6)
                colorCode |= 0xFF000000;

            color = FromUint(colorCode);
            return true;
        }

        public static Color Parse(string str)
        {
            if (!TryParse(str, out var color))
                throw new FormatException();
            return color;
        }

        public bool Equals(Color other)
        {
            return A == other.A && R == other.R && G == other.G && B == other.B;
        }

        public static bool operator ==(Color c1, Color c2)
        {
            return Equals(c1, c2);
        }

        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1 == c2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = A.GetHashCode();
                hashCode = (hashCode * 397) ^ R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                return hashCode;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is Color otherColor)
            {
                return CompareTo(otherColor);
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            var a = A == 0xFF ? string.Empty : $"{A:X2}";
            return $"#{a}{R:X2}{G:X2}{B:X2}";
        }

        private enum ColorCategory
        {
            Grays,
            Red,
            Orange,
            Yellow,
            Green,
            Cyan,
            Blue,
            Purple,
            Pink,
        }

        private ColorCategory GetColorCategory()
        {
            var rgb = ColorConversion.ColorToRgb(this);
            var hsv = ColorConversion.RgbToHsv(rgb);

            if (hsv[1] < 0.12f)
                return ColorCategory.Grays;

            float lumCutoff;
            if (hsv[1] > 0.9)
                lumCutoff = 0.1f;
            else if (hsv[1] > 0.8)
                lumCutoff = 0.13f;
            else if (hsv[1] > 0.6)
                lumCutoff = 0.19f;
            else if (hsv[1] > 0.4)
                lumCutoff = 0.22f;
            else
                lumCutoff = 0.28f;

            var lum = Math.Sqrt(ColorConversion.RgbToLum(rgb));
            if (lum < lumCutoff)
                return ColorCategory.Grays;

            if (hsv[0] <= 26)
                return ColorCategory.Red;
            if (hsv[0] <= 44)
                return ColorCategory.Orange;
            if (hsv[0] <= 69)
                return ColorCategory.Yellow;
            if (hsv[0] <= 152)
                return ColorCategory.Green;
            if (hsv[0] <= 184)
                return ColorCategory.Cyan;
            if (hsv[0] <= 257)
                return ColorCategory.Blue;
            if (hsv[0] <= 296)
                return ColorCategory.Purple;
            if (hsv[0] <= 330)
                return ColorCategory.Pink;
            return ColorCategory.Red;
        }

        public int CompareTo(Color other)
        {
            int comparison;

            var thisCategory = GetColorCategory();

            comparison = ((int)thisCategory).CompareTo((int)other.GetColorCategory());
            if (comparison != 0) return comparison;

            comparison = ColorConversion.RgbToLum(ColorConversion.ColorToRgb(this))
                .CompareTo(ColorConversion.RgbToLum(ColorConversion.ColorToRgb(other)));
            if ((int)thisCategory % 2 == 0)
                comparison = -comparison;
            if (comparison != 0) return comparison;

            return comparison;
        }
    }
}
