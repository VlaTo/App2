using System;

namespace LibraProgramming.Windows.Interaction
{
    public class InteractionRequestedEventArgs : EventArgs
    {
        public InteractionRequestContext Context
        {
            get;
        }

        public Action Callback
        {
            get;
        }

        public InteractionRequestedEventArgs(InteractionRequestContext context, Action callback)
        {
            Context = context;
            Callback = callback;
        }
    }
}