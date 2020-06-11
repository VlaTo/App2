using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public class PlaySoundAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty SourceProperty;
        public static readonly DependencyProperty VolumeProperty;

        private Popup popup;

        /// <summary>
        /// 
        /// </summary>
        public Uri Source
        {
            get
            {
                return (Uri) GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Volume
        {
            get
            {
                return (Double) GetValue(VolumeProperty);
            }
            set
            {
                SetValue(VolumeProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PlaySoundAction()
            : base(typeof (FrameworkElement))
        {
        }

        static PlaySoundAction()
        {
            SourceProperty = DependencyProperty
                .Register(
                    nameof(Source),
                    typeof (Uri),
                    typeof (PlaySoundAction),
                    new PropertyMetadata(null)
                );
            VolumeProperty = DependencyProperty
                .Register(
                    nameof(Volume),
                    typeof (Double),
                    typeof (PlaySoundAction),
                    new PropertyMetadata(1.0d)
                );
        }

        public PlaySoundAction(Type constraint) : base(constraint)
        {
        }

        protected override void Invoke(object value)
        {
            if (null == Source || null == AttachedObject)
            {
                return;
            }

            popup = new Popup();

            var media = new MediaElement
            {
                Visibility = Visibility.Collapsed
            };

            media.SetBinding(MediaElement.SourceProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Source))
            });
            media.SetBinding(MediaElement.VolumeProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(Volume))
            });
            media.MediaEnded += OnMediaFinished;
            media.MediaFailed += OnMediaFinished;

            popup.Child = media;
            popup.IsOpen = true;
        }

        private void OnMediaFinished(object sender, RoutedEventArgs e)
        {
            if (null == popup)
            {
                return;
            }

            var media = (MediaElement) popup.Child;

            media.ClearValue(MediaElement.SourceProperty);
            media.ClearValue(MediaElement.VolumeProperty);

            popup.Child = null;
            popup.IsOpen = false;
            popup = null;
        }
    }
}