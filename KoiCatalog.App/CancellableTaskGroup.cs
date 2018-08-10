using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoiCatalog.App
{
    public sealed class CancellableTaskGroup
    {
        private List<CancellableTask> Tasks { get; } = new List<CancellableTask>();

        private void FlushCompletedTasks()
        {
            Tasks.RemoveAll(i => i.Task.IsCompleted);
        }

        public void Add(CancellableTask task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            Tasks.Add(task);
            FlushCompletedTasks();
        }

        public bool IsAnyTaskRunning
        {
            get { return Tasks.Any(i => !i.Task.IsCompleted); }
        }

        public void WaitAll()
        {
            Task.WaitAll(Tasks.Select(i => i.Task).ToArray());
        }

        public void CancelAll()
        {
            foreach (var task in Tasks)
            {
                task.CancellationTokenSource.Cancel();
            }
        }
    }
}
