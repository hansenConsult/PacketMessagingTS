using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using PacketMessagingTS.ViewModels;
using Windows.Storage;
using PacketMessagingTS.Helpers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage.Search;
using System.Collections.ObjectModel;
using PacketMessagingTS.Models;
using FormControlBaseClass;
using System.Text;
using PacketMessagingTS.Services.CommunicationsService;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToolsPage : Page
    {
        public ToolsViewModel _toolsViewModel { get; } = new ToolsViewModel();

        StorageFile _selectedFile;
        private int _selectedFileIndex;
        PivotItem _currentPivotItem;



        public ToolsPage()
        {
            this.InitializeComponent();
        }

        public object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        public List<T> Sort_List<T>(List<T> data)
        {
            List<T> data_sorted = new List<T>();

            data_sorted = (from n in data
                           orderby GetDynamicSortProperty(n, "Time") ascending
                           select n).ToList();
            return data_sorted;
        }

        private async Task UpdateFileListAsync()
        {
            List<string> fileTypeFilter = new List<string>() { ".log" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the user's archive folder
            StorageFileQueryResult results = SharedData.MetroLogsFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();

            var observableCollection = new ObservableCollection<StorageFile>(files);
            LogFilesCollection.Source = observableCollection.OrderByDescending(f => f.Name);

            logFilesComboBox.SelectedIndex = _selectedFileIndex;
        }

        private async void logFilesComboBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedFile = (StorageFile)e.AddedItems[0];
                logFileTextBox.Text = await FileIO.ReadTextAsync(_selectedFile);
            }
            catch (UnauthorizedAccessException)
            {
                StorageFile fileCopy;
                try
                {
                    // Delete any file copy that for some reason was not deleted
                    fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                    await fileCopy.DeleteAsync();
                }
                catch
                { }
                // Create a copy of an open log file because it can not be read directly
                await _selectedFile.CopyAsync(SharedData.MetroLogsFolder, _selectedFile.Name + "-Copy");
                fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                logFileTextBox.Text = await FileIO.ReadTextAsync(fileCopy);
                await fileCopy.DeleteAsync();
            }
            catch (COMException)
            {

            }
        }

        private async void ConvertToForm_Click(object sender, RoutedEventArgs e)
        {
            //packetMsg.MessageBody = "
            //Message #1 \r\n
            //Date: Mon, 01 Feb 2016 12:29:03 PST\r\n
            //From: kz6dm @w3xsc.ampr.org\r\n
            //To: kz6dm\r\n
            //Subject: 6DM-907P_O/R_ICS213_Check-in\r\n
            //!PACF!6DM-907P_O/R_ICS213_Check-in\r\n
            //# EOC MESSAGE FORM \r\n
            //# JS-ver. PR-4.1-3.1, 01/19/17,\r\n
            //# FORMFILENAME: Message.html\r\n
            //MsgNo: [6DM - 907P]\r\n
            //1a.: [02/01/2016]\r\n
            //1b.: [1219]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n7.: [Operations]\r\n9a.: [MTVEOC]\r\n8.: [Operations]\r\n9b.: [HOSECM]\r\n10.: [Check-in]\r\n12.: [\\nMonday Check-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: []\r\nOpTime: []\r\n# EOF\r\n";

            // read data
            //!PACF!6DM - 681P_O / R_ICS213_ghjhgj
            //# EOC MESSAGE FORM 
            //# JS-ver. PR-4.1-3.1, 01/19/17
            //# FORMFILENAME: Message.html
            //MsgNo: [6DM - 681P]
            //1a.: [03/01/18]
            //1b.: [1650]
            //4.: [OTHER]
            //5.: [ROUTINE]
            //9a.: [gfhjgfj]
            //9b.: [gfhjgfhj]
            //10.: [ghjhgj]
            //12.: [\nghjghj]
            //Rec-Sent: [sent]
            //Method: [Other]
            //Other: [Packet]
            //OpCall: [KZ6DM]
            //OpName: [Poul Hansen]
            //OpDate: [03/01/2018]
            //OpTime: [1652]
            //#EOF

            // Find type
            // do a navigate to forms with index and form id (maybe file name}
            PacketMessage message = new PacketMessage()
            {
                ReceivedTime = DateTime.Parse(messageReceivedTime.Text),
                MessageNumber = await Helpers.Utilities.GetMessageNumberPacketAsync(),
                TNCName = "E-Mail",
            };
            message.MessageBody = $"Date: {message.ReceivedTime}\r\n";
            message.MessageBody += $"From: {messageTo.Text}\r\n";
            message.MessageBody += $"To: {messageFrom.Text}\r\n";
            message.MessageBody += $"Subject: {messageSubject.Text}\r\n";
            message.MessageBody += PacFormText.Text;
            CommunicationsService communicationsService = CommunicationsService.CreateInstance();
            await communicationsService.CreatePacketMessageFromMessageAsync(message);
        }

        private void ConvertFromBase64_Click(object sender, RoutedEventArgs e)
        {
            byte[] message = Convert.FromBase64String(PacFormText.Text);
            string decodedString = Encoding.UTF8.GetString(message);
            PacFormText.Text = decodedString;
        }

        private async void ToolsPagePivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _currentPivotItem = (PivotItem)e.AddedItems[0];

            if (_currentPivotItem.Name == "logFile")
            {
//                OpenTestMessageFile.Visibility = Visibility.Collapsed;
                await UpdateFileListAsync();
            }
            //else if (_currentPivotItem.Name == "testReceive")
            //{
            //    OpenTestMessageFile.Visibility = Visibility.Visible;
            //    await UpdateTestFileListAsync();
            //}
            //else if (_currentPivotItem.Name == "ics309")
            //{
            //    ToolsPageViewModel.ToolsPageCommLogPartViewModel viewModel = ToolsPageViewModel.toolsPageCommLogPartViewModel;
            //    incidentName.Text = viewModel.IncidentName;
            //    operationalPeriod.Text = FormatDateTime(viewModel.OperationalPeriodStart) + " to " + FormatDateTime(viewModel.OperationalPeriodEnd);
            //    radioNetName.Text = viewModel.RadioNetName;
            //    radioOperator.Text = $"{SettingsPageViewModel.IdentityPartViewModel.UserName}, {SettingsPageViewModel.IdentityPartViewModel.UserCallsign}";
            //    viewModel.DateTimePrepared = DateTime.Now;
            //    dateTimePrepared.Text = FormatDateTime(viewModel.DateTimePrepared);
            //    preparedByNameCallsign.Text = radioOperator.Text;
            //    ToolsPageViewModel.toolsPageCommLogPartViewModel.TotalPages = 1;
            //    ToolsPageViewModel.toolsPageCommLogPartViewModel.PageNo = 1;
            //    pageNoOf.Text = ToolsPageViewModel.toolsPageCommLogPartViewModel.PageNoAsString;

            //    await BuildLogDataSetAsync(viewModel.OperationalPeriodStart, viewModel.OperationalPeriodEnd);
            //}
        }

        private async Task BuildLogDataSetAsync(DateTime startTime, DateTime endTime)
        {
//            _toolsViewModel.ToolsPageCommLogPartViewModel viewModel = ToolsPageViewModel.toolsPageCommLogPartViewModel;
            CommLog.Instance.CommLogEntryList.Clear();
            // Get messages in the InBox and the Sent Messages folder
            List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages(SharedData.ReceivedMessagesFolder);
            foreach (PacketMessage packetMessage in messagesInFolder)
            {
                CommLog.Instance.AddCommLogEntry(packetMessage, startTime, endTime);
            }
            List<PacketMessage> messagesSentInFolder = await PacketMessage.GetPacketMessages(SharedData.SentMessagesFolder);
            //List<PacketMessage> messages = (List<PacketMessage>)messagesInFolder.Concat(messagesSentInFolder);
            //messagesInFolder.Concat(messagesSentInFolder);
            foreach (PacketMessage packetMessage in messagesSentInFolder)
            {
                CommLog.Instance.AddCommLogEntry(packetMessage, startTime, endTime);
            }
            List<CommLogEntry> sortedList = Sort_List(CommLog.Instance.CommLogEntryList);

            CommLogMessagesCollection.Source = new ObservableCollection<CommLogEntry>(sortedList);

        }

        private async void OperationalPeriod_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
