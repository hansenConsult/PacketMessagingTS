﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Views;
using PacketMessagingTS.ViewModels;

using PacketMessagingTS.Core.Helpers;

using SharedCode;
using SharedCode.Helpers;

namespace PacketMessagingTS.Services.CommunicationsService
{
    public class TNCInterface
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCInterface>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        enum ConnectState
        {
            None,
            PrepareTNCType,
            PreCommand,
            BBSTryConnect,
            BBSConnected,
            PostCommand,
            Disconnected,
            ConverseMode
        }
        ConnectState _connectState;
        private readonly List<PacketMessage> _packetMessagesSent = new List<PacketMessage>();
        readonly List<PacketMessage> _packetMessagesToSend;

        string _bbsConnectName = "";
        bool _forceReadBulletins = false;
        //string _Areas;
        string _AreasCommand;
        TNCDevice _TncDevice = null;
        SerialPort _serialPort = null;

        //const string _BBSPrompt = ") >";
        //const string _BBSPromptC = ") >\r";
        const string _BBSPromptRN = ") >\r\n";
        const string NewLine = "\r\n";
        //string _TNCPrompt = "cmd:";
        string _TNCPrompt;

        private bool _error = false;        // Disconnect if an error is detected

        private string _readBuffer = "";

        string _LastAccessedArea = "";
        bool _MessageListEmpty = false;

        //const byte send = 0x5;
        public TNCInterface()
        {

        }

        public TNCInterface(string bbsConnectName, ref TNCDevice tncDevice, bool forceReadBulletins, string areas, ref List<PacketMessage> packetMessagesToSend)
        {
            _bbsConnectName = bbsConnectName;
            _TncDevice = tncDevice;
            _TNCPrompt = _TncDevice.Prompts.Command;
            _forceReadBulletins = forceReadBulletins;
            //_Areas = areas;
            _AreasCommand = areas;
            _packetMessagesToSend = packetMessagesToSend;
        }

        public List<PacketMessage> PacketMessagesSent => _packetMessagesSent;

        public string ReceivedMessage
        { get; set; }

        public string SentMessage
        { get; set; }

        public DateTime BBSConnectTime
        { get; set; }

        public DateTime BBSDisconnectTime
        { get; set; }

        //public ConnectedDialog ConnectDlg
        //      { get; set; }

        //public bool Cancel
        //{ get { return _error; } set { _error = value; } }

        public void AbortConnection()
        {
            _logHelper.Log(LogLevel.Info, $"Connection aborted.");
            _error = true;
        }

        private List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
        public List<PacketMessage> PacketMessagesReceived
        {
            get => _packetMessagesReceived;
            set => _packetMessagesReceived = value;
        }

        private string KPC3Plus()
        {
            string readText;
            string cmdResult;
            //string readCmdText;
            _serialPort.Write("\r");
            //readText = _serialPort.ReadLine();
            readText = ReadLine();

            _logHelper.Log(LogLevel.Info, readText);

            cmdResult = ReadTo(_TNCPrompt);

            TNCCommand("D");

            TNCCommand("b");

            TNCCommand("Echo on");

            if (IdentityViewModel.Instance.UseTacticalCallsign)
            {
                _serialPort.Write($"my {IdentityViewModel.Instance.TacticalCallsign}\r");
                //TNCCommandNoResponse("my " + IdentityViewModel.Instance.TacticalCallsign);
            }
            else
            {
                _serialPort.Write($"my {IdentityViewModel.Instance.UserCallsign}\r");
            }
            readText = ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            cmdResult = ReadTo(_TNCPrompt);

            TNCCommand("Mon off");

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write($"daytime {dayTime}\r");
            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            cmdResult = ReadTo(_TNCPrompt);

            return cmdResult;
        }

        private string Kenwood()
        {
            string readText;
            string readPrompt;
            _serialPort.Write("\r");
            //readText = _serialPort.ReadLine();
            //readText = ReadLine();
            //readPrompt = ReadToTimeout(_TNCPrompt);

            readPrompt = ReadTo(_TNCPrompt);
            readPrompt = readPrompt.Trim();

            _logHelper.Log(LogLevel.Info, readPrompt);
            //AddTextToStatusWindowAsync(readPrompt);

            //_logHelper.Log(LogLevel.Info, readText);
            //AddTextToStatusWindowAsync(readText);

            //readPrompt = ReadTo(_TNCPrompt);

            TNCCommand("D");

            TNCCommand("b");

            TNCCommand("Echo on");

            if (IdentityViewModel.Instance.UseTacticalCallsign)
            {
                TNCCommand("my " + IdentityViewModel.Instance.TacticalCallsign);
            }
            else
            {
                TNCCommand("my " + IdentityViewModel.Instance.UserCallsign);
            }

            TNCCommand("Mon off");

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write($"daytime {dayTime}\r");

            readText = ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            readPrompt = ReadTo(_TNCPrompt);   // Ready for pre commands

            return readPrompt;
        }

