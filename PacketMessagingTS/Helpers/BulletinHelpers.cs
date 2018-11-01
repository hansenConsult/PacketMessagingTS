using PacketMessagingTS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PacketMessagingTS.Helpers
{
    public class BulletinHelpers
    {
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
                    File.WriteAllLines(filePath, BulletinDictionary[area]);
                }
                catch
                {
                    continue;
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
