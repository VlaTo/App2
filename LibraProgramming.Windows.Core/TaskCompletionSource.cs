using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraProgramming.Windows.Core
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskCompletionSource
    {
        private readonly TaskCompletionSource<object> tcs;

        /// <summary>
        /// 
        /// </summary>
        public Task Task => tcs.Task;

        /// <summary>
        /// 
        /// </summary>
        public TaskCompletionSource()
        {
            tcs = new TaskCompletionSource<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public TaskCompletionSource(object state)
        {
            tcs = new TaskCompletionSource<object>(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creationOptions"></param>
        public TaskCompletionSource(TaskCreationOptions creationOptions)
        {
            tcs = new TaskCompletionSource<object>(creationOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="creationOptions"></param>
        public TaskCompletionSource(object state, TaskCreationOptions creationOptions)
        {
            tcs = new TaskCompletionSource<object>(state, creationOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            tcs.SetCanceled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryCancel()
        {
            return tcs.TrySetCanceled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void Fail(Exception exception)
        {
            tcs.SetException(exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptions"></param>
        public void Fail(IEnumerable<Exception> exceptions)
        {
            tcs.SetException(exceptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public bool TryFail(IEnumerable<Exception> exceptions)
        {
            return tcs.TrySetException(exceptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TryFail(Exception exception)
        {
            return tcs.TrySetException(exception);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Complete()
        {
            tcs.SetResult(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryComplete()
        {
            return tcs.TrySetResult(null);
        }
    }
}