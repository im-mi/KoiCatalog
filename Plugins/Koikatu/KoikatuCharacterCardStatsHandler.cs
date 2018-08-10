using KoiCatalog.Data;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class KoikatuCharacterCardStatsHandler : StatsHandler
    {
        public static class Categories
        {
            public static readonly StatsCategory Sex =
                new StatsCategory("Sex", displayIndex: 1000);
            public static readonly StatsCategory Personality =
                new StatsCategory("Personality", displayIndex: 1001);
            public static readonly StatsCategory ClubActivity =
                new StatsCategory("Club Activity", displayIndex: 1002);
            public static readonly StatsCategory BloodType =
                new StatsCategory("Blood Type", displayIndex: 1003);
            public static readonly StatsCategory HeightType =
                new StatsCategory("Height Type", displayIndex: 1004);
            public static readonly StatsCategory BustSizeType =
                new StatsCategory("Bust Size Type", displayIndex: 1005);
            public static readonly StatsCategory HairStyle =
                new StatsCategory("Hair Style", displayIndex: 1006);
        }

        public override void GetStats(StatsLoader loader)
        {
            var card = loader.Entity.GetComponent(KoikatuCharacterCard.TypeCode);
            if (card == null) return;
            loader.Stats.Increment(card.Sex, Categories.Sex);
            loader.Stats.Increment(card.Personality, Categories.Personality);
            loader.Stats.Increment(card.ClubActivity, Categories.ClubActivity);
            loader.Stats.Increment(card.BloodType, Categories.BloodType);
            loader.Stats.Increment(card.HeightType, Categories.HeightType);
            loader.Stats.Increment(card.BustSizeType, Categories.BustSizeType);
            loader.Stats.Increment(card.HairStyle, Categories.HairStyle);
        }
    }
}
