using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Models;
using Windows.Storage;

namespace PacketMessagingTS.Helpers
{
    public static class SharedData
    {
        //public static Dictionary<string, TacticalCallsignData> _tacticalCallsignDataDictionary;

        public static StorageFolder ArchivedMessagesFolder
        {
            get;
            set;
        }

        public static StorageFolder DeletedMessagesFolder
        {
            get;
            set;
        }

        public static StorageFolder UnsentMessagesFolder
        {
            get;
            set;
        }

        public static StorageFolder DraftMessagesFolder
        {
            get;
            set;
        }

        public static StorageFolder ReceivedMessagesFolder
        {
            get;
            set;
        }

        public static StorageFolder SentMessagesFolder
        {
            get;
            set;
        }

        public static ApplicationDataContainer SettingsContainer
        {
            get; set;
        }

        //static Profile currentProfile;
        //public static Profile CurrentProfile
        //{
        //    get => currentProfile;
        //    set => currentProfile = value;
        //}

        //public static BBSDefinitions BbsArray { get; set; } = new BBSDefinitions();

        public static BBSData CurrentBBS { get; set; }

        //public static TNCDeviceArray TncDeviceArray { get; set; }

        public static TNCDevice CurrentTNCDevice { get; set; }

        //public static TNCDevice SavedTNCDevice { get; set; }

        public static string[] _Areas;
        public static bool _forceReadBulletins;

    }

}
