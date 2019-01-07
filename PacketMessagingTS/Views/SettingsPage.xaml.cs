﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.SerialCommunication;

using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using MetroLog;
using SharedCode;
using PacketMessagingTS.Controls;

namespace PacketMessagingTS.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<SettingsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public SettingsViewModel _settingsViewModel { get; } = Singleton<SettingsViewModel>.Instance;
        public IdentityViewModel _identityViewModel { get; } = Singleton<IdentityViewModel>.Instance;
        public PacketSettingsViewModel _packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;
        public TNCSettingsViewModel _TNCSettingsViewModel { get; } = Singleton<TNCSettingsViewModel>.Instance;
        public AddressBookViewModel _addressBookViewModel { get; } = new AddressBookViewModel();

        //private SuspendingEventHandler appSuspendEventHandler;
        //private EventHandler<Object> appResumeEventHandler;


        // Identity settings
        public static ObservableCollection<TacticalCallsignData> listOfTacticallsignsArea;

        //TacticalCallsignData _tacticalCallsignData;

        // Profiles settings
        

        public SettingsPage()
        {
            InitializeComponent();

            ObservableCollection<TNCDevice> tncDeviceCollection = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList );
            TNCDeviceListSource.Source = tncDeviceCollection;

            // Serial ports
    //        _TNCSettingsViewModel.CollectionOfSerialDevices = new ObservableCollection<string>();
            //_listOfBluetoothDevices = new List<DeviceInformation>();
            //CollectionOfBluetoothDevices = new ObservableCollection<DeviceInformation>();
            //_comportComparer = new ComportComparer();
            //_listOfDevices = new ObservableCollection<DeviceListEntry>();

            //mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, String>();
            //_watchersStarted = false;
            //_watchersSuspended = false;

            //_isAllDevicesEnumerated = false;

            //_packetSettingsViewModel.ObservableProfileCollection = new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);

            //ObservableCollection<EmailAccount> EmailAccountsObservableCollection = new ObservableCollection<EmailAccount>();
            //foreach (EmailAccount account in EmailAccountArray.Instance.EmailAccounts)
            //{
            //    EmailAccountsObservableCollection.Add(account);
            //}
            ////EmailAccountsSource.Source = EmailAccountsObservableCollection;
            EmailAccountsSource.Source = EmailAccountArray.Instance.EmailAccounts;

            ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();

            // Identity initialization
            //_identityViewModel.TacticalCallsignsAreaSource = new ObservableCollection<TacticalCallsignData>(App._TacticalCallsignDataList);

            // Distribution Lists Initialization
            distributionListName.ItemsSource = DistributionListArray.Instance.GetDistributionLists();
            if (DistributionListArray.Instance.ArrayOfDistributionLists != null && DistributionListArray.Instance.ArrayOfDistributionLists.Length > 0)
            {
                distributionListName.Text = DistributionListArray.Instance.ArrayOfDistributionLists[0].DistributionListName;
                distributionListItems.Text = DistributionListArray.Instance.ArrayOfDistributionLists[0].DistributionListItems;
            }
            appBarDistributionListsSave.IsEnabled = false;
            distributionListAddItem.IsEnabled = false;
            distributionListItems.IsReadOnly = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _settingsViewModel.Initialize();

            // Begin watching out for events
            //StartHandlingAppEvents();

            // Initialize the desired device watchers so that we can watch for when devices are connected/removed
            //InitializeDeviceWatchers();
            //StartDeviceWatchers();

            if (e.Parameter is null)
            {
                return;
            }
            else
            {
                SettingsPivot.SelectedIndex = (int)e.Parameter;
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //StopDeviceWatchers();
            //StopHandlingAppEvents();

            if (_TNCSettingsViewModel.IsAppBarSaveEnabled)
            {
                bool save = await Utilities.ShowYesNoMessageDialogAsync("Save changes?");
                if (save)
                {
                    appBarSaveTNC_ClickAsync(this, null);
                }
                // Disable Save button
                _TNCSettingsViewModel.ResetChangedProperty();
            }
            if (_packetSettingsViewModel.IsAppBarSaveEnabled)
            {
                bool save = await Utilities.ShowYesNoMessageDialogAsync("Save changes?");
                if (save)
                {
                    PacketSettingsSave_ClickAsync(this, null);
                }
                // Disable Save button
                _packetSettingsViewModel.ResetChangedProperty();
            }

            base.OnNavigatingFrom(e);
        }
        private void SettingsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((SettingsPivot.SelectedItem as PivotItem).Name)
            {
                case "pivotTNC":
                    _TNCSettingsViewModel.TNCDeviceSelectedIndex = Utilities.GetProperty("TNCDeviceSelectedIndex");
                    // Select current TNC device
                    break;
                case "pivotPacketSettings":
                    comboBoxProfiles.Visibility = Visibility.Visible;
                    textBoxNewProfileName.Visibility = Visibility.Collapsed;

                    _packetSettingsViewModel.ProfileSelectedIndex = Utilities.GetProperty("ProfileSelectedIndex");
                    break;
                case "pivotIdentity":
                    //if (_identityViewModel.TacticalCallsignSelectedIndex == -1)
                    //{
                    //    //_identityViewModel.TacticalCallsign = _identityViewModel.TacticalCallsignOther;
                    //    comboBoxTacticalCallsign.Text = _identityViewModel.TacticalCallsignOther;
                    //}
                    //_identityViewModel.TacticalCallsignAreaSelectedIndex = Utilities.GetProperty("TacticalCallsignAreaSelectedIndex");
                    break;
                    //    case "pivotItemAddressBook":
                    //        ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
                    //        break;
                    //    case "pivotItemDistributionLists":
                    //        //ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
                    //        break;
                    //    default:
                    //        SettingsCommandBar.Visibility = Visibility.Collapsed;
                    //        break;
            }
            // Disable Save button
            //_TNCSettingsViewModel.ResetChangedProperty();
        }
