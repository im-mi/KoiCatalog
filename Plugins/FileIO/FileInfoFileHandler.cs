namespace KoiCatalog.Plugins.FileIO
{
    public sealed class FileInfoFileHandler : FileHandler
    {
        public override void HandleFile(FileLoader loader)
        {
            var fileInfo = loader.Entity.AddComponent(FileInfo.TypeCode);
            fileInfo.Initialize(loader.Source);
        }
    }
}
