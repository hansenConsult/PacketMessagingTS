using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Views;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.Core;
using SharedCode.Helpers;

namespace PacketMessagingTS.Services.CommunicationsService
{
    public class TNCInterface
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCInterface>();
        private static LogHelper _logHelper = new LogHelper(log);

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
        private List<PacketMessage> _packetMessagesSent = new List<PacketMessage>();
        List<PacketMessage> _packetMessagesToSend;

        string _bbsConnectName = "";
        bool _forceReadBulletins = false;
        string _Areas;
        TNCDevice _TncDevice = null;
        SerialPort _serialPort = null;

        const string _BBSPrompt = ") >";
        const string _BBSPromptC = ") >\r";
        const string _BBSPromptRN = ") >\r\n";
        const string NewLine = "\r\n";
        string _TNCPrompt = "cmd:";

        private bool _error = false;        // Disconnect if an error is detected

        private string _readBuffer = "";

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

        //public bool Cancel
        //{ get { return _error; } set { _error = value; } }

        public void AbortConnection()
        {
            _error = true;
        }

        private List<PacketMessage> _packetMessagesReceived = new List<PacketMessage>();
        public List<PacketMessage> PacketMessagesReceived { get => _packetMessagesReceived; set => _packetMessagesReceived = value; }

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

            _serialPort.Write("D\r");
            readText = ReadLine();

            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            cmdResult = ReadTo(_TNCPrompt);

            _serialPort.Write("b\r");
            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            cmdResult = ReadTo(_TNCPrompt);

