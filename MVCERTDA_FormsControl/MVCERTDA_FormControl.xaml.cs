using System;
using System.Collections.Generic;
using System.Linq;

using FormControlBaseClass;
using FormControlBasicsNamespace;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MVCERTDA_FormsControl
{
    [FormControl(
        FormControlName = "MV_CERT_DA_Summary",
        FormControlMenuName = "MTV 213 CERT DA Summary",
        FormControlType = FormControlAttribute.FormType.CityForm)
    ]

    public sealed partial class MVCERTDAControl : FormControlBase
    {
        readonly string _subjectText = "Damage Summary for ";

        private List<TacticalCall> CERTLocationTacticalCalls { get => TacticalCallsigns.CreateMountainViewCERTList(); }    // Must be sorted by Agency Name

        public MVCERTDAControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            //TacticalCall currentTacticalCallsign = GetSelectedMTVTactical();
            //CERTLocation = currentTacticalCallsign;

            Severity = "other";
            HandlingOrder = "priority";
            actionNo.IsChecked = true;
            replyNo.IsChecked = true;
            forInfo.IsChecked = true;
            //autoSuggestBoxToICSPosition.Text = "Planning";
            comboBoxToICSPosition.SelectedItem = "Planning";
            comboBoxFromICSPosition.SelectedItem = "Planning";
            ToLocation = "Mountain View EOC";
            //subject.Text = subjectText;
            ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";

            UpdateFormFieldsRequiredColors();
        }

        private string tacticalCallsign;
        public string CERTLocationValue
        {
            get
            {
                if (tacticalCallsign is null)
                {
                    CERTLocationValue = GetSelectedMTVTactical()?.TacticalCallsign;
                }
                return tacticalCallsign;
            }
            set
            {
                string agencyName;
                if (FormPacketMessage == null)
                {
                    // New form
                    Set(ref tacticalCallsign, value);
                    agencyName = GetSelectedMTVTactical()?.AgencyName.Replace(" CERT", "");
                    
                }
                else
                {
                    // Display filled form
                    agencyName = GetAgencyNameFromPacketMessage();
                    tacticalCallsign = GetTacticalcallsignFromAgencyName(agencyName);
                }
                Subject = _subjectText + agencyName;
            }
        }

        public override string TacticalCallsign
        {
            get => CERTLocationValue;
            set => CERTLocationValue = value;
        }

        //private TacticalCall certLocation;
        //public TacticalCall CERTLocation
        //{
        //    get
        //    {
        //        if (certLocation is null)
        //        {
        //            CERTLocation = GetSelectedMTVTactical();
        //        }
        //        return certLocation;
        //    }
        //    set
        //    {
        //        Set(ref certLocation, value);
        //        subject.Text = subjectText + certLocation?.AgencyName;
        //    }
        //}

        private string toICSPosition;
        public string ToICSPosition
        {
            get => toICSPosition;
            set => Set(ref toICSPosition, value);
        }

        private string toLocation;
        public string ToLocation
        {
            get => toLocation;
            set => Set(ref toLocation, value);
        }

        private string fromLocation;
        public string FromLocation
        {
            get => fromLocation;
            set => Set(ref fromLocation, value);
        }

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CityForm;

        public override string PacFormName => "MV_CERT_DA_Summary";	// Used in CreateFileName() 

        public override string PacFormType => "MVCERTSummary";

        public override string MessageNo 
        { 
            get => base.MessageNo;
            set
            {
                base.MessageNo = value;
                OriginMsgNo = value;
            }
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override string CreateSubject()
        {
            //return (messageNo.Text + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_MTV213-CERT_" + subject.Text + comments.Text);
            return ("_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_MTV213-CERT_");
        }

        public override void AppendDrillTraffic()
        {
            message.Text += DrillTraffic;
        }

        protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        {
            string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length == 2)
            {
                if (data[1] == (-1).ToString() || string.IsNullOrEmpty(data[1]))
                {
                    return $"{id}: [ }}0]";
                }
                else
                {
                    if (formField.ControlName == "comboBoxToICSPosition" || formField.ControlName == "comboBoxFromICSPosition")
                    {
                        int index = Convert.ToInt32(data[1]);
                        return $"{id}: [{data[0]}}}{index + 1}]";
                    }
                    else
                    {
                        return $"{id}: [{data[0]}}}{data[1]}]";
                    }
                }
            }
            else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
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
            CreateDamageAssesmentMessage(ref packetMessage);

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

        //protected override void FillComboBoxFromFormFields(FormField formField, ComboBox comboBox)
        //{
        //    var data = formField.ControlContent.Split(new char[] { ',' });
        //    int index = Convert.ToInt32(data[1]);
        //    if (index < 0 && comboBox.IsEditable)
        //    {
        //        if (comboBox.Name == "comboBoxFromLocation")
        //        {
        //            FromLocation = data[0];
        //        }
        //        else
        //            comboBox.Text = data[0];
        //        //comboBox.SelectedIndex = index;
        //        //bool result = comboBox.Focus(FocusState.Programmatic);
        //        //comboBox.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        //comboBox.SelectedItem = data[0];
        //        comboBox.SelectedIndex = index;
        //    }
        //}


        private void CreateDamageAssesmentMessage(ref PacketMessage packetMessage)
        {
            string[] damageAssesmentMessage = new string[17];
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (formField.ControlName == "fires" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[0] += $"  F{formField.ControlContent}";
                if (formField.ControlName == "gasLeak" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[1] += $"  G{formField.ControlContent}";
                if (formField.ControlName == "waterLeak" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[2] += $"  W{formField.ControlContent}";
                if (formField.ControlName == "electrical" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[3] += $"  E{formField.ControlContent}";
                if (formField.ControlName == "chemical" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[4] += $"  C{formField.ControlContent}";
                if (formField.ControlName == "light" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[5] += $"  L{formField.ControlContent}";
                if (formField.ControlName == "mod" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[6] += $"  Mod{formField.ControlContent}";
                if (formField.ControlName == "heavy" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[7] += $"  H{formField.ControlContent}";
                if (formField.ControlName == "peopleImmediate" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[8] += $"  I{formField.ControlContent}";
                if (formField.ControlName == "delayed" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[9] += $"  D{formField.ControlContent}";
                if (formField.ControlName == "trapped" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[10] += $"  T{formField.ControlContent}";
                if (formField.ControlName == "morgue" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[11] += $"  Mor{formField.ControlContent}";
                if (formField.ControlName == "access" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[12] += $"  A{formField.ControlContent}";
                if (formField.ControlName == "noAccess" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[13] += $"  N{formField.ControlContent}";
                if (formField.ControlName == "messageOther" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[14] += $"  O{formField.ControlContent}";
                if (formField.ControlName == "surveyed" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[15] += $"  Nei{formField.ControlContent}";
                if (formField.ControlName == "message" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                {
                    // Remove the space character that was inserted to remove the required border color in message
                    if (formField.ControlContent == " ")
                        formField.ControlContent = "";
                    damageAssesmentMessage[16] = $"\n{formField.ControlContent}";
                }
            }
            string message = "";
            for (int i = 0; i < damageAssesmentMessage.Length; i++)
            {
                message += damageAssesmentMessage[i];
            }

            // insert the updated message
            var messageField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "message").FirstOrDefault();
            messageField.ControlContent = message;
        }

        private TacticalCall GetSelectedMTVTactical()
        {
            // Find Mountain View Tactical Call signs
            TacticalCallsignData mtvTacticalCallsigns = null;
            foreach (TacticalCallsignData data in TacticalCallsigns.TacticalCallsignDataDictionary.Values)
            {
                if (data.AreaName == "Local Mountain View")
                {
                    mtvTacticalCallsigns = data;
                    break;
                }
            }
            int index = mtvTacticalCallsigns.TacticalCallsigns.TacticalCallsignsArraySelectedIndex;
            if (index < 0)
            {
                return null;
            }
            else
            {
                return mtvTacticalCallsigns.TacticalCallsigns.TacticalCallsignsArray[index];
            }
        }

        private string GetAgencyNameFromPacketMessage()
        {
            FormField agencyNameField = FormPacketMessage.FormFieldArray.Where(formField => formField.ControlName == "textBoxFromLocation").FirstOrDefault();
            return agencyNameField.ControlContent;
        }

        private string GetTacticalcallsignFromAgencyName(string agencyName)
        {
            //List<TacticalCall> CERTLocationTacticalCalls
            foreach (TacticalCall tacticalCall in CERTLocationTacticalCalls)
            {
                if (tacticalCall.AgencyName == agencyName)
                {
                    return tacticalCall.TacticalCallsign;
                }
            }
            return "";
        }

        //private void DamageAccessmentRequired_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(message.Text))
        //        message.Text = " ";
        //     //TextBoxRequired_TextChanged(message, null);
        //}

        private void ComboBoxFromLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
            {
                return;
            }

            subject.Text = _subjectText + e.AddedItems[0].ToString();

            if ((sender as ComboBox).Name == "comboBoxFromLocation")
            {
                if (comboBoxFromLocation.SelectedIndex < 0 && comboBoxFromLocation.IsEditable)
                {
                    textBoxFromLocation.Text = comboBoxFromLocation.Text;
                    //FromLocation = comboBoxFromLocation.Text;
                }
                else
                {
                    textBoxFromLocation.Text = comboBoxFromLocation.SelectedItem.ToString();
                }
            }
            ComboBox_SelectionChanged(sender, e);
        }

        private void ICSPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Name == "comboBoxToICSPosition")
            {
                if (comboBoxToICSPosition.SelectedIndex < 0 && comboBoxToICSPosition.IsEditable)
                {
                    textBoxToICSPosition.Text = comboBoxToICSPosition.Text;
                }
                else
                {
                    textBoxToICSPosition.Text = comboBoxToICSPosition.SelectedItem.ToString();
                }
            }
            else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
            {
                if (comboBoxFromICSPosition.SelectedIndex < 0 && comboBoxFromICSPosition.IsEditable)
                {
                    textBoxFromICSPosition.Text = comboBoxFromICSPosition.Text;
                }
                else
                {
                    textBoxFromICSPosition.Text = comboBoxFromICSPosition.SelectedItem.ToString();
                }
            }
            ComboBox_SelectionChanged(sender, e);
        }

        private void ComboBoxFromLocation_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            subject.Text = _subjectText + args.Text;
            textBoxFromLocation.Text = args.Text;

            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl as Control;

                if (control.Name == sender.Name)
                {
                    if (string.IsNullOrEmpty(args.Text) && IsFieldRequired(sender))
                    {
                        sender.BorderBrush = formControl.RequiredBorderBrush;
                        sender.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        sender.BorderBrush = formControl.BaseBorderColor;
                        sender.BorderThickness = new Thickness(1);
                    }
                    break;
                }
            }
        }

    }
}

