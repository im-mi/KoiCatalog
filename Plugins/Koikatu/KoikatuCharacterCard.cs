using System;
using System.Collections.Generic;
using System.Linq;
using KoiCatalog.Data;
using PalettePal;

namespace KoiCatalog.Plugins.Koikatu
{
    [Serializable]
    public sealed class KoikatuCharacterCard : Component<KoikatuCharacterCard>
    {
        [Display(width: 120)]
        public string FullName => $"{LastName} {FirstName}";
        [Display(visibility: Visibility.Hidden)]
        public string LastName { get; private set; }
        [Display(visibility: Visibility.Hidden)]
        public string FirstName { get; private set; }
        [Display(width: 90)]
        public string Nickname { get; private set; }

        [Display(width: 50)]
        public Sex Sex { get; private set; }
        [Display(width: 110)]
        public Personality Personality { get; private set; }
        [Display(width: 120)]
        public ClubActivity ClubActivity { get; private set; }

        [Display(visibility: Visibility.Hidden, width: 75)]
        public BloodType BloodType { get; private set; }
        [Display(visibility: Visibility.Hidden, width: 70)]
        public TeethType TeethType { get; private set; }

        [Display(visibility: Visibility.Hidden, width: 70)]
        public float Height { get; private set; }
        [Display(visibility: Visibility.Hidden, width: 100)]
        public HeightType HeightType =>
            Height <= 0.33 ? HeightType.Short : Height >= 0.66 ? HeightType.Tall : HeightType.Medium;

        [Display(visibility: Visibility.Hidden, width: 90)]
        public float BustSize { get; private set; }
        [Display(visibility: Visibility.Hidden, width: 115)]
        public BustSizeType BustSizeType =>
            BustSize <= 0.33 ? BustSizeType.Small : BustSize >= 0.66 ? BustSizeType.Large : BustSizeType.Medium;

        [Display(visibility: Visibility.Hidden, width: 85)]
        public SkinType SkinType { get; private set; }
        [Display(visibility: Visibility.Hidden, width: 60)]
        public Color SkinColor { get; private set; }

        [Display(visibility: Visibility.Hidden, width: 85)]
        public HairStyle HairStyle { get; private set; }
        [Display(visibility: Visibility.Hidden, width: 100)]
        public IReadOnlyCollection<Color> HairColors { get; private set; } = Array.Empty<Color>();

        internal void Initialize(ChaFile chaFile)
        {
            if (chaFile == null) throw new ArgumentNullException(nameof(chaFile));

            if (chaFile.parameter != null)
            {
                LastName = chaFile.parameter.lastname ?? string.Empty;
                FirstName = chaFile.parameter.firstname ?? string.Empty;
                Nickname = chaFile.parameter.nickname ?? string.Empty;

                Sex = (Sex)chaFile.parameter.sex;
                BloodType = (BloodType)chaFile.parameter.bloodType;

                Personality = (Personality)chaFile.parameter.personality;
                ClubActivity = (ClubActivity)chaFile.parameter.clubActivities;
            }

            var face = chaFile.custom?.face;
            if (face != null)
            {
                TeethType = face.doubleTooth ? TeethType.Fangs : TeethType.Normal;
            }

            var body = chaFile.custom?.body;
            if (body != null)
            {
                var shapeValueBody = body.shapeValueBody;
                if (shapeValueBody != null)
                {
                    if (shapeValueBody.Length > (int)ChaFileDefine.BodyShapeIdx.Height)
                    {
                        Height = shapeValueBody[(int)ChaFileDefine.BodyShapeIdx.Height];
                    }
                    if (shapeValueBody.Length > (int)ChaFileDefine.BodyShapeIdx.BustSize)
                    {
                        BustSize = shapeValueBody[(int)ChaFileDefine.BodyShapeIdx.BustSize];
                    }
                }

                SkinColor = KoikatuColorConversion.KoikatuColorToColor(body.skinMainColor);
                if (body.detailId >= (int)SkinType.Normal && body.detailId <= (int)SkinType.Slender)
                    SkinType = (SkinType)body.detailId;
                else
                    SkinType = SkinType.Other;
            }

            var hair = chaFile.custom?.hair;
            if (hair != null)
            {
                HairStyle = (HairStyle)hair.kind;
                HairColors = hair.parts
                    .Select((item, index) => new { item, index })
                    // Skip hair slots set to "none" (usually ID 0). Note that back hair has no such option.
                    .Where(i => i.item.id != 0 || i.index == (int)ChaFileDefine.HairKind.back)
                    // Ignore option hair because it doesn't usually contribute much to the overall color.
                    .Where(i => i.index != (int)ChaFileDefine.HairKind.option)
                    .Select(i => i.item.baseColor)
                    .Select(KoikatuColorConversion.KoikatuColorToColor)
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }
    }
}
