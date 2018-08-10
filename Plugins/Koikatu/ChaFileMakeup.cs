using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileMakeup
    {
        public Version version { get; set; }
        public int eyeshadowId { get; set; }
        public Color eyeshadowColor { get; set; }
        public int cheekId { get; set; }
        public Color cheekColor { get; set; }
        public int lipId { get; set; }
        public Color lipColor { get; set; }
        public int[] paintId { get; set; }
        public Color[] paintColor { get; set; }
        public Vector4[] paintLayout { get; set; }
    }
}
