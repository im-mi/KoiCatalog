using System.IO;
using KoiCatalog.Data;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class FileInfoStatsHandler : StatsHandler
    {
        public static class Categories
        {
            public static readonly StatsCategory FileType = new StatsCategory("File Type", displayIndex: -100);
        }

        public override void GetStats(StatsLoader loader)
        {
            var fileInfo = loader.Entity.GetComponent(FileInfo.TypeCode);
            if (fileInfo == null) return;
            if (fileInfo.File.IsFile)
            {
                var extension = Path.GetExtension(fileInfo.File.AbsoluteUri).ToLowerInvariant();
                loader.Stats.Increment(extension, Categories.FileType);
            }
        }
    }
}
