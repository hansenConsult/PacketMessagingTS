using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class IdentityViewModel : BaseViewModel
    {
        public static TacticalCall _callsignData;
        public static TacticalCallsignData _tacticalCallsignData;

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
            set { SetProperty(ref userName, value, true); }
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

        private string tacticalCallsign;
        public string TacticalCallsign
        {
            get => GetProperty(ref tacticalCallsign);
            set
            {
                SetProperty(ref tacticalCallsign, value, true);
                if (tacticalCallsign.Length > 3)
                {
                    TacticalMsgPrefix = tacticalCallsign.Substring(tacticalCallsign.Length - 3, 3);
                }
            }
        }

        private Int64 tacticalCallsignAreaSelectedIndex;
        public Int64 TacticalCallsignAreaSelectedIndex
        {
            get => GetProperty(ref tacticalCallsignAreaSelectedIndex);
            set
            {
                SetProperty(ref tacticalCallsignAreaSelectedIndex, value, true);
            }
        }

        private Int64 tacticalCallsignSelectedIndex;
        public Int64 TacticalCallsignSelectedIndex
        { 
            get { return GetProperty(ref tacticalCallsignAreaSelectedIndex); }
            set
            {
                if (value == -1)
                {

                }
                else
                {
                    tacticalCallsignSelectedIndex = value;

                    int index = Convert.ToInt32(TacticalCallsignAreaSelectedIndex);
                    if (Views.SettingsPage.listOfTacticallsignsArea[index].TacticalCallsigns != null)
                    {
                        _callsignData = Views.SettingsPage.listOfTacticallsignsArea[index].TacticalCallsigns.TacticalCallsignsArray[value];
                        TacticalCallsign = _callsignData.TacticalCallsign;
                        TacticalMsgPrefix = _callsignData.Prefix;
                        TacticalPrimary = _callsignData.PrimaryBBS;
                        TacticalPrimaryActive = _callsignData.PrimaryBBSActive;
                        TacticalSecondary = _callsignData.SecondaryBBS;
                        //TacticalSecondaryActive = _callsignData.SecondaryBBSActive;
                    }
                    SetProperty(ref tacticalCallsignSelectedIndex, value, true);
                }
            }
        }

        private string tacticalCallsignOther;
        public string TacticalCallsignOther
        {
            get => GetProperty(ref tacticalCallsignOther);
            set
            {
                SetProperty(ref tacticalCallsignOther, value, true);

                TacticalCallsign = TacticalCallsignOther;
                TacticalMsgPrefix = TacticalCallsign.Substring(3, 3);
            }
        }

        private string tacticalMsgPrefix;
        public string TacticalMsgPrefix
        {
            get { return tacticalMsgPrefix; }
            set { SetProperty(ref tacticalMsgPrefix, value); }
        }

        private string tacticalPrimary;
        public string TacticalPrimary
        {
            get { return tacticalPrimary; }
            set { SetProperty(ref tacticalPrimary, value); }
        }

        private bool tacticalPrimaryActive;
        public bool TacticalPrimaryActive
        {
            get { return tacticalPrimaryActive; }
            set
            {
                tacticalPrimaryActive = value;
                if (_callsignData == null)
                {
                    int index = Convert.ToInt32(TacticalCallsignAreaSelectedIndex);
                    _callsignData = Views.SettingsPage.listOfTacticallsignsArea[index].TacticalCallsigns.TacticalCallsignsArray[TacticalCallsignSelectedIndex];
                }

                _callsignData.PrimaryBBSActive = tacticalPrimaryActive;
                _tacticalCallsignData.TacticalCallsignsChanged = true;
                //AddressBook.UpdateEntry(_callsignData);
                SetProperty(ref tacticalPrimaryActive, value);
            }
        }

        private string tacticalSecondary;
        public string TacticalSecondary
        {
            get { return tacticalSecondary; }
            set { SetProperty(ref tacticalSecondary, value); }
        }

    }
}
