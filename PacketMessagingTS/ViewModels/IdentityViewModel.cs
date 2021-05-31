using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;

using SharedCode.Models;


namespace PacketMessagingTS.ViewModels
{
    public class IdentityViewModel : ViewModelBase
    {
        //public static TacticalCall _callsignData;
        //public TacticalCallsignData _tacticalCallsignData;

        public static IdentityViewModel Instance { get; } = new IdentityViewModel();

        public IdentityViewModel()
        {

        }

        //public ObservableCollection<UserCallSign> UserCallsignsSource
        //{
        //    get => new ObservableCollection<UserCallSign>(UserCallsigns.Instance.UserCallsignsList);
        //}

        //private int userCallsignSelectedIndex;
        //public int UserCallsignSelectedIndex
        //{
        //    get => GetProperty(ref userCallsignSelectedIndex);
        //    set
        //    {
        //        SetProperty(ref userCallsignSelectedIndex, value, true);
        //        UserCallsign = UserCallsignsSource[userCallsignSelectedIndex].UserCallsign;
        //        UserName = UserCallsignsSource[userCallsignSelectedIndex].UserName;
        //        UserCity = UserCallsignsSource[userCallsignSelectedIndex].UserCity;
        //    }
        //}
        private void GetAddressBookEntry(string userCallsign)
        {
            if (string.IsNullOrEmpty(userCallsign))
                return;

            if (AddressBook.Instance.AddressBookDictionary.TryGetValue(userCallsign, out AddressBookEntry entry))
            {
                UserName = entry.NameDetail;
                UserCity = entry.City;
            }
            if (string.IsNullOrEmpty(entry?.Prefix) && userCallsign.Length > 3)
            {
                UserMsgPrefix = userCallsign.Substring(userCallsign.Length - 3, 3);
            }
            else
            {
                UserMsgPrefix = entry?.Prefix;
            }
        }

