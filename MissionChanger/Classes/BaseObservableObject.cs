using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MissionChanger.Classes
{
    public class BaseObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return SetField2(ref field, value, null, propertyName);
        }

        protected bool SetField2<T>(ref T field, T value, string secondaryPropertyName, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);

            if (secondaryPropertyName != null)
                OnPropertyChanged(secondaryPropertyName);

            return true;
        }

        public void NotifyPropertyChanges()
        {
            OnPropertyChanged(null);
        }
    }
}
