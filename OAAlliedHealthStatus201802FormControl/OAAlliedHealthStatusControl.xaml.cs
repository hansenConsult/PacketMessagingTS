using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using SharedCode;
using FormControlBaseClass;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAAlliedHealthStatus201802FormControl
{
    [FormControl(
                FormControlName = "XSC_OA_AlliedHealthStatus_v201802",
                FormControlMenuName = "OA AlliedHealthstatus",
                FormControlType = FormControlAttribute.FormType.HospitalForm)
    ]



    public sealed partial class OAAlliedHealthStatusControl : FormControlBase
    {
        public OAAlliedHealthStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeControls();

            sent.IsChecked = true; ;
            packet.IsChecked = true;
        }

        public override string Severity
        {
            get { return severity.GetRadioButtonCheckedState(); }
            set { severity.SetRadioButtonCheckedState(value); }
        }

        public override string HandlingOrder
        {
            get { return handlingOrder.GetRadioButtonCheckedState(); }
            set { handlingOrder.SetRadioButtonCheckedState(value); }
        }

        public override string PacFormName => "XSC_OA_MuniStatus_v20130101";

        public override string PacFormType => "OA Municipal Status";

        public override string CreateSubject()
        {
            return (MessageNo + '_' + Severity?.ToUpper()[0] + '/' + HandlingOrder?.ToUpper()[0] + "_OAAlliedHealth_" + facilityName.Text + '_' + facilityType.Text);
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:SCCo OA Allied Health Status (which4)",
                "# JS-ver. PR-4.4-1.4, 12/06/18",
                "# FORMFILENAME: XSC_OA_AlliedHealthStatus_v201802.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

    }
}
