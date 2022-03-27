using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using MetroLog;
using MetroLog.Targets;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Views;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;


namespace PacketMessagingTS
{
    public sealed partial class App : Application
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<App>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        private SuspendingDeferral suspendDeferral;

        private const string PropertiesDictionaryFileName = "PropertiesDictionary";
        public static Dictionary<string, object> Properties { get; set; }

        //private const string TacticalCallsArrayFileName = "TacticallsArray";
        //public static int[] TacticalCallsArray { get; set; }


        private readonly Lazy<ActivationService> _activationService;
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
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new StreamingFileTarget());
            GlobalCrashHandler.Configure(); // Write a FATAL entry, wait until all of the targets have finished writing, then call Application.Exit.

            TacticalCallsigns.CreateTacticalCallsignsData();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            _logHelper.Log(LogLevel.Info, "--------------------------------------");
            _logHelper.Log(LogLevel.Info, "Packet Messaging Application launched");

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Properties = await localFolder.ReadAsync<Dictionary<string, object>>(PropertiesDictionaryFileName) ?? new Dictionary<string, object>();
            if (Properties is null)
            {
                Properties = new Dictionary<string, object>();
            }

            //TacticalCallsArray = await localFolder.ReadAsync<int[]>(TacticalCallsArrayFileName);
#if DEBUG
            SharedData.TestFilesFolder = await localFolder.CreateFolderAsync("TestFiles", CreationCollisionOption.OpenIfExists);

            // Shows how to process tasks that return a value
            //IAsyncOperation<StorageFolder>[] folders = new IAsyncOperation<StorageFolder>[]
            //{
            //    localFolder.CreateFolderAsync("TestFiles", CreationCollisionOption.OpenIfExists),
            //};
            ////IAsyncOperation<StorageFolder> folders[0] =  localFolder.CreateFolderAsync("TestFiles", CreationCollisionOption.OpenIfExists);

            //List<Task> folderTasks = new List<Task>() { folders[0].AsTask<StorageFolder>() };
            //while (folderTasks.Count > 0)
            //{
            //    Task finishedTask = await Task.WhenAny(folderTasks);
            //    string s = finishedTask.ToString();
                
            //    if (finishedTask == folders[0])
            //    {
            //        SharedData.TestFilesFolder = folders[0].GetResults();
            //    }
            //    folderTasks.Remove(finishedTask);
            //}
#endif

            SharedData.MetroLogsFolder = await localFolder.CreateFolderAsync("MetroLogs", CreationCollisionOption.OpenIfExists);
            SharedData.ArchivedMessagesFolder = await localFolder.CreateFolderAsync("ArchivedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.DeletedMessagesFolder = await localFolder.CreateFolderAsync("DeletedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.DraftMessagesFolder = await localFolder.CreateFolderAsync("DraftMessages", CreationCollisionOption.OpenIfExists);
            SharedData.ReceivedMessagesFolder = await localFolder.CreateFolderAsync("ReceivedMessages", CreationCollisionOption.OpenIfExists);
            SharedData.SentMessagesFolder = await localFolder.CreateFolderAsync("SentMessages", CreationCollisionOption.OpenIfExists);
            SharedData.UnsentMessagesFolder = await localFolder.CreateFolderAsync("UnsentMessages", CreationCollisionOption.OpenIfExists);
            SharedData.PrintMessagesFolder = await localFolder.CreateFolderAsync("PrintMessages", CreationCollisionOption.OpenIfExists);

            //// Moved here for FormMenuIndexDefinitions.Instance.OpenAsync()
            //SharedData.Assemblies = new List<Assembly>();
            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //Assembly[] assemblies = currentDomain.GetAssemblies();
            //foreach (Assembly assembly in assemblies)
            //{
            //    if (!assembly.FullName.Contains("FormControl"))
            //        continue;

            //    //_logHelper.Log(LogLevel.Info, $"Assembly: {assembly}");
            //    SharedData.Assemblies.Add(assembly);
            //}
            ////_logHelper.Log(LogLevel.Info, $"Assembly count: {SharedData.Assemblies.Count}");

            foreach (TacticalCallsignData tacticalCallsignType in TacticalCallsigns.TacticalCallsignDataList)
            {
                //Task<TacticalCallsigns> taskFinished = TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);

                // taskFinished = TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);
                //tacticalCallsignType.TacticalCallsigns = taskFinished.Result;
                tacticalCallsignType.TacticalCallsigns = await TacticalCallsigns.OpenAsync(tacticalCallsignType.FileName);
            }
            //await UserCallsigns.OpenAsync();

            // The following Open() must happen in the right order
            await TNCDeviceArray.Instance.OpenAsync();
            await BBSDefinitions.Instance.OpenAsync();  //"ms-appx:///Assets/pdffile.pdf"
            await EmailAccountArray.Instance.OpenAsync();
            await ProfileArray.Instance.OpenAsync();

            List<Task> tasks = new List<Task>
            {
                UserAddressArray.Instance.OpenAsync(),
                DistributionListArray.Instance.OpenAsync(),
                HospitalRollCall.Instance.OpenAsync(),
                CustomFoldersArray.OpenAsync(),
            };
            while (tasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
            }

            AddressBook.Instance.CreateAddressBook();

            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            SharedData.SettingsContainer = localSettings.CreateContainer("SettingsContainer", ApplicationDataCreateDisposition.Always);

            //SharedData.FilesInInstalledLocation = await Package.Current.InstalledLocation.GetFilesAsync();

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
            foreach (Assembly assembly in assemblies)
            {
                if (!assembly.FullName.Contains("FormControl"))
                    continue;

                //_logHelper.Log(LogLevel.Info, $"Assembly: {assembly}");
                SharedData.Assemblies.Add(assembly);
            }

            await FormMenuIndexDefinitions.Instance.OpenAsync();    // Depends on SharedData.Assemblies
            //_logHelper.Log(LogLevel.Info, $"Assembly count: {SharedData.Assemblies.Count}");
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            //_logHelper.Log(LogLevel.Trace, "Entered OnActivated");

            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
            _logHelper.Log(LogLevel.Fatal, $"Unhandled exception: {e.Message}");
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs args)
        {
            Deferral deferral = args.GetDeferral();

            _logHelper.Log(LogLevel.Trace, "In App_EnteredBackground");
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                await localFolder.SaveAsync(PropertiesDictionaryFileName, Properties);

                await SuspendAndResumeService.Instance.SaveStateAsync();

                await MainViewModel.Instance.UpdateDownloadedBulletinsAsync();
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"{e.Message}");
            }

            deferral.Complete();
        }

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

                                await MainViewModel.Instance.UpdateDownloadedBulletinsAsync();
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

                //await MainViewModel.Instance.UpdateDownloadedBulletinsAsync();

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
            SuspendAndResumeService.Instance.ResumeApp();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            _logHelper.Log(LogLevel.Trace, "Entered OnBackgroundActivated");

            await ActivationService.ActivateAsync(args);
        }
    }
}
