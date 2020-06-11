using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class EventTriggerBase : TriggerBase
    {
        public static readonly DependencyProperty SourceObjectProperty;
        public static readonly DependencyProperty SourceNameProperty;

        private const string loadedEventName = nameof(Loaded);

        private IDisposable registrationToken;
        
        public object SourceObject
        {
            get
            {
                return GetValue(SourceObjectProperty);
            }
            set
            {
                SetValue(SourceObjectProperty, value);
            }
        }

        public string SourceName
        {
            get
            {
                return (string)GetValue(SourceNameProperty);
            }
            set
            {
                SetValue(SourceNameProperty, value);
            }
        }
        
        public object Source
        {
            get
            {
                object source = AttachedObject;

                if (null != SourceObject)
                {
                    source = SourceObject;
                }
                else if (IsSourceNameSet)
                {
                    source = SourceNameResolver.Object;

                    if (source != null && !SourceTypeConstraint.GetTypeInfo().IsAssignableFrom(source.GetType().GetTypeInfo()))
                    {
                        throw new InvalidOperationException("Retargeted type constraint violated.");
                    }
                }
                return source;
            }
        }

        /*protected sealed override Type AttachedObjectTypeConstraint
        {
            get
            {
                var attribute = GetType()
                    .GetTypeInfo()
                    .GetCustomAttributes(typeof (TypeConstraintAttribute), true)
                    .FirstOrDefault();

                if (null != attribute)
                {
                    return ((TypeConstraintAttribute) attribute).Constraint;
                }

                return typeof (FrameworkElement);
            }
        }*/

        protected Type SourceTypeConstraint
        {
            get;
        }

        private NameResolver SourceNameResolver
        {
            get;
        }

        private bool IsSourceChangedRegistered
        {
            get;
            set;
        }

        private bool IsLoadedRegistered
        {
            get; 
            set;
        }

        private bool IsSourceNameSet
        {
            get
            {
                if (String.IsNullOrEmpty(SourceName))
                {
                    return DependencyProperty.UnsetValue != ReadLocalValue(SourceNameProperty);
                }
                
                return true;
            }
        }

        protected EventTriggerBase(Type constraint)
        {
            AttachedObjectTypeConstraint = GetConstraint();
            SourceTypeConstraint = constraint;
            SourceNameResolver = new NameResolver();

            RegisterSourceChanged();
        }

        static EventTriggerBase()
        {
            SourceObjectProperty = DependencyProperty
                .Register(
                    nameof(SourceObject),
                    typeof (object),
                    typeof (EventTriggerBase),
                    new PropertyMetadata(null, OnSourceObjectChanged)
                );
            SourceNameProperty = DependencyProperty
                .Register(
                    nameof(SourceName),
                    typeof (string),
                    typeof (EventTriggerBase),
                    new PropertyMetadata(null, OnSourceNameChanged)
                );
        }

        private Type GetConstraint()
        {
            var attribute = GetType()
                .GetTypeInfo()
                .GetCustomAttributes(typeof(TypeConstraintAttribute), true)
                .FirstOrDefault();

            if (null != attribute)
            {
                return ((TypeConstraintAttribute)attribute).Constraint;
            }

            return typeof(FrameworkElement);
        }

        protected abstract string GetEventName();
    
        protected void OnEvent(object eventArgs)
        {
            DoInvokeActions(eventArgs);
        }

        private void OnSourceChanged(object oldSource, object newSource)
        {
            if (null == AttachedObject)
            {
                return;
            }

            OnSourceChangedImpl(oldSource, newSource);

        }
    
        internal virtual void OnSourceChangedImpl(object oldSource, object newSource)
        {
            if (String.IsNullOrEmpty(GetEventName()) || String.Equals(GetEventName(), loadedEventName, StringComparison.Ordinal))
            {
                return;
            }

            if (null != oldSource && SourceTypeConstraint.GetTypeInfo().IsAssignableFrom(oldSource.GetType().GetTypeInfo()))
            {
                UnregisterEvent(oldSource, GetEventName());
            }

            if (null == newSource || !SourceTypeConstraint.GetTypeInfo().IsAssignableFrom(newSource.GetType().GetTypeInfo()))
            {
                return;
            }

            RegisterEvent(newSource, GetEventName());
        }
        
        protected override void OnAttached()
        {
            base.OnAttached();
        
            var attached = AttachedObject;
            var behavior = attached as Behavior;
            
            RegisterSourceChanged();
            
            if (null != behavior)
            {
//                var associatedObject2 = ((IAttachedObject)behavior).AttachedObject;
                behavior.AttachedObjectChanged += OnBehaviorHostChanged;
            }
            else
            {
                if (null == SourceObject && null != attached)
                {
                    SourceNameResolver.NameScopeElement = attached;
//                    goto label_7;
                }
                else
                {
                    try
                    {
                        OnSourceChanged(null, Source);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
//            label_7:
            if (!String.Equals(GetEventName(), loadedEventName, StringComparison.Ordinal) || null == attached || Interaction.IsElementLoaded(attached))
            {
                return;
            }

            RegisterLoaded(attached);
        }
        
        protected override void OnDetaching()
        {
            base.OnDetaching();
        
            var behavior = AttachedObject as Behavior;
            
            var associatedElement = AttachedObject;

            try
            {
                OnSourceChanged(Source, null);
            }
            catch (InvalidOperationException)
            {
            }

            UnregisterSourceChanged();

            if (behavior != null)
            {
                behavior.AttachedObjectChanged -= OnBehaviorHostChanged;
            }

            SourceNameResolver.NameScopeElement = null;

            if (null != associatedElement && String.Equals(GetEventName(), loadedEventName, StringComparison.Ordinal))
            {
                UnregisterLoaded(associatedElement);
            }
        }

        private void RegisterSourceChanged()
        {
            if (IsSourceChangedRegistered)
            {
                return;
            }

            SourceNameResolver.ResolvedElementChanged += OnSourceNameResolverElementChanged;
            IsSourceChangedRegistered = true;
        }
        
        private void UnregisterSourceChanged()
        {
            if (!IsSourceChangedRegistered)
            {
                return;
            }

            SourceNameResolver.ResolvedElementChanged -= OnSourceNameResolverElementChanged;
            IsSourceChangedRegistered = false;
        }

        private void RegisterLoaded(FrameworkElement associatedElement)
        {
            if (IsLoadedRegistered || null == associatedElement)
            {
                return;
            }

            associatedElement.Loaded += DoEvent;

            IsLoadedRegistered = true;
        }

        private void UnregisterLoaded(FrameworkElement associatedElement)
        {
            if (!IsLoadedRegistered || null == associatedElement)
            {
                return;
            }

            associatedElement.Loaded -= DoEvent;

            IsLoadedRegistered = false;
        }

        private void RegisterEvent(object target, string eventName)
        {
            var @event = target.GetType().GetRuntimeEvent(eventName);

            if (null == @event)
            {
                if (null == SourceObject)
                {
                    return;
                }

                throw new ArgumentException("EventTrigger cannot find EventName");

            }

            if (false == IsValidEvent(@event))
            {
//                if (null == SourceObject)
                if (null == Source)
                {
                    return;
                }

                throw new ArgumentException("EventTriggerBase invalid event");
            }

            var method = typeof(EventTriggerBase).GetTypeInfo().GetDeclaredMethod(nameof(DoEvent));
            var handler = method.CreateDelegate(@event.EventHandlerType, this);

            if (IsEventRoutedEvent(@event))
            {
                Func<Delegate, EventRegistrationToken> @add = subscribe => (EventRegistrationToken) @event.AddMethod.Invoke(target, new object[] {subscribe});
                Action<EventRegistrationToken> @remove = unsubscribe => @event.RemoveMethod.Invoke(target, new object[] {unsubscribe});
                WindowsRuntimeMarshal.AddEventHandler(@add, @remove, handler);
                registrationToken = EventRegistration.CreateRoutedEventRegistration(@remove, handler);
            }
            else
            {
                @event.AddEventHandler(Source, handler);
                registrationToken = EventRegistration.CreateEventRegistration(@event, Source, handler);
            }
        }

        private void UnregisterEvent(object obj, string eventName)
        {
            if (String.Equals(eventName, loadedEventName, StringComparison.Ordinal))
            {
                var element = obj as FrameworkElement;

                if (null == element)
                {
                    return;
                }

                UnregisterLoaded(element);

            }
            else if (null != registrationToken)
            {
                registrationToken.Dispose();
                registrationToken = null;
            }
        }

        private void DoEvent(object _, object e) => OnEvent(e);

        private void OnBehaviorHostChanged(object sender, EventArgs e)
        {
            SourceNameResolver.NameScopeElement = ((IAttachedObject) sender).AttachedObject;
        }

        private void OnSourceNameResolverElementChanged(object sender, NameResolvedEventArgs e)
        {
            if (null != SourceObject)
            {
                return;
            }
            
            OnSourceChanged(e.OldObject, e.NewObject);
        }

        internal void OnEventNameChanged(string oldEventName, string newEventName)
        {
            if (null == AttachedObject)
            {
                return;
            }

            var element = Source as FrameworkElement;

            if (null != element && String.Equals(oldEventName, loadedEventName, StringComparison.Ordinal))
            {
                UnregisterLoaded(element);
            }
            else if (!String.IsNullOrEmpty(oldEventName))
            {
                UnregisterEvent(Source, oldEventName);
            }

            if (null != element && String.Equals(newEventName, loadedEventName, StringComparison.Ordinal))
            {
                RegisterLoaded(element);
            }
            else
            {
                if (String.IsNullOrEmpty(newEventName))
                {
                    return;
                }

                RegisterEvent(Source, newEventName);

            }
        }

        private static bool IsValidEvent(EventInfo eventInfo)
        {
            var type = eventInfo.EventHandlerType;

            if (!typeof (Delegate).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return false;
            }

            var parameters = type.GetTypeInfo().GetDeclaredMethod("Invoke").GetParameters();
            var objectTypeInfo = typeof (object).GetTypeInfo();

            if (2 == parameters.Length && objectTypeInfo.IsAssignableFrom(parameters[0].ParameterType.GetTypeInfo()))
            {
                return objectTypeInfo.IsAssignableFrom(parameters[1].ParameterType.GetTypeInfo());
            }

            return false;

        }

        private static bool IsEventRoutedEvent(EventInfo eventInfo)
        {
            var type = eventInfo.EventHandlerType;
            var parameters = type.GetTypeInfo().GetDeclaredMethod("Invoke").GetParameters();

            return 2 == parameters.Length && typeof (RoutedEventArgs).GetTypeInfo().IsAssignableFrom(parameters[1].ParameterType.GetTypeInfo());
        }

        private static void OnSourceObjectChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var eventTriggerBase = (EventTriggerBase)source;
            var newSource = (object)eventTriggerBase.SourceNameResolver.Object;

            if (null == args.NewValue)
            {
                eventTriggerBase.OnSourceChanged(args.OldValue, newSource);
            }
            else
            {
                if (null == args.OldValue && null != newSource)
                {
                    eventTriggerBase.UnregisterEvent(newSource, eventTriggerBase.GetEventName());
                }

                eventTriggerBase.OnSourceChanged(args.OldValue, args.NewValue);

            }
        }
    
        private static void OnSourceNameChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((EventTriggerBase)source).SourceNameResolver.Name = (string)args.NewValue;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventTriggerBase<T> : EventTriggerBase
        where T : class
    {
        public new T Source => (T)base.Source;

        protected EventTriggerBase()
            : base(typeof(T))
        {
        }

        internal sealed override void OnSourceChangedImpl(object oldSource, object newSource)
        {
            base.OnSourceChangedImpl(oldSource, newSource);
            OnSourceChanged(oldSource as T, newSource as T);
        }

        protected virtual void OnSourceChanged(T oldSource, T newSource)
        {
        }
    }
}
