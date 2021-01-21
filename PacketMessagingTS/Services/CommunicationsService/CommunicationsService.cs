using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormControlBaseClass;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Helpers.PrintHelpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services.SMTPClient;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Views;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using Windows.ApplicationModel.Email;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Core;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

namespace PacketMessagingTS.Services.CommunicationsService
{
    public class CommunicationsService
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CommunicationsService>();
        private readonly LogHelper _logHelper = new LogHelper(log);

        //ViewLifetimeControl rxTxStatusWindow = null;
        //Collection<DeviceListEntry> _listOfDevices;

        public List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
        private List<PacketMessage> _packetMessagesToSend = new List<PacketMessage>();

        //private static readonly Object singletonCreationLock = new Object();
        public static CommunicationsService Current = null;
        //static bool _deviceFound = false;
        //public StreamSocket _socket = null;
        public SerialPort _serialPort;
        private TNCInterface _tncInterface = null;



        public CommunicationsService()
        {
            Current = this;
        }

        //public static CommunicationsService CreateInstance()
        //{
        //    if (CommunicationsServiceInstance is null)
        //    {
        //        lock (singletonCreationLock)
        //        {
        //            if (CommunicationsServiceInstance is null)
        //            {
        //                CommunicationsServiceInstance = new CommunicationsService();
        //            }
        //        }
        //    }
        //    return CommunicationsServiceInstance;
        //}

        //private static RxTxStatusPage rxTxStatusPage;

        public void AddRxTxStatusAsync(string text)
        {

            //if (Singleton<RxTxStatViewModel>.Instance.Dispatcher is null)
//            if (RxTxStatusPage.rxtxStatusPage.Dispatcher is null)
//                return;
            //Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = text;
            //Thread.Sleep(0); No effect
            //{
            //await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            ////await rxTxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            ////await Singleton<RxTxStatusViewModel>.Instance.StatusPage.Dispatcher.RunTaskAsync( async () =>
//            await RxTxStatusPage.rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
                //RxTxStatusPage.rxtxStatusPage.RxTxStatusViewmodel.AppendRxTxStatus = text;
                RxTxStatusPage.Current.AddTextToStatusWindow(text);
                //Singleton<RxTxStatViewModel>.Instance.StatusPage.ScrollText();
//            });
        }

        public void AbortConnection()
        {
            _logHelper.Log(LogLevel.Info, $"Connection aborted.");
            _tncInterface?.AbortConnection();
        }

        public async Task CreatePacketMessageFromMessageAsync(PacketMessage pktMsg)
        {
            FormControlBase formControl = new MessageControl();
            // test for packet form!!
            //pktMsg.PacFormType = PacForms.Message;
            //pktMsg.PacFormName = "SimpleMessage";
            // Save the original message for post processing (tab characters are lost in the displayed message)
            string[] msgLines = pktMsg.MessageBody.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.None);

            bool subjectFound = false;
            for (int i = 0; i < Math.Min(msgLines.Length, 20); i++)
            {
                if (msgLines[i].StartsWith("Date:"))
                {
                    string startpos = "Date: ";
                    bool success = DateTime.TryParse(msgLines[i].Substring(startpos.Length), out DateTime JNOSDate);
                    pktMsg.JNOSDate = (success ? JNOSDate : (DateTime?)null);
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
                    //    pktMsg.FormProviderIndex = 0;
                }
            }

            //formControl.FormProvider = FormProviders.PacForm;
            pktMsg.FormProvider = FormProviders.PacForm;        // TODO update with real provider
            pktMsg.FormControlType = formControl.FormControlType;

            pktMsg.PacFormName = formControl.GetPacFormName();
            pktMsg.FormFieldArray = formControl.ConvertFromOutpost(pktMsg.MessageNumber, ref msgLines, pktMsg.FormProvider);
            //pktMsg.ReceivedTime = packetMessage.ReceivedTime;
            if (!pktMsg.CreateFileName())
            {
                throw new Exception();
            }
            string fileFolder = SharedData.ReceivedMessagesFolder.Path;
            pktMsg.Save(fileFolder);

