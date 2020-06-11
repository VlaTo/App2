using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace App2
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetValue<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            var comparer = EqualityComparer<TValue>.Default;

            if (comparer.Equals(field, value))
            {
                return false;
            }

            field = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            DoPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        private void DoPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
    }
}