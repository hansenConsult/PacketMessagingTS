using MetroLog;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Views;
using PacketMessagingTS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Email;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Popups;
using PacketMessagingTS.Services.SMTPClient;
using Windows.Devices.SerialCommunication;
using MessageFormControl;
using PacketMessagingTS.ViewModels;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

using FormControlBaseClass;
using SharedCode;


namespace PacketMessagingTS.Services.CommunicationsService
{
	public class CommunicationsService
	{
		protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CommunicationsService>();
        private LogHelper _logHelper = new LogHelper(log);


        //Collection<DeviceListEntry> _listOfDevices;

        public List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
		List<PacketMessage> _packetMessagesToSend = new List<PacketMessage>();

        private static readonly Object singletonCreationLock = new Object();
        static volatile CommunicationsService _communicationsService = null;
		static bool _deviceFound = false;
        public StreamSocket _socket = null;
        public SerialPort _serialPort;

        private CommunicationsService()
        {
        }

        public static CommunicationsService CreateInstance()
		{
            if (_communicationsService is null)
            {
                lock (singletonCreationLock)
                {
                    if (_communicationsService is null)
                    {
                        _communicationsService = new CommunicationsService();
                    }
                }
            }
			return _communicationsService;
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
					string startpos = new string(new char[] { 'D','a','t','e',':',' ' });
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
				else if (msgLines[i].StartsWith("# FORMFILENAME:"))
				{
					string html = ".html";
					string formName = msgLines[i].Substring(16).TrimEnd(new char[] { ' ' });
					formName = formName.Substring(0, formName.Length - html.Length);
                    pktMsg.PacFormName = formName;

                    formControl = FormsPage.CreateFormControlInstance(pktMsg.PacFormName);
					if (formControl is null)
					{
                        _logHelper.Log(LogLevel.Error, $"Form {pktMsg.PacFormName} not found");
						await Utilities.ShowMessageDialogAsync($"Form {pktMsg.PacFormName} not found");
						return ;
					}
					break;
				}
			}
			pktMsg.PacFormName = formControl.PacFormName;
			pktMsg.FormFieldArray = formControl.ConvertFromOutpost(pktMsg.MessageNumber, ref msgLines);
			//pktMsg.ReceivedTime = packetMessage.ReceivedTime;
			pktMsg.CreateFileName();
			string fileFolder = SharedData.ReceivedMessagesFolder.Path;
			pktMsg.Save(fileFolder);

			_logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} converted and saved");

