using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HospitalFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<HospitalFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public HospitalFormsViewModel _hospitalFormsViewModel { get; } = Singleton<HospitalFormsViewModel>.Instance;


        public HospitalFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeHospital);
            PopulateFormsPagePivot();
        }


        protected override int GetFormsPagePivotSelectedIndex()
        {
            return _hospitalFormsViewModel.FormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _hospitalFormsViewModel.FormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _hospitalFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
