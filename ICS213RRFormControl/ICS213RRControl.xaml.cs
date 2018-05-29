using System.Collections.Generic;
using Windows.UI.Xaml;
using FormControlBaseClass;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace EOC213RRFormControl
{
	[FormControl(
        FormControlName = "XSC_EOC-213RR_v1706",
        FormControlMenuName = "EOC Resource Request",
		FormControlType = FormControlAttribute.FormType.CountyForm)
	]
	
	public partial class EOC213RRControl : FormControlBase
	{
		public EOC213RRControl()
		{
			this.InitializeComponent();

			ScanControls(PrintableArea);

			InitializeControls();

            ReceivedOrSent = "sent";
            HowReceivedSent = "packet";
        }

        public string IncidentName
		{ get => GetTextBoxString(incidentName); set => SetTextBoxString(incidentName, value); }

		public override string PacFormName => "XSC_EOC-213RR_v1706";

        public override string PacFormType => "XSC_EOC_213RR";

        public override string CreateSubject()
		{
			return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_EOC213RR_" + IncidentName);
		}

        public override string CreateOutpostData(ref PacketMessage packetMessage)
		{
            outpostData = new List<string>
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:EOC Resource Request (which4)",
                "# JS-ver. PR-4.3-2.8, 09/15/17",
                "# FORMFILENAME: XSC_EOC-213RR_v1706.html"
            };
            outpostData = CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

			return CreateOutpostMessageBody(outpostData);
		}

    }
}
