using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.Interactivity
{
    public abstract class StoryboardAction : TriggerAction<FrameworkElement>
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

        protected StoryboardAction()
            : base(typeof (FrameworkElement))
        {
        }

        static StoryboardAction()
        {
            StoryboardProperty = DependencyProperty
                .Register(
                    nameof(Storyboard),
                    typeof (Storyboard),
                    typeof (StoryboardAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnStoryboardPropertyChanged)
                );
        }

        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnStoryboardPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((StoryboardAction) source).OnStoryboardChanged(e);
        }
    }
}