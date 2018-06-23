﻿using System;
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
#if DEBUG
        public static StorageFolder TestFilesFolder;
#endif

        public static StorageFolder ArchivedMessagesFolder;

        public static StorageFolder DeletedMessagesFolder;

        public static StorageFolder UnsentMessagesFolder;

        public static StorageFolder DraftMessagesFolder;

        public static StorageFolder ReceivedMessagesFolder;

        public static StorageFolder SentMessagesFolder;

        public static StorageFolder MetroLogsFolder;

        public static ApplicationDataContainer SettingsContainer;

        public static IReadOnlyList<StorageFile> FilesInInstalledLocation;

        public static string EMail = "E-Mail";

        //static Profile currentProfile;
        //public static Profile CurrentProfile
        //{
        //    get => currentProfile;
        //    set => currentProfile = value;
        //}

        //public static BBSData CurrentBBS { get; set; }

        //public static TNCDevice CurrentTNCDevice { get; set; }

        //public static TNCDevice SavedTNCDevice { get; set; }
    }

}
