using System;

using PacketMessagingTS.Services;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

using MetroLog;
using MetroLog.Targets;
using System.Collections.Generic;
using PacketMessagingTS.Models;
using PacketMessagingTS.Helpers;
using Windows.Storage;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Views;

namespace PacketMessagingTS
{
    public sealed partial class App : Application
    {
        public static Dictionary<string, TacticalCallsignData> _tacticalCallsignDataDictionary;
        public static List<TacticalCallsignData> _TacticalCallsignDataList;

        private const string PropertiesDictionaryFileName = "PropertiesDictionary";
        public static Dictionary<string, object> Properties { get; set; }

        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            //Suspending += App_SuspendingAsync;

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            GlobalCrashHandler.Configure(); // Write a FATAL entry, wait until all of the targets have finished writing, then call Application.Exit.

            _tacticalCallsignDataDictionary = new Dictionary<string, TacticalCallsignData>();
            _TacticalCallsignDataList = new List<TacticalCallsignData>();

            TacticalCallsignData tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "County Agencies",
                FileName = "CountyTacticalCallsigns.xml",
                StartString = "Santa Clara County Cities/Agencies",
                BulletinFileName = "SCCo Packet Tactical Calls"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "non County Agencies",
                FileName = "NonCountyTacticalCallsigns.xml",
                StartString = "Other (non-SCCo) Agencies",
                BulletinFileName = "SCCo Packet Tactical Calls"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "Local Mountain View",
                FileName = "MTVTacticalCallsigns.xml",
                TacticallWithBBS = "MTVEOC",
                StartString = "#Mountain View Tactical Call List",
                StopString = "#MTV001 thru MTV010 also permissible",
                RawDataFileName = "Tactical_Calls.txt"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "Local Cupertino",
                FileName = "CUPTacticalCallsigns.xml",
                TacticallWithBBS = "CUPEOC",
                StartString = "# Cupertino OES",
                StopString = "# City of Palo Alto",
                RawDataFileName = "Tactical_Calls.txt"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "County Hospitals",
                FileName = "HospitalsTacticalCallsigns.xml",
                TacticallWithBBS = "HOSDOC",
                StartString = "# SCCo Hospitals Packet Tactical Call Signs",
                StopString = "# HOS001 - HOS010",
                RawDataFileName = "Tactical_Calls.txt"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "All County",
                FileName = "AllCountyTacticalCallsigns.xml",
                StartString = "",
                BulletinFileName = "https://scc-ares-races.org/activities/showtacticalcalls.php"
            };
            _tacticalCallsignDataDictionary.Add(tacticalCallsignData.FileName, tacticalCallsignData);
            _TacticalCallsignDataList.Add(tacticalCallsignData);

            tacticalCallsignData = new TacticalCallsignData()
            {
                AreaName = "User Address Book",
                FileName = "UserAddressBook.xml",
                StartString = "",
                BulletinFileName = ""
            };

        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Properties = await localFolder.ReadAsync<Dictionary<string, object>>(PropertiesDictionaryFileName);
            if (Properties is null)
            {
                Properties = new Dictionary<string, object>();
            }

#if DEBUG
            SharedData.TestFilesFolder = await localFolder.CreateFolderAsync("TestFiles", CreationCollisionOption.OpenIfExists);
#endif

            SharedData.MetroLogsFolder = await localFolder.CreateFolderAsync("MetroLogs", CreationCollisionOption.OpenIfExists);
            SharedData.ArchivedMessagesFolder = await localFolder.CreateFolderAsync("ArchivedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.DeletedMessagesFolder = await localFolder.CreateFolderAsync("DeletedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.DraftMessagesFolder = await localFolder.CreateFolderAsync("DraftMessages", CreationCollisionOption.OpenIfExists);
            SharedData.ReceivedMessagesFolder = await localFolder.CreateFolderAsync("ReceivedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.SentMessagesFolder = await localFolder.CreateFolderAsync("SentMessages", CreationCollisionOption.OpenIfExists);
            SharedData.UnsentMessagesFolder = await localFolder.CreateFolderAsync("UnsentMessages", CreationCollisionOption.OpenIfExists);

            foreach (var tacticalCallsignType in _TacticalCallsignDataList)
            {
                tacticalCallsignType.TacticalCallsigns = await TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);
            }

            await BBSDefinitions.Instance.OpenAsync();  //"ms-appx:///Assets/pdffile.pdf"
            await TNCDeviceArray.Instance.OpenAsync();
            await EmailAccountArray.Instance.OpenAsync();
            await ProfileArray.Instance.OpenAsync();
            await AddressBook.Instance.OpenAsync();
            AddressBook.Instance.CreateAddressBook();
            await DistributionListArray.Instance.OpenAsync();

            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            SharedData.SettingsContainer = localSettings.CreateContainer("SettingsContainer", ApplicationDataCreateDisposition.Always);

            SharedData.FilesInInstalledLocation = await Package.Current.InstalledLocation.GetFilesAsync();

            Singleton<SettingsViewModel>.Instance.W1XSCStatusUp = Utilities.GetProperty<bool>("W1XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W2XSCStatusUp = Utilities.GetProperty<bool>("W2XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W3XSCStatusUp = Utilities.GetProperty<bool>("W3XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W4XSCStatusUp = Utilities.GetProperty<bool>("W4XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W5XSCStatusUp = Utilities.GetProperty<bool>("W5XSCStatusUp");


            Singleton<TNCSettingsViewModel>.Instance.TNCDeviceSelectedIndex = Utilities.GetProperty("TNCDeviceSelectedIndex");

            Singleton<TNCSettingsViewModel>.Instance.MailAccountSelectedIndex = Utilities.GetProperty("MailAccountSelectedIndex");

            Singleton<PacketSettingsViewModel>.Instance.ProfileSelectedIndex = Utilities.GetProperty("ProfileSelectedIndex");

            Singleton<MainViewModel>.Instance.MainPagePivotSelectedIndex = Utilities.GetProperty("MainPagePivotSelectedIndex");

            Singleton<FormsViewModel>.Instance.FormsPagePivotSelectedIndex = Utilities.GetProperty("FormsPagePivotSelectedIndex");

            Singleton<IdentityViewModel>.Instance.TacticalCallsignSelectedIndex = Utilities.GetProperty("TacticalCallsignSelectedIndex");

            //await UpdatePacFormsFiles.SyncPacFormFoldersAsync();

            bool displayIdentity = Properties.TryGetValue("DisplayIdentityAtStartup", out object displayIdentityAtStartup);
            bool displayProfiles = Properties.TryGetValue("DisplayProfileOnStart", out object displayProfileOnStart);
            if (displayIdentity && (bool)displayIdentityAtStartup)
            {
                NavigationService.Navigate(typeof(SettingsPage), 1);
            }
            else if (displayProfiles && (bool)displayProfileOnStart)
            {
                NavigationService.Navigate(typeof(SettingsPage), 2);
            }

        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.SaveAsync<Dictionary<string, object>>(PropertiesDictionaryFileName, Properties);

            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();


            deferral.Complete();
        }

        private async void App_SuspendingAsync(object sender, SuspendingEventArgs args)
        {
            SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.SaveAsync<Dictionary<string, object>>(PropertiesDictionaryFileName, Properties);

            deferral.Complete();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
    }
}
