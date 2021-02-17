using System;
using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HospitalRollCallFormControl
{
    [FormControl(
        FormControlName = "hospital_roll_call",
        FormControlMenuName = "Hospital RollCall",
        FormControlType = FormControlAttribute.FormType.HospitalForm,
        FormControlMenuIndex = 4)
    ]

    public sealed partial class HospitalRollCallControl : FormControlBase
    {
        public HospitalRollCallControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string GetPacFormName() => "hospital_roll_call";

        public override string PacFormType => "ICS-213 Test";

        public override string MessageNo
        {
            get => base.MessageNo;
            set
            {
                base.MessageNo = value;
                OriginMsgNo = value;
            }
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override void AppendDrillTraffic() {}

        public override string CreateOutpostData(ref PacketMessage packetMessage) => throw new NotImplementedException();

        public override string CreateSubject()
        {
            //return $"{messageNo.Text}_{HandlingOrder?.ToUpper()[0]}_ICS213_{subject.Text}";
            return $"_{HandlingOrder?.ToUpper()[0]}_ICS213_";
        }

        //public ObservableCollection<Hospital> DataGridSource => new ObservableCollection<Hospital>(HospitalRollCall.Instance.HospitalList);
    }

}
