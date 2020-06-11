using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LibraProgramming.Windows.Interactivity
{
    public static class Interaction
    {
        public static readonly DependencyProperty TriggersProperty;
        public static readonly DependencyProperty BehaviorsProperty;

        public static BehaviorCollection GetBehaviors(FrameworkElement element)
        {
            var behaviors = (BehaviorCollection)element.GetValue(BehaviorsProperty);

            if (null == behaviors)
            {
                behaviors = new BehaviorCollection();
                element.SetValue(BehaviorsProperty, behaviors);
            }

            return behaviors;
        }

        public static TriggerCollection GetTriggers(FrameworkElement element)
        {
            var triggers = (TriggerCollection)element.GetValue(TriggersProperty);

            if (null == triggers)
            {
                triggers = new TriggerCollection();
                element.SetValue(TriggersProperty, triggers);
            }

            return triggers;
        }

        static Interaction()
        {
            TriggersProperty = DependencyProperty.RegisterAttached("Triggers",
                typeof (TriggerCollection),
                typeof (Interaction),
                new PropertyMetadata(null,
                    OnTriggersPropertyChanged));
            BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors",
                typeof (BehaviorCollection),
                typeof (Interaction),
                new PropertyMetadata(null,
                    OnBehaviorsPropertyChanged));
        }
        
        private static void OnBehaviorsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var previous = args.OldValue as BehaviorCollection;
            var current = args.NewValue as BehaviorCollection;

            if (previous == current)
            {
                return;
            }

            if (previous != null && null != previous.AttachedObject)
            {
                previous.Detach();
            }

            if (null == current || null == source)
            {
                return;
            }

            if (null != current.AttachedObject)
            {
                throw new InvalidOperationException("Cannot Host BehaviorCollection Multiple Times");
            }

            var element = source as FrameworkElement;

            if (null == element)
            {
                throw new InvalidOperationException("Can only host BehaviorCollection on FrameworkElement");
            }

            current.Attach(element);
        }

        private static void OnTriggersPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var previous = e.OldValue as TriggerCollection;
            var current = e.NewValue as TriggerCollection;

            if (previous == current)
            {
                return;
            }

            if (null != previous && null != previous.AttachedObject)
            {
                previous.Detach();
            }

            if (null == current || null == source)
            {
                return;
            }

            if (null != current.AttachedObject)
            {
                throw new InvalidOperationException();
            }

            var element = source as FrameworkElement;

            if (null == element)
            {
                throw new InvalidOperationException();
            }

            current.Attach(element);
        }

        internal static bool IsElementLoaded(FrameworkElement element)
        {
            var visualRoot = Window.Current.Content;

            if (null != (element.Parent ?? VisualTreeHelper.GetParent(element)))
            {
                return true;
            }

            if (null != visualRoot)
            {
                return element == visualRoot;
            }

            return false;
        }
    }
}
