using MetroLog;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;

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
    }
}
