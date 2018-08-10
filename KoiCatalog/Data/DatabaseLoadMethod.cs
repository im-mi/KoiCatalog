using System;
using System.Collections.Generic;
using System.Threading;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    public abstract class DatabaseLoadMethod : Disposable, IDatabaseLoadMethod
    {
        protected bool Scanned { get; private set; }
        public abstract bool Scannable { get; }

        public int ItemCount
        {
            get
            {
                CheckDisposed();
                EnsureScanned();
                return _itemCount;
            }
            private set => _itemCount = value;
        }
        private int _itemCount;

        public void Scan(List<Entity> currentItems, ILogger logger, CancellationToken cancellationToken)
        {
            CheckDisposed();
            EnsureNotScanned();
            if (!Scannable)
                throw new NotSupportedException();
            Scanned = true;
            ItemCount = ScanInternal(currentItems, logger, cancellationToken);
        }

        public IEnumerable<Entity> Load(ILogger logger)
        {
            CheckDisposed();
            EnsureScanned();
            foreach (var item in LoadInternal(logger))
            {
                yield return item;
                CheckDisposed();
            }
        }

        protected virtual int ScanInternal(List<Entity> currentItems, ILogger logger, CancellationToken cancellationToken) => 0;
        protected abstract IEnumerable<Entity> LoadInternal(ILogger logger);

        protected void EnsureScanned()
        {
            if (Scanned || !Scannable) return;
            throw new InvalidOperationException("Scan has not been called.");
        }

        protected void EnsureNotScanned()
        {
            if (!Scanned) return;
            throw new InvalidOperationException("Scan has already been called.");
        }
    }
}
