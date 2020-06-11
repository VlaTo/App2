using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class InteractivityBase : FrameworkElement, IAttachedObject
    {
        protected FrameworkElement AttachedObject
        {
            get; 
            set;
        }

        protected Type AttachedObjectTypeConstraint
        {
            get;
            set;
        }

        internal event EventHandler AttachedObjectChanged;

        FrameworkElement IAttachedObject.AttachedObject => AttachedObject;

        protected InteractivityBase()
        {
        }

        public virtual void Attach(FrameworkElement element)
        {
            if (null == element)
            {
                return;
            }

            element.AddDataContextChangedHandler(DoDataContextChanged);

            if (null != element.DataContext)
            {
                DoDataContextChanged(element, EventArgs.Empty);
            }
        }

        public virtual void Detach()
        {
            if (null != AttachedObject)
            {
                AttachedObject.RemoveDataContextChangedHandler(DoDataContextChanged);
            }
        }

        protected void OnAssociatedObjectChanged()
        {
            var handler = AttachedObjectChanged;

            if (null == handler)
            {
                return;
            }

            handler(this, new EventArgs());
        }
        
        protected virtual void OnAttached()
        {
        }
        
        protected virtual void OnDetaching()
        {
        }
        
        protected virtual void OnDataContextChanged(object oldValue, object newValue)
        {
        }    

        private void DoDataContextChanged(object sender, EventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var dataContext = DataContext;

                SetBinding(DataContextProperty,
                    new Binding
                    {
                        Path = new PropertyPath(nameof(DataContext)),
                        Source = element
                    });

                OnDataContextChanged(dataContext, DataContext);
            }
        }
    }
}
