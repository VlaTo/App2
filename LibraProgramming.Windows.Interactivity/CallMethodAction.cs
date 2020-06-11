using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    /// <summary>
    /// 
    /// </summary>
    public class CallMethodAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty MethodNameProperty;
        public static readonly DependencyProperty TargetObjectProperty;

        private readonly ICollection<MethodDescriptor> methods;

        /// <summary>
        /// 
        /// </summary>
        public string MethodName
        {
            get => (string) GetValue(MethodNameProperty);
            set => SetValue(MethodNameProperty, value);
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
        public CallMethodAction()
            : base(typeof (FrameworkElement))
        {
            methods = new Collection<MethodDescriptor>();
        }

        static CallMethodAction()
        {
            MethodNameProperty = DependencyProperty
                .Register(
                    nameof(MethodName),
                    typeof (string),
                    typeof (CallMethodAction),
                    new PropertyMetadata(null, OnMethodNamePropertyChanged)
                );
            TargetObjectProperty = DependencyProperty
                .Register(
                    nameof(TargetObject),
                    typeof (object),
                    typeof (CallMethodAction),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnTargetObjectPropertyChanged)
                );
        }

        protected override void Invoke(object value)
        {
            if (null == AttachedObject)
            {
                return;
            }

            var descriptor = FindBestFitMethodFor(value);

            if (null != descriptor)
            {

                if (false == descriptor.HasArguments)
                {
                    descriptor.Method.Invoke(Target, null);
                    return;
                }

                var arguments = descriptor.Arguments;

                if (2 == arguments.Length && null != AttachedObject && null != value && CanFit(arguments[0].ParameterType, AttachedObject) && CanFit(descriptor.GetEventArgumentType(), value))
                {
                    descriptor.Method.Invoke(Target, new[] { AttachedObject, value });
                }
            }
            else if (null != TargetObject)
            {
                throw new ArgumentException();
            }
        }

        public override void Detach()
        {
            methods.Clear();
            base.Detach();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            RetrieveMethods();
        }

        private MethodDescriptor FindBestFitMethodFor(object arg)
        {
            var exists = null != arg;
            var ati = exists ? arg.GetType().GetTypeInfo() : null;

            return methods.FirstOrDefault(descriptor => false == descriptor.HasArguments || (exists && descriptor.GetEventArgumentType().GetTypeInfo().IsAssignableFrom(ati)));
        }

        private void RetrieveMethods()
        {
            methods.Clear();

            if (null == Target || String.IsNullOrEmpty(MethodName))
            {
                return;
            }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var candidates = new Collection<MethodDescriptor>();
            foreach (var method in Target.GetType().GetMethods(bindingFlags))
            {
                if (false == AreValidMethod(method))
                {
                    continue;
                }

                var parameters = method.GetParameters();

                if (AreParametersValid(parameters))
                {
                    candidates.Add(new MethodDescriptor(method, parameters));
                }
            }

            foreach (var candidate in candidates.OrderByDescending(CalculateMethodWeight))
            {
                methods.Add(candidate);
            }
        }

        private bool AreValidMethod(MethodInfo method)
        {
            if (false == String.Equals(MethodName, method.Name, StringComparison.Ordinal))
            {
                return false;
            }

            return typeof (void) == method.ReturnType;
        }

        private static int CalculateMethodWeight(MethodDescriptor descriptor)
        {
            var nesting = 0;

            if (!descriptor.HasArguments)
            {
                return nesting;
            }

            for (var type = descriptor.GetEventArgumentType();
                type != typeof (EventArgs);
                type = type.GetTypeInfo().BaseType)
            {
                nesting++;
            }

            return nesting + descriptor.ArgumentsCount;
        }

        private static bool AreParametersValid(ParameterInfo[] parameters)
        {
            if (2 == parameters.Length)
            {
                if (typeof (object) != parameters[0].ParameterType)
                {
                    return false;
                }

                //if (false == typeof (EventArgs).GetTypeInfo().IsAssignableFrom(parameters[1].ParameterType.GetTypeInfo()))
                if (false == typeof (EventArgs).IsAssignableFrom(parameters[1].ParameterType))
                {
                    return false;
                }
            }
            else if (0 != parameters.Length)
            {
                return false;
            }

            return true;
        }

        private static bool CanFit(Type parameterType, object value)
            => parameterType.IsInstanceOfType(value);

        private static void OnMethodNamePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
            => ((CallMethodAction) source).RetrieveMethods();

        private static void OnTargetObjectPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
            => ((CallMethodAction) source).RetrieveMethods();

        /// <summary>
        /// 
        /// </summary>
        private class MethodDescriptor
        {
            public MethodInfo Method
            {
                get;
            }

            public ParameterInfo[] Arguments
            {
                get;
            }

            public bool HasArguments => 0 != Arguments.Length;

            public int ArgumentsCount => Arguments.Length;

            public MethodDescriptor(MethodInfo method, ParameterInfo[] args)
            {
                Method = method;
                Arguments = args;
            }

            public Type GetEventArgumentType()
            {
                if (2 <= Arguments.Length)
                {
                    return Arguments[1].ParameterType;
                }

                return null;
            }
        }
    }
}