#region General
        //private void BBSPrimaryStatus_Toggled(object sender, RoutedEventArgs e)
        //{
        //    ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
        //}

#endregion General
#region Identity
        //private void ComboBoxTacticalCallsignArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
            //_identityViewModel._tacticalCallsignData = (TacticalCallsignData)e.AddedItems[0];

            ////_identityViewModel._tacticalCallsignData.TacticalCallsignsChanged = false;
            //if (_identityViewModel._tacticalCallsignData.TacticalCallsigns != null)
            //{
            //    ObservableCollection<TacticalCall> listOfTacticallsigns = new ObservableCollection<TacticalCall>(_identityViewModel._tacticalCallsignData.TacticalCallsigns.TacticalCallsignsArray);
            //    _identityViewModel.TacticalCallsignsSource = listOfTacticallsigns;
            //}  TacticalCallsignData> TacticalCallsignData> TacticalCallsignsAreaSource
            //if (_identityViewModel.TacticalCallsignsAreaSource[_identityViewModel.TacticalCallsignAreaSelectedIndex].AreaName == "Other")
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
        //}

        private void ComboBoxTacticalCallsign_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            return;
            //_identityViewModel.TacticalCallsignSelectedIndex = -1;
            if (sender.Text.Length == 6)
            {

                //_identityViewModel.TacticalCallsign = sender.Text;
                TacticalCall tacticalCall = new TacticalCall()
                {
                    TacticalCallsign = sender.Text,
                    AgencyName = "",
                    Prefix = sender.Text.Substring(sender.Text.Length - 3),
                };
                _identityViewModel.TacticalCallsignsSource.Add(tacticalCall);
                //_identityViewModel.TacticalSelectedIndexArray[_identityViewModel.TacticalCallsignAreaSelectedIndex] = _identityViewModel.TacticalCallsignsSource.IndexOf(tacticalCall);
                //_identityViewModel.TacticalSelectedIndexArray = _identityViewModel.TacticalSelectedIndexArray;
                //_identityViewModel.TacticalCallsignOther = sender.Text;
            }
        }

        private void textBoxTacticalCallsign_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
            if (((ComboBox)sender).Text.Length == 6)
            {
                _identityViewModel.TacticalCallsign = ((ComboBox)sender).Text;
                TacticalCall tacticalCall = new TacticalCall()
                {
                    TacticalCallsign = ((ComboBox)sender).Text,
                    AgencyName = "",
                    Prefix = ((ComboBox)sender).Text.Substring(((ComboBox)sender).Text.Length - 3),
                };
                _identityViewModel.TacticalCallsignsSource.Add(tacticalCall);
                _identityViewModel.TacticalCallsignOther = ((ComboBox)sender).Text;
            }
        }
