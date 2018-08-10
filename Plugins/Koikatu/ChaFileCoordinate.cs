using System;
using System.IO;
using MessagePack;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class ChaFileCoordinate
    {
        public static readonly string BlockName = "Coordinate";

        public int loadProductNo;
        public Version loadVersion;
        public ChaFileClothes clothes;
        public ChaFileAccessory accessory;

        public bool enableMakeup;
        public ChaFileMakeup makeup;

        public string coordinateName = string.Empty;
        public byte[] pngData;
        public string coordinateFileName { get; private set; }

        public void LoadBytes(byte[] data)
        {
            using (var input = new MemoryStream(data))
            {
                using (var binaryReader = new BinaryReader(input))
                {
                    int count = binaryReader.ReadInt32();
                    byte[] bytes = binaryReader.ReadBytes(count);
                    clothes = MessagePackSerializer.Deserialize<ChaFileClothes>(bytes, KoikatuConstants.MessagePackResolver);
                    count = binaryReader.ReadInt32();
                    bytes = binaryReader.ReadBytes(count);
                    accessory = MessagePackSerializer.Deserialize<ChaFileAccessory>(bytes, KoikatuConstants.MessagePackResolver);
                    enableMakeup = binaryReader.ReadBoolean();
                    count = binaryReader.ReadInt32();
                    bytes = binaryReader.ReadBytes(count);
                    makeup = MessagePackSerializer.Deserialize<ChaFileMakeup>(bytes, KoikatuConstants.MessagePackResolver);
                }
            }
        }
    }
}
