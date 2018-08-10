using System;
using System.Linq;
using KoiCatalog.Data;
using KoiCatalog.Util;

namespace KoiCatalog.Plugins
{
    public static class QueryUtil
    {
        public static CompoundQueryHandler OmniQueryHandler { get; } =
            new CompoundQueryHandler(PluginManager.GetNewTypes<QueryHandler>().ToArray());

        public static bool FilterOperatorString(this QueryParameter parameter, string value)
        {
            if (value == null) return true;
            var queryString = parameter.Value as string;
            if (string.IsNullOrWhiteSpace(queryString)) return true;
            var normalizedQuery = StringUtil.NormalizeString(queryString);
            return StringUtil.TestOperatorString(normalizedQuery, value.FuzzyContains);
        }

        public static bool FilterSelection(this QueryParameter parameter, object value)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            return parameter.Selection.Count == 0 || parameter.Selection.Contains(value);
        }
    }
}