        private void AddTextToStatusWindowAsync(string text)
        {
            //Thread.Sleep(1000);
//            RxTxStatViewModel rxTxStatViewModel = RxTxStatusPage.rxtxStatusPage.RxTxStatusViewmodel;

//            CoreDispatcher dispatcher = rxTxStatViewModel.ViewLifetimeControl.Dispatcher;
//            if (dispatcher == null)
//                return;

            //Debug.Write(text);
            //CoreDispatcher dispatcher = MainPage.Current.Dispatcher;
            //if (!dispatcher.HasThreadAccess)
            //{
//            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//            {
                //rxTxStatViewModel.AppendRxTxStatus = text;
                RxTxStatusPage.Current.AddTextToStatusWindow(text);
//            });
        }

        // Returns next line read, returns line read plus a \r. Supports timeout.
        private string ReadLine()
        {
            Task task = Task.Run(() =>
            {
                while (!_readBuffer.Contains(NewLine) && _serialPort.IsOpen)
                {
                    string newText = _serialPort.ReadExisting();
                    if (newText.Length > 0)
                    {
                        _readBuffer += newText;
                        AddTextToStatusWindowAsync(newText);

                        if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected) && _readBuffer.Contains(_TncDevice.Prompts.Timeout))
                        {
                            break;
                        }
                    }
                    Thread.Sleep(10);
                }
            });
            try
            {
                if (!task.Wait(_serialPort.ReadTimeout) || !_serialPort.IsOpen)
                {
                    _readBuffer = "";
                    _logHelper.Log(LogLevel.Error, $"Serial port timeout in ReadLine. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}");
                    throw (new Exception("Serial port timeout"));
                }
            }
            catch (AggregateException )
            {
                _logHelper.Log(LogLevel.Error, $"Serial port timeout in ReadLine. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}");
                _error = true;
            }

            if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected) && _readBuffer.Contains(_TncDevice.Prompts.Timeout))
            {
                string readText = _readBuffer;
                _readBuffer = "";
                //AddTextToStatusWindowAsync(readText);
                return readText;
            }

            int readLength = _readBuffer.IndexOf(NewLine) + NewLine.Length;
            string line = _readBuffer.Substring(0, _readBuffer.IndexOf(NewLine) + 1);   // Only return '\r'
            //string line = _readBuffer.Substring(0, readLength);   // return '\r\n'
            if (_readBuffer.Length > readLength)
            {
                _readBuffer = _readBuffer.Substring(readLength);
            }
            else
            {
                _readBuffer = "";
            }

            //AddTextToStatusWindowAsync(line);
            return line;
        }
        /*
        // Returns next line read, returns line read plua \r
        //private string ReadLineTimeout()
        //{
        //    //string readText = "";
        //    CancellationTokenSource tokenSource = new CancellationTokenSource(_serialPort.ReadTimeout);
        //    CancellationToken token = tokenSource.Token;
        //    Task task = Task.Run(() =>
        //    {
        //        while (!_readBuffer.Contains(NewLine))
        //        {
        //            string newText = "";
        //            if (_serialPort.IsOpen)
        //            {
        //                newText = _serialPort.ReadExisting();
        //            }
        //            else
        //            {
        //                tokenSource.Cancel();
        //                newText = "";
        //                break;
        //            }
        //            _readBuffer += newText;
        //            if (newText.Length > 0)
        //            {
        //                AddTextToStatusWindowAsync(newText);
        //            }
        //            if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected))
        //            {
        //                break;
        //            }
        //            Thread.Sleep(10);
        //        }
        //    }, token);
        //    try
        //    {
        //        task.Wait();
        //    }
        //    catch (AggregateException e)
        //    {
        //        _logHelper.Log(LogLevel.Error, $"Serial port timeout. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}");
        //        _logHelper.Log(LogLevel.Error, $"Serialport timeout, {e.InnerExceptions.GetType().Name}");
        //        throw;
        //    }

        //    int readLength = _readBuffer.IndexOf(NewLine) + NewLine.Length;
        //    string line = _readBuffer.Substring(0, _readBuffer.IndexOf(NewLine) + 1);   // Only return '\r'
        //    if (_readBuffer.Length > readLength)
        //    {
        //        _readBuffer = _readBuffer.Substring(readLength);
        //    }
        //    else
        //    {
        //        _readBuffer = "";
        //    }

        //    return line;
        //}

        // Returns next line read, returns line read plua \r
        //private string ReadLine()
        //{
        //    while (!_readBuffer.Contains(NewLine))
        //    {
        //        string newText = _serialPort.ReadExisting();
        //        if (newText.Length > 0)
        //        {
        //            _readBuffer += newText;
        //            AddTextToStatusWindowAsync(newText);
        //        }
        //        if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected))
        //        {
        //            break;
        //        }
        //        Thread.Sleep(10);
        //    }
        //    int readLength = _readBuffer.IndexOf(NewLine) + NewLine.Length;
        //    string line = _readBuffer.Substring(0, _readBuffer.IndexOf(NewLine) + 1);   // Only return '\r'
        //    if (_readBuffer.Length > readLength)
        //    {
        //        _readBuffer = _readBuffer.Substring(readLength);
        //    }
        //    else
        //    {
        //        _readBuffer = "";
        //    }

        //    return line;
        //}

        //private string ReadToTimeout(string readTo)
        //{
        //    var tokenSource = new CancellationTokenSource(_serialPort.ReadTimeout);
        //    var token = tokenSource.Token;
        //    Task task = Task.Run(() =>
        //    {
        //       while (!_readBuffer.Contains(readTo) && !token.IsCancellationRequested && _serialPort.IsOpen)
        //       {
        //           string newText = "";
        //           //if (_serialPort.IsOpen)
        //           //{
        //               newText = _serialPort.ReadExisting();
        //           //}
        //           //else
        //           //{
        //           //    tokenSource.Cancel();
        //           //    newText = "";
        //           //    break;
        //           //}
        //           if (newText.Length > 0)
        //           {
        //                _readBuffer += newText;
        //                AddTextToStatusWindowAsync(newText);
        //           }
        //           if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected))
        //           {
        //               break;
        //           }
        //           Thread.Sleep(10);
        //       }
        //    }, token);
        //    try
        //    {
        //        task.Wait();
        //        if (token.IsCancellationRequested || !_serialPort.IsOpen)
        //        {
        //            throw(new Exception("Serial port timeout in ReadTo"));
        //        }
        //    }
        //    catch (AggregateException e)
        //    {
        //        _logHelper.Log(LogLevel.Error, $"Serialport timeout, {e.InnerExceptions.GetType().Name}");
        //        throw;
        //    }
        //    finally
        //    {
        //        tokenSource.Dispose();
        //    }
        //    string readText = "";
        //    if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected))
        //    {
        //        readText = _readBuffer;
        //        _readBuffer = "";
        //        return readText;
        //    }

        //    int readLength = _readBuffer.IndexOf(readTo) + readTo.Length;
        //    readText = _readBuffer.Substring(0, readLength);
        //    if (_readBuffer.Length > readLength)
        //    {
        //        _readBuffer = _readBuffer.Substring(readLength);
        //    }
        //    else
        //    {
        //        _readBuffer = "";
        //    }

        //    return readText;
        //}
        */
        // Returns the string including readTo, as opposed to SerialPort.ReadTo(). Supports timeout.
        private string ReadTo(string readTo)
        {
            Task task = Task.Run(() =>
            {
                while (!_readBuffer.Contains(readTo) && _serialPort.IsOpen)
                {
                    string newText = _serialPort.ReadExisting();
                    if (newText.Length > 0)
                    {
                        _readBuffer += newText;
                        AddTextToStatusWindowAsync(newText);

                        if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected) && _readBuffer.Contains(_TncDevice.Prompts.Timeout))
                        {
                            break;
                        }
                    }
                    Thread.Sleep(10);
                }
            });
            try
            {
                if (!task.Wait(_serialPort.ReadTimeout) || !_serialPort.IsOpen)
                {
                    _readBuffer = "";
                    _logHelper.Log(LogLevel.Error, $"Serial port timeout in ReadTo. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}");
                    throw (new Exception("Serial port timeout in ReadTo"));
                }
            }
            catch (AggregateException e)
            {
                _logHelper.Log(LogLevel.Error, $"Serialport timeout in ReadTo, {e.InnerExceptions.GetType().Name}");
                _error = true;
                //throw;
            }
            string readText = "";
            if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected) && _readBuffer.Contains(_TncDevice.Prompts.Timeout))
            {
                readText = _readBuffer;
                _readBuffer = "";
                //AddTextToStatusWindowAsync(readText);
                return readText;
            }

            int readLength = _readBuffer.IndexOf(readTo) + readTo.Length;
            readText = _readBuffer.Substring(0, readLength);
            if (_readBuffer.Length > readLength)
            {
                _readBuffer = _readBuffer.Substring(readLength);
            }
            else
            {
                _readBuffer = "";
            }

            //AddTextToStatusWindowAsync(readText);
            return readText;
        }

        //private void WriteToBBS(string text)
        //{
        //    _serialPort.Write(text);
        //    string readText = ReadTo(_BBSPromptRN);      // read response eg R 1 plus message
        //    _logHelper.Log(LogLevel.Info, readText);
        //}

        //private void WriteToTNC(string text)
        //{
        //    _serialPort.Write(text + "\r");
        //    string readText = ReadLine();
        //    _logHelper.Log(LogLevel.Info, readText);
        //}

        private void TNCCommand(string text)
        {
            _serialPort.Write(text + "\r");
            string readText = ReadLine();
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            ReadTo(_TNCPrompt);
        }

        private void TNCCommandNoResponse(string text)
        {
            _serialPort.Write(text + "\r");
            string readText = ReadLine();
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readText);

            ReadTo(_TNCPrompt);
        }


        // Returns the string including readTo, as opposed to SerialPort.ReadTo()
        //private string ReadTo(string readTo)
        //{
        //    string readText = "";
        //    while (!_readBuffer.Contains(readTo))
        //    {
        //        string newText = _serialPort.ReadExisting();
        //        _readBuffer += newText;
        //        if (newText.Length > 0)
        //        {
        //            AddTextToStatusWindowAsync(newText);
        //        }
        //        Thread.Sleep(10);
        //        if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected))
        //        {
        //            readText = _readBuffer;
        //            _readBuffer = "";
        //            return readText;
        //        }
        //    }
        //    int readLength = _readBuffer.IndexOf(readTo) + readTo.Length;
        //    readText = _readBuffer.Substring(0, readLength);
        //    if (_readBuffer.Length > readLength)
        //    {
        //        _readBuffer = _readBuffer.Substring(readLength);
        //    }
        //    else
        //    {
        //        _readBuffer = "";
        //    }

        //    return readText;
        //}

        // \n = %5Cn
        //17-Aug 16:32:28: SRV0001: DataArrival: data=POST /TBD HTTP/1.1Host: 127.0.0.1:9334Content-Type: application/x-www-form-urlencodedConnection: closeContent-Length: 1017adn=SCCoPIFO&sub=6DM-751P_R_ICS213_RDF%20Results&urg=FALSE&msg=!SCCoPIFO!%0D%0A%23T%3A%20form-ics213.html%0D%0A%23V%3A%203.11-2.2%0D%0AMsgNo%3A%20%5B6DM-751P%5D%0D%0A1a.%3A%20%5B08%2F15%2F2023%5D%0D%0A1b.%3A%20%5B08%3A12%5D%0D%0A5.%3A%20%5BROUTINE%5D%0D%0A6a.%3A%20%5BNo%5D%0D%0A6b.%3A%20%5BNo%5D%0D%0A7.%3A%20%5BCounty%20ARES%5D%0D%0A8.%3A%20%5BObserver%5D%0D%0A9a.%3A%20%5BEverywhere%5D%0D%0A9b.%3A%20%5BCuesta%20Park%20MTV%5D%0D%0A10.%3A%20%5BRDF%20Results%5D%0D%0A12.%3A%20%5BCongratulations%20to%20Bertrand%20KN6YUY%2C%20Karlis%20KN6GLT%2C%20and%20Sorin%20KN6YUH%20for%20being%20the%20first%20team%20to%20find%20all%20four%20foxes%20at%20this%20year's%20Radio%20Direction%20Finding%20Mini-Drill.%5Cn%5Cn****%20This%20is%20drill%20traffic%20****%5D%0D%0ARec-Sent%3A%20%5Bsender%5D%0D%0AOpCall%3A%20%5BKZ6DM%5D%0D%0AOpName%3A%20%5BPoul%20Hansen%5D%0D%0AMethod%3A%20%5BOther%5D%0D%0AOther%3A%20%5BPacket%5D%0D%0AOpDate%3A%20%5B08%2F17%2F2023%5D%0D%0AOpTime%3A%20%5B16%3A32%5D%0D%0A!%2FADDON!%0D%0A&4VAO=%0D%0A%23EOF

        private void SendMessage(PacketMessage packetMessage)
        {
            int readTimeout = _serialPort.ReadTimeout;
            _serialPort.ReadTimeout = 300000;
            try
            {
                _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                string readText = ReadLine();
                _logHelper.Log(LogLevel.Info, readText);
                _serialPort.Write(packetMessage.Subject + "\r");
                readText = ReadLine();
                _logHelper.Log(LogLevel.Info, readText);
                //_serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");
                _serialPort.Write(packetMessage.MessageBody + "/EX\r");

                readText = ReadTo(_BBSPromptRN);      // read response
                _logHelper.Log(LogLevel.Info, readText);

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
            //_Areas = areas;
            _AreasCommand = areas;

            PacketMessagesReceived = packetMessagesReceived;
            SendMessageReceipts();
        }

        private void SendMessageReceipts()
        {
            if (PacketSettingsViewModel.Instance.SendReceipt)
            {
                // Do not send received receipt for receive receipt messages
                foreach (PacketMessage pktMsg in PacketMessagesReceived)
                {
                    if (pktMsg.Area.Length > 0) // Do not send receipt for bulletins
                        continue;

                    try
                    {
                        // Find the Subject line
                        string[] msgLines = pktMsg.MessageBody.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < Math.Min(msgLines.Length, 8); i++)
                        {
                            if (msgLines[i].StartsWith("Date:"))
                            {
                                pktMsg.JNOSDate = DateTime.Parse(msgLines[i].Substring(10, 21));
                            }
                            else if (msgLines[i].StartsWith("From:"))
                            {
                                pktMsg.MessageFrom = CommunicationsService.NormalizeEmailField(msgLines[i].Substring(6));
                            }
                            else if (msgLines[i].StartsWith("To:"))
                            {
                                pktMsg.MessageTo = CommunicationsService.NormalizeEmailField(msgLines[i].Substring(4));
                            }
                            else if (msgLines[i].StartsWith("Subject:"))
                            {
                                if (msgLines[i].Length > 10)
                                {
                                    pktMsg.Subject = msgLines[i].Substring(9);
                                    if (pktMsg.Subject.Contains("DELIVERED:"))
                                        break;
                                }
                            }
                        }
                        if (pktMsg.Subject.Contains("DELIVERED:"))
                            continue;

                        PacketMessage receiptMessage = new PacketMessage()
                        {
                            PacFormName = "SimpleMessage",
                            PacFormType = "SimpleMessage",
                            CreateTime = DateTime.Now,
                            MessageOrigin = MessageOriginHelper.MessageOrigin.Sent,
                            MessageNumber = Utilities.GetMessageNumberPacket(true),
                            BBSName = _bbsConnectName.Substring(0, _bbsConnectName.IndexOf('-')),
                            TNCName = _TncDevice.Name,
                            MessageTo = pktMsg.MessageFrom,
                            MessageFrom = IdentityViewModel.Instance.UseTacticalCallsign
                                    ? IdentityViewModel.Instance.TacticalCallsign
                                    : IdentityViewModel.Instance.UserCallsign,
                            Subject = $"DELIVERED: {pktMsg.Subject}",
                        };

                        FormField[] formFields = new FormField[2];

                        FormField formField = new FormField
                        {
                            ControlName = "messageBody"
                        };
                        StringBuilder controlContent = new StringBuilder();
                        controlContent.AppendLine($"!LMI!{pktMsg.MessageNumber}!DR!{DateTime.Now.ToString()}");
                        controlContent.AppendLine("Your Message");
                        controlContent.AppendLine($"To: {pktMsg.MessageTo}");
                        controlContent.AppendLine($"Subject: {pktMsg.Subject}");
                        var now = DateTime.Now;
                        //controlContent.AppendLine($"was delivered on {DateTime.Now}"); // 7/10/2017 14:35
                        controlContent.AppendLine($"was delivered on {now.ToString("MM/dd/yyyy HH:mm")}");
                        controlContent.AppendLine($"Recipient's Local Message ID: {pktMsg.MessageNumber}");
                        formField.ControlContent = controlContent.ToString();
                        formFields[0] = formField;
                        formField = new FormField
                        {
                            ControlName = "richTextMessageBody",
                            ControlContent = controlContent.ToString()
                        };
                        formFields[1] = formField;

                        receiptMessage.FormFieldArray = formFields;
                        MessageControl packetForm = new MessageControl();
                        receiptMessage.MessageBody = packetForm.CreateOutpostData(ref receiptMessage);
                        if(!receiptMessage.CreateFileName())
                        {
                            _logHelper.Log(LogLevel.Info, $"Error Creating file name: {receiptMessage.MessageTo}");
                            return;
                        }
                        receiptMessage.SentTime = DateTime.Now;
                        receiptMessage.UpdateMessageSize();
                        //_logHelper.Log(LogLevel.Info, $"Message To: {receiptMessage.MessageTo}");       // Disable if not testing
                        //_logHelper.Log(LogLevel.Info, $"Message Body: { receiptMessage.MessageBody}");  // Disable if not testing
                        SendMessage(receiptMessage);            // Disabled for testing
                    }
                    catch (Exception e)
                    {
                        _logHelper.Log(LogLevel.Error, "Delivered message exception: ", e.Message);
                        _error = true;
                        //throw;
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

            if (BulletinHelpers.BulletinDictionary.ContainsKey(area))
            {
                foreach (string subject in BulletinHelpers.BulletinDictionary[area])
                {
                    //_logHelper.Log(LogLevel.Info, $"Subject: {subject} - Bulletin subject: {bulletinSubject}");
                    if (subject.Trim().Contains(bulletinSubject.Trim()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private void ReceiveMessages(string area)
        {
            if (area.StartsWith('#'))
                return;

            string readText = "";
            _serialPort.ReadTimeout = 300000;
            try
            {
                if (area.Length != 0)
                {
                    //_serialPort.Write($"A {area}\r\x05");        // A XSCPERM
                    if (!area.StartsWith("L>"))
                    {
                        _serialPort.Write($"A {area}\r");        // A XSCPERM
                        _LastAccessedArea = area;
                        readText = ReadTo(_BBSPromptRN);        // read response
                        _logHelper.Log(LogLevel.Info, readText);
                        //}
                        //else
                        //{
                        //    _serialPort.Write($"A {area}\r");        // A XSCPERM
                        //    _LastAccessedArea = area;
                        //}
                        //readText = ReadTo(_BBSPromptRN);        // read response
                        //readText = _serialPort.ReadTo(_BBSPromptRN);        // read response
                        //_logHelper.Log(LogLevel.Info, readText + _BBSPromptRN);
                        //AddTextToStatusWindowAsync(readText + _BBSPrompt + "\n");
                        //_logHelper.Log(LogLevel.Info, readText);


                        if (!_forceReadBulletins && readText.Contains("0 messages"))
                        {
                            _MessageListEmpty = true;
                            return;
                        }
                        if (!_forceReadBulletins && readText.Contains("0 new"))
                        {
                            _MessageListEmpty = true;
                            return;
                        }
                        _MessageListEmpty = false;
                    }
                    //_logHelper.Log(LogLevel.Info, $"Force read bulletin {area}: {_forceReadBulletins.ToString()}");
                    if (area.Contains("XSCPERM") || area.Contains("XSCEVENT") || area.Contains("XSCTEST")) 
                    {
                        _serialPort.Write("LA\r");
                    }
                    else
                    {
                        if (area.StartsWith("L>") && !_MessageListEmpty)
                        {
                            _serialPort.Write($"{area}\r");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    //log.Info($"Timeout = {_serialPort.ReadTimeout}");        // For testing
                    _serialPort.Write("LM\r");
                }
                readText = ReadTo(_BBSPromptRN);      // read response
                _logHelper.Log(LogLevel.Info, readText);

                //ReceivedMessage = readText;     // For testing message content

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
                            readText = ReadTo(_BBSPromptRN);      // read response eg R 1 plus message

                            //string logText = readText.Replace('\n', '\r');
                            //_logHelper.Log(LogLevel.Info, logText);
                            _logHelper.Log(LogLevel.Info, readText);

                            ReceivedMessage = readText;     // For testing message content

                            packetMessage.MessageBody = readText.Substring(0, readText.Length - 3); // Remove beginning of prompt
                            packetMessage.ReceivedTime = DateTime.Now;
                            PacketMessagesReceived.Add(packetMessage);
                            if (area.Length == 0)
                            {
                                _serialPort.Write("K " + msgIndex + "\r");
                                readText = ReadTo(_BBSPromptRN);      // read response
                                _logHelper.Log(LogLevel.Info, readText);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Receive message exception: {e.Message}");
                //_serialPort.DiscardInBuffer();
                //_serialPort.DiscardOutBuffer();
                _error = true;
                throw;

            }
            finally
            {
                _serialPort.ReadTimeout = 300000;
            }
        }

        async void OnSerialPortErrorReceivedAsync(object sender, SerialErrorReceivedEventArgs e)
        {
            _logHelper.Log(LogLevel.Fatal, $"SerialPort Error: {e.EventType}");
            try
            {
                _error = true;
                await SharedCode.Helpers.ContentDialogs.ShowSingleButtonContentDialogAsync($"SerialPort Error: {e.EventType}", "Close", "TNC Connect Error");
                //await Utilities.ShowSingleButtonMessageDialogAsync(sender as CoreDispatcher, $"SerialPort Error: {e.EventType.ToString()}", "Close", "TNC Connect Error");
                _serialPort.Close();
            }
            catch
            { }
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

            bool exitedBeforeConnect = true;

            if (string.IsNullOrEmpty(_bbsConnectName))
                return;     // TODO Send via Email

            Parity parity = _TncDevice.CommPort.Parity;
            ushort dataBits = _TncDevice.CommPort.Databits;
            StopBits stopBits = _TncDevice.CommPort.Stopbits;
            try
            {
                _serialPort = new SerialPort(_TncDevice.CommPort.Comport, _TncDevice.CommPort.Baudrate,
                    parity, dataBits, stopBits)
                {
                    RtsEnable = _TncDevice.CommPort.Flowcontrol == Handshake.RequestToSend ? true : false,
                    Handshake = _TncDevice.CommPort.Flowcontrol,
                    WriteTimeout = 50000,
                    ReadTimeout = 5000,
                    ReadBufferSize = 8192,
                    WriteBufferSize = 4096
                };
            }
            catch (Exception ex)
            {
                _logHelper.Log(LogLevel.Error, $"Exception in comport setup {ex.Message}");
            }
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnSerialPortErrorReceivedAsync);
            _logHelper.Log(LogLevel.Info, "");
            _logHelper.Log(LogLevel.Info, $"{DateTime.Now}");
            _logHelper.Log(LogLevel.Info, $"{_TncDevice.Name}: {_TncDevice.CommPort.Comport}, {_TncDevice.CommPort.Baudrate}, {_TncDevice.CommPort.Databits}, {_TncDevice.CommPort.Stopbits}, {_TncDevice.CommPort.Parity}, {_TncDevice.CommPort.Flowcontrol}");
            string readText = "";
            string readCmdText = "";
            try
            {
                _connectState = ConnectState.None;

                _serialPort.Open();

                _connectState = ConnectState.PrepareTNCType;
                if (_TncDevice.Name == "XSC_Kantronics_KPC3-Plus")
                {
                    readCmdText = KPC3Plus();
                }
                else if (_TncDevice.Name == "XSC_Kenwood_TM-D710A" || _TncDevice.Name == "XSC_Kenwood_TH-D72A")
                {
                    readCmdText = Kenwood();
                    // The TNC prompt has been read at this point
                }
                else
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("Unsupported TNC device");
                    _serialPort.Close();
                    return;
                }
                // Send Precommands
                string preCommands = _TncDevice.InitCommands.Precommands;
                string[] preCommandLines = preCommands.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                _connectState = ConnectState.PreCommand;
                foreach (string commandLine in preCommandLines)
                {
                    if (_error)
                        break;

                    TNCCommand(commandLine);
                }
                // Connect to JNOS
                //int readTimeout = _serialPort.ReadTimeout;
                _serialPort.ReadTimeout = 300000;
                BBSConnectTime = DateTime.Now;

                if (_error)
                {
                    goto AbortWithoutConnect;
                }

                //goto AbortWithoutConnect;    //Test

                _connectState = ConnectState.BBSTryConnect;
                _serialPort.Write($"connect {_bbsConnectName}\r");

                readText = ReadLine();          // Read command
                _logHelper.Log(LogLevel.Info, $"{_TNCPrompt}  {readText}");      // log last Write command

                string readConnectText = ReadTo(_BBSPromptRN);      // read connect response  
                _logHelper.Log(LogLevel.Info, readConnectText);
                if (readConnectText.Contains(_TncDevice.Prompts.Timeout))
                {
                    _logHelper.Log(LogLevel.Error, $"Timeout while connecting to { _bbsConnectName}");
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach", "Close", "BBS Connect Error");
                    goto AbortWithoutConnect;
                }

                exitedBeforeConnect = false;
                _connectState = ConnectState.BBSConnected;

                _serialPort.Write("XM 0\r");

                readCmdText = ReadTo(_BBSPromptRN); // Read to prompt incl command
                _logHelper.Log(LogLevel.Info, readCmdText);

                _logHelper.Log(LogLevel.Info, $"Messages to send: {_packetMessagesToSend.Count}");
                _serialPort.ReadTimeout = 300000;
                // Send messages
                foreach (PacketMessage packetMessage in _packetMessagesToSend)
                {
                    if (_error)
                    {
                        _logHelper.Log(LogLevel.Error, $"Error detected in send messages");
                        break;
                    }

                    //_logHelper.Log(LogLevel.Info, $"BBSConnectName: {_bbsConnectName}, {packetMessage.BBSName}");
                    if (_bbsConnectName.Contains(packetMessage.BBSName))
                    {
                        // Use SendMessage(ref packetMessage) here
                        //SendMessage(packetMessage);
                        try
                        {
                            _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                            readText = ReadLine();
                            _logHelper.Log(LogLevel.Info, readText);
                            _serialPort.Write(packetMessage.Subject + "\r");
                            //_serialPort.Write(packetMessage.Subject + "\n");
                            readText = ReadLine();
                            _logHelper.Log(LogLevel.Info, readText);
                            //_serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");
                            //_serialPort.Write(packetMessage.MessageBody + "\r/EX\r");
                            SentMessage = packetMessage.MessageBody + "\n/EX\r";
                            _serialPort.Write(packetMessage.MessageBody + "\n/EX\r");

                            readText = ReadTo(_BBSPromptRN);      // read response
                            //sentMessage = sentMessage.Replace('\n', '\r');        // To show new line in log file
                            _logHelper.Log(LogLevel.Info, readText);   // Subject + message body plus stuff

                            packetMessage.MessageState = MessageState.Locked;
                            packetMessage.SentTime = DateTime.Now;
                            _packetMessagesSent.Add(packetMessage);
                        }
                        catch (Exception e)
                        {
                            _logHelper.Log(LogLevel.Info, $"Send message exception: {e.Message}");
                            //_serialPort.DiscardInBuffer();
                            //_serialPort.DiscardOutBuffer();
                            _error = true;
                            //throw;
                        }
                    }
                }

                if (!readConnectText.Contains("0 messages") && !_error)
                {
                    ReceiveMessages("");
                }

                //if (!string.IsNullOrEmpty(_Areas) && !_error)
                if (!string.IsNullOrEmpty(_AreasCommand) && !_error)
                {
                    //var areas = _Areas.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var areas = _AreasCommand.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
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

                readText = ReadLine();           // Read command
                _logHelper.Log(LogLevel.Info, readText);

                readText = ReadLine();           // Read disconnect response
                readText = readText.Replace('\0', ' ');
                _logHelper.Log(LogLevel.Info, readText);     // Log disconnect response

                //serialPort.Write(cmd, 0, 1);            // Ctrl-C to return to cmd mode. NOT for Kenwood

                _serialPort.ReadTimeout = 300000;
                readCmdText = ReadTo(_TNCPrompt);      // Next command
AbortWithoutConnect:
                BBSDisconnectTime = DateTime.Now;

                // Send PostCommands
                string postCommands = _TncDevice.InitCommands.Postcommands;
                string[] postCommandLines = postCommands.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                _connectState = ConnectState.PostCommand;
                foreach (string commandLine in postCommandLines)
                {
                    TNCCommand(commandLine);
                }
                if (!exitedBeforeConnect)
                {
                    // Enter converse mode and send FCC call sign
                    _connectState = ConnectState.ConverseMode;
                    _serialPort.Write(_TncDevice.Commands.Conversmode + "\r");
                    readText = ReadLine();       // Read command
                    _logHelper.Log(LogLevel.Info, readCmdText + " " + readText);

                    string fccId = $"de {IdentityViewModel.Instance.UserCallsign}";
                    _serialPort.Write(fccId + "\r");
                    readText = ReadLine();
                    _logHelper.Log(LogLevel.Info, readText);
                    _serialPort.Write("\x03\r");                        // Ctrl-C exits converse mode
                    readCmdText = ReadTo(_TNCPrompt);
                    _logHelper.Log(LogLevel.Info, readCmdText);
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Serial port exception: {e.GetType().ToString()}");
                _logHelper.Log(LogLevel.Error, $"Serial port exception. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}. {e.Message}");
                if (_connectState == ConnectState.BBSConnected)
                {
                    try
                    {
                        // Try to disconnect
                        _serialPort.Write("B\r");                   // Disconnect from BBS (JNOS)

                        readText = ReadLine();           // Read command
                        _logHelper.Log(LogLevel.Info, readText);

                        readText = ReadLine();           // Read disconnect response
                        readText = readText.Replace('\0', ' ');
                        _logHelper.Log(LogLevel.Info, readText);     // Log disconnect response
                    }
                    catch
                    {
                        _logHelper.Log(LogLevel.Error, $"Timeout attempting to disconnect after serial port exception.");
                    }

                    await ContentDialogs.ShowSingleButtonContentDialogAsync("It appears that the radio is tuned to the wrong frequency,\nor the BBS was out of reach", "Close", "BBS Connect Error");
                }
                else if (_connectState == ConnectState.PrepareTNCType)
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("Unable to connect to the TNC.\nIs the TNC on?\nFor Kenwood; is the radio in \"packet12\" mode?", "Close", "BBS Connect Error");
                }
                else if (_connectState == ConnectState.ConverseMode)
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync($"Error sending FCC Identification - {IdentityViewModel.Instance.UserCallsign}.", "Close", "TNC Converse Error");
                }
                //else if (e.Message.Contains("not exist"))
                else if (e.GetType() == typeof(IOException))
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("Looks like the USB or serial cable to the TNC is disconnected", "Close", "TNC Connect Error");
                }
                else if (e.GetType() == typeof(UnauthorizedAccessException))
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync($"The COM Port ({_TncDevice.CommPort.Comport}) is in use by another application. ", "Close", "TNC Connect Error");
                }

                //if (_connectState == ConnectState.BBSConnected)
                //{
                //    try
                //    {
                //        _serialPort.Write("B\r");
                //        string disconnectString = ReadLine();
                //        _logHelper.Log(LogLevel.Trace, disconnectString);
                //        BBSDisconnectTime = DateTime.Now;
                //    }
                //    catch
                //    {
                //        _logHelper.Log(LogLevel.Error, $"Timeout after attempting to disconnect.");
                //    }
                //}
            }
            finally
            {
                _serialPort.Close();
            }
        }

        //#region IDisposable Support
        //private bool disposedValue = false; // To detect redundant calls

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects).
        //            _serialPort.Dispose();
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //        // TODO: set large fields to null.

        //        disposedValue = true;
        //    }
        //}

        //// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //// ~TNCInterface() {
        ////   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        ////   Dispose(false);
        //// }

        //// This code added to correctly implement the disposable pattern.
        //void IDisposable.Dispose()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(true);
        //    // TODO: uncomment the following line if the finalizer is overridden above.
        //    // GC.SuppressFinalize(this);
        //}
        //#endregion

    }
}
