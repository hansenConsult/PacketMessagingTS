using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    //public class FormControlAttributes
    //{
    //    public string FormControlName
    //    { get; private set; }

    //    public string FormControlMenuName
    //    { get; private set; }

    //    public FormControlAttribute.FormType FormControlType
    //    { get; private set; }

    //    public StorageFile FormControlFileName
    //    { get; set; }

    //    public FormControlAttributes(string formControlType, string formControlMenuName, FormControlAttribute.FormType formType, StorageFile formControlFileName)
    //    {
    //        FormControlName = formControlType;
    //        FormControlMenuName = formControlMenuName;
    //        FormControlType = formType;
    //        FormControlFileName = formControlFileName;
    //    }
    //}

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<FormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public FormsViewModel _formsViewModel { get; } = Singleton<FormsViewModel>.Instance;

        //private enum MessageOrigin
        //{
        //    Archived,
        //    Deleted,
        //    Draft,
        //    Received,
        //    Sent,
        //    Unsent,
        //    New
        //}
        //private MessageOrigin _messageOrigin = MessageOrigin.New;

        //PacketMessage _packetMessage;
        //bool _loadMessage = false;

        //FormControlBase _packetForm;
        //SendFormDataControl _packetAddressForm;
        ////SimpleMessageHeader _headerControl;

        //List<FormControlAttributes> _attributeListTypeNone = new List<FormControlAttributes>();
        //List<FormControlAttributes> _attributeListTypeCounty = new List<FormControlAttributes>();
        //List<FormControlAttributes> _attributeListTypeCity = new List<FormControlAttributes>();
        //List<FormControlAttributes> _attributeListTypeHospital = new List<FormControlAttributes>();

        //private List<FormControlAttributes> _formControlAttributeList;

        //private PrintHelper printHelper;

        //public FormControlBase PacketForm
        //{
        //    get => _packetForm;
        //}

        public FormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeNone);
            _formControlAttributeList.AddRange(_attributeListTypeCounty);
            PopulateFormsPagePivot();
        }

        protected override int GetFormsPagePivotSelectedIndex()
        {
            return _formsViewModel.FormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _formsViewModel.FormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _formsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
