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
        FormControlType = FormControlAttribute.FormType.CountyForm)
    ]

    public sealed partial class OAShelterStatusControl : FormControlBase
    {
        List<ComboBoxPackItItem> Municipalities = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem("Campbell"),
                new ComboBoxPackItItem("Cupertino"),
                new ComboBoxPackItItem("Gilroy"),
                new ComboBoxPackItItem("Los Altos"),
                new ComboBoxPackItItem("Los Altos Hills"),
                new ComboBoxPackItItem("Los Gatos"),
                new ComboBoxPackItItem("Milpitas"),
                new ComboBoxPackItItem("Monte Sereno"),
                new ComboBoxPackItItem("Morgan Hill"),
                new ComboBoxPackItItem("Mountain View"),
                new ComboBoxPackItItem("Palo Alto"),
                new ComboBoxPackItItem("San Jose"),
                new ComboBoxPackItItem("Santa Clara"),
                new ComboBoxPackItItem("Saratoga"),
                new ComboBoxPackItItem("Sunnyvale"),
                new ComboBoxPackItItem("Unincorporated Areas", "Unincorporated")
        };

        List<ComboBoxPackItItem> ShelterTypes = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Type 1"),
                new ComboBoxPackItItem("Type 2"),
                new ComboBoxPackItItem("Type 3"),
                new ComboBoxPackItItem("Type 4"),
        };

        List<ComboBoxPackItItem> ShelterStatuses = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Open"),
                new ComboBoxPackItItem("Closed"),
                new ComboBoxPackItItem("Full"),
        };

        List<ComboBoxPackItItem> YesNoContent = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Yes", "checked"),
                new ComboBoxPackItItem("No", "false"),
        };

        List<ComboBoxPackItItem> Managers = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem("American Red Cross"),
                new ComboBoxPackItItem("Private"),
                new ComboBoxPackItItem("Community"),
                new ComboBoxPackItItem("Government"),
                new ComboBoxPackItItem("Other"),
        };

        public OAShelterStatusControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override string PacFormName => "form-oa-shelter-status";

        public override string PacFormType => "OAShelterStat";

        public override void AppendDrillTraffic()
        { }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-oa-shelter-status.html",
                $"#V: 2.17-{PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override string CreateSubject()
        {
            return $"{OriginMsgNo}_{HandlingOrder?.ToUpper()[0]}_OAShelterStat_{shelterName.Text}";
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

    }
}
