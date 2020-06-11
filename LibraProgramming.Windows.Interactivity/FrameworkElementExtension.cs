using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Interactivity
{
    public static class FrameworkElementExtension
    {
        /// <summary>
        /// Identifies the InheritedDataContext DependencyProperty.
        /// </summary>
        internal static readonly DependencyProperty InheritedDataContextProperty;

        /// <summary>
        /// Identifies the DataContextChangedHandler DependencyProperty.
        /// </summary>
        internal static readonly DependencyProperty DataContextChangedHandlerProperty;

        static FrameworkElementExtension()
        {
            InheritedDataContextProperty = DependencyProperty.RegisterAttached("InheritedDataContext",
                typeof (object),
                typeof (FrameworkElementExtension),
                new PropertyMetadata(null,
                    OnInheritedDataContextChanged));
            DataContextChangedHandlerProperty = DependencyProperty.RegisterAttached("DataContextChangedHandler",
                typeof (EventHandler),
                typeof (FrameworkElementExtension),
                null);
        }

        /// <summary>
        /// Adds the data context changed handler.
        /// </summary>
        /// <param name="element">Element to which the handler is added</param>
        /// <param name="handler">The handler to add</param>
        public static void AddDataContextChangedHandler(this FrameworkElement element, EventHandler handler)
        {
            if (null == element || null == handler)
            {
                return;
            }

            if (null == element.GetValue(InheritedDataContextProperty))
            {
                element.SetBinding(InheritedDataContextProperty, new Binding());
            }

            var currentHandler = (EventHandler)element.GetValue(DataContextChangedHandlerProperty);

            currentHandler += handler;

            element.SetValue(DataContextChangedHandlerProperty, currentHandler);
        }

        /// <summary>
        /// Removes the data context changed handler.
        /// </summary>
        /// <param name="element">The element from which the handler has to be removed</param>
        /// <param name="handler">The handler to remove</param>
        public static void RemoveDataContextChangedHandler(this FrameworkElement element, EventHandler handler)
        {
            if (null == element || null == handler)
            {
                return;
            }

            var currentHandler = (EventHandler)element.GetValue(DataContextChangedHandlerProperty);

            currentHandler -= handler;

            if (null == currentHandler)
            {
                element.ClearValue(DataContextChangedHandlerProperty);
                element.ClearValue(InheritedDataContextProperty);
            }
            else
            {
                element.SetValue(DataContextChangedHandlerProperty, currentHandler);
            }
        }

        /// <summary>
        /// Handles changes to the InheritedDataContext DependencyProperty.
        /// </summary>
        /// <param name="source">Instance with property change.</param>
        /// <param name="e">Property change details.</param>
        private static void OnInheritedDataContextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var handler = source.GetValue(DataContextChangedHandlerProperty) as EventHandler;

            if (null != handler)
            {
                handler(source, EventArgs.Empty);
            }
        }
    }
}
