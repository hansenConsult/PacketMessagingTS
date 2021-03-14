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

namespace HavBedReportFormControl
{
    [FormControl(
        FormControlName = "form-mhoc-beds-status",
        FormControlMenuName = "XSC HAvBed Report",
        FormControlType = FormControlAttribute.FormType.HospitalForm,
        FormControlMenuIndex = 0)
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HavBedReportControl : FormControlBase
    {
        private const string Key = "HAvBedNumberTextBox";
        readonly List<ComboBoxItem> CommandCenterStatus = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = null, Tag = "" },
            new ComboBoxItem() { Content = "Available", Background = LightGreenBrush },
            new ComboBoxItem() { Content = "Drill or Exercise", Background = LightGrayBrush },
            new ComboBoxItem() { Content = "Full Activation", Background = PinkBrush },
            new ComboBoxItem() { Content = "Monitoring", Background = OrangeBrush },
            new ComboBoxItem() { Content = "Not Activated", Background = LightGreenBrush },
            new ComboBoxItem() { Content = "Unvailable", Background = PinkBrush },
            new ComboBoxItem() { Content = "Limited Activation", Background = PinkBrush },
        };

        readonly List<ComboBoxItem> Decon = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = null, Tag = "" },
            new ComboBoxItem() { Content = "Exceeded", Background = BlackBrush, Foreground = WhiteBrush },
            new ComboBoxItem() { Content = "Full", Background = PinkBrush },
            new ComboBoxItem() { Content = "Inactive", Background = YellowBrush },
            new ComboBoxItem() { Content = "Open", Background = LightGreenBrush },
        };


        public HavBedReportControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.HeaderString1 = "SCCo Medical Health Branch - HAvBed Report";
            FormHeaderControl.HeaderSubstring = "EMResource: c190320";

            UpdateFormFieldsRequiredColors();
        }

        public HavBedReportControl(MessageState messageState)
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.HeaderString1 = "SCCo Medical Health Branch - HAvBed Report";
            FormHeaderControl.HeaderSubstring = "EMResource: c190320";

            if (string.IsNullOrEmpty(FormControlName) || FormControlType == FormControlAttribute.FormType.Undefined)
            {
                GetFormDataFromAttribute(GetType());
            }

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        //public override string GetPacFormName() => "form-mhoc-beds-status";

        public override string PacFormType => "HAVBEDSTATUS";

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_HAvBed_{(hospitalName.SelectedValue as ComboBoxItem)?.Content}";
        }

        //public override string CreateOutpostData(ref PacketMessage packetMessage)
        //{
        //    _outpostData = new List<string>
        //    {
        //        "!SCCoPIFO!",
        //        "#T: form-mhoc-beds-status.html",
        //        $"#V: {PackItFormVersion}-{PIF}",
        //    };
        //    CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

        //    return CreateOutpostMessageBody(_outpostData);
        //}

        public override void FormatTextBoxes()
        {
            if (FormPacketMessage is null)
                return;

            if (FormPacketMessage.MessageState == MessageState.Locked)
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    (string id, FrameworkElement control) = GetTagIndex(formField);
                    if (control is TextBox textBox)
                    {
                        if (string.Compare(id, "50.") <= 0 && string.Compare(id, "41.") >= 0)
                        {
                            textBox.TextAlignment = TextAlignment.Left;
                        }
                    }
                }
            }
        }
    }
}
