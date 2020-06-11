using System;
using System.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public class OrientationToStateAction : TargetTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty UseTransitionsProperty;
        public static readonly DependencyProperty FullScreenLandscapeStateNameProperty;
        public static readonly DependencyProperty FullScreenPortraitStateNameProperty;
        public static readonly DependencyProperty FilledStateNameProperty;
        public static readonly DependencyProperty SnappedStateNameProperty;

        public bool UseTransitions
        {
            get
            {
                return (bool) GetValue(UseTransitionsProperty);
            }
            set
            {
                SetValue(UseTransitionsProperty, value);
            }
        }

        public string FullScreenLandscapeStateName
        {
            get
            {
                return (string) GetValue(FullScreenLandscapeStateNameProperty);
            }
            set
            {
                SetValue(FullScreenLandscapeStateNameProperty, value);
            }
        }

        public string FullScreenPortraitStateName
        {
            get
            {
                return (string) GetValue(FullScreenPortraitStateNameProperty);
            }
            set
            {
                SetValue(FullScreenPortraitStateNameProperty, value);
            }
        }

        public string FilledStateName
        {
            get
            {
                return (string) GetValue(FilledStateNameProperty);
            }
            set
            {
                SetValue(FilledStateNameProperty, value);
            }
        }

        public string SnappedStateName
        {
            get
            {
                return (string) GetValue(SnappedStateNameProperty);
            }
            set
            {
                SetValue(SnappedStateNameProperty, value);
            }
        }

        private bool IsTargetObjectSet => (ReadLocalValue(TargetObjectProperty) != DependencyProperty.UnsetValue);

        private FrameworkElement StateTarget
        {
            get;
            set;
        }

        static OrientationToStateAction()
        {
            UseTransitionsProperty = DependencyProperty
                .Register(
                    nameof(UseTransitions),
                    typeof(bool),
                    typeof(OrientationToStateAction),
                    new PropertyMetadata(true)
                );
            FullScreenLandscapeStateNameProperty = DependencyProperty
                .Register(
                    nameof(FullScreenLandscapeStateName),
                    typeof(string),
                    typeof(OrientationToStateAction),
                    new PropertyMetadata(null)
                );
            FullScreenPortraitStateNameProperty = DependencyProperty
                .Register(
                    nameof(FullScreenPortraitStateName),
                    typeof(string),
                    typeof(OrientationToStateAction),
                    new PropertyMetadata(null)
                );
            FilledStateNameProperty = DependencyProperty
                .Register(
                    nameof(FilledStateName),
                    typeof(string),
                    typeof(OrientationToStateAction),
                    new PropertyMetadata(null)
                );
            SnappedStateNameProperty = DependencyProperty
                .Register(
                    nameof(SnappedStateName),
                    typeof(string),
                    typeof(OrientationToStateAction),
                    new PropertyMetadata(null)
                );
        }

        protected override void Invoke(object parameter)
        {
            if (null != StateTarget && Enum.IsDefined(typeof (ApplicationViewState), parameter))
            {
                string stateName;
                var viewState = (ApplicationViewState) parameter;

                switch (viewState)
                {
                    case ApplicationViewState.FullScreenLandscape:
                        stateName = FullScreenLandscapeStateName;
                        break;

                    case ApplicationViewState.FullScreenPortrait:
                        stateName = FullScreenPortraitStateName;
                        break;

                    case ApplicationViewState.Filled:
                        stateName = FilledStateName;
                        break;

                    case ApplicationViewState.Snapped:
                        //if (Window.Current.Bounds.Left > 0)
                        //    Debug.WriteLine("Right side");
                        //else
                        //    Debug.WriteLine("Left side");
                        stateName = SnappedStateName;
                        break;

                    default:
                        stateName = string.Empty;
                        break;
                }

                if (string.IsNullOrEmpty(stateName))
                {
                    stateName = viewState.ToString();
                }

                VisualStateUtilities.GoToState(StateTarget, stateName, UseTransitions);

            }
        }

        protected override void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            base.OnTargetChanged(oldTarget, newTarget);

            FrameworkElement resolvedControl;

            if (String.IsNullOrEmpty(TargetName) && !IsTargetObjectSet)
            {
                if (!VisualStateUtilities.TryFindNearestStatefulControl(AttachedObject, out resolvedControl) && (null != resolvedControl))
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        "Target {0} does not define any VisualStateGroups.",
                        new object[]
                        {
                            resolvedControl.Name
                        }));
                }
            }
            else
            {
                resolvedControl = Target;
            }

            StateTarget = resolvedControl;
        }
    }
}
