using System;
using KoiCatalog.Util;

namespace KoiCatalog.App
{
    public sealed class ActionLockTarget
    {
        private int LockDepth
        {
            get => _lockDepth;
            set
            {
                if (value < 0)
                    throw new InvalidOperationException();
                _lockDepth = value;
            }
        }
        private int _lockDepth;

        public bool IsLocked => LockDepth != 0;

        public ActionLockHandle AcquireLock()
        {
            return new ActionLockHandle(this);
        }

        public sealed class ActionLockHandle : Disposable
        {
            private ActionLockTarget Owner { get; }

            internal ActionLockHandle(ActionLockTarget owner)
            {
                if (owner == null) throw new ArgumentNullException(nameof(owner));
                Owner = owner;
                Owner.PushLock();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Owner.PopLock();
                }
                else
                {
                    throw new InvalidOperationException("Undisposed object detected.");
                }

                base.Dispose(disposing);
            }
        }

        private void PushLock() => LockDepth++;
        private void PopLock() => LockDepth--;
    }
}
