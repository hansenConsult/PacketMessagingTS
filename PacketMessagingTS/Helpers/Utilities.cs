using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MetroLog;

using Newtonsoft.Json;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
using Windows.Storage;
using Windows.Storage.Search;
//using SharedCode.Helpers;

using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Helpers
{
    public class Utilities
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<Utilities>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public static string GetMessageNumberPacket(bool markAsUsed = false) => GetMessageNumber(markAsUsed) + "P";

        public static string GetMessageNumber(bool reserveMessageNumber = false)
        {
            string messageNumberString;

            int messageNumber = GetProperty("MessageNumber");
            if (messageNumber == default(int))
            {
                messageNumber = FindHighestUsedMesageNumber() + 1;
                //messageNumber = 100;
            }

            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                messageNumberString = Singleton<IdentityViewModel>.Instance.TacticalMsgPrefix + "-" + messageNumber.ToString();
            }
            else
            {
                messageNumberString = Singleton<IdentityViewModel>.Instance.UserMsgPrefix + "-" + messageNumber.ToString();
            }
            if (reserveMessageNumber)
            {
                messageNumber++;
                //await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
                App.Properties["MessageNumber"] = messageNumber;
                
            }
            //_logHelper.Log(LogLevel.Info, $"GetMessageNumber Used:{reserveMessageNumber}, {messageNumberString}");
            return messageNumberString;
        }

        public static void MarkMessageNumberAsUsed(int startMessageNumber = -1)
        {
            int messageNumber;
            if (startMessageNumber < 0)
            {
                //messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
                messageNumber = GetProperty("MessageNumber");
                messageNumber++;
            }
            else
            {
                messageNumber = startMessageNumber++;
            }
            //await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
            App.Properties["MessageNumber"] = messageNumber;
            _logHelper.Log(LogLevel.Info, $"MarkMessageNumberAsUsed {messageNumber}");
        }

        public static int FindHighestUsedMesageNumberInFolder(string folder, string prefix)
        {
            DirectoryInfo DirInfo = new DirectoryInfo(folder);
            var fileNames = from f in DirInfo.EnumerateFiles()
                        where f.Name.StartsWith(prefix) && f.Extension == ".xml"
                        select f.Name;

            List<string> fileList = fileNames.ToList();

            if (fileList is null || fileList.Count == 0)
                return 0;

            fileList.Sort();
            string file = fileList.Last();
            int msgNo = 0;
            int startIndex = file.IndexOf('-') + 1;
            int endIndex = file.IndexOf('_');
            msgNo = int.Parse(file.Substring(startIndex, endIndex - 1 - startIndex));
            //_logHelper.Log(LogLevel.Info, $"{DirInfo.Name}, msgNumber: {msgNo}");
            return msgNo;
        }

        public static int FindHighestUsedMesageNumber()
        {
            string msgNoPrefix;
            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                msgNoPrefix = Singleton<IdentityViewModel>.Instance.TacticalMsgPrefix;
            }
            else
            {
                msgNoPrefix = Singleton<IdentityViewModel>.Instance.UserMsgPrefix;
            }

            List<int> msgNumbers = new List<int>();

            Task<int>[] taskArray = { Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.ArchivedMessagesFolder.Path, msgNoPrefix)),
                Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.DeletedMessagesFolder.Path, msgNoPrefix)),
                Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.DraftMessagesFolder.Path, msgNoPrefix)),
                Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.ReceivedMessagesFolder.Path, msgNoPrefix)),
                Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.SentMessagesFolder.Path, msgNoPrefix)),
                Task<int>.Factory.StartNew( () => FindHighestUsedMesageNumberInFolder(SharedData.UnsentMessagesFolder.Path, msgNoPrefix)),
            };

            var results = new int[taskArray.Length];
            for (int i = 0; i < taskArray.Length; i++)
            {
                msgNumbers.Add(taskArray[i].Result);
            }

            msgNumbers.Sort();

            //_logHelper.Log(LogLevel.Info, $"msgNumbers: {msgNumbers.Last()}");

            return msgNumbers.Last();
        }

        // This is just to show a use of ContentDialog.
        private static async Task<string> GetPinFromUserAsync(CoreDispatcher dispatcher)
        {
            return await dispatcher.RunTaskAsync(async () =>
            {
                var pinBox = new TextBox();
                var dialog = new ContentDialog()
                {
                    Title = "Enter Pin",
                    PrimaryButtonText = "OK",
                    Content = pinBox
                };
                await dialog.ShowAsync();
                return pinBox.Text;
            });
        }

        public static int GetProperty(string propertyName)
        {
            if (App.Properties != null && App.Properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = App.Properties[propertyName];
                return Convert.ToInt32(o);
            }
            else
                return default;
        }


        public static T GetProperty<T>(string propertyName)
        {
            if (App.Properties != null && App.Properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = App.Properties[propertyName];
                //T property = JsonConvert.DeserializeObject<T>(o as string);
                return (T)o;
                //return property;
            }
            else
                return default(T);
        }

        public static (string bbs, string tnc, string from) GetProfileData()
        {
            IdentityViewModel instance = Singleton<IdentityViewModel>.Instance;
            string from = instance.UseTacticalCallsign ? instance.TacticalCallsign : instance.UserCallsign;

            string bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS;
            string tnc = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC;
            //if (string.IsNullOrEmpty(bbs) || !bbs.Contains("XSC") && !tnc.Contains(SharedData.EMail))
            //{
            //    bbs = AddressBook.Instance.GetBBS(from);
            //}
            return (bbs, tnc, from);
        }

        public static (string bbs, string tnc, string from) GetProfileDataBBSStatusChecked()
        {
            IdentityViewModel instance = Singleton<IdentityViewModel>.Instance;
            string from = instance.UseTacticalCallsign ? instance.TacticalCallsign : instance.UserCallsign;
            string bbs = AddressBook.Instance.GetBBS(from);

            string profileBBS = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS;
            bool profileBBSUp = Singleton<SettingsViewModel>.Instance.IsBBSUp(profileBBS);
            if (profileBBSUp)
            {
                bbs = profileBBS;
            }
            string tnc = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC;
            return (bbs, tnc, from);
        }

        //public static string GetBBSName(out string from, out string tnc)
        //{
        //    IdentityViewModel instance = PacketMessagingTS.Core.Helpers.Singleton<IdentityViewModel>.Instance;
        //    from = instance.UseTacticalCallsign ? instance.TacticalCallsign : instance.UserCallsign;

        //    string bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS;
        //    tnc = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC;
        //    if (string.IsNullOrEmpty(bbs) || !bbs.Contains("XSC") && !tnc.Contains(SharedData.EMail)) 
        //    {
        //        bbs = AddressBook.Instance.GetBBS(from);
        //    }
        //    return bbs;
        //}

        public static void SetApplicationTitle(string bbsName = "")
        {
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = "";

            string title = "Packet Messaging, ";
            title += Singleton<IdentityViewModel>.Instance.UserCallsign;
            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                title += " as " + Singleton<IdentityViewModel>.Instance.TacticalCallsign;
            }

            (string bbs, string tnc, string from) = GetProfileData();
            if (!string.IsNullOrEmpty(bbs))
                title += " - " + (string.IsNullOrEmpty(bbsName) ? bbs : bbsName);
            title += " - " + tnc;

            appView.Title = title;
        }
    }

}
