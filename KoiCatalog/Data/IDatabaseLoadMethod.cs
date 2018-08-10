using System;
using System.Collections.Generic;
using System.Threading;

namespace KoiCatalog.Data
{
    public interface IDatabaseLoadMethod : IDisposable
    {
        bool Scannable { get; }
        int ItemCount { get; }
        void Scan(List<Entity> currentItems, ILogger logger, CancellationToken cancellationToken);
        IEnumerable<Entity> Load(ILogger logger);
    }
}
