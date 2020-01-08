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
            TNC,
            TNCAdd,
            TNCDelete,
            TNCEdit,
            EMail,
            EMailAdd,
            EMailDelete,
            EMailEdit,
        };

        public int _deletedIndex;
        public int _modifiedEmailAccountSelectedIndex;

        TNCDevice _SavedTNCDevice;

        private static object _syncRoot = new Object();


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

        private Visibility deviceListBoxVisibility = Visibility.Visible;
        public Visibility DeviceListBoxVisibility
        {
            get => deviceListBoxVisibility;
            set => Set(ref deviceListBoxVisibility, value);
        }

        private Visibility newTNCDeviceNameVisibility = Visibility.Collapsed;
        public Visibility NewTNCDeviceNameVisibility
        {
            get => newTNCDeviceNameVisibility;
            set => Set(ref newTNCDeviceNameVisibility, value);
        }

        private int tncDeviceSelectedIndex;
        public int TNCDeviceSelectedIndex
        {
            get => GetProperty(ref tncDeviceSelectedIndex);
            set
            {
                //_logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index: {value}, {tncDeviceSelectedIndex}");
                if (value >= 0 && value < TNCDeviceArray.Instance.TNCDeviceList.Count)
                {
                    _SavedTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[value];
                } else if (value >= TNCDeviceArray.Instance.TNCDeviceList.Count)
                {
                    _SavedTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[0];
                }

                if (value == tncDeviceSelectedIndex && !(currentTNCDevice is null))
                    return;

                SaveChanges(tncDeviceSelectedIndex, State);

                //_logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index after SaveChanges(): {value}, {tncDeviceSelectedIndex}");

                bool setPropertySuccess = false;
                if (value < 0)
                {
                    SetProperty(ref tncDeviceSelectedIndex, value);
                    _logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index after -1: {value}, {tncDeviceSelectedIndex}");
                    return;
                }
                else if (value >= TNCDeviceArray.Instance.TNCDeviceList.Count)
                {
                    setPropertySuccess = SetProperty(ref tncDeviceSelectedIndex, 0, true);
                }
                else
                {
                    setPropertySuccess = SetProperty(ref tncDeviceSelectedIndex, value, true);
                }
                if (setPropertySuccess)
                {
                    // Utilities.SetApplicationTitle();
                }
                CurrentTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[tncDeviceSelectedIndex];
                if (CurrentTNCDevice.Name.Contains(SharedData.EMail))
                {
                    UpdateMailState(TNCState.EMail);
                    MailAccountSelectedIndex = MailAccountSelectedIndex;
                    EMailSettingsVisibility = Visibility.Visible;
                    PivotTNCVisibility = Visibility.Collapsed;
                    IsAppBarEditEnabled = true;
                }
                else
                {
                    State = TNCState.TNC;
                    EMailSettingsVisibility = Visibility.Collapsed;
                    PivotTNCVisibility = Visibility.Visible;
                    IsAppBarEditEnabled = false;
                }
                //_logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index after: {value}, {tncDeviceSelectedIndex}");
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
                    TNCDeviceSelectedIndex = Utilities.GetProperty(nameof(TNCDeviceSelectedIndex));
                }
                
                return currentTNCDevice;
            }
            set
            {
                ResetChangedProperty();

                currentTNCDevice = value;

                if (!string.IsNullOrEmpty(currentTNCDevice.Name) && currentTNCDevice.Name.Contains(SharedData.EMail))
                {
                    //    // Update email account index
                    //    string mailPreample = SharedData.EMailPreample;
                    //    string mailUserName;
                    //    int index = currentTNCDevice.Name.IndexOf(mailPreample);
                    //    if (index == 0)
                    //    {
                    //        mailUserName = currentTNCDevice.Name.Substring(mailPreample.Length);
                    //        int i = 0;
                    //        for (; i < EmailAccountArray.Instance.EmailAccounts.Length; i++)
                    //        {
                    //            if (mailUserName.Contains(EmailAccountArray.Instance.EmailAccounts[i].MailUserName))
                    //            {
                    //                break;
                    //            }
                    //        }
                    //        if (i >= EmailAccountArray.Instance.EmailAccounts.Length)
                    //            MailAccountSelectedIndex = 0;
                    //        else
                    //            MailAccountSelectedIndex = i;
                    //    }
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
                //ResetChangedProperty();
            }
        }

        private string tncInitCommandsPre;
        public string TNCInitCommandsPre
        {
            get => tncInitCommandsPre;
            set
            {
                SetProperty(ref tncInitCommandsPre, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.InitCommands.Precommands, tncInitCommandsPre);
            }
        }

        private string tncInitCommandsPost;
        public string TNCInitCommandsPost
        {
            get => tncInitCommandsPost;
            set
            {
                SetProperty(ref tncInitCommandsPost, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.InitCommands.Postcommands, tncInitCommandsPost);
            }
        }

        private void UpdateTNCStateAndButtons<T>(T savedProperty, T newProperty)
        {
            bool changed = !Equals(savedProperty, newProperty);
            IsAppBarSaveEnabled = SaveEnabled(changed);
            if (State == TNCState.TNC && IsAppBarSaveEnabled)
            {
                State = TNCState.TNCEdit;
            }
        }

        //private void UpdateTNCStateAndButtons(string savedProperty, string newProperty)
        //{
        //    bool changed = !Equals(savedProperty, newProperty);
        //    IsAppBarSaveEnabled = SaveEnabled(changed);
        //    if (State == TNCState.TNC && IsAppBarSaveEnabled)
        //    {
        //        State = TNCState.TNCEdit;
        //    }
        //}

        //private void UpdateTNCStateAndButtons(bool changed)
        //{
        //    IsAppBarSaveEnabled = SaveEnabled(changed);
        //    if (State == TNCState.TNC && IsAppBarSaveEnabled)
        //    {
        //        State = TNCState.TNCEdit;
        //    }
        //}

        private bool isToggleSwitchOn;
        public bool IsToggleSwitchOn
        {
            get => isToggleSwitchOn;
            set
            {
                SetProperty(ref isToggleSwitchOn, value);

                if (isToggleSwitchOn)
                {
                    TNCComPortVisible = Visibility.Collapsed;
                    TNCComNameVisible = Visibility.Visible;
                }
                else
                {
                    TNCComPortVisible = Visibility.Visible;
                    TNCComNameVisible = Visibility.Collapsed;
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

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Comport, tncComPort);
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

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.BluetoothName, tncComName);
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

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Baudrate, tncComBaudRate);
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

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Databits, tncComDatabits);
            }
        }

        private StopBits tncComStopbits;
        public StopBits TNCComStopbits
        {
            get => tncComStopbits;
            set
            {
                SetProperty(ref tncComStopbits, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Stopbits, tncComStopbits);
            }
        }

        private Parity tncComParity;
        public Parity TNCComParity
        {
            get => tncComParity;
            set
            {
                SetProperty(ref tncComParity, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Parity, tncComParity);
            }
        }

        private Handshake tncComHandshake;
        public Handshake TNCComHandshake
        {
            get { return tncComHandshake; }
            set
            {
                SetProperty(ref tncComHandshake, value);

                if (TNCDeviceSelectedIndex < 0)
                    return;

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Flowcontrol, tncComHandshake);

            }
        }

        private string tncCommandsMyCall;
        public string TNCCommandsMyCall
        {
            get => tncCommandsMyCall;
            set
            {
                SetProperty(ref tncCommandsMyCall, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Commands.MyCall, tncCommandsMyCall);
            }
        }

        private string tncCommandsConnect;
        public string TNCCommandsConnect
        {
            get => tncCommandsConnect;
            set
            {
                SetProperty(ref tncCommandsConnect, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Commands.Connect, tncCommandsConnect);
            }
        }

        private string tncCommandsRetry;
        public string TNCCommandsRetry
        {
            get => tncCommandsRetry;
            set
            {
                SetProperty(ref tncCommandsRetry, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Commands.Retry, tncCommandsRetry);
            }
        }

        private string tncCommandsConversMode;
        public string TNCCommandsConversMode
        {
            get => tncCommandsConversMode;
            set
            {
                SetProperty(ref tncCommandsConversMode, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Commands.Conversmode, tncCommandsConversMode);
            }
        }

        private string tncCommandsDateTime;
        public string TNCCommandsDateTime
        {
            get => tncCommandsDateTime;
            set
            {
                SetProperty(ref tncCommandsDateTime, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Commands.Datetime, tncCommandsDateTime);
            }
        }

        private string tncPromptsCommand;
        public string TNCPromptsCommand
        {
            get => GetProperty(ref tncPromptsCommand);
            set
            {
                SetProperty(ref tncPromptsCommand, value);

                //bool changed = CurrentTNCDevice.Prompts.Command != tncPromptsCommand;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Command != tncPromptsCommand;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice.Prompts.Command, tncPromptsCommand);
            }
        }

        private string tncPromptsTimeout;
        public string TNCPromptsTimeout
        {
            get => GetProperty(ref tncPromptsTimeout);
            set
            {
                SetProperty(ref tncPromptsTimeout, value);

                //bool changed = CurrentTNCDevice.Prompts.Timeout != tncPromptsTimeout;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Timeout != tncPromptsTimeout;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice.Prompts.Timeout, tncPromptsTimeout);
            }
        }

        private string tncPromptsConnected;
        public string TNCPromptsConnected
        {
            get => GetProperty(ref tncPromptsConnected);
            set
            {
                SetProperty(ref tncPromptsConnected, value);

                //bool changed = CurrentTNCDevice.Prompts.Connected != tncPromptsConnected;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Connected != tncPromptsConnected;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice.Prompts.Connected, tncPromptsConnected);
            }
        }

        private string tncPromptsDisconnected;
        public string TNCPromptsDisconnected
        {
            get => GetProperty(ref tncPromptsDisconnected);
            set
            {
                SetProperty(ref tncPromptsDisconnected, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.Prompts.Disconnected, tncPromptsDisconnected);
            }
        }

        public async void SaveChanges(int selectedIndex, TNCState tncState)
        {
            if (IsAppBarSaveEnabled)
            {
                _logHelper.Log(LogLevel.Trace, $"In SaveChanges({selectedIndex}, {tncState})");
                // No matter what save button will be disabled
                ResetChangedProperty();
                IsAppBarSaveEnabled = false;
                // State is changed during await
                TNCState savedState = State;
                TNCState savedState2;
                //int savedIndex = TNCDeviceSelectedIndex;
                bool save = await Utilities.ShowDualButtonMessageDialogAsync("Save changes?", "Yes", "No");
                    if (save)
                    {
                        savedState2 = State;
                        State = savedState;
                        //TNCDeviceSelectedIndex = savedIndex;
                        AppBarSaveTNC(selectedIndex, tncState);
                        State = savedState2;
                    }
                    else
                    {
                    // Restore to default
                    //TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex];
                    TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[selectedIndex];
                    if (!string.IsNullOrEmpty(tncDevice.MailUserName))
                        {
                            MailAccountSelectedIndex = EmailAccountArray.Instance.GetSelectedIndexFromEmailUserName(tncDevice.MailUserName);
                            State = TNCState.EMail;
                        }
                        else
                        {
                            DeviceListBoxVisibility = Visibility.Visible;
                            NewTNCDeviceNameVisibility = Visibility.Collapsed;

                            State = TNCState.TNC;
                        }
                        // Disable Save button
                        ResetChangedProperty();
                        IsAppBarSaveEnabled = false;
                    }
            }
        }

        public void AppBarSaveTNC(int selectedIndex, TNCState tncState)
        {
            if (tncState == TNCState.EMail
             || tncState == TNCState.EMailDelete
             || tncState == TNCState.EMailEdit
             || tncState == TNCState.EMailAdd)
            {
                SaveEmailAccounts(selectedIndex, tncState);
            }
            else
            {
                SaveTNCDevices();
            }
            DeviceListBoxVisibility = Visibility.Visible;
            NewTNCDeviceNameVisibility = Visibility.Collapsed;

            // Make sure Packet settings have the latest TNC devices, also restore currently selected TNC
            PacketSettingsViewModel packetSettingsViewmodel = Singleton<PacketSettingsViewModel>.Instance;
            string selectedTNC = packetSettingsViewmodel.TNC;
            packetSettingsViewmodel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
            packetSettingsViewmodel.TNC = selectedTNC;

            // Disable Save button
            ResetChangedProperty();
            IsAppBarSaveEnabled = false;

        }

        public async void SaveTNCDevices()
        {
            if (State == TNCState.TNCAdd)
            {
                if (string.IsNullOrEmpty(NewTNCDeviceName))
                {
                    //await Utilities.ShowMessageDialogAsync("The new TNC Device must have a name.", "Add TNC Device error");
                    return;
                }
                TNCDevice tncDevice = TNCDeviceFromUI;
                CurrentTNCDevice = tncDevice;
                TNCDeviceArray.Instance.TNCDeviceListUpdate(TNCDeviceArray.Instance.TNCDeviceList.Count - 1, tncDevice);
                await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = TNCDeviceArray.Instance.TNCDeviceList.Count - 1;
                State = TNCState.TNC;
            }
            else if (State == TNCState.TNCEdit)
            {
                TNCDevice tncDevice = TNCDeviceFromUI;
                CurrentTNCDevice = tncDevice;
                TNCDeviceArray.Instance.TNCDeviceListUpdate(TNCDeviceSelectedIndex, tncDevice);
                await TNCDeviceArray.Instance.SaveAsync();
                //TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                State = TNCState.TNC;
                //_logHelper.Log(LogLevel.Trace, $"Saving, Comport: {tncDevice.CommPort.Comport}");
            }
            else if (State == TNCState.TNCDelete)
            {
                await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = Math.Min(TNCDeviceArray.Instance.TNCDeviceList.Count - 1, _deletedIndex);
                State = TNCState.TNC;
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
                //if (State == TNCState.EMailAdd || State == TNCState.EMailEdit)
                //    return;

                if (value < 0)
                {
                    //    SetProperty(ref mailAccountSelectedIndex, 0, true);
                    SetProperty(ref mailAccountSelectedIndex, value, true);
                    return;
                }
                if (value >= EmailAccountArray.Instance.EmailAccountList.Count)
                {
                    SetProperty(ref mailAccountSelectedIndex, EmailAccountArray.Instance.EmailAccountList.Count - 1, true);
                    //CurrentMailAccount = EmailAccountArray.Instance.EmailAccountList[mailAccountSelectedIndex];
                }
                else
                {
                    SetProperty(ref mailAccountSelectedIndex, value, true);
                }

                CurrentMailAccount = EmailAccountArray.Instance.EmailAccountList[mailAccountSelectedIndex];

                bool changed = CurrentTNCDevice.MailUserName != CurrentMailAccount.MailUserName;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private ObservableCollection<EmailAccount> _MailAccountListSource;
        public ObservableCollection<EmailAccount> MailAccountListSource
        {
            get => new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
            set => Set(ref _MailAccountListSource, value);
        }

        public EmailAccount EMailAccountFromUI
        {
            get
            {
                EmailAccount eMailAccountFromUI = new EmailAccount();

                eMailAccountFromUI.MailServer = MailServer;
                eMailAccountFromUI.MailServerPort = MailServerPort;
                eMailAccountFromUI.MailIsSSLField = IsMailSSL;
                eMailAccountFromUI.MailUserName = MailUserName;
                eMailAccountFromUI.MailPassword = MailPassword;

                return eMailAccountFromUI;
            }
        }

        private EmailAccount currentMailAccount;
        public EmailAccount CurrentMailAccount
        {
            get => currentMailAccount;
            set
            {
                currentMailAccount = value;

                MailServer = currentMailAccount.MailServer;
                MailServerPort = currentMailAccount.MailServerPort;
                IsMailSSL = currentMailAccount.MailIsSSLField;
                MailUserName = currentMailAccount.MailUserName;
                MailPassword = currentMailAccount.MailPassword;

                //ResetChangedProperty();
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
                bool changed = CurrentMailAccount?.MailServer != mailServer;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.Server = MailServer;
            }
        }

        private bool isMailServerPortEnabled;
        public bool IsMailServerPortEnabled
        {
            get => isMailServerPortEnabled;
            set => Set(ref isMailServerPortEnabled, value);
        }

        private ushort eMailServerPort;
        public ushort MailServerPort
        {
            get => eMailServerPort;
            set
            {
                SetProperty(ref eMailServerPort, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailServerPort != eMailServerPort;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.Port = MailServerPort;
            }
        }

        private Visibility isEMailServerSSLVisible;
        public Visibility IsMailServerSSLVisible
        {
            get => isEMailServerSSLVisible;
            set => Set(ref isEMailServerSSLVisible, value);
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

        private bool isEMailUserNameEnabled;
        public bool IsEMailUserNameEnabled
        {
            get => isEMailUserNameEnabled;
            set => Set(ref isEMailUserNameEnabled, value);
        }

        private string mailUserName;
        public string MailUserName
        {
            get => mailUserName;
            set
            {
                //if (State == TNCState.EMailAdd)
                //    return;

                SetProperty(ref mailUserName, value);

                bool changed = CurrentMailAccount?.MailUserName != mailUserName;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.UserName = MailUserName;
            }
        }

        private bool isEMailPasswordEnabled;
        public bool IsMailPasswordEnabled
        {
            get => isEMailPasswordEnabled;
            set => Set(ref isEMailPasswordEnabled, value);
        }

        private string mailPassword;
        public string MailPassword
        {
            get => mailPassword;
            set
            {
                SetProperty(ref mailPassword, value);

                bool changed = CurrentMailAccount?.MailPassword != mailPassword;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.Password = MailPassword;
            }
        }

        public void SetMailControlsEditState(bool enabledState)
        {
            IsMailPasswordEnabled = enabledState;
            if (enabledState)
            {
                IsMailServerSSLVisible = Visibility.Collapsed;
                IsEMailUserNameEnabled = false;
                //mailServer.Visibility = Visibility.Visible;
                IsMailServerPortEnabled = !enabledState;
                IsMailServerEnabled = !enabledState;
            }
            else
            {
                IsMailServerSSLVisible = Visibility.Collapsed;
                //mailServer.Visibility = Visibility.Collapsed;
            }
        }

        public void SetMailControlsEnabledState(bool enabledState)
        {
            IsMailPasswordEnabled = enabledState;
            IsEMailUserNameEnabled = true;
            if (enabledState)
            {
                IsMailServerSSLVisible = Visibility.Visible;
                IsMailServerPortEnabled = enabledState;
                IsMailServerEnabled = enabledState;
            }
            else
            {
                IsMailServerSSLVisible = Visibility.Collapsed;
                IsMailServerPortEnabled = enabledState;
                IsMailServerEnabled = enabledState;
            }
        }

        public void UpdateMailState(TNCState newMailState)
        {
            if (newMailState == State)
                return;

            State = newMailState;
            switch (newMailState)
            {
                case TNCState.EMail:
                    SetMailControlsEnabledState(false);
                    break;
                case TNCState.EMailEdit:
                    SetMailControlsEditState(true);
                    break;
                case TNCState.EMailDelete:
                    SetMailControlsEnabledState(false);
                    break;
                case TNCState.EMailAdd:
                    SetMailControlsEnabledState(true);
                    //mailServer.Text = "";
                    //MailServerPort = 0;
                    //MailUserName = "";
                    //MailPassword = "";
                    //IsMailSSL = false;
                    break;
                case TNCState.TNC:
                    SetMailControlsEnabledState(false);
                    break;
            }
        }

        private async void SaveEmailAccounts(int selectedIndex, TNCState tncState)
        {
            if (State == TNCState.EMail)
            {
                EmailAccount emailAccount = CurrentMailAccount;

                int tncDeviceSelectedIndex = TNCDeviceSelectedIndex;
                //TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex];
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[selectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;
                tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
                _logHelper.Log(LogLevel.Trace, $"SaveEmailAccounts(selectedIndex): {tncDeviceSelectedIndex}");
                await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = 0;
                TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
                Singleton<PacketSettingsViewModel>.Instance.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                Singleton<PacketSettingsViewModel>.Instance.TNC = tncDevice.Name;
            }
            else if (State == TNCState.EMailDelete)
            {
                int tncDeviceSelectedIndex = TNCDeviceSelectedIndex;

                await EmailAccountArray.Instance.SaveAsync();
                UpdateMailState(TNCSettingsViewModel.TNCState.EMail);
                MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
                MailAccountSelectedIndex = Math.Min(EmailAccountArray.Instance.EmailAccountList.Count - 1, _deletedIndex);

                EmailAccount emailAccount = CurrentMailAccount;
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;
                tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
                await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
            }
            else if (State == TNCState.EMailEdit)
            {
                EmailAccount emailAccount = EMailAccountFromUI;
                EmailAccountArray.Instance.EmailAccountList[_modifiedEmailAccountSelectedIndex] = emailAccount;
                await EmailAccountArray.Instance.SaveAsync();

                UpdateMailState(TNCState.EMail);
                MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
                MailAccountSelectedIndex = _modifiedEmailAccountSelectedIndex;

                int tncDeviceSelectedIndex = TNCDeviceSelectedIndex;
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;     // TODO double user name??
                tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
                await TNCDeviceArray.Instance.SaveAsync();

                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
            }
            else if (State == TNCState.EMailAdd)
            {
                EmailAccount emailAccount = EMailAccountFromUI;
                EmailAccountArray.Instance.EmailAccountList.Add(emailAccount);
                await EmailAccountArray.Instance.SaveAsync();
                UpdateMailState(TNCState.EMail);

                MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
                MailAccountSelectedIndex = _modifiedEmailAccountSelectedIndex;
                // No need to update connected devides because we always select the last used email account
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

        private new bool isAppBarSaveEnabled;
        public new bool IsAppBarSaveEnabled
        {
            get => isAppBarSaveEnabled;
            set => SetProperty(ref isAppBarSaveEnabled, value);
        }

    }
}
