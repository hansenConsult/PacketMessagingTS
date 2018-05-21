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

        private Int64 profileSelectedIndex;
        public Int64 ProfileSelectedIndex
        {
            get
            {
                GetProperty(ref profileSelectedIndex);
                Profile profile = ProfileArray.Instance.ProfileList[Convert.ToInt32(profileSelectedIndex)];
                DefaultTo = profile.SendTo;
                return profileSelectedIndex;
            }
            set
            {
                SetProperty(ref profileSelectedIndex, value, true);
                Profile profile = ProfileArray.Instance.ProfileList[Convert.ToInt32(profileSelectedIndex)];
                DefaultTo = profile.SendTo;
            }

        }

        private Profile currentProfile;
        public Profile CurrentProfile
        {
            get => currentProfile;
            set
            {
                currentProfile = value;

                Name = currentProfile.Name;
                TNC = currentProfile.TNC;
                BBS = currentProfile.BBS;

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
            set => SetProperty(ref tnc, value);
        }

        private string bbs;
        public string BBS
        {
            get => bbs;
            set
            {
                SetProperty(ref bbs, value);

            }
        }

        private string defaultTo;
        public string DefaultTo
        {
            get => defaultTo;
            set => SetProperty(ref defaultTo, value);
        }
    }
}
