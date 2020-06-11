using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LibraProgramming.Windows.Interactivity
{
    internal sealed class WorkItemExecutor
    {
        private readonly CoreDispatcher dispatcher;
        private readonly ConcurrentQueue<DispatchedHandler> queue;
        //private readonly ThreadPoolTimer timer;
        private readonly Task task;
        //private readonly CountdownEvent semaphore;
        private readonly CancellationTokenSource cts;
        private readonly PulseAwaiter semaphore;

        public WorkItemExecutor(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            queue = new ConcurrentQueue<DispatchedHandler>();
            //timer = ThreadPoolTimer.CreatePeriodicTimer(DoTimer, TimeSpan.FromMilliseconds(50));
            task = Task.Factory.StartNew(DoTaskAsync);
            //semaphore = new CountdownEvent(0);
            semaphore = new PulseAwaiter();
            cts = new CancellationTokenSource();
        }

        public void QueueItem(Action action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            queue.Enqueue(action.Invoke);

            semaphore.Pulse();
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
                while (false == cts.Token.IsCancellationRequested)
                {
                    await semaphore.WaitAsync();

                    if (queue.TryDequeue(out var action))
                    {
                        await dispatcher
                            .RunAsync(CoreDispatcherPriority.Normal, action)
                            .AsTask();
                    }
                }
            }
            finally
            {
                ;
            }
        }

        private sealed class PulseAwaiter
        {
            private readonly ConcurrentQueue<TaskCompletionSource<bool>> waiters;

            public PulseAwaiter()
            {
                waiters = new ConcurrentQueue<TaskCompletionSource<bool>>();
            }

            public void Pulse()
            {
                if (waiters.TryDequeue(out var tcs))
                {
                    tcs.SetResult(true);
                }
            }

            public Task WaitAsync()
            {
                if (false == waiters.TryPeek(out var tcs))
                {
                    tcs = new TaskCompletionSource<bool>();

                    waiters.Enqueue(tcs);
                }

                return tcs.Task;
            }
        }
    }
}