using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;
using MetroLog;
using SharedCode;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CityFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<HospitalFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public CityFormsViewModel _cityFormsViewModel { get; } = Singleton<CityFormsViewModel>.Instance;

        public CityFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeCity);
            PopulateFormsPagePivot();
        }

        protected override int GetFormsPagePivotSelectedIndex()
        {
            return _cityFormsViewModel.CityFormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _cityFormsViewModel.CityFormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _cityFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
