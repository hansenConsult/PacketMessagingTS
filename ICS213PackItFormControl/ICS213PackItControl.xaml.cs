using System;
using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213PackItFormControl
{
    [FormControl(
		FormControlName = "XSC_ICS-213_Message",
		FormControlMenuName = "ICS-213 Message",
		FormControlType = FormControlAttribute.FormType.TestForm)
	]

	public partial class ICS213PackItControl : FormControlBase
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

        public ICS213PackItControl()
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

        public override FormProviders DefaultFormProvider => FormProviders.PacItForm;

        public override string PacFormName => "XSC_ICS-213_Message_v070628";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override string CreateSubject()
		{
			return ($"{MessageNo}_{HandlingOrder?.ToUpper()[0]}_ICS213_{subject.Text}");
		}

        protected override string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            //formField.ControlContent = comboBoxDataSet[0];

            return comboBoxDataSet[0];
        }

        //protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        //{
        //    string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    switch (FormProvider)
        //    {
        //        case FormProviders.PacForm:
        //            if (data.Length == 2)
        //            {
        //                if (data[1] == (-1).ToString())
        //                {
        //                    return $"{id}: [ }}0]";
        //                }
        //                else
        //                {
        //                    if (formField.ControlName == "comboBoxToICSPosition" || formField.ControlName == "comboBoxFromICSPosition")
        //                    {
        //                        int index = Convert.ToInt32(data[1]);
        //                        return $"{id}: [{data[0]}}}{(index + 1).ToString()}]";
        //                    }
        //                    else
        //                    {
        //                        return $"{id}: [{data[0]}}}{data[1]}]";
        //                    }
        //                }
        //            }
        //            else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
        //            {
        //                return $"{id}: [ }}0]";
        //            }
        //            break;
        //        case FormProviders.PacItForm:
        //            break;
        //    }
        //    return "";
        //}

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>()
            {
                "!SCCoPIFO!",
                "#T: form-ics213.html",
                "#V: 2.12-1.3",
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