            _serialPort.Write("Echo on\r");

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            cmdResult = ReadTo(_TNCPrompt);

            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                _serialPort.Write($"my {Singleton<IdentityViewModel>.Instance.TacticalCallsign}\r");
            }
            else
            {
                _serialPort.Write($"my {Singleton<IdentityViewModel>.Instance.UserCallsign}\r");
            }
            readText = ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

            cmdResult = ReadTo(_TNCPrompt);

            _serialPort.Write("Mon off\r");

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

            cmdResult = ReadTo(_TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write($"daytime {dayTime}\r");
            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, cmdResult + " " + readText);

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
            //_serialPort.Write("D\r");

            //readText = ReadLine();

            //_logHelper.Log(LogLevel.Info, readPrompt + " " + readText);

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readText);

            //readPrompt = ReadTo(_TNCPrompt);

            TNCCommand("b");
            //_serialPort.Write("b\r");
            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readPrompt + " " + readText);

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readText);

            //readPrompt = ReadTo(_TNCPrompt);

            TNCCommand("Echo on");
            //_serialPort.Write("Echo on\r");

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readPrompt + " " + readText);

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readText);

            //readPrompt = ReadTo(_TNCPrompt);

            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                //_serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.TacticalCallsign + "\r");
                TNCCommand("my " + Singleton<IdentityViewModel>.Instance.TacticalCallsign);
            }
            else
            {
                //_serialPort.Write("my " + Singleton<IdentityViewModel>.Instance.UserCallsign + "\r");
                TNCCommand("my " + Singleton<IdentityViewModel>.Instance.UserCallsign);
            }
            //readText = ReadLine();       // Read command
            //_logHelper.Log(LogLevel.Info, readPrompt + " " + readText);

            //readText = ReadLine();
            //_logHelper.Log(LogLevel.Info, readText);

            //readPrompt = ReadTo(_TNCPrompt);

            TNCCommand("Mon off");
            //_serialPort.Write("Mon off\r");

            //readText = ReadLine();       // Read command
            //_logHelper.Log(LogLevel.Info, readPrompt + " " + readText);

            //readText = ReadLine();       // Result for Mon off
            //_logHelper.Log(LogLevel.Info, readText);

            //readPrompt = ReadTo(_TNCPrompt);

            DateTime dateTime = DateTime.Now;
            string dayTime = $"{dateTime.Year - 2000:d2}{dateTime.Month:d2}{dateTime.Day:d2}{dateTime.Hour:d2}{dateTime.Minute:d2}{dateTime.Second:d2}";
            _serialPort.Write("daytime " + dayTime + "\r");

            readText = ReadLine();       // Read command
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            readPrompt = ReadTo(_TNCPrompt);   // Ready for pre commands

            return readPrompt;
        }

        //private async void ShowInStatusWindowAsync(string text)
        //{
        //    await _viewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = text;
        //    });
        //}

        private async void AddTextToStatusWindowAsync(string text)
        {
            if (RxTxStatusPage.rxtxStatusPage.Dispatcher == null)
                return;

            //Debug.Write(text);
            //CoreDispatcher dispatcher = MainPage.Current.Dispatcher;
            //if (!dispatcher.HasThreadAccess)
            //{
            RxTxStatViewModel rxTxStatViewModel = RxTxStatusPage.rxtxStatusPage.RxTxStatusViewmodel;
            await rxTxStatViewModel.ViewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //await RxTxStatusPage.rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                //    //        //MainPage.Current.AddTextToStatusWindow("\nTesting");
                rxTxStatViewModel.AppendRxTxStatus(text);
                //RxTxStatusPage.rxtxStatusPage.TestAddRxTxStatus();
                //    //Singleton<RxTxStatusViewModel>.Instance.StatusPage.AddTextToStatusWindow(text);
            });
            //await RxTxStatusPage.rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //{
            //    RxTxStatusPage.rxtxStatusPage.ScrollText();
            //});

            //}
            //else
            //{
            //    Singleton<RxTxStatusViewModel>.Instance.AddStatusWindowText(text);
            //    //MainPage.Current.AddTextToStatusWindow(text);
            //}
            //Debug.Write(text);
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
            catch (AggregateException e)
            {
                _logHelper.Log(LogLevel.Error, $"Serial port timeout in ReadLine. Connect state: {Enum.Parse(typeof(ConnectState), _connectState.ToString())}");
                _error = true;
                //throw;
            }

            if (_readBuffer.Contains(_TncDevice.Prompts.Disconnected) && _readBuffer.Contains(_TncDevice.Prompts.Timeout))
            {
                string readText = _readBuffer;
                _readBuffer = "";
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

            return readText;
        }

        private void WriteToBBS(string text)
        {
            _serialPort.Write(text);
            string readText = ReadTo(_BBSPromptRN);      // read response eg R 1 plus message
            _logHelper.Log(LogLevel.Info, readText);
        }

        private void WriteToTNC(string text)
        {
            _serialPort.Write(text + "\r");
            string readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);
        }

        private void TNCCommand(string text)
        {
            _serialPort.Write(text + "\r");
            string readText = ReadLine();
            _logHelper.Log(LogLevel.Info, _TNCPrompt + " " + readText);

            readText = ReadLine();
            _logHelper.Log(LogLevel.Info, readText);

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

        private void SendMessage(PacketMessage packetMessage)
        {
            int readTimeout = _serialPort.ReadTimeout;
            _serialPort.ReadTimeout = 240000;
            try
            {
                _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                string readText = ReadLine();
                _logHelper.Log(LogLevel.Info, readText);
                _serialPort.Write(packetMessage.Subject + "\r");
                readText = ReadLine();
                _logHelper.Log(LogLevel.Info, readText);
                _serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");

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
            _Areas = areas;

            PacketMessagesReceived = packetMessagesReceived;
            SendMessageReceipts();
        }

        private void SendMessageReceipts()
        {
            if (Singleton<PacketSettingsViewModel>.Instance.SendReceipt)
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
                        for (int i = 0; i < Math.Min(msgLines.Length, 10); i++)
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
                        if(!receiptMessage.CreateFileName())
                        {
                            throw new Exception();
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
            string readText;
            _serialPort.ReadTimeout = 240000;
            try
            {
                if (area.Length != 0)
                {
                    //_serialPort.Write($"A {area}\r\x05");        // A XSCPERM
                    _serialPort.Write($"A {area}\r");        // A XSCPERM
                    //readText = _serialPort.ReadTo(_BBSPromptRN);        // read response
                    //_logHelper.Log(LogLevel.Info, readText + _BBSPrompt);
                    //AddTextToStatusWindowAsync(readText + _BBSPrompt + "\n");
                    readText = ReadTo(_BBSPromptRN);        // read response
                    _logHelper.Log(LogLevel.Info, readText);


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
                            readText = ReadTo(_BBSPromptRN);      // read response eg R 1 plus message
                            _logHelper.Log(LogLevel.Info, readText);

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
                _serialPort.ReadTimeout = 240000;
            }
        }

        async void OnSerialPortErrorReceivedAsync(object sender, SerialErrorReceivedEventArgs e)
        {
            _logHelper.Log(LogLevel.Fatal, $"SerialPort Error: {e.EventType.ToString()}");
            try
            {
                _error = true;
                await Utilities.ShowSingleButtonMessageDialogAsync(sender as CoreDispatcher, $"SerialPort Error: {e.EventType.ToString()}", "Close", "TNC Connect Error");
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
            _logHelper.Log(LogLevel.Info, $"{DateTime.Now.ToString()}");
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

                    //_serialPort.Write(commandLine + "\r");

                    //readText = ReadLine();       // Read command
                    //_logHelper.Log(LogLevel.Info, readCmdText + " " + readText);

                    //readText = ReadLine();       // Result for command
                    //_logHelper.Log(LogLevel.Info, readText);

                    //readCmdText = ReadTo(_TNCPrompt);	// Next command
                }
                // Connect to JNOS
                int readTimeout = _serialPort.ReadTimeout;
                //_serialPort.ReadTimeout = 5000;
                _serialPort.ReadTimeout = 240000;
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

                _serialPort.ReadTimeout = readTimeout;

                _serialPort.Write("XM 0\r");

                readCmdText = ReadTo(_BBSPromptRN); // Read to prompt incl command
                _logHelper.Log(LogLevel.Info, readCmdText);

                _logHelper.Log(LogLevel.Info, $"Messages to send: {_packetMessagesToSend.Count}");
                _serialPort.ReadTimeout = 240000;
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
                        // Use SendMessage(ref packetMessage) here
                        //SendMessage(packetMessage);
                        try
                        {
                            _serialPort.Write("SP " + packetMessage.MessageTo + "\r");
                            readText = ReadLine();
                            _logHelper.Log(LogLevel.Info, readText);
                            _serialPort.Write(packetMessage.Subject + "\r");
                            readText = ReadLine();
                            _logHelper.Log(LogLevel.Info, readText);
                            _serialPort.Write(packetMessage.MessageBody + "\r\x1a\r");

                            readText = ReadTo(_BBSPromptRN);      // read response
                            _logHelper.Log(LogLevel.Info, readText);   // Subject + message body plus stuff

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

                readText = ReadLine();           // Read command
                _logHelper.Log(LogLevel.Info, readText);

                readText = ReadLine();           // Read disconnect response
                readText = readText.Replace('\0', ' ');
                _logHelper.Log(LogLevel.Info, readText);     // Log disconnect response

                //serialPort.Write(cmd, 0, 1);            // Ctrl-C to return to cmd mode. NOT for Kenwood

                _serialPort.ReadTimeout = 5000;
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
                    //_serialPort.Write(commandLine + "\r");

                    //readText = ReadLine();				// Read command
                    //_logHelper.Log(LogLevel.Info, readCmdText + " " + readText);

                    //readText = ReadLine();              // Command result
                    //_logHelper.Log(LogLevel.Info, readText);

                    //readCmdText = ReadTo(_TNCPrompt);	// Next command
                }
                if (!exitedBeforeConnect)
                {
                    // Enter converse mode and send FCC call sign
                    _connectState = ConnectState.ConverseMode;
                    _serialPort.Write(_TncDevice.Commands.Conversmode + "\r");
                    readText = ReadLine();       // Read command
                    _logHelper.Log(LogLevel.Info, readCmdText + " " + readText);

                    string fccId = $"de {Singleton<IdentityViewModel>.Instance.UserCallsign}";
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
                    await ContentDialogs.ShowSingleButtonContentDialogAsync($"Error sending FCC Identification - {Singleton<IdentityViewModel>.Instance.UserCallsign}.", "Close", "TNC Converse Error");
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
