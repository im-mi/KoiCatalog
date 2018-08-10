using System;
using System.Collections.Generic;

namespace KoiCatalog.Data
{
    public interface IDatabaseSaveMethod : IDisposable
    {
        void Save(IEnumerable<Entity> items, ILogger logger);
    }
}
