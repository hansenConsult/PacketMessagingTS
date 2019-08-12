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
        FormControlName = "XSC_OA_ShelterStatus_v20130814",
        FormControlMenuName = "OA Shelter Status",
        FormControlType = FormControlAttribute.FormType.CountyForm)
    ]

    public sealed partial class OAShelterStatusControl : FormControlBase
    {
        string[] Municipalities = new string[] {
                "Campbell",
                "Cupertino",
                "Gilroy",
                "Loma Prieta",
                "Los Altos",
                "Los Altos Hills",
                "Los Gatos/Monte Sereno",
                "Milpitas",
                "Morgan Hill",
                "Mountain View",
                "NASA-Ames",
                "Palo Alto",
                "San Jose",
                "Santa Clara",
                "Saratoga",
                "Stanford",
                "Sunnyvale",
                "Unincorporated"
        };

        string[] ShelterTypes = new string[]
        {
                "Type 1",
                "Type 2",
                "Type 3",
                "Type 4",
        };

        string[] ShelterStatuses = new string[]
        {
                "Closed",
                "Full",
                "Open",
        };

        string[] Managers = new string[]
        {
                "American Red Cross",
                "Private",
                "Community",
                "Government",
                "Other",
        };

        public OAShelterStatusControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            sent.IsChecked = true; ;
            packet.IsChecked = true;
        }

        //public override FormProviders DefaultFormProvider => FormProviders.PacForm;

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override string PacFormName => "XSC_OA_ShelterStatus_v20130814";

        public override string PacFormType => "OA Shelter Status";

        //public string IncidentName
        //{ get => incidentName.Text; }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:SC OA Shelter Status (which4) ",
                "# JS-ver. PR-4.4-3.6, 08-29-18",
                "# FORMFILENAME: XSC_OA_ShelterStatus_v20130814.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override string CreateSubject()
        {
            return (MessageNo + '_' + Severity?.ToUpper()[0] + '/' + HandlingOrder?.ToUpper()[0] + "_OAShelterStat_" + '_' + incidentName.Text);

        }

        private void capacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            int occupancyInt = string.IsNullOrEmpty(occupancy.Text) ? 0 : Convert.ToInt32(occupancy.Text);
            int capacityInt = string.IsNullOrEmpty(capacity.Text) ? 0 : Convert.ToInt32(capacity.Text);
            availablity.Text = (capacityInt - occupancyInt).ToString();
        }

    }
}
