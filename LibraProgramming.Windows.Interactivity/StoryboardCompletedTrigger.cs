using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.Interactivity
{
    public sealed class StoryboardCompletedTrigger : StoryboardTrigger
    {
        public StoryboardCompletedTrigger()
        {
        }

        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs e)
        {
            var storyboard = (Storyboard) e.OldValue;

            /*if (current == storyboard)
            {
                return;
            }*/

            if (null != storyboard)
            {
                storyboard.Completed -= OnStoryboardCompleted;
            }

            storyboard = (Storyboard) e.NewValue;

            if (null != storyboard)
            {
                storyboard.Completed += OnStoryboardCompleted;
            }
        }

        private void OnStoryboardCompleted(object sender, object e)
        {
            if (null != Storyboard)
            {
                DoInvokeActions(e);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (null != Storyboard)
            {
                Storyboard.Completed -= OnStoryboardCompleted;
            }
        }
    }
}