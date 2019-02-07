using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MetroLog;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;
using SharedCode;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class RxTxStatusPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<RxTxStatusPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        public RxTxStatusViewModel _RxTxStatusViewModel { get; } = Singleton<RxTxStatusViewModel>.Instance;

        public RxTxStatusPage()
        {
            InitializeComponent();
        }

    }
}
