using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Activation;

using SharedCode;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Services
{
    // For more information on understanding and extending activation flow see
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/activation.md
    internal class ActivationService
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<ActivationService>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        private readonly App _app;
        private readonly Lazy<UIElement> _shell;
        private readonly Type _defaultNavItem;

        //private object _lastActivationArgs;

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement> shell = null)
        {
            _app = app;
            _shell = shell;
            _defaultNavItem = defaultNavItem;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            //_logHelper.Log(LogLevel.Trace, "Entered ActivateAsync");

            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (Window.Current.Content is null)
                {
                    // Create a Frame to act as the navigation context and navigate to the first page
                    Window.Current.Content = _shell?.Value ?? new Frame();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            //await HandleActivationAsync(activationArgs);
            //_lastActivationArgs = activationArgs;

            var activationHandler = GetActivationHandlers()
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs))
            {
                //var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
                var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(activationArgs))
                {
                    await defaultHandler.HandleAsync(activationArgs);
                }

                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task InitializeAsync()
        {
            await Singleton<BackgroundTaskService>.Instance.RegisterBackgroundTasksAsync().ConfigureAwait(false);
            await ThemeSelectorService.InitializeAsync().ConfigureAwait(false);
            await WindowManagerService.Current.InitializeAsync();
        }

        //private async Task HandleActivationAsync(object activationArgs)
        //{
        //    var activationHandler = GetActivationHandlers()
        //                                        .FirstOrDefault(h => h.CanHandle(activationArgs));

        //    if (activationHandler != null)
        //    {
        //        await activationHandler.HandleAsync(activationArgs);
        //    }

        //    if (IsInteractive(activationArgs))
        //    {
        //        var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
        //        if (defaultHandler.CanHandle(activationArgs))
        //        {
        //            await defaultHandler.HandleAsync(activationArgs);
        //        }
        //    }
        //}

        private async Task StartupAsync()
        {
            await ThemeSelectorService.SetRequestedThemeAsync();        // WinUI
            await Task.CompletedTask;
        }

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return Singleton<BackgroundTaskService>.Instance;
            yield return Singleton<SuspendAndResumeService>.Instance;
            //yield return Singleton<SchemeActivationHandler>.Instance; // Only used for external activation (Protocol Activated)
        }

        private bool IsInteractive(object args)
        {
            return args is IActivatedEventArgs;
        }

        //internal async Task ActivateFromShareTargetAsync(ShareTargetActivatedEventArgs activationArgs)
        //{
        //    var shareTargetHandler = GetActivationHandlers().FirstOrDefault(h => h.CanHandle(activationArgs));
        //    if (shareTargetHandler != null)
        //    {
        //        await shareTargetHandler.HandleAsync(activationArgs);
        //    }
        //}
        //private void Frame_Navigated(object sender, NavigationEventArgs e)    // WinUI
        //{
        //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack ?
        //        AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        //}

        //private void ActivationService_BackRequested(object sender, BackRequestedEventArgs e) // WinUI
        //{
        //    if (NavigationService.CanGoBack)
        //    {
        //        NavigationService.GoBack();
        //        e.Handled = true;
        //    }
        //}

    }
}
