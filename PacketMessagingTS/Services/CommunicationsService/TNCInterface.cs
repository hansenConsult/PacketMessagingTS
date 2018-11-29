using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using MessageFormControl;
using MetroLog;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Helpers;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Core;

using SharedCode;
using System.Text;

namespace PacketMessagingTS.Services.CommunicationsService
{
	public class TNCInterface: IDisposable
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCInterface>();
        private static LogHelper _logHelper = new LogHelper(log);

        enum ConnectState
        {
            ConnectStateNone,
            ConnectStatePrepareTNCType,
            ConnectStatePrepare,
			ConnectStateBBSTryConnect,
			ConnectStateBBSConnect,
            ConnectStatePost,
            ConnectStateDisconnected,
			ConnectStateConverseMode
        }
        ConnectState _connectState;

        List<PacketMessage> _packetMessagesSent = new List<PacketMessage>();
        List<PacketMessage> _packetMessagesToSend;

        string _bbsConnectName = "";
        bool _forceReadBulletins = false;
        string _Areas;
        TNCDevice _TncDevice = null;
        SerialPort _serialPort = null;

		const string _BBSPrompt = ") >";
		string _TNCPrompt = "cmd:";
        private bool _error = false;		// Disconnect if an error is detected

		//const byte send = 0x5;
		public TNCInterface()
        {

        }

        public TNCInterface(string messageBBS, ref TNCDevice tncDevice, bool forceReadBulletins, string areas, ref List<PacketMessage> packetMessagesToSend)
        {
            _bbsConnectName = messageBBS;
            _TncDevice = tncDevice;
			_TNCPrompt = _TncDevice.Prompts.Command;
            _forceReadBulletins = forceReadBulletins;
            _Areas = areas;
            _packetMessagesToSend = packetMessagesToSend;
        }

        public List<PacketMessage> PacketMessagesSent => _packetMessagesSent;

        public DateTime BBSConnectTime
        { get; set; }

		public DateTime BBSDisconnectTime
		{ get; set; }

		//public ConnectedDialog ConnectDlg
  //      { get; set; }

		public bool Cancel
		{ get { return _error; } set { _error = value; } }

        private List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
        public List<PacketMessage> PacketMessagesReceived { get => _packetMessagesReceived; set => _packetMessagesReceived = value; }

        private string KPC3Plus()
        {
            string readText = _serialPort.ReadLine();       // This appears to be a dummy read for KPC3
            log.Info(readText);

            readText = _serialPort.ReadLine();
            Debug.WriteLine(readText);
            log.Info(readText);

            readText = _serialPort.ReadLine();
            Debug.WriteLine(readText);
            log.Info(readText);

            readText = _serialPort.ReadLine();
            Debug.WriteLine(readText);
            log.Info(readText);

            string readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);   // First cmd:

            _serialPort.Write("D\r");
            //Thread.Sleep(100);

