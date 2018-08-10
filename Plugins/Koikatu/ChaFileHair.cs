using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileHair
    {
        [MessagePackObject(true)]
        public sealed class PartsInfo
        {
            public int id { get; set; }
            public Color baseColor { get; set; }
            public Color startColor { get; set; }
            public Color endColor { get; set; }
            public float length { get; set; }
            public Vector3 pos { get; set; }
            public Vector3 rot { get; set; }
            public Vector3 scl { get; set; }
            public Color[] acsColor { get; set; }
            public Color outlineColor { get; set; }
        }

        public Version version { get; set; }
        public PartsInfo[] parts { get; set; }
        public int kind { get; set; }
        public int glossId { get; set; }
    }
}
