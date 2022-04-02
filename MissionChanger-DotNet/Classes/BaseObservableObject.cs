using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MissionChanger.Classes
{

    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual object OldValue { get; private set; }
        public virtual object NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class BaseObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected void OnPropertyChanged(string propertyName, object oldValue, object newValue)
            => PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs(propertyName, oldValue, newValue));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return SetField2(ref field, value, null, propertyName);
        }

        protected bool SetField2<T>(ref T field, T value, string secondaryPropertyName, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            object oldVal = field;
            field = value;

            OnPropertyChanged(propertyName, oldVal, value);

            if (secondaryPropertyName != null)
            {
                if (secondaryPropertyName.Length == 0)
                    OnPropertyChanged(null);

                OnPropertyChanged(secondaryPropertyName);
            }
            if (secondaryPropertyName?.Length == 0)
                OnPropertyChanged(null);

            return true;
        }

        internal void NotifyPropertyChanges()
        {
            OnPropertyChanged(null);
        }
    }
}
