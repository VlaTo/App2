using Windows.UI.Xaml;
using ApplicationModel = Windows.ApplicationModel;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class LoadableAttachableCollection<T> : AttachableCollection<T>, ILifetimeTarget 
        where T : FrameworkElement, IAttachedObject
    {
        private bool isLoaded;
//        private WeakReference<FrameworkElement> reference;

        public void AttachedObjectLoaded(FrameworkElement element)
        {
            if (null != AttachedObject || isLoaded)
            {
                return;
            }

            isLoaded = true;

            if (!ApplicationModel.DesignMode.DesignModeEnabled)
            {
                AttachedObject = element;
            }

            OnAttached();
        }

        public void AttachedObjectUnloaded(FrameworkElement element)
        {
            if (isLoaded)
            {
                isLoaded = false;
                Detach();
            }
        }

        protected override void DoAttach(FrameworkElement element)
        {
            base.DoAttach(element);
            new WeakLifetimeObserver(element, this);
        }
    }
}
