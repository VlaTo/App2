using System;

namespace LibraProgramming.Windows.Interactivity
{
    public sealed class DisposableToken : IDisposable
    {
        private readonly Action disposer;
        private bool disposed;

        public DisposableToken(Action disposer)
        {
            if (null == disposer)
            {
                throw new ArgumentNullException(nameof(disposer));
            }

            this.disposer = disposer;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                if (dispose)
                {
                    disposer.Invoke();
                }
            }
            finally
            {
                disposed = true;
            }
        }
    }
}