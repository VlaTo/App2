using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public enum StoryboardControlOption
    {
        /// <summary>
        /// 
        /// </summary>
        Stop,

        /// <summary>
        /// 
        /// </summary>
        Play,

        /// <summary>
        /// 
        /// </summary>
        Pause,

        /// <summary>
        /// 
        /// </summary>
        Resume,

        /// <summary>
        /// 
        /// </summary>
        Toggle,

        /// <summary>
        /// 
        /// </summary>
        SkipToEnd
    }

    /// <summary>
    /// 
    /// </summary>
    public class StoryboardControlAction : StoryboardAction
    {
        public static readonly DependencyProperty ControlOptionProperty;

        private bool isPaused;

        /// <summary>
        /// 
        /// </summary>
        public StoryboardControlOption ControlOption
        {
            get
            {
                return (StoryboardControlOption) GetValue(ControlOptionProperty);
            }
            set
            {
                SetValue(ControlOptionProperty, value);
            }
        }

        static StoryboardControlAction()
        {
            ControlOptionProperty = DependencyProperty
                .Register(
                    nameof(ControlOption),
                    typeof (StoryboardControlOption),
                    typeof (StoryboardControlAction),
                    new PropertyMetadata(StoryboardControlOption.Play)
                );
        }

        protected override void Invoke(object value)
        {
            if (null == AttachedObject || null == Storyboard)
            {
                return;
            }

            switch (ControlOption)
            {
                case StoryboardControlOption.Stop:
                {
                    Storyboard.Stop();
                    break;
                }

                case StoryboardControlOption.Play:
                {
                    Storyboard.Begin();
                    break;
                }

                case StoryboardControlOption.Pause:
                {
                    Storyboard.Pause();
                    break;
                }

                case StoryboardControlOption.Resume:
                {
                    Storyboard.Resume();
                    break;
                }

                case StoryboardControlOption.SkipToEnd:
                {
                    Storyboard.SkipToFill();
                    break;
                }

                case StoryboardControlOption.Toggle:
                {
                    var state = ClockState.Stopped;

                    try
                    {
                        state = Storyboard.GetCurrentState();
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    if (ClockState.Stopped == state)
                    {
                        isPaused = false;
                        Storyboard.Begin();

                        return;
                    }

                    if (isPaused)
                    {
                        isPaused = false;
                        Storyboard.Resume();

                        return;
                    }

                    isPaused = true;
                    Storyboard.Pause();

                    return;
                }
            }
        }

        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs e)
        {
            isPaused = false;
        }
    }
}