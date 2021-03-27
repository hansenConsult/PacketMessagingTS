using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MutualAidRequestFormControl
{
    [FormControl(
        FormControlName = "form-oa-mutual-aid-request-v2",
        FormControlMenuName = "RACES Mutual Aid Request",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]


    public sealed partial class MutualAidRequestControl : FormControlBase
    {
        public MutualAidRequestControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.HeaderString1 = "Santa Clara County RACES -- Mutual Aid Request";
            FormHeaderControl.HeaderSubstring = "Version: 190614";

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBasics RootPanel => rootPanel;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "RACES-MAR";

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        // PEH-1316P_R_RACES-MAR_Agency
        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_RACES-MAR_{agencyName.Text}";
        }

        public override string GetPacFormName() => "form-oa-mutual-aid-request-v2";

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                //SignedDate = value;
                SetProperty(ref _msgDate, value);
                signedDate.Text = value;
                UpdateFormFieldsRequiredColors();
            }
        }

        //private string _SignedDate;
        //public string SignedDate
        //{
        //    get => _SignedDate;
        //    set => SetProperty(ref _SignedDate, value);
        //}

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false;
        //    foreach (FormField formField in formFields)
        //    {
        //        FrameworkElement control = GetFrameworkElement(formField);

        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is TextBox textBox)
        //        {
        //            switch (control.Name)
        //            {
        //                case "signedDate":
        //                    SignedDate = formField.ControlContent;
        //                    found1 = true;
        //                    break;
        //                case null:
        //                    continue;
        //            }
        //        }
        //        if (found1)
        //            break;
        //    }
        //    base.FillFormFromFormFields(formFields);
        //}

    }
}
