using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MutualAidRequestFormControl
{
    [FormControl(
        FormControlName = "form-oa-mutual-aid-request-v2",
        FormControlMenuName = "RACES Mutual Aid Request",
        FormControlType = FormControlAttribute.FormType.CountyForm,
        FormControlMenuIndex = 6)
    ]


    public sealed partial class MutualAidRequestControl : FormControlBase
    {
        public MutualAidRequestControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            formHeaderControl.NamePanel1Visibility = false;
            formHeaderControl.HeaderString1 = "Santa Clara County RACES -- Mutual Aid Request";
            formHeaderControl.HeaderSubstring = "Version: 190614";

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "XSC_Mutual_Aid_Request";

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_";
        }

        public override string GetPacFormName() => "form-oa-mutual-aid-request-v2";


    }
}