			return ;
		}

		public async void ProcessReceivedMessagesAsync()
		{
			if (_packetMessagesReceived.Count() > 0)
			{
				foreach (PacketMessage packetMessageOutpost in _packetMessagesReceived)
				{
					FormControlBase formControl = new MessageControl();
                    // test for packet form!!
                    PacketMessage pktMsg = new PacketMessage()
                    {
                        BBSName = packetMessageOutpost.BBSName,
                        TNCName = packetMessageOutpost.TNCName,
                        MessageSize = packetMessageOutpost.MessageSize,
                        Area = packetMessageOutpost.Area,
                        // Save the original message for post processing (tab characters are lost in the displayed message)
                        MessageBody = packetMessageOutpost.MessageBody,
                        //MessageReadOnly = true
                        MessageOpened = false,
                    };
                    string[] msgLines = packetMessageOutpost.MessageBody.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.None);

					bool subjectFound = false;
					for (int i = 0; i < Math.Min(msgLines.Length, 20); i++)
					{
						if (msgLines[i].StartsWith("Date:"))
						{
							pktMsg.JNOSDate = DateTime.Parse(msgLines[i].Substring(10, 21));
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

                            formControl = FormsPage.CreateFormControlInstance(pktMsg.PacFormName);
							if (formControl is null)
							{
                                _logHelper.Log(LogLevel.Error, $"Form {pktMsg.PacFormName} not found");
                                await Utilities.ShowMessageDialogAsync($"Form {pktMsg.PacFormName} not found");
								return;
							}
							break;
						}
					}

                    //pktMsg.MessageNumber = GetMessageNumberPacket();		// Filled in BBS connection
                    pktMsg.PacFormType = formControl.PacFormType;
                    pktMsg.PacFormName = formControl.PacFormName;
                    pktMsg.MessageNumber = packetMessageOutpost.MessageNumber;
					pktMsg.FormFieldArray = formControl.ConvertFromOutpost(pktMsg.MessageNumber, ref msgLines);
					pktMsg.ReceivedTime = packetMessageOutpost.ReceivedTime;
					if (pktMsg.ReceivedTime != null)
					{
						DateTime dateTime = (DateTime)pktMsg.ReceivedTime;
					}
					//if (pktMsg.ReceivedTime != null)   ???? duplicate
					//{
					//	DateTime dateTime = (DateTime)pktMsg.ReceivedTime;
					//}
					pktMsg.CreateFileName();
					pktMsg.Save(SharedData.ReceivedMessagesFolder.Path);

                    _logHelper.Log(LogLevel.Info, $"Message number {pktMsg.MessageNumber} received");

                    // If the received message is a delivery confirmation, update receivers message number in the original sent message
                    if (!string.IsNullOrEmpty(pktMsg.Subject) && pktMsg.Subject.Contains("DELIVERED:"))
					{
						var formField = pktMsg.FormFieldArray.FirstOrDefault(x => x.ControlName == "messageBody");
						if (formField.ControlContent.Contains("!LMI!"))
						{
							string[] searchStrings = new string[] { "Subject: ", "was delivered on ", "Recipient's Local Message ID: " };
							DateTime receiveTime = DateTime.Now;
							string receiversMessageId = "", sendersMessageId = "";
							var messageLines = formField.ControlContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


							foreach (string line in messageLines)
							{
								if (line.Contains(searchStrings[0]))
								{
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

                            List<string> fileTypeFilter = new List<string>() { ".xml" };
                            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

                            // Get the files in the user's archive folder
                            StorageFileQueryResult results = SharedData.SentMessagesFolder.CreateFileQueryWithOptions(queryOptions);
                            // Iterate over the results
                            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
                            foreach (StorageFile file in files)
                            {
                                // Update sent form with receivers message number and receive time
                                if (file.Name.Contains(sendersMessageId))
								{
									PacketMessage packetMessage = PacketMessage.Open(file);
									if (packetMessage.MessageNumber == sendersMessageId)
									{
										formField = packetMessage.FormFieldArray.FirstOrDefault(x => x.ControlName == "receiverMsgNo");
										if (formField != null)
										{
											formField.ControlContent = receiversMessageId;
										}
										packetMessage.ReceiverMessageNumber = receiversMessageId;
										packetMessage.ReceivedTime = receiveTime;
										//if (receiveTime != null)
										//{
										//	packetMessage.ReceivedTimeDisplay = $"{receiveTime.Month:d2}/{receiveTime.Date:d2}/{receiveTime.Year - 2000:d2} {receiveTime.Hour:d2}:{receiveTime.Minute:d2}";
										//}
										packetMessage.Save(SharedData.SentMessagesFolder.Path);
										break;
									}
								}
							}
						}
					}
				}
				//RefreshDataGrid();      // Display newly added messages
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

        public async void BBSConnectAsync()
        {
            FormControlBase formControl;

            //_listOfDevices = listOfDevices;

            //SharedData sharedData =  ViewModels.SharedData.SharedDataInstance;

            TNCDevice tncDevice = Singleton<PacketSettingsViewModel>.Instance.CurrentTNC;
            if (tncDevice != null)
            {
                //_deviceFound = await TryOpenComportAsync();
                _deviceFound = true;
                try
                {
                    _serialPort = new SerialPort(Singleton<TNCSettingsViewModel>.Instance.CurrentTNCDevice.CommPort.Comport);
                }
                catch 
                {
                    _deviceFound = false;
                }
                _serialPort.Close();

                TNCInterface tncInterface = null;
                PacketSettingsViewModel packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;

                BBSData bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
                if (bbs != null && bbs.Name.Length > 0)
                {
                    // Do we use a bluetooth device?
                    if ((bool)Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.CommPort?.IsBluetooth)
                    {
                        RfcommDeviceService service = null;
                        try
                        {
                            DeviceInformationCollection DeviceInfoCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
                            foreach (var deviceInfo in DeviceInfoCollection)
                            {
                                if (deviceInfo.Id.Contains(Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.CommPort.DeviceId))
                                {
                                    service = await RfcommDeviceService.FromIdAsync(deviceInfo.Id);
                                    _deviceFound = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            _logHelper.Log(LogLevel.Error, $"Error finding bluetooth device. { exp.Message }");
                        }
                        if (!_deviceFound)
                        {
                            _logHelper.Log(LogLevel.Warn, $"TNC not found. Sending the message via e-mail");
                            await Utilities.ShowMessageDialogAsync($"TNC not found. Sending the message via e-mail");
                        }
                        else
                        {
                            if (_socket != null)
                            {
                                // Disposing the socket with close it and release all resources associated with the socket
                                _socket.Dispose();
                            }

                            _socket = new StreamSocket();
                            bool success = true;
                            try
                            {
                                // Note: If either parameter is null or empty, the call will throw an exception
                                await _socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName);
                            }
                            catch (Exception ex)
                            {
                                success = false;
                                _logHelper.Log(LogLevel.Error, "Bluetootn Connect failed:" + ex.Message);
                            }
                            // If the connection was successful, the RemoteAddress field will be populated
                            try
                            {
                                if (success)
                                {
                                    //System.Diagnostics.Debug.WriteLine(msg);
                                    //await md.ShowAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                _logHelper.Log(LogLevel.Error, $"Overall Connect: { ex.Message}");
                                _socket.Dispose();
                                _socket = null;
                            }
                            tncInterface = new TNCInterface(bbs.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
                        }
                    }
                    // Collect messages to be sent
                    _packetMessagesToSend.Clear();
                    List<string> fileTypeFilter = new List<string>() { ".xml" };
                    QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

                    // Get the files in the user's Outbox folder
                    StorageFileQueryResult results = SharedData.UnsentMessagesFolder.CreateFileQueryWithOptions(queryOptions);
                    // Iterate over the results
                    IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
                    foreach (StorageFile file in files)
                    {
                        // Add Outpost message format by Filling the MessageBody field in packetMessage. 
                        PacketMessage packetMessage = PacketMessage.Open(file);
                        if (packetMessage is null)
                        {
                            continue;
                        }

                        DateTime now = DateTime.Now;

                        var operatorDateField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
                        if (operatorDateField != null)
                        {
                            operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                        }
                        var operatorTimeField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
                        if (operatorTimeField != null)
                            operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

                        formControl = FormsPage.CreateFormControlInstance(packetMessage.PacFormName);
                        if (formControl is null)
                        {
                            _logHelper.Log(LogLevel.Error, $"Form {packetMessage.PacFormName} not found");
                            MessageDialog messageDialog = new MessageDialog($"Form {packetMessage.PacFormName} not found");
                            await messageDialog.ShowAsync();
                            continue;
                        }

                        packetMessage.MessageBody = formControl.CreateOutpostData(ref packetMessage);
                        packetMessage.MessageSize = packetMessage.Size;
                        // Save updated message
                        packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

                        _packetMessagesToSend.Add(packetMessage);
                    }
                    if (_packetMessagesToSend.Count == 0)
                    {
                        _logHelper.Log(LogLevel.Info, $"No messages to send.");
                    }

                    tncInterface = new TNCInterface(bbs.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
                    // Send as email if a TNC is not reachable, or if message is defined as an e-mail message
                    if (!_deviceFound || tncDevice.Name.Contains(SharedData.EMail))
                    {
                        EmailMessage emailMessage;
                        try
                        {
                            foreach (PacketMessage packetMessage in _packetMessagesToSend)
                            {
                                // Mark message as sent by email
                                packetMessage.TNCName = tncDevice.Name;
                                if (!tncDevice.Name.Contains(SharedData.EMail))
                                {
                                    packetMessage.TNCName = "E-Mail-" + Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.MailUserName;
                                }

                                emailMessage = new EmailMessage();
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
                                SmtpClient smtpClient = Services.SMTPClient.SmtpClient.Instance;
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
                                        _logHelper.Log(LogLevel.Warn, $"Failed to send email message to {packetMessage.MessageTo}. Message No: {packetMessage.MessageNumber}");
                                    }
                                }
                                catch (Exception e)
                                {
                                    _logHelper.Log(LogLevel.Error, e.Message);
                                }

                                //								emailMessage.Subject = packetMessage.Subject;
                                //// Insert \r\n 
                                //string temp = packetMessage.MessageBody;
                                //string messageBody = temp.Replace("\r", "\r\n");
                                //var msgBody = temp.Split(new char[] { '\r' });
                                //messageBody = "";
                                //foreach (string s in msgBody)
                                //{
                                //	messageBody += (s + "\r\n");
                                //}
                                //emailMessage.Body = messageBody;
                                //								emailMessage.Body = packetMessage.MessageBody;

                                //IBuffer 
                                //var memStream = new InMemoryRandomAccessStream();
                                //var randomAccessStreamReference = RandomAccessStreamReference.CreateFromStream(memStream);
                                //emailMessage.SetBodyStream(EmailMessageBodyKind.PlainText, randomAccessStreamReference);
                                //emailMessage.Body = await memStream.WriteAsync(packetMessage.MessageBody);

                                //								await EmailManager.ShowComposeNewEmailAsync(emailMessage);
                                if (sendMailSuccess)
                                {
                                    DateTime dateTime = DateTime.Now;
                                    packetMessage.SentTime = dateTime;
                                    packetMessage.MailUserName = smtpClient.UserName;
                                    tncInterface.PacketMessagesSent.Add(packetMessage);

                                    _logHelper.Log(LogLevel.Info, $"Message sent via email {packetMessage.MessageNumber}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string text = ex.Message;
                            return;
                        }
                    }
                    else
                    {
                        // Send message via TNC and RF

                        //ref List<PacketMessage> packetMessagesReceived, ref List<PacketMessage> packetMessagesToSend, out DateTime bbsConnectTime, out DateTime bbsDisconnectTime
                        //string messageBBS, ref TNCDevice tncDevice, bool forceReadBulletins
                        _logHelper.Log(LogLevel.Info, $"Connect to: {bbs.ConnectName}");

                        //tncInterface.BBSConnect(ref packetMessagesReceived, ref packetMessagesToSend);
                        //ConnectedDialog connectDlg = new ConnectedDialog();
                        //connectDlg.Owner = Window.GetWindow(this);
                        //tncInterface.ConnectDlg = connectDlg;

                        await tncInterface.BBSConnectThreadProcAsync();

                        //EventHandlerForDevice.Instance.CloseDevice();

                        //if (!success)
                        //{
                        //    return;
                        //}

                        //var thread = new Thread(tncInterface.BBSConnectThreadProc);
                        //thread.SetApartmentState(ApartmentState.STA);
                        //connectDlg.ConnectThread = thread;
                        //connectDlg.ConnectError = tncInterface;
                        //thread.Start();

                        //// Blocks UI while connected to BBS
                        //connectDlg.ShowDialog();

                        Singleton<PacketSettingsViewModel>.Instance.ForceReadBulletins = false;
                        _logHelper.Log(LogLevel.Info, $"Disconnected from: {bbs.ConnectName}. Connect time = {tncInterface.BBSDisconnectTime - tncInterface.BBSConnectTime}");
                    }
                    // Move sent messages from unsent folder to the Sent folder
                    foreach (PacketMessage packetMsg in tncInterface.PacketMessagesSent)
                    {
                        _logHelper.Log(LogLevel.Info, $"Message number {packetMsg.MessageNumber} Sent");

                        var file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMsg.FileName, CreationCollisionOption.OpenIfExists);
                        await file.DeleteAsync();

                        // Do a save to ensure that updates from tncInterface.BBSConnect are saved
                        packetMsg.Save(SharedData.SentMessagesFolder.Path);
                    }
                    _packetMessagesReceived = tncInterface.PacketMessagesReceived;
                    ProcessReceivedMessagesAsync();
                }
                else
                {
                    MessageDialog messageDialog = new MessageDialog($"Could not find the requested BBS ({Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS}). Check Packet Settings");
                    await messageDialog.ShowAsync();
                    _logHelper.Log(LogLevel.Error, $"Could not find the requested BBS ({Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS}). Check Packet Settings");
                }
            }
            else
            {
                MessageDialog messageDialog = new MessageDialog($"Could not find the requested TNC ({Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC})");
                await messageDialog.ShowAsync();
                _logHelper.Log(LogLevel.Error, $"Could not find the requested TNC ({Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC})");
            }
        }

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

        public async void BBSConnectAsync2()
        {
            FormControlBase formControl;
            PacketSettingsViewModel packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;
            TNCDevice tncDevice;
            BBSData bbs;

            // Collect messages to be sent
            _packetMessagesToSend.Clear();
            List<string> fileTypeFilter = new List<string>() { ".xml" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the Outbox folder
            StorageFileQueryResult results = SharedData.UnsentMessagesFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                // Add Outpost message format by Filling the MessageBody field in packetMessage. 
                PacketMessage packetMessage = PacketMessage.Open(file);
                if (packetMessage is null)
                {
                    _logHelper.Log(LogLevel.Error, $"Error opening message file {file}");
                    continue;
                }

                DateTime now = DateTime.Now;

                var operatorDateField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
                if (operatorDateField != null)
                {
                    operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{(now.Year):d4}";
                }
                var operatorTimeField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
                if (operatorTimeField != null)
                    operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

                formControl = FormsPage.CreateFormControlInstance(packetMessage.PacFormName);
                if (formControl is null)
                {
                    _logHelper.Log(LogLevel.Error, $"Could not create an instance of {packetMessage.PacFormName}");
                    await Utilities.ShowMessageDialogAsync($"Form {packetMessage.PacFormName} not found");
                    continue;
                }
                packetMessage.MessageBody = formControl.CreateOutpostData(ref packetMessage);
                packetMessage.MessageSize = packetMessage.Size;
                // Save updated message
                packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

                _packetMessagesToSend.Add(packetMessage);
            }
            _logHelper.Log(LogLevel.Info, $"Send messages count: {_packetMessagesToSend.Count}");

            List<PacketMessage> messagesSentAsEMail = new List<PacketMessage>();
            //
            foreach (PacketMessage packetMessage in _packetMessagesToSend)
            {
                tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == packetMessage.TNCName).FirstOrDefault();
                bbs = BBSDefinitions.Instance.BBSDataList.Where(bBS => bBS.Name == packetMessage.BBSName).FirstOrDefault();

                //TNCInterface tncInterface = new TNCInterface(bbs?.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
                // Send as email if a TNC is not reachable, or if message is defined as an e-mail message
                if (tncDevice.Name.Contains(SharedData.EMail))
                {
                    try
                    {
                        // Mark message as sent by email
                        //packetMessage.TNCName = tncDevice.Name;
                        if (!tncDevice.Name.Contains(SharedData.EMail))
                        {
                            packetMessage.TNCName = "E-Mail-" + Singleton<PacketSettingsViewModel>.Instance.CurrentTNC.MailUserName;
                        }

                        bool sendMailSuccess = await SendMessageViaEMailAsync(packetMessage);

                        if (sendMailSuccess)
                        {
                            packetMessage.SentTime = DateTime.Now;
                            packetMessage.MailUserName = SmtpClient.Instance.UserName;
                            _logHelper.Log(LogLevel.Info, $"Message sent via E-Mail: {packetMessage.MessageNumber}");

                            var file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                            await file?.DeleteAsync();

                            // Do a save to ensure that updates are saved
                            packetMessage.Save(SharedData.SentMessagesFolder.Path);

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

            // Remove already processed E-Mail messages
            foreach (PacketMessage packetMessage in messagesSentAsEMail)
            {
                _packetMessagesToSend.Remove(packetMessage);
            }

            // TODO check if TNC connected otherwise suggest send via email
            if (_packetMessagesToSend.Count == 0)
            {
                tncDevice = Singleton<PacketSettingsViewModel>.Instance.CurrentTNC;
                //if (tncDevice.Name.Contains(SharedData.EMail))
                //{
                //    await Utilities.ShowMessageDialogAsync("Use TNC to send message. Not E-Email");
                //    return;
                //}
                bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
            }
            else
            {
                //tncDevice = Singleton<PacketSettingsViewModel>.Instance.CurrentTNC;
                tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == _packetMessagesToSend[0].TNCName).FirstOrDefault();
                bbs = BBSDefinitions.Instance.BBSDataList.Where(bBS => bBS.Name == _packetMessagesToSend[0].BBSName).FirstOrDefault();
                //bbs = Singleton<PacketSettingsViewModel>.Instance.CurrentBBS;
            }
            TNCInterface tncInterface = new TNCInterface(bbs?.ConnectName, ref tncDevice, packetSettingsViewModel.ForceReadBulletins, packetSettingsViewModel.AreaString, ref _packetMessagesToSend);
            // Collect remaining messages to be sent
            // Process files to be sent via BBS
            await tncInterface.BBSConnectThreadProcAsync();

            Singleton<PacketSettingsViewModel>.Instance.ForceReadBulletins = false;
            if (!string.IsNullOrEmpty(bbs?.Name))
            {
                _logHelper.Log(LogLevel.Info, $"Disconnected from: {bbs?.ConnectName}. Connect time = {tncInterface.BBSDisconnectTime - tncInterface.BBSConnectTime}");
            }

            // Move sent messages from unsent folder to the Sent folder
            foreach (PacketMessage packetMsg in tncInterface.PacketMessagesSent)
            {
                _logHelper.Log(LogLevel.Info, $"Message number {packetMsg.MessageNumber} Sent");

                var file = await SharedData.UnsentMessagesFolder.CreateFileAsync(packetMsg.FileName, CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync();

                // Do a save to ensure that updates from tncInterface.BBSConnect are saved
                packetMsg.Save(SharedData.SentMessagesFolder.Path);
            }
            _packetMessagesReceived = tncInterface.PacketMessagesReceived;
            ProcessReceivedMessagesAsync();
            //_deviceFound = true;
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
