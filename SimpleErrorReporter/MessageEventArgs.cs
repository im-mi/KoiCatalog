using System;

namespace SimpleErrorReporter
{
    public sealed class MessageEventArgs
    {
        public string Message { get; }

        public MessageEventArgs(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            Message = message;
        }
    }
}
