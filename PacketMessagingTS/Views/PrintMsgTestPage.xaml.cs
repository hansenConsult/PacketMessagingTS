using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class PrintMsgTestPage : BasePrintFormsPage
    {
        public PrintMsgTestViewModel ViewModel { get; } = Singleton<PrintMsgTestViewModel>.Instance;


        public PrintMsgTestPage()
        {
            InitializeComponent();
        }

        public override StackPanel ContentArea
        {
            get => contentArea;
        }
    }
}
