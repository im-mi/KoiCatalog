using System.Linq;

namespace KoiCatalog.Plugins.FileIO
{
    public static class FileIOUtil
    {
        public static CompoundFileHandler OmniFileHandler { get; } =
            new CompoundFileHandler(PluginManager.GetNewTypes<FileHandler>().ToArray());
        public static CompoundDefaultDirectoryHandler OmniDefaultDirectoryHandler { get; } =
            new CompoundDefaultDirectoryHandler(PluginManager.GetNewTypes<DefaultDirectoryHandler>().ToArray());
    }
}
