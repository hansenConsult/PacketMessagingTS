﻿using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213FormControl
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
			this.InitializeComponent();

			ScanControls(PrintableArea);

			InitializeControls();

			ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
			OtherText = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;
        }

        public override string ReceiverMsgNo
		{ get => receiverMsgNo.Text; set => receiverMsgNo.Text = value; }

		public override string MessageNo
		{ get => messageNo.Text; set => messageNo.Text = value; }

		public override string SenderMsgNo
        { get => senderMsgNo.Text; set => senderMsgNo.Text = value; }

        //public override string MsgDate
        //{ get { return GetTextBoxString(msgDate); } set { SetTextBoxString(msgDate, value); } }

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

        public override string Severity
        { get { return severity.GetRadioButtonCheckedState(); } set { severity.SetRadioButtonCheckedState(value); } }

        public override string HandlingOrder
        { get { return handlingOrder.GetRadioButtonCheckedState(); } set { handlingOrder.SetRadioButtonCheckedState(value); } }

        public string Action
		{ get { return action.GetRadioButtonCheckedState(); } set { action.SetRadioButtonCheckedState(value); } }

		public string Reply
		{ get { return reply.GetRadioButtonCheckedState(); } set { reply.SetRadioButtonCheckedState(value); } }

		public string ReplyBy
		{ get { return GetTextBoxString(replyBy); } set { SetTextBoxString(replyBy, value); } }

		public bool? ForInfo
		{ get { return GetCheckBoxCheckedState(forInfo); } set { SetCheckBoxCheckedState(forInfo, value); } }

		public string ToICSPosition
		{
            get { return GetAutoSuggestBoxString(autoSuggestBoxToICSPosition); }
            set { SetAutoSuggestBoxString(autoSuggestBoxToICSPosition, value); }
        }

		public string ToLocation
		{ get { return GetTextBoxString(toLocation); } set { SetTextBoxString(toLocation, value); } }

		public string ToName
		{ get { return GetTextBoxString(toName); } set { SetTextBoxString(toName, value); } }

		public string ToTelephone
		{
            get { return GetTextBoxString(toTelephone); }
            set { SetTextBoxString(toTelephone, value); }
        }

		//public string FromICSPositionComboBox
		//{ get { return GetComboBoxSelectedValuePath(comboBoxFromICSPosition); } set { SetComboBoxString(comboBoxFromICSPosition, value); } }
		public string FromICSPositionComboBox
		{ get { return GetAutoSuggestBoxString(autoSuggestBoxFromICSPosition); } set { SetAutoSuggestBoxString(autoSuggestBoxFromICSPosition, value); } }

		public string FromLocation
		{ get { return GetTextBoxString(fromLocation); } set { SetTextBoxString(fromLocation, value); } }

		public string FromName
		{ get { return GetTextBoxString(fromName); } set { SetTextBoxString(fromName, value); } }

		public string FromTelephone
		{ get { return GetTextBoxString(fromTelephone); } set { SetTextBoxString(fromTelephone, value); } }

		public string Subject
		{ get { return GetTextBoxString(subject); } set { SetTextBoxString(subject, value); } }

		public string Reference
		{ get { return GetTextBoxString(reference); } set { SetTextBoxString(reference, value); } }

		public string Message
		{ get { return (GetTextBoxString(message)); } set { SetTextBoxString(message, value); } }

		//public override string ReceivedOrSent
		//{ get { return receivedOrSent.GetRadioButtonCheckedState(); } set { receivedOrSent.SetRadioButtonCheckedState(value); } }

		//public override string HowReceivedSent
  //      { get { return howRecevedSent.GetRadioButtonCheckedState(); } set { howRecevedSent.SetRadioButtonCheckedState(value); } }

		public string OtherText
		{ get { return GetTextBoxString(otherText); } set { SetTextBoxString(otherText, value); } }

        //public override string OperatorCallsign
        //{ get { return GetTextBoxString(operatorCallsign); } set { SetTextBoxString(operatorCallsign, value); } }

        //public override string OperatorName
        //{ get { return GetTextBoxString(operatorName); } set { SetTextBoxString(operatorName, value); } }

        //public override string OperatorDate
        //{ get { return GetTextBoxString(operatorDate); } set { SetTextBoxString(operatorDate, value); } }

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

        public override string PacFormName => "XSC_ICS-213_Message_v070628";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override string CreateSubject()
		{
			return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_ICS213_" + Subject);
		}

		//protected override void CreateOutpostDataFromFormFields(ref PacketMessage packetMessage, ref List<string> outpostData)
		//{
  //          if (packetMessage.FormFieldArray is null)
  //          {
  //              // This may happen if called from view Outpost Data
  //              packetMessage.FormFieldArray = CreateFormFieldsInXML();
  //              FillFormFromFormFields(packetMessage.FormFieldArray);
  //          }

  //          foreach (FormField formField in packetMessage.FormFieldArray)
		//	{
		//		if (formField.ControlContent is null || formField.ControlContent.Length == 0)
		//			continue;

		//		switch (formField.ControlName)
		//		{
		//			//MsgNo: [6DM-150P]
		//			case "messageNo":
		//				outpostData.Add($"MsgNo: [{formField.ControlContent}]");
		//				break;
		//			//1a.: [02/02/2015]
		//			case "msgDate":
		//				outpostData.Add($"1a.: [{formField.ControlContent}]");
		//				break;
		//			//1b.: [1917]
		//			case "msgTime":
		//				outpostData.Add($"1b.: [{formField.ControlContent}]");
		//				break;
		//			//4.: [OTHER]
		//			case "severity":
		//				outpostData.Add($"4.: [{formField.ControlContent.ToUpper()}]");
		//				break;
		//			//5.: [ROUTINE]
		//			case "handlingOrder":
		//				outpostData.Add($"5.: [{formField.ControlContent.ToUpper()}]");
		//				break;
		//			// 6a.: [Yes]
		//			case "action":
		//				outpostData.Add($"6a: [{(formField.ControlContent == "actionYes" ? "Yes" : "No")}]");
		//				break;
		//			//6b.: [Yes]
		//			case "reply":
		//				outpostData.Add($"6b.: [{(formField.ControlContent == "replyYes" ? "Yes" : "No")}]");
		//				break;
		//			//6c.: [checked]
		//			case "forInfo":
		//				if (formField.ControlContent == "True")
		//				{
		//					outpostData.Add("6c.: [checked]");
		//				}
		//				break;
		//			case "replyBy":
		//				outpostData.Add($"6d.: [{formField.ControlContent}]");
		//				break;
		//			//8.: [Operations]
		//			case "autoSuggestBoxToICSPosition":
		//				outpostData.Add($"7.: [{formField.ControlContent}]");
		//				break;
		//			//9a.: [Jerry]
		//			case "toLocation":
		//				outpostData.Add($"9a.: [{formField.ControlContent}]");
		//				break;
		//			case "toName":
		//				outpostData.Add($"ToName.: [{formField.ControlContent}]");
		//				break;
		//			case "toTelephone":
		//				outpostData.Add($"ToTel.: [{formField.ControlContent}]");
		//				break;
		//			//8.: [Operations]
		//			case "autoSuggestBoxFromICSPosition":
		//				outpostData.Add($"8.: [{formField.ControlContent}]");
		//				break;
		//			//9b.: [Poul Hansen]
		//			case "fromLocation":
		//				outpostData.Add($"9b.: [{formField.ControlContent}]");
		//				break;
		//			case "fromName":
		//				outpostData.Add($"FmName.: [{formField.ControlContent}]");
		//				break;
		//			case "fromTelephone":
		//				outpostData.Add($"FmTel.: [{formField.ControlContent}]");
		//				break;
		//			//10.: [Check-in 02/02/2015 - KZ6DM - Poul - Mountain View]
		//			case "subject":
		//				outpostData.Add($"10.: [{formField.ControlContent}]");
		//				break;
		//			case "reference":
		//				outpostData.Add($"11.: [{formField.ControlContent}]");
		//				break;
		//			//12.: [\nweekly check in]
		//			case "message":
		//				outpostData.Add($"12.: [\\n{formField.ControlContent}]");
		//				break;
		//			//Rec-Sent: [Sent]
		//			case "receivedOrSent":
		//				outpostData.Add($"Rec-Sent: [{formField.ControlContent}]");
		//				break;
		//			//Method: [Other]
		//			case "howRecevedSent":
		//				string text = formField.ControlContent;
		//				if (text == "otherRecvdType")
		//					text = "Other";
		//				outpostData.Add($"Method: [{text}]");
		//				break;
		//			//Other: [Packet]
		//			case "otherText":
		//				outpostData.Add($"Other: [{formField.ControlContent}]");
		//				break;
		//			//OpCall: [KZ6DM]
		//			case "operatorCallsign":
		//				outpostData.Add($"OpCall: [{formField.ControlContent}]");
		//				break;
		//			//OpName: [Poul Hansen]
		//			case "operatorName":
		//				outpostData.Add($"OpName: [{formField.ControlContent}]");
		//				break;
		//			//OpDate: [02/02/2015]
		//			case "operatorDate":
		//				//formField.ControlContent = OperatorDate;
		//				outpostData.Add($"OpDate: [{formField.ControlContent}]");
		//				break;
		//			//OpTime: [1920]
		//			case "operatorTime":
		//				//formField.ControlContent = OperatorTime;
		//				outpostData.Add($"OpTime: [{formField.ControlContent}]");
		//				break;
		//		}
		//	}
		//	outpostData.Add("#EOF");

		//	//return outpostData;
		//}

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

		public override FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines)
		{
			FormField[] formFields = CreateEmptyFormFieldsArray();
			string radioButtonContent;
			foreach (FormField formField in formFields)
			{
				switch (formField.ControlName)
				{
					case "senderMsgNo":
						formField.ControlContent = GetOutpostValue("MsgNo", ref msgLines);
						break;
					case "messageNo":
						formField.ControlContent = msgNumber;
						break;
					case "msgDate":
						formField.ControlContent = GetOutpostValue("1a.", ref msgLines);
						break;
					case "msgTime":
						formField.ControlContent = GetOutpostValue("1b.", ref msgLines);
						break;
					case "severity":
						formField.ControlContent = GetOutpostValue("4.", ref msgLines).ToLower();
						break;
					case "handlingOrder":
						formField.ControlContent = GetOutpostValue("5.", ref msgLines).ToLower();
						break;
					case "action":
						radioButtonContent = GetOutpostValue("6a.", ref msgLines);
						if (radioButtonContent?.Length > 0)
						{
							formField.ControlContent = (radioButtonContent == "yes" ? "actionYes" : "actionNo");
						}
						break;
					case "reply":
						radioButtonContent = GetOutpostValue("6b.", ref msgLines);
						if (radioButtonContent?.Length > 0)
						{
							formField.ControlContent = (radioButtonContent == "yes" ? "replyYes" : "replyNo");
						}
						break;
					case "forInfo":
						formField.ControlContent = (GetOutpostValue("6c.", ref msgLines) == "checked" ? "True" : "False");
						break;
					case "replyBy":
						formField.ControlContent = GetOutpostValue("6d.", ref msgLines);
						break;
                    case "autoSuggestBoxToICSPosition":
                        formField.ControlContent = GetOutpostValue("7.", ref msgLines);
                        break;
                    case "toLocation":
						formField.ControlContent = GetOutpostValue("9a.", ref msgLines);
						break;
					case "toName":
						formField.ControlContent = GetOutpostValue("ToName", ref msgLines);
						break;
					case "toTelephone":
						formField.ControlContent = GetOutpostValue("ToTel", ref msgLines);
						break;
                    case "autoSuggestBoxFromICSPosition":
                        formField.ControlContent = GetOutpostValue("8.", ref msgLines);
                        break;
                    case "fromLocation":
						formField.ControlContent = GetOutpostValue("9b.", ref msgLines);
						break;
					case "fromName":
						formField.ControlContent = GetOutpostValue("FmName", ref msgLines);
						break;
					case "fromTelephone":
						formField.ControlContent = GetOutpostValue("FmTel", ref msgLines);
						break;
					case "subject":
						formField.ControlContent = GetOutpostValue("10.", ref msgLines);
						break;
					case "reference":
						formField.ControlContent = GetOutpostValue("11.", ref msgLines);
						break;
					case "message":
                        //formField.ControlContent = GetOutpostValue("12.", ref msgLines).Substring(2);
                        formField.ControlContent = GetOutpostValue("12.", ref msgLines);
                        break;
					case "receivedOrSent":
						formField.ControlContent = GetOutpostValue("Rec-Sent", ref msgLines).ToLower();
						break;
					case "howRecevedSent":
						radioButtonContent = GetOutpostValue("Method", ref msgLines);
						if (radioButtonContent?.Length > 0)
						{
							if (radioButtonContent == "EOC Radio")
							{
								formField.ControlContent = "eOCRadio";
							}
							else if (radioButtonContent == "Other")
							{
								formField.ControlContent = "other";
							}
							else
							{
								formField.ControlContent = radioButtonContent;
							}
						}
						break;
					case "otherText":
						formField.ControlContent = GetOutpostValue("Other", ref msgLines);
						break;
					case "operatorCallsign":
						formField.ControlContent = GetOutpostValue("OpCall", ref msgLines);
						break;
					case "operatorName":
						formField.ControlContent = GetOutpostValue("OpName", ref msgLines);
						break;
					case "operatorDate":
						formField.ControlContent = GetOutpostValue("OpDate", ref msgLines);
						break;
					case "operatorTime":
						formField.ControlContent = GetOutpostValue("OpTime", ref msgLines);
						break;
				}
			}
			return formFields;
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
