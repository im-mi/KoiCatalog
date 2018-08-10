using System;
using MessagePack;
using UnityEngine;

namespace KoiCatalog.Plugins.Koikatu
{
    [MessagePackObject(true)]
    public sealed class ChaFileStatus
    {
        [IgnoreMember]
        public static readonly string BlockName = "Status";

        public Version version { get; set; }

        public int coordinateType { get; set; }
        public int backCoordinateType { get; set; }

        public byte[] clothesState { get; set; }
        public byte shoesType { get; set; }
        public bool[] showAccessory { get; set; }

        public int eyebrowPtn { get; set; }
        public float eyebrowOpenMax { get; set; }

        public int eyesPtn { get; set; }
        public float eyesOpenMax { get; set; }
        public bool eyesBlink { get; set; }
        public bool eyesYure { get; set; }

        public int mouthPtn { get; set; }
        public float mouthOpenMax { get; set; }
        public bool mouthFixed { get; set; }
        public bool mouthAdjustWidth { get; set; }

        public byte tongueState { get; set; }
        public int eyesLookPtn { get; set; }
        public int eyesTargetType { get; set; }
        public float eyesTargetAngle { get; set; }
        public float eyesTargetRange { get; set; }
        public float eyesTargetRate { get; set; }

        public int neckLookPtn { get; set; }
        public int neckTargetType { get; set; }
        public float neckTargetAngle { get; set; }
        public float neckTargetRange { get; set; }
        public float neckTargetRate { get; set; }

        public bool disableMouthShapeMask { get; set; }
        public bool[,] disableBustShapeMask { get; set; }
        public float nipStandRate { get; set; }
        public float skinTuyaRate { get; set; }
        public float hohoAkaRate { get; set; }
        public byte tearsLv { get; set; }
        public bool hideEyesHighlight { get; set; }
        public byte[] siruLv { get; set; }

        public bool visibleSon { get; set; }
        public bool visibleSonAlways { get; set; }
        public bool visibleHeadAlways { get; set; }
        public bool visibleBodyAlways { get; set; }
        public bool visibleSimple { get; set; }
        public bool visibleGomu { get; set; }

        public Color simpleColor { get; set; }
    }
}
