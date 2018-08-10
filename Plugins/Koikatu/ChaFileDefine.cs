namespace KoiCatalog.Plugins.Koikatu
{
    public static class ChaFileDefine
    {
	    public enum BodyShapeIdx
	    {
		    Height,
		    HeadSize,
		    NeckW,
		    NeckZ,
		    BustSize,
		    BustY,
		    BustRotX,
		    BustX,
		    BustRotY,
		    BustSharp,
		    BustForm,
		    AreolaBulge,
		    NipWeight,
		    NipStand,
		    BodyShoulderW,
		    BodyShoulderZ,
		    BodyUpW,
		    BodyUpZ,
		    BodyLowW,
		    BodyLowZ,
		    WaistY,
		    Belly,
		    WaistUpW,
		    WaistUpZ,
		    WaistLowW,
		    WaistLowZ,
		    Hip,
		    HipRotX,
		    ThighUpW,
		    ThighUpZ,
		    ThighLowW,
		    ThighLowZ,
		    KneeLowW,
		    KneeLowZ,
		    Calf,
		    AnkleW,
		    AnkleZ,
		    ShoulderW,
		    ShoulderZ,
		    ArmUpW,
		    ArmUpZ,
		    ElbowW,
		    ElbowZ,
		    ArmLow
	    }

	    public enum FaceShapeIdx
	    {
		    FaceBaseW,
		    FaceUpZ,
		    FaceUpY,
		    FaceUpSize,
		    FaceLowZ,
		    FaceLowW,
		    ChinLowY,
		    ChinLowZ,
		    ChinY,
		    ChinW,
		    ChinZ,
		    ChinTipY,
		    ChinTipZ,
		    ChinTipW,
		    CheekBoneW,
		    CheekBoneZ,
		    CheekW,
		    CheekZ,
		    CheekY,
		    EyebrowY,
		    EyebrowX,
		    EyebrowRotZ,
		    EyebrowInForm,
		    EyebrowOutForm,
		    EyelidsUpForm1,
		    EyelidsUpForm2,
		    EyelidsUpForm3,
		    EyelidsLowForm1,
		    EyelidsLowForm2,
		    EyelidsLowForm3,
		    EyeY,
		    EyeX,
		    EyeZ,
		    EyeTilt,
		    EyeH,
		    EyeW,
		    EyeInX,
		    EyeOutY,
		    NoseTipH,
		    NoseY,
		    NoseBridgeH,
		    MouthY,
		    MouthW,
		    MouthZ,
		    MouthUpForm,
		    MouthLowForm,
		    MouthCornerForm,
		    EarSize,
		    EarRotY,
		    EarRotZ,
		    EarUpForm,
		    EarLowForm
	    }

	    public enum HairKind
	    {
		    back,
		    front,
		    side,
		    option
	    }

	    public enum ClothesKind
	    {
		    top,
		    bot,
		    bra,
		    shorts,
		    gloves,
		    panst,
		    socks,
		    shoes_inner,
		    shoes_outer
	    }

	    public enum ClothesSubKind
	    {
		    partsA,
		    partsB,
		    partsC
	    }

	    public enum CoordinateType
	    {
		    School01,
		    School02,
		    Gym,
		    Swim,
		    Club,
		    Plain,
		    Pajamas
	    }

	    public enum SiruParts
	    {
		    SiruKao,
		    SiruFrontUp,
		    SiruFrontDown,
		    SiruBackUp,
		    SiruBackDown
	    }

	    public const string CharaFileMark = "【KoiKatuChara】";

	    public const string ClothesFileMark = "【KoiKatuClothes】";

	    public const string CharaFileFemaleDir = "chara/female/";

	    public const string CharaFileMaleDir = "chara/male/";

	    public const string CoordinateFileDir = "coordinate/";
    }
}
