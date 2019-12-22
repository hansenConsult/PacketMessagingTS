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
    public sealed partial class CityFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CityFormsPage>();
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
            return _cityFormsViewModel.FormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _cityFormsViewModel.FormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _cityFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
