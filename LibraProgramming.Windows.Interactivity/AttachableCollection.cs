using System;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using ApplicationModel = Windows.ApplicationModel;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class AttachableCollection<T> : FrameworkElementCollection<T>, IAttachedObject 
        where T : FrameworkElement, IAttachedObject
    {
        public FrameworkElement AttachedObject
        {
            get; 
            protected set;
        }

        internal AttachableCollection()
        {
            CollectionChanged += OnCollectionChanged;
        }

        public void Attach(FrameworkElement element)
        {
            if (element == AttachedObject)
            {
                return;
            }

            if (null != AttachedObject)
            {
                throw new InvalidOperationException();
            }

            if (!ApplicationModel.DesignMode.DesignModeEnabled)
            {
                DoAttach(element);
            }
        }

        public void Detach()
        {
            if (null == AttachedObject)
            {
                return;
            }

            OnDetaching();

            AttachedObject = null;
        }

        internal abstract void ItemAdded(T item);

        internal abstract void ItemRemoved(T item);

        protected abstract void OnAttached();

        protected abstract void OnDetaching();

        protected virtual void DoAttach(FrameworkElement element)
        {
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (T item in e.NewItems)
                    {
                        ItemAdded(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (T item in e.OldItems)
                    {
                        ItemRemoved(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Replace:
                {
                    foreach (T item in e.OldItems)
                    {
                        ItemRemoved(item);
                    }

                    foreach (T item in e.NewItems)
                    {
                        ItemAdded(item);
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Reset:
                {
                    foreach (var item in this)
                    {
                        ItemRemoved(item);
                    }

                    foreach (var item in this)
                    {
                        ItemAdded(item);
                    }

                    break;
                }

                default:
                {
                    return;
                }
            }
        }
    }
}
