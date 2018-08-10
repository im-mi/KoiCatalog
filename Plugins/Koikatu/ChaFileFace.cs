using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileFace
    {
        [MessagePackObject(true)]
        public sealed class PupilInfo
        {
            public int id { get; set; }
            public Color baseColor { get; set; }
            public Color subColor { get; set; }
            public int gradMaskId { get; set; }
            public float gradBlend { get; set; }
            public float gradOffsetY { get; set; }
            public float gradScale { get; set; }
        }

        public Version version { get; set; }
        public float[] shapeValueFace { get; set; }
        public int headId { get; set; }
        public int skinId { get; set; }
        public int detailId { get; set; }
        public float detailPower { get; set; }
        public float cheekGlossPower { get; set; }
        public int eyebrowId { get; set; }
        public Color eyebrowColor { get; set; }
        public int noseId { get; set; }
        public PupilInfo[] pupil { get; set; }
        public int hlUpId { get; set; }
        public Color hlUpColor { get; set; }
        public int hlDownId { get; set; }
        public Color hlDownColor { get; set; }
        public int whiteId { get; set; }
        public Color whiteBaseColor { get; set; }
        public Color whiteSubColor { get; set; }
        public float pupilWidth { get; set; }
        public float pupilHeight { get; set; }
        public float pupilX { get; set; }
        public float pupilY { get; set; }
        public float hlUpY { get; set; }
        public float hlDownY { get; set; }
        public int eyelineUpId { get; set; }
        public float eyelineUpWeight { get; set; }
        public int eyelineDownId { get; set; }
        public Color eyelineColor { get; set; }
        public int moleId { get; set; }
        public Color moleColor { get; set; }
        public Vector4 moleLayout { get; set; }
        public int lipLineId { get; set; }
        public Color lipLineColor { get; set; }
        public float lipGlossPower { get; set; }
        public bool doubleTooth { get; set; }
        public ChaFileMakeup baseMakeup { get; set; }
        public byte foregroundEyes { get; set; }
        public byte foregroundEyebrow { get; set; }
    }
}