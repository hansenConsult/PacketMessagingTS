using System;

using MetroLog;

using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.Core;
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

        //public RxTxStatViewModel RxTxStatusViewmodel { get; } = new RxTxStatViewModel();
        private RxTxStatViewModel RxTxStatusViewmodel = RxTxStatViewModel.Instance;

        public ViewLifetimeControl _viewLifetimeControl;

        public static RxTxStatusPage Current;
        private ScrollViewer _scrollViewer;

        public RxTxStatusPage()
        {
            InitializeComponent();

            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
            RxTxStatusViewmodel.Initialize(_viewLifetimeControl);

            _viewLifetimeControl.Height = RxTxStatusViewmodel.ViewControlHeight;
            _viewLifetimeControl.Width = RxTxStatusViewmodel.ViewControlWidth;
        }

        //public async void CloseStatusWindowAsync()
        //{
        //    Rect rect = _viewLifetimeControl.GetBounds();
        //    RxTxStatusViewmodel.ViewControlHeight = rect.Height;

        //    await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
        //        ApplicationView.GetForCurrentView().Id,
        //        ApplicationViewSwitchingOptions.ConsolidateViews);
        //    //_viewLifetimeControl.StopViewInUse();
        //}

        //private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is T)
        //            return (T)child;
        //        else
        //        {
        //            T childOfChild = FindVisualChild<T>(child);
        //            if (childOfChild != null)
        //                return childOfChild;
        //        }
        //    }
        //    return null;
        //}

        private ScrollViewer FindScrollViewer()
        {
            ScrollViewer scrollViewer = null;
            var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                scrollViewer = (ScrollViewer)obj;
                break;
            }
            return scrollViewer;
        }

        private string FitStatusTextToTextBox(string text)
        {
            if (_scrollViewer is null)
            {
                _scrollViewer = FindScrollViewer();
            }

            string status = textBoxStatus.Text + text;
            while (_scrollViewer != null && _scrollViewer.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true))
            {
                int index = status.IndexOf('\r');
                status = status.Substring(index + 1);
            }
            return status;
        }

        public async void AddTextToStatusWindow(string text)
        {
            if (Dispatcher.HasThreadAccess)
            {
               textBoxStatus.Text = FitStatusTextToTextBox(text);
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    textBoxStatus.Text = FitStatusTextToTextBox(text);
                });
            }
        }

        //public void AddTextToStatusWindow(string text)
        //{

        //    //if (_scrollViewer is null)
        //    //{
        //    //    _scrollViewer = FindScrollViewer();
        //    //}

        //    bool? viewChanged = false;
        //    textBoxStatus.Text += text;

        //    //textBoxStatus.Text = FitStatusTextToTextBox();

        //    var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
        //    for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
        //    {
        //        object obj = VisualTreeHelper.GetChild(grid, i);
        //        if (!(obj is ScrollViewer)) continue;
        //        _scrollViewer = (ScrollViewer)obj;
        //        viewChanged = _scrollViewer.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
        //        break;
        //    }
        //    //Thread.Sleep(1);
        //    //_logHelper.Log(LogLevel.Trace, $"Scrolled: {viewChanged}, Height: {_scrollViewer.ExtentHeight} text: {text}");
        //}

        int i = 0;
        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            i++;
            AddTextToStatusWindow($"Test text{i}\r");
        }

        //ScrollViewer scrollViewer = null;
        //public async void ScrollText()
        ////public void ScrollText()
        //{
        //    if (_scrollViewer is null)
        //    {
        //        _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
        //    }

        //    //await rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
        //    await _viewLifetimeControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        bool? _viewChanged = _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
        //    });
        //}

        //private async void TextBoxStatus_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //var grid = (Grid)VisualTreeHelper.GetChild(textBoxStatus, 0);
        //    //for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
        //    //{
        //    //    object obj = VisualTreeHelper.GetChild(grid, i);
        //    //    if (!(obj is ScrollViewer)) continue;
        //    //    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f, true);
        //    //    break;
        //    //}

        //    //return;
        //    //_viewLifetimeControl.StartViewInUse();
        //    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        if (_scrollViewer is null)
        //        {
        //            _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
        //        }
        //        //await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //        bool? viewChanged = _scrollViewer.ChangeView(null, _scrollViewer.ExtentHeight, 1.0f, true);
        //    });
        //}

        //private void TextBoxStatus_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    Windows.Foundation.Rect bounds = Window.Current.CoreWindow.Bounds;
        //    _viewLifetimeControl.Height = bounds.Bottom;
        //}
    }
}
