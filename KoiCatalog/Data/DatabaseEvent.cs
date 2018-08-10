using System;

namespace KoiCatalog.Data
{
    public sealed class DatabaseEvent
    {
        public DatabaseEventType EventType { get; }
        public EventSeverity Severity { get; }
        public string Message { get; }
        public Progress Progress { get; }
        public Uri Uri { get; }

        public DatabaseEvent(
            DatabaseEventType eventType,
            Progress progress = null,
            string message = null,
            EventSeverity severity = EventSeverity.Message,
            Uri uri = null)
        {
            EventType = eventType;
            Severity = severity;
            Progress = progress;
            Message = message ?? (string)Convert.ChangeType(eventType, typeof(string));
            Uri = uri;
        }
    }
}
