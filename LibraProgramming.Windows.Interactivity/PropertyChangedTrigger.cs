using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public class PropertyChangedTrigger : TriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty BindingProperty;

        /// <summary>
        /// 
        /// </summary>
        public object Binding
        {
            get
            {
                return GetValue(BindingProperty);
            }
            set
            {
                SetValue(BindingProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected PropertyChangedTrigger()
            : base(typeof (FrameworkElement))
        {
        }

        static PropertyChangedTrigger()
        {
            BindingProperty = DependencyProperty
                .Register(
                    nameof(Binding),
                    typeof (object),
                    typeof (PropertyChangedTrigger),
                    new PropertyMetadata(null, OnBindingPropertyChanged)
                );
        }

        protected virtual void EvaluateBindingChanged(DependencyPropertyChangedEventArgs args)
        {
            DoInvokeActions(args);
        }

        private static void OnBindingPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((PropertyChangedTrigger) source).EvaluateBindingChanged(e);
        }
    }
}