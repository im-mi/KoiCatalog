using System;
using System.IO;

namespace KoiCatalog.IO
{
    static class BinaryReaderExtension
    {
        public static uint ReadUInt32BigEndian(this BinaryReader self)
        {
            var data = self.ReadBytes(sizeof(uint));
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }
    }
}
