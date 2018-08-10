namespace KoiCatalog.Data
{
    public sealed class QueryParameterTemplateCategory
    {
        public string Name { get; }
        public int DisplayIndex { get; }

        public QueryParameterTemplateCategory(string name, int displayIndex = 0)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            DisplayIndex = displayIndex;
        }
    }
}
