using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using FormControlBaseClass;
using MessageFormControl;
using MetroLog;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Helpers;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Core;

namespace PacketMessagingTS.Services.CommunicationsService
{
	public class TNCInterface: IDisposable
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCInterface>();
        Helpers.LogHelper _logHelper = new Helpers.LogHelper(log);

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
        List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
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

        public List<PacketMessage> PacketMessagesReceived => _packetMessagesReceived;

        public DateTime BBSConnectTime
        { get; set; }

		public DateTime BBSDisconnectTime
		{ get; set; }

		//public ConnectedDialog ConnectDlg
  //      { get; set; }

		public bool Cancel
		{ get { return _error; } set { _error = value; } }

        private string KPC3Plus()
        {
            string readText = _serialPort.ReadLine();       // This appears to be a dummy read for KPC3
            Debug.WriteLine(readText);
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
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("b\r");

            readText = _serialPort.ReadLine();
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("Echo on\r");

            readText = _serialPort.ReadLine();
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            log.Info(readText);

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
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("Mon off\r");

            readText = _serialPort.ReadLine();       // Read command
            log.Info(readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for Mon off
            log.Info(readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write("daytime " + dayTime + "\r");

            readText = _serialPort.ReadLine();       // Read command
            log.Info(readCmdText + _TNCPrompt + readText);
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

				string readText = _serialPort.ReadLine();       // Read SP
				Debug.WriteLine(readText);
				log.Info(readText);

				readText = _serialPort.ReadTo(_BBSPrompt);      // read response
				Debug.WriteLine(readText + _BBSPrompt);
				log.Info(readText + _BBSPrompt);

				readText = _serialPort.ReadTo("\n");         // Next command
				Debug.WriteLine(readText + "\n");

				packetMessage.SentTime = DateTime.Now;
				_packetMessagesSent.Add(packetMessage);
			}
			catch (Exception e)
			{
				log.Error("Send message exception:", e);
				_serialPort.DiscardInBuffer();
				_serialPort.DiscardOutBuffer();
				_error = true;
			}
			_serialPort.ReadTimeout = 5000;
		}

		private void SendMessageReceipts()
		{
			if (Singleton<PacketSettingsViewModel>.Instance.SendReceipt)
			{
				// do not send received receipt for receive receipt messages
				foreach (PacketMessage pktMsg in _packetMessagesReceived)
				{
					if (pktMsg.Area.Length > 0)			// Do not send receipt for bulletins
						continue;

					try
					{
						// Find the Subject line
						string[] msgLines = pktMsg.MessageBody.Split(new string[] { "\r\n" }, StringSplitOptions.None);
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

						if (!pktMsg.Subject.Contains("DELIVERED:") && pktMsg.Area.Length == 0)
						{
							PacketMessage receiptMessage = new PacketMessage();
							//receiptMessage.PacFormType = PacForms.Message;
							receiptMessage.PacFormName = "SimpleMessage";
							receiptMessage.MessageNumber = Utilities.GetMessageNumberPacket(true);
							receiptMessage.BBSName = _bbsConnectName.Substring(0, _bbsConnectName.IndexOf('-'));
							receiptMessage.TNCName = _TncDevice.Name;
							receiptMessage.MessageTo = pktMsg.MessageFrom;
							receiptMessage.MessageFrom = Singleton<IdentityViewModel>.Instance.UseTacticalCallsign
                                        ? Singleton<IdentityViewModel>.Instance.TacticalCallsign
                                        : Singleton<IdentityViewModel>.Instance.UserCallsign;

							receiptMessage.Subject = $"DELIVERED: {pktMsg.Subject}";

							FormField[] formFields = new FormField[1];

							FormField formField = new FormField();
							formField.ControlName = "messageBody";
							formField.ControlContent = $"!LMI!{pktMsg.MessageNumber}!DR!{pktMsg.ReceivedTime?.ToString("G")}\r\n";
							formField.ControlContent += "Your Message\r\n";
							formField.ControlContent += $"To: {pktMsg.MessageTo}\r\n";
							formField.ControlContent += $"Subject: {pktMsg.Subject}\r\n";
							//formField.ControlContent += $"was delivered on {pktMsg.MessageReceiveTime.ToShortDateString()} {pktMsg.MessageReceiveTime.ToShortTimeString()}\r\n";
							formField.ControlContent += $"was delivered on {pktMsg.ReceivedTime?.ToString("G")}\r\n";
							formField.ControlContent += $"Recipient's Local Message ID: {pktMsg.MessageNumber}\r\n";
							formFields[0] = formField;

							receiptMessage.FormFieldArray = formFields;
							MessageControl packetForm = new MessageControl();
							receiptMessage.MessageBody = packetForm.CreateOutpostData(ref receiptMessage);
							receiptMessage.CreateFileName();
							receiptMessage.SentTime = DateTime.Now;
							receiptMessage.MessageSize = receiptMessage.Size;
							log.Info(receiptMessage.MessageBody);   // Disable if not testing
							//SendMessage(ref receiptMessage);		// Disabled for testing
							_packetMessagesSent.Add(receiptMessage);
						}
					}
					catch (Exception e)
					{
                        _logHelper.Log(LogLevel.Error, "Delivered message exception: ", e.Message);
						_error = true;
					}
				}
			}
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
                    Debug.WriteLine(readText + _BBSPrompt);
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
					log.Info($"Read bulletin {_forceReadBulletins.ToString()}");
					_serialPort.Write("LA\r");
                }
                else
                {
                    //log.Info($"Timeout = {_serialPort.ReadTimeout}");        // For testing
                    _serialPort.Write("LM\r");
                }
                readText = _serialPort.ReadTo(") >");      // read response
                Debug.WriteLine(readText + _BBSPrompt);
                log.Info(readText + _BBSPrompt);

                readCmdText = _serialPort.ReadTo("\n");         // Next command
                Debug.WriteLine(readCmdText + "\n");

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
                        Debug.WriteLine(readText + _BBSPrompt);
                        log.Info(readText + _BBSPrompt);

                        readCmdText = _serialPort.ReadTo("\n");		// Next command
                        Debug.WriteLine(readCmdText + "\n");

                        packetMessage.MessageBody = readText.Substring(0, readText.Length - 3); // Remove beginning of prompt
						packetMessage.ReceivedTime = DateTime.Now;
						_packetMessagesReceived.Add(packetMessage);

                        if (area.Length == 0)
                        {
                            _serialPort.Write("K " + msgIndex + "\r\x05");
                            readText = _serialPort.ReadTo(") >");      // read response
                            Debug.WriteLine(readText + _BBSPrompt);
                            log.Info(readText + _BBSPrompt);

                            readCmdText = _serialPort.ReadTo("\n");       // Read rest of prompt
                            Debug.WriteLine(readCmdText + "\n");
                        }
                    }
				}
			}
			catch (Exception e)
            {
                log.Error("Receive message exception:", e);
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
				_error = true;
			}
			_serialPort.ReadTimeout = 5000;

        }

        async void OnSerialPortErrorReceivedAsync(object sender, SerialErrorReceivedEventArgs e)
        {
            log.Error($"SerialPort Error: {e.EventType.ToString()}");
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
            _packetMessagesReceived.Clear();

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
			log.Info("");
            Console.WriteLine($"{_TncDevice.Name}: {_TncDevice.CommPort.Comport}, {_TncDevice.CommPort.Baudrate}, {_TncDevice.CommPort.Databits}, {_TncDevice.CommPort.Stopbits}, {_TncDevice.CommPort.Parity}, {_TncDevice.CommPort.Flowcontrol}");
            log.Info($"{DateTime.Now.ToString()}");
            log.Info($"{_TncDevice.Name}: {_TncDevice.CommPort.Comport}, {_TncDevice.CommPort.Baudrate}, {_TncDevice.CommPort.Databits}, {_TncDevice.CommPort.Stopbits}, {_TncDevice.CommPort.Parity}, {_TncDevice.CommPort.Flowcontrol}");
            try
            {
                _connectState = ConnectState.ConnectStateNone;
           
                _serialPort.Open();

                string readText = "";
                string readCmdText = "";

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
                    _serialPort.Write(commandLine + "\r");

                    readText = _serialPort.ReadLine();       // Read command
                    log.Info(readCmdText + _TNCPrompt + readText);

                    readText = _serialPort.ReadLine();       // Result for command
                    log.Info(readText);

                    readCmdText = _serialPort.ReadTo(_TNCPrompt);	// Next command
                }
                // Connect to JNOS
                int readTimeout = _serialPort.ReadTimeout;
                _serialPort.ReadTimeout = 120000;
                BBSConnectTime = DateTime.Now;
				_connectState = ConnectState.ConnectStateBBSTryConnect;
				_serialPort.Write("connect " + _bbsConnectName + "\r");

                readText = _serialPort.ReadLine();			// Read command
                log.Info(readCmdText + _TNCPrompt + readText);		// log last Write command

				readText = _serialPort.ReadLine();			// Read command response
				if (readText.ToLower().Contains(_TncDevice.Prompts.Timeout))
				{
					await Utilities.ShowMessageDialogAsync("Timeout connecting to the BBS.\nIs the BBS connect name and frequency correct?\nIs the antenna connected.\nThe BBS may be out of reach.", "BBS Connect Error");
					goto Disconnect;
				}

				_connectState = ConnectState.ConnectStateBBSConnect;
                string readConnectText = _serialPort.ReadTo(") >");      // read connect response
                Debug.WriteLine(readConnectText + _BBSPrompt);
                log.Info(readText + "\n" + readConnectText + _BBSPrompt);
                _serialPort.ReadTimeout = readTimeout;

                readText = _serialPort.ReadTo("\n");	// Next command
                Debug.WriteLine(readText + "\n");

                _serialPort.Write("XM 0\r\x05");
                readText = _serialPort.ReadLine();      // Read command
                Debug.WriteLine(readText);
                log.Info(readText);

                readCmdText = _serialPort.ReadLine();	// Read prompt
                Debug.WriteLine(readCmdText);
                log.Info(readCmdText);

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
                            Debug.WriteLine(readText);
                            log.Info(readText);

                            readText = _serialPort.ReadTo(") >");      // read response
                            Debug.WriteLine(readText + _BBSPrompt);
                            log.Info(readText + _BBSPrompt);

                            readText = _serialPort.ReadTo("\n");         // Next command
                            Debug.WriteLine(readText + "\n");

                            packetMessage.SentTime = DateTime.Now;
                            _packetMessagesSent.Add(packetMessage);
                        }
                        catch (Exception e)
                        {
                            log.Error("Send message exception:", e);
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
				//SendMessageReceipts();					// Send message receipts
			
				_serialPort.Write("B\r\x05");				// Disconnect from BBS (JNOS)

				readText = _serialPort.ReadLine();           // Read command
                Debug.WriteLine(readText);
                log.Info(readText);
Disconnect:
				readText = _serialPort.ReadLine();           // Read disconnect response
                Console.WriteLine(readText);
                log.Info(readText);

                BBSDisconnectTime = DateTime.Now;
				//serialPort.Write(cmd, 0, 1);            // Ctrl-C to return to cmd mode. NOT for Kenwood

				SendMessageReceipts();          // TODO testing

				_serialPort.ReadTimeout = 5000;
				readCmdText = _serialPort.ReadTo(_TNCPrompt);      // Next command
                Debug.WriteLine(readCmdText + _TNCPrompt);

                // Send PostCommands
                string postCommands = _TncDevice.InitCommands.Postcommands;
                string[] postCommandLines = postCommands.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                _connectState = ConnectState.ConnectStatePost;
                foreach (string commandLine in postCommandLines)
                {
                    _serialPort.Write(commandLine + "\r");

                    readText = _serialPort.ReadLine();				// Read command
                    log.Info(readCmdText + _TNCPrompt + readText);

                    readText = _serialPort.ReadLine();              // Command result
					log.Info(readText);

                    readCmdText = _serialPort.ReadTo(_TNCPrompt);	// Next command
                }
				// Enter converse mode and send FCC call sign
				_connectState = ConnectState.ConnectStateConverseMode;
				_serialPort.Write(_TncDevice.Commands.Conversmode + "\r");
				readText = _serialPort.ReadLine();       // Read command
				log.Info(readCmdText + _TNCPrompt + readText);

				string fccId = $"FCC Station ID = {Singleton<IdentityViewModel>.Instance.UserCallsign}";
				_serialPort.Write(fccId + "\r");
				readText = _serialPort.ReadLine();
				log.Info(readText);
				_serialPort.Write("\x03\r");						// Ctrl-C exits converse mode
				readCmdText = _serialPort.ReadTo(_TNCPrompt);
				log.Info(readCmdText + _TNCPrompt);
			}
			catch (Exception e)
            {
                //Console.WriteLine($"Serial port exception: {e.GetType().ToString()}");
                log.Error($"Serial port exception. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}.", e);
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
                _serialPort.Close();
            }
            _serialPort.Close();

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
