using PalettePal;

namespace KoiCatalog.Plugins.Koikatu
{
    public static class KoikatuColorConversion
    {
        public static Vector3 KoikatuColorToRgb(Vector3 koikatuColor)
        {
            return ColorFilters.Normal.FilterColor(koikatuColor);
        }

        public static Color KoikatuColorToColor(Vector3 koikatuColor)
        {
            var rgb = KoikatuColorToRgb(koikatuColor);
            return ColorConversion.RgbToColor(rgb);
        }

        internal static Color KoikatuColorToColor(UnityEngine.Color koikatuColor)
        {
            var koikatuColor2 = UnityColorToRgb(koikatuColor);
            return KoikatuColorToColor(koikatuColor2);
        }

        internal static Vector3 UnityColorToRgb(UnityEngine.Color unityColor)
        {
            return new Vector3(unityColor.r, unityColor.g, unityColor.b);
        }
    }
}
