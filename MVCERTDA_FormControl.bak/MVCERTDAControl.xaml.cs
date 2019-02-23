using System.Collections.Generic;
using Windows.UI.Xaml;
using FormControlBaseClass;
using Windows.UI.Xaml.Controls;

using SharedCode;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MVCERTDA_FormControl
{
	[FormControl(
		FormControlName = "MTVCERTDA",
		FormControlMenuName = "MTV CERT DA Summary",
		FormControlType = FormControlAttribute.FormType.CountyForm)
	]

	public partial class MTVCERTDAControl : FormControlBase
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
            "Soutj Mountain View",
            "Sylvan park",
            "Varsity Park",
            "Wagon Wheel",
            "Mtn View Los Altos H. S. Dist",
            "Mtn View Whisman School Dist",
        };

		List<string> _ICSPositionFiltered = new List<string>();

		public MTVCERTDAControl()
		{
			this.InitializeComponent();

			ScanControls(PrintableArea);

			InitializeControls();

			ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
			otherText.Text = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;
        }

        public override string MsgTime
        {
            get => msgTime.Text;
            set
            {
                var filteredTime = value.Split(new char[] { ':' });
                if (filteredTime.Length == 2)
                {
                    msgTime.Text = filteredTime[0] + filteredTime[1];
                }
                else
                {
                    msgTime.Text = value;
                }
            }
        }

  //      public override string Severity
  //      {
		//	get { return severity.GetRadioButtonCheckedState(); }
		//	set { severity.SetRadioButtonCheckedState(value); }
		//}

  //      public override string HandlingOrder
  //      {
		//	get { return handlingOrder.GetRadioButtonCheckedState(); }
		//	set { handlingOrder.SetRadioButtonCheckedState(value); }
		//}

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

        public override string PacFormName => "MTVCERTDA";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override string CreateSubject()
		{
			return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "MTV213-CERT_Damage Summary for " + subject.Text);
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
    }
}
