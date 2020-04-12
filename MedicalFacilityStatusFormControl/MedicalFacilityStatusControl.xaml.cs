﻿using System.Collections.Generic;

using FormControlBaseClass;
using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MedicalFacilityStatusFormControl
{
    [FormControl(
        FormControlName = "form-medical-facility-status-v2",
        FormControlMenuName = "XSC Medical Facility Status",
        FormControlType = FormControlAttribute.FormType.HospitalForm)
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MedicalFacilityStatusControl : FormControlBase
    {
        readonly List<ComboBoxPackItItem> DiversionStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown", LightGrayBrush),
            new ComboBoxPackItItem("Open", LightGreenBrush),
            new ComboBoxPackItItem("Diverting Ambulances"),
            new ComboBoxPackItItem("Internal Disaster", BlackBrush, WhiteBrush),
            new ComboBoxPackItItem("Specialty Bypass", YellowBrush),
        };

        readonly List<ComboBoxPackItItem> DeconStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Not Available", OrangeBrush),
            new ComboBoxPackItItem("Active", LightGreenBrush),
            new ComboBoxPackItItem("Available", YellowBrush),
        };

        readonly List<ComboBoxPackItItem> CommandCenterStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Inactive", LightGreenBrush),
            new ComboBoxPackItItem("Activated", PinkBrush),
        };

        readonly List<ComboBoxPackItItem> MorgueStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Open", LightGreenBrush),
            new ComboBoxPackItItem("Full", YellowBrush),
        };

        readonly List<ComboBoxPackItItem> PowerStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Normal", LightGreenBrush),
            new ComboBoxPackItItem("Generator", YellowBrush),
            new ComboBoxPackItItem("None"),
        };

        readonly List<ComboBoxPackItItem> BuildingStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown", LightGrayBrush),
            new ComboBoxPackItItem("Restricted Use"),
            new ComboBoxPackItItem("Unsafe to Occupy"),
            new ComboBoxPackItItem("Not Inspected", LightGrayBrush),
            new ComboBoxPackItItem("Safe to Occupy", YellowBrush),
            new ComboBoxPackItItem("Normal", LightGreenBrush),
            new ComboBoxPackItItem("Compromised"),
            new ComboBoxPackItItem("Evacuating", PinkBrush),
            new ComboBoxPackItItem("Closed"),
        };

        readonly List<ComboBoxPackItItem> SecurityStatus = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Normal", LightGreenBrush),
            new ComboBoxPackItItem("Ekevated", YellowBrush),
            new ComboBoxPackItItem("Restricted Access", OrangeBrush),
            new ComboBoxPackItItem("Lockdown", PinkBrush),
        };

        readonly List<ComboBoxPackItItem> Status = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown"),
            new ComboBoxPackItItem("Adequate", LightGreenBrush),
            new ComboBoxPackItItem("Insufficient", LightGrayBrush),
        };

        public MedicalFacilityStatusControl()
        {
            this.InitializeComponent();

            DependencyObject panelName = (formHeaderControl as FormHeaderUserControl).Panel;
            ScanControls(panelName, formHeaderControl);

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            panelName = (radioOperatorControl as RadioOperatorUserControl).Panel;
            ScanControls(panelName, radioOperatorControl);

            FormHeaderControl.HeaderString1 = "Medical facility Status";
            FormHeaderControl.HeaderSubstring = "WebEOC: 20160101";
            FormHeaderControl.PIF = "3.2";

            UpdateFormFieldsRequiredColors(false);
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.HospitalForm;

        public override string PacFormName => "form-medical-facility-status-v2";

        public override string PacFormType => "XSC_MedicalCacilityStatus";

        public string CardiologyService
        { get; set; }


        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1, printPage2 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_MedFacStat_{(hospitalName.SelectedItem as ComboBoxPackItItem)?.Item}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-medical-facility-status-v2.html",
                $"#V: {PackItFormVersion}-{FormHeaderControl.PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
        }

        protected override void UpdateRequiredFields(bool required)
        {
            if (required)
            {
                diversionStatus.Tag = (diversionStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                deconStatus.Tag = (deconStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                commandCenterStatus.Tag = (commandCenterStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                morgueStatus.Tag = (morgueStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                powerStatus.Tag = (powerStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                buildingStatus.Tag = (buildingStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                securityStatus.Tag = (securityStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                staffingStatus.Tag = (staffingStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                medicalFacilitySuppliesStatus.Tag = (medicalFacilitySuppliesStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                clinicalSuppliesStatus.Tag = (clinicalSuppliesStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                cardiologyService.Tag = (cardiologyService.Tag as string).Replace(",conditionallyrequired", ",required");
                dialysisService.Tag = (dialysisService.Tag as string).Replace(",conditionallyrequired", ",required");
                cardiologyService.Tag = (cardiologyService.Tag as string).Replace(",conditionallyrequired", ",required");
                emergencyDepartmentService.Tag = (emergencyDepartmentService.Tag as string).Replace(",conditionallyrequired", ",required");
                neurologyService.Tag = (neurologyService.Tag as string).Replace(",conditionallyrequired", ",required");
                obstetricsService.Tag = (obstetricsService.Tag as string).Replace(",conditionallyrequired", ",required");
                obstetricsLaborService.Tag = (obstetricsLaborService.Tag as string).Replace(",conditionallyrequired", ",required");
                ophthalmologyService.Tag = (ophthalmologyService.Tag as string).Replace(",conditionallyrequired", ",required");
                orthopedicsService.Tag = (orthopedicsService.Tag as string).Replace(",conditionallyrequired", ",required");
                pediatricsService.Tag = (pediatricsService.Tag as string).Replace(",conditionallyrequired", ",required");
                surgeryService.Tag = (surgeryService.Tag as string).Replace(",conditionallyrequired", ",required");
                evacuatingService.Tag = (evacuatingService.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            else
            {
                diversionStatus.Tag = (diversionStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                deconStatus.Tag = (deconStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                commandCenterStatus.Tag = (commandCenterStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                morgueStatus.Tag = (morgueStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                powerStatus.Tag = (powerStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                buildingStatus.Tag = (buildingStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                securityStatus.Tag = (securityStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                staffingStatus.Tag = (staffingStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                medicalFacilitySuppliesStatus.Tag = (medicalFacilitySuppliesStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                clinicalSuppliesStatus.Tag = (clinicalSuppliesStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                cardiologyService.Tag = (cardiologyService.Tag as string).Replace(",required", ",conditionallyrequired");
                dialysisService.Tag = (dialysisService.Tag as string).Replace(",required", ",conditionallyrequired");
                cardiologyService.Tag = (cardiologyService.Tag as string).Replace(",required", ",conditionallyrequired");
                emergencyDepartmentService.Tag = (emergencyDepartmentService.Tag as string).Replace(",required", ",conditionallyrequired");
                neurologyService.Tag = (neurologyService.Tag as string).Replace(",required", ",conditionallyrequired");
                obstetricsService.Tag = (obstetricsService.Tag as string).Replace(",required", ",conditionallyrequired");
                obstetricsLaborService.Tag = (obstetricsLaborService.Tag as string).Replace(",required", ",conditionallyrequired");
                ophthalmologyService.Tag = (ophthalmologyService.Tag as string).Replace(",required", ",conditionallyrequired");
                orthopedicsService.Tag = (orthopedicsService.Tag as string).Replace(",required", ",conditionallyrequired");
                pediatricsService.Tag = (pediatricsService.Tag as string).Replace(",required", ",conditionallyrequired");
                surgeryService.Tag = (surgeryService.Tag as string).Replace(",required", ",conditionallyrequired");
                evacuatingService.Tag = (evacuatingService.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            UpdateFormFieldsRequiredColors();
        }

        private void SpecialtyService_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            string tag = "";
            ToggleButtonGroup toggleButtonGroupYN = null;
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup)
                {
                    if (radioButton.GroupName == toggleButtonGroup.Name)
                    {
                        tag = GetTagIndex(toggleButtonGroup);
                        toggleButtonGroupYN = toggleButtonGroup;
                        toggleButtonGroupYN.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
                    }
                }
            }

            string commentsTag = tag.TrimEnd('.') + "c.";
            FormControl formControlComment = null;
            string dateTag = tag.TrimEnd('.') + "d.";
            FormControl formControlDate = null;
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.InputControl is TextBox textBox)
                {
                    if (textBox.Tag as string == commentsTag)
                    {
                        formControlComment = formControl;
                    }
                    if (textBox.Tag as string == dateTag)
                    {
                        formControlDate = formControl;
                    }
                }
            }
            TextBox textBoxComment = formControlComment?.InputControl as TextBox;
            TextBox textBoxDate = formControlDate?.InputControl as TextBox;

            if ((radioButton.Tag as string).Contains("Y"))
            {
                textBoxComment.Text = "";
                textBoxComment.IsReadOnly = true;
                textBoxDate.IsReadOnly = true;
                textBoxDate.Text = "";                
            }
            else if ((radioButton.Tag as string).Contains("N"))
            {
                textBoxComment.IsReadOnly = false;
                //textBoxComment.Text = "";
                textBoxDate.IsReadOnly = false;
                //textBoxDate.Text = "";
            }
        }

    }
}
