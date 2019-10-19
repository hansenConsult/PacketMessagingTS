using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Services
{
    public delegate void ViewClosedHandler(ViewLifetimeControl viewControl, EventArgs e);

    // For instructions on testing this service see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/features/multiple-views.md
    // More details about showing multiple views at https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
    public class WindowManagerService
    {
        private static WindowManagerService _current;

        public static WindowManagerService Current => _current ?? (_current = new WindowManagerService());

        // Contains all the opened secondary views.
        public ObservableCollection<ViewLifetimeControl> SecondaryViews { get; } = new ObservableCollection<ViewLifetimeControl>();

        public int MainViewId { get; private set; }

        public CoreDispatcher MainDispatcher { get; private set; }

        public void Initialize()
        {
            MainViewId = ApplicationView.GetForCurrentView().Id;
            MainDispatcher = Window.Current.Dispatcher;
        }

        // Displays a view as a standalone
        // You can use the resulting ViewLifeTimeControl to interact with the new window.
        public async Task<ViewLifetimeControl> TryShowAsStandaloneAsync(string windowTitle, Type pageType)
        {
            ViewLifetimeControl viewControl = await CreateViewLifetimeControlAsync(windowTitle, pageType);
            SecondaryViews.Add(viewControl);
            viewControl.StartViewInUse();
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewControl.Id, ViewSizePreference.Custom, ApplicationView.GetForCurrentView().Id, ViewSizePreference.Default);
            viewControl.StopViewInUse();

            await viewControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool success = viewControl.ResizeView();
            });

            return viewControl;
        }

        // Displays a view in the specified view mode
        public async Task<ViewLifetimeControl> TryShowAsViewModeAsync(string windowTitle, Type pageType, ApplicationViewMode viewMode = ApplicationViewMode.Default)
        {
            ViewLifetimeControl viewControl = await CreateViewLifetimeControlAsync(windowTitle, pageType);
            SecondaryViews.Add(viewControl);
            viewControl.StartViewInUse();
            await ApplicationViewSwitcher.TryShowAsViewModeAsync(viewControl.Id, viewMode);
            viewControl.StopViewInUse();
            return viewControl;
        }

        private async Task<ViewLifetimeControl> CreateViewLifetimeControlAsync(string windowTitle, Type pageType)
        {
            ViewLifetimeControl viewControl = null;

            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                viewControl = ViewLifetimeControl.CreateForCurrentView();
                viewControl.Title = windowTitle;
                //viewControl.Height = 600;
                //viewControl.Width = 500;
                viewControl.StartViewInUse();
                var frame = new Frame();
                frame.RequestedTheme = ThemeSelectorService.Theme;
                frame.Navigate(pageType, viewControl);
                //Rect rect = Window.Current.CoreWindow.Bounds
                Window.Current.Content = frame;
                //Window.Current.Bounds = new Windows.Foundation.Rect(500,60,400,500);
                //bool success = ApplicationView.GetForCurrentView().TryResizeView(viewControlSize);
                Window.Current.Activate();
                //ApplicationView.GetForCurrentView().Title = "";
                ApplicationView.GetForCurrentView().Title = viewControl.Title;
                //bool success = ApplicationView.GetForCurrentView().TryResizeView(viewControlSize);
            });

            return viewControl;
        }

        public bool IsWindowOpen(string windowTitle) => SecondaryViews.Any(v => v.Title == windowTitle);

        //// This method is designed to always be run on the thread that's
        //// binding to the list above
        //public void UpdateTitle(String newTitle, int viewId)
        //{
        //    ViewLifetimeControl foundData;
        //    if (TryFindViewLifetimeControlForViewId(viewId, out foundData))
        //    {
        //        foundData.Title = newTitle;
        //    }
        //    else
        //    {
        //        throw new KeyNotFoundException("Couldn't find the view ID in the collection");
        //    }
        //}

        //// Loop through the collection to find the view ID
        //// This should only be run on the main thread.
        //private bool TryFindViewLifetimeControlForViewId(int viewId, out ViewLifetimeControl foundData)
        //{
        //    foreach (var ViewLifetimeControl in SecondaryViews)
        //    {
        //        if (ViewLifetimeControl.Id == viewId)
        //        {
        //            foundData = ViewLifetimeControl;
        //            return true;
        //        }
        //    }
        //    foundData = null;
        //    return false;
        //}

    }

}