            _logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} converted and saved");

            return;
        }

        /*
                 Mail area: kz6dm
            1 message  -  1 new

            St.  #  TO            FROM     DATE   SIZE SUBJECT
            > N   1 kz6dm         kz6dm    Sep 14  662 DELIVERED: 5DM-002_O/R_OAAlliedHeal
            (#1) >
            R 1
            Message #1 
            Date: Fri, 14 Sep 2018 18:51:37 PDT
            From: kz6dm@w3xsc.ampr.org
            To: kz6dm@w3xsc.ampr.org
            Subject: DELIVERED: 5DM-002_O/R_OAAlliedHealth_Facility Type_Facility Name

            !LMI!6DM-526P!DR!9/14/2018 6:51:22 PM
            Your Message
            To: KZ6DM
            Subject: 5DM-002_O/R_OAAlliedHealth_Facility Type_Facility Name
            was delivered on 9/14/2018 6:51:22 PM
            Recipient's Local Message ID: 6DM-526P

            (#1) >
            K 1
            Msg 1 Killed.

            
        Your message was delivered to:
        kz6dm@w3xsc.ampr.org at 9/14/2018 6:51:22 PM
        kz6dm@w3xsc.ampr.org assigned Msg ID: 6DM-526P
        */

        private async Task ProcessMessagesMarkedDeliveredAsync(PacketMessage pktMsg)
        {
            var formField = pktMsg.FormFieldArray.FirstOrDefault(x => x.ControlName == "messageBody");
            if (string.IsNullOrEmpty(formField.ControlContent))
            {
                formField = pktMsg.FormFieldArray.FirstOrDefault(x => x.ControlName == "richTextMessageBody");
            }
            if (formField.ControlContent.Contains("!LMI!"))
            {
                string[] searchStrings = new string[] { "Subject: ", "was delivered on ", "Recipient's Local Message ID: " };
                DateTime receiveTime = DateTime.Now;
                string receiversMessageId = "", sendersMessageId = "", senderSubject = "";
                var messageLines = formField.ControlContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in messageLines)
                {
                    if (line.Contains(searchStrings[0]))
                    {
                        senderSubject = line.Substring(searchStrings[0].Length);
                        int indexOfUnderscore = line.IndexOf('_');
                        int indexOfDelivered = line.IndexOf(searchStrings[0]);
                        if (indexOfUnderscore >= 0)
                        {
                            sendersMessageId = line.Substring(indexOfDelivered + searchStrings[0].Length, indexOfUnderscore - (indexOfDelivered + searchStrings[0].Length));
                        }
                    }
                    else if (line.Contains(searchStrings[1]))
                    {
                        int indexOfDeliveryTime = line.IndexOf(searchStrings[1]);
                        if (indexOfDeliveryTime >= 0)
                        {
                            string s = line.Substring(indexOfDeliveryTime + searchStrings[1].Length);
                            receiveTime = DateTime.Parse(s);
                        }
                    }
                    else if (line.Contains(searchStrings[2]))
                    {
                        receiversMessageId = line.Substring(line.IndexOf(searchStrings[2]) + searchStrings[2].Length);
                    }
                }
                StringBuilder messageBody = new StringBuilder();
                messageBody.AppendLine("Your message was delivered to:");
                messageBody.AppendLine($"{pktMsg.MessageFrom} at {receiveTime}");
                messageBody.AppendLine($"{pktMsg.MessageFrom} assigned Msg ID: {receiversMessageId}");
                formField.ControlContent = messageBody.ToString();

                List<string> fileTypeFilter = new List<string>() { ".xml" };
                QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

                // Get the files in the user's sent folder
                StorageFileQueryResult results = SharedData.SentMessagesFolder.CreateFileQueryWithOptions(queryOptions);
                // Iterate over the results
                IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
                foreach (StorageFile file in files)
                {
                    // Update sent form with receivers message number and receive time
                    if (file.Name.Contains(sendersMessageId))
                    {
                        PacketMessage packetMessage = PacketMessage.Open(file.Path);
                        if (packetMessage is null)
                        {
                            _logHelper.Log(LogLevel.Error, $"Failed to open {file.Path}");
                        }
                        else
                        {
                            if (packetMessage.MessageNumber == sendersMessageId && packetMessage.Subject == senderSubject)
                            {
                                formField = packetMessage.FormFieldArray.FirstOrDefault(x => x.ControlName == "receiverMsgNo");
                                if (formField != null)
                                {
                                    formField.ControlContent = receiversMessageId;
                                }
                                packetMessage.ReceiverMessageNumber = receiversMessageId;
                                packetMessage.ReceivedTime = receiveTime;
                                packetMessage.Save(SharedData.SentMessagesFolder.Path);
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static string NormalizeEmailField(string emailAddress)
        {
            int startIndex = emailAddress.IndexOf('<');
            if (startIndex < 0)
            {
                return emailAddress.Trim();
            }
            else
            {
                int stopIndex = emailAddress.IndexOf('>');
                if (stopIndex < 0)
                    return "";

                return emailAddress.Substring(startIndex + 1, stopIndex - startIndex - 1).Trim();
            }
        }

        public async void ProcessReceivedMessagesAsync()
		{
			if (_packetMessagesReceived.Count() > 0)
			{
                bool updateBulletinList = false;
				foreach (PacketMessage packetMessageOutpost in _packetMessagesReceived)
				{
					FormControlBase formControl = new MessageControl();
                    // test for packet form!!
                    PacketMessage pktMsg = new PacketMessage()
                    {
                        BBSName = packetMessageOutpost.BBSName,
                        TNCName = packetMessageOutpost.TNCName,
                        MessageSize = packetMessageOutpost.MessageSize,
                        MessageNumber = packetMessageOutpost.MessageNumber,
                        ReceivedTime = packetMessageOutpost.ReceivedTime,
                        CreateTime = DateTime.Now,
                        Area = packetMessageOutpost.Area,
                        // Save the original message for post processing (tab characters are lost in the displayed message)
                        MessageBody = packetMessageOutpost.MessageBody,
                        MessageState = MessageState.Locked,
                        MessageOpened = false,
                        MessageOrigin = MessageOriginHelper.MessageOrigin.Received,
                    };
                    string[] msgLines = packetMessageOutpost.MessageBody.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.None);
                    // Check if base64 encoded
                    int startOfMessage = 0;
                    int startOfMessage1 = 0;
                    int startOfMessage2 = 0;
                    int endOfMessage = 0;
                    bool dateFound = false;
                    bool subjectFound = false;
                    for (int k = 0; k < msgLines.Length; k++)
                    {
                        if (msgLines[k].StartsWith("Date:"))
                        {
                            dateFound = true;
                            startOfMessage1 = k + 1;
                        }
                        if (msgLines[k].StartsWith("Subject:"))
                        {
                            subjectFound = true;
                            startOfMessage2 = k + 1;
                        }
                        if (dateFound && subjectFound)
                        {
                            break;
                        }
                    }
                    startOfMessage = Math.Max(startOfMessage1, startOfMessage2);
                    endOfMessage = msgLines.Length - 1;
                    try
                    {
                        // Process encoded message
                        string message = "";
                        for (int j = startOfMessage; j <= endOfMessage; j++)
                        {
                            message += msgLines[j];
                        }
                        const string outpostEncodedMarker = "!B64!";
                        if (message.StartsWith(outpostEncodedMarker))
                        {
                            message = message.Substring(outpostEncodedMarker.Length);
                        }
                        byte[] messageText = Convert.FromBase64String(message);
                        string decodedString = Encoding.UTF8.GetString(messageText);

                        List<string> msgLinesList = msgLines.ToList();
                        msgLinesList.RemoveRange(startOfMessage, endOfMessage - startOfMessage + 1);
                        string[] decodedMsgLines = decodedString.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        msgLinesList.InsertRange(startOfMessage, decodedMsgLines);
                        msgLines = msgLinesList.ToArray();
                        _logHelper.Log(LogLevel.Info, "This message was an encoded message");
                    }
                    catch (FormatException)
                    {
                        // Not an encoded message
                        //_logHelper.Log(LogLevel.Info, "Not an encoded message");
                    }
                    bool toFound = false;
					subjectFound = false;
                    string prefix = "";
					for (int i = 0; i < Math.Min(msgLines.Length, 20); i++)
					{
                        if (msgLines[i].StartsWith("From:"))
                        {
                            pktMsg.MessageFrom = NormalizeEmailField(msgLines[i].Substring(6));
                        }
                        else if (!toFound && msgLines[i].StartsWith("To:"))
                        {
                            pktMsg.MessageTo = NormalizeEmailField(msgLines[i].Substring(4));
                            toFound = true;
                        }
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
                            if (pktMsg.Subject[3] == '-')
                            {
                                prefix = pktMsg.Subject.Substring(0, 3);
                            }
                            //pktMsg.MessageSubject = pktMsg.MessageSubject.Replace('\t', ' ');
                            subjectFound = true;
                        }
                        else if (msgLines[i].StartsWith("Date:"))
                        {
                            pktMsg.JNOSDate = DateTime.Parse(msgLines[i].Substring(10, 21));
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
                            pktMsg.SenderMessageNumber = FormControlBase.GetOutpostValue(msgLines[i + 1]);
                            pktMsg.FormProvider = FormProviders.PacForm;    // TODO update with real provider
                            break;
                        }
                        else if (msgLines[i].StartsWith("#T:"))
                        {
                            string[] fileNameString = msgLines[i].Split(new char[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
                            string formName = fileNameString[1];
                            formName.Trim();
                            pktMsg.PacFormName = formName;

                            formControl = FormsViewModel.CreateFormControlInstance(pktMsg.PacFormName, pktMsg.MessageState);
                            if (formControl is null)
                            {
                                _logHelper.Log(LogLevel.Error, $"Form {pktMsg.PacFormName} not found");
                                await ContentDialogs.ShowSingleButtonContentDialogAsync($"Form {pktMsg.PacFormName} not found");
                                return;
                            }
                            pktMsg.SenderMessageNumber = FormControlBase.GetOutpostValue(msgLines[i + 2]);
                            pktMsg.FormProvider = FormProviders.PacItForm;
                            break;
                        }
                    }
                    
                    //pktMsg.MessageNumber = GetMessageNumberPacket();		// Filled in BBS connection
                    pktMsg.PacFormType = formControl.PacFormType;
                    pktMsg.PacFormName = formControl.GetPacFormName();
                    pktMsg.FormControlType = formControl.FormControlType;
                    pktMsg.FormFieldArray = formControl.ConvertFromOutpost(pktMsg.MessageNumber, ref msgLines, pktMsg.FormProvider);
					//if (pktMsg.ReceivedTime != null)
					//{
					//	DateTime dateTime = (DateTime)pktMsg.ReceivedTime;
					//}
					if (!pktMsg.CreateFileName())
                    {
                        _logHelper.Log(LogLevel.Error, $"Error in Create FileName(), {pktMsg.MessageNumber}, {pktMsg.PacFormType}");
                        throw new Exception();
                    }

                    AddressBook.Instance.UpdateLastUsedBBS(pktMsg.MessageFrom, prefix);
                    _logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} received");

                    // If the received message is a delivery confirmation, update receivers message number in the original sent message
                    if (!string.IsNullOrEmpty(pktMsg.Subject) && pktMsg.Subject.Contains("DELIVERED:"))
					{
                        await ProcessMessagesMarkedDeliveredAsync(pktMsg);
					}
                    pktMsg.Save(SharedData.ReceivedMessagesFolder.Path);

                    if (!string.IsNullOrEmpty(pktMsg.Area))
                    {
                        updateBulletinList |= true;     // Does this work with xscevent

                        if (pktMsg.Area.ToLower() == "xscperm")
                        {
                            if (pktMsg.Subject.ToLower().Contains("scco packet frequencies"))
                            {
                                await BBSDefinitions.CreateFromBulletinAsync(pktMsg);
                            }
                            else if (pktMsg.Subject.ToLower().Contains("scco packet tactical calls"))
                            {
                                await TacticalCallsigns.CreatePacketTacticalCallsignsFromBulletinAsync(pktMsg);
                            }
                        }
                    }

                    // Do printing if requested
                    if (!string.IsNullOrEmpty(pktMsg.Subject) && !pktMsg.Subject.Contains("DELIVERED:")
                        && string.IsNullOrEmpty(pktMsg.Area) && Singleton<SettingsViewModel>.Instance.PrintReceivedMessages)
                    {
                        _logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} to be printed");

                        pktMsg.Save(SharedData.PrintMessagesFolder.Path);

                        SettingsViewModel settingsViewModel = Singleton<SettingsViewModel>.Instance;

                        Singleton<PrintQueue>.Instance.AddToPrintQueue(pktMsg.FileName, settingsViewModel.ReceivedCopyNamesAsArray());

                        // Test
                        await Singleton<PrintQueue>.Instance.PrintToDestinationsAsync();
                        //await Singleton<PrintQueue>.Instance.BackgroundPrintingTrigger.RequestAsync();
                    }
                }
                //RefreshDataGrid();      // Display newly added messages
                if (updateBulletinList)
                {
                    await Singleton<MainViewModel>.Instance.UpdateDownloadedBulletinsAsync();
                }
            }
        }

        //private async System.Threading.Tasks.Task<bool> TryOpenComportAsync()
        //{
        //	Boolean openSuccess = false;
        //	var aqsFilter = SerialDevice.GetDeviceSelector(Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.CommPort.Comport);
        //	var devices = await DeviceInformation.FindAllAsync(aqsFilter);
        //	if (devices.Count > 0)
        //	{
        //		DeviceInformation deviceInfo = devices[0];
        //		if (deviceInfo != null)
        //		{
        //			// Create an EventHandlerForDevice to watch for the device we are connecting to
        //			EventHandlerForDevice.CreateNewEventHandlerForDevice();

        //			// Get notified when the device was successfully connected to or about to be closed
        //			EventHandlerForDevice.Instance.OnDeviceConnected = this.OnDeviceConnected;
        //			EventHandlerForDevice.Instance.OnDeviceClose = this.OnDeviceClosing;

        //			openSuccess = await EventHandlerForDevice.Instance.OpenDeviceAsync(deviceInfo, aqsFilter);
        //			//SerialDevice device = await SerialDevice.FromIdAsync(deviceInfo.Id);
        //			//if (openSuccess)
        //			//{
        //			//	//device.Dispose();
        //			//	EventHandlerForDevice.Current.CloseDevice();
        //			//	openSuccess = true;
        //			//}
        //		}
        //	}
        //	return openSuccess;
        //}

        private async Task<bool> SendMessageViaEMailAsync(PacketMessage packetMessage)
        {
            EmailMessage emailMessage = new EmailMessage();
            // Create the to field.
            var messageTo = packetMessage.MessageTo.Split(new char[] { ' ', ';' });
            foreach (string address in messageTo)
            {
                var index = address.IndexOf('@');
                if (index > 0)
                {
                    index = address.ToLower().IndexOf("ampr.org");
                    if (index < 0)
                    {
                        emailMessage.To.Add(new EmailRecipient(address + ".ampr.org"));
                    }
                    else
                    {
                        emailMessage.To.Add(new EmailRecipient(address));
                    }
                }
                else
                {
                    string to = $"{packetMessage.MessageTo}@{packetMessage.BBSName}.ampr.org";
                    emailMessage.To.Add(new EmailRecipient(to));
                }
            }
            SmtpClient smtpClient = SmtpClient.Instance;
            if (smtpClient.Server == "smtp-mail.outlook.com")
            {
                if (!smtpClient.UserName.EndsWith("outlook.com") && !smtpClient.UserName.EndsWith("hotmail.com") && !smtpClient.UserName.EndsWith("live.com"))
                    throw new Exception("Mail from user must be a valid outlook.com address.");
            }
            else if (smtpClient.Server == "smtp.gmail.com")
            {
                if (!smtpClient.UserName.EndsWith("gmail.com"))
                    throw new Exception("Mail from user must be a valid gmail address.");
            }
            else if (string.IsNullOrEmpty(smtpClient.Server))
            {
                throw new Exception("Mail Server must be defined");
            }

            SmtpMessage message = new SmtpMessage(smtpClient.UserName, packetMessage.MessageTo, null, packetMessage.Subject, packetMessage.MessageBody);

            // adding an other To receiver
            //message.To.Add("Eleanore.Doe@somewhere.com");
            bool sendMailSuccess = false;
            try
            {
                sendMailSuccess = await smtpClient.SendMail(message);
                if (!sendMailSuccess)
                {
                    _logHelper.Log(LogLevel.Error, $"Failed to send email message to {packetMessage.MessageTo}. Message No: {packetMessage.MessageNumber}");
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, e.Message);
            }
            return sendMailSuccess;
        }

        //private CoreDispatcher _dispatcher;
        //public void BBSConnectAsync2(CoreDispatcher dispatcher)
        //{
        //    _dispatcher = dispatcher;
        //    BBSConnectAsync2();
        //}

        public async void BBSConnectAsync2()
        {
            (string bbsName, string tncName, string MessageFrom) = Utilities.GetProfileDataBBSStatusChecked();
            //BBSData bbs = Singleton<PacketSettingsViewModel>.Instance.BBSFromSelectedProfile;
            BBSData bbs = BBSDefinitions.Instance.BBSDataArray.Where(bBS => bBS.Name == bbsName).FirstOrDefault();
            //TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == tncName).FirstOrDefault();
            TNCDevice tncDevice = Singleton<PacketSettingsViewModel>.Instance.TNCFromSelectedProfile;

            //if (tncName.Contains(SharedData.EMail) && tncDevice is null)
            //{
            //    tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name.Contains(SharedData.EMail)).FirstOrDefault();
            //}

            _logHelper.Log(LogLevel.Info, "Start a new send/receive session");
            // Collect messages to be sent
            _packetMessagesToSend.Clear();
            List<string> fileTypeFilter = new List<string>() { ".xml" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the Outbox folder
            StorageFileQueryResult results = SharedData.UnsentMessagesFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> unsentFiles = await results.GetFilesAsync();

            foreach (StorageFile file in unsentFiles)
            {
                // Add Outpost message format by Filling the MessageBody field in packetMessage. 
                PacketMessage packetMessage = PacketMessage.Open(file.Path);
                if (packetMessage is null)
                {
                    _logHelper.Log(LogLevel.Error, $"Error opening message file {file.Path}");
                    continue;
                }

                // messages that are opened for editing will not be sent until editing is finished
                if (packetMessage.MessageState == MessageState.Edit)
                    continue;

                // Moved to send button processing
                //DateTime now = DateTime.Now;

                //var operatorDateField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
                //if (operatorDateField != null)
                //{
                //    operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{(now.Year):d4}";
                //}
                //var operatorTimeField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
                //if (operatorTimeField != null)
                //    operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

                FormControlBase formControl = FormsViewModel.CreateFormControlInstance(packetMessage.PacFormName, packetMessage.MessageState);
                if (formControl is null)
                {
                    _logHelper.Log(LogLevel.Error, $"Could not create an instance of {packetMessage.PacFormName}");
                    await ContentDialogs.ShowSingleButtonContentDialogAsync($"Form {packetMessage.PacFormName} not found");
                    continue;
                }
                packetMessage.MessageBody = formControl.CreateOutpostData(ref packetMessage);
                packetMessage.UpdateMessageSize();
                // Save updated message
                packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

                _packetMessagesToSend.Add(packetMessage);
            }
            _logHelper.Log(LogLevel.Info, $"Send messages count: {_packetMessagesToSend.Count}");

            if(tncDevice.Name.Contains(PublicData.EMail) && _packetMessagesToSend.Count == 0)
            {
                return;
            }
            List<PacketMessage> messagesSentAsEMail = new List<PacketMessage>();
            // Send email messages
            foreach (PacketMessage packetMessage in _packetMessagesToSend)
            {
                //tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == packetMessage.TNCName).FirstOrDefault();
                //bbs = BBSDefinitions.Instance.BBSDataList.Where(bBS => bBS.Name == packetMessage.BBSName).FirstOrDefault();

                //TNCInterface tncInterface = new TNCInterface(bbs?.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
                // Send as email if a TNC is not reachable, or if message is defined as an e-mail message
                if (tncDevice.Name.Contains(PublicData.EMail))
                {
                    try
                    {
                        // Mark message as sent by email
                        packetMessage.TNCName = tncDevice.Name;
                        //if (!tncDevice.Name.Contains(SharedData.EMail))
                        //{
                        //    packetMessage.TNCName = "E-Mail-" + Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.MailUserName;
                        //}

                        bool sendMailSuccess = await SendMessageViaEMailAsync(packetMessage);

                        if (sendMailSuccess)
                        {
                            packetMessage.MessageState = MessageState.Locked;
                            packetMessage.SentTime = DateTime.Now;
                            packetMessage.MailUserName = SmtpClient.Instance.UserName;
                            _logHelper.Log(LogLevel.Info, $"Message sent via E-Mail: {packetMessage.MessageNumber}");

                            var file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                            await file?.DeleteAsync();

                            // Do a save to ensure that updates are saved
                            packetMessage.Save(SharedData.SentMessagesFolder.Path);

                            //_packetMessagesToSend.Remove(packetMessage);
                            messagesSentAsEMail.Add(packetMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logHelper.Log(LogLevel.Error, $"Error sending e-mail message {packetMessage.MessageNumber}");
                        string text = ex.Message;
                        continue;
                    }
                }
            }

            // Remove already processed E-Mail messages. 
            foreach (PacketMessage packetMessage in messagesSentAsEMail)
            {
                _packetMessagesToSend.Remove(packetMessage);
            }

            // TODO check if TNC connected otherwise suggest send via email
            //if (_packetMessagesToSend.Count == 0)
            //{
            //    tncDevice = Singleton<PacketSettingsViewModel>.Instance.CurrentTNC;

            //    (string bbsName, string tncName, string MessageFrom) = Utilities.GetProfileData();
            //    //string MessageFrom = from;
            //    BBSData MessageBBS = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
            //    if (MessageBBS == null || !MessageBBS.Name.Contains("XSC") && !tncDevice.Name.Contains(SharedData.EMail))
            //    {
            //        //string bbsName = AddressBook.Instance.GetBBS(MessageFrom);
            //        bbs = BBSDefinitions.Instance.GetBBSFromName(bbsName);
            //    }
            //    else
            //    {
            //        bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
            //    }
            //    tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == tncName).FirstOrDefault();
            //}
            //else
            //{
            //    //tncDevice = Singleton<PacketSettingsViewModel>.Instance.CurrentTNC;
            //    tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == _packetMessagesToSend[0].TNCName).FirstOrDefault();
            //    bbs = BBSDefinitions.Instance.BBSDataList.Where(bBS => bBS.Name == _packetMessagesToSend[0].BBSName).FirstOrDefault();
            //    //Utilities.SetApplicationTitle(bbs.Name);
            //    //bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
            //}

            //Utilities.SetApplicationTitle(bbs?.Name);

            if (!tncDevice.Name.Contains(PublicData.EMail))
            {
                ViewLifetimeControl viewLifetimeControl = await WindowManagerService.Current.TryShowAsStandaloneAsync("Connection Status", typeof(RxTxStatusPage));
                //RxTxStatusPage.rxtxStatusPage._viewLifetimeControl.Height = RxTxStatusPage.rxtxStatusPage.RxTxStatusViewmodel.ViewControlHeight;
                //RxTxStatusPage.rxtxStatusPage._viewLifetimeControl.Width = RxTxStatusPage.rxtxStatusPage.RxTxStatusViewmodel.ViewControlWidth;

                //bool success = RxTxStatusPage.rxtxStatusPage._viewLifetimeControl.ResizeView();


                //return;     //Test

                PacketSettingsViewModel packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;

                _tncInterface = new TNCInterface(bbs?.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);

                // Collect remaining messages to be sent
                // Process files to be sent via BBS
                await _tncInterface.BBSConnectThreadProcAsync();

                // Close status window
                await RxTxStatusPage.Current._viewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //RxTxStatusPage.rxtxStatusPage.CloseStatusWindowAsync();
                    RxTxStatusPage.Current.RxTxStatusViewmodel.CloseStatusWindowAsync();
                });

                Singleton<PacketSettingsViewModel>.Instance.ForceReadBulletins = false;
                if (!string.IsNullOrEmpty(bbs?.Name))
                {
                    _logHelper.Log(LogLevel.Info, $"Disconnected from: {bbs?.ConnectName}. Connect time = {_tncInterface.BBSDisconnectTime - _tncInterface.BBSConnectTime}");
                }

                // Move sent messages from unsent folder to the Sent folder
                foreach (PacketMessage packetMsg in _tncInterface.PacketMessagesSent)
                {
                    try
                    {
                        _logHelper.Log(LogLevel.Info, $"Message number {packetMsg.MessageNumber} Sent");

                        StorageFile file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMsg.FileName, CreationCollisionOption.OpenIfExists);
                        await file.DeleteAsync();

                        // Do a save to ensure that updates from tncInterface.BBSConnect are saved
                        packetMsg.Save(SharedData.SentMessagesFolder.Path);
                    }
                    catch (Exception e)
                    {
                        _logHelper.Log(LogLevel.Error, $"Exception {e.Message}");
                        continue;
                    }
                    if (string.IsNullOrEmpty(packetMsg.Area) && Singleton<SettingsViewModel>.Instance.PrintSentMessages)
                    {
                        // Do printing if requested
                        _logHelper.Log(LogLevel.Info, $"Message number {packetMsg.MessageNumber} to be printed");

                        packetMsg.Save(SharedData.PrintMessagesFolder.Path);

                        SettingsViewModel settingsViewModel = Singleton<SettingsViewModel>.Instance;
                        Singleton<PrintQueue>.Instance.AddToPrintQueue(packetMsg.FileName, settingsViewModel.SentCopyNamesAsArray());
                    }

                }
                _packetMessagesReceived = _tncInterface.PacketMessagesReceived;
                ProcessReceivedMessagesAsync();

                /*
                ApplicationTrigger trigger = new ApplicationTrigger();
                var task = RxTxBackgroundTask.RegisterBackgroundTask(RxTxBackgroundTask.RxTxBackgroundTaskEntryPoint,
                                                                       RxTxBackgroundTask.ApplicationTriggerTaskName,
                                                                       trigger,
                                                                       null);
                task.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
                task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);


                // Register a ApplicationTriggerTask.
                RxTxBackgroundTask rxTxBackgroundTask = new RxTxBackgroundTask(bbs?.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
                            rxTxBackgroundTask.Register();
                // Start backgroung task
                // Reset the completion status
                var settings = ApplicationData.Current.LocalSettings;
                settings.Values.Remove(BackgroundTaskSample.ApplicationTriggerTaskName);

                //Signal the ApplicationTrigger
                var result = await trigger.RequestAsync();

                ApplicationTriggerResult result = await rxTxBackgroundTask._applicationTrigger.RequestAsync();
                //            await Singleton<BackgroundTaskService>.Instance.HandleAsync(RxTxBackgroundTask);
                // RxTxBackgroundTask is finished

                if (_connectState == ConnectState.ConnectStateBBSConnect)
                {
                    await Utilities.ShowSingleButtonContentDialogAsync(_result, "Close", "BBS Connect Error");
                    //_result = "It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach";
                }
                            else if (_connectState == ConnectState.ConnectStatePrepareTNCType)
                            {
                                await Utilities.ShowSingleButtonContentDialogAsync("Unable to connect to the TNC.\nIs the TNC on?\nFor Kenwood; is the radio in \"packet12\" mode?", "Close", "BBS Connect Error");
                                //_result = "";
                            }
                            else if (_connectState == ConnectState.ConnectStateConverseMode)
                            {
                                await Utilities.ShowSingleButtonContentDialogAsync($"Error sending FCC Identification - {Singleton<IdentityViewModel>.Instance.UserCallsign}.", "Close", "TNC Converse Error");
                                //_result = $"Error sending FCC Identification - { Singleton<IdentityViewModel>.Instance.UserCallsign}.";
                            }
                            //else if (e.Message.Contains("not exist"))
                            else if (e.GetType() == typeof(IOException))
                            {
                                await Utilities.ShowSingleButtonContentDialogAsync("Looks like the USB or serial cable to the TNC is disconnected", "Close", "TNC Connect Error");
                                //_result = "Looks like the USB or serial cable to the TNC is disconnected";
                            }
                            else if (e.GetType() == typeof(UnauthorizedAccessException))
                            {
                                await Utilities.ShowSingleButtonContentDialogAsync($"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application. ", "Close", "TNC Connect Error");
                                //_result = $"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application.";
                            }

                            Singleton<PacketSettingsViewModel>.Instance.ForceReadBulletins = false;
                            if (!string.IsNullOrEmpty(bbs?.Name))
                            {
                                _logHelper.Log(LogLevel.Info, $"Disconnected from: {bbs?.ConnectName}. Connect time = {rxTxBackgroundTask.BBSDisconnectTime - rxTxBackgroundTask.BBSConnectTime}");
                            }

                            // Move sent messages from unsent folder to the Sent folder
                            foreach (PacketMessage packetMsg in rxTxBackgroundTask.PacketMessagesSent)
                            {
                                try
                                {
                                    _logHelper.Log(LogLevel.Info, $"Message number {packetMsg.MessageNumber} Sent");

                                    StorageFile file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMsg.FileName, CreationCollisionOption.OpenIfExists);
                                    await file.DeleteAsync();

                                    // Do a save to ensure that updates from tncInterface.BBSConnect are saved
                                    packetMsg.Save(SharedData.SentMessagesFolder.Path);
                                }
                                catch (Exception e)
                                {
                                    _logHelper.Log(LogLevel.Error, $"Exception {e.Message}");
                                }
                            }
                            _packetMessagesReceived = rxTxBackgroundTask.PacketMessagesReceived;
                            ProcessReceivedMessagesAsync();
                */            //_deviceFound = true;
                              //try
                              //{
                              //    _serialPort = new SerialPort(Singleton<TNCSettingsViewModel>.Instance.CurrentTNCDevice.CommPort.Comport);
                              //}
                              //catch (IOException e)
                              //{
                              //    _deviceFound = false;
                              //}
                              //_serialPort.Close();
            }
        }


        /// <summary>
        /// If all the devices have been enumerated, select the device in the list we connected to. Otherwise let the EnumerationComplete event
        /// from the device watcher handle the device selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformation"></param>
        //private void OnDeviceConnected(EventHandlerForDevice sender, DeviceInformation deviceInformation)
        //{
        //          _logHelper.Log(LogLevel.Info, $"{Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.CommPort.Comport} Connected.");
        //}

        ///// <summary>
        ///// The device was closed. If we will autoreconnect to the device, reflect that in the UI
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="deviceInformation"></param>
        //private void OnDeviceClosing(EventHandlerForDevice sender, DeviceInformation deviceInformation)
        //{
        //          _logHelper.Log(LogLevel.Info, $"{Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.CommPort.Comport} Disconnected.");
        //}

    }
}
