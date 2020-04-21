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

        protected override int FormsPagePivotSelectedIndex
        {
            get => TestFormsViewModel.TestFormsPagePivotSelectedIndex;
            set => TestFormsViewModel.TestFormsPagePivotSelectedIndex = value;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            TestFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
