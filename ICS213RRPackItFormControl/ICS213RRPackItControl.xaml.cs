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
        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }


        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-scco-eoc-213rr";

        public override string PacFormType => "XSC_EOC_213RR";

        public override void AppendDrillTraffic()
        {
            specialInstructions.Text += DrillTraffic;
        }

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
                $"#V: 2.17-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        private void SuppReqFuel_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                suppReqFuelType.Tag = suppReqFuelType.Tag.ToString().Replace("conditionallyrequired", "required");
            }
            else
            {
                suppReqFuelType.Tag = suppReqFuelType.Tag.ToString().Replace("required", "conditionallyrequired");
            }
            base.TextBox_TextChanged(suppReqFuelType, null);
        }

        private void SuppReqOther_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                specialInstructions.Tag = specialInstructions.Tag.ToString().Replace("conditionallyrequired", "required");
            }
            else
            {
                specialInstructions.Tag = specialInstructions.Tag.ToString().Replace("required", "conditionallyrequired");
            }
            base.TextBox_TextChanged(specialInstructions, null);
        }

        protected override void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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
            base.TextBox_TextChanged(sender, e);
        }

    }
}
