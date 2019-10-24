using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using MetroLog;
using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using SharedCode;
using Windows.UI.Xaml;


namespace PacketMessagingTS.ViewModels
{
    public class TNCSettingsViewModel : BaseViewModel
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCSettingsViewModel>();
        private static LogHelper _logHelper = new LogHelper(log);

        public enum TNCState
        {
            None,
            TNC,
            TNCAdd,
            TNCDelete,
            TNCEdit,
            EMail,
            EMailAdd,
            EMailDelete,
            EMailEdit,
        };


        public TNCSettingsViewModel()
        {

        }

        public TNCState State { get; set; }

        private Visibility pivotTNCVisibility;
        public Visibility PivotTNCVisibility
        {
            get => pivotTNCVisibility;
            set => Set(ref pivotTNCVisibility, value);
        }

        private int pivotTNCSelectedIndex;
        public int PivotTNCSelectedIndex
        {
            get => GetProperty(ref pivotTNCSelectedIndex);
            set => SetProperty(ref pivotTNCSelectedIndex, value, true);
        }

        private int tncDeviceSelectedIndex;
        public int TNCDeviceSelectedIndex
        {
            get => GetProperty(ref tncDeviceSelectedIndex);
            set
            {
                bool setPropertySuccess = false;
                if (value < 0 || value >= TNCDeviceArray.Instance.TNCDeviceList.Count)
                {
                    setPropertySuccess = SetProperty(ref tncDeviceSelectedIndex, 0, true);
                }
                else
                {
                    setPropertySuccess = SetProperty(ref tncDeviceSelectedIndex, value, true);
                }
                if (setPropertySuccess)
                {
                    Utilities.SetApplicationTitle();
                }
                CurrentTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[tncDeviceSelectedIndex];
                if (CurrentTNCDevice.Name.Contains(SharedData.EMail))
                {
                    //State = TNCState.EMail;
                    //UpdateMailState(TNCState.EMail);                     
                    MailAccountSelectedIndex = MailAccountSelectedIndex;

                    EMailSettingsVisibility = Visibility.Visible;
                    PivotTNCVisibility = Visibility.Collapsed;
                }
                else
                {
                    State = TNCState.None;
                    EMailSettingsVisibility = Visibility.Collapsed;
                    PivotTNCVisibility = Visibility.Visible;
                }

            }
        }

        private ObservableCollection<TNCDevice> _TNCDeviceListSource;
        public ObservableCollection<TNCDevice> TNCDeviceListSource
        {
            get
            {
                return new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
            }
            set
            { 
                //_TNCDeviceListSource.ClearItems();
                Set(ref _TNCDeviceListSource, value);
            }
        }


        private string _NewTNCDeviceName = "new Device Name";
        public string NewTNCDeviceName
        {
            get => _NewTNCDeviceName;
            set => Set(ref _NewTNCDeviceName, value);
        }

        public TNCDevice TNCDeviceFromUI
        {
            get
            {
                TNCDevice tncDeviceFromUI = new TNCDevice();

                tncDeviceFromUI.InitCommands.Precommands = TNCInitCommandsPre;
                tncDeviceFromUI.InitCommands.Postcommands = TNCInitCommandsPost;
                tncDeviceFromUI.CommPort.IsBluetooth = IsToggleSwitchOn;
                tncDeviceFromUI.CommPort.Comport = TNCComPort;
                tncDeviceFromUI.CommPort.BluetoothName = TNCComName;
                tncDeviceFromUI.CommPort.Baudrate = TNCComBaudRate;
                tncDeviceFromUI.CommPort.Databits = TNCComDatabits;
                tncDeviceFromUI.CommPort.Stopbits = TNCComStopbits;
                tncDeviceFromUI.CommPort.Parity = TNCComParity;
                tncDeviceFromUI.CommPort.Flowcontrol = TNCComHandshake;
                tncDeviceFromUI.Commands.MyCall = TNCCommandsMyCall;
                tncDeviceFromUI.Commands.Connect = TNCCommandsConnect;
                tncDeviceFromUI.Commands.Retry = TNCCommandsRetry;
                tncDeviceFromUI.Commands.Conversmode = TNCCommandsConversMode;
                tncDeviceFromUI.Commands.Datetime = TNCCommandsDateTime;
                tncDeviceFromUI.Prompts.Command = TNCPromptsCommand;
                tncDeviceFromUI.Prompts.Timeout = TNCPromptsTimeout;
                tncDeviceFromUI.Prompts.Connected = TNCPromptsConnected;
                tncDeviceFromUI.Prompts.Disconnected = TNCPromptsDisconnected;

                if (State == TNCState.TNCAdd)
                {
                    tncDeviceFromUI.Name = NewTNCDeviceName;
                }
                else
                {
                    tncDeviceFromUI.Name = CurrentTNCDevice.Name;
                }
                return tncDeviceFromUI;
            }
        }

