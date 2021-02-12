using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;

namespace PacketMessagingTS.Views
{
    public sealed partial class PrintMessagePage : BasePrintFormsPage
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PrintMessagePage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public PrintMessageViewModel ViewModel { get; } = Singleton<PrintMessageViewModel>.Instance;

        public PrintMessagePage()
        {
            InitializeComponent();
        }
    }

}
