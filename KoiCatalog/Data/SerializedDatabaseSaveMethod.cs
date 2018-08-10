using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    public sealed class SerializedDatabaseSaveMethod : Disposable, IDatabaseSaveMethod
    {
        public string Path { get; }

        public SerializedDatabaseSaveMethod(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            Path = path;
        }

        public void Save(IEnumerable<Entity> items, ILogger logger)
        {
            try
            {
                CheckDisposed();

                var directory = System.IO.Path.GetDirectoryName(Path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var serializer = new BinaryFormatter();
                using (var stream = File.Open(Path, FileMode.Create))
                using (var bw = new BinaryWriter(stream))
                {
                    bw.Write(Encoding.ASCII.GetBytes(SerializationConstants.Header));
                    bw.Write(SerializationConstants.Version.Major);
                    bw.Write(SerializationConstants.Version.Minor);
                    bw.Write(SerializationConstants.Version.Build);
                    bw.Write((int)SerializationFormat.Normal);
                    bw.Write((int)CompressionMethod.GZip);

                    using (var gzipStream = new GZipStream(stream, CompressionLevel.Optimal, true))
                        serializer.Serialize(gzipStream, items.ToArray());
                }
            }
            catch (Exception ex)
            {
                logger.Log(new DatabaseEvent(DatabaseEventType.Message,
                    message: $"The database could not be saved ({ex.Message}).",
                    severity: EventSeverity.Error));
            }
        }
    }
}
