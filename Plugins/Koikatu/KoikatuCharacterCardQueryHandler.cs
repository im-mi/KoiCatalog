using System.Linq;
using KoiCatalog.Data;
using KoiCatalog.Drawing;
using KoiCatalog.Util;
using PalettePal;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class KoikatuCharacterCardQueryHandler : QueryHandler
    {
        public static class Categories
        {
            public static readonly QueryParameterTemplateCategory Character =
                new QueryParameterTemplateCategory("Character", displayIndex: 0);
            public static readonly QueryParameterTemplateCategory Face =
                new QueryParameterTemplateCategory("Face", displayIndex: 1);
            public static readonly QueryParameterTemplateCategory Body =
                new QueryParameterTemplateCategory("Body", displayIndex: 2);
            public static readonly QueryParameterTemplateCategory Hair =
                new QueryParameterTemplateCategory("Hair", displayIndex: 3);
        }

        public static class Parameters
        {
            public static readonly QueryParameterTemplate Name =
                new QueryParameterTemplate("Name", typeof(string), Categories.Character, displayIndex: 0);
            public static readonly QueryParameterTemplate Sex =
                new QueryParameterTemplate("Sex", typeof(Sex), Categories.Character, isValueSelectable: true, displayIndex: 1);
            public static readonly QueryParameterTemplate Personality =
                new QueryParameterTemplate("Personality", typeof(Personality), Categories.Character, isValueSelectable: true, displayIndex: 2);
            public static readonly QueryParameterTemplate ClubActivities =
                new QueryParameterTemplate("Club Activity", typeof(ClubActivity), Categories.Character, isValueSelectable: true, displayIndex: 3);
            public static readonly QueryParameterTemplate BloodType =
                new QueryParameterTemplate("Blood Type", typeof(BloodType), Categories.Character, isValueSelectable: true, displayIndex: 4);
            
            public static readonly QueryParameterTemplate TeethType =
                new QueryParameterTemplate("Teeth Type", typeof(TeethType), Categories.Face, isValueSelectable: true, displayIndex: 0);
            
            public static readonly QueryParameterTemplate HeightType =
                new QueryParameterTemplate("Height Type", typeof(HeightType), Categories.Body, isValueSelectable: true, displayIndex: 0);
            public static readonly QueryParameterTemplate BustSizeType =
                new QueryParameterTemplate("Bust Size Type", typeof(BustSizeType), Categories.Body, isValueSelectable: true, displayIndex: 1);
            public static readonly QueryParameterTemplate SkinType =
                new QueryParameterTemplate("Skin Type", typeof(SkinType), Categories.Body, isValueSelectable: true, displayIndex: 2);
            public static readonly QueryParameterTemplate SkinColorType =
                new QueryParameterTemplate("Skin Color Type", typeof(Adjective), Categories.Body, isValueSelectable: true, displayIndex: 3);
            public static readonly QueryParameterTemplate InverseSkinColorType =
                new QueryParameterTemplate("Inverse Skin Color Type", typeof(Adjective), Categories.Body, isValueSelectable: true, displayIndex: 4);
            
            public static readonly QueryParameterTemplate HairStyle =
                new QueryParameterTemplate("Hair Style", typeof(HairStyle), Categories.Hair, isValueSelectable: true, displayIndex: 0);
            public static readonly QueryParameterTemplate HairColorType =
                new QueryParameterTemplate("Hair Color Type", typeof(Adjective), Categories.Hair, isValueSelectable: true, displayIndex: 1);
            public static readonly QueryParameterTemplate InverseHairColorType =
                new QueryParameterTemplate("Inverse Hair Color Type", typeof(Adjective), Categories.Hair, isValueSelectable: true, displayIndex: 2);
        }

        public override void GetQueryFormat(IReadOnlyEntity entity, QueryFormat format)
        {
            var card = entity.GetComponent(KoikatuCharacterCard.TypeCode);
            if (card == null) return;

            format.AddParameter(Parameters.Name);
            format.AddSelectableValue(Parameters.Sex, card.Sex);
            format.AddSelectableValue(Parameters.Personality, card.Personality);
            format.AddSelectableValue(Parameters.ClubActivities, card.ClubActivity);
            format.AddSelectableValue(Parameters.BloodType, card.BloodType);

            format.AddSelectableValue(Parameters.TeethType, card.TeethType);

            format.AddSelectableValue(Parameters.HeightType, card.HeightType);
            format.AddSelectableValue(Parameters.BustSizeType, card.BustSizeType);
            format.AddSelectableValue(Parameters.SkinType, card.SkinType);
            foreach (var adjective in Palettes.Skin.GetAdjectives(card.SkinColor))
            {
                format.AddSelectableValue(Parameters.SkinColorType, adjective);
#if DEBUG
                format.AddSelectableValue(Parameters.InverseSkinColorType, adjective);
#endif
            }

            format.AddSelectableValue(Parameters.HairStyle, card.HairStyle);
            foreach (var adjective in card.HairColors.Select(i => Palettes.Hair.GetAdjectives(i)).SelectMany(i => i))
            {
                format.AddSelectableValue(Parameters.HairColorType, adjective);
#if DEBUG
                format.AddSelectableValue(Parameters.InverseHairColorType, adjective);
#endif
            }
        }

        public override bool Filter(IReadOnlyEntity entity, QueryParameter param)
        {
            var card = entity.GetComponent(KoikatuCharacterCard.TypeCode);
            if (card == null) return true;

            if (param.Template == Parameters.Name)
            {
                var value = param.Value as string;
                if (string.IsNullOrWhiteSpace(value)) return true;

                var normalizedValue = StringUtil.NormalizeString(value);
                var lastName = StringUtil.NormalizeString(card.LastName);
                var firstName = StringUtil.NormalizeString(card.FirstName);
                var nickname = StringUtil.NormalizeString(card.Nickname);

                return StringUtil.TestOperatorString(normalizedValue, query =>
                    nickname.FuzzyContains(query) ||
                    $"{lastName}{firstName}".FuzzyContains(query) ||
                    $"{lastName} {firstName}".FuzzyContains(query) ||
                    $"{firstName}{lastName}".FuzzyContains(query) ||
                    $"{firstName} {lastName}".FuzzyContains(query));
            }
            else if (param.Template == Parameters.Sex)
                return param.FilterSelection(card.Sex);
            else if (param.Template == Parameters.Personality)
                return param.FilterSelection(card.Personality);
            else if (param.Template == Parameters.ClubActivities)
                return param.FilterSelection(card.ClubActivity);
            else if (param.Template == Parameters.BloodType)
                return param.FilterSelection(card.BloodType);
            else if (param.Template == Parameters.TeethType)
                return param.FilterSelection(card.TeethType);
            else if (param.Template == Parameters.HeightType)
                return param.FilterSelection(card.HeightType);
            else if (param.Template == Parameters.BustSizeType)
                return param.FilterSelection(card.BustSizeType);
            else if (param.Template == Parameters.HairStyle)
                return param.FilterSelection(card.HairStyle);
            else if (param.Template == Parameters.SkinType)
                return param.FilterSelection(card.SkinType);
            else if (param.Template == Parameters.SkinColorType)
                return Palettes.Skin.GetAdjectives(card.SkinColor).Any(param.FilterSelection);
            else if (param.Template == Parameters.HairColorType)
            {
                return card.HairColors
                    .Select(i => Palettes.Hair.GetAdjectives(i))
                    .SelectMany(i => i)
                    .Any(param.FilterSelection);
            }
            else if (param.Template == Parameters.InverseSkinColorType)
            {
                return param.Selection.Count == 0 ||
                       !Palettes.Skin.GetAdjectives(card.SkinColor).Any(param.FilterSelection);
            }
            else if (param.Template == Parameters.InverseHairColorType)
            {
                return param.Selection.Count == 0 ||
                       !card.HairColors
                           .Select(i => Palettes.Hair.GetAdjectives(i))
                           .SelectMany(i => i)
                           .Any(param.FilterSelection);
            }
            else
            {
                return true;
            }
        }
    }
}
