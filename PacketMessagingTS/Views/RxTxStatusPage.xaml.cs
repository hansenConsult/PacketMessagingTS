using System;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.Core;
using Windows.UI.ViewManagement;
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

        public RxTxStatViewModel RxTxStatusViewmodel { get; } = Singleton<RxTxStatViewModel>.Instance;

        //private AppWindow _appWindow;
        ViewLifetimeControl _viewLifetimeControl;

        public static RxTxStatusPage rxtxStatusPage;

        public RxTxStatusPage()
        {
            InitializeComponent();

            rxtxStatusPage = this;
            //RxTxStatusViewModel.StatusPage = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //_appWindow = e.Parameter as AppWindow;
            //RxTxStatusViewModel.AppWindow = _appWindow;

            base.OnNavigatedTo(e);
            // To avoid problems with a new thread generated for the rxTXStatus edit control
            bool success = Singleton<RxTxStatViewModel>.UpdateInstance();

            _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
            RxTxStatusViewmodel.Initialize(_viewLifetimeControl, Dispatcher);
            RxTxStatusViewmodel.StatusPage = this;
            RxTxStatusViewmodel.RxTxStatus = "";

            //_currentHeight = Height;
            //_viewLifetimeControl.StartViewInUse();
            //_viewLifetimeControl.
            //Arrange
            //Height = 400;
            //_viewLifetimeControl.StopViewInUse();
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

        //public void AddTextToStatusWindow(string text)
        //{
        //    RxTxStatusViewmodel.AddRxTxStatus = text;
        //}

        //private async void AbortButton_ClickAsync(object sender, RoutedEventArgs e)
        //{
        //    //await _appWindow.CloseAsync();
        //    //RxTxStatusViewModel.RxTxStatus = "";
        //    _viewLifetimeControl.StartViewInUse();
        //    await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
        //        ApplicationView.GetForCurrentView().Id,
        //        ApplicationViewSwitchingOptions.ConsolidateViews);
        //    _viewLifetimeControl.StopViewInUse();
        //}

        int i = 0;
        //string textBoxText = "";
        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;

            RxTxStatusViewmodel.AddRxTxStatus = $"\nTest text{i}";
            //ScrollText();
        }

        ScrollViewer sv = null;
        public void ScrollText()
        {            
            //if (sv is null)
            //{
                sv = FindVisualChild<ScrollViewer>(textBoxStatus);
            //}
            //if (sv.Visibility == Visibility.Visible)
            {
                bool? viewChanged = sv?.ChangeView(null, sv.ExtentHeight, 1.0f, true);
            }
        }

        private void TextBoxStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            //return;
            ScrollViewer sv = FindVisualChild<ScrollViewer>(textBoxStatus);
            if (sv.Visibility == Visibility.Visible)
            {
                bool? viewChanged = sv?.ChangeView(null, sv.ExtentHeight, 1.0f, true);

                //_logHelper.Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {sv.ExtentHeight}");
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
