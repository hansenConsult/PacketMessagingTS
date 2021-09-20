using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class ICS309HeaderControl : UserControl
    {
        public readonly ICS309HeaderViewModel ViewModel = ICS309HeaderViewModel.Instance;

        public ICS309HeaderControl()
        {
            InitializeComponent();
        }
    }
}
