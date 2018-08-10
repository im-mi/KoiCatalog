using System.Reflection;
using PalettePal;

namespace KoiCatalog.Plugins.Koikatu
{
    public static class ColorFilters
    {
        public static IColorFilter Normal =>
            LoadColorGradingColorFilter("ColorFilters/Normal.png");

        private static ColorGradingColorFilter LoadColorGradingColorFilter(
            string path, Assembly assembly = null)
        {
            return Resources.Load(
                path, assembly ?? Assembly.GetCallingAssembly(),
                stream => new ColorGradingColorFilter { Texture = new Texture3D(stream) },
                () => new ColorGradingColorFilter());
        }
    }
}
