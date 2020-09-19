using System;
using System.Threading.Tasks;

using MetroLog;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SharedCode.Helpers
{
    public class ContentDialogs
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<ContentDialogs>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

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

        public static async Task ShowSingleButtonContentDialogAsync(XamlRoot xamlRoot, string dialogMessage, string closeButtonText = "Close", string title = "Packet Messaging")
        {
            // See https://docs.microsoft.com/en-us/windows/uwp/design/layout/show-multiple-views
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = title,
                Content = dialogMessage,
                CloseButtonText = closeButtonText,
            };
            contentDialog.XamlRoot = xamlRoot;
            await contentDialog.ShowAsync();
        }

        public static async Task<bool> ShowDualButtonMessageDialogAsync(string dialogMessage, string primaryButtonText = "OK", string closeButtonText = "Cancel", string title = "Packet Messaging")
        {
            ContentControl content = new ContentControl();
            content.Content = new TextBox();
            ((TextBox)content.Content).AcceptsReturn = true;
            ((TextBox)content.Content).TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            ((TextBox)content.Content).IsReadOnly = true;
            ((TextBox)content.Content).BorderThickness = new Windows.UI.Xaml.Thickness(0);
            //ScrollViewer.SetVerticalScrollBarVisibility(content, ScrollBarVisibility.Auto);
            ((TextBox)content.Content).Text = dialogMessage;

            ContentDialog contentDialog = new ContentDialog()
            {
                Title = title,
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

    }

}
