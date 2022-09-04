using System.Collections.Generic;

using FormControlBaseClass;
using FormUserControl;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using Windows.UI.Xaml.Controls;
using FormControlBaseMvvmNameSpace;
using OA_Allied_HealthStatusFormControl;
using PacketMessagingTS.Core.Helpers;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
using SharedCode;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAAlliedHealthStatus201802FormControl
{
    [FormControl(
                FormControlName = "form-allied-health-facility-status",
                FormControlMenuName = "Allied Health Facility status",
                FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]



    public sealed partial class OAAlliedHealthStatusControl : FormControlBase
    {
        readonly OAAlliedHealthStatusControlViewModel ViewModel = new OAAlliedHealthStatusControlViewModel();

        readonly new List<ComboBoxItem> ToICSPositionItems = new List<ComboBoxItem>
        {
            new ComboBoxItem() {Content = "EMS Unit"},
            new ComboBoxItem() {Content = "Public Health Unit"},
            new ComboBoxItem() {Content = "Medical Health Branch"},
            new ComboBoxItem() {Content = "Operations Section"},
        };


        public OAAlliedHealthStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            FormHeaderControl.ViewModel.HeaderString1 = "Allied Health Status Report Short Form";
            FormHeaderControl.ViewModel.HeaderString2 = "(DEOC-9)";
            FormHeaderControl.ViewModel.HeaderSubstring = "Version: February 2018";

            FormHeaderControl.SetToICSPosition(ToICSPositionItems);
            FormHeaderControl.SetToLocation(ToICSLocationItems);

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            FormHeaderControl.SetHandlingOrder(2);

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "Allied_Health_Status";

        public override void AppendDrillTraffic()
        { }

        public override void SetPracticeField(string practiceField)
        {
            facilityName.Text = practiceField;
            //UpdateFormFieldsRequiredColors();
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1, printPage2 };

        //public string FacilityStatus
        //{ get; set; }

        //public string NhicsChart
        //{ get; set; }

        public string ResReqForms
        { get; set; }

        public string RepFormStd
        { get; set; }

        public string IncidentActPlan
        { get; set; }

        public string ComsDirectory
        { get; set; }


        //public override string FacilityName
        //{
        //    get => GetProperty(ref _facilityName);
        //    set => SetProperty(ref _facilityName, value, true);
        //}

        //private string facilityType;
        //public string FacilityType
        //{
        //    get => GetProperty(ref facilityType);
        //    set => SetProperty(ref facilityType, value, true);
        //}

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.ViewModelBase.MessageNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_AHFacStat_{facilityName.Text}";
        }

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "facilityDate":
                            ViewModel.FacilityDate = formField.ControlContent;
                            found1 = true;
                            break;
                        case null:
                            continue;
                    }
                }
                if (found1)
                    break;
            }
            base.FillFormFromFormFields(formFields);
        }


        protected override void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
                facilityContactName.Tag = (facilityContactName.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityType.Tag = (facilityType.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityContactName.Tag = (facilityContactName.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityPhone.Tag = (facilityPhone.Tag as string).Replace(",required", ",conditionallyrequired");
                incidentName.Tag = (incidentName.Tag as string).Replace(",required", ",conditionallyrequired");
                incidentDate.Tag = (incidentDate.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityStatus.Tag = (facilityStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                resReqForms.Tag = (resReqForms.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityEOCContactNumber.Tag = (facilityEOCContactNumber.Tag as string).Replace(",required", ",conditionallyrequired");
                liasonToPublicHealth.Tag = (liasonToPublicHealth.Tag as string).Replace(",required", ",conditionallyrequired");
                altContact.Tag = (altContact.Tag as string).Replace(",required", ",conditionallyrequired");
                altContactNumber.Tag = (altContactNumber.Tag as string).Replace(",required", ",conditionallyrequired");
                altContactEmail.Tag = (altContactEmail.Tag as string).Replace(",required", ",conditionallyrequired");
                facilityStatusGreen.Foreground = new SolidColorBrush(Colors.Black);
                facilityStatusRed.Foreground = new SolidColorBrush(Colors.Black);
                facilityStatusBlack.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                facilityContactName.Tag = (facilityContactName.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityType.Tag = (facilityType.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityContactName.Tag = (facilityContactName.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityPhone.Tag = (facilityPhone.Tag as string).Replace(",conditionallyrequired", ",required");
                incidentName.Tag = (incidentName.Tag as string).Replace(",conditionallyrequired", ",required");
                incidentDate.Tag = (incidentDate.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityStatus.Tag = (facilityStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                resReqForms.Tag = (resReqForms.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityEOCContactNumber.Tag = (facilityEOCContactNumber.Tag as string).Replace(",conditionallyrequired", ",required");
                liasonToPublicHealth.Tag = (liasonToPublicHealth.Tag as string).Replace(",conditionallyrequired", ",required");
                altContact.Tag = (altContact.Tag as string).Replace(",conditionallyrequired", ",required");
                altContactNumber.Tag = (altContactNumber.Tag as string).Replace(",conditionallyrequired", ",required");
                altContactEmail.Tag = (altContactEmail.Tag as string).Replace(",conditionallyrequired", ",required");
                facilityStatusGreen.Foreground = new SolidColorBrush(Colors.Red);
                facilityStatusRed.Foreground = new SolidColorBrush(Colors.Red);
                facilityStatusBlack.Foreground = new SolidColorBrush(Colors.Red);

            }
            UpdateFormFieldsRequiredColors();
        }

        void UpdateRequiredResourceFields(string tagId)
        {
            List<Control> resourceRow = new List<Control>();
            bool rowEmpty = true;
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl as Control;
                if (control is TextBox textBox)
                {
                    string tag = control.Tag?.ToString();
                    if (!string.IsNullOrEmpty(tag) && tag.Length > 2)
                    {
                        string tagID = tag.Substring(0, 2);
                        if (tagID == tagId)
                        {
                            rowEmpty &= string.IsNullOrEmpty(textBox.Text);
                            
                            resourceRow.Add(control);
                        }
                    }
                }
                if (resourceRow.Count >= 5)
                {
                    break;
                }
            }
            foreach (Control control in resourceRow)
            {
                if (rowEmpty)
                {
                    control.Tag = (control.Tag as string).Replace(",required", ",conditionallyrequired");
                }
                else
                {
                    control.Tag = (control.Tag as string).Replace(",conditionallyrequired", ",required");
                }
            }
        }

        public override void PrintForm()
        {
            facilityStatusGrid.BorderThickness = new Thickness(1, 1, 1, 1);

            base.PrintForm();
        }

        protected override void ReleasePrintHelper()
        {
            facilityStatusGrid.BorderThickness = new Thickness(1, 1, 1, 0);

            base.ReleasePrintHelper();
        }

        private void TextBoxResource_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            string tagId = textBox.Tag.ToString().Substring(0, 2);
            UpdateRequiredResourceFields(tagId);

            UpdateFormFieldsRequiredColors();
        }

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false;
        //    foreach (FormField formField in formFields)
        //    {
        //        FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
        //        FrameworkElement control = formControl?.InputControl;
        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is TextBox textBox)
        //        {
        //            switch (control.Name)
        //            {
        //                case "facilityName":        // Needed for Practice subject
        //                    FacilityName = formField.ControlContent;
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

        //    UpdateFormFieldsRequiredColors();
        //}

        public override void MsgTimeChanged(string msgTime)
        {
            if (string.IsNullOrEmpty(facilityTime.Text))
            {
                facilityTime.Text = msgTime;
            }
        }

        protected override void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadioButtons radioButtons = sender as RadioButtons;
            if (radioButtons.Name != "facilityStatus")
            {
                base.RadioButtons_SelectionChanged(sender, e);
            }

            foreach (RadioButton radioButton in radioButtons.Items)
            {
                if (IsFieldRequired(radioButtons) && radioButtons.SelectedIndex == -1)
                {
                    radioButton.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    radioButton.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            if (IsFieldRequired(radioButtons) && radioButtons.SelectedIndex == -1)
            {
                facilityStatusGreen.Foreground = new SolidColorBrush(Colors.Red);
                facilityStatusRed.Foreground = new SolidColorBrush(Colors.Red);
                facilityStatusBlack.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                facilityStatusGreen.Foreground = new SolidColorBrush(Colors.Black);
                facilityStatusRed.Foreground = new SolidColorBrush(Colors.Black);
                facilityStatusBlack.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        //private void TextBox_DateChanged(object sender, TextChangedEventArgs e)
        //{

        //}
    }
}
