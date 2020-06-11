using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public interface ILifetimeTarget
    {
        void AttachedObjectLoaded(FrameworkElement element);

        void AttachedObjectUnloaded(FrameworkElement element);
    }
}
