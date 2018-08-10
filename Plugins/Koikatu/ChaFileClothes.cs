using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileClothes
    {
        [MessagePackObject(true)]
        public class PartsInfo
        {
            [MessagePackObject(true)]
            public class ColorInfo
            {
                public Color baseColor { get; set; }
                public int pattern { get; set; }
                public Vector2 tiling { get; set; }
                public Color patternColor { get; set; }
            }

            public int id { get; set; }
            public ColorInfo[] colorInfo { get; set; }
            public int emblemeId { get; set; }
        }

        public Version version { get; set; }
        public PartsInfo[] parts { get; set; }
        public int[] subPartsId { get; set; }
        public bool[] hideBraOpt { get; set; }
        public bool[] hideShortsOpt { get; set; }
    }
}
