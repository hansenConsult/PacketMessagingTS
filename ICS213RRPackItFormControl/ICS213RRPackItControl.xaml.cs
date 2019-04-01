using FormControlBaseClass;
using SharedCode;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using static SharedCode.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213RRPackItFormControl
{
    [FormControl(
    FormControlName = "XSC_EOC-213RR",
    FormControlMenuName = "EOC Resource Request",
    FormControlType = FormControlAttribute.FormType.TestForm)
]

    public sealed partial class ICS213RRPackItControl : FormControlBase
    {
        public string[] ICSPosition = new string[] {
                "Incident Commander",
                "Operations",
                "Planning",
                "Logistics",
                "Finance",
                "Public Info. Officer",
                "Liaison Officer",
                "Safety Officer"
        };

        List<string> _ICSPositionFiltered = new List<string>();

        public ICS213RRPackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ReceivedOrSent = "sent";
            HowReceivedSent = "packet";
        }

        public override FormProviders DefaultFormProvider => FormProviders.PacItForm;

        public override string PacFormName => "XSC_EOC-213RR_v1706";

        public override string PacFormType => "XSC_EOC_213RR";

        public override string CreateSubject()
        {
            return $"{MessageNo}_{Severity?.ToUpper()[0]}/{HandlingOrder?.ToUpper()[0]}_EOC213RR_{incidentName.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#Subject: " + packetMessage.Subject,    //6DM-175P_R_EOC213RR_
                "#T: form-scco-eoc-213rr.html",
                "#V: 1.2"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        private void TextBoxFromICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem as string;
        }

        private void TextBoxFromICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = null;
                _ICSPositionFiltered = new List<string>();
                foreach (string s in ICSPosition)
                {
                    string lowerS = s.ToLower();
                    if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
                    {
                        _ICSPositionFiltered.Add(s);
                    }
                }
                sender.ItemsSource = _ICSPositionFiltered;
            }
        }

    }
}
