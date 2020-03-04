using System;
using System.Collections.Generic;
using System.Text;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

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

            InitializeToggleButtonGroups();
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


        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.None;

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override string PacFormName => "SimpleMessage";

        public override string PacFormType => "SimpleMessage";

        public override void AppendDrillTraffic()
        {
            messageBody.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

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

		//public override string CreateSubject() => MessageNo + "_R_";
		public override string CreateSubject() => null;
	}
}
