using MetroLog;
using PacketMessagingTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Helpers
{
    public static class Utilities
    {

        public static string GetMessageNumberPacket(bool markAsUsed = false) => GetMessageNumber(markAsUsed) + "P";

        public static string GetMessageNumber(bool reserveMessageNumber = false)
        {
            string messageNumberString;

            //int messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
            int messageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
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

            return messageNumberString;
        }

        public static void MarkMessageNumberAsUsed(int startMessageNumber = -1)
        {
            int messageNumber;
            if (startMessageNumber < 0)
            {
                //messageNumber = await SettingsStorageExtensions.ReadAsync<int>(SharedData.SettingsContainer, "MessageNumber");
                messageNumber = Utilities.GetProperty("MessageNumber");
                messageNumber++;
            }
            else
            {
                messageNumber = startMessageNumber;
            }
            //await SettingsStorageExtensions.SaveAsync(SharedData.SettingsContainer, "MessageNumber", messageNumber);
            App.Properties["MessageNumber"] = messageNumber;
        }

        // TRhis is just to show a use of ContentDialog.
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

        public static async Task ShowMessageDialogAsync(CoreDispatcher dispatcher, string dialogMessage, string title = "Packet Messaging")
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

        public static async Task ShowMessageDialogAsync(string dialogMessage, string title = "Packet Messaging")
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = title,
                Content = dialogMessage,
                CloseButtonText = "Close"
            };
            await contentDialog.ShowAsync();
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
    }

}
