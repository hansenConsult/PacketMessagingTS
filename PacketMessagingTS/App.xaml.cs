using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using MetroLog;
using MetroLog.Targets;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Views;

using SharedCode;
using SharedCode.Models;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;


namespace PacketMessagingTS
{
    public sealed partial class App : Application
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<App>();
        private static  LogHelper _logHelper = new LogHelper(log);

        private SuspendingDeferral suspendDeferral;

        private const string PropertiesDictionaryFileName = "PropertiesDictionary";
        public static Dictionary<string, object> Properties { get; set; }

        private const string TacticalCallsArrayFileName = "TacticallsArray";
        public static int[] TacticalCallsArray { get; set; }


        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
            //Suspending += App_SuspendingAsync;

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            GlobalCrashHandler.Configure(); // Write a FATAL entry, wait until all of the targets have finished writing, then call Application.Exit.

            TacticalCallsigns.CreateTacticalCallsignsData();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            _logHelper.Log(LogLevel.Info, "--------------------------------------");
            _logHelper.Log(LogLevel.Info, "Packet Messaging Application started");

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Properties = await localFolder.ReadAsync<Dictionary<string, object>>(PropertiesDictionaryFileName) ?? new Dictionary<string, object>();
            if (Properties is null)
            {
                Properties = new Dictionary<string, object>();
            }

            //TacticalCallsArray = await localFolder.ReadAsync<int[]>(TacticalCallsArrayFileName);

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
            SharedData.PrintMessagesFolder = await localFolder.CreateFolderAsync("PrintMessages", CreationCollisionOption.OpenIfExists);

