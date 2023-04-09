using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;

using MetroLog;

using CommunityToolkit.Mvvm.Input;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml;


namespace PacketMessagingTS.ViewModels
{
    public class TNCSettingsViewModel : ViewModelBase
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TNCSettingsViewModel>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public static TNCSettingsViewModel Instance { get; } = new TNCSettingsViewModel();

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

        //private static readonly object _syncRoot = new Object();


        public TNCSettingsViewModel()
        {

        }

        public TNCState State { get; set; }

        private Visibility _pivotTNCVisibility;
        public Visibility PivotTNCVisibility
        {
            get => _pivotTNCVisibility;
            set => SetProperty(ref _pivotTNCVisibility, value);
        }

        private int _pivotTNCSelectedIndex;
        public int PivotTNCSelectedIndex
        {
            get => GetProperty(ref _pivotTNCSelectedIndex);
            set => SetPropertyPrivate(ref _pivotTNCSelectedIndex, value, true);
        }

        private Visibility _deviceListBoxVisibility = Visibility.Visible;
        public Visibility DeviceListBoxVisibility
        {
            get => _deviceListBoxVisibility;
            set => SetProperty(ref _deviceListBoxVisibility, value);
        }

        private Visibility _newTNCDeviceNameVisibility = Visibility.Collapsed;
        public Visibility NewTNCDeviceNameVisibility
        {
            get => _newTNCDeviceNameVisibility;
            set => SetProperty(ref _newTNCDeviceNameVisibility, value);
        }

