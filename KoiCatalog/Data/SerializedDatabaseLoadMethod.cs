using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KoiCatalog.Data
{
    public sealed class SerializedDatabaseLoadMethod : DatabaseLoadMethod
    {
        public string Path
        {
            get
            {
                CheckDisposed();
                return _path;
            }
            private set
            {
                CheckDisposed();
                _path = value;
            }
        }
        private string _path;

        public SerializedDatabaseLoadMethod(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            Path = path;
        }

        public override bool Scannable => false;

        protected override IEnumerable<Entity> LoadInternal(ILogger logger)
        {
            Entity[] items;

            try
            {
                var serializer = new BinaryFormatter();
                using (var stream = File.OpenRead(Path))
                using (var br = new BinaryReader(stream))
                {
                    var header = Encoding.ASCII.GetString(br.ReadBytes(SerializationConstants.Header.Length));
                    if (header != SerializationConstants.Header)
                        throw new IOException("Unsupported format.");

                    var version = new Version(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
                    if (version < SerializationConstants.Version)
                        throw new IOException("Version is older than supported.");
                    if (version > SerializationConstants.Version)
                        throw new IOException("Version is newer than supported.");

                    var format = (SerializationFormat)br.ReadInt32();
                    if (format != SerializationFormat.Normal)
                        throw new IOException("Unsupported format.");

                    Stream decompressionStream = null;
                    try
                    {
                        var compressionMethod = (CompressionMethod)br.ReadInt32();
                        switch (compressionMethod)
                        {
                            case CompressionMethod.Uncompressed:
                                decompressionStream = stream;
                                break;
                            case CompressionMethod.GZip:
                                decompressionStream = new GZipStream(stream, CompressionMode.Decompress);
                                break;
                            default:
                                throw new IOException("Unsupported compression format.");
                        }

                        items = (Entity[])serializer.Deserialize(decompressionStream);
                        foreach (var item in items)
                            item.LocalizeTypeCodes();
                    }
                    finally
                    {
                        decompressionStream?.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                logger.Log(new DatabaseEvent(DatabaseEventType.Message,
                    message: "The database could not be loaded. Try rebuilding.",
                    severity: EventSeverity.Error));
                items = Array.Empty<Entity>();
            }

            return items;
        }
    }
}
