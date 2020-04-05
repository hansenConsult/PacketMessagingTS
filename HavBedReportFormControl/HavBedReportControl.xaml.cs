using System.Collections.Generic;

using FormControlBaseClass;
using FormControlBasicsNamespace;
using FormUserControl;

using SharedCode;
using SharedCode.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HavBedReportFormControl
{
    [FormControl(
        FormControlName = "form-mhoc-beds-status",
        FormControlMenuName = "XSC HAvBed Report",
        FormControlType = FormControlAttribute.FormType.HospitalForm)
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HavBedReportControl : FormControlBase
    {
        readonly List<ComboBoxPackItItem> CommandCenterStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Available", LightGreenBrush),
            new ComboBoxPackItItem("Drill or Exercise", LightGrayBrush),
            new ComboBoxPackItItem("Full Activation", PinkBrush),
            new ComboBoxPackItItem("Monitoring", OrangeBrush),
            new ComboBoxPackItItem("Not Activated", LightGreenBrush),
            new ComboBoxPackItItem("Unvailable", PinkBrush),
            new ComboBoxPackItItem("Limited Activation", PinkBrush),
        };

        readonly List<ComboBoxPackItItem> Decon = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Exceeded", BlackBrush, WhiteBrush),
            new ComboBoxPackItItem("Full", PinkBrush),
            new ComboBoxPackItItem("Interactive", YellowBrush),
            new ComboBoxPackItItem("Open", LightGreenBrush),
        };


        public HavBedReportControl()
        {
            this.InitializeComponent();

            DependencyObject panelName = (formHeaderControl as FormHeaderUserControl).Panel;
            ScanControls(panelName, formHeaderControl);

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            panelName = (radioOperatorControl as RadioOperatorUserControl).Panel;
            ScanControls(panelName, radioOperatorControl);

            FormHeaderControl.HeaderString1 = "SCCo Medical Health Branch - HAvBed Report";
            FormHeaderControl.HeaderSubstring = "EMResource: c190320";
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "form-mhoc-beds-status";

        public override string PacFormType => "HAVBEDSTATUS";

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_HAvBed_{(hospitalName.SelectedItem as ComboBoxPackItItem)?.Item}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-mhoc-beds-status.html",
                $"#V: 2.17-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

    }
}
