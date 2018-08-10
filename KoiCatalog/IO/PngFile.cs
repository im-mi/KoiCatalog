using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KoiCatalog.IO
{
    public static class PngFile
    {
        private static readonly IReadOnlyList<byte> PngHeader = new List<byte>
        {
            137, 80, 78, 71, 13, 10, 26, 10,
        }.AsReadOnly();

        private static class ChunkType
        {
            public const int ChunkTypeSize = 4;
            public static readonly IReadOnlyList<byte> IEND =
                Encoding.ASCII.GetBytes("IEND").ToList().AsReadOnly();
        }

        public static void SkipPng(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                var header = reader.ReadBytes(PngHeader.Count);
                if (!header.SequenceEqual(PngHeader))
                {
                    throw new IOException("Not a valid PNG file.");
                }
                
                while (true)
                {
                    var length = reader.ReadUInt32BigEndian();
                    var chunkType = reader.ReadBytes(ChunkType.ChunkTypeSize);
                    reader.BaseStream.Position += length;
                    var crc = reader.ReadUInt32BigEndian();
                    
                    if (chunkType.SequenceEqual(ChunkType.IEND))
                        break;
                }
            }
        }
    }
}
