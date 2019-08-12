using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAAlliedHealthStatus201802FormControl
{
    [FormControl(
                FormControlName = "form-allied-health-facility-status",
                FormControlMenuName = "Allied Health Facility status",
                FormControlType = FormControlAttribute.FormType.TestForm)
    ]



    public sealed partial class OAAlliedHealthStatusControl : FormControlBase
    {
        public OAAlliedHealthStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }


        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-allied-health-facility-status";

        public override string PacFormType => "Allied_Health_Status";

        public string ReportType
        { get; set; }

        public string FacilityStatus
        { get; set; }

        public string NhicsChart
        { get; set; }

        public string ResReqForms
        { get; set; }

        public string RepFormStd
        { get; set; }

        public string IncidentActPlan
        { get; set; }

        public string ComsDirectory
        { get; set; }

        //private string _facilityName;
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

        //6DM-783P_R_AHFacStat_dfg
        public override string CreateSubject()
        {
            return $"{MessageNo}_{HandlingOrder?.ToUpper()[0]}_AHFacStat_{facilityName.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-allied-health-facility-status.html",
                $"#V: 2.17-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
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

    }
}
