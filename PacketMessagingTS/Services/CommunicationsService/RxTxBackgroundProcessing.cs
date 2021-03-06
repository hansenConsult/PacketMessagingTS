﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FormControlBaseClass;

using MessageFormControl;

using MetroLog;

//using PacketMessagingTS.BackgroundTasks;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using SharedCode;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;

namespace PacketMessagingTS.Services.CommunicationsService
{
    public sealed class RxTxBackgroundTask : IBackgroundTask
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<RxTxBackgroundTask>();
        private static LogHelper _logHelper = new LogHelper(log);

        public ApplicationTrigger _applicationTrigger = new ApplicationTrigger();

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
        const string _BBSPromptRN = ") >\r\n";
        string _TNCPrompt = "cmd:";

        private bool _error = false;        // Disconnect if an error is detected

        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null;
        string _result = "";

        // The Run method is the entry point of a background task.        
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _logHelper.Log(LogLevel.Info, $"Background {taskInstance.Task.Name} Starting...");

            // Query BackgroundWorkCost
            // Guidance: If BackgroundWorkCost is high, then perform only the minimum amount
            // of work in the background task and return immediately.
            var cost = BackgroundWorkCost.CurrentBackgroundWorkCost;
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values["BackgroundWorkCost"] = cost.ToString();

            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            await BBSConnectThreadProcAsync();
        }

        // Handles background task cancellation.
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Indicate that the background task is canceled.
            _cancelRequested = true;
            _cancelReason = reason;

            _logHelper.Log(LogLevel.Info, "Background {sender.Task.Name} Cancel Requested...");
        }

        public static string Message { get; set; }


        public RxTxBackgroundTask(string messageBBS, ref TNCDevice tncDevice, bool forceReadBulletins, string areas, ref List<PacketMessage> packetMessagesToSend)
        {
            _bbsConnectName = messageBBS;
            _TncDevice = tncDevice;
            _TNCPrompt = _TncDevice.Prompts.Command;
            _forceReadBulletins = forceReadBulletins;
            _Areas = areas;
            _packetMessagesToSend = packetMessagesToSend;
        }

        public RxTxBackgroundTask()
        {
        }

        public void Initialize(string messageBBS, ref TNCDevice tncDevice, bool forceReadBulletins, string areas, ref List<PacketMessage> packetMessagesToSend)
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

        private List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
        public List<PacketMessage> PacketMessagesReceived { get => _packetMessagesReceived; set => _packetMessagesReceived = value; }

        private string KPC3Plus()
        {
            string readText = _serialPort.ReadLine();       // This appears to be a dummy read for KPC3
            _logHelper.Log(LogLevel.Info, readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            string readCmdText = _serialPort.ReadTo(_TNCPrompt);

            _serialPort.Write("D\r");
            //Thread.Sleep(100);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);

            _serialPort.Write("b\r");

            readText = _serialPort.ReadLine();       // Command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for b
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);

            _serialPort.Write("Echo on\r");

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for Echo on
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);

            // TODO add tactical callsign
            _serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.UserCallsign + "\r");

            readText = _serialPort.ReadLine();       // Read command
            log.Info(readCmdText + _TNCPrompt + readText);
            // Note no command response

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);

            _serialPort.Write("Mon off\r");

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + readText);

            readText = _serialPort.ReadLine();       // Result for Mon off
            _logHelper.Log(LogLevel.Info, readText);

            readCmdText = _serialPort.ReadTo(_TNCPrompt);
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write("daytime " + dayTime + "\r");

            readText = _serialPort.ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + readText);
            // Note no command response

            readCmdText = _serialPort.ReadTo(_TNCPrompt);      // Ready for pre commands
            _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt);
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

            readCmdText = _serialPort.ReadTo(_TNCPrompt);   // Ready for pre commands
            return readCmdText;
        }

        // Returns the string including readTo. As opposed to SerialPort.ReadTo()
        private string ReadTo(string readTo)
        {
            string readText = "";
            while (!readText.Contains(readTo))
            {
                string newText = _serialPort.ReadExisting();
                readText += newText;
                //if (newText.Length > 0)
                //{
                //    Debug.Write(newText);
                //}
                Thread.Sleep(10);
            }

            return readText;
        }

        private void SendMessage(ref PacketMessage packetMessage)
        {
            int readTimeout = _serialPort.ReadTimeout;
            _serialPort.ReadTimeout = 240000;
            try
            {
                _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                _serialPort.Write(packetMessage.Subject + "\r");
                _serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");

                //string readText = _serialPort.ReadLine();       // Read SP
                //_logHelper.Log(LogLevel.Info, readText);

                string readText = _serialPort.ReadTo(_BBSPromptRN);      // read response
                _logHelper.Log(LogLevel.Info, readText);

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
            _serialPort.ReadTimeout = readTimeout;
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
                    if (pktMsg.Area.Length > 0) // Do not send receipt for bulletins
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
                        SendMessage(ref receiptMessage);            // Disabled for testing
                    }
                    catch (Exception e)
                    {
                        _logHelper.Log(LogLevel.Error, "Delivered message exception: ", e.Message);
                        _error = true;
                        throw;
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
            _serialPort.ReadTimeout = 240000;
            //_serialPort.ReadTimeout = 10000;
            try
            {
                if (area.Length != 0)
                {
                    //_serialPort.Write($"A {area}\r\x05");        // A XSCPERM
                    _serialPort.Write($"A {area}\r");        // A XSCPERM
                    readText = _serialPort.ReadTo(_BBSPromptRN);        // read response
                    _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);

                    if (!_forceReadBulletins && readText.Contains("0 messages"))
                    {
                        return;
                    }
                    if (!_forceReadBulletins && readText.Contains("0 new"))
                    {
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
                //readText = _serialPort.ReadTo(_BBSPromptRN);      // read response
                //_logHelper.Log(LogLevel.Info, readText + _BBSPrompt);
                readText = ReadTo(_BBSPromptRN);      // read response
                _logHelper.Log(LogLevel.Info, readText);

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
                        // St.  #  TO         FROM     DATE   SIZE SUBJECT
                        // > N   1 kz6dm      kz6dm    Feb  3  867 6DM-349P_O/R_ICS213_dfdsfsdfggh    
                        string lineCopy = line.TrimStart(new char[] { ' ', '>' });  // Remove the first character which may be ' ' or '>'

                        firstMessageDescriptionDetected = true;
                        string[] lineSections = lineCopy.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        //Console.WriteLine("lineSections length: " + lineSections.Length);
                        if (char.IsLetter(lineSections[1][0]))        // No more messages in the list. Not sure this works!
                            break;

                        string bulletinSubject = GetBulletinSubject(lineCopy);
                        // Read messages or bulletins if the bulletin is not already read.
                        if (area.Length == 0 || !IsBulletinDownLoaded(area, bulletinSubject))
                        {
                            PacketMessage packetMessage = new PacketMessage()
                            {
                                BBSName = _bbsConnectName.Substring(0, _bbsConnectName.IndexOf('-')),
                                TNCName = _TncDevice.Name,
                                MessageNumber = Utilities.GetMessageNumberPacket(true),
                                Area = area,
                                MessageSize = Convert.ToInt32(lineSections[6]),
                                MessageState = MessageState.Locked,
                            };
                            int msgIndex = Convert.ToInt32(lineSections[1]);

                            _serialPort.Write("R " + msgIndex + "\r");
                            //readText = _serialPort.ReadTo(_BBSPromptRN);      // read response eg R1 plus message
                            //_logHelper.Log(LogLevel.Info, readText + _BBSPrompt);
                            readText = ReadTo(_BBSPromptRN);      // read response eg R1 plus message
                            _logHelper.Log(LogLevel.Info, readText);

                            packetMessage.MessageBody = readText.Substring(0, readText.Length - 3); // Remove beginning of prompt
                            packetMessage.ReceivedTime = DateTime.Now;
                            PacketMessagesReceived.Add(packetMessage);
                            if (area.Length == 0)
                            {
                                _serialPort.Write("K " + msgIndex + "\r");
                                readText = _serialPort.ReadTo(_BBSPromptRN);      // read response
                                _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);
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
                throw;

            }
            finally
            {
                _serialPort.ReadTimeout = 5000;
            }
        }

        async void OnSerialPortErrorReceivedAsync(object sender, SerialErrorReceivedEventArgs e)
        {
            _logHelper.Log(LogLevel.Fatal, $"SerialPort Error: {e.EventType.ToString()}");
            _error = true;
            await Utilities.ShowSingleButtonMessageDialogAsync(sender as CoreDispatcher, $"SerialPort Error: {e.EventType.ToString()}", "Close", "TNC Connect Error");
            _serialPort.Close();
            return;
        }

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
                    //_serialPort.Write(commandLine + "\r\n");
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
                //_serialPort.ReadTimeout = 5000;
                _serialPort.ReadTimeout = 120000;
                BBSConnectTime = DateTime.Now;
                _connectState = ConnectState.ConnectStateBBSTryConnect;
                _serialPort.Write("connect " + _bbsConnectName + "\r");

                readText = _serialPort.ReadLine();			// Read command
                _logHelper.Log(LogLevel.Info, readCmdText + _TNCPrompt + " " + readText + "\r\n");		// log last Write command

                _connectState = ConnectState.ConnectStateBBSConnect;
                string readConnectText = _serialPort.ReadTo(_BBSPromptRN);      // read connect response  
                _logHelper.Log(LogLevel.Info, readText + "\r\n" + readConnectText + _BBSPrompt);

                //_logHelper.Log(LogLevel.Info, readText + "\r\n" + readConnectText);
                _serialPort.ReadTimeout = readTimeout;

                _serialPort.Write("XM 0\r");
                //readText = _serialPort.ReadLine();      // Read command
                //_logHelper.Log(LogLevel.Info, readText);

                readCmdText = _serialPort.ReadTo(_BBSPromptRN);	// Read to prompt incl command
                _logHelper.Log(LogLevel.Info, readCmdText + _BBSPrompt);

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
                            _serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");

                            //readText = _serialPort.ReadLine();       // Read SP
                            //_logHelper.Log(LogLevel.Info, readText);

                            readText = _serialPort.ReadTo(_BBSPromptRN);      // read response
                            _logHelper.Log(LogLevel.Info, readText + _BBSPrompt);   // Subject + message body plus stuff

                            packetMessage.SentTime = DateTime.Now;
                            _packetMessagesSent.Add(packetMessage);
                        }
                        catch (Exception e)
                        {
                            _logHelper.Log(LogLevel.Info, $"Send message exception: {e.Message}");
                            _serialPort.DiscardInBuffer();
                            _serialPort.DiscardOutBuffer();
                            _error = true;
                            throw;
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

                _logHelper.Log(LogLevel.Info, readText);     // Log disconnect response

                readText = _serialPort.ReadTo(_TNCPrompt);
                _logHelper.Log(LogLevel.Info, readText + _TNCPrompt);

                BBSDisconnectTime = DateTime.Now;
                //serialPort.Write(cmd, 0, 1);            // Ctrl-C to return to cmd mode. NOT for Kenwood

                _serialPort.ReadTimeout = 5000;
                //readCmdText = _serialPort.ReadTo(_TNCPrompt);      // Next command
                readCmdText = "";

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
                if (_connectState == ConnectState.ConnectStateBBSConnect)
                {
                    //await Utilities.ShowSingleButtonContentDialogAsync("It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach", "Close", "BBS Connect Error");
                    _result = "It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach";
                }
                else if (_connectState == ConnectState.ConnectStatePrepareTNCType)
                {
                    //await Utilities.ShowSingleButtonContentDialogAsync("Unable to connect to the TNC.\nIs the TNC on?\nFor Kenwood; is the radio in \"packet12\" mode?", "Close", "BBS Connect Error");
                    _result = "";
                }
                else if (_connectState == ConnectState.ConnectStateConverseMode)
                {
                    //await Utilities.ShowSingleButtonContentDialogAsync($"Error sending FCC Identification - {Singleton<IdentityViewModel>.Instance.UserCallsign}.", "Close", "TNC Converse Error");
                    _result = $"Error sending FCC Identification - { Singleton<IdentityViewModel>.Instance.UserCallsign}.";
                }
                //else if (e.Message.Contains("not exist"))
                else if (e.GetType() == typeof(IOException))
                {
                    //await Utilities.ShowSingleButtonContentDialogAsync("Looks like the USB or serial cable to the TNC is disconnected", "Close", "TNC Connect Error");
                    _result = "Looks like the USB or serial cable to the TNC is disconnected";
                }
                else if (e.GetType() == typeof(UnauthorizedAccessException))
                {
                    //await Utilities.ShowSingleButtonContentDialogAsync($"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application. ", "Close", "TNC Connect Error");
                    _result = $"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application.";
                }

                if (_connectState == ConnectState.ConnectStateBBSConnect)
                {
                    _serialPort.Write("B\r\n");
                    string disconnectString = _serialPort.ReadLine();
                    _logHelper.Log(LogLevel.Trace, disconnectString);
                    BBSDisconnectTime = DateTime.Now;
                }
            }
            finally
            {
                _serialPort.Close();
            }

            //SendMessageReceipts();          // TODO testing
            //CloseDlgWindow(ConnectDlg);

            // Indicate that the background task has completed.
            _deferral.Complete();
        }

        //public override void Register()
        //{
        //    string taskName = GetType().Name;
        //    //var taskRegistration = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == taskName).Value;

        //    //if (taskRegistration == null)
        //    //{
        //        var builder = new BackgroundTaskBuilder()
        //        {
        //            Name = taskName
        //        };

        //        // TODO WTS: Define the trigger for your background task and set any (optional) conditions
        //        // More details at https://docs.microoft.com/windows/uwp/launch-resume/create-and-register-an-inproc-background-task
        //        builder.SetTrigger(_applicationTrigger);
        //        //builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

        //        builder.Register();
        //    // }
        //}

        //public override Task RunAsyncInternal(IBackgroundTaskInstance taskInstance)
        //{
        //    if (taskInstance == null)
        //    {
        //        return null;
        //    }

        //    _deferral = taskInstance.GetDeferral();

        //    return Task.Run(async () =>
        //    {
        //        //// TODO WTS: Insert the code that should be executed in the background task here.
        //        //// This sample initializes a timer that counts to 100 in steps of 10.  It updates Message each time.

        //        //// Documentation:
        //        ////      * General: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/support-your-app-with-background-tasks
        //        ////      * Debug: https://docs.microsoft.com/en-us/windows/uwp/launch-resume/debug-a-background-task
        //        ////      * Monitoring: https://docs.microsoft.com/windows/uwp/launch-resume/monitor-background-task-progress-and-completion

        //        //// To show the background progress and message on any page in the application,
        //        //// subscribe to the Progress and Completed events.
        //        //// You can do this via "BackgroundTaskService.GetBackgroundTasksRegistration"

        //        _taskInstance = taskInstance;
        //        await BBSConnectThreadProcAsync();
        //    });

        //}

        //public override void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        //{
        //    _cancelRequested = true;

        //    // TODO WTS: Insert code to handle the cancelation request here.
        //    // Documentation: https://docs.microsoft.com/windows/uwp/launch-resume/handle-a-cancelled-background-task
        //}

        private void OnProgress(IBackgroundTaskRegistration task, BackgroundTaskProgressEventArgs args)
        {
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    ExampleProgressElement.Text = "Progress is at " args.Progress + "%.";
            //});
        }

    }
}
