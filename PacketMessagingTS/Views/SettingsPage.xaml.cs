using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MetroLog;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<SettingsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public SettingsViewModel SettingsViewModel { get; } = Singleton<SettingsViewModel>.Instance;
        public IdentityViewModel IdentityViewModel { get; } = Singleton<IdentityViewModel>.Instance;
        public PacketSettingsViewModel PacketSettingsViewmodel { get; } = Singleton<PacketSettingsViewModel>.Instance;
        public TNCSettingsViewModel TncSettingsViewModel { get; } = Singleton<TNCSettingsViewModel>.Instance;
        public AddressBookViewModel AddressBookViewModel { get; } = new AddressBookViewModel();

        //string[] CopyDestinations = new string[]
        //{
        //    "Radio Room",
        //    "Magement",
        //    "Operations",
        //    "Planning",
        //    "Finance",
        //};


        public SettingsPage()
        {
            InitializeComponent();

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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await SettingsViewModel.InitializeAsync();

            //_TNCSettingsViewModel.ListOfSerialPorts.Clear();
            //List<string> listOfSerialPorts = new List<string>();

            //DeviceInformationCollection deviceCollectionCom = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{86e0d1e0-8089-11d0-9ce4-08003e301f73}\"");
            //foreach (DeviceInformation deviceInformation in deviceCollectionCom)
            //{
            //    SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInformation.Id);
            //    if (serialDevice != null)
            //    {
            //        listOfSerialPorts.Add(serialDevice.PortName);
            //        listOfSerialPorts.Sort(new ComportComparer());

            //        _logHelper.Log(LogLevel.Info, $"devInfo: {serialDevice.PortName}");

            //        serialDevice.Dispose();     // Necessary to avoid crash on removed device
            //    }
            //}
            //_TNCSettingsViewModel.CollectionOfSerialDevices = new ObservableCollection<string>(listOfSerialPorts);
            //_logHelper.Log(LogLevel.Info, $"List of Serial Ports: {listOfSerialPorts.Count}");
            //_TNCSettingsViewModel.CollectionOfSerialDevices = await CreateComportObservableCollectionAsync();

            var deviceCollection = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{0ecef634-6ef0-472a-8085-5ad023ecbccd}\"");
            ObservableCollection<string> printers = new ObservableCollection<string>();
            foreach (DeviceInformation deviceInformation in deviceCollection)
            {
                printers.Add(deviceInformation.Name);
            }
            selectedPrinter.ItemsSource = printers;

            if (!(e.Parameter is null))
            {
                SettingsViewModel.SettingsPivotSelectedIndex = (int)e.Parameter;
            }
            SettingsPivot.SelectedIndex = SettingsViewModel.SettingsPivotSelectedIndex;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //if (_TNCSettingsViewModel.IsAppBarSaveEnabled)
            //{
            //    _TNCSettingsViewModel.SaveChanges(_TNCSettingsViewModel.TNCDeviceSelectedIndex);
            //}
            if (PacketSettingsViewmodel.IsAppBarSaveEnabled)
            {
                bool save = await ContentDialogs.ShowDualButtonMessageDialogAsync("Save changes to Packet Settings?", "Yes", "No");
                if (save)
                {
                    PacketSettingsViewmodel.PacketSettingsSave();
                }
                // Disable Save button
                PacketSettingsViewmodel.IsAppBarSaveEnabled = false;
                //_PacketSettingsViewmodel.ResetChangedProperty();
            }

            SettingsViewModel.SettingsPivotSelectedIndex = SettingsPivot.SelectedIndex;

            base.OnNavigatingFrom(e);

            //_logHelper.Log(LogLevel.Trace, $"Exiting OnNavigatingFrom() in Settings. {(SettingsPivot.SelectedItem as PivotItem).Name}");
        }

        private void EnableCopyTo(string sentReceived)
        {
            CheckBox checkBox = pivotSettings.FindName($"{sentReceived}Print") as CheckBox;

            int copyCount;
            //if (sentReceived.Contains("received"))
            //{
            muxc.NumberBox receivedSentCount = pivotSettings.FindName($"{sentReceived}CopyCount") as muxc.NumberBox;
            if (string.IsNullOrEmpty(receivedSentCount.Text))
                copyCount = 0;
            else
                copyCount = (int)receivedSentCount.Value;

            if (copyCount < 0)
                copyCount = 0;
            //}
            //else
            //{
            //    TextBox receivedSentCount = pivotSettings.FindName($"{sentReceived}CopyCount") as TextBox;
            //    //int copyCount;
            //    if (string.IsNullOrEmpty(receivedSentCount.Text))
            //        copyCount = 0;
            //    else
            //        copyCount = Convert.ToInt32(receivedSentCount.Text);
            //}

            //string[] copyDestinations = new string[copyCount + 1];
            //Grid grid = pivotSettings.FindName($"{sentReceived}CopyDestinations") as Grid;
            //string baseName = $"{sentReceived}Dest";
            //for (int i = 1; i <= 4; i++)
            //{
            //    string comboBoxName = baseName + i.ToString();
            //    ComboBox comboBox = grid.FindName(comboBoxName) as ComboBox;
            //    if (comboBox == null)
            //        continue;

            //    if ((bool)checkBox.IsChecked && i <= copyCount)
            //    {
            //        comboBox.IsEnabled = true;
            //        copyDestinations[i] = comboBox.SelectedItem as string;
            //    }
            //    else
            //    {
            //        comboBox.IsEnabled = false;
            //    }
            //}
            if (checkBox?.IsChecked == null || !(bool)checkBox.IsChecked)
            {
                return;
            }
        }

        //private async Task<ObservableCollection<string>> CreateComportObservableCollectionAsync()
        //{
        //    List<string> listOfSerialPorts = new List<string>();

        //    var deviceCollection = await DeviceInformation.FindAllAsync("System.Devices.InterfaceClassGuid:=\"{86e0d1e0-8089-11d0-9ce4-08003e301f73}\"");
        //    foreach (DeviceInformation deviceInformation in deviceCollection)
        //    {
        //        SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInformation.Id);
        //        if (serialDevice != null)
        //        {
        //            listOfSerialPorts?.Add(serialDevice.PortName);
        //            listOfSerialPorts?.Sort(new ComportComparer());

        //            //_logHelper.Log(LogLevel.Info, $"devInfo: {serialDevice.PortName}");

        //            serialDevice.Dispose();     // Necessary to avoid crash on removed device
        //        }
        //    }
        //    return new ObservableCollection<string>(listOfSerialPorts);
        //}

        private void SettingsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((SettingsPivot.SelectedItem as PivotItem).Name)
            {
                case "pivotSettings":
                    EnableCopyTo("received");
                    EnableCopyTo("sent");
                    break;
                case "pivotTNC":
                    //_TNCSettingsViewModel.CollectionOfSerialDevices = await CreateComportObservableCollectionAsync();
                    // Select current TNC device
                    //_TNCSettingsViewModel.TNCDeviceSelectedIndex = Utilities.GetProperty("TNCDeviceSelectedIndex");
                    break;
                case "pivotPacketSettings":
                    //comboBoxProfiles.Visibility = Visibility.Visible;
                    //textBoxNewProfileName.Visibility = Visibility.Collapsed;
                    //PacketSettingsViewmodel.ProfileNameVisibility = true;

                    int profileSelectedIndex = Utilities.GetProperty(nameof(PacketSettingsViewmodel.ProfileSelectedIndex));
                    string tnc = ProfileArray.Instance.ProfileList[profileSelectedIndex].TNC;
                    // Trigger a change to force opdate if the TNC list is changed
                    PacketSettingsViewmodel.TNC = "";
                    PacketSettingsViewmodel.TNC = tnc;
                    break;
                case "pivotIdentity":
                    //_identityViewModel.UserCallsign = Utilities.GetProperty<string>("UserCallsign");
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
            //_logHelper.Log(LogLevel.Trace, $"Exiting SettingsPivot_SelectionChanged(). {(SettingsPivot.SelectedItem as PivotItem).Name}");
        }

        private void ReceivedCopyCount_ValueChanged(muxc.NumberBox sender, muxc.NumberBoxValueChangedEventArgs args)
        {
            if (sender.Name.Contains("received"))
            {
                EnableCopyTo("received");
            }
            else if (sender.Name.Contains("sent"))
            {
                EnableCopyTo("sent");
            }
        }

        //private void CopyCount_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    string senderName = (sender as TextBox).Name;
        //    if (senderName.Contains("received"))
        //    {
        //        EnableCopyTo("received");
        //    }
        //    else if (senderName.Contains("sent"))
        //    {
        //        EnableCopyTo("sent");
        //    }
        //}

        private void Print_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            string senderName = (sender as CheckBox).Name;
            if (senderName.Contains("received"))
            {
                EnableCopyTo("received");
            }
            else if (senderName.Contains("sent"))
            {
                EnableCopyTo("sent");
            }
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

        private async void AutoSuggestBoxUserCallsign_TextChangedAsync(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = AddressBook.Instance.GetCallsigns(comboBoxUsercallsign.Text);
                if ((sender.ItemsSource as List<string>).Count == 0)
                {
                    IdentityViewModel.UserName = "";
                    IdentityViewModel.UserCity = "";
                    IdentityViewModel.UserMsgPrefix = "";

                    bool retval = await ContentDialogs.ShowDualButtonMessageDialogAsync("The Call-Sign is not in the Address Book. \nPlease select 'Add to Address Book'.", "Add to Address Book");
                    if (retval)
                    {
                        //AddressBook addressBook = AddressBook.Instance;

                        ContentDialogAddressBookEntry contentDialog = new ContentDialogAddressBookEntry();
                        AddressBookEntry emptyEntry = new AddressBookEntry()
                        {
                            Callsign = "",
                            NameDetail = "",
                            City = "",
                            Prefix = "",
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
                            emptyEntry.City = contentDialog.AddressBookCity;
                            emptyEntry.Prefix = contentDialog.AddressBookPrefix;
                            emptyEntry.BBSPrimary = contentDialog.SelectedPrimaryBBS;
                            emptyEntry.BBSSecondary = contentDialog.SelectedSecondaryBBS;
                            bool success = UserAddressArray.Instance.AddAddressAsync(emptyEntry);
                            if (!success)
                            {
                                await ContentDialogs.ShowSingleButtonContentDialogAsync("Error adding a new address book entry.");
                            }
                            else
                            {
                                await UserAddressArray.Instance.SaveAsync();
                                await ContentDialogs.ShowSingleButtonContentDialogAsync("Call Sign successfully added. Now try to add a user Call Sign again.");
                            }
                        }
                    }
                }
            }
        }

        private void AutoSuggestBoxUserCallsign_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            if (AddressBook.Instance.AddressBookDictionary.TryGetValue(args.SelectedItem as string, out AddressBookEntry value))
            {
                IdentityViewModel.UserName = value.NameDetail;
                IdentityViewModel.UserCity = value.City;
            }
        }

        //private void ComboBoxTacticalCallsign_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        //{
        //    return;
        //    //_identityViewModel.TacticalCallsignSelectedIndex = -1;
        //    if (sender.Text.Length == 6)
        //    {

        //        //_identityViewModel.TacticalCallsign = sender.Text;
        //        TacticalCall tacticalCall = new TacticalCall()
        //        {
        //            TacticalCallsign = sender.Text,
        //            AgencyName = "",
        //            Prefix = sender.Text.Substring(sender.Text.Length - 3),
        //        };
        //        _identityViewModel.TacticalCallsignsSource.Add(tacticalCall);
        //        //_identityViewModel.TacticalSelectedIndexArray[_identityViewModel.TacticalCallsignAreaSelectedIndex] = _identityViewModel.TacticalCallsignsSource.IndexOf(tacticalCall);
        //        //_identityViewModel.TacticalSelectedIndexArray = _identityViewModel.TacticalSelectedIndexArray;
        //        //_identityViewModel.TacticalCallsignOther = sender.Text;
        //    }
        //}

        private void TextBoxTacticalCallsign_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
            if (((ComboBox)sender).Text.Length == 6)
            {
                IdentityViewModel.TacticalCallsign = ((ComboBox)sender).Text;
                TacticalCall tacticalCall = new TacticalCall()
                {
                    TacticalCallsign = ((ComboBox)sender).Text,
                    AgencyName = "",
                    Prefix = ((ComboBox)sender).Text.Substring(((ComboBox)sender).Text.Length - 3),
                };
                IdentityViewModel.TacticalCallsignsSource.Add(tacticalCall);
                IdentityViewModel.TacticalCallsignOther = ((ComboBox)sender).Text;
            }
        }
        #endregion Identity
        #region Profiles
        //private bool _bbsChanged = false;
        //private bool _tncChanged = false;
        //private bool _defaultToChanged = false;

        //private async Task ProfileSaveAsync()
        //{
        //    Profile profile = comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile;
        //    //int index = comboBoxProfiles.SelectedIndex;
        //    //Profile profile = ProfileArray.Instance.ProfileList.[index];
        //    if (comboBoxProfiles.Visibility == Visibility.Collapsed)
        //    {
        //        //	newProfile = new Profile();

        //        profile.Name = textBoxNewProfileName.Text;
        //        //	newProfile.BBS = comboBoxBBS.SelectedValue as string;
        //        //	newProfile.TNC = comboBoxTNCs.SelectedValue as string;
        //        //	newProfile.SendTo = textBoxTo.Text;
        //        //	newProfile.Selected = true;

        //        //	ViewModels.SharedData._profileArray.Profiles.SetValue(newProfile, ViewModels.SharedData._profileArray.Profiles.Length - 1);
        //        //comboBoxProfiles.Visibility = Visibility.Visible;
        //        //textBoxNewProfileName.Visibility = Visibility.Collapsed;
        //        _PacketSettingsViewmodel.ProfileNameVisibility = true;
        //    }
        //    //else
        //    //{
        //    //int index = comboBoxProfiles.SelectedIndex;
        //    //Profile profile = ViewModels.SharedData._profileArray.Profiles[index];

        //    profile.BBS = comboBoxBBS.SelectedValue as string;
        //    profile.TNC = comboBoxTNCs.SelectedValue as string;
        //    profile.SendTo = textBoxTo.Text;
        //    //profile.Selected = true;

        //    //ProfileArray.Instance.ProfileList[ProfileArray.Instance.ProfileList.IndexOf(profile)] = profile;
        //    ProfileArray.Instance.ProfileList[comboBoxProfiles.SelectedIndex] = profile;
        //    //SharedData.ProfileArray.Profiles.SetValue(profile, index);
        //    //}

        //    await ProfileArray.Instance.SaveAsync();

        //    //_profileCollection = new ObservableCollection<Profile>();
        //    //foreach (Profile prof in ProfileArray.Instance.ProfileList)
        //    //{
        //    //    _profileCollection.Add(prof);
        //    //}
        //    //_packetSettingsViewModel.ObservableProfileCollection = new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);


        //    //_bbsChanged = false;
        //    //_tncChanged = false;
        //    //_defaultToChanged = false;
        //    //profileSave.IsEnabled = false;
        //}

        //private async void PacketSettingsSave_ClickAsync(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    //int index = comboBoxProfiles.SelectedIndex;
        //    if (comboBoxProfiles.Visibility == Visibility.Collapsed)
        //    {
        //        Profile newProfile = new Profile()
        //        {
        //            Name = textBoxNewProfileName.Text,
        //        	BBS = comboBoxBBS.SelectedValue as string,
        //        	TNC = comboBoxTNCs.SelectedValue as string,
        //        	SendTo = textBoxTo.Text,
        //        };
        //        if (newProfile.TNC.Contains(SharedData.EMail))
        //        {
        //            comboBoxBBS.SelectedIndex = -1;
        //            newProfile.BBS = "";
        //        }
        //        ProfileArray.Instance.ProfileList.Add(newProfile);
        //        await ProfileArray.Instance.SaveAsync();
        //        //comboBoxProfiles.Visibility = Visibility.Visible;
        //        //textBoxNewProfileName.Visibility = Visibility.Collapsed;
        //        _PacketSettingsViewmodel.ProfileNameVisibility = true;
        //        //index = ProfileArray.Instance.ProfileList.Count - 1;
        //    }
        //    else
        //    {
        //        Profile profile = comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile;

        //        profile.Name = comboBoxProfiles.SelectedItem.ToString();
        //        profile.BBS = comboBoxBBS.SelectedValue as string;
        //        profile.TNC = comboBoxTNCs.SelectedValue as string;
        //        profile.SendTo = textBoxTo.Text;
        //        profile.Subject = _PacketSettingsViewmodel.DefaultSubject;
        //        profile.Message = _PacketSettingsViewmodel.DefaultMessage;

        //        //ProfileArray.Instance.ProfileList[ProfileArray.Instance.ProfileList.IndexOf(profile)] = profile;
        //        ProfileArray.Instance.ProfileList[comboBoxProfiles.SelectedIndex] = profile;
        //    }
        //    if ((comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile).TNC.Contains(SharedData.EMail))
        //    {
        //        comboBoxBBS.SelectedIndex = -1;
        //        (comboBoxProfiles.Items[comboBoxProfiles.SelectedIndex] as Profile).BBS = "";
        //    }

        //    await ProfileArray.Instance.SaveAsync();

        //    Utilities.SetApplicationTitle();

        //    //_PacketSettingsViewmodel.ResetChangedProperty();
        //    _PacketSettingsViewmodel.IsAppBarSaveEnabled = false;
        //    //comboBoxProfiles.SelectedIndex = index;

        //}

        //private void ProfileSettingsAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    profileSave.IsEnabled = true;

        //    //comboBoxProfiles.Visibility = Visibility.Collapsed;
        //    //textBoxNewProfileName.Visibility = Visibility.Visible;
        //    _PacketSettingsViewmodel.ProfileNameVisibility = false;

        //    //Profile newProfile = new Profile()
        //    //{
        //    //    BBS = comboBoxBBS.SelectedValue as string,
        //    //    TNC = comboBoxTNCs.SelectedValue as string,
        //    //    SendTo = textBoxTo.Text
        //    //};
        //    //ProfileArray.Instance.ProfileList.Add(newProfile);

        //    //ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
        //    //foreach (Profile profile in ProfileArray.Instance.ProfileList)
        //    //{
        //    //    //profile.Selected = false;
        //    //    profileCollection.Add(profile);
        //    //}
        //    ////sharedData.ProfileArray.Profiles[length].Selected = true;
        //    //ProfilesCollection.Source = profileCollection;
        //    //comboBoxProfiles.SelectedIndex = ProfileArray.Instance.ProfileList.Count - 1;
        //}

        //private void ProfileSettingsDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    int index = comboBoxProfiles.SelectedIndex;

        //    Profile profile = comboBoxProfiles.SelectedItem as Profile;
        //    ProfileArray.Instance.ProfileList.Remove(profile);
        //    //_packetSettingsViewModel.ObservableProfileCollection = new ObservableCollection<Profile>(ProfileArray.Instance.ProfileList);

        //    //int index = comboBoxProfiles.SelectedIndex;
        //    //var length = SharedData.ProfileArray.Profiles.Length;
        //    //Profile[] tempProfileArray = new Profile[length - 1];

        //    //ObservableCollection<Profile> profileCollection = new ObservableCollection<Profile>();
        //    //for (int i = 0, j = 0; i < length; i++)
        //    //{
        //    //    if (i != index)
        //    //    {
        //    //        tempProfileArray.SetValue(SharedData.ProfileArray?.Profiles[i], j);
        //    //        profileCollection.Add(SharedData.ProfileArray?.Profiles[i]);
        //    //        j++;
        //    //    }
        //    //}
        //    //ProfilesCollection.Source = profileCollection;
        //    //SharedData.ProfileArray.Profiles = tempProfileArray;

        //    comboBoxProfiles.SelectedIndex = Math.Max(index - 1, 0);
        //    profileSave.IsEnabled = true;
        //}

        #endregion
        #region Interface

        //private void TNCSaveAsCurrent()
        //{
        //    if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.TNCAdd)      // New setting have been created but not saved _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCSettingsViewModel.TNCState.TNC;
        //        return;

        //    //TNCDevice tncDevice = SharedData.CurrentTNCDevice;

        //    UpdateTNCFromUI(Singleton<PacketSettingsViewModel>.Instance.CurrentTNC);
        //}

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

        //private void NewTNCDevice()
        //{
        //    _TNCSettingsViewModel.DeviceListBoxVisibility = Visibility.Collapsed;
        //    _TNCSettingsViewModel.NewTNCDeviceNameVisibility = Visibility.Visible;

        //    textBoxInitCommandsPre.Text = "";
        //    textBoxInitCommandsPost.Text = "";

        //    toggleSwitchBluetooth.IsOn = false;
        //    //SetComportComboBoxVisibility();

        //    //if (CollectionOfBluetoothDevices.Count > 0)
        //    //{
        //    //    comboBoxComName.SelectedItem = CollectionOfBluetoothDevices[0];
        //    //}

        //    if (_TNCSettingsViewModel.CollectionOfSerialDevices.Count > 0)
        //    {
        //        comboBoxComPort.SelectedItem = _TNCSettingsViewModel.CollectionOfSerialDevices[0];
        //    }

        //    comboBoxBaudRate.SelectedItem = 9600;
        //    comboBoxDatabits.SelectedItem = 8;

        //    int i = 0;
        //    var values = Enum.GetValues(typeof(SerialParity));
        //    for (; i < values.Length; i++)
        //    {
        //        if ((SerialParity)values.GetValue(i) == SerialParity.None) break;
        //    }
        //    comboBoxParity.SelectedIndex = i;

        //    values = Enum.GetValues(typeof(SerialStopBitCount));
        //    for (i = 0; i < values.Length; i++)
        //    {
        //        if ((SerialStopBitCount)values.GetValue(i) == SerialStopBitCount.One) break;
        //    }
        //    comboBoxStopBits.SelectedIndex = i;

        //    values = Enum.GetValues(typeof(SerialHandshake));
        //    for (i = 0; i < values.Length; i++)
        //    {
        //        if ((SerialHandshake)values.GetValue(i) == SerialHandshake.RequestToSend)
        //        {
        //            break;
        //        }
        //    }
        //    comboBoxFlowControl.SelectedIndex = i;

        //    _TNCSettingsViewModel.TNCPromptsCommand = "";
        //    textBoxPromptsTimeout.Text = "";
        //    textBoxPromptsConnected.Text = "";
        //    textBoxPromptsDisconnected.Text = "";

        //    textBoxCommandsConnect.Text = "";
        //    textBoxCommandsConversMode.Text = "";
        //    textBoxCommandsMyCall.Text = "";
        //    textBoxCommandsRetry.Text = "";
        //    textBoxCommandsDateTime.Text = "";

        //    //TNCDevice tncDevice = _TNCSettingsViewModel.TNCDeviceFromUI;
        //    //TNCDeviceArray.Instance.TNCDeviceList.Add(tncDevice);
        //    //_TNCSettingsViewModel.CurrentTNCDevice = tncDevice;
        //    _TNCSettingsViewModel.IsAppBarSaveEnabled = true;
        //}

        //private void ConnectDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // TODO Use index changed only. Need to move email code to viewmodel

        //    if (e.AddedItems.Count > 0)
        //    {
        //        var TNCDevices = e.AddedItems;
        //        if (TNCDevices != null && TNCDevices.Count == 1)
        //        {
        //            //if (_TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.TNC
        //            //    && _TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.EMail
        //            //    && _TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.TNCDelete
        //            //    && _TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.TNCAdd)

        //            //if (_TNCSettingsViewModel.IsAppBarSaveEnabled)
        //            //    _TNCSettingsViewModel.SaveChanges(3);
        //            //if (_TNCSettingsViewModel.IsAppBarSaveEnabled)
        //            //{
        //            //    bool save = await Utilities.ShowDualButtonMessageDialogAsync("Save changes?", "Yes", "No");
        //            //    if (save)
        //            //    {
        //            //        AppBarSaveTNC_ClickAsync(this, null);
        //            //    }
        //            //    else
        //            //    {
        //            //        // Restore to default
        //            //        string mailUserName = "";
        //            //        foreach (TNCDevice tncDevice in TNCDeviceArray.Instance.TNCDeviceList)
        //            //        {
        //            //            if (!string.IsNullOrEmpty(tncDevice.MailUserName))
        //            //            {
        //            //                mailUserName = tncDevice.MailUserName;
        //            //            }
        //            //        }
        //            //        int i = EmailAccountArray.Instance.GetSelectedIndexFromEmailUserName(mailUserName);
        //            //        _TNCSettingsViewModel.MailAccountSelectedIndex = i;
        //            //    }
        //            //}
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        //SerialDevice serialDevice = null;
        //        //UseBluetooth = tncDevice.CommPort.IsBluetooth;
        //        //toggleSwitchBluetooth.IsOn = (bool)tncDevice.CommPort?.IsBluetooth;
        //        //SetComportComboBoxVisibility();

        //        //foreach (DeviceInformation deviceInfo in CollectionOfBluetoothDevices)
        //        //{
        //        //    if (deviceInfo.Name == tncDevice.CommPort.BluetoothName)
        //        //    {
        //        //        comboBoxComName.SelectedItem = deviceInfo;
        //        //        break;
        //        //    }
        //        //}
        //        //foreach (SerialDevice device in CollectionOfSerialDevices)
        //        //{
        //        //    if (device.PortName == tncDevice.CommPort.Comport)
        //        //    {
        //        //        comboBoxComPort.SelectedItem = device;
        //        //        break;
        //        //    }
        //        //}
        //        //if (serialDevice != null)
        //        {
        //            //comboBoxComPort.SelectedItem = serialDevice;
        //            //comboBoxBaudRate.SelectedValue = tncDevice.CommPort.Baudrate;
        //            //comboBoxDatabits.SelectedValue = tncDevice.CommPort.Databits;

        //            //int i = 0;
        //            //var values = Enum.GetValues(typeof(SerialParity));
        //            //for (; i < values.Length; i++)
        //            //{
        //            //    if ((SerialParity)values.GetValue(i) == tncDevice.CommPort.Parity) break;
        //            //}
        //            //comboBoxParity.SelectedIndex = i;
        //            //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedParity = tncDevice.CommPort.Parity;

        //            //values = Enum.GetValues(typeof(SerialStopBitCount));
        //            //for (i = 0; i < values.Length; i++)
        //            //{
        //            //    if ((SerialStopBitCount)values.GetValue(i) == tncDevice.CommPort.Stopbits) break;
        //            //}
        //            //comboBoxStopBits.SelectedIndex = i;

        //            //ViewModels.SettingsPageViewModel.TNCPartViewModel.SelectedStopBits = tncDevice.CommPort.Stopbits;
        //            //values = Enum.GetValues(typeof(SerialHandshake));
        //            //for (i = 0; i < values.Length; i++)
        //            //{
        //            //    if ((SerialHandshake)values.GetValue(i) == tncDevice.CommPort.Flowcontrol)
        //            //    {
        //            //        break;
        //            //    }
        //            //}
        //            //comboBoxFlowControl.SelectedIndex = i;
        //        }
        //        //else
        //        //{
        //        //	MessageDialog messageDialog = new MessageDialog("Com port not found. \nIs the TNC plugged in?");
        //        //	await messageDialog.ShowAsync();
        //        //}
        //        //_TNCSettingsViewModel.TNCPromptsCommand = tncDevice.Prompts.Command;
        //        //_TNCSettingsViewModel.TNCPromptsTimeout = tncDevice.Prompts.Timeout;
        //        //textBoxPrompsCommand.Text = tncDevice.Prompts.Command;
        //        //textBoxPromptsTimeout.Text = tncDevice.Prompts.Timeout;
        //        //textBoxPromptsConnected.Text = tncDevice.Prompts.Connected;
        //        //textBoxPromptsDisconnected.Text = tncDevice.Prompts.Disconnected;

        //        //_TNCSettingsViewModel.TNCCommandsConnect = tncDevice.Commands.Connect;
        //        //textBoxCommandsConnect.Text = tncDevice.Commands.Connect;
        //        //textBoxCommandsConversMode.Text = tncDevice.Commands.Conversmode;
        //        //textBoxCommandsMyCall.Text = tncDevice.Commands.MyCall;
        //        //textBoxCommandsRetry.Text = tncDevice.Commands.Retry;
        //        //textBoxCommandsDateTime.Text = tncDevice.Commands.Datetime;
        //    }
        //}

        private void MailUserNameComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            if (TncSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailAdd)
            {
                TncSettingsViewModel.MailUserName = args.Text;
            }
            else if (TncSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailEdit)
            {
                TncSettingsViewModel.MailUserName = args.Text;
            }
        }

        private void MailServerComboBox_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            if (TncSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailAdd)
            {
                TncSettingsViewModel.MailServer = args.Text;
            }
        }

        //private void AppBarAddTNC_Clicked(object sender, RoutedEventArgs e)
        //{
        //    if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMail)
        //    {
        //        _TNCSettingsViewModel._modifiedEmailAccountSelectedIndex = _TNCSettingsViewModel.MailAccountSelectedIndex;
        //        _TNCSettingsViewModel.UpdateMailState(TNCSettingsViewModel.TNCState.EMailAdd);
        //        _TNCSettingsViewModel.IsAppBarSaveEnabled = true;
        //    }
        //    else if (_TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.EMailDelete
        //                    || _TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.EMailEdit
        //                    || _TNCSettingsViewModel.State != TNCSettingsViewModel.TNCState.EMailAdd)
        //    {
        //        // Not an e-mail device
        //        _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNCAdd;
        //        NewTNCDevice();
        //    }
        //}

        //private void AppBarEditTNC_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMail)
        //    {
        //        _TNCSettingsViewModel.UpdateMailState(TNCSettingsViewModel.TNCState.EMailEdit);
        //        _TNCSettingsViewModel._modifiedEmailAccountSelectedIndex = _TNCSettingsViewModel.MailAccountSelectedIndex;
        //    }
        //    else
        //    {
        //        _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNCEdit;
        //    }
        //}

        //private void AppBarDeleteTNC_Click(object sender, RoutedEventArgs e)
        //{
        //    if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMail)
        //    {
        //        _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.EMailDelete;
        //        _TNCSettingsViewModel._deletedIndex = _TNCSettingsViewModel.MailAccountSelectedIndex;
        //        EmailAccountArray.Instance.EmailAccountList.RemoveAt(_TNCSettingsViewModel._deletedIndex);
        //        _TNCSettingsViewModel.IsAppBarSaveEnabled = true;
        //    }
        //    else
        //    {
        //        _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNCDelete;
        //        _TNCSettingsViewModel._deletedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;
        //        TNCDeviceArray.Instance.TNCDeviceList.RemoveAt(_TNCSettingsViewModel._deletedIndex);
        //        //_TNCSettingsViewModel.TNCDeviceListSource = TNCDeviceArray.Instance.TNCDeviceList;
        //        _TNCSettingsViewModel.IsAppBarSaveEnabled = true;
        //    }
        //}

        //private async void AppBarSaveTNC_ClickAsync(object sender, RoutedEventArgs e)
        //{
        //    _TNCSettingsViewModel.AppBarSaveTNC(_TNCSettingsViewModel.TNCDeviceSelectedIndex, _TNCSettingsViewModel.State);
        //    //int selectedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;
        //    //_TNCSettingsViewModel.AppBarSaveTNC(selectedIndex);
        //    //_TNCSettingsViewModel.TNCDeviceSelectedIndex = selectedIndex;
        //    return;
        //    if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMail
        //        || _TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailDelete
        //        || _TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailEdit
        //        || _TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailAdd)
        //    {
        //        if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMail)
        //        {
        //            EmailAccount emailAccount = _TNCSettingsViewModel.CurrentMailAccount;

        //            int tncDeviceSelectedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;
        //            TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[_TNCSettingsViewModel.TNCDeviceSelectedIndex];
        //            tncDevice.MailUserName = emailAccount.MailUserName;
        //            tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
        //            await TNCDeviceArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
        //        }
        //        else if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailDelete)
        //        {
        //            int tncDeviceSelectedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;

        //            await EmailAccountArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.UpdateMailState(TNCSettingsViewModel.TNCState.EMail);
        //            _TNCSettingsViewModel.MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
        //            _TNCSettingsViewModel.MailAccountSelectedIndex = Math.Min(EmailAccountArray.Instance.EmailAccountList.Count - 1, _TNCSettingsViewModel._deletedIndex);

        //            EmailAccount emailAccount = _TNCSettingsViewModel.CurrentMailAccount;
        //            TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[_TNCSettingsViewModel.TNCDeviceSelectedIndex];
        //            tncDevice.MailUserName = emailAccount.MailUserName;
        //            tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
        //            await TNCDeviceArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
        //        }
        //        else if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailEdit)
        //        {
        //            EmailAccount emailAccount = _TNCSettingsViewModel.EMailAccountFromUI;
        //            EmailAccountArray.Instance.EmailAccountList[_TNCSettingsViewModel._modifiedEmailAccountSelectedIndex] = emailAccount;
        //            await EmailAccountArray.Instance.SaveAsync();

        //            _TNCSettingsViewModel.UpdateMailState(TNCSettingsViewModel.TNCState.EMail);
        //            _TNCSettingsViewModel.MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
        //            _TNCSettingsViewModel.MailAccountSelectedIndex = _TNCSettingsViewModel._modifiedEmailAccountSelectedIndex;

        //            int tncDeviceSelectedIndex = _TNCSettingsViewModel.TNCDeviceSelectedIndex;
        //            TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList[_TNCSettingsViewModel.TNCDeviceSelectedIndex];
        //            tncDevice.MailUserName = emailAccount.MailUserName;     // TODO double user name??
        //            tncDevice.Name = $"{SharedData.EMailPreample}{emailAccount.MailUserName}";
        //            await TNCDeviceArray.Instance.SaveAsync();

        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.TNCDeviceSelectedIndex = tncDeviceSelectedIndex;
        //        }
        //        else if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.EMailAdd)
        //        {
        //            EmailAccount emailAccount = _TNCSettingsViewModel.EMailAccountFromUI;
        //            EmailAccountArray.Instance.EmailAccountList.Add(emailAccount);
        //            await EmailAccountArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.UpdateMailState(TNCSettingsViewModel.TNCState.EMail);

        //            _TNCSettingsViewModel.MailAccountListSource = new ObservableCollection<EmailAccount>(EmailAccountArray.Instance.EmailAccountList);
        //            _TNCSettingsViewModel.MailAccountSelectedIndex = _TNCSettingsViewModel._modifiedEmailAccountSelectedIndex;
        //            // No need to update connected devides because we always select the last used email account
        //        }
        //    }
        //    else
        //    {
        //        if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.TNCAdd)
        //        {
        //            if (string.IsNullOrEmpty(_TNCSettingsViewModel.NewTNCDeviceName))
        //            {
        //                //await Utilities.ShowMessageDialogAsync("The new TNC Device must have a name.", "Add TNC Device error");
        //                return;
        //            }
        //            TNCDevice tncDevice = _TNCSettingsViewModel.TNCDeviceFromUI;
        //            _TNCSettingsViewModel.CurrentTNCDevice = tncDevice;
        //            TNCDeviceArray.Instance.TNCDeviceListUpdate(TNCDeviceArray.Instance.TNCDeviceList.Count - 1, tncDevice);
        //            await TNCDeviceArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.TNCDeviceSelectedIndex = TNCDeviceArray.Instance.TNCDeviceList.Count - 1;
        //            _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNC;
        //        }
        //        else if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.TNCEdit)
        //        {
        //            TNCDevice tncDevice = _TNCSettingsViewModel.TNCDeviceFromUI;
        //            _TNCSettingsViewModel.CurrentTNCDevice = tncDevice;
        //            TNCDeviceArray.Instance.TNCDeviceListUpdate(_TNCSettingsViewModel.TNCDeviceSelectedIndex, tncDevice);
        //            await TNCDeviceArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNC;
        //            //_logHelper.Log(LogLevel.Trace, $"Saving, Comport: {tncDevice.CommPort.Comport}");
        //        }
        //        else if (_TNCSettingsViewModel.State == TNCSettingsViewModel.TNCState.TNCDelete)
        //        {
        //            await TNCDeviceArray.Instance.SaveAsync();
        //            _TNCSettingsViewModel.TNCDeviceListSource = new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);
        //            _TNCSettingsViewModel.TNCDeviceSelectedIndex = Math.Min(TNCDeviceArray.Instance.TNCDeviceList.Count - 1, _TNCSettingsViewModel._deletedIndex);
        //            _TNCSettingsViewModel.State = TNCSettingsViewModel.TNCState.TNC;
        //        }
        //    }
        //    ConnectDevices.Visibility = Visibility.Visible;
        //    newTNCDeviceName.Visibility = Visibility.Collapsed;

        //    // Disable Save button
        //    _TNCSettingsViewModel.ResetChangedProperty();
        //    _TNCSettingsViewModel.IsAppBarSaveEnabled = false;

        //}
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

        private async void AddressBookAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            AddressBook addressBook = AddressBook.Instance;

            ContentDialogAddressBookEntry contentDialog = new ContentDialogAddressBookEntry();
            AddressBookEntry emptyEntry = new AddressBookEntry()
            {
                Callsign = "",
                NameDetail = "",
                City = "",
                Prefix = "",
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
                emptyEntry.City = contentDialog.AddressBookCity;
                emptyEntry.Prefix = contentDialog.AddressBookPrefix;
                emptyEntry.BBSPrimary = contentDialog.SelectedPrimaryBBS;
                emptyEntry.BBSSecondary = contentDialog.SelectedSecondaryBBS;
                bool success = UserAddressArray.Instance.AddAddressAsync(emptyEntry);
                if (!success)
                {
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("Error adding a new address book entry.");
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
            //AddressBook addressBook = AddressBook.Instance;

            UserAddressArray.Instance.DeleteAddress(_selectedEntry);

            ContactsCVS.Source = AddressBook.Instance.GetContactsGrouped();
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
            contentDialog.AddressBookCity = _selectedEntry.City;
            contentDialog.AddressBookPrefix = _selectedEntry.Prefix;
            contentDialog.SelectedPrimaryBBS = _selectedEntry.BBSPrimary;
            contentDialog.SelectedSecondaryBBS = _selectedEntry.BBSSecondary;
            contentDialog.SelectedLastUsedBBS = _selectedEntry.LastUsedBBS;
            contentDialog.LastUsedBBSDate = _selectedEntry.LastUsedBBSDate;

            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _selectedEntry.Callsign = contentDialog.AddressBookCallsign;
                _selectedEntry.NameDetail = contentDialog.AddressBookName;
                _selectedEntry.City = contentDialog.AddressBookCity;
                _selectedEntry.Prefix = contentDialog.AddressBookPrefix;
                _selectedEntry.BBSPrimary = contentDialog.SelectedPrimaryBBS;
                _selectedEntry.BBSSecondary = contentDialog.SelectedSecondaryBBS;
                _selectedEntry.LastUsedBBS = contentDialog.SelectedLastUsedBBS;
                _selectedEntry.LastUsedBBSDate = contentDialog.LastUsedBBSDate;

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
            await UserAddressArray.Instance.SaveAsync();

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
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("The Distribution List already exists.", "Close", "DistributionList List Error");
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
                    await ContentDialogs.ShowSingleButtonContentDialogAsync("The Distribution List does not exist.", "DistributionList List Error");
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
