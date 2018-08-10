using System;

namespace KoiCatalog.Util
{
    public abstract class Disposable : IDisposable
    {
        protected void CheckDisposed()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        protected bool Disposed { get; private set; }
        
        public void Dispose()
        {
            CheckDisposed();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
        
        ~Disposable()
        {
            Dispose(false);
        }
    }
}
