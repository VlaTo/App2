using System;
using System.Reflection;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class Behavior : InteractivityBase
    {
        internal Behavior(Type typeConstraint)
        {
            AttachedObjectTypeConstraint = typeConstraint;
        }
    
        public override void Attach(FrameworkElement element)
        {
            if (element == AttachedObject)
            {
                return;
            }

            if (null != AttachedObject)
            {
                throw new InvalidOperationException("Cannot host behavior multiple times.");
            }

            if (null != element && !AttachedObjectTypeConstraint.GetTypeInfo().IsAssignableFrom(element.GetType().GetTypeInfo()))
            {
                throw new InvalidOperationException("Type constraint violated.");
            }
            
            AttachedObject = element;
            
            OnAssociatedObjectChanged();
            
            //Attach handles the DataContext
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
    }

    public abstract class Behavior<T> : Behavior 
        where T : FrameworkElement
    {
        protected new T AttachedObject
        {
            get
            {
                return (T) base.AttachedObject;
            }
        }

        protected Behavior()
            : base(typeof(T))
        {
        }
    }
}
