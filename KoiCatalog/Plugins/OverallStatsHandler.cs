using KoiCatalog.Data;
using KoiCatalog.Util;

namespace KoiCatalog.Plugins
{
    public sealed class OverallStatsHandler : StatsHandler
    {
        public static class Categories
        {
            public static readonly StatsCategory Overall = new StatsCategory("Overall", displayIndex: -1000);
            public static readonly StatsCategory DataType = new StatsCategory("Data Type", displayIndex: -999);
        }

        public override void GetStats(StatsLoader loader)
        {
            loader.Stats.Increment("Total Database Item Count", Categories.Overall);

            foreach (var component in loader.Entity.GetComponents())
            {
                loader.Stats.Increment(StringUtil.FormatCamelcase(component.GetType().Name), Categories.DataType);
            }
        }
    }
}
