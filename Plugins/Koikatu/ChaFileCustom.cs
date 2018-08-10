using System;
using System.IO;
using MessagePack;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class ChaFileCustom
    {
        public const string BlockName = "Custom";

        public ChaFileFace face;
        public ChaFileBody body;
        public ChaFileHair hair;
        
        public void LoadBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var input = new MemoryStream(data))
            {
                using (var binaryReader = new BinaryReader(input))
                {
                    int count = binaryReader.ReadInt32();
                    byte[] bytes = binaryReader.ReadBytes(count);
                    face = MessagePackSerializer.Deserialize<ChaFileFace>(bytes, KoikatuConstants.MessagePackResolver);
                    count = binaryReader.ReadInt32();
                    bytes = binaryReader.ReadBytes(count);
                    body = MessagePackSerializer.Deserialize<ChaFileBody>(bytes, KoikatuConstants.MessagePackResolver);
                    count = binaryReader.ReadInt32();
                    bytes = binaryReader.ReadBytes(count);
                    hair = MessagePackSerializer.Deserialize<ChaFileHair>(bytes, KoikatuConstants.MessagePackResolver);
                }
            }
        }
    }
}
