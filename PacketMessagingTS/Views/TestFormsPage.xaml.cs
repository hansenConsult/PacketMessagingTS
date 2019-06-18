using System.IO;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestFormsPage : BaseFormsPage
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TestFormsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public TestFormsViewModel TestFormsViewModel { get; } = Singleton<TestFormsViewModel>.Instance;

        public TestFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeTestForms);
            PopulateFormsPagePivot();
        }

        protected override int GetFormsPagePivotSelectedIndex()
        {
            return TestFormsViewModel.TestFormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            TestFormsViewModel.TestFormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            TestFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Initialize common helper class and register for printing
            printHelper = new PrintHelper(this);
            printHelper.RegisterForPrinting();

            if (e.Parameter is null)
            {
                // Open last used empty form
                _formsPagePivot.SelectedIndex = GetFormsPagePivotSelectedIndex();
                return;
            }

            // Open a form with content
            int index = 0;
            string packetMessagePath = e.Parameter as string;
            _packetMessage = PacketMessage.Open(packetMessagePath);
            if (_packetMessage is null)
            {
                _logHelper.Log(LogLevel.Error, $"Failed to open {packetMessagePath}");
            }
            else
            {
                _packetMessage.MessageOpened = true;
                string directory = Path.GetDirectoryName(packetMessagePath);
                _loadMessage = true;
                foreach (PivotItem pivotItem in _formsPagePivot.Items)
                {
                    if (pivotItem.Name == _packetMessage.PacFormName) // If PacFormType is not set
                    {
                        _formsPagePivot.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                // Show SimpleMessage header formatted by where the message came from
                if (_packetMessage.PacFormName == "SimpleMessage")
                {
                    if (directory.Contains("Received"))
                    {
                        _messageOrigin = MessageOrigin.Received;
                    }
                    else if (directory.Contains("Sent"))
                    {
                        _messageOrigin = MessageOrigin.Sent;
                    }
                    else
                    {
                        _messageOrigin = MessageOrigin.New;
                    }
                }
                _packetMessage.Save(directory);
            }
        }

    }
}
