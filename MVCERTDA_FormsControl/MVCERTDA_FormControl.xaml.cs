using System;
using System.Collections.Generic;
using System.Linq;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;
using FormControlBaseMvvmNameSpace;
using MVCERTDA_FormControl;
using Windows.UI.Xaml;
using PacketMessagingTS.Core.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MVCERTDA_FormsControl
{
    [FormControl(
        FormControlName = "MTV_213_CERT_Summary-v210323",
        FormControlMenuName = "MTV 213 CERT DA Summary",
        FormControlType = FormControlAttribute.FormType.CityForm
        )
    ]

    public sealed partial class MVCERTDAControl : FormControlBase
    {
        //public MVCERTDAControlViewModel ViewModel = MVCERTDAControlViewModel.Instance;
        public MVCERTDAControlViewModel ViewModel = new MVCERTDAControlViewModel();

        readonly string _subjectText = "Damage Summary for ";

        //private List<TacticalCall> CERTLocationTacticalCalls { get => TacticalCallsigns.CreateMountainViewCERTList(); }    // Must be sorted by Agency Name

        public MVCERTDAControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            other.IsChecked = true;
            ViewModel.HandlingOrder = "priority";
            actionNo.IsChecked = true;
            replyNo.IsChecked = true;
            forInfo.IsChecked = true;
            //autoSuggestBoxToICSPosition.Text = "Planning";
            comboBoxToICSPosition.SelectedItem = "Planning";
            comboBoxFromICSPosition.SelectedItem = "Planning";
            //ToLocation = "Mountain View EOC";
            textBoxToLocation.Text = "Mountain View EOC";
            ViewModel.ReceivedOrSent = "sent";
            ViewModel.HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            UpdateFormFieldsRequiredColors();
        }


        //private string toICSPosition;
        //public string ToICSPosition
        //{
        //    get => toICSPosition;
        //    set => SetProperty(ref toICSPosition, value);
        //}

        //private string toLocation;
        //public string ToLocation
        //{
        //    get => toLocation;
        //    //set => SetProperty(ref toLocation, value);
        //    set => textBoxToLocation.Text = value;
        //}

        //private string fromLocation;
        //public string FromLocation
        //{
        //    get => fromLocation;
        //    set => SetProperty(ref fromLocation, value);
        //}

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override string PacFormType => "MVCERTSummary";

        //public override string MessageNo 
        //{ 
        //    get => base.MessageNo;
        //    set
        //    {
        //        base.MessageNo = value;
        //        ViewModel.OriginMsgNo = value;
        //    }
        //}

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override string CreateSubject()
        {
            //return (messageNo.Text + "_" + Severity?.ToUpper()[0] + "/" + HandlingOrder?.ToUpper()[0] + "_MTV213-CERT_" + subject.Text + comments.Text);
            return $"{messageNo.Text}_{ViewModel.HandlingOrder?.ToUpper()[0]}_MTV213-CERT_{subject.Text}";
        }

        public override void AppendDrillTraffic()
        {
            message.Text += DrillTraffic;
        }

        //protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        //{
        //    string[] data = formField.ControlContent.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
        //    if (data.Length == 2)
        //    {
        //        if (data[1] == (-1).ToString() || string.IsNullOrEmpty(data[1]))
        //        {
        //            return $"{id}: [}}0]";
        //        }
        //        else
        //        {
        //            //if (formField.ControlName == "comboBoxToICSPosition" || formField.ControlName == "comboBoxFromICSPosition")
        //            //{
        //            int index = Convert.ToInt32(data[1]);
        //            return $"{id}: [{data[0]}}}{index + 1}]";
        //            //}
        //            //else
        //            //{
        //            //    return $"{id}: [{data[0]}}}{data[1]}]";
        //            //}
        //        }
        //    }
        //    else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
        //    {
        //        return $"{id}: [}}0]";
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            CreateDamageAssesmentMessage(ref packetMessage);

            List<string> outpostData = new List<string>()
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:MTV 213 CERT SUMMARY (which4) ",
                "# JS-ver. MV/PR-4.7-3.7, 03/23/21",
                "# FORMFILENAME: MTV_213_CERT_Summary-v210323.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        private void CreateDamageAssesmentMessage(ref PacketMessage packetMessage)
        {
            string[] damageAssesmentMessage = new string[17];
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (formField.ControlName == "fires" && !string.IsNullOrWhiteSpace(formField.ControlContent))
                    damageAssesmentMessage[0] += $"F{formField.ControlContent}";
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

        //private TacticalCall GetSelectedMTVTactical()
        //{
        //    // Find Mountain View Tactical Call signs
        //    TacticalCallsignData mtvTacticalCallsigns = null;
        //    foreach (TacticalCallsignData data in TacticalCallsigns.TacticalCallsignDataDictionary.Values)
        //    {
        //        if (data.AreaName == "Local Mountain View")
        //        {
        //            mtvTacticalCallsigns = data;
        //            break;
        //        }
        //    }
        //    int index = mtvTacticalCallsigns.TacticalCallsigns.TacticalCallsignsArraySelectedIndex;
        //    if (index < 0)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return mtvTacticalCallsigns.TacticalCallsigns.TacticalCallsignsArray[index];
        //    }
        //}

        //private string GetAgencyNameFromPacketMessage()
        //{
        //    FormField agencyNameField = FormPacketMessage.FormFieldArray.Where(formField => formField.ControlName == "textBoxFromLocation").FirstOrDefault();
        //    return agencyNameField.ControlContent;
        //}

        //private string GetTacticalcallsignFromAgencyName(string agencyName)
        //{
        //    //List<TacticalCall> CERTLocationTacticalCalls
        //    foreach (TacticalCall tacticalCall in ViewModel.CERTLocationTacticalCalls)
        //    {
        //        if (tacticalCall.AgencyName == agencyName)
        //        {
        //            return tacticalCall.TacticalCallsign;
        //        }
        //    }
        //    return "";
        //}

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = false, found2 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox)
                {
                    switch (control.Name)
                    {
                        case "comboBoxFromLocationTextBox":
                            //string tacticalCallsign = GetTacticalcallsignFromAgencyName(formField.ControlContent);
                            //ViewModel.TacticalCallsign = tacticalCallsign;
                            ViewModel.TacticalCallsign = formField.ControlContent;
                            //comboBoxFromLocation.Text = formField.ControlContent;
                            found1 = true;
                            break;
                        case null:
                            continue;
                    }
                }

                if (control is ComboBox)
                {
                    switch (control.Name)
                    {
                        case "comboBoxFromLocation":
                            found2 = true;
                            // Filter out the selected index
                            int index = formField.ControlContent.LastIndexOf('}');
                            string location = "";
                            if (index > 0)
                            {
                                location = formField.ControlContent.Substring(0, index);
                            }
                            //string tacticalCallsign = GetTacticalcallsignFromAgencyName(location);
                            //ViewModel.TacticalCallsign = tacticalCallsign;
                            ViewModel.TacticalCallsign = location;
                            break;
                        case null:
                            continue;
                    }
                }
                if (found1 && found2)
                    break;
            }
            base.FillFormFromFormFields(formFields);

            UpdateDASummeryFields();
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
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.Name == "comboBoxFromLocation")
            {
                if (comboBoxFromLocation.SelectedIndex < 0 && comboBoxFromLocation.IsEditable)
                {
                    comboBoxFromLocationTextBox.Text = comboBoxFromLocation.Text;
                    //comboBox.Visibility = Visibility.Collapsed;
                    //comboBoxFromLocationTextBox.Visibility = Visibility.Visible;
                }
                else
                {
                    comboBoxFromLocationTextBox.Text = comboBoxFromLocation.SelectedItem.ToString();
                    //comboBoxFromLocationTextBox.Visibility = Visibility.Collapsed;
                    //comboBox.Visibility = Visibility.Visible;
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
                    comboBoxToICSPositionTextBox.Text = comboBoxToICSPosition.Text;
                }
                else
                {
                    comboBoxToICSPositionTextBox.Text = comboBoxToICSPosition.SelectedItem.ToString();
                }
            }
            else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
            {
                if (comboBoxFromICSPosition.SelectedIndex < 0 && comboBoxFromICSPosition.IsEditable)
                {
                    comboBoxFromICSPositionTextBox.Text = comboBoxFromICSPosition.Text;
                }
                else
                {
                    comboBoxFromICSPositionTextBox.Text = comboBoxFromICSPosition.SelectedItem.ToString();
                }
            }
            UpdateFormFieldsRequiredColors();
            //ComboBox_SelectionChanged(sender, e);
        }

        public void UpdateDASummeryFields()
        {
            List<TextBox> daSummeryList = new List<TextBox>();
            bool listStart = false;
            bool daSummeryFilled = false;
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl as Control;
                if (control is TextBox textBox)
                {
                    if (textBox.Name == "fires")
                        listStart = true;

                    if (listStart)
                    {
                        daSummeryList.Add(textBox);
                        if (!string.IsNullOrEmpty(textBox.Text))
                            daSummeryFilled = true;
                    }
                    if (textBox.Name == "messageOther")
                    {
                        FormControl msgFormControl = _formControlsList.Find(x => x.InputControl.Name == "message");
                        TextBox messageControl = msgFormControl.InputControl as TextBox;
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            messageControl.Tag = (messageControl.Tag as string).Replace(",required", ",conditionallyrequired");
                        }
                        else
                        {
                            messageControl.Tag = (messageControl.Tag as string).Replace(",conditionallyrequired", ",required");
                        }
                    }
                    if (textBox.Name == "surveyed")
                        break;
                }
            }
            foreach (TextBox textBox in daSummeryList)
            {
                if (daSummeryFilled)
                {
                    textBox.Tag = (textBox.Tag as string).Replace(",required", ",conditionallyrequired");
                }
                else
                {
                    textBox.Tag = (textBox.Tag as string).Replace(",conditionallyrequired", ",required");
                }
            }
        }

        private void DASummery_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDASummeryFields();
            UpdateFormFieldsRequiredColors();
        }

        private void ComboBoxFromLocation_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            //subject.Text = _subjectText + args.Text;
            comboBoxFromLocationTextBox.Text = args.Text;
        }

    }
}

