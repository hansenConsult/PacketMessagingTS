using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using MetroLog;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using Newtonsoft.Json;

using SharedCode;

namespace PacketMessagingTS.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<ViewModelBase>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        private readonly Dictionary<string, bool> SaveEnabledDictionary = new Dictionary<string, bool>();
        private readonly Dictionary<string, object> _properties = App.Properties;

        protected bool SaveEnabled(bool propertyChanged, [CallerMemberName] string propertyName = "")
        {
            SaveEnabledDictionary[propertyName] = propertyChanged;
            bool saveEnabled = false;
            foreach (bool value in SaveEnabledDictionary.Values)
            {
                saveEnabled |= value;
            }
            return saveEnabled;
        }

        protected bool _isAppBarSaveEnabled;
        public bool IsAppBarSaveEnabled
        {
            get => _isAppBarSaveEnabled;
            set => SetProperty(ref _isAppBarSaveEnabled, value);
        }

        protected bool _isAppBarSendEnabled = false;
        public virtual bool IsAppBarSendEnabled
        {
            get => _isAppBarSendEnabled;
            set => SetProperty(ref _isAppBarSendEnabled, value);
        }


        protected int GetProperty(ref int backingStore, [CallerMemberName] string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                backingStore = Convert.ToInt32(o);
            }
            return backingStore;
        }

        protected int[] GetProperty(ref int[] backingStore, [CallerMemberName] string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                try
                {
                    var o = _properties[propertyName];
                    backingStore = JsonConvert.DeserializeObject<int[]>(_properties[propertyName].ToString());
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Error, e.Message);
                    return backingStore;
                }
            }
            return backingStore;
        }

        protected T GetProperty<T>(ref T backingStore, [CallerMemberName] string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                try
                {
                    object o = _properties[propertyName];
                    backingStore = (T)o;
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Error, e.Message);
                    return backingStore;
                }
            }
            return backingStore;
        }

        protected bool SetPropertyPrivate<T>(ref T backingStore, T value, bool persist = false, [CallerMemberName] string propertyName = "")
        {
            if (persist)
            {
                _properties[propertyName] = value;
            }
            return SetProperty(ref backingStore, value);
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

    }
}
