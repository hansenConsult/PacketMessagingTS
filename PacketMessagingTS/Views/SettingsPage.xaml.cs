using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.Foundation;
using MetroLog;
using Windows.Devices.Bluetooth;

namespace PacketMessagingTS.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = Singleton<SettingsViewModel>.Instance;
        public IdentityViewModel _identityViewModel { get; } = Singleton<IdentityViewModel>.Instance;
        public PacketSettingsViewModel _packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;
        //public PacketSettingsViewModel _packetSettingsViewModel { get; } = new PacketSettingsViewModel();
        public TNCSettingsViewModel _TNCSettingsViewModel { get; } = new TNCSettingsViewModel();
        //public MailSettingsViewModel _mailSettingsViewModel { get; } = new MailSettingsViewModel();
        public AddressBookViewModel _addressBookViewModel { get; } = new AddressBookViewModel();


        static ObservableCollection<Profile> _profileCollection;

        ComportComparer comportComparer;

        private ObservableCollection<DeviceListEntry> listOfDevices;

        private List<SerialDevice> _listOfSerialDevices;
        private ObservableCollection<SerialDevice> CollectionOfSerialDevices;
        private List<DeviceInformation> _listOfBluetoothDevices;
        private ObservableCollection<DeviceInformation> CollectionOfBluetoothDevices;

        private ObservableCollection<uint> listOfBaudRates;
        private ObservableCollection<ushort> listOfDataBits;

        private string _bluetoothDeviceSelector;
        private Dictionary<DeviceWatcher, String> mapDeviceWatchersToDeviceSelector;

        private Boolean watchersSuspended;
        private Boolean watchersStarted;

        // Has all the devices enumerated by the device watcher?
        private Boolean isAllDevicesEnumerated;

        private SuspendingEventHandler appSuspendEventHandler;
        private EventHandler<Object> appResumeEventHandler;

        // Identity settings
        public static ObservableCollection<TacticalCallsignData> listOfTacticallsignsArea;

        TacticalCallsignData _tacticalCallsignData;


        public SettingsPage()
        {
            InitializeComponent();

            ObservableCollection <BBSData> bbsDataCollection = new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataList);
            BBSDataCollection.Source = bbsDataCollection;
            //comboBoxBBS.SelectedValue = SharedData.CurrentBBS;

            ObservableCollection<TNCDevice> tncDeviceCollection = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList );
            DeviceListSource.Source = tncDeviceCollection;
            //comboBoxTNCs.SelectedValue = SharedData.CurrentTNCDevice;

            // Serial ports
            listOfDevices = new ObservableCollection<DeviceListEntry>();
            _listOfSerialDevices = new List<SerialDevice>();
            CollectionOfSerialDevices = new ObservableCollection<SerialDevice>();
            _listOfBluetoothDevices = new List<DeviceInformation>();
            CollectionOfBluetoothDevices = new ObservableCollection<DeviceInformation>();
            comportComparer = new ComportComparer();

            mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, String>();
            watchersStarted = false;
            watchersSuspended = false;
            isAllDevicesEnumerated = false;

            listOfBaudRates = new ObservableCollection<uint>();
            for (uint i = 1200; i < 39000; i *= 2)
            {
                listOfBaudRates.Add(i);
            }
            BaudRateListSource.Source = listOfBaudRates;

            // data bits
            listOfDataBits = new ObservableCollection<ushort>() { 7, 8 };
            DataBitsListSource.Source = listOfDataBits;

            //ParitiesListSource.Source = Enum.GetValues(typeof(SerialParity));

            //StopBitsListSource.Source = Enum.GetValues(typeof(SerialStopBitCount));

            ProfilesCollection.Source = ProfileArray.Instance.ProfileList;

            //ObservableCollection<EmailAccount> EmailAccountsObservableCollection = new ObservableCollection<EmailAccount>();
            //foreach (EmailAccount account in EmailAccountArray.Instance.EmailAccounts)
            //{
            //    EmailAccountsObservableCollection.Add(account);
            //}
            ////EmailAccountsSource.Source = EmailAccountsObservableCollection;
            EmailAccountsSource.Source = EmailAccountArray.Instance.EmailAccounts;

            ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();

            // Identity initialization
            listOfTacticallsignsArea = new ObservableCollection<TacticalCallsignData>();
            foreach (var callsignData in App._tacticalCallsignDataDictionary.Values)
            {
                listOfTacticallsignsArea.Add(callsignData);
            }
            TacticalCallsignsAreaSource.Source = listOfTacticallsignsArea;

            //distributionListName.ItemsSource = DistributionListArray.Instance.GetDistributionLists();
            //distributionListAddItem.IsEnabled = false;
            //distributionListItems.IsReadOnly = true;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
            //Singleton<PacketSettingsViewModel>.Instance.ProfileSelectedIndex = Convert.ToUInt32(App.Properties["ProfileSelectedIndex"]);
            // Initialize the desired device watchers so that we can watch for when devices are connected/removed
            InitializeDeviceWatchers();
            StartDeviceWatchers();
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
        #region General
        //private async void FirstMessageNumber_TextChangedAsync(object sender, TextChangedEventArgs e)
        //{
        //    int messageNumber = Convert.ToInt32(((TextBox)sender).Text);
        //    await Utilities.MarkMessageNumberAsUsed(messageNumber);
        //    ViewModel.FirstMessageNumber = messageNumber.ToString();
        //}

        #endregion General
        #region Identity
        private void ComboBoxTacticalCallsignArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _tacticalCallsignData = (TacticalCallsignData)e.AddedItems[0];

            _tacticalCallsignData.TacticalCallsignsChanged = false;
            IdentityViewModel._tacticalCallsignData = _tacticalCallsignData;
            if (_tacticalCallsignData.TacticalCallsigns != null)
            {
                ObservableCollection<TacticalCall> listOfTacticallsigns = new ObservableCollection<TacticalCall>();
                foreach (var callsignData in _tacticalCallsignData.TacticalCallsigns.TacticalCallsignsArray)
                {
                    listOfTacticallsigns.Add(callsignData);
                }
                TacticalCallsignsSource.Source = listOfTacticallsigns;
            }
            if (_tacticalCallsignData.AreaName == "Other")
            {
                textBoxTacticalCallsign.Visibility = Visibility.Visible;
                comboBoxTacticalCallsign.Visibility = Visibility.Collapsed;
                comboBoxAdditionalText.SelectedItem = null;
            }
            else
            {
                textBoxTacticalCallsign.Visibility = Visibility.Collapsed;
                comboBoxTacticalCallsign.Visibility = Visibility.Visible;
            }
        }

        private void textBoxTacticalCallsign_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTacticalCallsign.Text.Length == 6)
                _identityViewModel.TacticalCallsignOther = textBoxTacticalCallsign.Text;
        }
        #endregion Identity
        #region Profiles
        private bool _bbsChanged = false;
        private bool _tncChanged = false;
        private bool _defaultToChanged = false;

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

            _profileCollection = new ObservableCollection<Profile>();
            foreach (Profile prof in ProfileArray.Instance.ProfileList)
            {
                _profileCollection.Add(prof);
            }
            ProfilesCollection.Source = _profileCollection;


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
                ProfileArray.Instance.ProfileList.Add(newProfile);
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

            ProfilesCollection.Source = ProfileArray.Instance.ProfileList;
            comboBoxProfiles.SelectedIndex = index;

            //_bbsChanged = false;
            //_tncChanged = false;
            //_defaultToChanged = false;
            //profileSave.IsEnabled = false;
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
            ProfilesCollection.Source = ProfileArray.Instance.ProfileList;

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

        private void ComboBoxTNCs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Count > 0)
            //{
            //    var selectedTNCDevice = (TNCDevice)e.AddedItems[0];

            //    if (string.IsNullOrEmpty(selectedTNCDevice.Prompts.Command))
            //    {
            //        comboBoxBBS.SelectedIndex = -1;
            //    }
            //    if (_packetSettingsViewModel.CurrentProfile != null)
            //    {
            //        _tncChanged = _packetSettingsViewModel.CurrentProfile.TNC != selectedTNCDevice.Name;
            //    }

            //    profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
            //    SharedData.CurrentTNCDevice = selectedTNCDevice;
            //}
        }

        private void ComboBoxBBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Count > 0)
            //{
            //    var selectedBBS = (BBSData)e.AddedItems[0];
            //    SharedData.CurrentBBS = selectedBBS;
                //ViewModels.SharedData._currentProfile.BBS = selectedBBS.Name;
 //               textBoxDescription.Text = selectedBBS.Description;
 //               textBoxFrequency1.Text = selectedBBS.Frequency1;
 //               textBoxFrequency2.Text = selectedBBS.Frequency2;
                //if (_packetSettingsViewModel.CurrentProfile != null)
                //{
                //    _bbsChanged = _packetSettingsViewModel.CurrentProfile.BBS != selectedBBS.Name;
                //}

                //profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
            //}
            //else
            //{
            //    textBoxDescription.Text = "";
            //    textBoxFrequency1.Text = "";
            //    textBoxFrequency2.Text = "";
            //}
        }

        private void TextBoxTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (_packetSettingsViewModel.CurrentProfile.SendTo != ((TextBox)sender).Text)
            //{
            //    _defaultToChanged = true;
            //}
            //else
            //{
            //    _defaultToChanged = false;
            //}
            //profileSave.IsEnabled = _bbsChanged | _tncChanged | _defaultToChanged;
        }

        private void ComboBoxProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (Profile profile in e.RemovedItems)
            //{
            //	profile.Selected = false;
            //}

            try
            {
                //Profile profile = (Profile)((ComboBox)sender).SelectedItem;
                //if (profile != null)
                //{
                //    _packetSettingsViewModel.CurrentProfile = profile;
                //    //comboBoxTNCs.SelectedValuePath = "Name";
                //    comboBoxTNCs.SelectedValue = profile.TNC;
                //    //BBSSelectedValue = profile.BBS;
                //    comboBoxBBS.SelectedValue = profile.BBS;
                //    //textBoxTo.Text = profile.SendTo;
                //}
                //_bbsChanged = false;
                //_tncChanged = false;
                //_defaultToChanged = false;

                //profileSave.IsEnabled = false;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
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

        private void ClearDeviceEntries()
        {
            listOfDevices.Clear();
            foreach (SerialDevice serialDevice in _listOfSerialDevices)
            {
                serialDevice.Dispose();
            }
            _listOfSerialDevices.Clear();
            CollectionOfSerialDevices.Clear();
            _listOfBluetoothDevices.Clear();
            CollectionOfBluetoothDevices.Clear();
        }

        private void InitializeDeviceWatchers()
        {
            // Serial devices device selector
            var deviceSelector = SerialDevice.GetDeviceSelector();

            // Create a device watcher to look for instances of the Serial Device that match the device selector
            // used earlier.
            var deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

            // Allow the EventHandlerForDevice to handle device watcher events that relates or effects our device (i.e. device removal, addition, app suspension/resume)
            AddDeviceWatcher(deviceWatcher, deviceSelector);

            // Bluetooth devices device selector
            _bluetoothDeviceSelector = BluetoothDevice.GetDeviceSelector();
            deviceWatcher = DeviceInformation.CreateWatcher(_bluetoothDeviceSelector);
            AddDeviceWatcher(deviceWatcher, _bluetoothDeviceSelector);
        }

        private void StartDeviceWatchers()
        {
            // Start all device watchers
            watchersStarted = true;
            isAllDevicesEnumerated = false;

            foreach (DeviceWatcher deviceWatcher in mapDeviceWatchersToDeviceSelector.Keys)
            {
                if ((deviceWatcher.Status != DeviceWatcherStatus.Started)
                    && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Start();
                }
            }
        }

        /// <summary>
        /// Stops all device watchers.
        /// </summary>
        private void StopDeviceWatchers()
        {
            // Stop all device watchers
            foreach (DeviceWatcher deviceWatcher in mapDeviceWatchersToDeviceSelector.Keys)
            {
                if ((deviceWatcher.Status == DeviceWatcherStatus.Started)
                    || (deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Stop();
                }
            }

            // Clear the list of devices so we don't have potentially disconnected devices around
            ClearDeviceEntries();

            watchersStarted = false;
        }

        private DeviceListEntry FindDevice(String deviceId)
        {
            if (deviceId != null)
            {
                foreach (DeviceListEntry entry in listOfDevices)
                {
                    if (entry.DeviceInformation.Id == deviceId)
                    {
                        return entry;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// We must stop the DeviceWatchers because device watchers will continue to raise events even if
        /// the app is in suspension, which is not desired (drains battery). We resume the device watcher once the app resumes again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAppSuspension(Object sender, SuspendingEventArgs args)
        {
            if (watchersStarted)
            {
                watchersSuspended = true;
                StopDeviceWatchers();
            }
            else
            {
                watchersSuspended = false;
            }
        }

        /// <summary>
        /// See OnAppSuspension for why we are starting the device watchers again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAppResume(Object sender, Object args)
        {
            if (watchersSuspended)
            {
                watchersSuspended = false;
                StartDeviceWatchers();
            }
        }

        /// <summary>
        /// We will remove the device from the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformationUpdate"></param>
        private async void OnDeviceRemovedAsync(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            await RemoveDeviceFromListAsync(deviceInformationUpdate.Id);
        }

        /// <summary>
        /// This function will add the device to the listOfDevices so that it shows up in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformation"></param>
        private async void OnDeviceAddedAsync(DeviceWatcher sender, DeviceInformation deviceInformation)
        {
            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                new DispatchedHandler(() =>
                {
                    AddDeviceToListAsync(deviceInformation, mapDeviceWatchersToDeviceSelector[sender]);
                }));
        }


        /// <summary>
        /// Notify the UI whether or not we are connected to a device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDeviceEnumerationComplete(DeviceWatcher sender, Object args)
        {
            isAllDevicesEnumerated = true;

            //await rootPage.Dispatcher.RunAsync(
            //    CoreDispatcherPriority.Normal,
            //    new DispatchedHandler(() =>
            //    {
            //        isAllDevicesEnumerated = true;

            //    // If we finished enumerating devices and the device has not been connected yet, the OnDeviceConnected method
            //    // is responsible for selecting the device in the device list (UI); otherwise, this method does that.
            //    if (EventHandlerForDevice.Current.IsDeviceConnected)
            //        {
            //            SelectDeviceInList(EventHandlerForDevice.Current.DeviceInformation.Id);

            //            ButtonDisconnectFromDevice.Content = ButtonNameDisconnectFromDevice;

            //        //rootPage.NotifyUser("Connected to - " +
            //        //					EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);

            //        EventHandlerForDevice.Current.ConfigureCurrentlyConnectedDevice();
            //        }
            //        else if (EventHandlerForDevice.Current.IsEnabledAutoReconnect && EventHandlerForDevice.Current.DeviceInformation != null)
            //        {
            //        // We will be reconnecting to a device
            //        ButtonDisconnectFromDevice.Content = ButtonNameDisableReconnectToDevice;

            //        //rootPage.NotifyUser("Waiting to reconnect to device -  " + EventHandlerForDevice.Current.DeviceInformation.Id, NotifyType.StatusMessage);
            //    }
            //        else
            //        {
            //        //rootPage.NotifyUser("No device is currently connected", NotifyType.StatusMessage);
            //    }
            //    }));
        }

        private void StartHandlingAppEvents()
        {
            appSuspendEventHandler = new SuspendingEventHandler(this.OnAppSuspension);
            appResumeEventHandler = new EventHandler<Object>(this.OnAppResume);

            // This event is raised when the app is exited and when the app is suspended
            App.Current.Suspending += appSuspendEventHandler;
            App.Current.Resuming += appResumeEventHandler;
        }

        private void StopHandlingAppEvents()
        {
            // This event is raised when the app is exited and when the app is suspended
            App.Current.Suspending -= appSuspendEventHandler;
            App.Current.Resuming -= appResumeEventHandler;
        }

        /// <summary>
        /// Registers for Added, Removed, and Enumerated events on the provided deviceWatcher before adding it to an internal list.
        /// </summary>
        /// <param name="deviceWatcher"></param>
        /// <param name="deviceSelector">The AQS used to create the device watcher</param>
        private void AddDeviceWatcher(DeviceWatcher deviceWatcher, String deviceSelector)
        {
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(this.OnDeviceAddedAsync);
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(this.OnDeviceRemovedAsync);
            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(this.OnDeviceEnumerationComplete);

            mapDeviceWatchersToDeviceSelector.Add(deviceWatcher, deviceSelector);
        }

        private async void AddDeviceToListAsync(DeviceInformation deviceInformation, string deviceSelector)
        {
            try
            {
                // search the device list for a device with a matching interface ID
                var match = FindDevice(deviceInformation.Id);

                // Add the device if it's new
                if (match == null)
                {
                    // Create a new element for this device interface, and queue up the query of its
                    // device information
                    match = new DeviceListEntry(deviceInformation, deviceSelector);

                    // Add the new element to the end of the list of devices
                    listOfDevices.Add(match);

                    if (deviceInformation.Pairing.IsPaired)
                    {
                        // Bluetooth device
                        try
                        {
                            if (deviceInformation.Kind == DeviceInformationKind.AssociationEndpoint)
                            {
                                _listOfBluetoothDevices.Add(deviceInformation);
                                CollectionOfBluetoothDevices = new ObservableCollection<DeviceInformation>(_listOfBluetoothDevices);
                                ComNameListSource.Source = CollectionOfBluetoothDevices;
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Log(LogLevel.Error, $"Overall Connect: { ex.Message}");
                        }
                    }
                    else
                    {
                        // USB serial port
                        SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInformation.Id);
                        if (serialDevice != null)
                        {
                            //string name = serialDevice.PortName;
                            _listOfSerialDevices.Add(serialDevice);
                            // Sort list
                            _listOfSerialDevices = _listOfSerialDevices.OrderBy(s => s.PortName, comportComparer).ToList();
                            CollectionOfSerialDevices = new ObservableCollection<SerialDevice>(_listOfSerialDevices);
                            ComPortListSource.Source = CollectionOfSerialDevices;
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Debug.WriteLine($"{e.Message}, DeviceId = {deviceInformation.Id}");
                LogHelper.Log(LogLevel.Error, $"{e.Message}, DeviceId = {deviceInformation.Id}");
#endif
            }
        }

        private async Task RemoveDeviceFromListAsync(String deviceId)
        {
            // Removes the device entry from the internal list; therefore the UI
            var deviceEntry = FindDevice(deviceId);

            listOfDevices.Remove(deviceEntry);

            SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceId);
            if (serialDevice != null)
            {
                _listOfSerialDevices.Remove(serialDevice);
                // Sort list
                _listOfSerialDevices = _listOfSerialDevices.OrderBy(s => s.PortName, comportComparer).ToList();
                CollectionOfSerialDevices = new ObservableCollection<SerialDevice>(_listOfSerialDevices);
                ComPortListSource.Source = CollectionOfSerialDevices;
            }
        }


        private void comboBoxStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void SetMailControlsEnabledState(bool enabledState)
        {
            mailPortString.IsEnabled = enabledState;
            mailUserName.IsEnabled = enabledState;
            mailPassword.IsEnabled = enabledState;
            if (enabledState == true)
            {
                mailIsSSL.Visibility = Visibility.Visible;
                mailServerComboBox.Visibility = Visibility.Collapsed;
                mailServer.Visibility = Visibility.Visible;
            }
            else
            {
                mailIsSSL.Visibility = Visibility.Collapsed;
                mailServerComboBox.Visibility = Visibility.Visible;
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
                    SetMailControlsEnabledState(true);
                    //appBarSaveTNC.IsEnabled = true;
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

            if (CollectionOfBluetoothDevices.Count > 0)
            {
                comboBoxComName.SelectedItem = CollectionOfBluetoothDevices[0];
            }

            if (CollectionOfSerialDevices.Count > 0)
            {
                comboBoxComPort.SelectedItem = CollectionOfSerialDevices[0];
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
                    if (tncDevice.Name.Contains("E-Mail"))
                    {
                        UpdateMailState(TNCState.EMail);
                        EMailSettings.Visibility = Visibility.Visible;
                        PivotTNC.Visibility = Visibility.Collapsed;
                        mailServerComboBox.SelectedIndex = Convert.ToInt32(_TNCSettingsViewModel.MailAccountSelectedIndex);
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
            //if (SettingsPageViewModel.TNCPartViewModel.CurrentMailAccount.MailUserName != (string)((TextBox)sender).Text)
            //{
            //    _emailMailUserNameChanged = true;
            //}
            //else
            //{
            //    _emailMailUserNameChanged = false;
            //}
            //appBarSettingsSave.IsEnabled = _emailMailServerChanged | _emailMailServerPortChanged
            //    | _emailMailUserNameChanged | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

        private void MailPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //if (SettingsPageViewModel.TNCPartViewModel.CurrentMailAccount.MailPassword != (string)((PasswordBox)sender).Password)
            //{
            //    _emailMailPasswordChanged = true;
            //}
            //else
            //{
            //    _emailMailPasswordChanged = false;
            //}
            //appBarSettingsSave.IsEnabled = _emailMailServerChanged | _emailMailServerPortChanged
            //    | _emailMailUserNameChanged | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

        private void MailIsSSL_Toggled(object sender, RoutedEventArgs e)
        {
            //if (SettingsPageViewModel.TNCPartViewModel.CurrentMailAccount.MailIsSSLField != ((ToggleSwitch)sender).IsOn)
            //{
            //    _emailMailIsSSLChanged = true;
            //}
            //else
            //{
            //    _emailMailIsSSLChanged = false;
            //}
            //appBarSettingsSave.IsEnabled = _emailMailServerChanged | _emailMailServerPortChanged
            //    | _emailMailUserNameChanged | _emailMailPasswordChanged | _emailMailIsSSLChanged;
        }

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
            if (_tncState == TNCState.EMailDelete)
            {
                //await EmailAccountArray.Instance.SaveAsync();
                //UpdateMailState(TNCState.EMail);
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

            //ResetTNCDeviceChanged();

            _TNCSettingsViewModel.ResetChangedProperty();
            //appBarSaveTNC.IsEnabled = false;

        }
        #endregion Interface
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
        #endregion Distribution Lists
    }
}
