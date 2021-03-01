using System.Collections.Generic;

using FormControlBaseClass;
using FormControlBasicsNamespace;
using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAAlliedHealthStatus201802FormControl
{
    [FormControl(
                FormControlName = "form-allied-health-facility-status",
                FormControlMenuName = "Allied Health Facility status",
                FormControlType = FormControlAttribute.FormType.CountyForm,
        FormControlMenuIndex = 5)
    ]



    public sealed partial class OAAlliedHealthStatusControl : FormControlBase
    {
        public OAAlliedHealthStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            //panelName = radioOperatorControl.Panel;
            //ScanControls(panelName, radioOperatorControl);

            FormHeaderControl.HeaderString1 = "Allied Health Status Report Short Form";
            FormHeaderControl.HeaderString2 = "(DEOC-9)";
            FormHeaderControl.HeaderSubstring = "Version: February 2018";
            FormHeaderControl.PIF = "2.2";

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override string GetPacFormName() => "form-allied-health-facility-status";

        public override string PacFormType => "Allied_Health_Status";


        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                Set(ref _msgDate, value);
                facilityDate.Text = value;
            }
        }

        public override void AppendDrillTraffic()
        { }

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
            return $"{formHeaderControl.MessageNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_AHFacStat_{facilityName.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-allied-health-facility-status.html",
                $"#V: {PackItFormVersion}-{FormHeaderControl.PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
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

        private void TextBoxResource_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            //string pattern = @"\b[0-9]+\b";
            //bool match = Regex.IsMatch(textBox.Text, pattern);
            //if (!match)
            //{
            //    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            //}

            string tagId = textBox.Tag.ToString().Substring(0, 2);
            UpdateRequiredResourceFields(tagId);

            UpdateFormFieldsRequiredColors();
        }

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = false;
            foreach (FormField formField in formFields)
            {
                FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                FrameworkElement control = formControl?.InputControl;
                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "facilityName":        // Needed for Practice subject
                            FacilityName = formField.ControlContent;
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

            UpdateFormFieldsRequiredColors();
        }

        public override void MsgTimeChanged(string msgTime)
        {
            if (string.IsNullOrEmpty(facilityTime.Text))
            {
                facilityTime.Text = msgTime;
            }
        }

    }
}
