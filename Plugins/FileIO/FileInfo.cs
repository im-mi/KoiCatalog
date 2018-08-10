using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using KoiCatalog.Data;
using KoiCatalog.Util;

namespace KoiCatalog.Plugins.FileIO
{
    [Serializable]
    public sealed class FileInfo : Component<FileInfo>
    {
        [Display(order: -1, width: 62, minWidth: 45)]
        public Uri File { get; private set; }
        [Display(order: 1000)]
        public string FileName => Path.GetFileName(File?.LocalPath);
        [Display(order: 1000, width: 120)]
        public DateTime ModifiedTime { get; private set; }
        /// <summary>
        /// The MD5 of the file in lowercase hexadecimal.
        /// </summary>
        [Display("MD5", order: 1000, visibility: Visibility.Hidden)]
        public string HashMD5 { get; private set; }
        /// <summary>
        /// The SHA-1 of the <see cref="MD5"/> (ASCII-encoded) in lowercase hexadecimal.
        /// </summary>
        [Display("SHA-1 of MD5", order: 1000, visibility: Visibility.Hidden)]
        public string HashSHA1OfMD5
        {
            get
            {
                if (HashMD5 == null) return null;
                using (var sha1 = SHA1.Create())
                    return StringUtil.BytesToString(sha1.ComputeHash(Encoding.ASCII.GetBytes(HashMD5)));
            }
        }

        public Stream OpenRead()
        {
            using (var webClient = new WebClient())
            {
                return webClient.OpenRead(File);
            }
        }

        internal void Initialize(Uri source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            File = source;
            ModifiedTime = System.IO.File.GetLastWriteTimeUtc(source.LocalPath);

            using (var stream = OpenRead())
            using (var md5 = MD5.Create())
                HashMD5 = StringUtil.BytesToString(md5.ComputeHash(stream));
        }
    }
}