        private int _tncDeviceSelectedIndex;
        public int TNCDeviceSelectedIndex
        {
            get
            {
                GetProperty(ref _tncDeviceSelectedIndex);
                if (CurrentTNCDevice == null)
                {
                    CurrentTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[_tncDeviceSelectedIndex];
                }
                return _tncDeviceSelectedIndex;
            }
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

                if (value == _tncDeviceSelectedIndex && !(_currentTNCDevice is null))
                    return;

                //SaveChanges(tncDeviceSelectedIndex, State);

                //_logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index after SaveChanges(): {value}, {tncDeviceSelectedIndex}");

                bool setPropertySuccess;
                if (value < 0)
                {
                    SetProperty(ref _tncDeviceSelectedIndex, value);
                    _logHelper.Log(LogLevel.Trace, $"Set TNCDevice Sel Index after -1: {value}, {_tncDeviceSelectedIndex}");
                    return;
                }
                else if (value >= TNCDeviceArray.Instance.TNCDeviceList.Count)
                {
                    setPropertySuccess = SetPropertyPrivate(ref _tncDeviceSelectedIndex, 0, true);
                }
                else
                {
                    setPropertySuccess = SetPropertyPrivate(ref _tncDeviceSelectedIndex, value, true);
                }
                if (setPropertySuccess)
                {
                    SaveChanges(_tncDeviceSelectedIndex, State);
                    // Utilities.SetApplicationTitle();
                }
                CurrentTNCDevice = TNCDeviceArray.Instance.TNCDeviceList[_tncDeviceSelectedIndex];
                if (CurrentTNCDevice.Name.Contains(PublicData.EMail))
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
                SetProperty(ref _TNCDeviceListSource, value);
            }
        }


        private string _NewTNCDeviceName = "new Device Name";
        public string NewTNCDeviceName
        {
            get => _NewTNCDeviceName;
            set => SetProperty(ref _NewTNCDeviceName, value);
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

        private TNCDevice _currentTNCDevice;
        public TNCDevice CurrentTNCDevice
        {
            get
            {
                if (_currentTNCDevice is null)
                {
                    TNCDeviceSelectedIndex = Utilities.GetProperty(nameof(TNCDeviceSelectedIndex));
                }
                
                return _currentTNCDevice;
            }
            set
            {
                ResetChangedProperty();

                _currentTNCDevice = value;

                if (!string.IsNullOrEmpty(_currentTNCDevice.Name) && _currentTNCDevice.Name.Contains(PublicData.EMail))
                {
                    //// Update email account index
                    //string mailPreample = PublicData.EMailPreample;
                    //string mailUserName;
                    //int index = currentTNCDevice.Name.IndexOf(mailPreample);
                    //if (index == 0)
                    //{
                    //    mailUserName = currentTNCDevice.Name.Substring(mailPreample.Length);
                    //    int i = 0;
                    //    for (; i < EmailAccountArray.Instance.EmailAccounts.Length; i++)
                    //    {
                    //        if (mailUserName.Contains(EmailAccountArray.Instance.EmailAccounts[i].MailUserName))
                    //        {
                    //            break;
                    //        }
                    //    }
                    //    if (i >= EmailAccountArray.Instance.EmailAccounts.Length)
                    //        MailAccountSelectedIndex = 0;
                    //    else
                    //        MailAccountSelectedIndex = i;
                    //}
                }
                else
                {
                    //_logHelper.Log(LogLevel.Trace, $"Current device, Comport: {currentTNCDevice.CommPort.Comport}");

                    TNCInitCommandsPre = _currentTNCDevice.InitCommands.Precommands;
                    TNCInitCommandsPost = _currentTNCDevice.InitCommands.Postcommands;
                    IsToggleSwitchOn = _currentTNCDevice.CommPort.IsBluetooth;
                    TNCComPort = _currentTNCDevice.CommPort.Comport;
                    TNCComName = _currentTNCDevice.CommPort.BluetoothName;
                    TNCComBaudRate = _currentTNCDevice.CommPort.Baudrate;
                    TNCComDatabits = _currentTNCDevice.CommPort.Databits;
                    TNCComStopbits = _currentTNCDevice.CommPort.Stopbits;
                    TNCComParity = _currentTNCDevice.CommPort.Parity;
                    TNCComHandshake = _currentTNCDevice.CommPort.Flowcontrol;
                    TNCCommandsMyCall = _currentTNCDevice.Commands.MyCall;
                    TNCCommandsConnect = _currentTNCDevice.Commands.Connect;
                    TNCCommandsRetry = _currentTNCDevice.Commands.Retry;
                    TNCCommandsConversMode = _currentTNCDevice.Commands.Conversmode;
                    TNCCommandsDateTime = _currentTNCDevice.Commands.Datetime;
                    TNCPromptsCommand = _currentTNCDevice.Prompts.Command;
                    TNCPromptsTimeout = _currentTNCDevice.Prompts.Timeout;
                    TNCPromptsConnected = _currentTNCDevice.Prompts.Connected;
                    TNCPromptsDisconnected = _currentTNCDevice.Prompts.Disconnected;
                }
                //ResetChangedProperty();
            }
        }

        private string _tncInitCommandsPre;
        public string TNCInitCommandsPre
        {
            get => _tncInitCommandsPre;
            set
            {
                SetProperty(ref _tncInitCommandsPre, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.InitCommands.Precommands, _tncInitCommandsPre);
            }
        }

        private string _tncInitCommandsPost;
        public string TNCInitCommandsPost
        {
            get => _tncInitCommandsPost;
            set
            {
                SetProperty(ref _tncInitCommandsPost, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.InitCommands.Postcommands, _tncInitCommandsPost);
            }
        }

        private void UpdateTNCStateAndButtons<T>(T savedProperty, T newProperty)
        {
            if (savedProperty == null || newProperty == null)
                return;

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

        private bool _isToggleSwitchOn;
        public bool IsToggleSwitchOn
        {
            get => _isToggleSwitchOn;
            set
            {
                SetProperty(ref _isToggleSwitchOn, value);

                if (_isToggleSwitchOn)
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

        public List<string> ListOfSerialPorts = new List<string>();

        private ObservableCollection<string> _collectionOfSerialDevices;
        public ObservableCollection<string> CollectionOfSerialDevices
        {
            get => _collectionOfSerialDevices;
            //get
            //{
            //    //if (collectionOfSerialDevices is null)
            //    //{
            //    //    collectionOfSerialDevices = CreateComportObservableCollectionAsync().Result;
            //    //}
            //    return collectionOfSerialDevices;
            //    //_logHelper.Log(LogLevel.Info, $"Serial Port count: {ListOfSerialPorts.Count}");
            //    //return new ObservableCollection<string>(ListOfSerialPorts);
            //}
            set => SetProperty(ref _collectionOfSerialDevices, value);
        }

        private string _tncComPort;
        public string TNCComPort
        {
            get => _tncComPort;
            set
            {
                //_logHelper.Log(LogLevel.Trace, $"Comport: {value}");

                if (value is null || CollectionOfSerialDevices is null || CollectionOfSerialDevices.Count == 0)
                    return;

                SetProperty(ref _tncComPort, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.Comport, _tncComPort);
            }
        }

        private Visibility _tncComPortVisible;
        public Visibility TNCComPortVisible
        {
            get => _tncComPortVisible;
            set
            {
                SetProperty(ref _tncComPortVisible, value);
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
        private string _tncComName;
        public string TNCComName
        {
            get => _tncComName;
            set
            {
                SetProperty(ref _tncComName, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice.CommPort.BluetoothName, _tncComName);
            }
        }

        private Visibility _tncComNameVisible;
        public Visibility TNCComNameVisible
        {
            get => _tncComNameVisible;
            set
            {
                SetProperty(ref _tncComNameVisible, value);
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

        private ushort _tncComBaudRate;
        public ushort TNCComBaudRate
        {
            get => _tncComBaudRate;
            set
            {
                SetProperty(ref _tncComBaudRate, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.CommPort?.Baudrate, _tncComBaudRate);
            }
        }

        public ObservableCollection<ushort> DatabitsCollection
        {
            get => new ObservableCollection<ushort>() { 7, 8 };
        }

        private ushort _tncComDatabits;
        public ushort TNCComDatabits
        {
            get => _tncComDatabits;
            set
            {
                SetProperty(ref _tncComDatabits, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.CommPort?.Databits, _tncComDatabits);
            }
        }

        private StopBits _tncComStopbits;
        public StopBits TNCComStopbits
        {
            get => _tncComStopbits;
            set
            {
                SetProperty(ref _tncComStopbits, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.CommPort?.Stopbits, _tncComStopbits);
            }
        }

        private Parity _tncComParity;
        public Parity TNCComParity
        {
            get => _tncComParity;
            set
            {
                SetProperty(ref _tncComParity, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.CommPort?.Parity, _tncComParity);
            }
        }

        private Handshake _tncComHandshake;
        public Handshake TNCComHandshake
        {
            get { return _tncComHandshake; }
            set
            {
                SetProperty(ref _tncComHandshake, value);

                if (TNCDeviceSelectedIndex < 0)
                    return;

                UpdateTNCStateAndButtons(_SavedTNCDevice?.CommPort?.Flowcontrol, _tncComHandshake);

            }
        }

        private string _tncCommandsMyCall;
        public string TNCCommandsMyCall
        {
            get => _tncCommandsMyCall;
            set
            {
                SetProperty(ref _tncCommandsMyCall, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Commands?.MyCall, _tncCommandsMyCall);
            }
        }

        private string _tncCommandsConnect;
        public string TNCCommandsConnect
        {
            get => _tncCommandsConnect;
            set
            {
                SetProperty(ref _tncCommandsConnect, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Commands?.Connect, _tncCommandsConnect);
            }
        }

        private string _tncCommandsRetry;
        public string TNCCommandsRetry
        {
            get => _tncCommandsRetry;
            set
            {
                SetProperty(ref _tncCommandsRetry, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Commands?.Retry, _tncCommandsRetry);
            }
        }

        private string _tncCommandsConversMode;
        public string TNCCommandsConversMode
        {
            get => _tncCommandsConversMode;
            set
            {
                SetProperty(ref _tncCommandsConversMode, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Commands?.Conversmode, _tncCommandsConversMode);
            }
        }

        private string _tncCommandsDateTime;
        public string TNCCommandsDateTime
        {
            get => _tncCommandsDateTime;
            set
            {
                SetProperty(ref _tncCommandsDateTime, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Commands?.Datetime, _tncCommandsDateTime);
            }
        }

        private string _tncPromptsCommand;
        public string TNCPromptsCommand
        {
            get => GetProperty(ref _tncPromptsCommand);
            set
            {
                SetProperty(ref _tncPromptsCommand, value);

                //bool changed = CurrentTNCDevice.Prompts.Command != tncPromptsCommand;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Command != tncPromptsCommand;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice?.Prompts?.Command, _tncPromptsCommand);
            }
        }

        private string _tncPromptsTimeout;
        public string TNCPromptsTimeout
        {
            get => GetProperty(ref _tncPromptsTimeout);
            set
            {
                SetProperty(ref _tncPromptsTimeout, value);

                //bool changed = CurrentTNCDevice.Prompts.Timeout != tncPromptsTimeout;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Timeout != tncPromptsTimeout;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice?.Prompts?.Timeout, _tncPromptsTimeout);
            }
        }

        private string _tncPromptsConnected;
        public string TNCPromptsConnected
        {
            get => GetProperty(ref _tncPromptsConnected);
            set
            {
                SetProperty(ref _tncPromptsConnected, value);

                //bool changed = CurrentTNCDevice.Prompts.Connected != tncPromptsConnected;
                //bool changed = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex].Prompts.Connected != tncPromptsConnected;
                //UpdateTNCStateAndButtons(changed);
                UpdateTNCStateAndButtons(_SavedTNCDevice?.Prompts?.Connected, _tncPromptsConnected);
            }
        }

        private string _tncPromptsDisconnected;
        public string TNCPromptsDisconnected
        {
            get => GetProperty(ref _tncPromptsDisconnected);
            set
            {
                SetProperty(ref _tncPromptsDisconnected, value);

                UpdateTNCStateAndButtons(_SavedTNCDevice?.Prompts?.Disconnected, _tncPromptsDisconnected);
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
                bool save = await ContentDialogs.ShowDualButtonMessageDialogAsync("Save changes?", "Yes", "No");
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
                SaveEmailAccounts(selectedIndex);
            }
            else
            {
                SaveTNCDevices();
            }
            DeviceListBoxVisibility = Visibility.Visible;
            NewTNCDeviceNameVisibility = Visibility.Collapsed;

            // Make sure Packet settings have the latest TNC devices, also restore currently selected TNC
            PacketSettingsViewModel packetSettingsViewmodel = PacketSettingsViewModel.Instance;
            //string selectedTNC = packetSettingsViewmodel.TNC;
            packetSettingsViewmodel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
            packetSettingsViewmodel.ProfileSelectedIndex = Utilities.GetProperty(nameof(packetSettingsViewmodel.ProfileSelectedIndex));

            //packetSettingsViewmodel.TNC = selectedTNC;

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
                //await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = TNCDeviceArray.Instance.TNCDeviceList.Count - 1;
                State = TNCState.TNC;
            }
            else if (State == TNCState.TNCEdit)
            {
                TNCDevice tncDevice = TNCDeviceFromUI;
                CurrentTNCDevice = tncDevice;
                TNCDeviceArray.Instance.TNCDeviceListUpdate(TNCDeviceSelectedIndex, tncDevice);
                //await TNCDeviceArray.Instance.SaveAsync();
                //TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                State = TNCState.TNC;
                //_logHelper.Log(LogLevel.Trace, $"Saving, Comport: {tncDevice.CommPort.Comport}");
            }
            //else if (State == TNCState.TNCDelete)
            //{
                await TNCDeviceArray.Instance.SaveAsync();
            //    TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
            //    TNCDeviceSelectedIndex = Math.Min(TNCDeviceArray.Instance.TNCDeviceList.Count - 1, _deletedIndex);
            //    State = TNCState.TNC;
            //}
        }

#region Mail Settings
        private Visibility _eMailSettingsVisibility;
        public Visibility EMailSettingsVisibility
        {
            get => _eMailSettingsVisibility;
            set => SetProperty(ref _eMailSettingsVisibility, value);
        }

        private int _mailAccountSelectedIndex;
        public int MailAccountSelectedIndex
        {
            get
            {
                GetProperty(ref _mailAccountSelectedIndex);

                //if (mailAccountSelectedIndex >= 0)
                //{
                //    CurrentMailAccount = EmailAccountArray.Instance.EmailAccounts[mailAccountSelectedIndex];
                //}
                return _mailAccountSelectedIndex;
            }
            set
            {
                //if (State == TNCState.EMailAdd || State == TNCState.EMailEdit)
                //    return;

                if (value < 0)
                {
                    //    SetProperty(ref mailAccountSelectedIndex, 0, true);
                    SetPropertyPrivate(ref _mailAccountSelectedIndex, value, true);
                    return;
                }
                if (value >= EmailAccountArray.Instance.EmailAccountList.Count)
                {
                    SetPropertyPrivate(ref _mailAccountSelectedIndex, EmailAccountArray.Instance.EmailAccountList.Count - 1, true);
                    //CurrentMailAccount = EmailAccountArray.Instance.EmailAccountList[mailAccountSelectedIndex];
                }
                else
                {
                    SetPropertyPrivate(ref _mailAccountSelectedIndex, value, true);
                }

                CurrentMailAccount = EmailAccountArray.Instance.EmailAccountList[_mailAccountSelectedIndex];

                bool changed = CurrentTNCDevice.MailUserName != CurrentMailAccount.MailUserName;
                IsAppBarSaveEnabled = SaveEnabled(changed);
            }
        }

        private ObservableCollection<EmailAccount> _MailAccountListSource;
        public ObservableCollection<EmailAccount> MailAccountListSource
        {
            get => new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
            set => SetProperty(ref _MailAccountListSource, value);
        }

        public EmailAccount EMailAccountFromUI
        {
            get
            {
                EmailAccount eMailAccountFromUI = new EmailAccount()
                {
                    MailServer = MailServer,
                    MailServerPort = MailServerPort,
                    MailIsSSLField = IsMailSSL,
                    MailUserName = MailUserName,
                    MailPassword = MailPassword,
                };
                return eMailAccountFromUI;
            }
        }

        private EmailAccount _currentMailAccount;
        public EmailAccount CurrentMailAccount
        {
            get => _currentMailAccount;
            set
            {
                _currentMailAccount = value;

                MailServer = _currentMailAccount.MailServer;
                MailServerPort = _currentMailAccount.MailServerPort;
                IsMailSSL = _currentMailAccount.MailIsSSLField;
                MailUserName = _currentMailAccount.MailUserName;
                MailPassword = _currentMailAccount.MailPassword;

                //ResetChangedProperty();
            }
        }

        private bool _isMailServerEnabled;
        public bool IsMailServerEnabled
        {
            get => _isMailServerEnabled;
            set => SetProperty(ref _isMailServerEnabled, value);
        }

        private string _mailServer;
        public string MailServer
        {
            get => GetProperty(ref _mailServer);
            set
            {
                SetProperty(ref _mailServer, value);
                bool changed = CurrentMailAccount?.MailServer != _mailServer;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.Server = MailServer;
            }
        }

        private bool _isMailServerPortEnabled;
        public bool IsMailServerPortEnabled
        {
            get => _isMailServerPortEnabled;
            set => SetProperty(ref _isMailServerPortEnabled, value);
        }

        private ushort _eMailServerPort;
        public ushort MailServerPort
        {
            get => _eMailServerPort;
            set
            {
                SetProperty(ref _eMailServerPort, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailServerPort != _eMailServerPort;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.Port = MailServerPort;
            }
        }

        private Visibility _isEMailServerSSLVisible;
        public Visibility IsMailServerSSLVisible
        {
            get => _isEMailServerSSLVisible;
            set => SetProperty(ref _isEMailServerSSLVisible, value);
        }

        private bool _isMailSSL;
        public bool IsMailSSL
        {
            get => _isMailSSL;
            set
            {
                SetProperty(ref _isMailSSL, value);

                if (CurrentMailAccount != null)
                {
                    bool changed = CurrentMailAccount.MailIsSSLField != _isMailSSL;
                    IsAppBarSaveEnabled = SaveEnabled(changed);
                }

                Services.SMTPClient.SmtpClient.Instance.IsSsl = IsMailSSL;
            }
        }

        private bool _isEMailUserNameEnabled;
        public bool IsEMailUserNameEnabled
        {
            get => _isEMailUserNameEnabled;
            set => SetProperty(ref _isEMailUserNameEnabled, value);
        }

        private string _mailUserName;
        public string MailUserName
        {
            get => _mailUserName;
            set
            {
                //if (State == TNCState.EMailAdd)
                //    return;

                SetProperty(ref _mailUserName, value);

                bool changed = CurrentMailAccount?.MailUserName != _mailUserName;
                IsAppBarSaveEnabled = SaveEnabled(changed);

                Services.SMTPClient.SmtpClient.Instance.UserName = MailUserName;
            }
        }

        private bool _isEMailPasswordEnabled;
        public bool IsMailPasswordEnabled
        {
            get => _isEMailPasswordEnabled;
            set => SetProperty(ref _isEMailPasswordEnabled, value);
        }

        private string _mailPassword;
        public string MailPassword
        {
            get => _mailPassword;
            set
            {
                SetProperty(ref _mailPassword, value);

                bool changed = CurrentMailAccount?.MailPassword != _mailPassword;
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

        private async void SaveEmailAccounts(int selectedIndex)
        {
            if (State == TNCState.EMail)
            {
                EmailAccount emailAccount = CurrentMailAccount;

                int tncDeviceSelectedIndex = TNCDeviceSelectedIndex;
                //TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[TNCDeviceSelectedIndex];
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[selectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;
                tncDevice.Name = $"{PublicData.EMailPreample}{emailAccount.MailUserName}";
                _logHelper.Log(LogLevel.Trace, $"SaveEmailAccounts(selectedIndex): {tncDeviceSelectedIndex}");
                await TNCDeviceArray.Instance.SaveAsync();
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = 0;
                TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
                PacketSettingsViewModel.Instance.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                PacketSettingsViewModel.Instance.TNC = tncDevice.Name;
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
                tncDevice.Name = $"{PublicData.EMailPreample}{emailAccount.MailUserName}";
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
                tncDevice.Name = $"{PublicData.EMailPreample}{emailAccount.MailUserName}";
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
        private bool _isAppBarAddEnabled = true;
        public bool IsAppBarAddEnabled
        {
            get => _isAppBarAddEnabled;
            set => SetProperty(ref _isAppBarAddEnabled, value);
        }

        private bool _isAppBarDeleteEnabled = true;
        public bool IsAppBarDeleteEnabled
        {
            get => _isAppBarDeleteEnabled;
            set => SetProperty(ref _isAppBarDeleteEnabled, value);
        }

        private bool _isAppBarEditEnabled = true;
        public bool IsAppBarEditEnabled
        {
            get => _isAppBarEditEnabled;
            set => SetProperty(ref _isAppBarEditEnabled, value);
        }

        //private bool _isAppBarSaveEnabled;
        //public override bool IsAppBarSaveEnabled
        //{
        //    get => _isAppBarSaveEnabled;
        //    set => SetProperty(ref _isAppBarSaveEnabled, value);
        //}

        private void NewTNCDevice()
        {
            DeviceListBoxVisibility = Visibility.Collapsed;
            NewTNCDeviceNameVisibility = Visibility.Visible;

            TNCInitCommandsPre = "";
            TNCInitCommandsPost = "";

            IsToggleSwitchOn = false;
            //SetComportComboBoxVisibility();

            //if (CollectionOfBluetoothDevices.Count > 0)
            //{
            //    comboBoxComName.SelectedItem = CollectionOfBluetoothDevices[0];
            //}

            if (CollectionOfSerialDevices.Count > 0)
            {
                TNCComPort = CollectionOfSerialDevices[0];
            }

            TNCComBaudRate = 9600;
            TNCComDatabits = 8;

            //int i = 0;
            //var values = Enum.GetValues(typeof(SerialParity));
            //for (; i < values.Length; i++)
            //{
            //    if ((SerialParity)values.GetValue(i) == SerialParity.None) break;
            //}
            //comboBoxParity.SelectedIndex = i;
            TNCComParity = Parity.None;

            //values = Enum.GetValues(typeof(SerialStopBitCount));
            //for (i = 0; i < values.Length; i++)
            //{
            //    if ((SerialStopBitCount)values.GetValue(i) == SerialStopBitCount.One) break;
            //}
            //comboBoxStopBits.SelectedIndex = i;
            TNCComStopbits = StopBits.One;

            //values = Enum.GetValues(typeof(SerialHandshake));
            //for (i = 0; i < values.Length; i++)
            //{
            //    if ((SerialHandshake)values.GetValue(i) == SerialHandshake.RequestToSend)
            //    {
            //        break;
            //    }
            //}
            //comboBoxFlowControl.SelectedIndex = i;
            TNCComHandshake = Handshake.RequestToSend;

            TNCPromptsCommand = "";
            TNCPromptsTimeout = "";
            TNCPromptsConnected = "";
            TNCPromptsDisconnected = "";

            TNCCommandsConnect = "";
            TNCCommandsConversMode = "";
            TNCCommandsMyCall = "";
            TNCCommandsRetry = "";
            TNCCommandsDateTime = "";

            //TNCDevice tncDevice = new TNCDevice();
            //tncDevice = TNCDeviceFromUI;
            //TNCDeviceArray.Instance.TNCDeviceList.Add(tncDevice);
            //CurrentTNCDevice = tncDevice;
            IsAppBarSaveEnabled = true;
        }

        private RelayCommand _AppBarAddTNCCommand;
        public RelayCommand AppBarAddTNCCommand => _AppBarAddTNCCommand ?? (_AppBarAddTNCCommand = new RelayCommand(AppBarAddTNC));
        private void AppBarAddTNC()
        {
            if (State == TNCState.EMail)
            {
                _modifiedEmailAccountSelectedIndex = MailAccountSelectedIndex;
                UpdateMailState(TNCSettingsViewModel.TNCState.EMailAdd);
                IsAppBarSaveEnabled = true;
            }
            else if (State != TNCState.EMailDelete
                            || State != TNCState.EMailEdit
                            || State != TNCState.EMailAdd)
            {
                // Not an Email device
                State = TNCState.TNCAdd;
                NewTNCDevice();
            }
        }

        private RelayCommand _AppBarEditTNCCommand;
        public RelayCommand AppBarEditTNCCommand => _AppBarEditTNCCommand ?? (_AppBarEditTNCCommand = new RelayCommand(AppBarEditTNC));
        private void AppBarEditTNC()
        {
            if (State == TNCState.EMail)
            {
                UpdateMailState(TNCState.EMailEdit);
                _modifiedEmailAccountSelectedIndex = MailAccountSelectedIndex;
            }
            else
            {
                State = TNCState.TNCEdit;
            }
        }

        private RelayCommand _AppBarSaveTNCCommand;
        public RelayCommand AppBarSaveTNCCommand => _AppBarSaveTNCCommand ?? (_AppBarSaveTNCCommand = new RelayCommand(AppBarSaveTNC));
        private void AppBarSaveTNC()
        {
            if (State == TNCState.TNCAdd)
            {
                TNCDevice tncDevice = TNCDeviceFromUI;
                TNCDeviceArray.Instance.TNCDeviceList.Add(tncDevice);
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                CurrentTNCDevice = tncDevice;
                TNCDeviceSelectedIndex = TNCDeviceArray.Instance.TNCDeviceList.Count - 1;
                State = TNCState.TNCAdd;
            }
            AppBarSaveTNC(TNCDeviceSelectedIndex, State);
            //int selectedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;
            //_TNCSettingsViewModel.AppBarSaveTNC(selectedIndex);
            //_TNCSettingsViewModel.TNCDeviceSelectedIndex = selectedIndex;
            return;
        }

        private RelayCommand _AppBarDeleteTNCCommand;
        public RelayCommand AppBarDeleteTNCCommand => _AppBarDeleteTNCCommand ?? (_AppBarDeleteTNCCommand = new RelayCommand(AppBarDeleteTNC));
        private void AppBarDeleteTNC()
        {
            if (State == TNCState.EMail)
            {
                State = TNCState.EMailDelete;
                _deletedIndex = MailAccountSelectedIndex;
                EmailAccountArray.Instance.EmailAccountList.RemoveAt(_deletedIndex);
                IsAppBarSaveEnabled = true;
            }
            else
            {
                State = TNCState.TNCDelete;
                _deletedIndex = TNCDeviceSelectedIndex;
                TNCDeviceArray.Instance.TNCDeviceList.RemoveAt(_deletedIndex);
                TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceSelectedIndex = Math.Max(_deletedIndex - 1, 0);
                IsAppBarSaveEnabled = true;
            }
        }

    }
}
