using System;

namespace PalettePal
{
    public struct ColorSample
    {
        public Color Color { get; set; }
        public bool Polarity { get; set; }

        public ColorSample(Color color, bool polarity)
        {
            Color = color;
            Polarity = polarity;
        }

        public override string ToString()
        {
            return $"{(Polarity ? string.Empty : "-")}{Color}";
        }

        public static ColorSample Parse(string source)
        {
            if (!TryParse(source, out var sample))
                throw new FormatException();
            return sample;
        }

        public static bool TryParse(string source, out ColorSample sample)
        {
            var result = new ColorSample
            {
                Polarity = true
            };

            if (source.StartsWith("-"))
            {
                result.Polarity = false;
                source = source.Substring(1);
            }

            if (!Color.TryParse(source, out var color))
            {
                sample = default(ColorSample);
                return false;
            }

            result.Color = color;
            sample = result;
            return true;
        }
    }
}
