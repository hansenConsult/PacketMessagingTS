using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MetroLog;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;
using SharedCode;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class ShellPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<SettingsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public ShellViewModel ViewModel { get; } = Singleton<ShellViewModel>.Instance;

        private List<DeviceListEntry> _listOfDevices;
        // DeviceWatcher vars
        private static Dictionary<DeviceWatcher, String> mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, String>();

        private static bool _watchersSuspended = false;
        private static bool _watchersStarted = false;
        private bool _isAllDevicesEnumerated = false;

        ComportComparer _comportComparer;



        public ShellPage()
        {
            InitializeComponent();
            HideNavViewBackButton();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView);

            // Serial ports
            ViewModel.CollectionOfSerialDevices = new ObservableCollection<string>();
            //_listOfBluetoothDevices = new List<DeviceInformation>();
            //CollectionOfBluetoothDevices = new ObservableCollection<DeviceInformation>();
            _comportComparer = new ComportComparer();
            _listOfDevices = new List<DeviceListEntry>();

            mapDeviceWatchersToDeviceSelector = new Dictionary<DeviceWatcher, String>();
            _watchersStarted = false;
            _watchersSuspended = false;

            _isAllDevicesEnumerated = false;

            // Initialize the desired device watchers so that we can watch for when devices are connected/removed
            InitializeDeviceWatchers();
            StartDeviceWatchers();

        }

        private void HideNavViewBackButton()
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6))
            {
                navigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            }
        }

        private void ClearDeviceEntries()
        {
            _listOfDevices.Clear();     // List of all devices
            //_listOfSerialDevices.Clear();
            ViewModel.CollectionOfSerialDevices.Clear();
            //_listOfBluetoothDevices.Clear();
            //CollectionOfBluetoothDevices.Clear();
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

            _watchersStarted = false;
        }

        /// <summary>
        /// Searches through the existing list of devices for the first DeviceListEntry that has
        /// the specified device Id.
        /// </summary>
        /// <param name="deviceId">Id of the device that is being searched for</param>
        /// <returns>DeviceListEntry that has the provided Id; else a nullptr</returns>
        private DeviceListEntry FindDevice(String deviceId)
        {
            if (deviceId != null)
            {
                foreach (DeviceListEntry entry in _listOfDevices)
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
            if (_watchersStarted)
            {
                _watchersSuspended = true;
                StopDeviceWatchers();
            }
            else
            {
                _watchersSuspended = false;
            }
        }

        /// <summary>
        /// See OnAppSuspension for why we are starting the device watchers again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAppResume(Object sender, Object args)
        {
            if (_watchersSuspended)
            {
                _watchersSuspended = false;
                StartDeviceWatchers();
            }
        }

        private void RemoveDeviceFromList(String deviceId)
        {
            // Remove bluetooth devices as well  TODO

            //SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceId);

            //if (serialDevice != null)
            //{
            //    //_listOfSerialDevices.Remove(serialDevice);
            //    // Sort list
            //    //_listOfSerialDevices = _listOfSerialDevices.OrderBy(s => s.PortName, comportComparer).ToList();
            //    //ViewModel.CollectionOfSerialDevices = new ObservableCollection<SerialDevice>(_listOfSerialDevices);
            //    ViewModel.CollectionOfSerialDevices.Remove(serialDevice);
            //    ComPortListSource.Source = ViewModel.CollectionOfSerialDevices.OrderBy(s => s.PortName, _comportComparer);
            //}
            // Removes the device entry from the internal list
            var deviceEntry = FindDevice(deviceId);

            _listOfDevices.Remove(deviceEntry);
        }

        /// <summary>
        /// We will remove the device from the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformationUpdate"></param>
        private void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            try
            {
                //await this.Dispatcher.RunAsync(
                //    CoreDispatcherPriority.Normal,
                //    new DispatchedHandler(async () =>
                //    {
                RemoveDeviceFromList(deviceInformationUpdate.Id);
                //}));
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"Remove serial device error, {e.Message}");
            }
        }

        private async void AddDeviceToListAsync(DeviceInformation deviceInformation, string deviceSelector)
        {
            try
            {
                // search the device list for a device with a matching interface ID
                var match = FindDevice(deviceInformation.Id);

                // Add the device if it's new
                if (match is null)
                {
                    // Create a new element for this device interface, and queue up the query of its
                    // device information
                    match = new DeviceListEntry(deviceInformation, deviceSelector);

                    // Add the new element to the end of the list of devices
                    _listOfDevices.Add(match);
                }
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"{e.Message}, Device = {deviceInformation.Id}");
            }

            if (deviceInformation.Pairing.IsPaired)
            {
                // Bluetooth device
                try
                {
                    if (deviceInformation.Kind == DeviceInformationKind.AssociationEndpoint)
                    {
                        //_listOfBluetoothDevices.Add(deviceInformation);
                        //CollectionOfBluetoothDevices = new ObservableCollection<DeviceInformation>(_listOfBluetoothDevices);
                        //CollectionOfBluetoothDevices.Add(deviceInformation);
                        //ComNameListSource.Source = CollectionOfBluetoothDevices;
                    }
                }
                catch (Exception ex)
                {
                    _logHelper.Log(LogLevel.Error, $"Add Bluetooth device: { ex.Message}");
                }
            }
            else
            {
                try
                {
                    if (!deviceInformation.Id.Contains("Bluetooth"))
                    {
                        // USB serial port
                        SerialDevice serialDevice = await SerialDevice.FromIdAsync(deviceInformation.Id);
                        if (serialDevice != null)
                        {
                            ViewModel.CollectionOfSerialDevices.Add(serialDevice.PortName);
                            //ComPortListSource.Source = ViewModel.CollectionOfSerialDevices.OrderBy(s => s, _comportComparer);
                            //ViewModel.CollectionOfSerialDevices
                            //    = (ObservableCollection<string>)ViewModel.CollectionOfSerialDevices.OrderBy(s => s, _comportComparer);
                            serialDevice.Dispose();     // Necessary to avoid crash on removed device
                        }
                    }
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Error, $"{e.Message}, Device = {deviceInformation.Id}");
                }
            }
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
            _isAllDevicesEnumerated = true;

            // Select current TNC device
            //comboBoxComPort.SelectedValue = _TNCSettingsViewModel.CurrentTNCDevice.CommPort.Comport;

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

        private void InitializeDeviceWatchers()
        {
            // Target all Serial Devices present on the system
            string deviceSelector = SerialDevice.GetDeviceSelector();

            // Create a device watcher to look for instances of the Serial Device that match the device selector
            // used earlier.
            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

            // Allow the EventHandlerForDevice to handle device watcher events that relates or effects our device (i.e. device removal, addition, app suspension/resume)
            AddDeviceWatcher(deviceWatcher, deviceSelector);

            //// Bluetooth devices device selector
            //string bluetoothDeviceSelector = BluetoothDevice.GetDeviceSelector();
            //deviceWatcher = DeviceInformation.CreateWatcher(bluetoothDeviceSelector);
            //AddDeviceWatcher(deviceWatcher, bluetoothDeviceSelector);
        }

        private void StartDeviceWatchers()
        {
            // Start all device watchers
            _watchersStarted = true;
            _isAllDevicesEnumerated = false;

            foreach (DeviceWatcher deviceWatcher in mapDeviceWatchersToDeviceSelector.Keys)
            {
                if ((deviceWatcher.Status != DeviceWatcherStatus.Started)
                    && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Start();
                }
            }
        }

        private void AddDeviceWatcher(DeviceWatcher deviceWatcher, String deviceSelector)
        {
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAddedAsync);
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);
            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(OnDeviceEnumerationComplete);

            mapDeviceWatchersToDeviceSelector.Add(deviceWatcher, deviceSelector);
        }

    }
}
