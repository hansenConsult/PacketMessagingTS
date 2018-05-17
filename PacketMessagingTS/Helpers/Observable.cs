using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace PacketMessagingTS.Helpers
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();  //Application.Current.Properties;


        protected T GetProperty<T>(ref T backingStore, [CallerMemberName]string propertyName = "")
        {
            if (properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = properties[propertyName];
                return (T)o;
            }
            else
                return backingStore;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, bool persist = false,
                    [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            if (persist)
            {
                // store value
                properties[propertyName] = value;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
