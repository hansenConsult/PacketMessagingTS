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

        private string defaultTo;
        public string DefaultTo
        {
            get => defaultTo;
            set => SetProperty(ref defaultTo, value);
        }
    }
}
