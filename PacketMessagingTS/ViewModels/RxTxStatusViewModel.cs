using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.Views;

using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class RxTxStatViewModel : BaseViewModel
    {

        public RxTxStatusPage StatusPage
        { get; set; }

        public CoreDispatcher Dispatcher
        { get; set; }

        //public AppWindow AppWindow
        //{ get; set; }

        private string rxTxStatus;
        public string RxTxStatus
        {
            get => rxTxStatus;
            //set => Set(ref rxTxStatus, value);
            set => rxTxStatus = value;
        }

        public string AddRxTxStatus
        {
            get => rxTxStatus;
            set
            {
                string status = rxTxStatus + value;
                //StatusPage.AddTextToStatusWindow.Text = status;
                //Debug.Write(value);
                Set(ref rxTxStatus, status);

                RxTxStatusPage.rxtxStatusPage.ScrollText();
            }
        }

        //public void AddStatusWindowText(string text)
        //{
        //    StatusPage.AddTextToStatusWindow(text);
        //}

        private ICommand _abortCommand;

        public ICommand AbortCommand => _abortCommand ?? (_abortCommand = new RelayCommand(AbortConnectionAsync));

        public async void AbortConnectionAsync()
        {
            _viewLifetimeControl.StartViewInUse();
            await ApplicationViewSwitcher.SwitchAsync(WindowManagerService.Current.MainViewId,
                ApplicationView.GetForCurrentView().Id,
                ApplicationViewSwitchingOptions.ConsolidateViews);
            _viewLifetimeControl.StopViewInUse();
        }

        private ViewLifetimeControl _viewLifetimeControl;

        public void Initialize(ViewLifetimeControl viewLifetimeControl, CoreDispatcher dispatcher)
        {
            _viewLifetimeControl = viewLifetimeControl;
            _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
            Dispatcher = dispatcher;
        }

        public void Initialize(ViewLifetimeControl viewLifetimeControl, RxTxStatusPage statusPage)
        {
            _viewLifetimeControl = viewLifetimeControl;
            _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
            StatusPage = statusPage;
        }

        private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        {
            _viewLifetimeControl.Released -= OnViewLifetimeControlReleased;
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                WindowManagerService.Current.SecondaryViews.Remove(_viewLifetimeControl);
            });
        }

    }

}
