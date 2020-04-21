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
    public sealed partial class FormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<FormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        //public FormsViewModel _formsViewModel { get; } = Singleton<FormsViewModel>.Instance;


        public FormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeNone);
            _formControlAttributeList.AddRange(_attributeListTypeCounty);
            PopulateFormsPagePivot();
        }

        protected override int FormsPagePivotSelectedIndex
        {
            get => _formsViewModel.FormsPagePivotSelectedIndex;
            set => _formsViewModel.FormsPagePivotSelectedIndex = value;
        }

        //protected override int GetFormsPagePivotSelectedIndex()
        //{
        //    return _formsViewModel.FormsPagePivotSelectedIndex;
        //}

        //protected override void SetFormsPagePivotSelectedIndex(int index)
        //{
        //    _formsViewModel.FormsPagePivotSelectedIndex = index;
        //}

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _formsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
