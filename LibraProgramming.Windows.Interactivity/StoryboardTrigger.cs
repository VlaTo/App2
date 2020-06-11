using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class StoryboardTrigger : TriggerBase<FrameworkElement>
    {
        public static readonly DependencyProperty StoryboardProperty;

        /// <summary>
        /// 
        /// </summary>
        public Storyboard Storyboard
        {
            get
            {
                return (Storyboard) GetValue(StoryboardProperty);
            }
            set
            {
                SetValue(StoryboardProperty, value);
            }
        }

        protected StoryboardTrigger()
            : base(typeof (FrameworkElement))
        {
        }

        static StoryboardTrigger()
        {
            StoryboardProperty = DependencyProperty
                .Register(
                    nameof(Storyboard),
                    typeof (Storyboard),
                    typeof (StoryboardTrigger),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnStoryboardPropertyChanged)
                );
        }

        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnStoryboardPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((StoryboardTrigger) source).OnStoryboardChanged(e);
        }
    }
}