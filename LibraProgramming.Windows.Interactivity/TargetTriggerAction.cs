using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class TargetTriggerAction : TriggerAction
    {
        public static readonly DependencyProperty TargetObjectProperty;
        public static readonly DependencyProperty TargetNameProperty;

        private readonly Type targetTypeConstraint;
        private readonly NameResolver targetResolver;

        public object TargetObject
        {
            get
            {
                return GetValue(TargetObjectProperty);
            }
            set
            {
                SetValue(TargetObjectProperty, value);
            }
        }

        public string TargetName
        {
            get
            {
                return (string) GetValue(TargetNameProperty);
            }
            set
            {
                SetValue(TargetNameProperty, value);
            }
        }

        protected object Target
        {
            get
            {
                object target = AttachedObject;

                if (null != TargetObject)
                {
                    target = TargetObject;
                }
                else if (IsTargetNameSet)
                {
                    target = TargetResolver.Object;
                }
                if (null == target ||
                    TargetTypeConstraint.GetTypeInfo().IsAssignableFrom(target.GetType().GetTypeInfo()))
                {
                    return target;
                }

                throw new InvalidOperationException("Retargeted Type Constraint Violated");
            }
        }

        protected new Type AttachedObjectTypeConstraint
        {
            get
            {
                var customAttributes = GetType()
                    .GetTypeInfo()
                    .GetCustomAttributes(typeof (TypeConstraintAttribute), true);

                if (customAttributes.Any())
                {
                    return ((TypeConstraintAttribute) customAttributes.First()).Constraint;
                }

                return typeof (FrameworkElement);
            }
        }

        protected Type TargetTypeConstraint
        {
            get
            {
                return targetTypeConstraint;
            }
        }

        private bool IsTargetNameSet
        {
            get
            {
                if (String.IsNullOrEmpty(TargetName))
                {
                    return ReadLocalValue(TargetNameProperty) != DependencyProperty.UnsetValue;
                }

                return true;
            }
        }

        private NameResolver TargetResolver
        {
            get
            {
                return targetResolver;
            }
        }

        private bool IsTargetChangedRegistered
        {
            get; 
            set;
        }

        static TargetTriggerAction()
        {
            TargetObjectProperty = DependencyProperty
                .Register(
                    nameof(TargetObject), // "TargetObject",
                    typeof (object),
                    typeof (TargetTriggerAction),
                    new PropertyMetadata(null, OnTargetObjectChanged)
                );
            TargetNameProperty = DependencyProperty
                .Register(
                    nameof(TargetName), //"TargetName",
                    typeof (string),
                    typeof (TargetTriggerAction),
                    new PropertyMetadata(null, OnTargetNameChanged)
                );
        }

        internal TargetTriggerAction(Type targetTypeConstraint)
            : base(typeof (FrameworkElement))
        {
            this.targetTypeConstraint = targetTypeConstraint;
            targetResolver = new NameResolver();
            RegisterTargetChanged();
        }

        /// <summary>
        /// Called when the target changes.
        /// 
        /// </summary>
        /// <param name="oldTarget">The old target.</param><param name="newTarget">The new target.</param>
        /// <remarks>
        /// This function should be overriden in derived classes to hook and unhook functionality from the changing source objects.
        /// </remarks>
        internal virtual void OnTargetChangedImpl(object oldTarget, object newTarget)
        {
        }

        /// <summary>
        /// Called after the action is attached to an AssociatedObject.
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            var associatedObject = AttachedObject;
            var behavior = associatedObject as Behavior;

            RegisterTargetChanged();

            if (null != behavior)
            {
                associatedObject = ((IAttachedObject) behavior).AttachedObject;
                behavior.AttachedObjectChanged += OnBehaviorHostChanged;
            }

            TargetResolver.NameScopeElement = associatedObject;
        }

        /// <summary>
        /// Called when the action is being detached from its AssociatedObject, but before it has actually occurred.
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            var behavior = AttachedObject as Behavior;

            base.OnDetaching();

            OnTargetChangedImpl(TargetResolver.Object, null);
            UnregisterTargetChanged();

            if (null != behavior)
            {
                behavior.AttachedObjectChanged -= OnBehaviorHostChanged;
            }

            TargetResolver.NameScopeElement = null;
        }

        private void OnBehaviorHostChanged(object sender, EventArgs e)
        {
            TargetResolver.NameScopeElement = ((IAttachedObject) sender).AttachedObject;
        }

        private void RegisterTargetChanged()
        {
            if (IsTargetChangedRegistered)
            {
                return;
            }

            TargetResolver.ResolvedElementChanged += OnTargetChanged;

            IsTargetChangedRegistered = true;
        }

        private void UnregisterTargetChanged()
        {
            if (!IsTargetChangedRegistered)
            {
                return;
            }

            TargetResolver.ResolvedElementChanged -= OnTargetChanged;

            IsTargetChangedRegistered = false;
        }

        private void OnTargetChanged(object sender, NameResolvedEventArgs e)
        {
            if (null == AttachedObject)
            {
                return;
            }

            OnTargetChangedImpl(e.OldObject, e.NewObject);
        }

        private static void OnTargetObjectChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((TargetTriggerAction) source).OnTargetChanged(source, new NameResolvedEventArgs(args.OldValue, args.NewValue));
        }

        private static void OnTargetNameChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((TargetTriggerAction) source).TargetResolver.Name = (string) args.NewValue;
        }
    }
    
    public abstract class TargetTriggerAction<T> : TargetTriggerAction 
        where T : class
    {
        protected new T Target
        {
            get
            {
                return (T) base.Target;
            }
        }

        protected TargetTriggerAction()
            : base(typeof(T))
        {
        }

        internal sealed override void OnTargetChangedImpl(object oldTarget, object newTarget)
        {
            base.OnTargetChangedImpl(oldTarget, newTarget);
            OnTargetChanged(oldTarget as T, newTarget as T);
        }

        /// <summary>
        /// Called when the target property changes.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Override this to hook and unhook functionality on the specified Target, rather than the AssociatedObject.
        /// </remarks>
        /// <param name="oldTarget">The old target.</param><param name="newTarget">The new target.</param>
        protected virtual void OnTargetChanged(T oldTarget, T newTarget)
        {
        }
    }
}