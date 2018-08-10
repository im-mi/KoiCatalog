using System;
using System.Threading;
using System.Threading.Tasks;

namespace KoiCatalog.App
{
    public sealed class CancellableTask
    {
        public Task Task { get; }
        public CancellationTokenSource CancellationTokenSource { get; }

        public CancellableTask(Task task, CancellationTokenSource cancellationTokenSource)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            Task = task;
            CancellationTokenSource = cancellationTokenSource;
        }
    }
}
