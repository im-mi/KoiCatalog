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
        public IReadOnlyCollection<Color> HairColors { get; private set; } =
            new List<Color>().AsReadOnly();

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
                if (body.shapeValueBody != null && body.shapeValueBody.Length > 0)
                {
                    Height = body.shapeValueBody[0];
                }
                if (body.shapeValueBody != null && body.shapeValueBody.Length > 4)
                {
                    BustSize = body.shapeValueBody[4];
                }
                var skinColor = body.skinMainColor;
                SkinColor = KoikatuColorConversion.KoikatuColorToColor(skinColor);

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
                    // ID 0 is usually reserved for the "none" option. However, hair part 0 has no such option.
                    .Where(i => i.index == 0 || i.item.id != 0)
                    // Skip ahoge slot.
                    .Where(i => i.index != 3)
                    .Select(i => i.item.baseColor)
                    .Select(KoikatuColorConversion.KoikatuColorToColor)
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }
    }
}
