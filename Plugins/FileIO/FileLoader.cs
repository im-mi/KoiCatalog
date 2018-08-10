using System;
using KoiCatalog.Data;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class FileLoader
    {
        public Uri Source { get; internal set; }
        public Entity Entity { get; internal set; }
        public ILogger Logger { get; internal set; }

        /// <summary>
        /// Logs a generic file load error.
        /// </summary>
        /// <param name="value"></param>
        public void LogError(object value)
        {
            Logger?.Log(new DatabaseEvent(
                DatabaseEventType.Message, message: $"File loader error: {value}",
                severity: EventSeverity.Error, uri: Source));
        }

        internal FileLoader() { }
    }
}
