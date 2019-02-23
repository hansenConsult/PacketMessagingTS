using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using FormControlBaseClass;
using SharedCode;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MVCERTDA_FormsControl
{
    [FormControl(
        FormControlName = "MV_CERT_DA_Summary",
        FormControlMenuName = "MTV 213 CERT DA Summary",
        FormControlType = FormControlAttribute.FormType.CountyForm)
    ]

    public sealed partial class MVCERTDAControl : FormControlBase
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

        public string[] CERTLocation = new string[]
        {
            "Ada Park",
            "Appletree",
            "Cooper Neighborhood",
            "Cuesta Park",
            "Gemello",
            "Greater San Antonio",
            "Monta Loma",
            "Mtn View Gardens",
            "North Whisman",
            "Old Mtn View",
            "Rex Manor",
            "St Francis Acres",
            "Slater",
            "South Mountain View",
            "Sylvan park",
            "Varsity Park",
            "Wagon Wheel",
            "Mtn View Los Altos H. S. Dist",
            "Mtn View Whisman School Dist",
        };

        List<string> _ICSPositionFiltered = new List<string>();

        public MVCERTDAControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeControls();

            other.IsChecked = true;
            priority.IsChecked = true;
            ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxToICSPosition.Text = "Planning";
            autoSuggestBoxFromICSPosition.Text = "Planning";
            toLocation.Text = "Mountain View EOC";
            subject.Text = "Damage Summary for ";
        }

        public override string OperatorTime
        {
            get { return GetTextBoxString(operatorTime); }
            set
            {
                var filteredTime = value.Split(new char[] { ':' });
                if (filteredTime.Length == 2)
                {
                    operatorTime.Text = filteredTime[0] + filteredTime[1];
                }
                else
                {
                    operatorTime.Text = value;
                }
            }
        }

        public override string PacFormName => "MV_CERT_DA_Summary";	// Used in CreateFileName() 

        public override string PacFormType => "MVCERTSummary";

        public override string CreateSubject()
        {
            return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_MTV213-CERT_" + subject.Text + comments.Text);
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>()
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:MTV 213 CERT SUMMARY (which4) ",
                "# JS-ver. MV/PR-4.4-3.2, 09/19/18",
                "# FORMFILENAME: MTV_213_CERT_Summary.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        private void textBoxFromICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem as string;
        }

        private void textBoxFromICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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

        private void ComboBoxFromLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            subject.Text += e.AddedItems[0] as string;
        }
    }
}
