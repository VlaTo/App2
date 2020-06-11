using System;
using System.Collections.Concurrent;
using Windows.System.Threading;
using Windows.UI.Core;

namespace LibraProgramming.Windows.Interactivity
{
    internal sealed class WorkItemExecutor
    {
        private readonly CoreDispatcher dispatcher;
        private readonly ConcurrentQueue<DispatchedHandler> queue;
        private readonly ThreadPoolTimer timer;

        public WorkItemExecutor(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            queue = new ConcurrentQueue<DispatchedHandler>();
            timer = ThreadPoolTimer.CreatePeriodicTimer(DoTimer, TimeSpan.FromMilliseconds(50));
        }

        public void QueueItem(Action action)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            queue.Enqueue(action.Invoke);
        }

        private async void DoTimer(ThreadPoolTimer _)
        {
            if (false == queue.TryDequeue(out var action))
            {
                return;
            }

            await dispatcher
                .RunAsync(CoreDispatcherPriority.Normal, action)
                .AsTask();
        }
    }
}