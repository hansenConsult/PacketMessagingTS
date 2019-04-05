using System;
using System.Collections.Generic;
using System.IO;

using MetroLog;

using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.Storage;

namespace PacketMessagingTS.Helpers
{
    public class BulletinHelpers
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BulletinHelpers>();
        private static LogHelper _logHelper = new LogHelper(log);

        public static Dictionary<string, List<string>> BulletinDictionary;

        public const string BulletinFilePrefix = "BulletinFile";

        public static void SaveBulletinDictionary(string[] areas)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            foreach (string area in areas)
            {
                string filePath = Path.Combine(localFolder.Path, GetBulletinsFileName(area));
                try
                {
                    bool success = BulletinDictionary.TryGetValue(area, out List<string> areaBulletinList);
                    if (success && !(areaBulletinList is null))
                    {
                        //File.WriteAllLines(filePath, BulletinDictionary[area]);
                        File.WriteAllLines(filePath, areaBulletinList);
                    }
                }
                catch (Exception e)
                {
                    //continue;
                    _logHelper.Log(LogLevel.Error, $"Error saving Bulletin Dictionary, {e.Message}");
                }
            }
        }

        public static void CreateBulletinDictionaryFromFiles()
        {
            PacketSettingsViewModel packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;
            string[] areas = packetSettingsViewModel.AreaString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            BulletinDictionary = new Dictionary<string, List<string>>();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string[] areaBulletin;
            foreach (string area in areas)
            {
                string filePath = Path.Combine(localFolder.Path, GetBulletinsFileName(area));

                try
                {
                    if (File.Exists(filePath))
                    {
                        areaBulletin = File.ReadAllLines(filePath);
                        foreach (string subject in areaBulletin)
                        {
                            if (!BulletinDictionary.TryGetValue(area, out List<string> bulletinList))
                            {
                                BulletinDictionary[area] = new List<string>();
                            }
                            BulletinDictionary[area].Add(subject);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private static string GetBulletinsFileName(string area)
        {
            return BulletinFilePrefix + area + ".txt";
        }
    }
}
