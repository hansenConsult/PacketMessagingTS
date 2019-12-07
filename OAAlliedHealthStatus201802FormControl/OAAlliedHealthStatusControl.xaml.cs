using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            InitializeToggleButtonGroups();

            sent.IsChecked = true; ;
            packet.IsChecked = true;
        }

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "XSC_OA_MuniStatus_v20130101";

        public override string PacFormType => "OA Municipal Status";

        public override void AppendDrillTraffic()
        { }

        //private string facilityName;
        //public string FacilityName
        //{
        //    get => GetProperty(ref facilityName);
        //    set => SetProperty(ref facilityName, value, true);
        //}

        private string facilityType;
        public string FacilityType
        {
            get => GetProperty(ref facilityType);
            set => SetProperty(ref facilityType, value, true);
        }

        public override string CreateSubject()
        {
            return (MessageNo + '_' + Severity?.ToUpper()[0] + '/' + HandlingOrder?.ToUpper()[0] + "_OAAlliedHealth_" + facilityNameTextBox.Text + '_' + facilityTypeTextBox.Text);
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