        private TNCDevice currentTNCDevice;
        public TNCDevice CurrentTNCDevice
        {
            get
            {
                if (currentTNCDevice is null)
                {
                    TNCDeviceSelectedIndex = Utilities.GetProperty("TNCDeviceSelectedIndex");
                }
                //if (currentTNCDevice.Name.Contains(SharedData.EMail))
                //{ }
                //else
                //{
                 //   currentTNCDevice.CommPort.Comport = TNCComPort;
                //}
                
                return currentTNCDevice;
            }
            set
            {
                currentTNCDevice = value;

                if (!string.IsNullOrEmpty(currentTNCDevice.Name) && currentTNCDevice.Name.Contains(SharedData.EMail))
                {
                    // Update email account index
                    string mailPreample = SharedData.EMail + "-";
                    string mailUserName;
                    int index = currentTNCDevice.Name.IndexOf(mailPreample);
                    if (index == 0)
                    {
                        mailUserName = currentTNCDevice.Name.Substring(mailPreample.Length);
                        int i = 0;
                        for (; i < EmailAccountArray.Instance.EmailAccounts.Length; i++)
                        {
                            if (mailUserName.Contains(EmailAccountArray.Instance.EmailAccounts[i].MailUserName))
                            {
                                break;
                            }
                        }
                        if (i >= EmailAccountArray.Instance.EmailAccounts.Length)
                            MailAccountSelectedIndex = 0;
                        else
                            MailAccountSelectedIndex = i;
                    }
                }
                else
                {
                    //_logHelper.Log(LogLevel.Trace, $"Current device, Comport: {currentTNCDevice.CommPort.Comport}");

                    TNCInitCommandsPre = currentTNCDevice.InitCommands.Precommands;
                    TNCInitCommandsPost = currentTNCDevice.InitCommands.Postcommands;
                    IsToggleSwitchOn = currentTNCDevice.CommPort.IsBluetooth;
                    TNCComPort = currentTNCDevice.CommPort.Comport;
                    TNCComName = currentTNCDevice.CommPort.BluetoothName;
                    TNCComBaudRate = currentTNCDevice.CommPort.Baudrate;
                    TNCComDatabits = currentTNCDevice.CommPort.Databits;
                    TNCComStopbits = currentTNCDevice.CommPort.Stopbits;
                    TNCComParity = currentTNCDevice.CommPort.Parity;
                    TNCComHandshake = currentTNCDevice.CommPort.Flowcontrol;
                    TNCCommandsMyCall = currentTNCDevice.Commands.MyCall;
                    TNCCommandsConnect = currentTNCDevice.Commands.Connect;
                    TNCCommandsRetry = currentTNCDevice.Commands.Retry;
                    TNCCommandsConversMode = currentTNCDevice.Commands.Conversmode;
                    TNCCommandsDateTime = currentTNCDevice.Commands.Datetime;
                    TNCPromptsCommand = currentTNCDevice.Prompts.Command;
                    TNCPromptsTimeout = currentTNCDevice.Prompts.Timeout;
                    TNCPromptsConnected = currentTNCDevice.Prompts.Connected;
                    TNCPromptsDisconnected = currentTNCDevice.Prompts.Disconnected;
                }
                ResetChangedProperty();
            }
        }

