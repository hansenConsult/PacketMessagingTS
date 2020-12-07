using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FormControlBaseClass;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SharedCode.Helpers;
using Windows.UI.Xaml;

namespace PacketMessagingTS.Views
{
    public sealed partial class WebViewPage : Page
    {
        static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<WebViewPage>();
        private LogHelper _logHelper = new LogHelper(log);

        public WebViewViewModel ViewModel { get; } = new WebViewViewModel();

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is null)
            {
                ViewModel.PopulateEmptyForm = true;
                return;
            }

            ViewModel.PopulateEmptyForm = false;

            string packetMessagePath = e.Parameter as string;
            PacketMessage packetMessage = PacketMessage.Open(packetMessagePath);
            if (packetMessage != null)
            {
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

                FormControlBase formControl = FormsViewModel.CreateFormControlInstance(packetMessage.PacFormName, packetMessage.MessageState);

                ConvertFromOutpost(ref packetMessage, ref formControl, FormProviders.PacForm);
            }
        }

        private void WebViewPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)e.AddedItems[0];
            switch (pivotItem.Name)
            {
                case "webViewPivotItemICS213":
                    ViewModel.SourceUrl = "ms-appx-web:///PacFORMS/XSC_ICS-213_Message_v070628.html";
                    /* test code for Edge 
                    string msgNumber = Utilities.GetMessageNumberPacket();
                    string userCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign ?? "";
                    string userName = Singleton<IdentityViewModel>.Instance.UserName ?? "";

                    string html = await ViewModel.CreateSourceFormAsync("XSC_ICS-213_Message_v070628.html", msgNumber, userCallsign, userName);
                    string path = SharedData.TestFilesFolder.Path;
                    string fileName = "XSC_ICS-213_Message_v070628.html";
                    string filePath = Path.Combine(path, fileName);
                    File.WriteAllText(filePath, html);

                    StorageFolder pacFormsLocation = await ApplicationData.Current.LocalFolder.GetFolderAsync("TestFiles");
                    StorageFile pacFormFile = await pacFormsLocation.TryGetItemAsync(fileName) as StorageFile;

                    await Launcher.LaunchFileAsync(pacFormFile);
                    */
                    break;
                case "webViewPivotItemICS213RR":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_EOC-213RR_v1708.html");
                    break;
                case "webViewPivotItemMuniStatus":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_OA_MuniStatus_v20130101.html");
                    break;
                case "webViewPivotItemShelterStatus":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_OA_ShelterStatus_v20130814.html");
                    break;
                case "webViewPivotItemAlliedHealthStatus":
                    ViewModel.Source = new Uri("ms-appx-web:///PacFORMS/XSC_OA_AlliedHealthStatus_v201802.html");
                    break;
            }
        }

        public void ConvertFromOutpost(ref PacketMessage packetMessage, ref FormControlBase formControl, FormProviders formProvider)
        {
            List<string> inlList = new List<string>();
            List<string> inrList = new List<string>();

            FormField[] formFields = packetMessage.FormFieldArray;
            if (packetMessage.MessageBody is null)
            {
                packetMessage.MessageBody = formControl.CreateOutpostData(ref packetMessage);
            }
            string[] msgLines = packetMessage.MessageBody.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string value = "";
            foreach (FormField formField in formFields)
            {
                (string id, FrameworkElement control) = formControl.GetTagIndex(formField);
                value = FormControlBase.GetOutpostValue(id, ref msgLines);
                if (!string.IsNullOrEmpty(value))
                {
                    inrList.Add(value);
                    inlList.Add(id);
                }
            }
            ViewModel.InlList = inlList;
            ViewModel.InrList = inrList;
        }

        public async Task CreatePacketMessageFromMessageAsync(PacketMessage pktMsg, string folderPath)
        {
            FormControlBase formControl = new MessageControl();
            // test for packet form!!
            //pktMsg.PacFormType = PacForms.Message;
            //pktMsg.PacFormName = "SimpleMessage";
            // Save the original message for post processing (tab characters are lost in the displayed message)
            string[] msgLines = pktMsg.MessageBody.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            bool subjectFound = false;
            for (int i = 0; i < Math.Min(msgLines.Length, 20); i++)
            {
                if (msgLines[i].StartsWith("Date:"))
                {
                    string startpos = new string(new char[] { 'D', 'a', 't', 'e', ':', ' ' });
                    pktMsg.JNOSDate = DateTime.Parse(msgLines[i].Substring(startpos.Length));
                }
                else if (msgLines[i].StartsWith("From:"))
                    pktMsg.MessageFrom = msgLines[i].Substring(6);
                else if (msgLines[i].StartsWith("To:"))
                    pktMsg.MessageTo = msgLines[i].Substring(4);
                else if (msgLines[i].StartsWith("Cc:"))
                {
                    pktMsg.MessageTo += (", " + msgLines[i].Substring(4));
                    while (msgLines[i + 1].Length == 0)
                    {
                        i++;
                    }
                    if (msgLines[i + 1][0] == ' ')
                    {
                        pktMsg.MessageTo += msgLines[i + 1].TrimStart(new char[] { ' ' });
                    }
                }
                else if (!subjectFound && msgLines[i].StartsWith("Subject:"))
                {
                    pktMsg.Subject = msgLines[i].Substring(9);
                    //pktMsg.MessageSubject = pktMsg.MessageSubject.Replace('\t', ' ');
                    subjectFound = true;
                }
                else if (msgLines[i].StartsWith("!PACF!") && !subjectFound)     // Added
                {
                    pktMsg.Subject = msgLines[i].Substring(7);
                    subjectFound = true;
                }
                else if (msgLines[i].StartsWith("# FORMFILENAME:"))
                {
                    string html = ".html";
                    string formName = msgLines[i].Substring(16).TrimEnd(new char[] { ' ' });
                    formName = formName.Substring(0, formName.Length - html.Length);
                    pktMsg.PacFormName = formName;

                    formControl = FormsViewModel.CreateFormControlInstance(pktMsg.PacFormName, pktMsg.MessageState);
                    if (formControl is null)
                    {
                        _logHelper.Log(LogLevel.Error, $"Form {pktMsg.PacFormName} not found");
                        await ContentDialogs.ShowSingleButtonContentDialogAsync($"Form {pktMsg.PacFormName} not found");
                        return;
                    }
                    break;
                }
            }
            pktMsg.PacFormName = formControl.GetPacFormName();
            pktMsg.PacFormType = formControl.PacFormType;       // Added line
            pktMsg.FormControlType = formControl.FormControlType;
            pktMsg.FormFieldArray = formControl.ConvertFromOutpost(pktMsg.MessageNumber, ref msgLines, FormProviders.PacForm);
            //pktMsg.ReceivedTime = packetMessage.ReceivedTime;
            pktMsg.CreateFileName();
            //string fileFolder = SharedData.UnsentMessagesFolder.Path;       // This line is different
            //pktMsg.Save(fileFolder);
            pktMsg.Save(folderPath);

            _logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} converted and saved");

            return;
        }

        private async void WebView_ScriptNotifyAsync(object sender, NotifyEventArgs e)
        {
            bool validUri = false;
            switch (e.CallingUri.LocalPath.ToString())
            {
                case "/PacFORMS/XSC_ICS-213_Message_v070628.html":
                    validUri = true;
                    break;
                case "/PacFORMS/XSC_EOC-213RR_v1708.html":
                    validUri = true;
                    break;
                case "/PacFORMS/XSC_OA_MuniStatus_v20130101.html":
                    validUri = true;
                    break;
                //case default:  use C# 7.1
                //    // Unauthorized page
                //    return;
            }
            if (!validUri)
                return;

            // open payload in SimpleMessage in FormsPage. Fill in To and send
            string payload = e.Value;

            IdentityViewModel viewModel = Singleton<IdentityViewModel>.Instance;
            PacketMessage packetMessage = new PacketMessage()
            {
                //ReceivedTime = DateTime.Parse(messageReceivedTime.Text),
                MessageFrom = viewModel.UseTacticalCallsign ? viewModel.TacticalCallsign : viewModel.UserCallsign,
                MessageNumber = Utilities.GetMessageNumberPacket(),
                MessageBody = payload,
                //TNCName = "E-Mail-" + Singleton<TNCSettingsViewModel>.Instance.CurrentMailAccount.MailUserName,
            };

            string folderPath = SharedData.UnsentMessagesFolder.Path;
            //CommunicationsService communicationsService = CommunicationsService.CreateInstance();
            //await communicationsService.CreatePacketMessageFromMessageAsync(packetMessage);
            await CreatePacketMessageFromMessageAsync(packetMessage, folderPath);

            string packetMessagePath = Path.Combine(folderPath, packetMessage.FileName);
            NavigationService.Navigate(typeof(CountyFormsPage), packetMessagePath);
        }
    }
}
