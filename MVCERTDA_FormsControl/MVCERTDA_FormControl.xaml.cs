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
using SharedCode.Helpers;
using SharedCode.Models;
using Windows.UI;


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

        List<string> _ICSPositionFiltered = new List<string>();
        string subjectText;

        public List<TacticalCall> CERTLocationTacticalCalls { get; }

        public MVCERTDAControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            CERTLocationTacticalCalls = TacticalCallsigns.CreateMountainViewCERTList();

            Severity = "other";
            //other.IsChecked = true;
            HandlingOrder = "priority";
            //priority.IsChecked = true;
            actionNo.IsChecked = true;
            replyNo.IsChecked = true;
            forInfo.IsChecked = true;
            //autoSuggestBoxToICSPosition.Text = "Planning";
            comboBoxToICSPosition.SelectedItem = "Planning";
            comboBoxFromICSPosition.SelectedItem = "Planning";
            ToLocation = "Mountain View EOC";
            subjectText = "Damage Summary for ";
            ReceivedOrSent = "sent";
            HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";
        }

        private string tacticalCallsign;
        public string CERTLocationValue
        {
            get => tacticalCallsign;
            set => Set(ref tacticalCallsign, value);
        }

        public override string TacticalCallsign
        {
            get => CERTLocationValue;
            set => CERTLocationValue = value;
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

        private string _toLocation;
        public string ToLocation
        {
            get => _toLocation;
            set => Set(ref _toLocation, value);
        }
        public override string PacFormName => "MV_CERT_DA_Summary";	// Used in CreateFileName() 

        public override string PacFormType => "MVCERTSummary";

        public override string CreateSubject()
        {
            return (MessageNo + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_MTV213-CERT_" + subject.Text + comments.Text);
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

        //public override void InitializeForm()
        //{
        //    toLocation.Text = "Mountain View EOC";
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

        //private void textBoxFromICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //    sender.Text = args.SelectedItem as string;
        //}

        //private void textBoxFromICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        //Set the ItemsSource to be your filtered dataset
        //        //sender.ItemsSource = null;
        //        _ICSPositionFiltered = new List<string>();
        //        foreach (string s in ICSPosition)
        //        {
        //            string lowerS = s.ToLower();
        //            if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
        //            {
        //                _ICSPositionFiltered.Add(s);
        //            }
        //        }
        //        sender.ItemsSource = _ICSPositionFiltered;
        //    }
        //}

        private void DamageAccessmentRequired_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(message.Text))
                message.Text = " ";
             //TextBoxRequired_TextChanged(message, null);
        }

        private void ComboBoxFromLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            subject.Text = subjectText + e.AddedItems[0].ToString();

            ComboBoxRequired_SelectionChanged(sender, e);
        }

        private void ICSPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Name == "comboBoxToICSPosition")
            {
                textBoxToICSPosition.Text = comboBoxToICSPosition.Text;
            }
            else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
            {
                textBoxFromICSPosition.Text = comboBoxFromICSPosition.Text;
            }
            ComboBoxRequired_SelectionChanged(sender, e);
        }
    }
}