            readText = _serialPort.ReadLine();
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            Debug.WriteLine(readText);
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);

            _serialPort.Write("b\r");

            readText = _serialPort.ReadLine();       // Command
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for b
            Debug.WriteLine(readText);
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);

            _serialPort.Write("Echo on\r");

            readText = _serialPort.ReadLine();       // Read command
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for Echo on
            Debug.WriteLine(readText);
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);

            // TODO add tactical callsign
            _serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.UserCallsign + "\r");

            readText = _serialPort.ReadLine();       // Read command
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);
            // Note no command response

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);

            _serialPort.Write("Mon off\r");

            readText = _serialPort.ReadLine();       // Read command
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for Mon off
            Debug.WriteLine(readText);
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            Debug.WriteLine(readCmdText + _TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write("daytime " + dayTime + "\r");

            readText = _serialPort.ReadLine();       // Read command
            Debug.WriteLine(readText);
            log.Info(readCmdText + _TNCPrompt + readText);
            // Note no command response

            readCmdText = _serialPort.ReadTo(_TNCPrompt);      // Ready for pre commands
            Debug.WriteLine(readCmdText + _TNCPrompt);
			return readCmdText;
		}

		private string Kenwood()
        {
            _serialPort.Write("\r");

            string readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("D\r");

            string readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("b\r");

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("Echo on\r");

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                _serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.TacticalCallsign + "\r");
            }
            else
            {
                _serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.UserCallsign + "\r");
            }

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("Mon off\r");

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

            readText = _serialPort.ReadLine();       // Result for Mon off
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write("daytime " + dayTime + "\r");

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);
            // Note no command response

            readCmdText = _serialPort.ReadTo(_TNCPrompt);	// Ready for pre commands
			return readCmdText;
		}

		private void SendMessage(ref PacketMessage packetMessage)
		{
			_serialPort.ReadTimeout = 240000;
			try
			{
				_serialPort.Write("SP " + packetMessage.MessageTo + "\r");
				_serialPort.Write(packetMessage.Subject + "\r");
				_serialPort.Write(packetMessage.MessageBody + "\r\x1a\r\x05");

				//string readText = _serialPort.ReadLine();       // Read SP
                //_logHelper.Log(LogLevel.Info, readText);

				string readText = _serialPort.ReadTo(_BBSPrompt);      // read response
                _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

				//readText = _serialPort.ReadTo("\n");         // Next command
                //_logHelper.Log(LogLevel.Info, readText + "\n");

				packetMessage.SentTime = DateTime.Now;
				_packetMessagesSent.Add(packetMessage);
			}
			catch (Exception e)
			{
                _logHelper.Log(LogLevel.Error, $"Send message exception: {e.Message}");
				_serialPort.DiscardInBuffer();
				_serialPort.DiscardOutBuffer();
				_error = true;
			}
			_serialPort.ReadTimeout = 5000;
		}
        /*
        SP kz6dm@w3xsc.ampr.org
        DELIVERED: 5DM-002_O/R_OAAlliedHealth_Facility Type_Facility Name
        !LMI!6DM-526P!DR!9/14/2018 6:51:22 PM
        Your Message
        To: KZ6DM
        Subject: 5DM-002_O/R_OAAlliedHealth_Facility Type_Facility Name
        was delivered on 9/14/2018 6:51:22 PM
        Recipient's Local Message ID: 6DM-526P
        /EX
        Subject:
        Enter message.  End with /EX or ^Z in first column (^A aborts):
        Msg queued
        */

        // For testing SendMessageReceipts()
        public void SendMessageReceipts(string messageBBS, ref TNCDevice tncDevice, string areas, List<PacketMessage> packetMessagesReceived)
        {
            _bbsConnectName = messageBBS + "-1";
            _TncDevice = tncDevice;
            _Areas = areas;

            PacketMessagesReceived = packetMessagesReceived;
            SendMessageReceipts();
        }

        private void SendMessageReceipts()
		{
			if (Singleton<PacketSettingsViewModel>.Instance.SendReceipt)
			{
				// do not send received receipt for receive receipt messages
				foreach (PacketMessage pktMsg in PacketMessagesReceived)
				{
					if (pktMsg.Area.Length > 0)	// Do not send receipt for bulletins
						continue;

					try
					{
						// Find the Subject line
						string[] msgLines = pktMsg.MessageBody.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < Math.Min(msgLines.Length, 10); i++)
						{
							if (msgLines[i].StartsWith("Date:"))
								pktMsg.JNOSDate = DateTime.Parse(msgLines[i].Substring(10, 21));
							else if (msgLines[i].StartsWith("From:"))
								pktMsg.MessageFrom = msgLines[i].Substring(6);
							else if (msgLines[i].StartsWith("To:"))
								pktMsg.MessageTo = msgLines[i].Substring(4);
							else if (msgLines[i].StartsWith("Subject:"))
							{
								if (msgLines[i].Length > 10)
								{
									pktMsg.Subject = msgLines[i].Substring(9);
								}
								break;
							}
						}
                        if (pktMsg.Subject.Contains("DELIVERED:"))
                            continue;

                        PacketMessage receiptMessage = new PacketMessage()
                        {
                            PacFormName = "SimpleMessage",
                            PacFormType = "SimpleMessage",
                            MessageNumber = Utilities.GetMessageNumberPacket(true),
                            BBSName = _bbsConnectName.Substring(0, _bbsConnectName.IndexOf('-')),
                            TNCName = _TncDevice.Name,
                            MessageTo = pktMsg.MessageFrom,
                            MessageFrom = Singleton<IdentityViewModel>.Instance.UseTacticalCallsign
                                    ? Singleton<IdentityViewModel>.Instance.TacticalCallsign
                                    : Singleton<IdentityViewModel>.Instance.UserCallsign,
                            Subject = $"DELIVERED: {pktMsg.Subject}",
                        };

						FormField[] formFields = new FormField[1];

						FormField formField = new FormField();
						formField.ControlName = "messageBody";
                        StringBuilder controlContent = new StringBuilder();
                        controlContent.AppendLine($"!LMI!{pktMsg.MessageNumber}!DR!{DateTime.Now.ToString()}");
                        controlContent.AppendLine("Your Message");
                        controlContent.AppendLine($"To: {pktMsg.MessageTo}");
                        controlContent.AppendLine($"Subject: {pktMsg.Subject}");
                        controlContent.AppendLine($"was delivered on {DateTime.Now.ToString()}"); // 7/10/2017 6:35:51 PM
                        controlContent.AppendLine($"Recipient's Local Message ID: {pktMsg.MessageNumber}");
                        formField.ControlContent = controlContent.ToString();
                        formFields[0] = formField;

						receiptMessage.FormFieldArray = formFields;
						MessageControl packetForm = new MessageControl();
						receiptMessage.MessageBody = packetForm.CreateOutpostData(ref receiptMessage);
						receiptMessage.CreateFileName();
						receiptMessage.SentTime = DateTime.Now;
						receiptMessage.MessageSize = receiptMessage.Size;
                        //_logHelper.Log(LogLevel.Info, $"Message To: {receiptMessage.MessageTo}");       // Disable if not testing
                        //_logHelper.Log(LogLevel.Info, $"Message Body: { receiptMessage.MessageBody}");  // Disable if not testing
						SendMessage(ref receiptMessage);		    // Disabled for testing
						//_packetMessagesSent.Add(receiptMessage);
					}
					catch (Exception e)
					{
                        _logHelper.Log(LogLevel.Error, "Delivered message exception: ", e.Message);
						_error = true;
					}
				}
			}
		}

        private string GetBulletinSubject(string bulletinInfo)
        {
            string subject = "";
            string[] bulletinData = bulletinInfo.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 7; i < bulletinData.Length; i++)
            {
                subject += bulletinData[i] + ' '; 
            }
            subject.Trim();

            return subject;
        }

        public bool IsBulletinDownLoaded(string area, string bulletinSubject)
        {
            if (string.IsNullOrEmpty(area))
                return false;

            foreach (string subject in BulletinHelpers.BulletinDictionary[area])
            {
                //_logHelper.Log(LogLevel.Info, $"Subject: {subject} - Bulletin subject: {bulletinSubject}");
                if (subject.Trim() == bulletinSubject.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        private void ReceiveMessages(string area)
        {
            string readText;
            string readCmdText;
            _serialPort.ReadTimeout = 240000;
            try
            {
                if (area.Length != 0)
                {
                    _serialPort.Write($"A {area}\r\x05");        // A XSCPERM
                    readText = _serialPort.ReadTo(") >");        // read response
                    _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                    //readText = _serialPort.ReadTo("\n");         // Next command
                    //Debug.WriteLine(readText + "\n");

                    if (!_forceReadBulletins && readText.Contains("0 messages"))
                    {
						//log.Info("Skip read bulletin 1");
						return;
                    }
                    if (!_forceReadBulletins && readText.Contains("0 new"))
                    {
						//log.Info("Skip read bulletin 2");
						return;
                    }
                    _logHelper.Log(LogLevel.Info, $"Force read bulletin {area}: {_forceReadBulletins.ToString()}");
					_serialPort.Write("LA\r");
                }
                else
                {
                    //log.Info($"Timeout = {_serialPort.ReadTimeout}");        // For testing
                    _serialPort.Write("LM\r");
                }
                readText = _serialPort.ReadTo(") >");      // read response
                _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                readCmdText = _serialPort.ReadTo("\n");         // Next command

                // read messages
                string[] lines = readText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                bool firstMessageDescriptionDetected = false;
                foreach (string line in lines)
                {
                    if (_error)
                    {
                        _logHelper.Log(LogLevel.Error, $"Error in receive messages");
                        break;
                    }

                    if (line[0] != '(' && (line[0] == '>' || firstMessageDescriptionDetected))
                    {
                        //string lineCopy = line.Substring(1);        // Remove the first character which may be ' ' or '>'
                        string lineCopy = line.TrimStart(new char[] { ' ', '>' });

                        firstMessageDescriptionDetected = true;
                        string[] lineSections = lineCopy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        //Console.WriteLine("lineSections length: " + lineSections.Length);
                        if (char.IsLetter(lineSections[1][0]))        // No more messages in the list. Not sure this works!
                            break;

                        string bulletinSubject = GetBulletinSubject(lineCopy);
                        if (area.Length == 0 || !IsBulletinDownLoaded(area, bulletinSubject))
                        {
                            PacketMessage packetMessage = new PacketMessage()
                            {
                                BBSName = _bbsConnectName.Substring(0, _bbsConnectName.IndexOf('-')),
                                TNCName = _TncDevice.Name,
                                MessageNumber = Utilities.GetMessageNumberPacket(true),
                                Area = area,
                                MessageSize = Convert.ToInt32(lineSections[6]),
                            };
                            //Console.WriteLine(packetMessage.MessageSize.ToString());
                            int msgIndex = Convert.ToInt32(lineSections[1]);
                            //Console.WriteLine(msgIndex.ToString());

                            _serialPort.Write("R " + msgIndex + "\r\x05");
                            readText = _serialPort.ReadTo(") >");      // read response
                                                                       //Debug.WriteLine(readText + _BBSPrompt);
                            _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                            readCmdText = _serialPort.ReadTo("\n");     // Next command
                                                                        //Debug.WriteLine(readCmdText + "\n");

                            packetMessage.MessageBody = readText.Substring(0, readText.Length - 3); // Remove beginning of prompt
                            packetMessage.ReceivedTime = DateTime.Now;
                            PacketMessagesReceived.Add(packetMessage);
                            if (area.Length == 0)
                            {
                                _serialPort.Write("K " + msgIndex + "\r\x05");
                                readText = _serialPort.ReadTo(") >");      // read response
                                _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                                readCmdText = _serialPort.ReadTo("\n");       // Read rest of prompt
                            }
                        }
                    }
                }
			}
			catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Receive message exception: {e.Message}");
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
				_error = true;
			}
			_serialPort.ReadTimeout = 5000;
        }

        async void OnSerialPortErrorReceivedAsync(object sender, SerialErrorReceivedEventArgs e)
        {
            _logHelper.Log(LogLevel.Fatal, $"SerialPort Error: {e.EventType.ToString()}");
            _error = true;
            await Utilities.ShowMessageDialogAsync(sender as CoreDispatcher, $"SerialPort Error: {e.EventType.ToString()}", "TNC Connect Error");
            _serialPort.Close();
            return;
        }

        //delegate void CloseDialogWindow(Window window);
        //void CloseWindow(Window window) => window.Close();

        //public void CloseDlgWindow(Window window)
        //{
        //    if ((window.Dispatcher.CheckAccess()))
        //    {
        //        window.Close();
        //    }
        //    else
        //    {
        //        window.Dispatcher.Invoke(DispatcherPriority.Normal, new CloseDialogWindow(CloseWindow), window);
        //    }
        //}

        public async Task BBSConnectThreadProcAsync()
        {
            _packetMessagesSent.Clear();
            PacketMessagesReceived.Clear();

            if (string.IsNullOrEmpty(_bbsConnectName))
                return;

            Parity parity = _TncDevice.CommPort.Parity;
            ushort dataBits = _TncDevice.CommPort.Databits;
            StopBits stopBits = _TncDevice.CommPort.Stopbits;
            _serialPort = new SerialPort(_TncDevice.CommPort.Comport, _TncDevice.CommPort.Baudrate,
                parity, dataBits, stopBits);
            _serialPort.RtsEnable = _TncDevice.CommPort.Flowcontrol == Handshake.RequestToSend ? true : false;
            _serialPort.Handshake = _TncDevice.CommPort.Flowcontrol;
            _serialPort.WriteTimeout = 5000;
            _serialPort.ReadTimeout = 5000;
            _serialPort.ReadBufferSize = 8192;
            _serialPort.WriteBufferSize = 4096;
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnSerialPortErrorReceivedAsync);
            _logHelper.Log(LogLevel.Info, "");
            //Console.WriteLine($"{_TncDevice.Name}: {_TncDevice.CommPort.Comport}, {_TncDevice.CommPort.Baudrate}, {_TncDevice.CommPort.Databits}, {_TncDevice.CommPort.Stopbits}, {_TncDevice.CommPort.Parity}, {_TncDevice.CommPort.Flowcontrol}");
            _logHelper.Log(LogLevel.Info, $"{DateTime.Now.ToString()}");
            _logHelper.Log(LogLevel.Info, $"{_TncDevice.Name}: {_TncDevice.CommPort.Comport}, {_TncDevice.CommPort.Baudrate}, {_TncDevice.CommPort.Databits}, {_TncDevice.CommPort.Stopbits}, {_TncDevice.CommPort.Parity}, {_TncDevice.CommPort.Flowcontrol}");
            string readText = "";
            string readCmdText = "";
            try
            {
                _connectState = ConnectState.ConnectStateNone;

                _serialPort.Open();

                _connectState = ConnectState.ConnectStatePrepareTNCType;
                if (_TncDevice.Name == "XSC_Kantronics_KPC3-Plus")
                {
                    readCmdText = KPC3Plus();
                }
                else if (_TncDevice.Name == "XSC_Kenwood_TM-D710A" || _TncDevice.Name == "XSC_Kenwood_TH-D72A")
                {
                    readCmdText = Kenwood();
                }
                // Send Precommands
                string preCommands = _TncDevice.InitCommands.Precommands;
                string[] preCommandLines = preCommands.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                _connectState = ConnectState.ConnectStatePrepare;
                foreach (string commandLine in preCommandLines)
                {
                    _serialPort.Write(commandLine + "\r\n");

                    readText = _serialPort.ReadLine();       // Read command
                    _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

                    if (!commandLine.Contains("daytime"))   // daytime has no result
                    {
                        readText = _serialPort.ReadLine();       // Result for command
                        _logHelper.Log(LogLevel.Info, readText);
                    }

                    readCmdText = _serialPort.ReadTo(_TNCPrompt);	// Next command
                }
                // Connect to JNOS
                int readTimeout = _serialPort.ReadTimeout;
                _serialPort.ReadTimeout = 120000;
                BBSConnectTime = DateTime.Now;
                _connectState = ConnectState.ConnectStateBBSTryConnect;
                _serialPort.Write("connect " + _bbsConnectName + "\r");

                readText = _serialPort.ReadLine();			// Read command
                _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText + "\r\n");		// log last Write command

                //readText = _serialPort.ReadLine();			// Read command response
                //if (readText.ToLower().Contains(_TncDevice.Prompts.Timeout))
                //{
                //	await Utilities.ShowMessageDialogAsync("Timeout connecting to the BBS.\nIs the BBS connect name and frequency correct?\nIs the antenna connected.\nThe BBS may be out of reach.", "BBS Connect Error");
                //	goto Disconnect;
                //}

                _connectState = ConnectState.ConnectStateBBSConnect;
                string readConnectText = _serialPort.ReadTo(") >");      // read connect response  
                //Debug.WriteLine(readConnectText + _BBSPrompt);
                _logHelper.Log(LogLevel.Info, readText + "\r\n" + readConnectText + _BBSPrompt);
                _serialPort.ReadTimeout = readTimeout;

                readText = _serialPort.ReadTo("\n");	// Next command
                //Debug.WriteLine(readText + "\n");

                _serialPort.Write("XM 0\r\x05");
                readText = _serialPort.ReadLine();      // Read command
                //Debug.WriteLine(readText);
                _logHelper.Log(LogLevel.Info, readText);

                readCmdText = _serialPort.ReadLine();	// Read prompt
                //Debug.WriteLine(readCmdText);
                _logHelper.Log(LogLevel.Info, readCmdText);

                _logHelper.Log(LogLevel.Info, $"Messages to send: {_packetMessagesToSend.Count}");
                // Send messages
                foreach (PacketMessage packetMessage in _packetMessagesToSend)
                {
                    if (_error)
                    {
                        _logHelper.Log(LogLevel.Error, $"Error detected in send messages");
                        break;
                    }

                    if (_bbsConnectName.Contains(packetMessage.BBSName))
                    {
                        _serialPort.ReadTimeout = 240000;
                        try
                        {
                            _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                            _serialPort.Write(packetMessage.Subject + "\r");
                            _serialPort.Write(packetMessage.MessageBody + "\r\x1a\r\x05");

                            readText = _serialPort.ReadLine();       // Read SP
                            //Debug.WriteLine(readText);
                            _logHelper.Log(LogLevel.Info, readText);

                            readText = _serialPort.ReadTo(") >");      // read response
                            //Debug.WriteLine(readText + _BBSPrompt);
                            _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                            readText = _serialPort.ReadTo("\n");         // Next command
                            //Debug.WriteLine(readText + "\n");

                            packetMessage.SentTime = DateTime.Now;
                            _packetMessagesSent.Add(packetMessage);
                        }
                        catch (Exception e)
                        {
                            _logHelper.Log(LogLevel.Info, $"Send message exception: {e.Message}");
                            _serialPort.DiscardInBuffer();
                            _serialPort.DiscardOutBuffer();
                            _error = true;
                        }
                        _serialPort.ReadTimeout = 5000;
                    }
                }

                if (!readConnectText.Contains("0 messages") && !_error)
                {
                    ReceiveMessages("");
                }

                if (!string.IsNullOrEmpty(_Areas) && !_error)
                {
                    var areas = _Areas.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string area in areas)
                    {
                        ReceiveMessages(area);
                    }
                }
                if (!_error)
                {
                    SendMessageReceipts();                  // Send message receipts
                }

                _serialPort.Write("B\r");                   // Disconnect from BBS (JNOS)

                readText = _serialPort.ReadLine();           // Read command
                //Debug.WriteLine(readText);
                _logHelper.Log(LogLevel.Info, readText);
                readText = _serialPort.ReadLine();           // Read disconnect command

                _logHelper.Log(LogLevel.Info, readText);

                readText = _serialPort.ReadLine();           // Read disconnect response
                _logHelper.Log(LogLevel.Info, readText);

                BBSDisconnectTime = DateTime.Now;
                //serialPort.Write(cmd, 0, 1);            // Ctrl-C to return to cmd mode. NOT for Kenwood

                _serialPort.ReadTimeout = 5000;
                readCmdText = _serialPort.ReadTo(_TNCPrompt);      // Next command

                // Send PostCommands
                string postCommands = _TncDevice.InitCommands.Postcommands;
                string[] postCommandLines = postCommands.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                _connectState = ConnectState.ConnectStatePost;
                foreach (string commandLine in postCommandLines)
                {
                    _serialPort.Write(commandLine + "\r");

                    readText = _serialPort.ReadLine();				// Read command
                    _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

                    readText = _serialPort.ReadLine();              // Command result
                    _logHelper.Log(LogLevel.Info, readText);

                    readCmdText = _serialPort.ReadTo(_TNCPrompt);	// Next command
                }
                // Enter converse mode and send FCC call sign
                _connectState = ConnectState.ConnectStateConverseMode;
                _serialPort.Write(_TncDevice.Commands.Conversmode + "\r");
                readText = _serialPort.ReadLine();       // Read command
                _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText);

                string fccId = $"FCC Station ID = {Singleton<IdentityViewModel>.Instance.UserCallsign}";
                _serialPort.Write(fccId + "\r");
                readText = _serialPort.ReadLine();
                _logHelper.Log(LogLevel.Info, readText);
                _serialPort.Write("\x03\r");                        // Ctrl-C exits converse mode
                readCmdText = _serialPort.ReadTo(_TNCPrompt);
                _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Serial port exception: {e.GetType().ToString()}");
                _logHelper.Log(LogLevel.Error, $"Serial port exception. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())} {e.Message}");
                if (_connectState == ConnectState.ConnectStateBBSTryConnect)
                {
                    await Utilities.ShowMessageDialogAsync("It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach", "BBS Connect Error");
                }
                else if (_connectState == ConnectState.ConnectStatePrepareTNCType)
                {
                    await Utilities.ShowMessageDialogAsync("Unable to connect to the TNC.\nIs the TNC on?\nFor Kenwood; is the radio in \"packet12\" mode?", "BBS Connect Error");
                }
                else if (_connectState == ConnectState.ConnectStateConverseMode)
                {
                    await Utilities.ShowMessageDialogAsync($"Error sending FCC Identification - {Singleton<IdentityViewModel>.Instance.UserCallsign}.", "TNC Converse Error");
                }
                //else if (e.Message.Contains("not exist"))
                else if (e.GetType() == typeof(IOException))
                {
                    await Utilities.ShowMessageDialogAsync("Looks like the USB cable to the TNC is disconnected", "TNC Connect Error");
                }
                else if (e.GetType() == typeof(UnauthorizedAccessException))
                {
                    await Utilities.ShowMessageDialogAsync($"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application. ", "TNC Connect Error");
                }
                //_serialPort.Write("B\r\n");
                //_serialPort.Close();
            }
            finally
            {
                _serialPort.Close();
            }

            //SendMessageReceipts();          // TODO testing
            //CloseDlgWindow(ConnectDlg);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					_serialPort.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~TNCInterface() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
