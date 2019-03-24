using System.Collections.Generic;
using Windows.UI.Xaml;
using FormControlBaseClass;
using Windows.UI.Xaml.Controls;

using SharedCode;
using System;
using SharedCode.Helpers;
using static SharedCode.Helpers.FormProvidersHelper;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213_070628_FormControl
{
	[FormControl(
		FormControlName = "XSC_ICS-213_Message_v070628",
		FormControlMenuName = "ICS-213 Message",
		FormControlType = FormControlAttribute.FormType.CountyForm)
	]

	public partial class ICS213Control : FormControlBase
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

        public ICS213Control()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;
        }

        private string _subject;
        public override string Subject
        {
            get => _subject;
            set => Set(ref _subject, value);
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

        public override string OperatorTime
        {
            get => operatorTime.Text;
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

        public override FormProviders DefaultFormProvider => FormProviders.PacForm;

        private FormProviders formProvider;
        public override FormProviders FormProvider
        {
            get => formProvider;
            set => formProvider = value;
        }

        public override string PacFormName => "XSC_ICS-213_Message_v070628";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override string CreateSubject()
		{
			return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_ICS213_" + subject.Text);
		}

        protected override string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            //formField.ControlContent = comboBoxDataSet[0];

            return comboBoxDataSet[0];
        }

        protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        {
            string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length == 2)
            {
                if (data[1] == (-1).ToString())
                {
                    return $"{id}: [ }}0]";
                }
                else
                {
                    if (formField.ControlName == "comboBoxToICSPosition" || formField.ControlName == "comboBoxFromICSPosition")
                    {
                        int index = Convert.ToInt32(data[1]);
                        return $"{id}: [{data[0]}}}{(index + 1).ToString()}]";
                    }
                    else
                    {
                        return $"{id}: [{data[0]}}}{data[1]}]";
                    }
                }
            }
            else if (data[0] == "-1")
            {
                return $"{id}: [ }}0]";
            }
            else
            {
                return "";
            }
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>()
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:EOC MESSAGE FORM (which4) ",
                "# JS-ver. PR-4.4-4.3, 05/02/18",
                "# FORMFILENAME: XSC_ICS-213_Message_v070628.html"
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

        //private void ICSPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as ComboBox).Name == "comboBoxToICSPosition")
        //    {
        //        textBoxToICSPosition.Text = comboBoxToICSPosition.Text;
        //    }
        //    else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
        //    {
        //        textBoxFromICSPosition.Text = comboBoxFromICSPosition.Text;
        //    }
        //    ComboBoxRequired_SelectionChanged(sender, e);
        //}

    }
}
