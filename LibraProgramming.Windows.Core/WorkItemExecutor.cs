using LibraProgramming.Windows.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LibraProgramming.Windows.Core
{
    public sealed class WorkItemExecutor
    {
        private readonly CoreDispatcher dispatcher;
        private readonly ConcurrentQueue<DispatchedHandler> queue;
        private readonly Task task;
        private readonly CancellationTokenSource cts;
        private TaskCompletionSource pulse;

        public WorkItemExecutor(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;

            queue = new ConcurrentQueue<DispatchedHandler>();
            cts = new CancellationTokenSource();
            task = Task.Factory.StartNew(DoTaskAsync);
        }

        public void QueueItem(Action action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            queue.Enqueue(action.Invoke);

            pulse?.TryComplete();
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
                    var success = queue.TryDequeue(out var action);

                    if (success)
                    {
                        await dispatcher
                            .RunAsync(CoreDispatcherPriority.Normal, action)
                            .AsTask(cancellationToken);
                        continue;
                    }

                    pulse = new TaskCompletionSource();

                    await Task.WhenAny(pulse.Task, Task.Delay(timeout), cancellationToken.AsTask());
                }
            }
            finally
            {
                ;
            }
        }
    }
}