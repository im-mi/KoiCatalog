using System.Collections.Generic;
using MessagePack;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class BlockHeader
    {
        [MessagePackObject(true)]
        public class Info
        {
            public string name { get; set; } = string.Empty;
            public string version { get; set; } = string.Empty;
            public long pos { get; set; }
            public long size { get; set; }
        }

        public List<Info> lstInfo { get; set; }

        public BlockHeader()
        {
            lstInfo = new List<Info>();
        }

        public Info SearchInfo(string name) => lstInfo.Find(n => n.name == name);
    }
}
