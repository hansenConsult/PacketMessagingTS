using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;


namespace PacketMessagingTS.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();
        public IdentityViewModel _identityViewModel { get; } = new IdentityViewModel();
        public PacketSettingsViewModel _packetSettingsViewModel { get; } = new PacketSettingsViewModel();
        public TNCSettingsViewModel _TNCSettingsViewModel { get; } = new TNCSettingsViewModel();
        public MailSettingsViewModel _mailSettingsViewModel { get; } = new MailSettingsViewModel();

        static ObservableCollection<Profile> _profileCollection;

        private ObservableCollection<SerialDevice> CollectionOfSerialDevices;

        private ObservableCollection<uint> listOfBaudRates;
        private ObservableCollection<ushort> listOfDataBits;



        public SettingsPage()
        {
            InitializeComponent();

            ObservableCollection<BBSData> bbsDataCollection = new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            BBSDataCollection.Source = bbsDataCollection;
            comboBoxBBS.SelectedValue = SharedData.CurrentBBS;

            ObservableCollection<TNCDevice> tncDeviceCollection = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList );
            DeviceListSource.Source = tncDeviceCollection;
            comboBoxTNCs.SelectedValue = SharedData.CurrentTNCDevice;

            CollectionOfSerialDevices = new ObservableCollection<SerialDevice>();

            listOfBaudRates = new ObservableCollection<uint>();
            for (uint i = 1200; i < 39000; i *= 2)
            {
                listOfBaudRates.Add(i);
            }
            BaudRateListSource.Source = listOfBaudRates;

            // data bits
            listOfDataBits = new ObservableCollection<ushort>() { 7, 8 };
            DataBitsListSource.Source = listOfDataBits;

            // Parity
            foreach (SerialParity item in Enum.GetValues(typeof(SerialParity)))
            {
                comboBoxParity.Items.Add(item);
            }

            foreach (SerialStopBitCount item in Enum.GetValues(typeof(SerialStopBitCount)))
            {
                comboBoxStopBits.Items.Add(item);
            }

            foreach (SerialHandshake item in Enum.GetValues(typeof(SerialHandshake)))
            {
                comboBoxFlowControl.Items.Add(item);
            }

            ProfilesCollection.Source = ProfileArray.Instance.ProfileList;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
        }

        private void SettingsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //switch ((SettingsPivot.SelectedItem as PivotItem).Name)
            //{
            //    case "pivotTNC":
            //        ResetTNCDeviceChanged();
            //        appBarSettingsSave.Visibility = Visibility.Visible;
            //        appBarSettingsSave.IsEnabled = false;
            //        appBarSettingsAdd.Visibility = Visibility.Visible;
            //        appBarSettingsEdit.Visibility = Visibility.Visible;
            //        appBarsettingsDelete.Visibility = Visibility.Visible;
            //        SettingsCommandBar.Visibility = Visibility.Visible;
            //        break;
            //    case "pivotItemAddressBook":
            //        ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
            //        appBarSettingsSave.Visibility = Visibility.Collapsed;
            //        SettingsCommandBar.Visibility = Visibility.Visible;
            //        break;
            //    case "pivotItemDistributionLists":
            //        //ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
            //        appBarSettingsSave.Visibility = Visibility.Visible;
            //        appBarSettingsSave.IsEnabled = DistributionListArray.Instance.DataChanged;
            //        appBarSettingsEdit.Visibility = Visibility.Visible;
            //        appBarsettingsDelete.Visibility = Visibility.Visible;
            //        SettingsCommandBar.Visibility = Visibility.Visible;
            //        break;
            //    default:
            //        SettingsCommandBar.Visibility = Visibility.Collapsed;
            //        break;
            //}
        }

        private void ComboBoxTacticalCallsignArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_tacticalCallsignData = (TacticalCallsignData)e.AddedItems[0];

            //_tacticalCallsignData.TacticalCallsignsChanged = false;
            //ViewModels.IdentityPartViewModel._tacticalCallsignData = _tacticalCallsignData;
            //if (_tacticalCallsignData.TacticalCallsigns != null)
            //{
            //    ObservableCollection<TacticalCall> listOfTacticallsigns = new ObservableCollection<TacticalCall>();
            //    foreach (var callsignData in _tacticalCallsignData.TacticalCallsigns.TacticalCallsignsArray)
            //    {
            //        listOfTacticallsigns.Add(callsignData);
            //    }
            //    TacticalCallsignsSource.Source = listOfTacticallsigns;
            //}
            //if (_tacticalCallsignData.AreaName == "Other")
            //{
            //    textBoxTacticalCallsign.Visibility = Visibility.Visible;
            //    comboBoxTacticalCallsign.Visibility = Visibility.Collapsed;
            //    comboBoxAdditionalText.SelectedItem = null;
            //}
            //else
            //{
            //    textBoxTacticalCallsign.Visibility = Visibility.Collapsed;
            //    comboBoxTacticalCallsign.Visibility = Visibility.Visible;
            //}
        }

        private void textBoxTacticalCallsign_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTacticalCallsign.Text.Length == 6)
                _identityViewModel.TacticalCallsignOther = textBoxTacticalCallsign.Text;
        }

        #region Profiles
        private bool _bbsChanged = false;
        private bool _tncChanged = false;
        private bool _defaultToChanged = false;

        private async Task ProfileSaveAsync()
        {
            //Profile newProfile = null;
            int index = comboBoxProfiles.SelectedIndex;
            Profile profile = SharedData.ProfileArray.Profiles[index];
            if (comboBoxProfiles.Visibility == Visibility.Collapsed)
            {
                //	newProfile = new Profile();

                profile.Name = textBoxNewProfileName.Text;
                //	newProfile.BBS = comboBoxBBS.SelectedValue as string;
                //	newProfile.TNC = comboBoxTNCs.SelectedValue as string;
                //	newProfile.SendTo = textBoxTo.Text;
                //	newProfile.Selected = true;

                //	ViewModels.SharedData._profileArray.Profiles.SetValue(newProfile, ViewModels.SharedData._profileArray.Profiles.Length - 1);
                comboBoxProfiles.Visibility = Visibility.Visible;
                textBoxNewProfileName.Visibility = Visibility.Collapsed;
            }
            //else
            //{
            //int index = comboBoxProfiles.SelectedIndex;
            //Profile profile = ViewModels.SharedData._profileArray.Profiles[index];

            profile.BBS = comboBoxBBS.SelectedValue as string;
            profile.TNC = comboBoxTNCs.SelectedValue as string;
            profile.SendTo = textBoxTo.Text;
            //profile.Selected = true;

            SharedData.ProfileArray.Profiles.SetValue(profile, index);
            //}

            await ProfileArray.Instance.SaveAsync();

            _profileCollection = new ObservableCollection<Profile>();
            foreach (Profile prof in SharedData.ProfileArray.Profiles)
            {
                _profileCollection.Add(prof);
            }
            ProfilesCollection.Source = _profileCollection;


            _bbsChanged = false;
            _tncChanged = false;
            _defaultToChanged = false;
            profileSave.IsEnabled = false;
        }

        private void ProfileSave_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ProfileSaveAsync();
        }

        private void ProfileSettingsAdd_Click(object sender, RoutedEventArgs e)
        {
            profileSave.IsEnabled = true;

            comboBoxProfiles.Visibility = Visibility.Collapsed;
            textBoxNewProfileName.Visibility = Visibility.Visible;

            var length = SharedData.ProfileArray.Profiles.Length;
            Profile[] tempProfileArray = new Profile[length + 1];
            SharedData.ProfileArray.Profiles.CopyTo(tempProfileArray, 0);
            Profile newProfile = new Profile()
            {
                BBS = comboBoxBBS.SelectedValue as string,
                TNC = comboBoxTNCs.SelectedValue as string,
                SendTo = textBoxTo.Text
            };
            tempProfileArray.SetValue(newProfile, length);
            SharedData.ProfileArray.Profiles = tempProfileArray;

            ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
            foreach (Profile profile in SharedData.ProfileArray.Profiles)
            {
                //profile.Selected = false;
                profileCollection.Add(profile);
            }
            //sharedData.ProfileArray.Profiles[length].Selected = true;
            ProfilesCollection.Source = profileCollection;
            comboBoxProfiles.SelectedIndex = length;
        }

        private void ProfileSettingsDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = comboBoxProfiles.SelectedIndex;
            var length = SharedData.ProfileArray.Profiles.Length;
            Profile[] tempProfileArray = new Profile[length - 1];

            ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
            for (int i = 0, j = 0; i < length; i++)
            {
                if (i != index)
                {
                    tempProfileArray.SetValue(SharedData.ProfileArray?.Profiles[i], j);
                    profileCollection.Add(SharedData.ProfileArray?.Profiles[i]);
                    j++;
                }
            }
            ProfilesCollection.Source = profileCollection;
            SharedData.ProfileArray.Profiles = tempProfileArray;

            comboBoxProfiles.SelectedIndex = Math.Max(index - 1, 0);
            profileSave.IsEnabled = true;
        }

        private void ComboBoxTNCs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedTNCDevice = (TNCDevice)e.AddedItems[0];
                if (string.IsNullOrEmpty(selectedTNCDevice.Prompts.Command))
                {
                    comboBoxBBS.SelectedIndex = -1;
                }
                if (SharedData.CurrentProfile.TNC != selectedTNCDevice.Name)
                {
                    _tncChanged = true;
                }
                else
                {
                    _tncChanged = false;
                }
                profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
                SharedData.CurrentTNCDevice = selectedTNCDevice;
            }
        }

        private void ComboBoxBBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedBBS = (BBSData)e.AddedItems[0];
                SharedData.CurrentBBS = selectedBBS;
                //ViewModels.SharedData._currentProfile.BBS = selectedBBS.Name;
                textBoxDescription.Text = selectedBBS.Description;
                textBoxFrequency1.Text = selectedBBS.Frequency1;
                textBoxFrequency2.Text = selectedBBS.Frequency2;
                if (SharedData.CurrentProfile.BBS != selectedBBS.Name)
                {
                    _bbsChanged = true;
                }
                else
                {
                    _bbsChanged = false;
                }
                profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
            }
            else
            {
                textBoxDescription.Text = "";
                textBoxFrequency1.Text = "";
                textBoxFrequency2.Text = "";
            }
        }

        private void TextBoxTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SharedData.CurrentProfile.SendTo != ((TextBox)sender).Text)
            {
                _defaultToChanged = true;
            }
            else
            {
                _defaultToChanged = false;
            }
            profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
        }

        private void ComboBoxProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (Profile profile in e.RemovedItems)
            //{
            //	profile.Selected = false;
            //}

            try
            {
                Profile profile = (Profile)((ComboBox)sender).SelectedItem;
                if (profile != null)
                {
                    //comboBoxTNCs.SelectedValuePath = "Name";
                    comboBoxTNCs.SelectedValue = profile.TNC;
                    //BBSSelectedValue = profile.BBS;
                    comboBoxBBS.SelectedValue = profile.BBS;
                    textBoxTo.Text = profile.SendTo;
                    //MessageTo = profile.SendTo;
                    SharedData.CurrentProfile = profile;
                    //profile.Selected = true;
                }
                _bbsChanged = false;
                _tncChanged = false;
                _defaultToChanged = false;

                profileSave.IsEnabled = false;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
        }

        #endregion
        enum TNCState
        {
            None,
            Edit,
            Add,
            Delete,
            EMail,
            EMailEdit,
            EMailDelete,
            EMailAdd
        }
        TNCState _tncState = TNCState.None;

        private void comboBoxStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBoxPromptsConnected_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_promptsConnectedChanged = SharedData.SavedTNCDevice.Prompts.Connected != (string)((TextBox)sender).Text;

            //_promptsChanged = _promptsCommandChanged | _promptsTimeoutChanged | _promptsConnectedChanged | _promptsDisconnectedChanged;
            //appBarSettingsSave.IsEnabled = _comportSettingsChanged | _initCommandsChanged
            //        | _commandsChanged | _promptsChanged;
        }

        private void UpdateTNCFromUI(TNCDevice tncDevice)
        {
            //if (_initCommandsChanged)
            //{
            //    tncDevice.InitCommands.Precommands = textBoxInitCommandsPre.Text;
            //    tncDevice.InitCommands.Postcommands = textBoxInitCommandsPost.Text;
            //}

            //if (_comportSettingsChanged)
            //{
            //    tncDevice.CommPort.IsBluetooth = toggleSwitchBluetooth.IsOn;
            //    if (tncDevice.CommPort.IsBluetooth)
            //    {
            //        tncDevice.CommPort.BluetoothName = comboBoxComName.SelectedValue as string;
            //        tncDevice.CommPort.DeviceId = ((DeviceInformation)comboBoxComName.SelectedItem)?.Id as string;
            //    }
            //    tncDevice.CommPort.Comport = comboBoxComPort.SelectedValue as string;
            //    tncDevice.CommPort.Baudrate = (uint)comboBoxBaudRate.SelectedValue;
            //    tncDevice.CommPort.Databits = (ushort)comboBoxDatabits.SelectedValue;
            //    tncDevice.CommPort.Stopbits = (SerialStopBitCount)comboBoxStopBits.SelectedValue;
            //    tncDevice.CommPort.Parity = (SerialParity)comboBoxParity.SelectedValue;
            //    tncDevice.CommPort.Flowcontrol = (SerialHandshake)comboBoxFlowControl.SelectedValue;
            //}

            //if (_promptsChanged)
            //{
            //    tncDevice.Prompts.Command = textBoxPrompsCommand.Text;
            //    tncDevice.Prompts.Timeout = textBoxPromptsTimeout.Text;
            //    tncDevice.Prompts.Connected = textBoxPromptsConnected.Text;
            //    tncDevice.Prompts.Disconnected = textBoxPromptsDisconnected.Text;
            //}

            //if (_commandsChanged)
            //{
            //    tncDevice.Commands.Connect = textBoxCommandsConnect.Text;
            //    tncDevice.Commands.Conversmode = textBoxCommandsConversMode.Text;
            //    tncDevice.Commands.MyCall = textBoxCommandsMyCall.Text;
            //    tncDevice.Commands.Retry = textBoxCommandsRetry.Text;
            //    tncDevice.Commands.Datetime = textBoxCommandsDateTime.Text;
            //}
        }

        private void TNCSaveAsCurrent()
        {
            if (_tncState == TNCState.Add)      // New setting have been created but not saved
                return;

            TNCDevice tncDevice = SharedData.CurrentTNCDevice;

            UpdateTNCFromUI(SharedData.CurrentTNCDevice);
        }

        private async void appBarSaveTNC_ClickAsync(object sender, RoutedEventArgs e)
        {
            TNCSaveAsCurrent();
            //for (int i = 0; i < SharedData.TncDeviceArray.TNCDevices.Length; i++)
            foreach (TNCDevice tncDevice in TNCDeviceArray.Instance.TNCDeviceList)
            {
                if (tncDevice.Name == SharedData.CurrentTNCDevice.Name)
                {
                    tncDevice.InitCommands.Precommands = SharedData.CurrentTNCDevice.InitCommands.Precommands;
                    tncDevice.InitCommands.Postcommands = SharedData.CurrentTNCDevice.InitCommands.Postcommands;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.IsBluetooth = SharedData.CurrentTNCDevice.CommPort.IsBluetooth;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.BluetoothName = SharedData.CurrentTNCDevice.CommPort.BluetoothName;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.DeviceId = SharedData.CurrentTNCDevice.CommPort.DeviceId;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Comport = SharedData.CurrentTNCDevice.CommPort.Comport;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Baudrate = SharedData.CurrentTNCDevice.CommPort.Baudrate;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Databits = SharedData.CurrentTNCDevice.CommPort.Databits;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Stopbits = SharedData.CurrentTNCDevice.CommPort.Stopbits;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Parity = SharedData.CurrentTNCDevice.CommPort.Parity;
//                    SharedData.TncDeviceArray.TNCDevices[i].CommPort.Flowcontrol = SharedData.CurrentTNCDevice.CommPort.Flowcontrol;
//                    SharedData.TncDeviceArray.TNCDevices[i].Prompts.Command = SharedData.CurrentTNCDevice.Prompts.Command;
//                    SharedData.TncDeviceArray.TNCDevices[i].Prompts.Timeout = SharedData.CurrentTNCDevice.Prompts.Timeout;
//                    SharedData.TncDeviceArray.TNCDevices[i].Prompts.Connected = SharedData.CurrentTNCDevice.Prompts.Connected;
//                    SharedData.TncDeviceArray.TNCDevices[i].Prompts.Disconnected = SharedData.CurrentTNCDevice.Prompts.Disconnected;
////                    SharedData.TncDeviceArray.TNCDevices[i].Commands.Connect = textBoxCommandsConnect.Text;
//                    SharedData.TncDeviceArray.TNCDevices[i].Commands.Conversmode = SharedData.CurrentTNCDevice.Commands.Conversmode;
//                    SharedData.TncDeviceArray.TNCDevices[i].Commands.MyCall = SharedData.CurrentTNCDevice.Commands.MyCall;
//                    SharedData.TncDeviceArray.TNCDevices[i].Commands.Retry = SharedData.CurrentTNCDevice.Commands.Retry;
//                    SharedData.TncDeviceArray.TNCDevices[i].Commands.Datetime = SharedData.CurrentTNCDevice.Commands.Datetime;

                    break;
                }
            }

            await SharedData.TncDeviceArray.SaveAsync();
            SharedData.SavedTNCDevice = new TNCDevice(SharedData.CurrentTNCDevice);
            appBarSaveTNC.IsEnabled = false;
        }

        private void SetMailControlsEnabledState(bool enabledState)
        {
            //mailPortString.IsEnabled = enabledState;
            //mailUserName.IsEnabled = enabledState;
            //mailPassword.IsEnabled = enabledState;
            //if (enabledState == true)
            //{
            //    mailIsSSL.Visibility = Visibility.Visible;
            //    mailServerComboBox.Visibility = Visibility.Collapsed;
            //    mailServer.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    mailIsSSL.Visibility = Visibility.Collapsed;
            //    mailServerComboBox.Visibility = Visibility.Visible;
            //    mailServer.Visibility = Visibility.Collapsed;
            //}
        }

        private void UpdateMailState(TNCState newMailState)
        {
            if (newMailState == _tncState)
                return;

            _tncState = newMailState;
            switch (newMailState)
            {
                case TNCState.EMail:
                    SetMailControlsEnabledState(false);
                    break;
                case TNCState.EMailEdit:
                    SetMailControlsEnabledState(true);
                    break;
                case TNCState.EMailDelete:
                    SetMailControlsEnabledState(false);
                    break;
                case TNCState.EMailAdd:
                    SetMailControlsEnabledState(true);
                    //mailServer.Text = "";
                    //mailPortString.Text = "0";
                    //mailUserName.Text = "";
                    //mailPassword.Password = "";
                    //mailIsSSL.IsOn = false;
                    break;
                case TNCState.None:
                    SetMailControlsEnabledState(false);
                    break;
            }
        }

        private void ConnectDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((PivotItem)PivotTNC.SelectedItem == null)
            //    return;

            if (e.AddedItems.Count > 0)
            {
                TNCDevice tncDevice = null;
                var TNCDevices = e.AddedItems;
                if (TNCDevices.Count == 1)
                {
                    tncDevice = (TNCDevice)TNCDevices[0];
                    SharedData.CurrentTNCDevice = tncDevice;
                    SharedData.SavedTNCDevice = new TNCDevice(tncDevice);
                    if (tncDevice.Name == "E-Mail")
                    {
                        UpdateMailState(TNCState.EMail);
//                        EMailSettings.Visibility = Visibility.Visible;
                        PivotTNC.Visibility = Visibility.Collapsed;
//                        mailServerComboBox.SelectedIndex = _mailSettingsViewModel.MailAccountSelectedIndex;
                    }
                    else
                    {
                        if (_tncState == TNCState.EMail)
                        {
                            _tncState = TNCState.None;
                        }
//                        EMailSettings.Visibility = Visibility.Collapsed;
                        PivotTNC.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    return;
                }
                textBoxInitCommandsPre.Text = tncDevice.InitCommands.Precommands;
                textBoxInitCommandsPost.Text = tncDevice.InitCommands.Postcommands;

                //SerialDevice serialDevice = null;
                //UseBluetooth = tncDevice.CommPort.IsBluetooth;
                //toggleSwitchBluetooth.IsOn = (bool)tncDevice.CommPort?.IsBluetooth;
                //SetComportComboBoxVisibility();

                //foreach (DeviceInformation deviceInfo in CollectionOfBluetoothDevices)
                //{
                //    if (deviceInfo.Name == tncDevice.CommPort.BluetoothName)
                //    {
                //        comboBoxComName.SelectedItem = deviceInfo;
                //        break;
                //    }
                //}
                foreach (SerialDevice device in CollectionOfSerialDevices)
                {
                    if (device.PortName == tncDevice.CommPort.Comport)
                    {
                        comboBoxComPort.SelectedItem = device;
                        break;
                    }
                }
                //if (serialDevice != null)
                {
                    //comboBoxComPort.SelectedItem = serialDevice;
                    comboBoxBaudRate.SelectedValue = tncDevice.CommPort.Baudrate;
                    comboBoxDatabits.SelectedValue = tncDevice.CommPort.Databits;

                    int i = 0;
                    var values = Enum.GetValues(typeof(SerialParity));
                    for (; i < values.Length; i++)
                    {
                        if ((SerialParity)values.GetValue(i) == tncDevice.CommPort.Parity) break;
                    }
                    comboBoxParity.SelectedIndex = i;
                    //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedParity = tncDevice.CommPort.Parity;

                    values = Enum.GetValues(typeof(SerialStopBitCount));
                    for (i = 0; i < values.Length; i++)
                    {
                        if ((SerialStopBitCount)values.GetValue(i) == tncDevice.CommPort.Stopbits) break;
                    }
                    comboBoxStopBits.SelectedIndex = i;

                    //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedStopBits = tncDevice.CommPort.Stopbits;
                    values = Enum.GetValues(typeof(SerialHandshake));
                    for (i = 0; i < values.Length; i++)
                    {
                        if ((SerialHandshake)values.GetValue(i) == tncDevice.CommPort.Flowcontrol)
                        {
                            break;
                        }
                    }
                    comboBoxFlowControl.SelectedIndex = i;
                }
                //else
                //{
                //	MessageDialog messageDialog = new MessageDialog("Com port not found. \nIs the TNC plugged in?");
                //	await messageDialog.ShowAsync();
                //}
                _TNCSettingsViewModel.TNCPromptsCommand = tncDevice.Prompts.Command;
                _TNCSettingsViewModel.TNCPromptsTimeout = tncDevice.Prompts.Timeout;
                //textBoxPrompsCommand.Text = tncDevice.Prompts.Command;
                //textBoxPromptsTimeout.Text = tncDevice.Prompts.Timeout;
                textBoxPromptsConnected.Text = tncDevice.Prompts.Connected;
                textBoxPromptsDisconnected.Text = tncDevice.Prompts.Disconnected;

                _TNCSettingsViewModel.TNCCommandsConnect = tncDevice.Commands.Connect;
                //textBoxCommandsConnect.Text = tncDevice.Commands.Connect;
                textBoxCommandsConversMode.Text = tncDevice.Commands.Conversmode;
                textBoxCommandsMyCall.Text = tncDevice.Commands.MyCall;
                textBoxCommandsRetry.Text = tncDevice.Commands.Retry;
                textBoxCommandsDateTime.Text = tncDevice.Commands.Datetime;
            }
        }

        private void textBoxPromptsDisconnected_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
