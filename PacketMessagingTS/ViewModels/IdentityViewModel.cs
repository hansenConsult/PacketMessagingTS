using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

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

        string userCallsign;
        public string UserCallsign
        {
            get
            {
                if (string.IsNullOrEmpty(userCallsign))
                {
                    GetProperty(ref userCallsign);
                    GetAddressBookEntry(userCallsign);
                }
                return userCallsign;
            }
            set
            {
                if (SetPropertyPrivate(ref userCallsign, value, true))
                {
                    if (!string.IsNullOrEmpty(userCallsign))
                    {
                        Utilities.SetApplicationTitle();
                    }
                }

                GetAddressBookEntry(userCallsign);
            }
        }

        string userName;
        public string UserName
        {
            get => userName;
            set
            {
                SetProperty(ref userName, value);
                if (!string.IsNullOrEmpty(userName))
                {
                    string userFirstName = userName;
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

        string userFirstName;
        public string UserFirstName
        {
            get => userFirstName;
            set => SetProperty(ref userFirstName, value);
        }

        string userCity;
        public string UserCity
        {
            get => userCity;
            set => SetProperty(ref userCity, value);
        }

        string userPrefix;
        public string UserMsgPrefix
        {
            get => userPrefix;
            set => SetProperty(ref userPrefix, value);
        }

        private bool useTacticalCallsign;
        public bool UseTacticalCallsign
        {
            get => GetProperty(ref useTacticalCallsign);
            set
            {
                if (SetPropertyPrivate(ref useTacticalCallsign, value, true))
                {
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public ObservableCollection<TacticalCallsignData> TacticalCallsignsAreaSource
        {
            get => new ObservableCollection<TacticalCallsignData>(TacticalCallsigns._TacticalCallsignDataList);
        }

        private ObservableCollection<TacticalCall> tacticalCallsignsSource;
        public ObservableCollection<TacticalCall> TacticalCallsignsSource
        {
            get => tacticalCallsignsSource;
            set
            {
                SetProperty(ref tacticalCallsignsSource, value);
                TacticalCallsignSelectedIndex = TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex];
            }
        }

        private int tacticalCallsignAreaSelectedIndex;
        public int TacticalCallsignAreaSelectedIndex
        {
            get => GetProperty(ref tacticalCallsignAreaSelectedIndex);
            set
            {
                SetPropertyPrivate(ref tacticalCallsignAreaSelectedIndex, value, true);
                TacticalCallsignsSource = new ObservableCollection<TacticalCall>(TacticalCallsignsAreaSource[value].TacticalCallsigns?.TacticalCallsignsArray);
            }
        }

        //private int[] tacticalSelectedIndexArrayBackingStorage;
        private int[] tacticalSelectedIndexArray;
        public int[] TacticalSelectedIndexArray
        {
            //get => tacticalSelectedIndexArray);
            get
            {
                if (tacticalSelectedIndexArray is null)
                {
                    //return GetProperty(ref tacticalSelectedIndexArray);
                    if (GetProperty(ref tacticalSelectedIndexArray) is null)
                    {
                        tacticalSelectedIndexArray = new int[TacticalCallsigns._TacticalCallsignDataList.Count];
                        for (int i = 0; i < tacticalSelectedIndexArray.Length; i++)
                        {
                            tacticalSelectedIndexArray[i] = 0;
                        }
                    }
                }
                return tacticalSelectedIndexArray;
            }
            set => SetPropertyPrivate(ref tacticalSelectedIndexArray, value, true);
        }

        private int tacticalCallsignSelectedIndex;
        public int TacticalCallsignSelectedIndex
        {
            get
            {
                if (tacticalCallsignSelectedIndex < 0)
                {
                    return TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex];
                }
                else
                {
                    //TacticalCallsign = TacticalCallsignOther;
                    return tacticalCallsignSelectedIndex;
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
                    bool propertyChanged = SetProperty(ref tacticalCallsignSelectedIndex, value);
                    if (propertyChanged)
                    {
                        int[] temp = new int[TacticalSelectedIndexArray.Length];
                        TacticalSelectedIndexArray.CopyTo(temp, 0);
                        temp[TacticalCallsignAreaSelectedIndex] = value;
                        //TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex] = value;
                        //TacticalSelectedIndexArray = TacticalSelectedIndexArray;
                        TacticalSelectedIndexArray = temp;
                        //SetProperty(ref tacticalCallsignSelectedIndex, value);
                        TacticalCallsign = TacticalCallsignsSource[tacticalCallsignSelectedIndex].TacticalCallsign;
                        //TacticalCallsignOther = TacticalCallsign;
                        TacticalAgencyNameSelectedIndex = tacticalCallsignSelectedIndex;
                        TacticalMsgPrefix = TacticalCallsignsSource[tacticalCallsignSelectedIndex].Prefix;
                        if (UseTacticalCallsign)
                        {
                            TacticalCallsignsAreaSource[TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArraySelectedIndex = tacticalCallsignSelectedIndex;
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

        private string tacticalCallsignOther = "";
        public string TacticalCallsignOther
        {
            get => tacticalCallsignOther;
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

                SetProperty(ref tacticalCallsignOther, value);

                if (tacticalCallsignOther.Length == 6)
                {
                    TacticalCallsign = tacticalCallsignOther;
                    TacticalCall tacticalCall = new TacticalCall()
                    {
                        TacticalCallsign = tacticalCallsignOther,
                        AgencyName = "",
                        Prefix = tacticalCallsignOther.Substring(tacticalCallsignOther.Length - 3),
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

        private string tacticalCallsign;
        public string TacticalCallsign
        {
            get
            {
                if (TacticalCallsignsSource is null)
                {
                    TacticalCallsignAreaSelectedIndex = Utilities.GetProperty("TacticalCallsignAreaSelectedIndex");
                }
                return tacticalCallsign;
            }
            set
            {
                if (SetProperty(ref tacticalCallsign, value))
                {
                    Utilities.SetApplicationTitle();
                }
            }
        }

        private int tacticalAgencyNameSelectedIndex;
        public int TacticalAgencyNameSelectedIndex
        {
            get => tacticalAgencyNameSelectedIndex;
            set
            {
                SetProperty(ref tacticalAgencyNameSelectedIndex, value);
                //if (SetProperty(ref tacticalAgencyNameSelectedIndex, value))
                //{
                //    TacticalCallsignSelectedIndex = tacticalAgencyNameSelectedIndex;
                //}
            }
        }

        private string tacticalAgencyName;
        public string TacticalAgencyName
        {
            get => TacticalCallsignsSource[tacticalCallsignSelectedIndex].AgencyName;
            set
            {
                SetProperty(ref tacticalAgencyName, value);
                TacticalCallsignSelectedIndex = tacticalAgencyNameSelectedIndex;
            }
        }

        private string tacticalPrefix;
        public string TacticalMsgPrefix
        {
            get => tacticalPrefix;
            set { SetProperty(ref tacticalPrefix, value); }
        }

        private bool displayIdentityAtStartup;
        public bool DisplayIdentityAtStartup
        {
            get => GetProperty(ref displayIdentityAtStartup);
            set => SetProperty(ref displayIdentityAtStartup, value, true);
        }
    }
}
