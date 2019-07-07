using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OAMunicipalStatusFormControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    [FormControl(
        FormControlName = "XSC_OA_MuniStatus_v20130101",
        FormControlMenuName = "OA Municipal Status",
        FormControlType = FormControlAttribute.FormType.CountyForm)
    ]

    public sealed partial class OAMunicipalStatusControl : FormControlBase
    {
        string[] Municipalities = new string[] {
                "Campbell",
                "Cupertino",
                "Gilroy",
                "Loma Prieta",
                "Los Altos",
                "Los Altos Hills",
                "Los Gatos/Monte Sereno",
                "Milpitas",
                "Morgan Hill",
                "Mountain View",
                "NASA-Ames",
                "Palo Alto",
                "San Jose",
                "Santa Clara",
                "Saratoga",
                "Stanford",
                "Sunnyvale",
                "Unincorporated"
        };

        string[] OfficeStatus = new string[]
        {
                "Unknown",
                "Open",
                "Closed"
        };

        string[] UnknownYesNo = new string[] {
                "Unknown",
                "Yes",
                "No"
        };

        string[] ActivationLevel = new string[] {
                "Unknown",
                "Monitor",
                "Minimal",
                "Partial",
                "Full"
        };

        string[] CurrentSituation = new string[] {
                "Unknown",
                "Normal",
                "Problem",
                "Failure"
        };

        public OAMunicipalStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            sent.IsChecked = true; ;
            packet.IsChecked = true;
        }

        public string MunicipalityName
        //{ get => (municipalityName.SelectedIndex == 0 ? "" : municipalityName.SelectedItem as string); }
        { get => municipalityName.SelectedItem as string; }

        public override FormProviders DefaultFormProvider => FormProviders.PacForm;

        private FormProviders formProvider = FormProviders.PacForm;
        public override FormProviders FormProvider
        {
            get => formProvider;
            set => formProvider = value;
        }

        public override string PacFormName => "XSC_OA_MuniStatus_v20130101";

        public override string PacFormType => "OA Municipal Status";

        public override string CreateSubject()
        {
            return (MessageNo + '_' + Severity?.ToUpper()[0] + '/' + HandlingOrder?.ToUpper()[0] + "_OAMuniStat_" + MunicipalityName + '_' + incidentName.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formControl"></param>
        /// <param name="validationState">true - invalid, false - Valid </param>
        void UpdateControlValidationInfo(FormControl formControl, bool validationState)
        {
            if (validationState)
            {
                AddToErrorString(GetTagErrorMessage(formControl.InputControl));
                formControl.InputControl.BorderBrush = formControl.RequiredBorderBrush;
            }
            else
            {
                formControl.InputControl.BorderBrush = formControl.BaseBorderColor;
            }
        }

        public override string ValidateForm(string errorText = "")
        {
            base.ValidateForm(errorText);

            foreach (FormControl formControl in FormControlsList)
            {
                bool notUpdated;
                Control control = formControl.InputControl;
                switch (control.Name)
                {
                    case "municipalityName":
                        notUpdated = (control as ComboBox).SelectedIndex == -1;
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "eocOpen":
                        notUpdated = (control as ComboBox).SelectedIndex == -1;
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "activationLevel":
                        notUpdated = control.IsEnabled && (control as ComboBox).SelectedIndex == -1;
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "stateOfEmergency":
                        notUpdated = (control as ComboBox).SelectedIndex == -1;
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "howSent":
                        notUpdated = stateOfEmergency.SelectedIndex == 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsCommunications":
                        notUpdated = comboBoxCommunications.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsDebris":
                        notUpdated = comboBoxDebris.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsFlooding":
                        notUpdated = comboBoxFlooding.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsHazmat":
                        notUpdated = comboBoxHazmat.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsEmergencyServices":
                        notUpdated = comboBoxEmergencyServices.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsCasualties":
                        notUpdated = comboBoxCasualties.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsUtilitiesGas":
                        notUpdated = comboBoxUtilitiesGas.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsUtilitiesElectric":
                        notUpdated = comboBoxUtilitiesElectric.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsInfrastructurePower":
                        notUpdated = comboBoxInfrastructurePower.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsInfrastructureWater":
                        notUpdated = comboBoxInfrastructureWater.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsInfrastructureSewer":
                        notUpdated = comboBoxInfrastructureSewer.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsSearchAndRescue":
                        notUpdated = comboBoxSearchAndRescue.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsTransportationsRoads":
                        notUpdated = comboBoxTransportationsRoads.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsTransportationsBridges":
                        notUpdated = comboBoxTransportationsBridges.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsCivilUnrest":
                        notUpdated = comboBoxCivilUnrest.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsAnimalIssues":
                        notUpdated = comboBoxAnimalIssues.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                }
            }
            return ValidationResultMessage;
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            outpostData = new List<string>
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:SC-OA-Muni Status (which4)",
                "# JS-ver. PR-4.4-1.9, 06/29/18",
                "# FORMFILENAME: XSC_OA_MuniStatus_v20130101.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        //public override FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines)
        //{
        //    FormField[] formFields = CreateEmptyFormFieldsArray();

        //    foreach (FormField formField in formFields)
        //    {
        //        (string id, Control control) = GetTagIndex(formField);

        //        if (control is ToggleButtonGroup)
        //        {
        //            foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
        //            {
        //                string radioButtonIndex = GetTagIndex(radioButton);
        //                if ((GetOutpostValue(radioButtonIndex, ref msgLines)?.ToLower()) == "true")
        //                {
        //                    formField.ControlContent = radioButton.Name;
        //                }
        //            }

        //        }
        //        else if (control is CheckBox)
        //        {
        //            formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "true" ? "True" : "False");
        //        }
        //        else if (control is ComboBox comboBox)
        //        {
        //            string conboBoxData = GetOutpostValue(id, ref msgLines);
        //            var comboBoxDataSet = conboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
        //            formField.ControlContent = comboBoxDataSet[0];
        //        }
        //        else
        //        {
        //            formField.ControlContent = GetOutpostValue(id, ref msgLines);
        //        }
        //    }
        //    return formFields;
        //}

        private void eocOpen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activationLevel != null)
            {
                activationLevel.IsEnabled = (sender as ComboBox).SelectedIndex == 1;
                if ((sender as ComboBox).SelectedIndex == 1)
                {
                    ComboBoxRequired_SelectionChanged(activationLevel, null);
                }
                ComboBoxRequired_SelectionChanged(sender, e);
            }
        }

        private void UpdateCommentsTag(ComboBox comboBox, TextBox textBox)
        {
            if (comboBox.SelectedIndex > 1)
            {
                textBox.Tag = (textBox.Tag as string).Replace("conditionallyrequired", "required");
                ValidateForm();
            }
            else
            {
                textBox.Tag = (textBox.Tag as string).Replace(",required", ",conditionallyrequired");
                ValidateForm();
            }
        }

        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as ComboBox).Name)
            {
                case "comboBoxCommunications":
                    UpdateCommentsTag(sender as ComboBox, commentsCommunications);
                    break;
                case "comboBoxDebris":
                    UpdateCommentsTag(sender as ComboBox, commentsDebris);
                    break;
                case "comboBoxFlooding":
                    UpdateCommentsTag(sender as ComboBox, commentsFlooding);
                    break;
                case "comboBoxHazmat":
                    UpdateCommentsTag(sender as ComboBox, commentsHazmat);
                    break;
                case "comboBoxEmergencyServices":
                    UpdateCommentsTag(sender as ComboBox, commentsEmergencyServices);
                    break;
                case "comboBoxCasualties":
                    UpdateCommentsTag(sender as ComboBox, commentsCasualties);
                    break;
                case "comboBoxUtilitiesGas":
                    UpdateCommentsTag(sender as ComboBox, commentsUtilitiesGas);
                    break;
                case "comboBoxUtilitiesElectric":
                    UpdateCommentsTag(sender as ComboBox, commentsUtilitiesElectric);
                    break;
                case "comboBoxInfrastructurePower":
                    UpdateCommentsTag(sender as ComboBox, commentsInfrastructurePower);
                    break;
                case "comboBoxInfrastructureWater":
                    UpdateCommentsTag(sender as ComboBox, commentsInfrastructureWater);
                    break;
                case "comboBoxInfrastructureSewer":
                    UpdateCommentsTag(sender as ComboBox, commentsInfrastructureSewer);
                    break;
                case "comboBoxSearchAndRescue":
                    UpdateCommentsTag(sender as ComboBox, commentsSearchAndRescue);
                    break;
                case "comboBoxTransportationsRoads":
                    UpdateCommentsTag(sender as ComboBox, commentsTransportationsRoads);
                    break;
                case "comboBoxTransportationsBridges":
                    UpdateCommentsTag(sender as ComboBox, commentsTransportationsBridges);
                    break;
                case "comboBoxCivilUnrest":
                    UpdateCommentsTag(sender as ComboBox, commentsCivilUnrest);
                    break;
                case "comboBoxAnimalIssues":
                    UpdateCommentsTag(sender as ComboBox, commentsAnimalIssues);
                    break;
            }
        }

        private void StateOfEmergency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex == 1)
            {
                howSent.Tag = (howSent.Tag as string).Replace(",conditionallyrequired", ",required");
                ValidateForm();
            }
            else
            {
                howSent.Tag = (howSent.Tag as string).Replace(",required", ",conditionallyrequired");
                ValidateForm();
            }
            ComboBoxRequired_SelectionChanged(sender, e);
        }

    }
}

