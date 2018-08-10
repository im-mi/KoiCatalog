namespace KoiCatalog.Data
{
    /// <summary>
    /// A component filter that allows full access to all components.
    /// </summary>
    public sealed class FullAccessComponentFilter : IReadOnlyComponentFilter
    {
        public bool this[ComponentTypeCode typeCode] => true;

        private FullAccessComponentFilter() { }
        public static FullAccessComponentFilter Instance { get; } = new FullAccessComponentFilter();
    }
}
