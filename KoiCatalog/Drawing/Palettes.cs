using System.Reflection;
using PalettePal;

namespace KoiCatalog.Drawing
{
    public static class Palettes
    {
        public static ConcurrentCachingPalette Hair =>
            LoadConcurrentCachingPalette("Palettes/Hair.pal");
        public static ConcurrentCachingPalette Skin =>
            LoadConcurrentCachingPalette("Palettes/Skin.pal");

        private static ConcurrentCachingPalette LoadConcurrentCachingPalette(
            string path, Assembly assembly = null)
        {
            return Resources.Load(
                path, assembly ?? Assembly.GetCallingAssembly(),
                stream => new ConcurrentCachingPalette(Palette.Load(stream)),
                () => new ConcurrentCachingPalette(new Palette()));
        }
    }
}
