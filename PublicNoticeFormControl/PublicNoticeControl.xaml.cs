using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FormControlBaseClass;
using FormUserControl;
using PacketMessagingTS.Core.Helpers;
using SharedCode;
using SharedCode.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PublicNoticeFormControl
{
    [FormControl(
    FormControlName = "PublicNotice",
    FormControlMenuName = "SCCo Public Notice",
    FormControlType = FormControlAttribute.FormType.TestForm)
]

    public sealed partial class PublicNoticeControl : FormControlBase
    {
        public PublicNoticeControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            DependencyObject panelName = (radioOperatorControl as RadioOperatorUserControl).Panel;
            ScanControls(panelName, radioOperatorControl);

            UpdateFormFieldsRequiredColors();
        }

        public override FormProvidersHelper.FormProviders FormProvider => FormProviders.PacForm;

        public override string PacFormName => "PublicNotice";

        public override string PacFormType => "PublicNoticeForm";

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override void AppendDrillTraffic() => throw new NotImplementedException();

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>()
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:Public Notice (which4) ",
                "# JS-ver. PR-4.7-1.2, 02/27/20",
                "# FORMFILENAME: PublicNotice.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override string CreateSubject()
        {
            return (messageNo.Text + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0]);
        }

    }
}
