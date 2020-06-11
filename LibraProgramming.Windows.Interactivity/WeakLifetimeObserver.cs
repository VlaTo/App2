using System;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public class WeakLifetimeObserver
    {
        private readonly WeakReference<ILifetimeTarget> reference;

        public WeakLifetimeObserver(FrameworkElement element, ILifetimeTarget target)
        {
            reference = new WeakReference<ILifetimeTarget>(target);
            element.Loaded += OnElementLoaded;
            element.Unloaded += OnElementUnloaded;
        }

        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;

            ILifetimeTarget target;

            if (reference.TryGetTarget(out target))
            {
                target.AttachedObjectLoaded(element);
            }
        }

        private void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;

            ILifetimeTarget target;

            if (reference.TryGetTarget(out target))
            {
                target.AttachedObjectUnloaded(element);
            }
        }
    }
}