using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;
using ToggleButtonGroupControl;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MedicalHealthBranchRRFormControl
{
    [FormControl(
        FormControlName = "form-medical-resource-request",
        FormControlMenuName = "XSC Medical Resource Request",
        FormControlType = FormControlAttribute.FormType.HospitalForm)
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MedicalHealthBranchRRControl : FormControlBase
    {
        public MedicalHealthBranchRRControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "form-medical-resource-request";

        public override string PacFormType => "XSC_MedicalResourceRequest";

        public override string PIFString => "PIF: 3.1";

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                RequestMsgDate = value;
                Set(ref _msgDate, value);
                
            }
        }

        private string requestMsgDate;
        public string RequestMsgDate
        {
            get => requestMsgDate;
            set => Set(ref requestMsgDate, value);
        }

        public override string MsgTime
        {
            get => _msgTime;
            set
            {
                string time = TimeCheck(value);
                Set(ref _msgTime, time);
                requestTime.Text = time;
            }
        }

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        //public override Panel PrintPage1 => printPage1;
        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_MedResReq_{requestingFacility.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-medical-resource-request.html",
                $"#V: 2.18c-{PIFString}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
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

                Control control = formControl?.InputControl as Control;

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
                        case "requestDate":
                            RequestMsgDate = textBox.Text;
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
