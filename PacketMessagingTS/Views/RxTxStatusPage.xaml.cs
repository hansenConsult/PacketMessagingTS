using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.ViewModels;

using SharedCode;

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
            //ScrollText();
            //TestAddRxTxStatus();
            //for (int j = 0; j < 100; j++)
            //{
            i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;

            RxTxStatusViewmodel.AddRxTxStatus = $"\nTest text{i}";
            //RxTxStatusViewmodel.RxTxStatus = $"\nTest text{i}";

            ScrollText();
                //}
        }

        //ScrollViewer scrollViewer = null;
        public void ScrollText()
        {
            //if (sv is null)
            //{
            ScrollViewer scrollViewer = FindVisualChild<ScrollViewer>(textBoxStatus);

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
            bool? viewChanged = scrollViewer?.ChangeView(null, scrollViewer.ExtentHeight, 1.0f, true);
            //_logHelper.Log(LogLevel.Trace, $"View Changed: {viewChanged}, ExtendHeight: {scrollViewer.ExtentHeight}");
            //}
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

        private void TextBoxStatus_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Windows.Foundation.Rect bounds = Window.Current.CoreWindow.Bounds;
            _viewLifetimeControl.Height = bounds.Bottom;
        }
    }
}
