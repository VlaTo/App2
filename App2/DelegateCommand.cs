using System;
using System.Windows.Input;

namespace App2
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly Predicate<object> condition;
        private bool canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> action, Predicate<object> condition = null)
        {
            if (null == action)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.action = action;
            this.condition = condition ?? Condition<object>.True;

            canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            var value = condition.Invoke(parameter);

            if (value != canExecute)
            {
                canExecute = value;

                DoCanExecuteChanged(EventArgs.Empty);
            }

            return value;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                action.Invoke(parameter);
            }
        }

        private void DoCanExecuteChanged(EventArgs e)
        {
            var handler = CanExecuteChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
    }
}