//            ToolsPageViewModel.ToolsPageCommLogPartViewModel viewModel = ToolsPageViewModel.toolsPageCommLogPartViewModel;

            string opPeriod = operationalPeriod.Text;
            var startStop = opPeriod.Split(new string[] { "to", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (startStop != null && (startStop.Count() != 3 && startStop.Count() != 4))
                return;

            int endTimeIndex = 3;
            if (startStop.Count() == 3)
            {
                endTimeIndex = 2;
            }

            if (startStop[1].Length != 4)
                return;

            if (startStop[endTimeIndex].Length != 4)
                return;

            string dateTime = startStop[0] + " " + startStop[1].Insert(2, ":");

            DateTime operationalPeriodStart;
            if (!DateTime.TryParse(dateTime, out operationalPeriodStart))
                return;

            if (startStop.Count() == 3)
            {
                dateTime = startStop[0] + " " + startStop[endTimeIndex].Insert(2, ":");
            }
            else
            {
                dateTime = startStop[2] + " " + startStop[endTimeIndex].Insert(2, ":");
            }

            DateTime operationalPeriodEnd;
            if (!DateTime.TryParse(dateTime, out operationalPeriodEnd))
                return;

            if (operationalPeriodEnd < operationalPeriodStart)
                return;

            _toolsViewModel.OperationalPeriodStart = operationalPeriodStart;
            _toolsViewModel.OperationalPeriodEnd = operationalPeriodEnd;

            await BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
        }

        private void IncidentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _toolsViewModel.IncidentName = incidentName.Text;
        }

        private void RadioNetName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _toolsViewModel.RadioNetName = radioNetName.Text;
        }


        private async void AppBarButton_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //StorageFile file = await storageFolder.GetFileAsync(textBoxFileName.Text);
            //if (file != null)
            //{
            //    receivedMessage.Text = await FileIO.ReadTextAsync(file);
            //}
            //else
            //{
            //    return;
            //}

        }

        private async void AppBarButton_SaveFileAsync(object sender, RoutedEventArgs e)
        {
            //if (_currentPivotItem.Name == "testReceive")
            //{
            //    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //    string fileName = textBoxFileName.Text + ".txt";
            //    StorageFile file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            //    if (file != null)
            //    {
            //        await FileIO.WriteTextAsync(file, receivedMessage.Text);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
        }

        private async void AppBarButton_DeleteFileAsync(object sender, RoutedEventArgs e)
        {
            //if (_currentPivotItem.Name == "testReceive")
            //{
            //    StorageFile deleteFile = comboBoxTestFiles.SelectedItem as StorageFile;

            //    await deleteFile.DeleteAsync();

            //    await UpdateTestFileListAsync();
            //}
            //else
            {
                StorageFile deleteFile = logFilesComboBox.SelectedItem as StorageFile;

                await deleteFile.DeleteAsync();

                await UpdateFileListAsync();
            }
        }

    }

}
