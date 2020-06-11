using System;
using System.Reflection;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangePropertyAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty DurationProperty;
        public static readonly DependencyProperty EasingFunctionProperty;
        public static readonly DependencyProperty IsIncrementingProperty;
        public static readonly DependencyProperty PropertyNameProperty;
        public static readonly DependencyProperty TargetObjectProperty;
        public static readonly DependencyProperty ValueProperty;

        /// <summary>
        /// 
        /// </summary>
        public Duration Duration
        {
            get => (Duration) GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get => (EasingFunctionBase) GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsIncrementing
        {
            get => (bool) GetValue(IsIncrementingProperty);
            set => SetValue(IsIncrementingProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get => (string) GetValue(PropertyNameProperty);
            set => SetValue(PropertyNameProperty, value);
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
        public object TargetObject
        {
            get => GetValue(TargetObjectProperty);
            set => SetValue(TargetObjectProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        internal object Target => TargetObject ?? AttachedObject;

        /// <summary>
        /// 
        /// </summary>
        public ChangePropertyAction()
            : base(typeof (FrameworkElement))
        {
        }

        static ChangePropertyAction()
        {
            DurationProperty = DependencyProperty
                .Register(
                    nameof(Duration),
                    typeof (Duration),
                    typeof (ChangePropertyAction),
                    new PropertyMetadata(Duration.Automatic)
                );
            EasingFunctionProperty = DependencyProperty
                .Register(
                    nameof(EasingFunction),
                    typeof (EasingFunctionBase),
                    typeof (ChangePropertyAction),
                    new PropertyMetadata(null)
                );
            IsIncrementingProperty = DependencyProperty
                .Register(
                    nameof(IsIncrementing),
                    typeof (bool),
                    typeof (ChangePropertyAction),
                    new PropertyMetadata(false)
                );
            TargetObjectProperty = DependencyProperty
                .Register(
                    nameof(TargetObject),
                    typeof (object),
                    typeof (CallMethodAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
            PropertyNameProperty = DependencyProperty
                .Register(
                    nameof(PropertyName),
                    typeof (string),
                    typeof (ChangePropertyAction),
                    new PropertyMetadata(null)
                );
            ValueProperty = DependencyProperty
                .Register(
                    nameof(Value),
                    typeof (object),
                    typeof (ChangePropertyAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue)
                );
        }

        protected override void Invoke(object notused)
        {
            if (null == AttachedObject || String.IsNullOrEmpty(PropertyName) || null == Target)
            {
                return;
            }

            var type = Target.GetType();
            var property = type.GetRuntimeProperty(PropertyName);

            EnsureProperty(property);

            var value = Value;
            Exception cause = null;

            try
            {
                if (Duration.HasTimeSpan)
                {
                    EnsureAnimationPossible(type);

                    var current = GetCurrentPropertyValue(Target, property);

                    AnimatePropertyChange(property, current, value);
                }
                else
                {
                    if (IsIncrementing)
                    {
                        value = GetIncrementedValue(property);
                    }

                    property.SetValue(Target, Convert.ChangeType(value, property.PropertyType));
                }
            }
            catch (FormatException exception)
            {
                cause = exception;
            }
            catch (ArgumentException exception)
            {
                cause = exception;
            }

            if (null != cause)
            {
                throw new ArgumentException("", cause);
            }
        }

        private object GetIncrementedValue(PropertyInfo property)
        {
            if (false == property.CanRead)
            {
                throw new ArgumentException();
            }

            var current = property.GetValue(Target);
            var value = Value;

            if (null == value || null == current)
            {
                return null;
            }

            if (typeof (Double).IsAssignableFrom(property.PropertyType))
            {
                return Convert.ToDouble(current) + Convert.ToDouble(value);
            }

            if (typeof (int).IsAssignableFrom(property.PropertyType))
            {
                return Convert.ToInt32(current) + Convert.ToInt32(value);
            }

            if (typeof (float).IsAssignableFrom(property.PropertyType))
            {
                return Convert.ToSingle(current) + Convert.ToSingle(value);
            }

            if (typeof (String).IsAssignableFrom(property.PropertyType))
            {
                return new StringBuilder(Convert.ToString(current)).Append(Convert.ToString(value)).ToString();
            }

            return TryAdding(current, value);
        }

        private Timeline CreateDoubleAnimation(double from, double to)
        {
            return new DoubleAnimation
            {
                From = from,
                To = to,
                EasingFunction = EasingFunction
            };
        }

        private Timeline CreateColorAnimation(Color from, Color to)
        {
            return new ColorAnimation
            {
                From = from,
                To = to,
                EasingFunction = EasingFunction
            };
        }

        private Timeline CreatePointAnimation(Point from, Point to)
        {
            return new PointAnimation
            {
                From = from,
                To = to,
                EasingFunction = EasingFunction
            };
        }

        private Timeline CreateKeyframeAnimation(object from, object to)
        {
            var animation = new ObjectAnimationUsingKeyFrames();

            animation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
                Value = from
            });
            animation.KeyFrames.Add(new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(Duration.TimeSpan),
                Value = to
            });

            return animation;
        }

        private void AnimatePropertyChange(PropertyInfo property, object current, object value)
        {
            var storyboard = new Storyboard();
            Timeline timeline;

            if (typeof (Double).IsAssignableFrom(property.PropertyType))
            {
                timeline = CreateDoubleAnimation((double) current, (double) value);
            }
            else if (typeof (Color).IsAssignableFrom(property.PropertyType))
            {
                timeline = CreateColorAnimation((Color) current, (Color) value);
            }
            else if (typeof (Point).IsAssignableFrom(property.PropertyType))
            {
                timeline = CreatePointAnimation((Point) current, (Point) value);
            }
            else
            {
                timeline = CreateKeyframeAnimation(current, value);
            }

            timeline.Duration = Duration;
            storyboard.Children.Add(timeline);

            Storyboard.SetTarget(storyboard, (DependencyObject) Target);
            Storyboard.SetTargetProperty(storyboard, property.Name);

            storyboard.FillBehavior = FillBehavior.Stop;
            storyboard.Completed += (sender, e) => property.SetValue(Target, value, new object[0]);
            storyboard.Begin();
        }

        private object GetCurrentPropertyValue(object target, PropertyInfo property)
        {
            var element = target as FrameworkElement;
            //target.GetType()
            var temp = property.GetValue(target);

            if (null == element || (false == String.Equals(nameof(element.Width), property.Name) && false == String.Equals(nameof(element.Height), property.Name)))
            {
                return temp;
            }

            if (false == Double.IsNaN((double) temp))
            {
                return temp;
            }

            return String.Equals(nameof(element.Width), property.Name) ? element.ActualWidth : element.ActualHeight;
        }

        private object TryAdding(object current, object value)
        {
//            var typeinfo = value.GetType().GetTypeInfo();
            var type = current.GetType();
            MethodInfo candidate = null;

            foreach (var method in type.GetRuntimeMethods())
            {
                if (false == String.Equals("op_Addition", method.Name, StringComparison.Ordinal))
                {
                    continue;
                }

                var arguments = method.GetParameters();

                if (arguments.Length >= 2 &&
                    arguments[0].ParameterType.IsAssignableFrom(type) &&
                    arguments[1].ParameterType.IsInstanceOfType(value))
                {
                    candidate = method;
                    break;
                }
            }

            if (null != candidate)
            {
                return candidate.Invoke(null, new[] { current, value });
            }

            return value;
        }

        private void EnsureAnimationPossible(Type targetType)
        {
            if (IsIncrementing)
            {
                throw new InvalidOperationException();
            }

            if (false == typeof (DependencyObject).IsAssignableFrom(targetType))
            {
                throw new ArgumentException();
            }
        }

        private static void EnsureProperty(PropertyInfo property)
        {
            if (null == property)
            {
                throw new ArgumentException("");
            }

            if (false == property.CanWrite)
            {
                throw new ArgumentException();
            }
        }
    }
}