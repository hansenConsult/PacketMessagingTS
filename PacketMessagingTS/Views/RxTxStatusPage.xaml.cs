using System;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.Views
{
    public sealed partial class RxTxStatusPage : Page
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<RxTxStatusPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);


        public RxTxStatusViewModel RxTxStatusViewModel { get; } = Singleton<RxTxStatusViewModel>.Instance;

        private AppWindow _appWindow;

        public RxTxStatusPage()
        {
            InitializeComponent();

            RxTxStatusViewModel.StatusPage = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _appWindow = e.Parameter as AppWindow;
            RxTxStatusViewModel.AppWindow = _appWindow;
            base.OnNavigatedTo(e);
            //    _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
            //    _RxTxStatusViewModel.RxTxStatus = "";
            //    _currentHeight = Height;
            //    //_viewLifetimeControl.StartViewInUse();
            //    //Height = 400;
            //    //_viewLifetimeControl.StopViewInUse();
            //    //_viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        }

        private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public void AddTextToStatusWindow(string text)
        {
            //Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = text;
            string current = textBoxStatus.Text;
            textBoxStatus.Text = current + text;
        }

        //private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        //{
        //    _viewLifetimeControl.Released -= OnViewLifetimeControlReleased;
        //    await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        WindowManagerService.Current.SecondaryViews.Remove(_viewLifetimeControl);
        //    });

        //    // The released event is fired on the thread of the window
        //    // it pertains to.
        //    //
        //    // It's important to make sure no work is scheduled on this thread
        //    // after it starts to close (no data binding changes, no changes to
        //    // XAML, creating new objects in destructors, etc.) since
        //    // that will throw exceptions
        //    Window.Current.Close();
        //}

        private async void AbortButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await _appWindow.CloseAsync();
            RxTxStatusViewModel.RxTxStatus = "";
            //_viewLifetimeControl.StartViewInUse();
            //await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
            //    ApplicationView.GetForCurrentView().Id,
            //    ApplicationViewSwitchingOptions.ConsolidateViews);
            //_viewLifetimeControl.StopViewInUse();
        }

        int i = 0;
        string textBoxText = "";
        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            i++;
            textBoxText += $" \nTest text{i}";
            textBoxStatus.Text = textBoxText;

            //RxTxStatusViewModel.AddRxTxStatus = $"\nTest text{i}";
        }

        private void TextBoxStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            return;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(textBoxStatus);
            if (sv.Visibility == Visibility.Visible)
            {
                bool? viewChanged = sv?.ChangeView(null, sv.ExtentHeight, 1.0f, true);

                _logHelper.Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {sv.ExtentHeight}");
            }
            //DependencyObject grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
            //for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            //{
            //    DependencyObject obj = VisualTreeHelper.GetChild(grid, i);
            //    if (!(obj is ScrollViewer)) continue;
            //    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f, true);
            //    break;
            //}
        }

    }
}
