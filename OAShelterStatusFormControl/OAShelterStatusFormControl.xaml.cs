using System;
using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAShelterStatusFormControl
{
    [FormControl(
        FormControlName = "form-oa-shelter-status",
        FormControlMenuName = "OA Shelter Status",
        FormControlType = FormControlAttribute.FormType.TestForm)
    ]

    public sealed partial class OAShelterStatusControl : FormControlBase
    {
        List<ComboBoxPackItItem> Municipalities = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem("Campbell", "Campbell"),
                new ComboBoxPackItItem("Cupertino", "Cupertino"),
                new ComboBoxPackItItem("Gilroy", "Gilroy"),
                new ComboBoxPackItItem("Los Altos", "Los Altos"),
                new ComboBoxPackItItem("Los Altos Hills", "Los Altos Hills"),
                new ComboBoxPackItItem("Los Gatos", "Los Gatos"),
                new ComboBoxPackItItem("Milpitas", "Milpitas"),
                new ComboBoxPackItItem("Monte Sereno", "Monte Sereno"),
                new ComboBoxPackItItem("Morgan Hill", "Morgan Hill"),
                new ComboBoxPackItItem("Mountain View", "Mountain View"),
                new ComboBoxPackItItem("Palo Alto", "Palo Alto"),
                new ComboBoxPackItItem("San Jose", "San Jose"),
                new ComboBoxPackItItem("Santa Clara", "Santa Clara"),
                new ComboBoxPackItItem("Saratoga", "Saratoga"),
                new ComboBoxPackItItem("Sunnyvale", "Sunnyvale"),
                new ComboBoxPackItItem("Unincorporated Areas", "Unincorporated")
        };

        List<ComboBoxPackItItem> ShelterTypes = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Type 1", "Type 1"),
                new ComboBoxPackItItem("Type 2", "Type 2"),
                new ComboBoxPackItItem("Type 3", "Type 3"),
                new ComboBoxPackItItem("Type 4", "Type 4"),
        };

        List<ComboBoxPackItItem> ShelterStatuses = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Open", "Open"),
                new ComboBoxPackItItem("Closed", "Closed"),
                new ComboBoxPackItItem("Full", "Full"),
        };

        List<ComboBoxPackItItem> YesNoContent = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Yes", "checked"),
                new ComboBoxPackItItem("No", "false"),
        };

        List<ComboBoxPackItItem> Managers = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem("American Red Cross", "American Red Cross"),
                new ComboBoxPackItItem("Private", "Private"),
                new ComboBoxPackItItem("Community", "Community"),
                new ComboBoxPackItItem("Government", "Government"),
                new ComboBoxPackItItem("Other", "Other"),
        };

        public OAShelterStatusControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

//            InitializeControls();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormName => "form-oa-shelter-status";

        public override string PacFormType => "OA Shelter Status";

        public string ReportType
        { get; set; }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-oa-shelter-status.html",
                "#V: 2.17-2.1",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override string CreateSubject()
        {
            return $"{MessageNo}_'{HandlingOrder?.ToUpper()[0]}_OAShelterStat_{ShelterName}";
        }

        private void capacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int occupancyInt = string.IsNullOrEmpty(occupancy.Text) ? 0 : Convert.ToInt32(occupancy.Text);
            int capacityInt = string.IsNullOrEmpty(capacity.Text) ? 0 : Convert.ToInt32(capacity.Text);
            availablity.Text = (capacityInt - occupancyInt).ToString();
        }

        protected override void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
                shelterType.Tag = (shelterType.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterStatus.Tag = (shelterStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterAddress.Tag = (shelterAddress.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterCity.Tag = (shelterCity.Tag as string).Replace(",required", ",conditionallyrequired");
                capacity.Tag = (capacity.Tag as string).Replace(",required", ",conditionallyrequired");
                occupancy.Tag = (occupancy.Tag as string).Replace(",required", ",conditionallyrequired");
                managedBy.Tag = (managedBy.Tag as string).Replace(",required", ",conditionallyrequired");
                primaryContact.Tag = (primaryContact.Tag as string).Replace(",required", ",conditionallyrequired");
                primaryContactPhone.Tag = (primaryContactPhone.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            else
            {
                shelterType.Tag = (shelterType.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterStatus.Tag = (shelterStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterAddress.Tag = (shelterAddress.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterCity.Tag = (shelterCity.Tag as string).Replace(",conditionallyrequired", ",required");
                capacity.Tag = (capacity.Tag as string).Replace(",conditionallyrequired", ",required");
                occupancy.Tag = (occupancy.Tag as string).Replace(",conditionallyrequired", ",required");
                managedBy.Tag = (managedBy.Tag as string).Replace(",conditionallyrequired", ",required");
                primaryContact.Tag = (primaryContact.Tag as string).Replace(",conditionallyrequired", ",required");
                primaryContactPhone.Tag = (primaryContactPhone.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            UpdateFormFieldsRequiredColors();
        }

        //protected override void ReportType_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    bool required = (bool)(sender as RadioButton).IsChecked && (sender as RadioButton).Name == "complete";
        //    UpdateRequiredFields(required);
        //    ValidateForm();
        //}

    }
}
