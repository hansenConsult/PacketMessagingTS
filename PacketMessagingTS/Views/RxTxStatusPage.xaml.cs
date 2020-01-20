using System;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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

        ViewLifetimeControl _viewLifetimeControl;

        public static RxTxStatusPage rxtxStatusPage;
        private ScrollViewer _scrollViewer;

        public RxTxStatusPage()
        {
            InitializeComponent();

            rxtxStatusPage = this;
            //RxTxStatusViewModel.StatusPage = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // To avoid problems with a new thread generated for the rxTXStatus edit control
            bool success = Singleton<RxTxStatViewModel>.UpdateInstance();

            _viewLifetimeControl = e.Parameter as ViewLifetimeControl;
            //RxTxStatusViewmodel.Initialize(_viewLifetimeControl, Dispatcher);
            RxTxStatusViewmodel.Initialize(_viewLifetimeControl);
            //RxTxStatusViewmodel.StatusPage = this;
            RxTxStatusViewmodel.RxTxStatus = "";

            _viewLifetimeControl.Height = RxTxStatusViewmodel.ViewControlHeight;
            _viewLifetimeControl.Width = RxTxStatusViewmodel.ViewControlWidth;
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

        //public void AddTextToStatusWindow(string text)
        //{
        //    RxTxStatusViewmodel.AddRxTxStatus = text;
        //}

        int i = 0;
        //string textBoxText = "";
        public void TestAddRxTxStatus()
        {
            i++;
            RxTxStatusViewmodel.AddRxTxStatus = $"Test text{i}\n";

            //ScrollText();
        }

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;
            
            RxTxStatusViewmodel.AddRxTxStatus = $"Test text{i}\r";
            //RxTxStatusViewmodel.RxTxStatus = $"\nTest text{i}";            

            //ScrollText();
        }

        //ScrollViewer scrollViewer = null;
        public async void ScrollText()
        //public void ScrollText()
        {
            if (_scrollViewer is null)
            {
                _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
            }

            await rxtxStatusPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //ScrollViewer _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
                bool? _viewChanged = _scrollViewer?.ChangeView(null, _scrollViewer.ExtentHeight, null, true);
            });

 //           ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);

            //double textBoxHeight = textBoxStatus.ActualHeight;
            ////while (scrollViewer.ExtentHeight > textBoxHeight)
            //{
            //    int index = RxTxStatusViewmodel.RxTxStatus.IndexOfAny(new char[] { '\n', '\r' });
            //    if (index >= 0)
            //    {
            //        RxTxStatusViewmodel.RxTxStatus = RxTxStatusViewmodel.RxTxStatus.Substring(index + 1);
            //    }
            //}
            //_logHelper.Log(LogLevel.Trace, $"TextBox Hight: {textBoxHeight}, ExtendHeight: {scrollViewer.ExtentHeight}");

            //}
            //if (scrollViewer.Visibility == Visibility.Visible)
            //{
 //           bool? viewChanged = scrollViewer?.ChangeView(null, scrollViewer.ExtentHeight, 1.0f, true);
            //_logHelper.Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {scrollViewer.ExtentHeight}");
            //}
        }

        private async void TextBoxStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_scrollViewer is null)
            {
                _scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);
            }
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool? viewChanged = _scrollViewer.ChangeView(null, _scrollViewer.ExtentHeight, 1.0f, true);

                //_logHelper.Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {_scrollViewer.ExtentHeight}");
            });
        }

        private void TextBoxStatus_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.Foundation.Rect bounds = Window.Current.CoreWindow.Bounds;
            _viewLifetimeControl.Height = bounds.Bottom;
        }
    }
}
