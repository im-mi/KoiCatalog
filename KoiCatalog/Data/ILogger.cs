namespace KoiCatalog.Data
{
    public interface ILogger
    {
        void Log(DatabaseEvent message);
    }
}
