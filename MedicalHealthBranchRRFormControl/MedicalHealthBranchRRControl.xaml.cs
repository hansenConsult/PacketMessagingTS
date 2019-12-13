using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MedicalHealthBranchRRFormControl
{
    [FormControl(
        FormControlName = "form-medical-resource-request",
        FormControlMenuName = "XSC Medical Resource Request",
        FormControlType = FormControlAttribute.FormType.HospitalForm)
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MedicalHealthBranchRRControl : FormControlBase
    {
        public MedicalHealthBranchRRControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "form-medical-resource-request";

        public override string PacFormType => "XSC_MedicalResourceRequest";

        public override string PIFString => "PIF: 3.1";

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                Set(ref _msgDate, value);
                requestDate.Text = _msgDate;
            }
        }

        public override string MsgTime
        {
            get => _msgTime;
            set
            {
                string time = TimeCheck(value);
                Set(ref _msgTime, time);
                requestTime.Text = time;
            }
        }

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_MedResReq_{requestingFacility.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-medical-resource-request.html",
                $"#V: 2.18c-{PIFString}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

    }
}
