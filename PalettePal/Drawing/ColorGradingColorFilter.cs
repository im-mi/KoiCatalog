namespace PalettePal
{
    public sealed class ColorGradingColorFilter : IColorFilter
    {
        public Texture3D Texture { get; set; }

        public Vector3 FilterColor(Vector3 rgb)
        {
            if (Texture == null) return rgb;
            return Texture.SampleLinear(rgb);
        }
    }
}
