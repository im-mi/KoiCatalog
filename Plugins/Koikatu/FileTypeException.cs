using System;

namespace KoiCatalog.Plugins.Koikatu
{
    public sealed class FileTypeException : Exception
    {
        public FileTypeException(
            string message, Exception innerException = null) : base(message, innerException)
        {
        }
    }
}
