using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace KoiCatalog.Util
{
    /// <summary>
    /// A thread-safe collection that can only be added to. Enumeration over this
    /// collection will block until the next element is available or until
    /// <see cref="CompleteAdding"/> has been called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class ConcurrentList<T> : IReadOnlyList<T>
    {
        private List<T> BackingCollection { get; } = new List<T>();
        public bool IsAddingCompleted
        {
            get => _isAddingCompleted;
            private set => _isAddingCompleted = value;
        }
        private volatile bool _isAddingCompleted;

        public int Count => _count;
        private int _count;

        public T this[int index]
        {
            get
            {
                lock (BackingCollection)
                {
                    return BackingCollection[index];
                }
            }
        }

        public void Add(T item)
        {
            lock (BackingCollection)
            {
                BackingCollection.Add(item);
            }
            Interlocked.Increment(ref _count);
        }

        public void CompleteAdding()
        {
            IsAddingCompleted = true;
        }

        public IEnumerator<T> GetEnumerator(CancellationToken cancellationToken)
        {
            return new Enumerator(this, cancellationToken);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : Disposable, IEnumerator<T>
        {
            public T Current { get; private set; }
            object IEnumerator.Current => Current;
            
            private ConcurrentList<T> Owner { get; }
            private CancellationToken CancellationToken { get; }
            private int i;

            public Enumerator(ConcurrentList<T> owner, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (owner == null) throw new ArgumentNullException(nameof(owner));
                Owner = owner;
                CancellationToken = cancellationToken;
            }

            public bool MoveNext()
            {
                CheckDisposed();
                
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        return false;
                    }

                    var itemsRemaining = Owner.Count - i;
                    if (itemsRemaining > 0)
                    {
                        Current = Owner[i++];
                        return true;
                    }

                    if (Owner.IsAddingCompleted)
                    {
                        return false;
                    }

                    // Todo: Don't use polling.
                    try
                    {
                        Thread.Sleep(1);
                    }
                    catch (ThreadInterruptedException)
                    {
                    }
                }
            }

            public void Reset()
            {
                i = 0;
            }
        }
    }
}
