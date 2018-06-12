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

        Dictionary<string, object> _properties = App.Properties;
        Dictionary<string, bool> _propertyFirstTime = new Dictionary<string, bool>();

        public BaseViewModel()
        {
            SaveEnabledDictionary = new Dictionary<string, bool>();

            foreach (string key in _properties.Keys)
            {
                _propertyFirstTime[key] = true;
            }
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


        public bool GetProperty<T>(string propertyName, out T property)
        {
            if (_properties != null && App.Properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                property = (T)o;
                return true;
            }
            else
            {
                property = default(T);
                return false;
            }
        }

        protected int GetProperty(ref int backingStore, [CallerMemberName]string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                int temp = Convert.ToInt32(o);
                backingStore = temp;
                return temp;
            }
            else
                return backingStore;
        }

        protected T GetProperty<T>(ref T backingStore, [CallerMemberName]string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                backingStore = (T)o;
                return (T)o;
            }
            else
                return backingStore;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, bool persist = false, bool forceUpdate = false,
                    [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            bool firstTime;
            if (_propertyFirstTime.ContainsKey(propertyName))
            {
                firstTime = _propertyFirstTime[propertyName];
            }
            else
            {
                firstTime = true;
            }
            // Do not update displayed value if not changed or not first time or not forced
            if (Equals(backingStore, value) && !firstTime && !forceUpdate)
            {                
                return false;
            }
            _propertyFirstTime[propertyName] = false;

            if (persist)
            {
                // store value
                _properties[propertyName] = value;
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
