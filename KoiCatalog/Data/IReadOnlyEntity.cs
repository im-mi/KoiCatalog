namespace KoiCatalog.Data
{
    public interface IReadOnlyEntity
    {
        T GetComponent<T>(ComponentTypeCode<T> typeCode) where T : Component, new();
        Component GetComponent(ComponentTypeCode typeCode);
        Component[] GetComponents();
    }
}
