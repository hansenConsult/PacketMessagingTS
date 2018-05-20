using System;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();

            //if (SharedData.CurrentProfile == null)
            //{
            //    SharedData.CurrentProfile = ProfileArray.Instance.Profiles[0];
            //}

            //foreach (BBSData bbs in BBSDefinitions.Instance.BBSDataList)
            //{
            //    if (SharedData.CurrentProfile.BBS == bbs.Name)
            //    {
            //        SharedData.CurrentBBS = bbs;
            //        break;
            //    }
            //}

            //foreach (TNCDevice tncDevice in TNCDeviceArray.Instance.TNCDeviceList)
            //{
            //    if (SharedData.CurrentProfile.TNC == tncDevice.Name)
            //    {
            //        SharedData.CurrentTNCDevice = tncDevice;
            //        break;
            //    }
            //}

        }
    }
}
