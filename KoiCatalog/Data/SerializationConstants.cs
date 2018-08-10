using System;

namespace KoiCatalog.Data
{
    static class SerializationConstants
    {
        public static string Header { get; } = "KoiCatalogDB";
        public static Version Version { get; } = new Version(1, 1, 0);
    }
}
