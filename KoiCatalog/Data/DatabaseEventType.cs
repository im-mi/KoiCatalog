namespace KoiCatalog.Data
{
    public enum DatabaseEventType
    {
        Message,

        SaveStart,
        SaveEnd,

        LoadStart,
        ScanStart,
        ScanEnd,
        LoadProgress,
        LoadEnd,
    }
}
