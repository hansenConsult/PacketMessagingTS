using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class IdentityViewModel : BaseViewModel
    {
        public static TacticalCall _callsignData;
        public TacticalCallsignData _tacticalCallsignData;

        public IdentityViewModel()
        {

        }

        string userCallsign;
        public string UserCallsign
        {
            get => GetProperty(ref userCallsign);
            set
            {
                SetProperty(ref userCallsign, value, true);
                if (userCallsign.Length > 3)
                {
                    UserMsgPrefix = userCallsign.Substring(userCallsign.Length - 3, 3);
                }
            }
        }

        string userName;
        public string UserName
        {
            get => GetProperty(ref userName);
            set
            {
                SetProperty(ref userName, value, true);
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

        string userFirstName;
        public string UserFirstName
        {
            get => GetProperty(ref userFirstName);
            set { SetProperty(ref userFirstName, value, true); }
        }

        string userCity;
        public string UserCity
        {
            get => GetProperty(ref userCity);
            set { SetProperty(ref userCity, value, true); }
        }

        string userMsgPrefix;
        public string UserMsgPrefix
        {
            get { return GetProperty(ref userName); }
            set { SetProperty(ref userMsgPrefix, value, true); }
        }

        private bool useTacticalCallsign;
        public bool UseTacticalCallsign
        {
            get => GetProperty(ref useTacticalCallsign);
            set => SetProperty(ref useTacticalCallsign, value, true);
        }

        public ObservableCollection<TacticalCallsignData> TacticalCallsignsAreaSource
        {
            get => new ObservableCollection<TacticalCallsignData>(App._TacticalCallsignDataList);
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
                SetProperty(ref tacticalCallsignAreaSelectedIndex, value, true);
                TacticalCallsignsSource = new ObservableCollection<TacticalCall>(TacticalCallsignsAreaSource[value].TacticalCallsigns.TacticalCallsignsArray);
            }
        }

        private int[] tacticalSelectedIndexArray;
        public int[] TacticalSelectedIndexArray
        {
            get
            {
                if (tacticalSelectedIndexArray is null)
                {
                    // Only get saved value on startup
                    return GetProperty(ref tacticalSelectedIndexArray);
                }
                //return GetProperty(ref tacticalSelectedIndexArray);
                return tacticalSelectedIndexArray;
            }
            set => SetProperty(ref tacticalSelectedIndexArray, value, true);
        }

        private int tacticalCallsignSelectedIndex;
        public int TacticalCallsignSelectedIndex
        {
            get
            {
                if (tacticalCallsignSelectedIndex >= 0)
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
                if (value >= TacticalCallsignsSource.Count)
                {
                    value = 0;
                }
                TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex] = value;
                TacticalSelectedIndexArray = TacticalSelectedIndexArray;
                SetProperty(ref tacticalCallsignSelectedIndex, value);
                if (value != -1)
                {
                    //TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex] = value;
                    //TacticalSelectedIndexArray = TacticalSelectedIndexArray;
                    //SetProperty(ref tacticalCallsignSelectedIndex, value);
                    // TODO improve on this
                    //if (value != -1 && App._TacticalCallsignDataList[TacticalCallsignAreaSelectedIndex].TacticalCallsigns != null)
                    //_callsignData = TacticalCallsignsSource[tacticalCallsignSelectedIndex];
                    //_callsignData = tacticalCallsigns.TacticalCallsignsArray[value];
                    //_callsignData = App._TacticalCallsignDataList[TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArray[value];
                    //SelectedTacticalCall = App._TacticalCallsignDataList[TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArray[value];
                    //TacticalCallsign = _callsignData.TacticalCallsign;
                    TacticalCallsign = TacticalCallsignsSource[tacticalCallsignSelectedIndex].TacticalCallsign;
                    TacticalAgencyNameSelectedIndex = tacticalCallsignSelectedIndex;
                    //TacticalMsgPrefix = _callsignData.Prefix;
                    TacticalMsgPrefix = TacticalCallsignsSource[tacticalCallsignSelectedIndex].Prefix;
                }
                //else
                //{
                //    TacticalCallsign = TacticalCallsignOther;
                //}
            }
        }

        private string tacticalCallsignOther = "";
        public string TacticalCallsignOther
        {
            get => tacticalCallsignOther;
            set
            {
                SetProperty(ref tacticalCallsignOther, value);
                //tacticalCallsignOther = value;

                //TacticalCallsignSelectedIndex = -1;
                //TacticalCallsign = tacticalCallsignOther;
                //TacticalAgencyNameSelectedIndex = -1;
                //if (TacticalCallsign.Length >= 6)
                //    TacticalMsgPrefix = TacticalCallsign.Substring(3, 3);
            }
        }

        private TacticalCall selectedTacticalCallArea;
        public TacticalCall SelectedTacticalCallArea
        {
            get => selectedTacticalCallArea;
            set
            {
                SetProperty(ref selectedTacticalCallArea, value);
            }
        }

        private TacticalCall selectedTacticalCall;
        public TacticalCall SelectedTacticalCall
        {
            get => selectedTacticalCall;
            set
            {
                SetProperty(ref selectedTacticalCall, value);

                //TacticalCallsign = selectedTacticalCall.TacticalCallsign;
                TacticalMsgPrefix = selectedTacticalCall.Prefix;
                TacticalAgencyName = selectedTacticalCall.AgencyName;
            }

        }

        private string tacticalCallsign;
        public string TacticalCallsign
        {
            get
            {
                if (TacticalCallsignsSource is null)
                {
                    TacticalCallsignAreaSelectedIndex = Utilities.GetProperty("TacticalCallsignAreaSelectedIndex");
                }
                //tacticalCallsign = TacticalCallsignsSource[TacticalSelectedIndexArray[TacticalCallsignAreaSelectedIndex]].TacticalCallsign;
                return tacticalCallsign;
            }
            set => SetProperty(ref tacticalCallsign, value);
        }

        private int tacticalAgencyNameSelectedIndex;
        public int TacticalAgencyNameSelectedIndex
        {
            get => tacticalAgencyNameSelectedIndex;
            set
            {
                if (SetProperty(ref tacticalAgencyNameSelectedIndex, value))
                {
                    TacticalCallsignSelectedIndex = tacticalAgencyNameSelectedIndex;
                }
            }
        }

        private string tacticalAgencyName;
        public string TacticalAgencyName
        {
            get => tacticalAgencyName;
            set
            {
                SetProperty(ref tacticalAgencyName, value);
                //TacticalCallsignSelectedIndex = tacticalAgencyNameSelectedIndex;
            }
        }

        private string tacticalMsgPrefix;
        public string TacticalMsgPrefix
        {
            get => tacticalMsgPrefix;
            set { SetProperty(ref tacticalMsgPrefix, value); }
        }

        private bool displayIdentityAtStartup;
        public bool DisplayIdentityAtStartup
        {
            get => GetProperty(ref displayIdentityAtStartup);
            set => SetProperty(ref displayIdentityAtStartup, value, true);
        }
    }
}
