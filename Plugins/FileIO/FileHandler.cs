namespace KoiCatalog.Plugins.FileIO
{
    public abstract class FileHandler
    {
        public virtual void HandleFile(FileLoader loader) { }
        public virtual void FinishHandleFile(FileLoader loader) { }
    }
}
