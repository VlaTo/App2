using System;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class TriggerAction : InteractivityBase
    {
        public static readonly DependencyProperty IsEnabledProperty;

        [DefaultValue(true)]
        public bool IsEnabled
        {
            get
            {
                return (bool) GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        internal bool IsHosted
        {
            get; 
            set;
        }

        protected TriggerAction(Type constraint)
        {
            AttachedObjectTypeConstraint = constraint;
        }

        static TriggerAction()
        {
            IsEnabledProperty = DependencyProperty
                .Register(
                    nameof(IsEnabled),
                    typeof (bool),
                    typeof (TriggerAction),
                    new PropertyMetadata(true)
                );
        }

        public override void Attach(FrameworkElement element)
        {
            if (element == AttachedObject)
            {
                return;
            }

            if (null != AttachedObject)
            {
                throw new InvalidOperationException("Cannot Host TriggerAction Multiple Times");
            }

//            if (null != element && !AttachedObjectTypeConstraint.GetTypeInfo().IsAssignableFrom(element.GetType().GetTypeInfo()))
            if (null != element && (null == AttachedObjectTypeConstraint || false == AttachedObjectTypeConstraint.IsInstanceOfType(element)))
            {
                throw new InvalidOperationException("Type Constraint Violated");
            }

            AttachedObject = element;

            OnAssociatedObjectChanged();

            base.Attach(element);
            
            OnAttached();
        }

        public override void Detach()
        {
            base.Detach();

            OnDetaching();

            AttachedObject = null;

            OnAssociatedObjectChanged();
        }

        internal void Call(object value)
        {
            if (IsEnabled)
            {
                Invoke(value);
            }
        }

        protected abstract void Invoke(object value);

    }

    public abstract class TriggerAction<T> : TriggerAction
        where T : FrameworkElement
    {
        protected new T AttachedObject => (T) base.AttachedObject;

        /*protected override sealed Type AttachedObjectTypeConstraint
        {
            get
            {
                return base.AttachedObjectTypeConstraint;
            }
        }*/

        protected TriggerAction(Type constraint) 
            : base(constraint)
        {
        }
    }
}
