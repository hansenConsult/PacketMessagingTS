using System.Collections.Generic;

using FormControlBaseClass;

//using Microsoft.Toolkit.Uwp.Helpers;

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
        //List<ComboBoxPackItItem> ResourceInfoPriority = new List<ComboBoxPackItItem>
        //{
        //        new ComboBoxPackItItem("Now"),
        //        new ComboBoxPackItItem("High(0-4 hrs.)", "High"),
        //        new ComboBoxPackItItem("Medium(5-12 hrs.)", "Medium"),
        //        new ComboBoxPackItItem("Low(12+ hrs.)", "Low"),
        //};

        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
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

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

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

        //public override async void PrintForm()
        //{
        //    DirectPrintContainer.Children.Remove(PrintableContent);

        //    _printHelper = new PrintHelper(Container);
        //    _printHelper.AddFrameworkElementToPrint(PrintableContent);

        //    _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
        //    _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
        //    _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

        //    // Create a new PrintHelperOptions instance

        //    await _printHelper.ShowPrintUIAsync("ICS-213 Message");
        //}

        //protected override void ReleasePrintHelper()
        //{
        //    _printHelper.Dispose();

        //    if (!DirectPrintContainer.Children.Contains(PrintableContent))
        //    {
        //        DirectPrintContainer.Children.Add(PrintableContent);
        //    }
        //}

    }
}
