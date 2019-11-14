using System;
using System.Threading.Tasks;
using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
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

            int messageNumber = Convert.ToInt32(GetProperty("MessageNumber"));
            if (messageNumber == default(int))
            {
                messageNumber = 100;
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

        public static async Task ShowSingleButtonMessageDialogAsync(CoreDispatcher dispatcher, string dialogMessage, string closeButtonText = "Close", string title = "Packet Messaging")
        {
            await dispatcher.RunTaskAsync(async () =>
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = title,
                    Content = dialogMessage,
                    CloseButtonText = "Close"
                };
                await contentDialog.ShowAsync();
            });
        }

        public static async Task ShowSingleButtonContentDialogAsync(string dialogMessage, string closeButtonText = "Close", string title = "Packet Messaging")
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = title,
                Content = dialogMessage,
                CloseButtonText = closeButtonText,
            };
            await contentDialog.ShowAsync();
        }

        public static async Task<bool> ShowDualButtonMessageDialogAsync(string dialogMessage, string primaryButtonText = "OK", string closeButtonText = "Cancel", string title = "Packet Messaging")
        {
            ContentControl content = new ContentControl();
            content.Content = new TextBox();
            ((TextBox)content.Content).AcceptsReturn = true;
            ((TextBox)content.Content).TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            ((TextBox)content.Content).IsReadOnly = true;
            //ScrollViewer.SetVerticalScrollBarVisibility(content, ScrollBarVisibility.Auto);
            ((TextBox)content.Content).Text = dialogMessage;

            ContentDialog contentDialog = new ContentDialog()
            {
                Title = title,
                //Content = dialogMessage,
                Content = content,
                CloseButtonText = closeButtonText,
                PrimaryButtonText = primaryButtonText,
            };
            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                return true;
            else
                return false;
        }

        //public static async Task<bool> ShowYesNoMessageDialogAsync(string dialogMessage, string title = "Packet Messaging")
        //{
        //    ContentDialog contentDialog = new ContentDialog()
        //    {
        //        Title = title,
        //        Content = dialogMessage,
        //        CloseButtonText = "No",
        //        PrimaryButtonText = "Yes",
        //    };
        //    ContentDialogResult result = await contentDialog.ShowAsync();
        //    if (result == ContentDialogResult.Primary)
        //        return true;
        //    else
        //        return false;
        //}

        public static int GetProperty(string propertyName)
        {
            if (App.Properties != null && App.Properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = App.Properties[propertyName];
                return Convert.ToInt32(o);
            }
            else
                return 0;
        }


        public static T GetProperty<T>(string propertyName)
        {
            if (App.Properties != null && App.Properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = App.Properties[propertyName];
                return (T)o;
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
