using System.Collections.Concurrent;

namespace Brainary.Commons.Util
{
    /// <summary>
    /// Background tasks queue control.
    /// </summary>
    public class BackgroundQueueWorker
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new();
        private readonly SemaphoreSlim signal = new(0);

        public async Task<Func<CancellationToken, Task>?> Dequeue(CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            workItems.TryDequeue(out var workItem);

            return workItem;
        }

        public void Enqueue(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            workItems.Enqueue(workItem);
            signal.Release();
        }
    }
}
