using System.Windows.Input;

using MetroLog;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.Views;
using SharedCode;

using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class RxTxStatViewModel : BaseViewModel
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<RxTxStatViewModel>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        private string rxTxStatus;
        public string RxTxStatus
        {
            get => rxTxStatus;
            set => Set(ref rxTxStatus, value);
            //get
            //{
            //    ScrollViewer _scrollViewer =  RxTxStatusPage.rxtxStatusPage._scrollViewer;
            //    _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
            //    return rxTxStatus;
            //}
            //set
            //{
            //    Set(ref rxTxStatus, value);
            //    ScrollViewer _scrollViewer = RxTxStatusPage.rxtxStatusPage._scrollViewer;
            //    _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
            //}
        }

        private string appendText;
        public string AppendRxTxStatus
        {
            get => appendText;
            set
            {
                appendText = value;
                //string status = rxTxStatus + appendText;
                RxTxStatus = rxTxStatus + appendText;
                _logHelper.Log(LogLevel.Trace, $"text: {appendText}");
            }
            //string status = rxTxStatus + appendedStatus;
            //RxTxStatus = status;
        }

        //public void AppendRxTxStatus(string appendedStatus)
        //{
        //    string status = rxTxStatus + appendedStatus;
        //    RxTxStatus = status;
        //}

        private double _viewControlHeight = 600;
        public double ViewControlHeight
        {
            get => GetProperty(ref _viewControlHeight);
            set => SetProperty(ref _viewControlHeight, value, true);
        }

        private double _viewControlWidth = 500;
        public double ViewControlWidth
        {
            get => GetProperty(ref _viewControlWidth);
            set => SetProperty(ref _viewControlWidth, value, true);
        }

        private double _RxTxStatusAppWindowOffsetX;
        public double RxTxStatusAppWindowOffsetX
        {
            get
            {
                GetProperty(ref _RxTxStatusAppWindowOffsetX);
                if (_RxTxStatusAppWindowOffsetX == 0)
                {
                    _RxTxStatusAppWindowOffsetX = 50;
                }
                return _RxTxStatusAppWindowOffsetX;
            }
            set => SetProperty(ref _RxTxStatusAppWindowOffsetX, value, true);
        }

        private double _RxTxStatusAppWindowOffsetY;
        public double RxTxStatusAppWindowOffsetY
        {
            get
            {
                GetProperty(ref _RxTxStatusAppWindowOffsetY);
                if (_RxTxStatusAppWindowOffsetY == 0)
                {
                    _RxTxStatusAppWindowOffsetY = 20;
                }
                return _RxTxStatusAppWindowOffsetY;
            }
            set => SetProperty(ref _RxTxStatusAppWindowOffsetY, value, true);
        }

        //public async void CloseStatusWindowAsync()
        //{
        //    //_viewLifetimeControl.StartViewInUse();
        //    Rect rect = ViewLifetimeControl.GetBounds();
        //    ViewControlHeight = rect.Height;

        //    await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
        //        ApplicationView.GetForCurrentView().Id,
        //        ApplicationViewSwitchingOptions.ConsolidateViews);
        //    //_viewLifetimeControl.StopViewInUse();
        //}

        private ICommand _abortCommand;

        public ICommand AbortCommand => _abortCommand ?? (_abortCommand = new RelayCommand(AbortConnection));

        public void AbortConnection()
        {
            CommunicationsService.CreateInstance().AbortConnection();
        }

        public ViewLifetimeControl ViewLifetimeControl { get; private set; }

        public AppWindow RxTxAppWindow { get; set; }

        public Frame RxTxAppWindowFrame { get; set; }

        public CoreDispatcher AppWindowDispatcher { get; set; }

        //public void Initialize(ViewLifetimeControl viewLifetimeControl)
        //{
        //    ViewLifetimeControl = viewLifetimeControl;
        //    ViewLifetimeControl.Released += OnViewLifetimeControlReleased;
        //}

        //public void Initialize(ViewLifetimeControl viewLifetimeControl, CoreDispatcher dispatcher)
        //{
        //    _viewLifetimeControl = viewLifetimeControl;
        //    _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        //    Dispatcher = dispatcher;
        //}

        //public void Initialize(ViewLifetimeControl viewLifetimeControl, RxTxStatusPage statusPage)
        //{
        //    _viewLifetimeControl = viewLifetimeControl;
        //    _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        //    StatusPage = statusPage;
        //}

        //private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        //{
        //    ViewLifetimeControl.Released -= OnViewLifetimeControlReleased;            
        //    await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        WindowManagerService.Current.SecondaryViews.Remove(ViewLifetimeControl);
        //    });
        //}

    }

}
