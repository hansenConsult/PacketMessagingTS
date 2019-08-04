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
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class RxTxStatusViewModel : BaseViewModel
    {

        public RxTxStatusPage StatusPage
        { get; set; }

        public AppWindow AppWindow
        { get; set; }

        private string rxTxStatus;
        public string RxTxStatus
        {
            get => rxTxStatus;
            set => Set(ref rxTxStatus, value);
        }

        public string AddRxTxStatus
        {
            get => rxTxStatus;
            set
            {
                string status = rxTxStatus + value;
                //StatusPage.AddTextToStatusWindow.Text = status;
                Debug.Write(value);
                Set(ref rxTxStatus, status);

            }
            //set => RxTxStatusPage.AddRxTxStatus = value;
        }

        public void AddStatusWindowText(string text)
        {
            StatusPage.AddTextToStatusWindow(text);
        }

        //private ICommand _abortCommand;

        //public ICommand AbortCommand
        //{
        //    get
        //    {
        //        if (_abortCommand is null)
        //        {
        //            _abortCommand = new RelayCommand(AbortConnectionAsync);
        //        }

        //        return _abortCommand;
        //    }
        //}

        public async Task AbortConnectionAsync()
        {
            //CommunicationsService.CreateInstance().AbortConnection();
            await AppWindow?.CloseAsync();
        }

        //private ViewLifetimeControl _viewLifetimeControl;

        //public void Initialize(ViewLifetimeControl viewLifetimeControl)
        //{
        //    _viewLifetimeControl = viewLifetimeControl;
        //    _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        //}

        //private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        //{
        //    _viewLifetimeControl.Released -= OnViewLifetimeControlReleased;
        //    await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        //    {
        //        WindowManagerService.Current.SecondaryViews.Remove(_viewLifetimeControl);
        //    });
        //}

    }


}
