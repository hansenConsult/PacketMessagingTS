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
                MessageNumber = Helpers.Utilities.GetMessageNumberPacket(),
                TNCName = "E-Mail-" + Singleton<TNCSettingsViewModel>.Instance.CurrentMailAccount.MailUserName,

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
                await UpdateFileListAsync();
            }
            else if (_currentPivotItem.Name == "testReceive")
            {
                await UpdateTestFileListAsync();
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType">.txt or .log</param>
        /// <returns></returns>
        private async Task UpdateTestFileListAsync()
        {
            List<string> fileTypeFilter = new List<string>() { ".txt" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the user's archive folder
            StorageFolder storageFolder = SharedData.TestFilesFolder;
            StorageFileQueryResult results = storageFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
            if (files.Count > 0)
            {
                var observableCollection = new ObservableCollection<StorageFile>(files);
                TestFilesCollection.Source = observableCollection.OrderByDescending(f => f.Name);

                comboBoxTestFiles.SelectedIndex = _selectedFileIndex;
            }
        }

        private async void TestFilesComboBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedFile = (StorageFile)e.AddedItems[0];
                receivedMessage.Text = await FileIO.ReadTextAsync(_selectedFile);
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

        private void TestReceivedMessage_Click(object sender, RoutedEventArgs e)
        {
            bool TestWithoutConnecting = true;
            if (TestWithoutConnecting)
            {
                DateTime dateTime = DateTime.Now;
                PacketMessage packetMsg = new PacketMessage()
                {
                    MessageBody = receivedMessage.Text,
                    ReceivedTime = dateTime,
                    BBSName = BBSDefinitions.Instance.BBSDataList[2].Name,
                    TNCName = TNCDeviceArray.Instance.TNCDeviceList[1].Name,
                    MessageNumber = Helpers.Utilities.GetMessageNumberPacket(true),
                    Area = "",
                    MessageSize = receivedMessage.Text.Length,
                };

                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 24 Aug 2015 21:07:37 PDT\r\nFrom: kz6dm @w1xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM - 483_O/R_CityScan_Mountain View Emergency Declared: no\r\n\r\n!PACF!6DM - 483_O/R_CityScan_Mountain View Emergency Declared: no\r\n# CITY-SCAN UPDATE FLASH REPORT \r\n# JS-ver. PR-4.1-3.9, 01/11/15\r\n# FORMFILENAME: city-scan.html\r\nMsgNo: [6DM-483]\r\nD.: [OTHER]\r\nE.: [ROUTINE]\r\n1a.: [Mountain View]\r\n2.: [08/24/2015]\r\n3.: [2028]\r\n4.: [Poul Hansen]\r\n6.: [000-000-0000]\r\n9.: [no]\r\nOpDate: [08/24/2015]\r\nOpTime: [2030]\r\n#EOF\r\n";

                // Message
                //packetMsg.MessageSize = 30;
                //packetMsg.MessageBody = "Message #3 \r\nDate: Sun, 30 Aug 2015 12:42:30 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-185P:\r\nThis is a test message.\r\nNext line\r\n";

                //packetMsg.MessageSize = 30;
                //packetMsg.MessageBody = "Message #3 \r\nDate: Mon, 07 Sep 2015 20:40:57 PDT\r\nFrom: pktmon @w2xsc.ampr.org\r\nTo: k6taa @w3xsc\r\nCc: KI6SEP @W4XSC, KI6SEP@W3XSC, KZ6DM @W3XSC, WB6PVU@W3XSC, K6DLC @W2XSC,\r\n      KI6PUR@W3XSC, SNYEOC @W3XSC, AA6WK@W2XSC, KG6HI @W3XSC\r\nSubject: POV-5470: _O/R_SCCo ARES / RACES Packet Check-in Report For: Monday, September 07, 2015.Total = 18 call signs / 23 check - ins";

                //packetMsg.MessageBody = "Message #5 \r\nDate: Sun, 04 Oct 2015 14:47:44 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-571P_O/R_ICS213_Check-in 10/04/15-KZ6DM-Poul-Mountain View\r\n\r\n!PACF! 6DM-571P_O/R_ICS213_Check-in 10/04/15-KZ6DM-Poul-Mountain View\r\n# EOC MESSAGE FORM \r\n# JS-ver. PR-4.1-2.9, 01/11/15, \r\n# FORMFILENAME: Message.html \r\nMsgNo: [6DM-571P]\r\n1a.: [10/04/2015]\r\n1b.: [1444]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n6c.: [checked]\r\n7.: [Planning]\r\n9a.: [MTVEOC]\r\n8.: [Planning]\r\n9b.: [Radio]\r\n10.: [Check-in 10/04/15 - KZ6DM - Poul - Mountain View]\r\n12.: [\nCheck-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: [10/04/2015]\r\nOpTime: [1446]\r\n#EOF";
                //packetMsg.MessageBody = "Message #5 \r\nDate: Sun, 04 Oct 2015 14:47:44 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-571P_O/R_ICS213_Check-in 10/04/15 - KZ6DM - Poul - Mountain View\r\n\r\n!PACF!1234_O/P_ICS213_Damage Summary for Rex Manor subject 2\r\n# EOC MESSAGE FORM \r\n# JS-ver. MV/PR-4.2.4-2.18, 08/21/15\r\n# FORMFILENAME: CERT-DA-MTVUniversal-message.html\r\nMsgNo: [1234]\r\n1a.: [10/05/2015]\r\n1b.: [09:27]\r\n4.: [OTHER]\r\n5.: [PRIORITY]\r\n6a.: [No]\r\n6b.: [No]\r\n7.: [Planning]\r\n9a.: [Mountain View EOC]\r\n8.: [Planning]\r\n9b.: [Rex Manor]\r\n10.: [Damage Summary for Rex Manor subject 2]\r\n12.: [\n  F1  G2  W3  E4  C5  L6  Mod7  H8  I9  D10  T11 Mor12  A13  N14  O15  Nei16\nOther means other]\r\nRec-Sent: [Sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: [10/05/2015]\r\nOpTime: [09:29]\r\n# EOF";

                //packetMsg.MessageSize = 893;
                //packetMsg.MessageBody = "Message #1\r\nDate: Sun, 08 Nov 2015 13:44:51 PST\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-289P_O/R_EOC Logistics Req_Mountain View\r\n\r\rn!PACF!6DM-289P_O/R_EOC Logistics Req_Mountain View\r\n# JS:EOC Logistics Request (which4)\r\n# JS-ver. PR-4.1-2.5, 01/11/15\r\n# FORMFILENAME: EOCLogisticsRequest.html\r\n1: [6DM-289P]\r\n5: [true]\r\n8: [true]\r\n11: [true]\r\n13: [Check-in]\r\n14: [Poul Hansen]\r\n16: [Display List of Cities]\r\n17: [Mountain View]\r\n19: [Mountain View}10]\r\n\r\n28: [true]\r\n36: [\nCheck-in 11/08/15 - KZ6DM]\r\n37: [Jerry]\r\n39: [true]\r\n41: [true]\r\n42: [KZ6DM]\r\n43: [Poul Hansen]\r\n44: [11/08/2015{odate]\r\n45: [13:43{otime]\r\n# EOF";

                //packetMsg.MessageSize = 652;
                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 23 Nov 2015 19:02:04 PST\r\nFrom: pktmon @w2xsc.ampr.org\r\nTo: kz6dm @w3xsc.ampr.org\r\nSubject: DELIVERED:	6DM-648P_O/R_EOC Logistics Req_Mountain View\r\n\r\n!LMI!POV-5932P!DR!11/23/2015 6:59:22 PM\r\nYour Message\r\nTo: PKTMON\r\nSubject: 6DM-648P_O/R_EOC Logistics Req_Mountain View\r\nwas delivered on 11/23/2015 6:59:22 PM\r\nRecipient's Local Message ID: POV-5932P";

                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 01 Feb 2016 12:29:03 PST\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-907P_O/R_ICS213_Check-in\r\n!PACF!6DM-907P_O/R_ICS213_Check-in\r\n# EOC MESSAGE FORM \r\n# JS-ver. PR-4.1-3.1, 01/19/17,\r\n# FORMFILENAME: Message.html\r\nMsgNo: [6DM - 907P]\r\n1a.: [02/01/2016]\r\n1b.: [1219]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n7.: [Operations]\r\n9a.: [MTVEOC]\r\n8.: [Operations]\r\n9b.: [HOSECM]\r\n10.: [Check-in]\r\n12.: [\\nMonday Check-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: []\r\nOpTime: []\r\n# EOF\r\n";

                //				packetMsg.MessageSize = 4002;
                //				packetMsg.MessageBody = @"Message #4 
                //Date: Sun, 06 Mar 2016 18:00:48 PST
                //From: xsceoc@w1xsc.ampr.org
                //To: xscperm
                //Subject: SCCo Packet Tactical Calls v160306

                //# Primary Tactical Calls and BBSs for Santa Clara County Cities/Agencies
                //#=======================================================================
                //# Last revised:  06-Mar-2016 at 17:17 by Michael Fox, N6MEF
                //#
                //# IMPORTANT:  Post a copy of this file in your radio room and retain a copy on
                //# your packet computer. The suggested location is the Outpost Archive folder.
                //#
                //# BBS Naming/Addressing:
                //#  BBS Call Sign:    W#XSC  (where # is 1-6)     (Ex: w1xsc, w2xsc, ...)
                //#  AX.25 connect:    <BBScall>-1                 (Ex: connect w1xsc-1)
                //#  AMPRnet/Internet: <BBScall>.ampr.org          (Ex: w2xsc.ampr.org)	
                //#  BBS Network:      <BBScall>.#nca.ca.usa.noam  (Ex: w3xsc.#nca.ca.usa.noam)
                //#
                //# Usage:
                //# -- All users:  connect to the primary BBS for your city/agency.  If the
                //#    primary is down, connect to the secondary.
                //# -- Individuals:  connect using your FCC call sign
                //# -- Cities/Agencies:  connect using a tactical call assigned by your EC/CRO
                //#
                //# Each line in the table below contains:
                //# 1)  Primary tactical call sign for the agency (typically for the DOC/EOC)
                //# 2)  Agency name
                //# 3)  3-letter tactical prefix assigned to agency
                //# 4)  Primary BBS name (agency normally connects to and receives mail here)
                //# 5)  Secondary BBS name (agency connects here if primary BBS is down)
                //#
                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	-----------------------------	---	------	------
                //ARCEOC		American Red Cross		ARC	W1XSC	W4XSC
                //SCUCIC		Cal-Fire VIPs Santa Clara Unit	SCU	W2XSC	W1XSC
                //CBLEOC		Campbell, City of		CBL	W1XSC	W4XSC
                //CUPEOC		Cupertino, City of		CUP	W1XSC	W4XSC
                //GILEOC		Gilroy, City of			GIL	W2XSC	W1XSC
                //HOSDOC		Hospitals, all SCCo		HOS	W2XSC	W1XSC
                //LMPEOC		Loma Prieta Region		LMP	W2XSC	W1XSC
                //LOSEOC		Los Altos, City of		LOS	W3XSC	W1XSC
                //LAHEOC		Los Altos Hills, Town of	LAH	W3XSC	W1XSC
                //LGTEOC		Los Gatos, Town of		LGT	W1XSC	W4XSC
                //MLPEOC		Milpitas, City of		MLP	W4XSC	W1XSC
                //MSOEOC		Monte Sereno, City of		MSO	W1XSC	W4XSC
                //MRGEOC		Morgan Hill, City of		MRG	W2XSC	W1XSC
                //MTVEOC		Mountain View, City of		MTV	W3XSC	W1XSC
                //NAMEOC		NASA - Ames			NAM	W3XSC	W1XSC
                //PAFEOC		Palo Alto, City of		PAF	W3XSC	W1XSC
                //SJCEOC		San Jose, City of		SJC	W1XSC	W2XSC
                //SJWEOC		San Jose Water Co		SJW	W1XSC	W2XSC
                //SNCEOC		Santa Clara, City of		SNC	W1XSC	W2XSC
                //XSCEOC		Santa Clara County		XSC	W1XSC	W4XSC
                //VWDEOC		SC Valley Water District	VWD	W1XSC	W2XSC
                //SAREOC		Saratoga, City of		SAR	W1XSC	W4XSC
                //STUEOC		Stanford University		STU	W4XSC	W1XSC
                //SNYEOC		Sunnyvale, City of		SNY	W1XSC	W3XSC
                //#
                //# Each of the above agencies also has ten (10) numbered tactical call signs
                //# which begin with their assigned prefix and end with the numbers 001 - 010.
                //#
                //# Each of the above agencies may also have additional tactical call signs,
                //# defined by the EC/CRO, which begin with the agency's assigned prefix.
                //#
                //#
                //# Primary Tactical Calls for Other (non-SCCo) Agencies
                //#=====================================================
                //#
                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	--------------------------	---	------	------
                //COSEOC		CalEMA - Coastal Region		COS	W1XSC
                //XALEOC		Alameda County			XAL	W4XSC
                //XCCEOC		Contra Costa County		XCC	W1XSC
                //XMREOC		Marin County			XMR	W4XSC
                //XMYEOC		Monterey County			XMY	W2XSC
                //XBEEOC		San Benito County		XBE	W2XSC
                //XSFEOC		San Francisco County		XSF	W4XSC
                //XSMEOC		San Mateo County		XSM	W4XSC
                //XCZEOC		Santa Cruz County		XCZ	W2XSC
                //#
                //#
                //# Other Special Tactical Prefixes
                //#================================
                //#
                //#Tactical	Function			Pfx	Pri	Sec
                //#--------	--------------------------	---	------	------
                //PKT***		SCCo Packet Net Check-ins	PKT	W2XSC	W4XSC
                //#
                //#
                //#======== End Of Primary Tactical Calls ===================
                //		";

                ///*				packetMsg.MessageBody = @"Message #4 
                //Date: Sun, 06 Mar 2016 18:00:48 PST
                //From: xsceoc@w1xsc.ampr.org
                //To: xscperm
                //Subject: SCCo Packet Tactical Calls v160306

                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	-----------------------------	---	------	------
                //ARCEOC		American Red Cross		ARC	W1XSC	W4XSC
                //SCUCIC		Cal-Fire VIPs Santa Clara Unit	SCU	W2XSC	W1XSC
                //				";
                //*/

                //                packetMsg.MessageBody = @"Message #1 
                //Date: Wed, 20 Sep 2017 20:43:47 PDT
                //From: kz6dm @w3xsc.ampr.org
                //To: kz6dm
                //Subject: 6DM-264P_O/R_EOC213RR_Incident Name

                //!PACF! 6DM-264P_O/R_EOC213RR_Incident Name
                //# JS:EOC Resource Request (which4)
                //# JS-ver. PR-4.3-2.8, 09/15/17
                //# FORMFILENAME: XSC_EOC-213RR_v1706.html
                //1: [6DM-264P]
                //6: [true]
                //9: [true]
                //13: [\nIncident Name]
                //14: [09/25/2017]
                //15: [16:10]
                //16: [\nRequested by]
                //17: [\nPrepared by]
                //18: [\nApproved by]
                //19: [\n1]
                //20: [\nDescription]
                //21: [\nArrival]
                //25: [true]
                //26: [\n20]
                //27: [\nDelivery to]
                //28: [\nLocvation]
                //29: [\nSubstitutes]
                //32: [true]
                //33: [Propane]
                //34: [true]
                //39: [\nInstructions]
                //41: [true]
                //43: [true]
                //44: [KZ6DM]
                //45: [Poul Hansen]
                //46: [09/25/2017{odate]
                //47: [16:10{otime]
                //#EOF

                //";
                Services.CommunicationsService.CommunicationsService communicationService = Services.CommunicationsService.CommunicationsService.CreateInstance();
                communicationService._packetMessagesReceived.Add(packetMsg);
                communicationService.ProcessReceivedMessagesAsync();
            }
            else
            {
                //BBSConnect();
            }

        }

        private async void AppBarButtonTest_SaveFileAsync(object sender, RoutedEventArgs e)
        {
            // Make sure the extension is .txt
            int index = textBoxTestFileName.Text.IndexOf('.');
            if (index < 0)
            {
                textBoxTestFileName.Text += ".txt";
            }
            else if (textBoxTestFileName.Text.Substring(index) != ".txt")
            {
                textBoxTestFileName.Text = textBoxTestFileName.Text.Substring(0, index) + ".txt";
            }
            StorageFile file = await SharedData.TestFilesFolder.CreateFileAsync(textBoxTestFileName.Text);
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, receivedMessage.Text);
            }
            else
            {
                return;
            }
        }

        private async void AppBarButtonTest_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            StorageFile file = await SharedData.TestFilesFolder.TryGetItemAsync(textBoxTestFileName.Text) as StorageFile;
            if (file != null)
            {
                receivedMessage.Text = await FileIO.ReadTextAsync(file);
            }
            else
            {
                return;
            }
        }

        private async void AppBarButton_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(textBoxTestFileName.Text);
            if (file != null)
            {
                receivedMessage.Text = await FileIO.ReadTextAsync(file);
            }
            else
            {
                return;
            }
        }

        private async void AppBarButton_SaveFileAsync(object sender, RoutedEventArgs e)
        {
            //if (_currentPivotItem.Name == "testReceive")
            //{
            //    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            //    string fileName = textBoxTestFileName.Text + ".txt";
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