        private string tncInitCommandsPre;
        public string TNCInitCommandsPre
        {
            get => tncInitCommandsPre;
            set
            {
                SetProperty(ref tncInitCommandsPre, value);

                bool changed = CurrentTNCDevice.InitCommands.Precommands != tncInitCommandsPre;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncInitCommandsPost;
        public string TNCInitCommandsPost
        {
            get => tncInitCommandsPost;
            set
            {
                SetProperty(ref tncInitCommandsPost, value);

                bool changed = CurrentTNCDevice.InitCommands.Precommands != tncInitCommandsPre;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private bool isToggleSwitchOn;
        public bool IsToggleSwitchOn
        {
            get => isToggleSwitchOn;
            set
            {
                SetProperty(ref isToggleSwitchOn, value);

                if (isToggleSwitchOn)
                {
                    TNCComPortVisible = Windows.UI.Xaml.Visibility.Collapsed;
                    TNCComNameVisible = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    TNCComPortVisible = Windows.UI.Xaml.Visibility.Visible;
                    TNCComNameVisible = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        //private List<string> comPortNames;
        //public List<string> ComPortNames
        //{
        //    get
        //    {

        //        comPortNames = comportStringArray.ToList();
        //        comPortNames = comPortNames.OrderBy(s => s, new ComportComparer()).ToList();
        //        return comPortNames;
        //    }
        //}

        //private ObservableCollection<string> collectionOfSerialDevices;
        public ObservableCollection<string> CollectionOfSerialDevices
        {
            get => Singleton<ShellViewModel>.Instance.CollectionOfSerialDevices;
            //get => new ObservableCollection<string>(SerialPort.GetPortNames());
        }

        private string tncComPort;
        public string TNCComPort
        {
            get => tncComPort;
            set
            {
                //_logHelper.Log(LogLevel.Trace, $"Comport: {value}");

                if (value is null || CollectionOfSerialDevices.Count == 0)
                    return;

                SetProperty(ref tncComPort, value);                

                bool changed = CurrentTNCDevice.CommPort.Comport != tncComPort;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Visibility tncComPortVisible;
        public Visibility TNCComPortVisible
        {
            get => tncComPortVisible;
            set
            {
                SetProperty(ref tncComPortVisible, value);
            }
        }

        public IEnumerable<Parity> SerialParityValues
        {
            get
            {
                return Enum.GetValues(typeof(Parity))
                    .Cast<Parity>();
            }
        }

        public IEnumerable<StopBits> SerialStopBitCountValues
        {
            get
            {
                return Enum.GetValues(typeof(StopBits))
                    .Cast<StopBits>();
            }
        }

        public IEnumerable<Handshake> MyEnumTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(Handshake))
                    .Cast<Handshake>();
            }
        }
        private string tncComName;
        public string TNCComName
        {
            get => tncComName;
            set
            {
                SetProperty(ref tncComName, value);

                bool changed = CurrentTNCDevice.CommPort.BluetoothName != tncComName;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Visibility tncComNameVisible;
        public Visibility TNCComNameVisible
        {
            get => tncComNameVisible;
            set
            {
                SetProperty(ref tncComNameVisible, value);
            }
        }

        private ObservableCollection<ushort> CreateBaudRatesCollection()
        {
            ObservableCollection<ushort> baudRatesCollection = new ObservableCollection<ushort>();
            for (ushort i = 1200; i< 39000; i *= 2)
            {
                baudRatesCollection.Add(i);
            }
            return baudRatesCollection;
        }

        public ObservableCollection<ushort> BaudRatesCollection
        {
            get => CreateBaudRatesCollection();
        }

        private ushort tncComBaudRate;
        public ushort TNCComBaudRate
        {
            get => tncComBaudRate;
            set
            {
                SetProperty(ref tncComBaudRate, value);

                bool changed = CurrentTNCDevice.CommPort.Baudrate != tncComBaudRate;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        public ObservableCollection<ushort> DatabitsCollection
        {
            get => new ObservableCollection<ushort>() { 7, 8 };
        }

        private ushort tncComDatabits;
        public ushort TNCComDatabits
        {
            get => tncComDatabits;
            set
            {
                SetProperty(ref tncComDatabits, value);

                bool changed = CurrentTNCDevice.CommPort.Databits != tncComDatabits;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private StopBits tncComStopbits;
        public StopBits TNCComStopbits
        {
            get => tncComStopbits;
            set
            {
                SetProperty(ref tncComStopbits, value);

                bool changed = CurrentTNCDevice?.CommPort.Stopbits != tncComStopbits;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Parity tncComParity;
        public Parity TNCComParity
        {
            get => tncComParity;
            set
            {
                SetProperty(ref tncComParity, value);

                bool changed = CurrentTNCDevice?.CommPort.Parity != tncComParity;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private Handshake tncComHandshake;
        public Handshake TNCComHandshake
        {
            get { return tncComHandshake; }
            set
            {
                SetProperty(ref tncComHandshake, value);

                bool changed = CurrentTNCDevice?.CommPort.Flowcontrol != tncComHandshake;
                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }

        private string tncCommandsMyCall;
        public string TNCCommandsMyCall
        {
            get => tncCommandsMyCall;
            set
            {
                SetProperty(ref tncCommandsMyCall, value);

                bool changed = CurrentTNCDevice.Commands.MyCall != tncCommandsMyCall;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsConnect;
        public string TNCCommandsConnect
        {
            get => tncCommandsConnect;
            set
            {
                SetProperty(ref tncCommandsConnect, value);

                bool changed = CurrentTNCDevice.Commands.Connect != value;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsRetry;
        public string TNCCommandsRetry
        {
            get => tncCommandsRetry;
            set
            {
                SetProperty(ref tncCommandsRetry, value);

                bool changed = CurrentTNCDevice.Commands.Retry != tncCommandsRetry;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsConversMode;
        public string TNCCommandsConversMode
        {
            get => tncCommandsConversMode;
            set
            {
                SetProperty(ref tncCommandsConversMode, value);

                bool changed = CurrentTNCDevice.Commands.Conversmode != tncCommandsConversMode;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncCommandsDateTime;
        public string TNCCommandsDateTime
        {
            get => tncCommandsDateTime;
            set
            {
                SetProperty(ref tncCommandsDateTime, value);

                bool changed = CurrentTNCDevice.Commands.Datetime != tncCommandsDateTime;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsCommand;
        public string TNCPromptsCommand
        {
            get => GetProperty(ref tncPromptsCommand);
            set
            {
                SetProperty(ref tncPromptsCommand, value);

                bool changed = CurrentTNCDevice.Prompts.Command != tncPromptsCommand;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsTimeout;
        public string TNCPromptsTimeout
        {
            get => GetProperty(ref tncPromptsTimeout);
            set
            {
                SetProperty(ref tncPromptsTimeout, value);

                bool changed = CurrentTNCDevice.Prompts.Timeout != tncPromptsTimeout;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsConnected;
        public string TNCPromptsConnected
        {
            get => GetProperty(ref tncPromptsConnected);
            set
            {
                SetProperty(ref tncPromptsConnected, value);

                bool changed = CurrentTNCDevice.Prompts.Connected != tncPromptsConnected;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private string tncPromptsDisconnected;
        public string TNCPromptsDisconnected
        {
            get => GetProperty(ref tncPromptsDisconnected);
            set
            {
                SetProperty(ref tncPromptsDisconnected, value);

                bool changed = CurrentTNCDevice.Prompts.Disconnected != tncPromptsDisconnected;
                IsAppBarSaveEnabled = SaveEnabled(changed);

            }
        }
        #region Mail Settings
        private Visibility eMailSettingsVisibility;
        public Visibility EMailSettingsVisibility
        {
            get => eMailSettingsVisibility;
            set => Set(ref eMailSettingsVisibility, value);
        }

        private int mailAccountSelectedIndex;
        public int MailAccountSelectedIndex
        {
            get
            {
                GetProperty(ref mailAccountSelectedIndex);

                //if (mailAccountSelectedIndex >= 0)
                //{
                //    CurrentMailAccount = EmailAccountArray.Instance.EmailAccounts[mailAccountSelectedIndex];
                //}
                return mailAccountSelectedIndex;
            }
            set
            {
                bool setPropertySuccess = false;
                if (value < 0 || value >= EmailAccountArray.Instance.EmailAccountList.Count)
                {
                    setPropertySuccess = SetProperty(ref mailAccountSelectedIndex, 0, true);
                }
                else
                {
                    setPropertySuccess = SetProperty(ref mailAccountSelectedIndex, value, true);
                }
                if (setPropertySuccess)
                {
                    Utilities.SetApplicationTitle();
                }

                EmailAccount mailAccount = EmailAccountArray.Instance.EmailAccountList[mailAccountSelectedIndex];
                MailServer = mailAccount.MailServer;
                MailPort = mailAccount.MailServerPort;
                IsMailSSL = mailAccount.MailIsSSLField;
                MailUserName = mailAccount.MailUserName;
                MailPassword = mailAccount.MailPassword;

                CurrentMailAccount = mailAccount;
            }
        }

        private ObservableCollection<EmailAccount> _MailAccountListSource;
        public ObservableCollection<EmailAccount> MailAccountListSource
        {
            get
            {
                return new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
            }
            set
            {
                Set(ref _MailAccountListSource, value);
            }
        }

        private EmailAccount _SelectedMailAccount;
        public EmailAccount SelectedMailAccount
        {
            get => _SelectedMailAccount;
            set => Set(ref _SelectedMailAccount, value);
        }

        private EmailAccount currentMailAccount;
        public EmailAccount CurrentMailAccount
        {
            get => GetProperty(ref currentMailAccount);
            //get => currentMailAccount;
            set
            {
                //currentMailAccount = value;

                //MailServer = currentMailAccount.MailServer;
                //MailPort = currentMailAccount.MailServerPort;
                //IsMailSSL = currentMailAccount.MailIsSSLField;
                //MailUserName = currentMailAccount.MailUserName;
                //MailPassword = currentMailAccount.MailPassword;

                SetProperty(ref currentMailAccount, value);

            }
        }

        private bool isMailServerEnabled;
        public bool IsMailServerEnabled
        {
            get => isMailServerEnabled;
            set => Set(ref isMailServerEnabled, value);
        }

        private string mailServer;
        public string MailServer
        {
            get => GetProperty(ref mailServer);
            set
            {
                SetProperty(ref mailServer, value);
                //foreach (EmailAccount account in EmailAccountArray.Instance.EmailAccounts)
                //{
                //	if (account.MailServer == MailServer)
                //	{
                //		MailPortString = account.MailServerPort.ToString();
                //		IsMailSSL = account.MailIsSSLField;
                //		MailUserName = account.MailUserName;
                //		MailPassword = account.MailPassword;
                //		break;
                //	}
                //}
                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailServer != mailServer;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.Server = MailServer;
            }
        }

        private bool isMailServerPortEnabled;
        public bool IsMailServerPortEnabled
        {
            get => isMailServerPortEnabled;
            set => Set(ref isMailServerPortEnabled, value);
        }

        private Int32 mailPort;
        public Int32 MailPort
        {
            get => mailPort;
            set
            {
                SetProperty(ref mailPort, value);
                //if (MailPortString != MailPort.ToString())
                //    MailPortString = MailPort.ToString();

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailServerPort != mailPort;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.Port = MailPort;
            }
        }

        private Visibility isMailServerSSLVisible;
        public Visibility IsMailServerSSLVisible
        {
            get => isMailServerSSLVisible;
            set => Set(ref isMailServerSSLVisible, value);
        }

        bool isMailSSL;
        public bool IsMailSSL
        {
            get => isMailSSL;
            set
            {
                SetProperty(ref isMailSSL, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailIsSSLField != isMailSSL;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.IsSsl = IsMailSSL;
            }
        }

        private bool isMailUserNameEnabled;
        public bool IsMailUserNameEnabled
        {
            get => isMailUserNameEnabled;
            set => Set(ref isMailUserNameEnabled, value);
        }

        private string mailUserName;
        public string MailUserName
        {
            get => mailUserName;
            set
            {
                SetProperty(ref mailUserName, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailUserName != mailUserName;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.UserName = MailUserName;
            }
        }

        private bool isMailPasswordEnabled;
        public bool IsMailPasswordEnabled
        {
            get => isMailPasswordEnabled;
            set => Set(ref isMailPasswordEnabled, value);
        }

        private string mailPassword;
        public string MailPassword
        {
            get => mailPassword;
            set
            {
                SetProperty(ref mailPassword, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailPassword != mailPassword;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.Password = MailPassword;
            }
        }
        #endregion Mail Settings
        private bool isAppBarAddEnabled = true;
        public bool IsAppBarAddEnabled
        {
            get => isAppBarAddEnabled;
            set => SetProperty(ref isAppBarAddEnabled, value);
        }

        private bool isAppBarDeleteEnabled = true;
        public bool IsAppBarDeleteEnabled
        {
            get => isAppBarDeleteEnabled;
            set => SetProperty(ref isAppBarDeleteEnabled, value);
        }

        private bool isAppBarEditEnabled = true;
        public bool IsAppBarEditEnabled
        {
            get => isAppBarEditEnabled;
            set => SetProperty(ref isAppBarEditEnabled, value);
        }
    }
}
