using System;
using LibraProgramming.Windows.Interactivity;

namespace LibraProgramming.Windows.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class InteractionRequest : IInteractionRequest
    {
        private readonly WeakEventHandler<InteractionRequestedEventArgs> raised;

        public event EventHandler<InteractionRequestedEventArgs> Raised
        {
            add => raised.AddHandler(value);
            remove => raised.RemoveHandler(value);
        }

        public InteractionRequest()
        {
            raised = new WeakEventHandler<InteractionRequestedEventArgs>();
        }

        protected void DoRaiseEvent(InteractionRequestedEventArgs e)
        {

            raised.Invoke(this, e);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class InteractionRequest<TContext> : InteractionRequest
        where TContext : InteractionRequestContext
    {
        public void Raise(TContext context, Action callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            DoRaiseEvent(new InteractionRequestedEventArgs(context, callback));
        }

        public void Raise(TContext context)
        {
            DoRaiseEvent(new InteractionRequestedEventArgs(context, null));
        }
    }
}