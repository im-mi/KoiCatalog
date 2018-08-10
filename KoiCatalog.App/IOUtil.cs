using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace KoiCatalog.App
{
    public static class IOUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void OpenFileFolder(Uri uri)
        {
            Process.Start("explorer", $"/select,\"{uri}\"");
        }

        public static void OpenUri(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            Process.Start(uri.ToString());
        }
    }
}
