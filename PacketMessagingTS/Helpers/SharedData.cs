using System.Collections.Generic;
using System.Reflection;

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

        public static StorageFolder DraftMessagesFolder;

        public static StorageFolder ReceivedMessagesFolder;

        public static StorageFolder SentMessagesFolder;

        public static StorageFolder UnsentMessagesFolder;

        public static StorageFolder MetroLogsFolder;

        public static StorageFolder PrintMessagesFolder;

        public static ApplicationDataContainer SettingsContainer;

        //public static IReadOnlyList<StorageFile> FilesInInstalledLocation;

        public static List<Assembly> Assemblies;

        public static List<FormControlAttributes> FormControlAttributeCityList;

        public static List<FormControlAttributes> FormControlAttributeCountyList;

        public static List<FormControlAttributes> FormControlAttributeHospitalList;

        public static List<FormControlAttributes> FormControlAttributeTestList;
    }

}