            foreach (TacticalCallsignData tacticalCallsignType in TacticalCallsigns._TacticalCallsignDataList)
            {
                //Task<TacticalCallsigns> taskFinished = TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);

                // taskFinished = TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);
                //tacticalCallsignType.TacticalCallsigns = taskFinished.Result;
                tacticalCallsignType.TacticalCallsigns = await TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);
            }
            //await UserCallsigns.OpenAsync();

            await TNCDeviceArray.Instance.OpenAsync();
            await BBSDefinitions.Instance.OpenAsync();  //"ms-appx:///Assets/pdffile.pdf"
            await EmailAccountArray.Instance.OpenAsync();
            await ProfileArray.Instance.OpenAsync();
            await UserAddressArray.Instance.OpenAsync();
            AddressBook.Instance.CreateAddressBook();
            await DistributionListArray.Instance.OpenAsync();
            await HospitalRollCall.Instance.OpenAsync();

            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            SharedData.SettingsContainer = localSettings.CreateContainer("SettingsContainer", ApplicationDataCreateDisposition.Always);

            //SharedData.FilesInInstalledLocation = await Package.Current.InstalledLocation.GetFilesAsync();

            Singleton<SettingsViewModel>.Instance.W1XSCStatusUp = Utilities.GetProperty<bool>("W1XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W2XSCStatusUp = Utilities.GetProperty<bool>("W2XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W3XSCStatusUp = Utilities.GetProperty<bool>("W3XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W4XSCStatusUp = Utilities.GetProperty<bool>("W4XSCStatusUp");
            Singleton<SettingsViewModel>.Instance.W5XSCStatusUp = Utilities.GetProperty<bool>("W5XSCStatusUp");

            //await UpdatePacFormsFiles.SyncPacFormFoldersAsync();

            BulletinHelpers.CreateBulletinDictionaryFromFiles();

            bool displayIdentity = Properties.TryGetValue("DisplayIdentityAtStartup", out object displayIdentityAtStartup);
            bool callsignExist = Properties.TryGetValue("UserCallsign", out object userCallsign);
            bool displayProfiles = Properties.TryGetValue("DisplayProfileOnStart", out object displayProfileOnStart);
            // Show Identity dialog if Call-sign is empty
            if (displayIdentity && (bool)displayIdentityAtStartup || !callsignExist || string.IsNullOrEmpty((string)userCallsign))
            { 
                NavigationService.Navigate(typeof(SettingsPage), 1);
            }
            else if (displayProfiles && (bool)displayProfileOnStart)
            {
                NavigationService.Navigate(typeof(SettingsPage), 2);
            }
            Utilities.SetApplicationTitle();

            SharedData.Assemblies = new List<Assembly>();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] assemblies = currentDomain.GetAssemblies();
            //System.Reflection.Assembly[] AppDomain.GetAssemblies
            foreach (Assembly assembly in assemblies)
            {
                if (!assembly.FullName.Contains("FormControl"))
                    continue;

                //_logHelper.Log(LogLevel.Info, $"Assembly: {assembly}");
                SharedData.Assemblies.Add(assembly);
            }
            //_logHelper.Log(LogLevel.Info, $"Assembly count: {SharedData.Assemblies.Count}");
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            _logHelper.Log(LogLevel.Trace, "Entered OnActivated");

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
            Deferral deferral = e.GetDeferral();

            _logHelper.Log(LogLevel.Trace, "Entered App_EnteredBackground");

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.SaveAsync<Dictionary<string, object>>(PropertiesDictionaryFileName, Properties);

            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();

            await Singleton<MainViewModel>.Instance.UpdateDownloadedBulletinsAsync();

            deferral.Complete();
        }

        //private void App_Resuming(object sender, object e)
        //{
        //    Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        //}

        private async void App_SuspendingAsync(object sender, SuspendingEventArgs args)
        {
            try
            {
                suspendDeferral = args.SuspendingOperation.GetDeferral();

                //DateTimeOffset deadline = args.SuspendingOperation.Deadline;
                _logHelper.Log(LogLevel.Trace, $"Entered App_SuspendingAsync.");

                using (var session = new ExtendedExecutionSession())
                {
                    session.Reason = ExtendedExecutionReason.SavingData;
                    session.Description = "Saving application state.";
                    session.Revoked += ExtendedExecutionSessionRevoked;

                    ExtendedExecutionResult result = await session.RequestExtensionAsync();
                    switch (result)
                    {
                        case ExtendedExecutionResult.Allowed:
                            // We can perform a longer save operation (e.g., upload to the cloud).
                            try
                            {
                                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                                await localFolder.SaveAsync(PropertiesDictionaryFileName, Properties);

                                await Singleton<MainViewModel>.Instance.UpdateDownloadedBulletinsAsync();
                            }
                            catch (TaskCanceledException te)
                            {
                                _logHelper.Log(LogLevel.Trace, $"Extended Execution Error. {te.Message}");
                            }
                            break;
                        default:
                        case ExtendedExecutionResult.Denied:
                            // We must perform a fast save operation.
                            _logHelper.Log(LogLevel.Trace, $"Extended Execution denied");
                            //MainPage.DisplayToast("Performing a fast save operation.");
                            //await Task.Delay(TimeSpan.FromSeconds(1));
                            //MainPage.DisplayToast("Fast save complete.");
                            break;
                    }

                    session.Revoked -= ExtendedExecutionSessionRevoked;
                }

                //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                //await localFolder.SaveAsync(PropertiesDictionaryFileName, Properties);

                //await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();

                //await Singleton<MainViewModel>.Instance.UpdateDownloadedBulletinsAsync();

            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"App_SuspendingAsync exception. {e.Message}");
            }
            finally
            {
                suspendDeferral?.Complete();
                suspendDeferral = null;
            }
        }

        private void ExtendedExecutionSessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            //If session is revoked, make the OnSuspending event handler stop or the application will be terminated
            //if (cancellationTokenSource != null) { cancellationTokenSource.Cancel(); }

            switch (args.Reason)
            {
                case ExtendedExecutionRevokedReason.Resumed:
                    // A resumed app has returned to the foreground
                    _logHelper.Log(LogLevel.Error, $"Extended execution revoked due to returning to foreground.");
                    break;

                case ExtendedExecutionRevokedReason.SystemPolicy:
                    //An app can be in the foreground or background when a revocation due to system policy occurs
                    _logHelper.Log(LogLevel.Error, $"Extended execution revoked due to system policy.");
                    break;
            }

            suspendDeferral?.Complete();
            suspendDeferral = null;
        }

        private void App_Resuming(object sender, object e)
        {
            Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            _logHelper.Log(LogLevel.Trace, "Entered OnBackgroundActivated");

            await ActivationService.ActivateAsync(args);
        }
    }
}
