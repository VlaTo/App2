using System;
using System.Collections.Generic;
using System.Reflection;

namespace LibraProgramming.Windows.Core
{
    public sealed class WeakEventHandler<TArgs>
        where TArgs : EventArgs
    {
        private readonly List<Tuple<WeakReference, MethodInfo>> targets;

        public bool IsAlive
        {
            get
            {
                return targets.Exists(tuple => tuple.Item1.IsAlive);
            }
        }

        public WeakEventHandler()
        {
            targets = new List<Tuple<WeakReference, MethodInfo>>();
        }

        public void AddHandler(EventHandler<TArgs> handler)
        {
            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var target = handler.Target;

            for (var index = 0; index < targets.Count;)
            {
                var reference = targets[index];

                if (false == reference.Item1.IsAlive)
                {
                    targets.RemoveAt(index);
                    continue;
                }

                if (reference.Item1 == target)
                {
                    return;
                }

                index++;
            }

            targets.Add(new Tuple<WeakReference, MethodInfo>(
                new WeakReference(target),
                handler.GetMethodInfo()
            ));
        }

        public void RemoveHandler(EventHandler<TArgs> handler)
        {
            if (null == handler)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var target = handler.Target;

            for (var index = 0; index < targets.Count;)
            {
                var reference = targets[index];

                if (false == reference.Item1.IsAlive || reference.Item1 == target)
                {
                    targets.RemoveAt(index);
                    continue;
                }
                
                index++;
            }
        }

        public void Invoke(object sender, TArgs args)
        {
            var list = new List<Delegate>();

            for (var index = 0; index < targets.Count;)
            {
                var reference = targets[index];

                if (false == reference.Item1.IsAlive)
                {
                    targets.RemoveAt(index);
                    continue;
                }

                var handler = reference.Item2.CreateDelegate(typeof(EventHandler<TArgs>), reference.Item1.Target);

                list.Add(handler);

                index++;
            }

            var handlers = list.ToArray();

            if (0 == handlers.Length)
            {
                return;
            }

            var @delegate = Delegate.Combine(handlers);

            @delegate.DynamicInvoke(sender, args);
        }
    }
}