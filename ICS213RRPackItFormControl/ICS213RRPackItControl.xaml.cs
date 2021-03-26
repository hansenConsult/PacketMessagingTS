
using System;
using System.Collections.Generic;
using System.Reflection;

using FormControlBaseClass;
using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

//[assembly: FormControl("form-scco-eoc-213rr", "EOC Resource Request", FormControlAttribute.FormType.CountyForm, 2)]
//    FormControlName = "form-scco-eoc-213rr",
//    FormControlMenuName = "EOC Resource Request",
//    FormControlType = FormControlAttribute.FormType.CountyForm,
//    FormControlMenuIndex = 2)
//]

//[assembly: FormControl("form-scco-eoc-213rr", "EOC Resource Request", FormControlAttribute.FormType.CountyForm, 2)]
namespace ICS213RRPackItFormControl
{
    [FormControl(
        FormControlName = "form-scco-eoc-213rr",
        FormControlMenuName = "EOC Resource Request",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]

    public sealed partial class ICS213RRPackItControl : FormControlBase
    {
        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.HeaderString1 = "SCCo EOC Resource Request Form 213RR";
            FormHeaderControl.HeaderSubstring = "Version 8/17";

            if (string.IsNullOrEmpty(FormControlName) || FormControlType == FormControlAttribute.FormType.Undefined)
            {
                //GetFormDataFromAttribute(typeof(ICS213RRPackItControl));
                GetFormDataFromAttribute(GetType());
            }

            UpdateFormFieldsRequiredColors();
        }

        //public ICS213RRPackItControl(MessageState messageState)
        //{
        //    Messagestate = messageState;

        //     InitializeComponent();

        //    ScanControls(PrintableArea);

        //    InitializeToggleButtonGroups();

        //    FormHeaderControl.HeaderString1 = "SCCo EOC Resource Request Form 213RR";
        //    FormHeaderControl.HeaderSubstring = "Version 8/17";

        //    UpdateFormFieldsRequiredColors();
        //}

        public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        //public override string GetPacFormName()
        //{
        //    return FormControlName;

        //    //    //bool isdef = Attribute.IsDefined(assembly, typeof(FormControlAttribute));
        //    //    //if (isdef)
        //    //    //{
        //    //    //    FormControlAttribute adAttr =
        //    //    //            (FormControlAttribute)Attribute.GetCustomAttribute(
        //    //    //            assembly, typeof(FormControlAttribute));
        //    //    //    if (adAttr != null)
        //    //    //    {
        //    //    //        fileName = adAttr.FormControlName;
        //    //    //    }
        //    //    //}
        //    //return "form-scco-eoc-213rr";
        //}

        public override string PacFormType => "XSC_EOC_213RR";

        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                initiatedDate.Text = value;
                SetProperty(ref _msgDate, value);
            }
        }

        //private string _initiatedDate;
        //public string InitiatedDate
        //{
        //    get => _initiatedDate;
        //    set => SetProperty(ref _initiatedDate, value);
        //}

        public override void AppendDrillTraffic()
        {
            specialInstructions.Text += DrillTraffic;
        }

        public override void SetPracticeField(string practiceField)
        {
            incidentName.Text = practiceField;
            UpdateFormFieldsRequiredColors();       // TODO check this
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string PIF => "2.3";

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_EOC213RR_{incidentName.Text}";
        }

        //public override string CreateOutpostData(ref PacketMessage packetMessage)
        //{
        //    _outpostData = new List<string>
        //    {
        //        "!SCCoPIFO!",
        //        $"#T: {GetPacFormName()}.html",
        //        $"#V: {PackItFormVersion}-{PIF}",
        //    };
        //    CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

        //    return CreateOutpostMessageBody(_outpostData);
        //}

        private void SuppReqFuel_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                fuelType.Tag = (fuelType.Tag as string).Replace("conditionallyrequired", "required");
            }
            else
            {
                fuelType.Tag = (fuelType.Tag as string).Replace("required", "conditionallyrequired");
            }
            TextBox textBox = FindName("fuelType") as TextBox;
            TextBox_TextChanged(textBox, null);
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
            TextBox textBox = FindName("specialInstructions") as TextBox;
            TextBox_TextChanged(textBox, null);
        }

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false, found2 = true;
        //    foreach (FormField formField in formFields)
        //    {
        //        FrameworkElement control = GetFrameworkElement(formField);

        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is TextBox textBox)
        //        {
        //            switch (control.Name)
        //            {
        //                case "initiatedDate":
        //                    InitiatedDate = formField.ControlContent;
        //                    found1 = true;
        //                    break;
        //                //case "incidentName":
        //                //    IncidentName = formField.ControlContent;
        //                //    found2 = true;
        //                //    break;
        //                case null:
        //                    continue;
        //            }
        //        }
        //        if (found1 && found2)
        //            break;
        //    }
        //    base.FillFormFromFormFields(formFields);
        //}

        //private void resourceInfoPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e is null)
        //        return;

        //    if (e.AddedItems.Count == 0)
        //        return;

        //    foreach (FormControl formControl in _formControlsList)
        //    {
        //        if (sender is ComboBox comboBox && comboBox.Name == formControl.InputControl.Name)
        //        {
        //            if (e.AddedItems[0] is ComboBoxPackItItem comboBoxPackItItem)
        //            {
        //                comboBox.Background = comboBoxPackItItem.BackgroundBrush;
        //                comboBox.Foreground = comboBoxPackItItem.ForegroundBrush;

        //                if (IsFieldRequired(comboBox) && comboBox.SelectedIndex < 0)
        //                {
        //                    comboBox.BorderBrush = formControl.RequiredBorderBrush;
        //                    comboBox.BorderThickness = new Thickness(2);
        //                }
        //                else
        //                {
        //                    comboBox.BorderBrush = formControl.BaseBorderColor;
        //                    comboBox.BorderThickness = new Thickness(1);
        //                }
        //            }
        //        }
        //    }
        //}
       
        public override void MsgTimeChanged(string msgTime)
        {
            if (string.IsNullOrEmpty(initiatedTime.Text))
            {
                initiatedTime.Text = msgTime;
            }
        }

    }
}
