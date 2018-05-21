using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace PacketMessagingTS.Helpers
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        Dictionary<string, bool> SaveEnabledDictionary;

        Dictionary<string, object> properties = App.Properties;

        public BaseViewModel()
        {
            SaveEnabledDictionary = new Dictionary<string, bool>();
        }

        public virtual void ResetChangedProperty()
        {
            string[] keyArray = new string[SaveEnabledDictionary.Count];

            int i = 0;
            foreach (string key in SaveEnabledDictionary.Keys)
            {
                keyArray[i++] = key;
            }
            for (i = 0; i < SaveEnabledDictionary.Count; i++)
            {
                SaveEnabledDictionary[keyArray[i]] = false;
            }
        }

        protected bool SaveEnabled(bool propertyChanged, [CallerMemberName]string propertyName = "")
        {
            SaveEnabledDictionary[propertyName] = propertyChanged;
            bool saveEnabled = false;
            foreach (bool value in SaveEnabledDictionary.Values)
            {
                saveEnabled |= value;
            }
            return saveEnabled;
        }

        protected bool isAppBarSaveEnabled = false;
        public bool IsAppBarSaveEnabled
        {
            get => isAppBarSaveEnabled;
            set => SetProperty(ref isAppBarSaveEnabled, value);
        }

        protected T GetProperty<T>(ref T backingStore, [CallerMemberName]string propertyName = "")
        {
            if (properties != null && properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = properties[propertyName];
                //if (o.GetType() == typeof(Int64))
                //{
                //    int retval = Convert.ToInt32(o);
                //    return retval;
                //}
                //else
                //{
                    return (T)o;
                //}
            }
            else
                return backingStore;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, bool persist = false,
                    [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (Equals(backingStore, value))
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

        //protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        //{
        //    if (Equals(storage, value))
        //    {
        //        return;
        //    }

        //    storage = value;
        //    OnPropertyChanged(propertyName);
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