#endregion Identity
#region Profiles
        //private bool _bbsChanged = false;
        //private bool _tncChanged = false;
        //private bool _defaultToChanged = false;

        private async Task ProfileSaveAsync()
        {
            Profile profile = comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile;
            //int index = comboBoxProfiles.SelectedIndex;
            //Profile profile = ProfileArray.Instance.ProfileList.[index];
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

            //ProfileArray.Instance.ProfileList[ProfileArray.Instance.ProfileList.IndexOf(profile)] = profile;
            ProfileArray.Instance.ProfileList[comboBoxProfiles.SelectedIndex] = profile;
            //SharedData.ProfileArray.Profiles.SetValue(profile, index);
            //}

            await ProfileArray.Instance.SaveAsync();

            //_profileCollection = new ObservableCollection<Profile>();
            //foreach (Profile prof in ProfileArray.Instance.ProfileList)
            //{
            //    _profileCollection.Add(prof);
            //}
            //_packetSettingsViewModel.ObservableProfileCollection = new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);


            //_bbsChanged = false;
            //_tncChanged = false;
            //_defaultToChanged = false;
            //profileSave.IsEnabled = false;
        }

        private async void PacketSettingsSave_ClickAsync(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            int index = comboBoxProfiles.SelectedIndex;
            if (comboBoxProfiles.Visibility == Visibility.Collapsed)
            {
                Profile newProfile = new Profile()
                {
                    Name = textBoxNewProfileName.Text,
                	BBS = comboBoxBBS.SelectedValue as string,
                	TNC = comboBoxTNCs.SelectedValue as string,
                	SendTo = textBoxTo.Text,
                };
                if (newProfile.TNC.Contains(SharedData.EMail))
                {
                    comboBoxBBS.SelectedIndex = -1;
                    newProfile.BBS = "";
                }
                ProfileArray.Instance.ProfileList.Add(newProfile);
                await ProfileArray.Instance.SaveAsync();
                comboBoxProfiles.Visibility = Visibility.Visible;
                textBoxNewProfileName.Visibility = Visibility.Collapsed;
                index = ProfileArray.Instance.ProfileList.Count - 1;
            }
            else
            {
                Profile profile = comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile;

                profile.BBS = comboBoxBBS.SelectedValue as string;
                profile.TNC = comboBoxTNCs.SelectedValue as string;
                profile.SendTo = textBoxTo.Text;

                //ProfileArray.Instance.ProfileList[ProfileArray.Instance.ProfileList.IndexOf(profile)] = profile;
                ProfileArray.Instance.ProfileList[comboBoxProfiles.SelectedIndex] = profile;
            }

            await ProfileArray.Instance.SaveAsync();

            _packetSettingsViewModel.ResetChangedProperty();
            //comboBoxProfiles.SelectedIndex = index;

        }

        private void ProfileSettingsAdd_Click(object sender, RoutedEventArgs e)
        {
            profileSave.IsEnabled = true;

            comboBoxProfiles.Visibility = Visibility.Collapsed;
            textBoxNewProfileName.Visibility = Visibility.Visible;

            //Profile newProfile = new Profile()
            //{
            //    BBS = comboBoxBBS.SelectedValue as string,
            //    TNC = comboBoxTNCs.SelectedValue as string,
            //    SendTo = textBoxTo.Text
            //};
            //ProfileArray.Instance.ProfileList.Add(newProfile);

            //ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
            //foreach (Profile profile in ProfileArray.Instance.ProfileList)
            //{
            //    //profile.Selected = false;
            //    profileCollection.Add(profile);
            //}
            ////sharedData.ProfileArray.Profiles[length].Selected = true;
            //ProfilesCollection.Source = profileCollection;
            //comboBoxProfiles.SelectedIndex = ProfileArray.Instance.ProfileList.Count - 1;
        }

        private void ProfileSettingsDelete_Click(object sender, RoutedEventArgs e)
        {
            int index = comboBoxProfiles.SelectedIndex;

            Profile profile = comboBoxProfiles.SelectedItem as Profile;
            ProfileArray.Instance.ProfileList.Remove(profile);
            //_packetSettingsViewModel.ObservableProfileCollection = new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);

            //int index = comboBoxProfiles.SelectedIndex;
            //var length = SharedData.ProfileArray.Profiles.Length;
            //Profile[] tempProfileArray = new Profile[length - 1];

            //ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
            //for (int i = 0, j = 0; i < length; i++)
            //{
            //    if (i != index)
            //    {
            //        tempProfileArray.SetValue(SharedData.ProfileArray?.Profiles[i], j);
            //        profileCollection.Add(SharedData.ProfileArray?.Profiles[i]);
            //        j++;
            //    }
            //}
            //ProfilesCollection.Source = profileCollection;
            //SharedData.ProfileArray.Profiles = tempProfileArray;

            comboBoxProfiles.SelectedIndex = Math.Max(index - 1, 0);
            profileSave.IsEnabled = true;
        }

