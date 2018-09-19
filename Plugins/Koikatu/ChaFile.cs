using System;
using System.Collections.Generic;
using System.IO;
using KoiCatalog.IO;
using MessagePack;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class ChaFile
    {
        public int loadProductNo { get; set; }
        public Version loadVersion { get; set; }

        public ChaFileCustom custom { get; set; }
        public ChaFileParameter parameter { get; set; }
        public ChaFileStatus status { get; set; }
        public ChaFileCoordinate[] coordinate { get; set; }

        public ChaFile() { }

        public ChaFile(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                try
                {
                    PngFile.SkipPng(stream);

                    try
                    {
                        br.ReadByte();
                        stream.Position -= sizeof(byte);
                    }
                    catch (EndOfStreamException)
                    {
                        throw new IOException("File is just a plain image with no card data.");
                    }

                    loadProductNo = br.ReadInt32();
                    var title = br.ReadString();
                    if (title != ChaFileDefine.CharaFileMark)
                        throw new IOException("Not a character card.");
                }
                catch (Exception ex)
                {
                    throw new FileTypeException("File is not a Koikatu character card or the file type could not be determined.", ex);
                }

                var version = new Version(br.ReadString());
                var facePngDataLength = br.ReadInt32();
                stream.Position += facePngDataLength;
                var blockHeader = MessagePackSerializer.Deserialize<BlockHeader>(br.ReadBytes(br.ReadInt32()), KoikatuConstants.MessagePackResolver);
                var blocksLength = br.ReadInt64();
                var blocksPosition = stream.Position;
                
                var customBlockInfo = blockHeader.SearchInfo(ChaFileCustom.BlockName);
                if (customBlockInfo != null)
                {
                    stream.Position = blocksPosition + customBlockInfo.pos;
                    var data = br.ReadBytes((int)customBlockInfo.size);
                    custom = new ChaFileCustom();
                    custom.LoadBytes(data);
                }

                var coordinateBlockInfo = blockHeader.SearchInfo(ChaFileCoordinate.BlockName);
                if (coordinateBlockInfo != null)
                {
                    stream.Position = blocksPosition + coordinateBlockInfo.pos;
                    var data = br.ReadBytes((int)coordinateBlockInfo.size);
                    var coordinateBytesCollection = MessagePackSerializer.Deserialize<List<byte[]>>(data, KoikatuConstants.MessagePackResolver);
                    var coordinates = new List<ChaFileCoordinate>();
                    foreach (var coordinateBytes in coordinateBytesCollection)
                    {
                        var coordinate = new ChaFileCoordinate();
                        coordinate.LoadBytes(coordinateBytes);
                        coordinates.Add(coordinate);
                    }
                    coordinate = coordinates.ToArray();
                }

                var parameterBlockInfo = blockHeader.SearchInfo(ChaFileParameter.BlockName);
                if (parameterBlockInfo != null)
                {
                    stream.Position = blocksPosition + parameterBlockInfo.pos;
                    var data = br.ReadBytes((int)parameterBlockInfo.size);
                    parameter = MessagePackSerializer.Deserialize<ChaFileParameter>(data, KoikatuConstants.MessagePackResolver);
                }
                
                var statusBlockInfo = blockHeader.SearchInfo(ChaFileStatus.BlockName);
                if (statusBlockInfo != null)
                {
                    stream.Position = blocksPosition + statusBlockInfo.pos;
                    var data = br.ReadBytes((int)statusBlockInfo.size);
                    status = MessagePackSerializer.Deserialize<ChaFileStatus>(data, KoikatuConstants.MessagePackResolver);
                }
            }
        }
    }
}
