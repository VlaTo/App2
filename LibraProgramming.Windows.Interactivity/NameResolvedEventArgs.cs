using System;

namespace LibraProgramming.Windows.Interactivity
{
    internal sealed class NameResolvedEventArgs : EventArgs
    {
        public object OldObject
        {
            get; 
            private set;
        }

        public object NewObject
        {
            get; 
            private set;
        }

        public NameResolvedEventArgs(object oldObject, object newObject)
        {
            OldObject = oldObject;
            NewObject = newObject;
        }
    }
}
