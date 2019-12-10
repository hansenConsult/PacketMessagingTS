using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FormControlBaseClass;
using SharedCode;
using SharedCode.Helpers;
using ToggleButtonGroupControl;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
            new ComboBoxPackItItem("Internal Disaster", _blackBrush),
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
            new ComboBoxPackItItem("Acyivated", PinkBrush),
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

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-medical-facility-status-v2";

        public override string PacFormType => "XSC_MedicalCacilityStatus";

        public override string PIFString => "PIF: 3.1";

        public string CardiologyService
        { get; set; }


        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_MedFacStat_{hospitalName.SelectedItem}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-medical-facility-status-v2.html",
                $"#V: 2.18c-3.1",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
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

        private void SpecialtyServeice_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            var parent = radioButton.Parent;
            if ((radioButton.Tag as string).Contains("Y") && ReportType == "update")
            {
                cardiologyServiceComment.Text = "";
                cardiologyServiceComment.IsReadOnly = true;
                cardiologyServiceReopening.IsReadOnly = true;
                cardiologyServiceReopening.Text = "";

            }
            else
            {
                cardiologyServiceComment.IsReadOnly = false;
                cardiologyServiceComment.Text = "";
                cardiologyServiceReopening.IsReadOnly = false;
                cardiologyServiceReopening.Text = "";

            }

            //foreach (FormControl formControl in _formControlsList)
            //{
            //    if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup)
            //    {
            //        if (radioButton.GroupName == toggleButtonGroup.RadioButtonGroup[0].GroupName)
            //        {
            //            string tag = GetTagIndex(toggleButtonGroup);
            //        }
            //    }
            //}

            //if (IsFieldRequired(control) && string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()))
            //{
            //    toggleButtonGroup.ToggleButtonGroupBrush = formControl.RequiredBorderBrush;
            //}
            //else
            //{
            //    toggleButtonGroup.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
            //}
            UpdateFormFieldsRequiredColors();
        }

    }
}
