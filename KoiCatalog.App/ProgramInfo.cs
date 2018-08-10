using System;
using System.Diagnostics;
using System.Reflection;

namespace KoiCatalog.App
{
    public static class ProgramInfo
    {
        private static FileVersionInfo FileVersionInfo =>
            FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        public static string ProductName => FileVersionInfo.ProductName;

        /// <summary>
        /// The product name, including decorators (ex. [Debug] for debug builds).
        /// </summary>
        public static string ProductFullName
        {
            get
            {
                var fullProductName = ProductName;
#if DEBUG
                fullProductName += " [Debug]";
#endif
                return fullProductName;
            }
        }
        public static string ProductVersion => FileVersionInfo.ProductVersion;

        public static string Author => FileVersionInfo.CompanyName;
        public static Uri HomePage => new Uri(@"https://github.com/im-mi/KoiCatalog");
    }
}
