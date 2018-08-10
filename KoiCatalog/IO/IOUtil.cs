using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using KoiCatalog.Util;

namespace KoiCatalog.IO
{
    public static class IOUtil
    {
        public static string NormalizePath(string path)
        {
            path = Path.GetFullPath(path);
            path = path.Replace('/', '\\');
            path = path.TrimEnd('\\');
            return path;
        }

        public static string PathToHash(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            path = NormalizePath(path);
            string key;
            using (var md5 = MD5.Create())
                key = StringUtil.BytesToString(md5.ComputeHash(Encoding.UTF8.GetBytes(path)));
            return key;
        }
    }
}
