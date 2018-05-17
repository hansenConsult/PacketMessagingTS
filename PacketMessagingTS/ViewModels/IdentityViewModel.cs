using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class IdentityViewModel : BaseViewModel
    {
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
            get { return userMsgPrefix; }
            set { SetProperty(ref userMsgPrefix, value); }
        }

        bool useTacticalCallsign;
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

        string tacticalMsgPrefix;
        public string TacticalMsgPrefix
        {
            get { return tacticalMsgPrefix; }
            set { SetProperty(ref tacticalMsgPrefix, value); }
        }

        public int TacticalCallsignAreaSelectedIndex
        {
            get;
            set;
            //get { return _settings.TacticalCallsignAreaSelectedIndex; }
            //set
            //{
            //    _settings.TacticalCallsignAreaSelectedIndex = value;
            //    base.RaisePropertyChanged();
            //}
        }

        public int TacticalCallsignSelectedIndex
        { get; set;
            //get { return _settings.TacticalCallsignSelectedIndex; }
            //set
            //{
            //    if (value == -1)
            //    {

            //    }
            //    else
            //    {
            //        _settings.TacticalCallsignSelectedIndex = value;

            //        if (Views.SettingsPage.listOfTacticallsignsArea[_settings.TacticalCallsignAreaSelectedIndex].TacticalCallsigns != null)
            //        {
            //            _callsignData = Views.SettingsPage.listOfTacticallsignsArea[_settings.TacticalCallsignAreaSelectedIndex].TacticalCallsigns.TacticalCallsignsArray[value];
            //            TacticalCallsign = _callsignData.TacticalCallsign;
            //            TacticalMsgPrefix = _callsignData.Prefix;
            //            TacticalPrimary = _callsignData.PrimaryBBS;
            //            TacticalPrimaryActive = _callsignData.PrimaryBBSActive;
            //            TacticalSecondary = _callsignData.SecondaryBBS;
            //            //TacticalSecondaryActive = _callsignData.SecondaryBBSActive;
            //        }
            //        base.RaisePropertyChanged();
            //    }
            //}
        }

        public string TacticalCallsignOther
        {
            get;
            set;
            //get => _settings.TacticalCallsignOther;
            //set
            //{
            //    _settings.TacticalCallsignOther = value;
            //    base.RaisePropertyChanged();
            //    TacticalCallsign = _settings.TacticalCallsignOther;
            //    TacticalMsgPrefix = TacticalCallsign.Substring(3, 3);
            //}
        }

    }
}
