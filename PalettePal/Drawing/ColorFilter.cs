namespace PalettePal
{
    public static class ColorFilter
    {
        /// <summary>
        /// Saturate a color in the HSV color space.
        /// </summary>
        /// <param name="rgb"></param>
        /// <param name="strength"></param>
        /// <returns></returns>
        public static Vector3 SaturateHsv(Vector3 rgb, float strength)
        {
            var hsv = ColorConversion.RgbToHsv(rgb);
            hsv[1] = MathEx.Clamp(hsv[1] + hsv[1] * strength, 0, 1);
            return ColorConversion.HsvToRgb(hsv);
        }
    }
}