#endregion
#region Interface
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

            //TNCDevice tncDevice = SharedData.CurrentTNCDevice;

            UpdateTNCFromUI(Singleton<PacketSettingsViewModel>.Instance.CurrentTNC);
        }

        //private async void appBarSaveTNC_ClickAsync(object sender, RoutedEventArgs e)
        //{
        //    TNCSaveAsCurrent();
        //    foreach (TNCDevice tncDevice in TNCDeviceArray.Instance.TNCDeviceList)
        //    {
        //        if (tncDevice.Name == SharedData.CurrentTNCDevice.Name)
        //        {
        //            tncDevice.InitCommands.Precommands = SharedData.CurrentTNCDevice.InitCommands.Precommands;
        //            tncDevice.InitCommands.Postcommands = SharedData.CurrentTNCDevice.InitCommands.Postcommands;
        //            tncDevice.CommPort.IsBluetooth = SharedData.CurrentTNCDevice.CommPort.IsBluetooth;
        //            tncDevice.CommPort.BluetoothName = SharedData.CurrentTNCDevice.CommPort.BluetoothName;
        //            tncDevice.CommPort.DeviceId = SharedData.CurrentTNCDevice.CommPort.DeviceId;
        //            tncDevice.CommPort.Comport = SharedData.CurrentTNCDevice.CommPort.Comport;
        //            tncDevice.CommPort.Baudrate = SharedData.CurrentTNCDevice.CommPort.Baudrate;
        //            tncDevice.CommPort.Databits = SharedData.CurrentTNCDevice.CommPort.Databits;
        //            tncDevice.CommPort.Stopbits = SharedData.CurrentTNCDevice.CommPort.Stopbits;
        //            tncDevice.CommPort.Parity = SharedData.CurrentTNCDevice.CommPort.Parity;
        //            tncDevice.CommPort.Flowcontrol = SharedData.CurrentTNCDevice.CommPort.Flowcontrol;
        //            tncDevice.Prompts.Command = SharedData.CurrentTNCDevice.Prompts.Command;
        //            tncDevice.Prompts.Timeout = SharedData.CurrentTNCDevice.Prompts.Timeout;
        //            tncDevice.Prompts.Connected = SharedData.CurrentTNCDevice.Prompts.Connected;
        //            tncDevice.Prompts.Disconnected = SharedData.CurrentTNCDevice.Prompts.Disconnected;
        //            tncDevice.Commands.Connect = textBoxCommandsConnect.Text;
        //            tncDevice.Commands.Conversmode = SharedData.CurrentTNCDevice.Commands.Conversmode;
        //            tncDevice.Commands.MyCall = SharedData.CurrentTNCDevice.Commands.MyCall;
        //            tncDevice.Commands.Retry = SharedData.CurrentTNCDevice.Commands.Retry;
        //            tncDevice.Commands.Datetime = SharedData.CurrentTNCDevice.Commands.Datetime;
        //            break;
        //        }
        //    }

        //    await TNCDeviceArray.Instance.SaveAsync();
        //    SharedData.SavedTNCDevice = new TNCDevice(SharedData.CurrentTNCDevice);
        //    appBarSaveTNC.IsEnabled = false;
        //}

        private void SetMailControlsEditState(bool enabledState)
        {
            mailPortString.IsEnabled = enabledState;
            mailUserName.IsEnabled = enabledState;
            mailPassword.IsEnabled = enabledState;
            if (enabledState)
            {
                mailIsSSL.Visibility = Visibility.Visible;
                mailServerComboBox.Visibility = Visibility.Visible;
                mailServerComboBox.IsEnabled = false;
                //mailServer.Visibility = Visibility.Visible;
            }
            else
            {
                mailIsSSL.Visibility = Visibility.Collapsed;
                mailServerComboBox.Visibility = Visibility.Visible;
                mailServer.Visibility = Visibility.Collapsed;
            }
        }

        private void SetMailControlsEnabledState(bool enabledState)
        {
            mailPortString.IsEnabled = enabledState;
            mailUserName.IsEnabled = enabledState;
            mailPassword.IsEnabled = enabledState;
            if (enabledState)
            {
                mailIsSSL.Visibility = Visibility.Visible;
                mailServerComboBox.Visibility = Visibility.Collapsed;
                mailServer.Visibility = Visibility.Visible;
            }
            else
            {
                mailIsSSL.Visibility = Visibility.Collapsed;
                mailServerComboBox.Visibility = Visibility.Visible;
                mailServerComboBox.IsEnabled = true;
                mailServer.Visibility = Visibility.Collapsed;
            }
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
                    //SetMailControlsEnabledState(true);
                    SetMailControlsEditState(true);
                    break;
                case TNCState.EMailDelete:
                    SetMailControlsEnabledState(false);
                    break;
                case TNCState.EMailAdd:
                    SetMailControlsEnabledState(true);
                    mailServer.Text = "";
                    mailPortString.Text = "0";
                    mailUserName.Text = "";
                    mailPassword.Password = "";
                    mailIsSSL.IsOn = false;
                    break;
                case TNCState.None:
                    SetMailControlsEnabledState(false);
                    break;
            }
        }

        //private void SetComportComboBoxVisibility()
        //{
        //    if (toggleSwitchBluetooth.IsOn)
        //    {
        //        comboBoxComName.Visibility = Visibility.Visible;
        //        comboBoxComPort.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        comboBoxComName.Visibility = Visibility.Collapsed;
        //        comboBoxComPort.Visibility = Visibility.Visible;
        //    }
        //}

        private void NewTNCDevice()
        {
            ConnectDevices.Visibility = Visibility.Collapsed;
            newTNCDeviceName.Visibility = Visibility.Visible;

            textBoxInitCommandsPre.Text = "";
            textBoxInitCommandsPost.Text = "";

            toggleSwitchBluetooth.IsOn = false;
            //SetComportComboBoxVisibility();

            //if (CollectionOfBluetoothDevices.Count > 0)
            //{
            //    comboBoxComName.SelectedItem = CollectionOfBluetoothDevices[0];
            //}

            if (_TNCSettingsViewModel.CollectionOfSerialDevices.Count > 0)
            {
                comboBoxComPort.SelectedItem = _TNCSettingsViewModel.CollectionOfSerialDevices[0];
            }

            comboBoxBaudRate.SelectedItem = 9600;
            comboBoxDatabits.SelectedItem = 8;


            int i = 0;
            var values = Enum.GetValues(typeof(SerialParity));
            for (; i < values.Length; i++)
            {
                if ((SerialParity)values.GetValue(i) == SerialParity.None) break;
            }
            comboBoxParity.SelectedIndex = i;

            values = Enum.GetValues(typeof(SerialStopBitCount));
            for (i = 0; i < values.Length; i++)
            {
                if ((SerialStopBitCount)values.GetValue(i) == SerialStopBitCount.One) break;
            }
            comboBoxStopBits.SelectedIndex = i;

            values = Enum.GetValues(typeof(SerialHandshake));
            for (i = 0; i < values.Length; i++)
            {
                if ((SerialHandshake)values.GetValue(i) == SerialHandshake.RequestToSend)
                {
                    break;
                }
            }
            comboBoxFlowControl.SelectedIndex = i;

            textBoxPrompsCommand.Text = "";
            textBoxPromptsTimeout.Text = "";
            textBoxPromptsConnected.Text = "";
            textBoxPromptsDisconnected.Text = "";

            textBoxCommandsConnect.Text = "";
            textBoxCommandsConversMode.Text = "";
            textBoxCommandsMyCall.Text = "";
            textBoxCommandsRetry.Text = "";
            textBoxCommandsDateTime.Text = "";
        }

        private void ConnectDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((PivotItem)PivotTNC.SelectedItem is null)
            //    return;

            if (e.AddedItems.Count > 0)
            {
                TNCDevice tncDevice = null;
                var TNCDevices = e.AddedItems;
                if (TNCDevices != null && TNCDevices.Count == 1)
                {
                    tncDevice = (TNCDevice)TNCDevices[0];
                    Singleton<PacketSettingsViewModel>.Instance.CurrentTNC = tncDevice;
                    if (tncDevice.Name.Contains(SharedData.EMail))
                    {
                        UpdateMailState(TNCState.EMail);
                        EMailSettings.Visibility = Visibility.Visible;
                        PivotTNC.Visibility = Visibility.Collapsed;
                        mailServerComboBox.SelectedIndex = _TNCSettingsViewModel.MailAccountSelectedIndex;
                    }
                    else
                    {
                        if (_tncState == TNCState.EMail)
                        {
                            _tncState = TNCState.None;
                        }
                        EMailSettings.Visibility = Visibility.Collapsed;
                        PivotTNC.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    return;
                }

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
                //foreach (SerialDevice device in CollectionOfSerialDevices)
                //{
                //    if (device.PortName == tncDevice.CommPort.Comport)
                //    {
                //        comboBoxComPort.SelectedItem = device;
                //        break;
                //    }
                //}
                //if (serialDevice != null)
                {
                    //comboBoxComPort.SelectedItem = serialDevice;
                    //comboBoxBaudRate.SelectedValue = tncDevice.CommPort.Baudrate;
                    //comboBoxDatabits.SelectedValue = tncDevice.CommPort.Databits;

                    //int i = 0;
                    //var values = Enum.GetValues(typeof(SerialParity));
                    //for (; i < values.Length; i++)
                    //{
                    //    if ((SerialParity)values.GetValue(i) == tncDevice.CommPort.Parity) break;
                    //}
                    //comboBoxParity.SelectedIndex = i;
                    //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedParity = tncDevice.CommPort.Parity;

                    //values = Enum.GetValues(typeof(SerialStopBitCount));
                    //for (i = 0; i < values.Length; i++)
                    //{
                    //    if ((SerialStopBitCount)values.GetValue(i) == tncDevice.CommPort.Stopbits) break;
                    //}
                    //comboBoxStopBits.SelectedIndex = i;

                    //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedStopBits = tncDevice.CommPort.Stopbits;
                    //values = Enum.GetValues(typeof(SerialHandshake));
                    //for (i = 0; i < values.Length; i++)
                    //{
                    //    if ((SerialHandshake)values.GetValue(i) == tncDevice.CommPort.Flowcontrol)
                    //    {
                    //        break;
                    //    }
                    //}
                    //comboBoxFlowControl.SelectedIndex = i;
                }
                //else
                //{
                //	MessageDialog messageDialog = new MessageDialog("Com port not found. \nIs the TNC plugged in?");
                //	await messageDialog.ShowAsync();
                //}
                //_TNCSettingsViewModel.TNCPromptsCommand = tncDevice.Prompts.Command;
                //_TNCSettingsViewModel.TNCPromptsTimeout = tncDevice.Prompts.Timeout;
                //textBoxPrompsCommand.Text = tncDevice.Prompts.Command;
                //textBoxPromptsTimeout.Text = tncDevice.Prompts.Timeout;
                //textBoxPromptsConnected.Text = tncDevice.Prompts.Connected;
                //textBoxPromptsDisconnected.Text = tncDevice.Prompts.Disconnected;

                //_TNCSettingsViewModel.TNCCommandsConnect = tncDevice.Commands.Connect;
                //textBoxCommandsConnect.Text = tncDevice.Commands.Connect;
                //textBoxCommandsConversMode.Text = tncDevice.Commands.Conversmode;
                //textBoxCommandsMyCall.Text = tncDevice.Commands.MyCall;
                //textBoxCommandsRetry.Text = tncDevice.Commands.Retry;
                //textBoxCommandsDateTime.Text = tncDevice.Commands.Datetime;
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch)
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch.IsHitTestVisible && !(toggleSwitch.FocusState == FocusState.Unfocused))
                {
                    AddressBook addressBook = AddressBook.Instance;

                    Grid parent = toggleSwitch.Parent as Grid;
                    string callsign = (parent.Children[1] as TextBlock).Text;
                    if (string.IsNullOrEmpty(callsign))
                        return;

                    addressBook.UpdateAddressBookEntry(callsign, toggleSwitch.IsOn);
                    ContactsCVS.Source = addressBook.GetContactsGrouped();
                }
            }
        }

        private void UpdateEMailAccountFromUI(ref EmailAccount emailAccount)
        {
            emailAccount.MailServer = mailServer.Text;
            emailAccount.MailServerPort = Convert.ToUInt16(mailPortString.Text);
            emailAccount.MailUserName = mailUserName.Text;
            emailAccount.MailPassword = mailPassword.Password;
            emailAccount.MailIsSSLField = mailIsSSL.IsOn;
        }

        //private void MailServerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        private void MailServer_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //if (SettingsPageViewModel.TNCPartViewModel.CurrentMailAccount.MailServer != (string)((AutoSuggestBox)sender).Text)
            //{
            //    _emailMailServerChanged = true;
            //}
            //else
            //{
            //    _emailMailServerChanged = false;
            //}
            //appBarSettingsSave.IsEnabled = _emailMailServerChanged | _emailMailServerPortChanged
            //    | _emailMailUserNameChanged | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

        private void MailServer_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //sender.Text = args.SelectedItem.ToString();
            //TNCPartViewModel viewModel = SettingsPageViewModel.TNCPartViewModel;
            UpdateMailState(TNCState.EMail);
            _TNCSettingsViewModel.CurrentMailAccount = args.SelectedItem as EmailAccount;
            // Set comboBox selection
            for (int i = 0; i < EmailAccountArray.Instance.EmailAccounts.Length; i++)
            {
                if (_TNCSettingsViewModel.MailServer == EmailAccountArray.Instance.EmailAccounts[i].MailServer)
                {
                    mailServerComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void MailPortString_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (SettingsPageViewModel.TNCPartViewModel.CurrentMailAccount.MailServerPort.ToString() != (string)((TextBox)sender).Text)
            //{
            //    _emailMailServerPortChanged = true;
            //}
            //else
            //{
            //    _emailMailServerPortChanged = false;
            //}
            //appBarSettingsSave.IsEnabled = _emailMailServerChanged | _emailMailServerPortChanged
            //    | _emailMailUserNameChanged | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

        private void MailUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Update TNC mail name
            var eMailTNC = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name.Contains(SharedData.EMail)).FirstOrDefault();
            eMailTNC.MailUserName = ((TextBox)sender).Text;
            eMailTNC.Name = $"E-Mail-{eMailTNC.MailUserName}";
            //await TNCDeviceArray.Instance.SaveAsync();

            TNCDeviceListSource.Source = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        }

        //private void MailPassword_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //}

        //private void MailIsSSL_Toggled(object sender, RoutedEventArgs e)
        //{
        //}

        private void AppBarAddTNC_Clicked(object sender, RoutedEventArgs e)
        {
            if (_tncState == TNCState.EMail || _tncState == TNCState.EMailEdit)
            {
                UpdateMailState(TNCState.EMailAdd);
            }
            else if (_tncState != TNCState.EMail || _tncState != TNCState.EMailEdit || _tncState != TNCState.EMailAdd)
            {
                _tncState = TNCState.Add;
                NewTNCDevice();
            }

        }

        private void AppBarEditTNC_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_tncState == TNCState.EMail)
            {
                UpdateMailState(TNCState.EMailEdit);
            }
        }

        private async void appBarDeleteTNC_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_tncState == TNCState.EMail)
            {
                int index = mailServerComboBox.SelectedIndex;
                EmailAccount emailAccount = _TNCSettingsViewModel.CurrentMailAccount;
                List<EmailAccount> emailAccountsList = EmailAccountArray.Instance.EmailAccounts.ToList();
                bool success = emailAccountsList.Remove(emailAccount);
                EmailAccountArray.Instance.EmailAccounts = emailAccountsList.ToArray();

                if (!success)
                {
                    return;
                }

                EmailAccountsSource.Source = EmailAccountArray.Instance.EmailAccounts;

                mailServerComboBox.SelectedIndex = Math.Max(0, index - 1);
                await EmailAccountArray.Instance.SaveAsync();
                UpdateMailState(TNCState.EMail);
            }

        }

        private async void appBarSaveTNC_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_tncState == TNCState.EMail)
            {
                // Server changed
                EmailAccount emailAccount = _TNCSettingsViewModel.CurrentMailAccount;
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[_TNCSettingsViewModel.TNCDeviceSelectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;
                tncDevice.Name = $"{SharedData.EMail}-" + emailAccount.MailUserName;
                await TNCDeviceArray.Instance.SaveAsync();

                //TNCDeviceListSource.Source = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
                TNCDeviceListSource.Source = TNCDeviceArray.Instance.TNCDeviceList;
            }
            else if (_tncState == TNCState.EMailDelete)
            {
                EmailAccountArray.Instance.EmailAccountList.Remove(_TNCSettingsViewModel.CurrentMailAccount);
                await EmailAccountArray.Instance.SaveAsync();
                UpdateMailState(TNCState.EMail);
            }
            else if (_tncState == TNCState.EMailEdit)
            {
                EmailAccount emailAccount = _TNCSettingsViewModel.CurrentMailAccount;
                UpdateEMailAccountFromUI(ref emailAccount);
                for (int i = 0; i < EmailAccountArray.Instance.EmailAccounts.Length; i++)
                {
                    if (EmailAccountArray.Instance.EmailAccounts[i].MailServer == emailAccount.MailServer)
                    {
                        EmailAccountArray.Instance.EmailAccounts[i] = emailAccount;
                        break;
                    }
                }
                await EmailAccountArray.Instance.SaveAsync();
                TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[_TNCSettingsViewModel.TNCDeviceSelectedIndex];
                tncDevice.MailUserName = emailAccount.MailUserName;
                tncDevice.Name += "-" + emailAccount.MailUserName;
                await TNCDeviceArray.Instance.SaveAsync();
                UpdateMailState(TNCState.EMail);
            }
            else if (_tncState == TNCState.EMailAdd)
            {
                EmailAccount emailAccount = new EmailAccount();
                UpdateEMailAccountFromUI(ref emailAccount);
                List<EmailAccount> emailAccountList = EmailAccountArray.Instance.EmailAccounts.ToList();
                emailAccountList.Add(emailAccount);
                EmailAccountArray.Instance.EmailAccounts = emailAccountList.ToArray();
                await EmailAccountArray.Instance.SaveAsync();
                UpdateMailState(TNCState.EMail);

                //ObservableCollection<EmailAccount> EmailAccountsObservableCollection = new ObservableCollection<EmailAccount>();
                //foreach (EmailAccount account in EmailAccountArray.Instance.EmailAccounts)
                //{
                //    EmailAccountsObservableCollection.Add(account);
                //}
                //int selectedIndex = SettingsPageViewModel.TNCPartViewModel.MailAccountSelectedIndex;
                EmailAccountsSource.Source = EmailAccountArray.Instance.EmailAccounts;
                mailServerComboBox.SelectedIndex = EmailAccountArray.Instance.EmailAccounts.Length - 1;
            }
            else if (_tncState == TNCState.Add)
            {
                string tncDeviceName = newTNCDeviceName.Text;
                if (string.IsNullOrEmpty(tncDeviceName))
                {
                    //await Utilities.ShowMessageDialogAsync("The new TNC Device must have a name.", "Add TNC Device error");
                    return;
                }
                TNCDevice tncDevice = new TNCDevice();
                tncDevice.Name = tncDeviceName;
                UpdateTNCFromUI(tncDevice);       // Fill the new TNCDevice with data
                                                  // Add to existing array
                TNCDeviceArray.Instance.TNCDeviceList.Add(tncDevice);
                await TNCDeviceArray.Instance.SaveAsync();
                _tncState = TNCState.None;
            }
            ConnectDevices.Visibility = Visibility.Visible;
            newTNCDeviceName.Visibility = Visibility.Collapsed;

            // Disable Save button
            _TNCSettingsViewModel.ResetChangedProperty();
        }
        #endregion Interface
        #region Address Book
        AddressBookEntry _selectedEntry;
        private void AddressBookListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _selectedEntry = e.AddedItems[0] as AddressBookEntry;
            }
        }

        private async void AddressBookAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            AddressBook addressBook = AddressBook.Instance;

            ContentDialogAddressBookEntry contentDialog = new ContentDialogAddressBookEntry();
            AddressBookEntry emptyEntry = new AddressBookEntry()
            {
                Callsign = "",
                NameDetail = "",
                BBSPrimary = "",
                BBSSecondary = "",
                BBSPrimaryActive = true
            };
            //SetAddressBookEntryEditData(emptyEntry);
            contentDialog.Title = "Add Address Book Entry";
            //contentDialog.PrimaryButtonText = "Add";
            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                emptyEntry.Callsign = contentDialog.AddressBookCallsign;
                emptyEntry.NameDetail = contentDialog.AddressBookName;
                emptyEntry.BBSPrimary = contentDialog.SelectedPrimaryBBS;
                emptyEntry.BBSSecondary = contentDialog.SelectedSecondaryBBS;
                bool success = addressBook.AddAddressAsync(emptyEntry);
                if (!success)
                {
                    await Utilities.ShowMessageDialogAsync("Error adding a new address book entry.");
                }
                ContactsCVS.Source = addressBook.GetContactsGrouped();
                addressBookSave.IsEnabled = true;
            }
            else
            {
                addressBookSave.IsEnabled = false;
            }
        }

        private void AddressBookDelete_Click(object sender, RoutedEventArgs e)
        {
            AddressBook addressBook = AddressBook.Instance;

            addressBook.DeleteAddress(_selectedEntry);

            ContactsCVS.Source = addressBook.GetContactsGrouped();
            addressBookSave.IsEnabled = true;
        }

        private async void AddressBookEdit_ClickAsync(object sender, RoutedEventArgs e)
        {
            AddressBook addressBook = AddressBook.Instance;

            ContentDialogAddressBookEntry contentDialog = new ContentDialogAddressBookEntry();
            contentDialog.Title = "Edit Address Book Entry";
            //contentDialog.PrimaryButtonText = "Edit";

            contentDialog.AddressBookCallsign = _selectedEntry.Callsign;
            contentDialog.AddressBookName = _selectedEntry.NameDetail;
            contentDialog.SelectedPrimaryBBS = _selectedEntry.BBSPrimary;
            contentDialog.SelectedSecondaryBBS = _selectedEntry.BBSSecondary;

            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _selectedEntry.Callsign = contentDialog.AddressBookCallsign;
                _selectedEntry.NameDetail = contentDialog.AddressBookName;
                _selectedEntry.BBSPrimary = contentDialog.SelectedPrimaryBBS;
                _selectedEntry.BBSSecondary = contentDialog.SelectedSecondaryBBS;

                ContactsCVS.Source = addressBook.GetContactsGrouped();
                addressBookSave.IsEnabled = true;
            }
            else
            {
                addressBookSave.IsEnabled = false;
            }
        }

        private async void AddressBookSave_ClickAsync(object sender, RoutedEventArgs e)
        {
            await AddressBook.Instance.SaveAsync();

            addressBookSave.IsEnabled = false;
        }

        #endregion Address Book
        #region Distribution Lists
        enum DistributionListState
        {
            None,
            Edit,
            Add,
            Delete
        }
        DistributionListState _distributionListState = DistributionListState.None;

        private void DistributionListName_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
            distributionListItems.Text = DistributionListArray.Instance.DistributionListsDict[sender.Text];
        }

        private void DistributionListName_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (string.IsNullOrEmpty(distributionListName.Text))
            {
                sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames();
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames(distributionListName.Text);
            }
        }

        private void DistributionListAddItem_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
            if (!distributionListItems.Text.Contains(sender.Text))
            {
                if (string.IsNullOrEmpty(distributionListItems.Text))
                {
                    distributionListItems.Text = $"{sender.Text}";
                }
                else
                {
                    distributionListItems.Text += $", {sender.Text}";
                }
            }
            sender.Text = "";
        }

        private void DistributionListAddItem_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (string.IsNullOrEmpty(distributionListAddItem.Text))
            {
                sender.ItemsSource = null;
                return;
            }

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = AddressBook.Instance.GetAddressNames(distributionListAddItem.Text);
            }
        }

        private void AppBarDistributionListsAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            _distributionListState = DistributionListState.Add;
            distributionListName.Text = "";
            distributionListItems.Text = "";
            appBarDistributionListsSave.IsEnabled = true;
            distributionListAddItem.IsEnabled = true;
            distributionListItems.IsReadOnly = false;
        }

        private void AppBarDistributionListsDelete_ClickAsync(object sender, RoutedEventArgs e)
        {
            _distributionListState = DistributionListState.Delete;
            appBarDistributionListsSave.IsEnabled = true;
            DistributionListArray.Instance.RemoveDistributionList(distributionListName.Text);
        }

        private void AppBarDistributionListsEdit_ClickAsync(object sender, RoutedEventArgs e)
        {
            _distributionListState = DistributionListState.Edit;
            distributionListAddItem.IsEnabled = true;
            distributionListItems.IsReadOnly = false;
            appBarDistributionListsSave.IsEnabled = true;
        }

        private async void AppBarDistributionListsSave_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (_distributionListState == DistributionListState.Add)
            {
                // Must not exist
                if (DistributionListArray.Instance.DistributionListsDict.TryGetValue(distributionListName.Text, out string items))
                {
                    await Utilities.ShowMessageDialogAsync("The Distribution List already exists.", "DistributionList List Error");
                    return;
                }
                DistributionList list = new DistributionList()
                {
                    DistributionListName = distributionListName.Text,
                    DistributionListItems = distributionListItems.Text
                };
                DistributionListArray.Instance.AddDistributionList(list);
            }
            else if (_distributionListState == DistributionListState.Edit)
            {
                // Must exist
                if (!DistributionListArray.Instance.DistributionListsDict.TryGetValue(distributionListName.Text, out string items))
                {
                    await Utilities.ShowMessageDialogAsync("The Distribution List does not exist.", "DistributionList List Error");
                    return;
                }
                DistributionList list = new DistributionList()
                {
                    DistributionListName = distributionListName.Text,
                    DistributionListItems = distributionListItems.Text
                };
                DistributionListArray.Instance.UpdateDistributionList(list);
            }
            else if (_distributionListState == DistributionListState.Delete)
            {
                DistributionListArray.Instance.RemoveDistributionList(distributionListName.Text);
                distributionListName.Text = "";
                distributionListItems.Text = "";
            }
            await DistributionListArray.Instance.SaveAsync();

            _distributionListState = DistributionListState.None;
            appBarDistributionListsSave.IsEnabled = false;
            distributionListAddItem.IsEnabled = false;
            distributionListItems.IsReadOnly = true;
        }
        #endregion Distribution Lists

    }
}
