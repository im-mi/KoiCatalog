using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileAccessory
    {
        [MessagePackObject(true)]
        public sealed class PartsInfo
        {
            public int type { get; set; }
            public int id { get; set; }
            public string parentKey { get; set; }
            public Vector3[,] addMove { get; set; }
            public Color[] color { get; set; }
            public int hideCategory { get; set; }
            [IgnoreMember]
            public bool partsOfHead { get; set; }
        }

        public Version version { get; set; }
        public PartsInfo[] parts { get; set; }
    }
}
