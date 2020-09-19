using System;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.Views;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class RxTxStatViewModel : BaseViewModel
    {
        //private string rxTxStatus;
        //public string RxTxStatus
        //{
        //    get => rxTxStatus;
        //    set => Set(ref rxTxStatus, value);
        //    //get
        //    //{
        //    //    ScrollViewer _scrollViewer =  RxTxStatusPage.rxtxStatusPage._scrollViewer;
        //    //    _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
        //    //    return rxTxStatus;
        //    //}
        //    //set
        //    //{
        //    //    Set(ref rxTxStatus, value);
        //    //    ScrollViewer _scrollViewer = RxTxStatusPage.rxtxStatusPage._scrollViewer;
        //    //    _scrollViewer?.ChangeView(0.0f, _scrollViewer.ExtentHeight, 1.0f, true);
        //    //}
        //}

        //private string appendText;
        //public string AppendRxTxStatus
        //{
        //    get => appendText;
        //    set
        //    {
        //        appendText = value;
        //        RxTxStatus = rxTxStatus + appendText; ;
        //    }
        //}

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

        public async void CloseStatusWindowAsync()
        {
            //_viewLifetimeControl.StartViewInUse();
            //Rect rect = ViewLifetimeControl.GetBounds();
            Size rect = RxTxStatusPage.Current.XamlRoot.Size;
            ViewControlHeight = rect.Height;

            await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
                ApplicationView.GetForCurrentView().Id,
                ApplicationViewSwitchingOptions.ConsolidateViews);
            //_viewLifetimeControl.StopViewInUse();
        }

        private ICommand _abortCommand;

        public ICommand AbortCommand => _abortCommand ?? (_abortCommand = new RelayCommand(AbortConnection));

        public static void AbortConnection() => CommunicationsService.Current.AbortConnection();

        public ViewLifetimeControl ViewLifetimeControl { get; private set; }

        public void Initialize(ViewLifetimeControl viewLifetimeControl)
        {
            ViewLifetimeControl = viewLifetimeControl;
            ViewLifetimeControl.Released += OnViewLifetimeControlReleased;
        }

        private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        {
            ViewLifetimeControl.Released -= OnViewLifetimeControlReleased;            
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                WindowManagerService.Current.SecondaryViews.Remove(ViewLifetimeControl);
            });
        }

    }
}
