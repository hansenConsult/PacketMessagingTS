using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MedicalHealthBranchRRFormControl
{
    [FormControl(
        FormControlName = "form-medical-resource-request",
        FormControlMenuName = "XSC Medical Resource Request",
        FormControlType = FormControlAttribute.FormType.HospitalForm
        )
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MedicalHealthBranchRRControl : FormControlBase
    {
        public MedicalHealthBranchRRControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.NamePanel1Visibility = false;
            FormHeaderControl.HeaderString1 = "SCCo Medical Health Branch\rResource Request Form #9A";
            FormHeaderControl.HeaderSubstring = "Version: September 2009";
            FormHeaderControl.PIF = PIF;

            if (string.IsNullOrEmpty(FormControlName) || FormControlType == FormControlAttribute.FormType.Undefined)
            {
                GetFormDataFromAttribute(GetType());
            }

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        //public override string GetPacFormName() => "form-medical-resource-request";

        public override string PacFormType => "XSC_MedicalResourceRequest";

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                //RequestMsgDate = value;
                requestDate.Text = value;
                SetProperty(ref _msgDate, value);
                UpdateFormFieldsRequiredColors();
            }
        }

        //private string requestMsgDate;
        //public string RequestMsgDate
        //{
        //    get => requestMsgDate;
        //    set => SetProperty(ref requestMsgDate, value);
        //}

        public override string PIF => "3.1";

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        //public override Panel PrintPage1 => printPage1;
        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_MedResReq_{requestingFacility.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-medical-resource-request.html",
                $"#V: {PackItFormVersion}-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
        }

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                bool found1 = false, found2 = true;
                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "msgDate":
                            found1 = true;
                            MsgDate = formField.ControlContent;
                            break;
                        //case "requestDate":
                        //    found2 = true;
                        //    RequestMsgDate = formField.ControlContent;
                        //    break;
                        //case "subject":
                        //    Subject = textBox.Text;
                        //    break;
                        case null:
                            continue;
                    }
                }
                if (found1 && found2)
                    break;
            }
            base.FillFormFromFormFields(formFields);

            UpdateFormFieldsRequiredColors();
        }

        public override void MsgTimeChanged(string msgTime)
        {
            if (string.IsNullOrEmpty(requestTime.Text))
            {
                requestTime.Text = msgTime;
            }
        }

        //public void TextBox_MsgTimeChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (sender is TextBox textBox)
        //    {
        //        FormControl formControl = null;
        //        if (!string.IsNullOrEmpty(textBox.Name))
        //            formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);

        //        if (formControl == null || !CheckTimeFormat(formControl))
        //        {
        //            return;
        //        }

        //        if (string.IsNullOrEmpty(requestTime.Text))
        //        {
        //            requestTime.Text = msgTime.Text;
        //        }
        //    }
        //}

    }
}
