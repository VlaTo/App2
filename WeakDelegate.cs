using System;
using System.Reflection;

namespace LibraProgramming.Data.Interop
{
    public sealed class WeakDelegate<TDelegate> : IEquatable<TDelegate>
    {
        private readonly WeakReference reference;
        private readonly MethodInfo method;

        public bool IsAlive
        {
            get
            {
                return null != reference && reference.IsAlive;
            }
        }

        public WeakDelegate(Delegate @delegate)
        {
            if (null != @delegate.Target)
            {
                reference = new WeakReference(@delegate.Target);
            }
            method = @delegate.GetMethodInfo();
        }

        public TDelegate CreateDelegate()
        {
            return (TDelegate) ((object) CreateDelegateInternal());
        }

        private Delegate CreateDelegateInternal()
        {
            if (null != reference)
            {
                return method.CreateDelegate(typeof (TDelegate), reference.Target);
            }

            return method.CreateDelegate(typeof(TDelegate));
        }

        public bool Equals(TDelegate other)
        {
            var @delegate = (Delegate)((object)other);
            return null != @delegate && reference.Target == @delegate.Target && method.Equals(@delegate.GetMethodInfo());
        }

        public void Invoke(params object[] args)
        {
            CreateDelegateInternal().DynamicInvoke(args);
        }
    }
}
