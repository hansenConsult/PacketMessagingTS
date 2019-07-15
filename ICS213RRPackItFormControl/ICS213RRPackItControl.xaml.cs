using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213RRPackItFormControl
{
    [FormControl(
    FormControlName = "form-scco-eoc-213rr",
    FormControlMenuName = "EOC Resource Request",
    FormControlType = FormControlAttribute.FormType.TestForm)
]

    public sealed partial class ICS213RRPackItControl : FormControlBase
    {
        //public string[] ICSPosition = new string[] {
        //        "Incident Commander",
        //        "Operations",
        //        "Planning",
        //        "Logistics",
        //        "Finance",
        //        "Public Info. Officer",
        //        "Liaison Officer",
        //        "Safety Officer"
        //};

        //List<string> _ICSPositionFiltered = new List<string>();

        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ReceivedOrSent = "sender";
            HowReceivedSent = "packet";
        }

        //public override FormProviders DefaultFormProvider => FormProviders.PacItForm;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-scco-eoc-213rr";

        public override string PacFormType => "XSC_EOC_213RR";

        public override string CreateSubject()
        {
            return $"{MessageNo}_{HandlingOrder?.ToUpper()[0]}_EOC213RR_{incidentName.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-scco-eoc-213rr.html",
                "#V: 2.16-2.0",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        private void SuppReqFuel_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                suppReqFuelType.Tag = "36d.,required";
            }
            else
            {
                suppReqFuelType.Tag = "36d.";
            }
            base.TextBoxRequired_TextChanged(suppReqFuelType, null);
        }

        protected override void TextBoxRequired_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Name == "suppReqFuelType")
            {
                if (string.IsNullOrEmpty(suppReqFuelType.Text))
                {
                    //suppReqFuel.IsChecked = false;
                }
                else
                {
                    suppReqFuel.IsChecked = true;
                    return;
                }
            }
            base.TextBoxRequired_TextChanged(sender, e);
        }
    }
}
