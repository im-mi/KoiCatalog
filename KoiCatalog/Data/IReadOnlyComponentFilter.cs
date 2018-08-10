namespace KoiCatalog.Data
{
    public interface IReadOnlyComponentFilter
    {
        bool this[ComponentTypeCode typeCode] { get; }
    }
}
