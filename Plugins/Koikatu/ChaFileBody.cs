using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileBody
    {
        public Version version { get; set; }
        public float[] shapeValueBody { get; set; }
        public float bustSoftness { get; set; }
        public float bustWeight { get; set; }
        public int skinId { get; set; }
        public int detailId { get; set; }
        public float detailPower { get; set; }
        public Color skinMainColor { get; set; }
        public Color skinSubColor { get; set; }
        public float skinGlossPower { get; set; }
        public int[] paintId { get; set; }
        public Color[] paintColor { get; set; }
        public int[] paintLayoutId { get; set; }
        public Vector4[] paintLayout { get; set; }
        public int sunburnId { get; set; }
        public Color sunburnColor { get; set; }
        public int nipId { get; set; }
        public Color nipColor { get; set; }
        public float nipGlossPower { get; set; }
        public float areolaSize { get; set; }
        public int underhairId { get; set; }
        public Color underhairColor { get; set; }
        public Color nailColor { get; set; }
        public float nailGlossPower { get; set; }
        public bool drawAddLine { get; set; }
    }
}
