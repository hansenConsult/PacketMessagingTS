using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

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
            new ComboBoxPackItItem("Drill or Exercise", LightGreenBrush),
            new ComboBoxPackItItem("Full Activation", PinkBrush),
            new ComboBoxPackItItem("Monitoring", OrangeBrush),
            new ComboBoxPackItItem("Not Activated", LightGreenBrush),
            new ComboBoxPackItItem("Unvailable", PinkBrush),
            new ComboBoxPackItItem("Limited Activation", PinkBrush),
        };

        readonly List<ComboBoxPackItItem> Decon = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Exceeded", BlackBrush),
            new ComboBoxPackItItem("Full", PinkBrush),
            new ComboBoxPackItItem("Interactive", YellowBrush),
            new ComboBoxPackItItem("Open", LightGreenBrush),
        };


        public HavBedReportControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "form-mhoc-beds-status";

        public override string PacFormType => "HAVBEDSTATUS";

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_HAvBed_{(hospitalName.SelectedItem as ComboBoxPackItItem)?.Item}";
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
