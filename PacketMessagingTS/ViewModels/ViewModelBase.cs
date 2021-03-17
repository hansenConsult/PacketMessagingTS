using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using Newtonsoft.Json;

using PacketMessagingTS.Core.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class ViewModelBase : ObservableRecipient
    {
        Dictionary<string, bool> SaveEnabledDictionary = new Dictionary<string, bool>();
        Dictionary<string, object> _properties = App.Properties;

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

        protected bool isAppBarSaveEnabled;
        public bool IsAppBarSaveEnabled
        {
            get => isAppBarSaveEnabled;
            set => SetProperty(ref isAppBarSaveEnabled, value);
        }

        protected bool isAppBarSendEnabled = false;
        public virtual bool IsAppBarSendEnabled
        {
            get => isAppBarSendEnabled;
            set => SetProperty(ref isAppBarSendEnabled, value);
        }


        protected int GetProperty(ref int backingStore, [CallerMemberName] string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                //int property = Convert.ToInt32(o);
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
                    string msg = e.Message;
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
                    string msg = e.Message;
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
            return SetProperty(ref backingStore, value, propertyName);
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
