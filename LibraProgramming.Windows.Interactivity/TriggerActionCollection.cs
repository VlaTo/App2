using System;
using Windows.UI.Xaml;
using ApplicationModel = Windows.ApplicationModel;

namespace LibraProgramming.Windows.Interactivity
{
    public class TriggerActionCollection : AttachableCollection<TriggerAction>
    {
        internal override void ItemAdded(TriggerAction item)
        {
            if (item.IsHosted)
            {
                throw new InvalidOperationException("Cannot Host TriggerAction Multiple Times");
            }
            
            if (null != AttachedObject)
            {
                item.Attach(AttachedObject);
            }
            
            item.IsHosted = true;
        }

        internal override void ItemRemoved(TriggerAction item)
        {
            if (null != ((IAttachedObject)item).AttachedObject)
            {
                item.Detach();
            }

            item.IsHosted = false;
        }

        protected override void DoAttach(FrameworkElement element)
        {
            base.DoAttach(element);

            if (null == AttachedObject)
            {
                if (!ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    AttachedObject = element;
                }
                
                OnAttached();
            }
        }

        protected override void OnAttached()
        {
            foreach (var trigger in this)
            {
                trigger.Attach(AttachedObject);
            }
        }

        protected override void OnDetaching()
        {
            foreach (var trigger in this)
            {
                trigger.Detach();
            }
        }
    }
}
