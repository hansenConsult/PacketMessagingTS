using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class PacketSettingsViewModel : BaseViewModel
    {
        public PacketSettingsViewModel()
        {

        }

        public override void ResetChangedProperty()
        {
            base.ResetChangedProperty();
            IsAppBarSaveEnabled = false;
        }

        private int profileSelectedIndex;
        public int ProfileSelectedIndex
        {
            get
            {
                return GetProperty(ref profileSelectedIndex);
            }
            set
            {
                SetProperty(ref profileSelectedIndex, value, true);
                CurrentProfile = ProfileArray.Instance.ProfileList[Convert.ToInt32(profileSelectedIndex)];
            }
        }

        private Profile currentProfile;
        public Profile CurrentProfile
        {
            get => currentProfile;
            set
            {
                currentProfile = value;

                CurrentTNC = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == currentProfile.TNC).FirstOrDefault();
                CurrentBBS = BBSDefinitions.Instance.BBSDataList.Where(bbs => bbs.Name == currentProfile.BBS).FirstOrDefault();
                Name = currentProfile.Name;
                TNC = currentProfile.TNC;
                BBSSelectedValue = currentProfile.BBS;
                DefaultTo = currentProfile.SendTo;

                ResetChangedProperty();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string tnc;
        public string TNC
        {
            get => tnc;
            set
            {
                SetProperty(ref tnc, value);

                bool changed = CurrentProfile.TNC != tnc;
                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }

        private TNCDevice currentTNC;
        public TNCDevice CurrentTNC
        {
            get => currentTNC;
            set => currentTNC = value;
        }

        //private string bbs;
        //public string BBS
        //{
        //    get => bbs;
        //    set
        //    {
        //        SetProperty(ref bbs, value);
        //    }
        //}

        private string bbsSelectedValue;
        public string BBSSelectedValue
        {
            get => bbsSelectedValue;
            set
            {
                SetProperty(ref bbsSelectedValue, value);

                bool changed = CurrentProfile.BBS != bbsSelectedValue;
                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }

        private BBSData currentBBS;
        public BBSData CurrentBBS
        {
            get => currentBBS;
            set
            {
                currentBBS = value;
                BBSDescription = currentBBS?.Description;
                BBSFrequency1 = currentBBS?.Frequency1;
                BBSFrequency2 = currentBBS?.Frequency2;

            }
        }

        private string bbsDescription;
        public string BBSDescription
        {
            get => bbsDescription;
            set => SetProperty(ref bbsDescription, value);
        }
        
        private string bbsFrequency1;
        public string BBSFrequency1
        {
            get => bbsFrequency1;
            set => SetProperty(ref bbsFrequency1, value);
        }

        private string bbsFrequency2;
        public string BBSFrequency2
        {
            get => bbsFrequency2;
            set => SetProperty(ref bbsFrequency2, value);
        }

        private string defaultTo;
        public string DefaultTo
        {
            get => defaultTo;
            set
            {
                SetProperty(ref defaultTo, value);

                bool changed = CurrentProfile.SendTo != defaultTo;

                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }

        private bool displayProfileOnStart;
        public bool DisplayProfileOnStart
        {
            get => GetProperty(ref displayProfileOnStart);
            set => SetProperty(ref displayProfileOnStart, value, true);
        }

        private int firstMessageNumber;
        public int FirstMessageNumber
        {
            get
            {
                bool found = App.Properties.TryGetValue("MessageNumber", out object first);
                if (!found)
                {
                    App.Properties["MessageNumber"] = 100;
                }
                firstMessageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
                return firstMessageNumber;
            }
            set
            {
                Utilities.MarkMessageNumberAsUsed(value);
                SetProperty(ref firstMessageNumber, value);
            }
        }

        private string areaString = "XSCPERM, XSCEVENT";
        public string AreaString
        {
            get => GetProperty(ref areaString);
            set => SetProperty(ref areaString, value, true);
        }

        private bool sendReceipt;
        public bool SendReceipt
        {
            get => GetProperty(ref sendReceipt);
            set => SetProperty(ref sendReceipt, value, true);
        }

        private bool forceReadBulletins;
        public bool ForceReadBulletins
        {
            get => forceReadBulletins;
            set => SetProperty(ref forceReadBulletins, value);
        }
    }
}
