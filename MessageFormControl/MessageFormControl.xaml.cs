﻿using System.Collections.Generic;
using System.Text;

using SharedCode;
using FormControlBaseClass;
using System;
using SharedCode.Helpers;
using static SharedCode.Helpers.FormProvidersHelper;


namespace MessageFormControl
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	[FormControl(
		FormControlName = "SimpleMessage",
		FormControlMenuName = "Simple Message",
		FormControlType = FormControlAttribute.FormType.None)
	]

	public partial class MessageControl : FormControlBase
    {
        public MessageControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeControls();
		}


        private bool inBoxHeaderVisibility;
        public bool InBoxHeaderVisibility
        {
            get => inBoxHeaderVisibility;
            set
            {
                Set(ref inBoxHeaderVisibility, value);
                if (value)
                {
                    SentHeaderVisibility = false;
                    NewHeaderVisibility = false;
                }
            }
        }

        private bool sentHeaderVisibility;
        public bool SentHeaderVisibility
        {
            get => sentHeaderVisibility;
            set
            {
                Set(ref sentHeaderVisibility, value);
                if (value)
                {
                    InBoxHeaderVisibility = false;
                    NewHeaderVisibility = false;
                }
            }
        }

        private bool newHeaderVisibility;
        public bool NewHeaderVisibility
        {
            get => newHeaderVisibility;
            set
            {
                Set(ref newHeaderVisibility, value);
                if (value)
                {
                    InBoxHeaderVisibility = false;
                    SentHeaderVisibility = false;
                }
            }
        }

        //public DateTime MessageReceivedTime
        //{ get; set; }

        //public DateTime MessageSentTime
        //{ get; set; }

        //public override string MessageNo
        //{ get; set; }

        //public override string MsgDate
        //{ get; set; }

        //public override string MsgTime
        //{ get; set; }

        //public override string OperatorCallsign
        //{ get; set; }


        public override FormProviders DefaultFormProvider => FormProviders.PacForm;

        private FormProviders formProvider = FormProviders.PacForm;
        public override FormProviders FormProvider
        {
            get => formProvider;
            set => formProvider = value;
        }

        public override string PacFormName => "SimpleMessage";

        public override string PacFormType => "SimpleMessage";

        protected override void CreateOutpostDataFromFormFields(ref PacketMessage packetMessage, ref List<string> outpostData)
        {
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (formField.ControlContent is null || formField.ControlContent.Length == 0)
                    continue;

                switch (formField.ControlName)
                {
                    case "messageBody":
                        string filteredMsg = formField.ControlContent.Replace("\r\n", "\r");
                        outpostData.Add($"{filteredMsg}");
                        break;
                }
            }
            //return outpostData;
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>();

            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines, FormProviders formProvider)
        {
            StringBuilder sb = new StringBuilder();
            // Skip to start of message
            int i = 0;
            for (; i < msgLines.Length; i++)
            {
                if (msgLines[i].StartsWith("Subject:"))
                {
                    i++;
                    break;
                }
            }
            // Message
            for (; i < msgLines.Length; i++)
            {
				string convertedLine = ConvertLineTabsToSpaces(msgLines[i], 8);

				//sb.AppendLine(msgLines[i]);
				sb.AppendLine(convertedLine);
			}
            string messageBody = sb.ToString();

			FormField[] formFields = CreateEmptyFormFieldsArray();
            foreach (FormField formField in formFields)
            {
                switch (formField.ControlName)
                {
                    case "messageBody":
                        formField.ControlContent = messageBody;
                        break;
                }
            }
            return formFields;
        }

		//public override string CreateSubject() => MessageNo + "_O/R_";
		public override string CreateSubject() => null;
	}
}
