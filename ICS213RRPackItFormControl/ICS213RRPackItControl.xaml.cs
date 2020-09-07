using System.Collections.Generic;

using FormControlBaseClass;
using FormControlBasicsNamespace;

using FormUserControl;

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
    FormControlType = FormControlAttribute.FormType.CountyForm)
]

    public sealed partial class ICS213RRPackItControl : FormControlBase
    {
        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            FormHeaderControl.HeaderString1 = "SCCo EOC RESOURCE REQUEST FORM 213RR";
            FormHeaderControl.HeaderSubstring = "Version 8/17";
            FormHeaderControl.PIF = PIF;

            UpdateFormFieldsRequiredColors();
        }


        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

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

        public override void AppendDrillTraffic()
        {
            specialInstructions.Text += DrillTraffic;
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

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-scco-eoc-213rr.html",
                $"#V: {PackItFormVersion}-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
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
            bool found1 = false, found2 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "initiatedDate":
                            InitiatedDate = formField.ControlContent;
                            found1 = true;
                            break;
                        case "incidentName":
                            IncidentName = formField.ControlContent;
                            found2 = true;
                            break;
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
