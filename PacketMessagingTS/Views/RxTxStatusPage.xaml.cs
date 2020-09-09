using System;
using System.Threading;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;
using Windows.Foundation;
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
        //public RxTxStatViewModel RxTxStatusViewmodel { get; } = new RxTxStatViewModel();

        public ViewLifetimeControl _viewLifetimeControl;

        AppWindow appWindow;

        public static RxTxStatusPage rxtxStatusPage;
        private ScrollViewer _scrollViewer;

        public RxTxStatusPage()
        {
            InitializeComponent();

            //bool success = Singleton<RxTxStatViewModel>.UpdateInstance();

            rxtxStatusPage = this;
            //RxTxStatusViewModel.StatusPage = this;

            Loaded += AppWindowPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // To avoid problems with a new thread generated for the rxTXStatus edit control
//            bool success = Singleton<RxTxStatViewModel>.UpdateInstance();

//            _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
//            //RxTxStatusViewmodel.Initialize(_viewLifetimeControl, Dispatcher);
//            RxTxStatusViewmodel.Initialize(_viewLifetimeControl);
//            //RxTxStatusViewmodel.StatusPage = this;
//            RxTxStatusViewmodel.RxTxStatus = "";

//            _viewLifetimeControl.Height = RxTxStatusViewmodel.ViewControlHeight;
//            _viewLifetimeControl.Width = RxTxStatusViewmodel.ViewControlWidth;

            //_scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
            //_scrollViewer = FindScrollViewer();
        }

        public async void CloseStatusWindowAsync()
        {
            //_viewLifetimeControl.StartViewInUse();
            Rect rect = _viewLifetimeControl.GetBounds();
            RxTxStatusViewmodel.ViewControlHeight = rect.Height;

            await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
                ApplicationView.GetForCurrentView().Id,
                ApplicationViewSwitchingOptions.ConsolidateViews);
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

        private ScrollViewer FindScrollViewer()
        {
            var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                _scrollViewer = (ScrollViewer)obj;
                //viewChanged = _scrollViewer.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
                break;
            }
            return _scrollViewer;
        }

        public void AddTextToStatusWindow(string text)
        {
            //if (_scrollViewer is null)
            //{
            //    _scrollViewer = FindScrollViewer();
            //}

            bool? viewChanged = false;
            textBoxStatus.Text += text;
            var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                _scrollViewer = (ScrollViewer)obj;
                viewChanged = _scrollViewer.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
                break;
            }
            //Thread.Sleep(1);
            //_logHelper.Log(LogLevel.Trace, $"Scrolled: {viewChanged}, Height: {_scrollViewer.ExtentHeight} text: {text}");
        }

        int i = 0;
        //string textBoxText = "";
        //public void TestAddRxTxStatus()
        //{
        //    i++;
        //    RxTxStatusViewmodel.AppendRxTxStatus = $"Test text{i}\n";

        //    //ScrollText();
        //}

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;

            RxTxStatusViewmodel.AppendRxTxStatus = $"Test text{i}\r";
            //RxTxStatusViewmodel.AppendRxTxStatus($"Test text{i}\r");
            //RxTxStatusViewmodel.RxTxStatus = $"\nTest text{i}";            

            //ScrollText();

            //AddTextToStatusWindow($"Test text{i}\r");
        }

        //ScrollViewer scrollViewer = null;
        public async void ScrollText()
        //public void ScrollText()
        {
            if (_scrollViewer is null)
            {
                _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
            }

            //await rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            await _viewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool? _viewChanged = _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
            });
        }

        private void TextBoxStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
            //for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            //{
            //    object obj = VisualTreeHelper.GetChild(grid, i);
            //    if (!(obj is ScrollViewer)) continue;
            //    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f, true);
            //    break;
            //}

            //return;
            //_viewLifetimeControl.StartViewInUse();
            if (_scrollViewer is null)
            {
                _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
            }
            //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //            await RxTxStatusViewmodel.ViewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //            {
            //                //_viewLifetimeControl.StartViewInUse();
            bool? viewChanged = _scrollViewer.ChangeView(null, _scrollViewer.ExtentHeight, 1.0f, true);
            //                    //if (viewChanged != true)
            //                    //Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {_scrollViewer.ExtentHeight} text: {RxTxStatusViewmodel.AppendRxTxStatus}");
            //                //_viewLifetimeControl.StopViewInUse();
            //            });
            //_viewLifetimeControl.StopViewInUse();
        }

        private void TextBoxStatus_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.Foundation.Rect bounds = Window.Current.CoreWindow.Bounds;
            _viewLifetimeControl.Height = bounds.Bottom;
        }

        private void AppWindowPage_Loaded(object sender, RoutedEventArgs e)
        {
            appWindow = RxTxStatusViewmodel.RxTxAppWindow;

            //appWindow.Changed += AppWindow_Changed;
            //appWindow.CloseRequested += AppWindow_CloseRequested;   // Does not work
        }

        private void AppWindow_CloseRequested(AppWindow sender, AppWindowCloseRequestedEventArgs args)
        {
            AppWindowPlacement appWindowPlacement = sender.GetPlacement();
            Singleton<RxTxStatViewModel>.Instance.ViewControlWidth = appWindowPlacement.Size.Width;
            Singleton<RxTxStatViewModel>.Instance.ViewControlHeight = appWindowPlacement.Size.Height;
            //Singleton<RxTxStatViewModel>.Instance.RxTxStatusAppWindowOffset = appWindowPlacement.Offset;
        }

        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            AppWindowPlacement appWindowPlacement = sender.GetPlacement();
            if (args.DidSizeChange)
            {
                if (appWindowPlacement.Size == new Size(0, 0))
                    return;

                Singleton<RxTxStatViewModel>.Instance.ViewControlWidth = appWindowPlacement.Size.Width;
                Singleton<RxTxStatViewModel>.Instance.ViewControlHeight = appWindowPlacement.Size.Height;
            }
            //Singleton<RxTxStatViewModel>.Instance.RxTxStatusAppWindowOffset = appWindowPlacement.Offset;
        }
    }
}
