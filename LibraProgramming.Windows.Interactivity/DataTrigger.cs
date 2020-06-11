using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public enum ComparisonConditionType
    {
        /// <summary>
        /// 
        /// </summary>
        Equal,

        /// <summary>
        /// 
        /// </summary>
        NotEqual,

        /// <summary>
        /// 
        /// </summary>
        LessThan,

        /// <summary>
        /// 
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// 
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 
        /// </summary>
        GreaterThanOrEqual
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataTrigger : PropertyChangedTrigger
    {
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty ComparisonProperty;

        /// <summary>
        /// 
        /// </summary>
        public ComparisonConditionType Comparison
        {
            get => (ComparisonConditionType) GetValue(ComparisonProperty);
            set => SetValue(ComparisonProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTrigger()
        {
        }

        static DataTrigger()
        {
            ComparisonProperty = DependencyProperty
                .Register(
                    nameof(Comparison),
                    typeof (ComparisonConditionType),
                    typeof (DataTrigger),
                    new PropertyMetadata(ComparisonConditionType.Equal, OnComparisonPropertyChanged)
                );
            ValueProperty = DependencyProperty
                .Register(
                    nameof(Value),
                    typeof (object),
                    typeof (DataTrigger),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnValuePropertyChanged)
                );
        }

        protected override void EvaluateBindingChanged(DependencyPropertyChangedEventArgs args)
        {
            if (EvaluateCompare())
            {
                DoInvokeActions(args);
            }
        }

        private bool EvaluateCompare()
            => null != AttachedObject && Interactivity.Comparison.Evaluate(Comparison, Binding, Value);

        private static void OnComparisonPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
            => ((DataTrigger) source).EvaluateBindingChanged(e);

        private static void OnValuePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
            => ((DataTrigger) source).EvaluateBindingChanged(e);
    }
}