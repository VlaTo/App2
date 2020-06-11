using System;
using System.Reflection;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace LibraProgramming.Windows.Interactivity
{
    public sealed class InvokeCommandAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty TriggerParameterPathProperty;

        private string commandName;

        public ICommand Command
        {
            get
            {
                return (ICommand) GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public string CommandName
        {
            get
            {
                return commandName;
            }
            set
            {
                if (String.Equals(value, commandName))
                {
                    return;
                }

                commandName = value;
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public string TriggerParameterPath
        {
            get
            {
                return (string) GetValue(TriggerParameterPathProperty);
            }
            set
            {
                SetValue(TriggerParameterPathProperty, value);
            }
        }

        public InvokeCommandAction()
            : base(typeof(FrameworkElement))
        {
        }

        static InvokeCommandAction()
        {
            CommandProperty = DependencyProperty
                .Register(
                    nameof(Command),
                    typeof (ICommand),
                    typeof (InvokeCommandAction),
                    null
                );
            CommandParameterProperty = DependencyProperty
                .Register(
                    nameof(CommandParameter),
                    typeof (object),
                    typeof (InvokeCommandAction),
                    null
                );
            TriggerParameterPathProperty = DependencyProperty
                .Register(
                    nameof(TriggerParameterPath),
                    typeof (string),
                    typeof (InvokeCommandAction),
                    null
                );
        }

        protected override void Invoke(object value)
        {
            if (null == AttachedObject)
            {
                return;
            }

            var command = ResolveCommand();
            var parameter = CommandParameter ?? value;

            if (false == String.IsNullOrEmpty(TriggerParameterPath))
            {
                var segments = TriggerParameterPath.Split('.');
                var propertyValue = parameter;

                foreach (var segment in segments)
                {
                    var pi = propertyValue.GetType().GetProperty(segment);
                    propertyValue = pi.GetValue(propertyValue);
                }

                parameter = propertyValue;
            }

            if (null != command && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        private ICommand ResolveCommand()
        {
            var command = Command;

            if (null != command)
            {
                return command;
            }

            if (null != AttachedObject)
            {
                var type = AttachedObject.GetType().GetTypeInfo();
                var property = type.GetDeclaredProperty(CommandName);

                if (null != property && typeof(ICommand).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo()))
                {
                    command = (ICommand) property.GetValue(AttachedObject);
                }
            }

            return command;
        }
    }
}
