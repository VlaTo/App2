using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using LibraProgramming.Windows.Interactivity.Extensions;

namespace LibraProgramming.Windows.Interactivity
{
    internal sealed class WorkItemExecutor
    {
        private readonly CoreDispatcher dispatcher;
        private readonly ConcurrentQueue<DispatchedHandler> queue;
        private readonly Task task;
        private readonly CancellationTokenSource cts;
        private TaskCompletionSource<bool> tcs;

        public WorkItemExecutor(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            queue = new ConcurrentQueue<DispatchedHandler>();
            task = Task.Factory.StartNew(DoTaskAsync);
            cts = new CancellationTokenSource();
            tcs = new TaskCompletionSource<bool>();
        }

        public void QueueItem(Action action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            queue.Enqueue(action.Invoke);

            tcs?.TrySetResult(true);
        }

        public void Cancel()
        {
            cts.Cancel();
        }

        private async Task DoTaskAsync()
        {
            var timeout = TimeSpan.FromMilliseconds(50.0d);

            try
            {
                var cancellationToken = cts.Token;

                while (false == cancellationToken.IsCancellationRequested)
                {
                    tcs = new TaskCompletionSource<bool>();

                    await Task.WhenAny(tcs.Task, Task.Delay(timeout), cancellationToken.AsTask());

                    if (queue.TryDequeue(out var action))
                    {
                        await dispatcher
                            .RunAsync(CoreDispatcherPriority.Normal, action)
                            .AsTask(cancellationToken);
                    }
                }
            }
            finally
            {
                ;
            }
        }
    }
}