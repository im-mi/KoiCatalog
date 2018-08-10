namespace PalettePal
{
    public sealed class NullColorFilter : IColorFilter
    {
        public Vector3 FilterColor(Vector3 rgb)
        {
            return rgb;
        }
    }
}
