using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FormControlBaseClass;
using SharedCode;
using SharedCode.Helpers;
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
        readonly List<ComboBoxPackItItem> Status = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem(null, ""),
            new ComboBoxPackItItem("Unknown", LightGrayBrush),
            new ComboBoxPackItItem("Open", LightGreenBrush),
            new ComboBoxPackItItem("Diverting Ambulances"),
            new ComboBoxPackItItem("Internal Disaster", _blackBrush),
            new ComboBoxPackItItem("Specialty Bypass", YellowBrush),
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
                cardiologyServiceComment.Text = "{CLEAR}";
                cardiologyServiceComment.IsReadOnly = true;
                //cardiologyServiceReopening.Tag = (cardiologyServiceReopening.Tag as string).Replace(",required", ",conditionallyrequired");
                cardiologyServiceReopening.IsReadOnly = true;
                cardiologyServiceReopening.Text = "{CLEAR}";
                //cardiologyServiceReopening.BorderThickness = new Thickness(1);
                //cardiologyServiceReopening.BorderBrush = new SolidColorBrush(Colors.Black);

            }
            else
            {
                cardiologyServiceComment.IsReadOnly = false;
                cardiologyServiceComment.Text = "";
                //cardiologyServiceReopening.Tag = (cardiologyServiceReopening.Tag as string).Replace(",conditionallyrequired", ",required");
                cardiologyServiceReopening.IsReadOnly = false;
                cardiologyServiceReopening.Text = "";
                //cardiologyServiceReopening.BorderThickness = new Thickness(2);
                //cardiologyServiceReopening.BorderBrush = new SolidColorBrush(Colors.Red);

            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
