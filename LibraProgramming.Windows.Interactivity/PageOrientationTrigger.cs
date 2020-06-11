using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace LibraProgramming.Windows.Interactivity
{
    [ContentProperty(Name = "Actions")]
    public class PageOrientationTrigger : TriggerBase<FrameworkElement>
    {
        private const ApplicationViewOrientation DefaultOrientation = ApplicationViewOrientation.Landscape;

        public PageOrientationTrigger()
            : base(typeof(FrameworkElement))
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Window.Current.SizeChanged += WindowSizeChanged;
            DoInvokeActions(GetCurrentOrientation());
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            Window.Current.SizeChanged -= WindowSizeChanged;
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            DoInvokeActions(GetCurrentOrientation());
        }

        private ApplicationViewOrientation GetCurrentOrientation()
        {
            var view = ApplicationView.GetForCurrentView();
            return view?.Orientation ?? DefaultOrientation;
        }
    }
}
