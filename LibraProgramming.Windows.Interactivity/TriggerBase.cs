using LibraProgramming.Windows.Core;
using System;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace LibraProgramming.Windows.Interactivity
{
    [ContentProperty(Name = nameof(Actions))]
    public abstract class TriggerBase : InteractivityBase
    {
        public static readonly DependencyProperty ActionsProperty;

        private readonly WorkItemExecutor executor;

        public TriggerActionCollection Actions => (TriggerActionCollection) GetValue(ActionsProperty);

        public event EventHandler<CancelEventArgs> PreviewInvoke;

        static TriggerBase()
        {
            ActionsProperty = DependencyProperty.Register(nameof(Actions),
                typeof (TriggerActionCollection),
                typeof (TriggerBase),
                null);
        }

        internal TriggerBase()
        {
            executor = new WorkItemExecutor(Dispatcher);

            //AttachedObjectTypeConstraint = constraint;
            
            var actionCollection = new TriggerActionCollection();
            
            SetValue(ActionsProperty, actionCollection);
        }

        public override void Attach(FrameworkElement element)
        {
            if (element == AttachedObject)
            {
                return;
            }

            if (null != AttachedObject)
            {
                throw new InvalidOperationException("Cannot Host Trigger Multiple Times");
            }

            if (null != element && false == CheckTypeConstraint(element))
            {
                throw new InvalidOperationException("Type Constraint Violated");
            }

            AttachedObject = element;

            OnAssociatedObjectChanged();

            //Attach handles the DataContext
            base.Attach(element);
            
            Actions.Attach(element);
        }

        protected void DoInvokeActions(object parameter)
        {
            var handler = PreviewInvoke;

            executor.QueueItem(() =>
            {
                if (null != handler)
                {
                    var e = new CancelEventArgs(false);

                    handler.Invoke(this, e);

                    if (e.Cancel)
                    {
                        return;
                    }
                }

                foreach (var trigger in Actions)
                {
                    trigger.Call(parameter);
                }
            });
        }

        private bool CheckTypeConstraint(FrameworkElement element)
        {
            //AttachedObjectTypeConstraint.GetTypeInfo().IsAssignableFrom(element.GetType().GetTypeInfo())
            return AttachedObjectTypeConstraint.IsInstanceOfType(element);
        }
    }

    public abstract class TriggerBase<T> : TriggerBase where T : FrameworkElement
    {
        protected new T AttachedObject => (T) base.AttachedObject;

        protected TriggerBase(Type constraint)
        {
            AttachedObjectTypeConstraint = constraint;
        }
    }
}
