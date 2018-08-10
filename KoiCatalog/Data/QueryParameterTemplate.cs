using System;

namespace KoiCatalog.Data
{
    public sealed class QueryParameterTemplate
    {
        public string Name { get; }
        public Type Type { get; }
        public QueryParameterTemplateCategory Category { get; }
        public int DisplayIndex { get; }
        public bool IsValueSelectable { get; }

        public QueryParameterTemplate(string name, Type type, QueryParameterTemplateCategory category = null,
            bool isValueSelectable = false, int displayIndex = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Category = category;
            DisplayIndex = displayIndex;
            IsValueSelectable = isValueSelectable;
        }
    }
}
