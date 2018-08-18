using System;
using System.Collections.Generic;
using System.IO;
using FormControlBaseClass;
using PacketMessagingTS.ViewModels;
using SharedCode;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.Views
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewViewModel ViewModel { get; } = new WebViewViewModel();

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null)
            {
                ViewModel.PopulateEmptyForm = true;
                return;
            }

            ViewModel.PopulateEmptyForm = false;

            string packetMessagePath = e.Parameter as string;
            PacketMessage packetMessage = PacketMessage.Open(packetMessagePath);
            packetMessage.MessageOpened = true;
            string directory = Path.GetDirectoryName(packetMessagePath);
            if (packetMessage.PacFormName.Contains("213RR"))
            {
                webViewPivot.SelectedIndex = 1;
            }
            else if (packetMessage.PacFormName.Contains("213"))
            {
                webViewPivot.SelectedIndex = 0;
            }
            else
            {
                webViewPivot.SelectedIndex = 2;
            }

            FormControlBase formControl = FormsPage.CreateFormControlInstance(packetMessage.PacFormName);

            ConvertFromOutpost(ref packetMessage, ref formControl);

        }

        private async void WebViewPivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            //StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets\\Pacforms");

            PivotItem pivotItem = (PivotItem)e.AddedItems[0];
            switch (pivotItem.Name)
            {
                case "webViewPivotItemICS213":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_ICS-213_Message_v070628.html");
                    break;
                case "webViewPivotItemICS213RR":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_EOC-213RR_v1708.html");
                    break;
                case "webViewPivotItemMuniStatus":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_OA_MuniStatus_v20130101.html");
                    break;
            }
        }

        public void ConvertFromOutpost(ref PacketMessage packetMessage, ref FormControlBase formControl)
        {
            List<string> inlList = new List<string>();
            List<string> inrList = new List<string>();

            FormField[] formFields = packetMessage.FormFieldArray;
            string[] msgLines = packetMessage.MessageBody.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string value = "";
            foreach (FormField formField in formFields)
            {
                (string id, Control control) = formControl.GetTagIndex(formField);
                value = formControl.GetOutpostValue(id, ref msgLines);
                if (!string.IsNullOrEmpty(value))
                {
                    inrList.Add(value);
                    inlList.Add(id);
                }
            }
            ViewModel.InlList = inlList;
            ViewModel.InrList = inrList;
        }

    }
}
