using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = nameof(Actions))]
    public class EventTrigger : EventTriggerBase<object>
    {
        public static readonly DependencyProperty EventNameProperty;

        public string EventName
        {
            get => (string)GetValue(EventNameProperty);
            set => SetValue(EventNameProperty, value);
        }

        public EventTrigger()
        {
        }

        public EventTrigger(string eventName)
        {
            EventName = eventName;
        }

        protected override string GetEventName() => EventName;

        static EventTrigger()
        {
            EventNameProperty = DependencyProperty
                .Register(nameof(EventName),
                    typeof (string),
                    typeof (EventTrigger),
                    new PropertyMetadata("Loaded", OnEventNameChanged)
                );
        }

        private static void OnEventNameChanged(object source, DependencyPropertyChangedEventArgs args)
            => ((EventTriggerBase) source).OnEventNameChanged((string) args.OldValue, (string) args.NewValue);
    }
}
