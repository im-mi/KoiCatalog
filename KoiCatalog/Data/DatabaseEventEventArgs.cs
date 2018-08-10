using System;

namespace KoiCatalog.Data
{
    public sealed class DatabaseEventEventArgs : EventArgs
    {
        public DatabaseEvent Event { get; }

        public DatabaseEventEventArgs(DatabaseEvent e)
        {
            Event = e ?? throw new ArgumentNullException(nameof(e));
        }
    }
}
