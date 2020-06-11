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

        public WorkItemExecutor(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            queue = new ConcurrentQueue<DispatchedHandler>();
            //timer = ThreadPoolTimer.CreatePeriodicTimer(DoTimer, TimeSpan.FromMilliseconds(50));
            task = Task.Factory.StartNew(DoTaskAsync);
            //semaphore = new CountdownEvent(0);
            cts = new CancellationTokenSource();
        }

        public void QueueItem(Action action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            queue.Enqueue(action.Invoke);

            //semaphore.AddCount();
            //semaphore.Signal();
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
                var spinWait = new SpinWait();

                while (false == cts.Token.IsCancellationRequested)
                {
                    /*var hasSignal = semaphore.Wait(timeout, cts.Token);

                    if (false == hasSignal)
                    {
                        continue;
                    }*/

                    if (false == queue.TryDequeue(out var action))
                    {
                        spinWait.SpinOnce();
                        continue;
                    }

                    await dispatcher
                        .RunAsync(CoreDispatcherPriority.Normal, action)
                        .AsTask();
                }
            }
            finally
            {
                ;
            }
        }

        /*private async void DoTimer(ThreadPoolTimer _)
        {
            if (false == queue.TryDequeue(out var action))
            {
                return;
            }

            await dispatcher
                .RunAsync(CoreDispatcherPriority.Normal, action)
                .AsTask();
        }*/
    }
}