using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibraProgramming.Windows.Interactivity
{
    public static class VisualStateUtilities
    {
        public static bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
        {
            if (String.IsNullOrEmpty(stateName))
            {
                return false;
            }

            var control = element as Control;

            if (null != control)
            {
                control.ApplyTemplate();
                return VisualStateManager.GoToState(control, stateName, useTransitions);
            }

            return false;
        }

        public static bool TryFindNearestStatefulControl(FrameworkElement context, out FrameworkElement resolvedControl)
        {
            var element = context;

            if (null == element)
            {
                resolvedControl = null;
                return false;
            }

            var parent = element.Parent as FrameworkElement;
            var flag = true;

            while (!HasVisualStateGroupsDefined(element) && ShouldContinueTreeWalk(parent))
            {
                element = parent;
                parent = parent.Parent as FrameworkElement;
            }
            if (HasVisualStateGroupsDefined(element))
            {
                if (element.Parent is Control)
                {
                    element = element.Parent as FrameworkElement;
                }
                else if (parent is UserControl)
                {
                    element = parent;
                }
            }
            else
            {
                flag = false;
            }

            resolvedControl = element;

            return flag;
        }
        private static bool HasVisualStateGroupsDefined(FrameworkElement element)
        {
            return ((null != element) && (VisualStateManager.GetVisualStateGroups(element).Count != 0));
        }
        
        private static bool ShouldContinueTreeWalk(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }
            if (element is UserControl)
            {
                return false;
            }
            if (element.Parent == null)
            {
                var element2 = FindTemplatedParent(element);

                if ((null == element2) || (!(element2 is Control) && !(element2 is ContentPresenter)))
                {
                    return false;
                }
            }

            return true;

        }
    
        private static FrameworkElement FindTemplatedParent(FrameworkElement parent)
        {
            return (parent.Parent as FrameworkElement);
        }
    }
}
