using FormControlBaseClass;
using SharedCode;
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
using static SharedCode.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213RRPackItFormControl
{
    [FormControl(
    FormControlName = "XSC_EOC-213RR_v1706",
    FormControlMenuName = "EOC Resource Request",
    FormControlType = FormControlAttribute.FormType.TestForm)
]

    public sealed partial class ICS213RRPackItControl : FormControlBase
    {
        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ReceivedOrSent = "sent";
            HowReceivedSent = "packet";
        }

        public override FormProviders DefaultFormProvider => FormProviders.PacItForm;

        public override string PacFormName => "XSC_EOC-213RR_v1706";

        public override string PacFormType => "XSC_EOC_213RR";

        public override string CreateSubject()
        {
            return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_EOC213RR_" + incidentName.Text);
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            switch (packetMessage.FormProvider)
            {
                case FormProviders.PacForm:
                    outpostData = new List<string>
                    {
                        "!PACF! " + packetMessage.Subject,
                        "# JS:EOC Resource Request (which4)",
                        "# JS-ver. PR-4.4-2.9, 06/29/18",
                        "# FORMFILENAME: XSC_EOC-213RR_v1708.html"
                    };
                    break;
                case FormProviders.PacItForm:
                    outpostData = new List<string>
                    {
                        "!SCCoPIFO!",
                        "#Subject: " + packetMessage.Subject,    //6DM-175P_R_EOC213RR_
                        "#T: form-scco-eoc-213rr.html",
                        "#V: 1.2"
                    };
                    break;
            }
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

    }
}
