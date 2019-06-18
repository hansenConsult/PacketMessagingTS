using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
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
			InitializeComponent();

			ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ReceivedOrSent = "sent";
            HowReceivedSent = "packet";
        }

        //public override string Severity
        //{
        //	get { return severity.GetRadioButtonCheckedState(); }
        //	set { severity.SetRadioButtonCheckedState(value); }
        //}

        //public override string HandlingOrder
        //{
        //	get { return handlingOrder.GetRadioButtonCheckedState(); }
        //	set { handlingOrder.SetRadioButtonCheckedState(value); }
        //}

        //private string _incidentName;
        //public override string IncidentName
        //{
        //    get => _incidentName;
        //    set => Set(ref _incidentName, value);
        //}
 
        public override FormProviders DefaultFormProvider => FormProviders.PacForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override string PacFormName => "XSC_EOC-213RR_v1706";

        public override string PacFormType => "XSC_EOC_213RR";

        public override string CreateSubject()
		{
			return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_EOC213RR_" + incidentName.Text);
		}

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:EOC Resource Request (which4)",
                "# JS-ver. PR-4.4-2.9, 06/29/18",
                "# FORMFILENAME: XSC_EOC-213RR_v1708.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
		}


    }
}
