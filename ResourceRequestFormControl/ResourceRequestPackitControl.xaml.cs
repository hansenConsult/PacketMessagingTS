using System.Collections.Generic;

using FormControlBaseClass;

using FormUserControl;

using SharedCode;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FormControlBaseMvvmNameSpace;
using SharedCode.Models;
using PacketMessagingTS.Core.Helpers;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceRequestPackItFormControl
{
    [FormControl(
    FormControlName = "form-resource-request",
    FormControlMenuName = "Resource Request",
    FormControlType = FormControlAttribute.FormType.CountyForm)
    ]
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResourceRequestPackItControl : FormControlBase
    {
        readonly ResourceRequestPackItControlViewModel ViewModel = new ResourceRequestPackItControlViewModel();

        public ResourceRequestPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            FormHeaderControl.ViewModel.HeaderString1 = "Resource Request";
            FormHeaderControl.ViewModel.HeaderSubstring = "PDF: 20250811";
            FormHeaderControl.SetToLocation("County EOC");
            FormHeaderControl.ViewModel.PIF = "1.0";

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;
        
        public override string PacFormType => "ResourceRequest";
        
        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override void SetPracticeField(string practiceField)
        {
            FormHeaderControl.SetToLocation("County EOC");  //XSCEOC
            FormHeaderControl.SetToICSPosition("Planning Section");
            //if (practiceField == null)
            //{
            //    incidentName.Text = "";
            //}
            //else
            //{
            //    incidentName.Text = practiceField;
            //}
            UpdateFormFieldsRequiredColors();       // TODO check this. Needed for subject
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            //return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_EOC213RR_{incidentName.Text}";
            return "";
        }



    }
}