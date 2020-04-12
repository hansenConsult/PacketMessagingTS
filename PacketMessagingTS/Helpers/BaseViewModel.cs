using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using PacketMessagingTS.Models;

using SharedCode.Models;


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

        protected bool isAppBarSaveEnabled;
        public bool IsAppBarSaveEnabled
        {
            get => isAppBarSaveEnabled;
            set => SetProperty(ref isAppBarSaveEnabled, value);
        }

        protected bool isAppBarSendEnabled = false;
        public bool IsAppBarSendEnabled
        {
            get => isAppBarSendEnabled;
            set => SetProperty(ref isAppBarSendEnabled, value);
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
                try
                {
                    // Retrieve value from dictionary
                    object o = _properties[propertyName];
                    backingStore = (T)o;
                    return (T)o;
                }
                catch
                {
                    return backingStore;
                }
            }
            else
                return backingStore;
        }

        protected int[] GetProperty(ref int[] backingStore, [CallerMemberName]string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                try
                {
                    // Retrieve value from dictionary
                    var o = _properties[propertyName];
                    var intArray = JsonConvert.DeserializeObject<int[]>(o.ToString());
                    backingStore = intArray;
                    return intArray;
                }
                catch
                {
                    return backingStore;
                }
            }
            else
            {
                backingStore = new int[TacticalCallsigns._TacticalCallsignDataList.Count];
                for (int i = 0; i < backingStore.Length; i++)
                {
                    backingStore[i] = 0;
                }
                return backingStore;
            }
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

        private bool Equals(ObservableCollection<EmailAccount> collection1, ObservableCollection<EmailAccount> collection2)
        {
            if (collection1?.Count != collection2.Count)
                return false;

            bool equal = false;
            foreach (EmailAccount account1 in collection1)
            {
                foreach (EmailAccount account2 in collection2)
                {
                    //if (account1.MailUserName == account2.MailUserName)
                    //{
                        equal = EmailAccount.IsEMailAccountsEqual(account1, account2);
                        if (!equal)
                            return false;
                    //}
                }
            }
            return equal;
        }

        protected void Set(ref ObservableCollection<EmailAccount> storage, ObservableCollection<EmailAccount> value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private bool Equals(ObservableCollection<TNCDevice> collection1, ObservableCollection<TNCDevice> collection2)
        {
            if (collection1?.Count != collection2.Count)
                return false;

            bool equal = false;
            foreach (TNCDevice dev1 in collection1)
            {
                foreach (TNCDevice dev2 in collection2)
                {
                    if (dev1.Name == dev2.Name)
                    {
                        equal = dev1.MailUserName == dev2.MailUserName;
                        if (!equal)
                            return false;
                        equal = TNCDevicePrompts.IsTNCDevicePromptsEqual(dev1.Prompts, dev2.Prompts);
                        if (!equal)
                            return false;
                        equal = TNCDeviceCommands.IsTNCDeviceCommandsEqual(dev1.Commands, dev2.Commands);
                        if (!equal)
                            return false;
                        equal = TNCDeviceInitCommands.IsTNCDeviceInitCommandsEqual(dev1.InitCommands, dev2.InitCommands);
                        if (!equal)
                            return false;
                        equal = TNCDeviceCommPort.IsTNCDeviceComportsEqual(dev1.CommPort, dev2.CommPort);
                        if (!equal)
                            return false;
                    }
                }
            }
            return equal;
        }

        protected void Set(ref ObservableCollection<TNCDevice> storage, ObservableCollection<TNCDevice> value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
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
