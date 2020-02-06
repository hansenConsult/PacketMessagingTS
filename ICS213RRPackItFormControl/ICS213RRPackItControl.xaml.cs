using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ToggleButtonGroupControl;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213RRPackItFormControl
{
    [FormControl(
    FormControlName = "form-scco-eoc-213rr",
    FormControlMenuName = "EOC Resource Request",
    FormControlType = FormControlAttribute.FormType.CountyForm)
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

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                InitiatedDate = value;
                Set(ref _msgDate, value);
            }
        }

        private string _initiatedDate;
        public string InitiatedDate
        {
            get => _initiatedDate;
            set => Set(ref _initiatedDate, value);
        }

        public override string MsgTime
        {
            get => _msgTime;
            set
            {
                string time = TimeCheck(value);
                Set(ref _msgTime, time);
                initiatedTime.Text = time;
            }
        }

        public override void AppendDrillTraffic()
        {
            specialInstructions.Text += DrillTraffic;
        }

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_EOC213RR_{incidentName.Text}";
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

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            foreach (FormField formField in formFields)
            {
                FormControl formControl;
                if (string.IsNullOrEmpty(formField.ControlName))
                {
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == formField.FormIndex);
                }
                else
                {
                    formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                }

                Control control = formControl?.InputControl;

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    textBox.Text = formField.ControlContent;
                    // Fields that use Binding requires special handling
                    switch (control.Name)
                    {
                        case "msgDate":
                            MsgDate = textBox.Text;
                            break;
                        case "msgTime":
                            MsgTime = textBox.Text;
                            break;
                        case "initiatedDate":
                            InitiatedDate = textBox.Text;
                            break;
                        case "incidentName":
                            IncidentName = textBox.Text;
                            break;
                        case "subject":
                            Subject = textBox.Text;
                            break;
                        case "operatorCallsign":
                            OperatorCallsign = textBox.Text;
                            break;
                        case "operatorName":
                            OperatorName = textBox.Text;
                            break;
                        case null:
                            continue;
                    }
                }
                else if (control is AutoSuggestBox autoSuggsetBox)
                {
                    autoSuggsetBox.Text = formField.ControlContent;
                }
                else if (control is ComboBox comboBox)
                {
                    FillComboBoxFromFormFields(formField, comboBox);
                }
                else if (control is ToggleButtonGroup toggleButtonGroup)
                {
                    toggleButtonGroup.SetRadioButtonCheckedState(formField.ControlContent);
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.IsChecked = formField.ControlContent == "True" ? true : false;
                }
            }
            UpdateFormFieldsRequiredColors();
        }

    }
}