        private string _userCallsign;
        public string UserCallsign
        {
            get
            {
                if (string.IsNullOrEmpty(_userCallsign))
                {
                    GetProperty(ref _userCallsign);
                    GetAddressBookEntry(_userCallsign);
                }
                return _userCallsign;
            }
            set
            {
                if (SetPropertyPrivate(ref _userCallsign, value, true))
                {
                    if (!string.IsNullOrEmpty(_userCallsign))
                    {
                        Utilities.SetApplicationTitle();
                    }
                }

                GetAddressBookEntry(_userCallsign);
            }
        }

        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                if (!string.IsNullOrEmpty(_userName))
                {
                    string userFirstName = _userName;
                    int index = userFirstName.IndexOf(' ');
                    if (index < 0 && userFirstName.Length > 0)
                    {
                        index = userFirstName.Length;
                    }
                    if (index > 0)
                    {
                        UserFirstName = userFirstName.Substring(0, index);
                    }
                    else
                    {
                        UserFirstName = "";
                    }
                }
            }
        }

        string _userFirstName;
        public string UserFirstName
        {
            get => _userFirstName;
            set => SetProperty(ref _userFirstName, value);
        }

        string _userCity;
        public string UserCity
        {
            get => _userCity;
            set => SetProperty(ref _userCity, value);
        }

        string _userPrefix;
        public string UserMsgPrefix
        {
            get => _userPrefix;
            set => SetProperty(ref _userPrefix, value);
        }

        private bool _useTacticalCallsign;
        public bool UseTacticalCallsign
        {
            get
            {
                bool temp = _useTacticalCallsign;
                if (temp != GetProperty(ref _useTacticalCallsign))
                {
                    SetProperty(ref _useTacticalCallsign, _useTacticalCallsign);
                }
                TacticalCallsignAreaEnabled = _useTacticalCallsign;
                TacticalCallsignEnabled = _useTacticalCallsign;
                AdditionalTextEnabled = _useTacticalCallsign;
                TacticalPrefixEnabled = _useTacticalCallsign;
                return _useTacticalCallsign;
            }
            set
            {
                if (SetPropertyPrivate(ref _useTacticalCallsign, value, true))
                {
                    Utilities.SetApplicationTitle();
                }
            }
        }

        private bool _tacticalCallsignAreaEnabled;
        public bool TacticalCallsignAreaEnabled
        {
            get => _tacticalCallsignAreaEnabled;
            set => SetProperty(ref _tacticalCallsignAreaEnabled, value);
        }
        
        private bool _tacticalCallsignEnabled;
        public bool TacticalCallsignEnabled
        {
            get => _tacticalCallsignEnabled;
            set => SetProperty(ref _tacticalCallsignEnabled, value);
        }
        
        private bool _additionalTextEnabled;
        public bool AdditionalTextEnabled
        {
            get => _additionalTextEnabled;
            set => SetProperty(ref _additionalTextEnabled, value);
        }
        
        private bool _tacticalPrefixEnabled;
        public bool TacticalPrefixEnabled
        {
            get => _tacticalPrefixEnabled;
            set => SetProperty(ref _tacticalPrefixEnabled, value);
        }

        public ObservableCollection<TacticalCallsignData> TacticalCallsignsAreaSource
        {
            get => new ObservableCollection<TacticalCallsignData>(TacticalCallsigns.TacticalCallsignDataList);
        }

        private ObservableCollection<TacticalCall> _tacticalCallsignsSource;
        public ObservableCollection<TacticalCall> TacticalCallsignsSource
        {
            get => _tacticalCallsignsSource;
            set
            {
                SetProperty(ref _tacticalCallsignsSource, value);
                TacticalCallsignSelectedIndex = TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex];
            }
        }

        private int _tacticalCallsignAreaSelectedIndex;
        public int TacticalCallsignAreaSelectedIndex
        {
            get => GetProperty(ref _tacticalCallsignAreaSelectedIndex);
            set
            {
                SetPropertyPrivate(ref _tacticalCallsignAreaSelectedIndex, value, true);
                if (TacticalCallsignsAreaSource[value].TacticalCallsigns != null)
                {
                    TacticalCallsignsSource = new ObservableCollection<TacticalCall>(TacticalCallsignsAreaSource[value].TacticalCallsigns?.TacticalCallsignsArray);
                }
            }
        }

        //private int[] tacticalSelectedIndexArrayBackingStorage;
        private int[] _tacticalSelectedIndexArray;
        public int[] TacticalSelectedIndexArray
        {
            //get => tacticalSelectedIndexArray);
            get
            {
                if (_tacticalSelectedIndexArray is null)
                {
                    //return GetProperty(ref tacticalSelectedIndexArray);
                    if (GetProperty(ref _tacticalSelectedIndexArray) is null)
                    {
                        _tacticalSelectedIndexArray = new int[TacticalCallsigns.TacticalCallsignDataList.Count];
                        for (int i = 0; i < _tacticalSelectedIndexArray.Length; i++)
                        {
                            _tacticalSelectedIndexArray[i] = 0;
                        }
                    }
                }
                return _tacticalSelectedIndexArray;
            }
            set => SetPropertyPrivate(ref _tacticalSelectedIndexArray, value, true);
        }

        private int _tacticalCallsignSelectedIndex;
        public int TacticalCallsignSelectedIndex
        {
            get
            {
                if (_tacticalCallsignSelectedIndex < 0)
                {
                    return TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex];
                }
                else
                {
                    //TacticalCallsign = TacticalCallsignOther;
                    return _tacticalCallsignSelectedIndex;
                }
            }
            set
            {
                // Checked on startup
                //if (value >= TacticalCallsignsSource.Count)
                //{
                //    TacticalCallsignSelectedIndex = 0;
                //    //return;
                //}
                if (value < TacticalCallsignsSource.Count && value >= 0)
                {
                    bool propertyChanged = SetProperty(ref _tacticalCallsignSelectedIndex, value);
                    if (propertyChanged)
                    {
                        int[] temp = new int[TacticalSelectedIndexArray.Length];
                        TacticalSelectedIndexArray.CopyTo(temp, 0);
                        temp[TacticalCallsignAreaSelectedIndex] = value;
                        //TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex] = value;
                        //TacticalSelectedIndexArray = TacticalSelectedIndexArray;
                        TacticalSelectedIndexArray = temp;
                        //SetProperty(ref tacticalCallsignSelectedIndex, value);
                        TacticalCallsign = TacticalCallsignsSource[_tacticalCallsignSelectedIndex].TacticalCallsign;
                        //TacticalCallsignOther = TacticalCallsign;
                        TacticalAgencyNameSelectedIndex = _tacticalCallsignSelectedIndex;
                        TacticalMsgPrefix = TacticalCallsignsSource[_tacticalCallsignSelectedIndex].Prefix;
                        if (UseTacticalCallsign)
                        {
                            TacticalCallsignsAreaSource[TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArraySelectedIndex = _tacticalCallsignSelectedIndex;
                        }
                        //else
                        //{
                        //    TacticalCallsignsAreaSource[TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArraySelectedIndex = -1;
                        //}
                    }
                }
                else
                {

                }
            }
        }

        private string _tacticalCallsignOther = "";
        public string TacticalCallsignOther
        {
            get => _tacticalCallsignOther;
            set
            {
                //SetProperty(ref tacticalCallsignOther, value);

                // if not in list add to Source
                foreach (TacticalCall tacticalCall in TacticalCallsignsSource)
                {
                    if (value.Length < 6 || tacticalCall.TacticalCallsign == value)
                    {
                        return;
                    }
                }

                SetProperty(ref _tacticalCallsignOther, value);

                if (_tacticalCallsignOther.Length == 6)
                {
                    TacticalCallsign = _tacticalCallsignOther;
                    TacticalCall tacticalCall = new TacticalCall()
                    {
                        TacticalCallsign = _tacticalCallsignOther,
                        AgencyName = "",
                        Prefix = _tacticalCallsignOther.Substring(_tacticalCallsignOther.Length - 3),
                    };
                    TacticalCallsignsSource.Add(tacticalCall);
                }
            }
        }

        //private TacticalCall selectedTacticalCallArea;
        //public TacticalCall SelectedTacticalCallArea
        //{
        //    get => selectedTacticalCallArea;
        //    set
        //    {
        //        SetProperty(ref selectedTacticalCallArea, value);
        //    }
        //}

        //private TacticalCall selectedTacticalCall;
        //public TacticalCall SelectedTacticalCall
        //{
        //    get => selectedTacticalCall;
        //    set
        //    {
        //        SetProperty(ref selectedTacticalCall, value);

        //        //TacticalCallsign = selectedTacticalCall.TacticalCallsign;
        //        TacticalMsgPrefix = selectedTacticalCall.Prefix;
        //        TacticalAgencyName = selectedTacticalCall.AgencyName;
        //    }

        //}

        private string _tacticalCallsign;
        public string TacticalCallsign
        {
            get
            {
                if (TacticalCallsignsSource is null)
                {
                    TacticalCallsignAreaSelectedIndex = Utilities.GetProperty("TacticalCallsignAreaSelectedIndex");
                }
                return _tacticalCallsign;
            }
            set
            {
                if (SetProperty(ref _tacticalCallsign, value))
                {
                    Utilities.SetApplicationTitle();
                }
            }
        }

        private int _tacticalAgencyNameSelectedIndex;
        public int TacticalAgencyNameSelectedIndex
        {
            get => _tacticalAgencyNameSelectedIndex;
            set
            {
                SetProperty(ref _tacticalAgencyNameSelectedIndex, value);
                //if (SetProperty(ref tacticalAgencyNameSelectedIndex, value))
                //{
                //    TacticalCallsignSelectedIndex = tacticalAgencyNameSelectedIndex;
                //}
            }
        }

        private string _tacticalAgencyName;
        public string TacticalAgencyName
        {
            get => TacticalCallsignsSource[_tacticalCallsignSelectedIndex].AgencyName;
            set
            {
                SetProperty(ref _tacticalAgencyName, value);
                TacticalCallsignSelectedIndex = _tacticalAgencyNameSelectedIndex;
            }
        }

        private string _tacticalPrefix;
        public string TacticalMsgPrefix
        {
            get => _tacticalPrefix;
            set => SetProperty(ref _tacticalPrefix, value);
        }

        private bool _displayIdentityAtStartup;
        public bool DisplayIdentityAtStartup
        {
            get => GetProperty(ref _displayIdentityAtStartup);
            set => SetPropertyPrivate(ref _displayIdentityAtStartup, value, true);
        }
    }
}
