using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public interface IAttachedObject
    {
        FrameworkElement AttachedObject
        {
            get;
        }

        void Attach(FrameworkElement element);

        void Detach();
    }
}
