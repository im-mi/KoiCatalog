using System.Linq;

namespace KoiCatalog.Plugins.FileIO
{
    public static class FileIOUtil
    {
        public static CompoundFileHandler CreateOmniFileHandler() =>
            new CompoundFileHandler(PluginManager.GetNewTypes<FileHandler>().ToArray());
        public static CompoundDefaultDirectoryHandler CreateOmniDefaultDirectoryHandler() =>
            new CompoundDefaultDirectoryHandler(PluginManager.GetNewTypes<DefaultDirectoryHandler>().ToArray());
    }
}
