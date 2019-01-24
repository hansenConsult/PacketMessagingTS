﻿using FormControlBaseClass;
using SharedCode;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

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
                "SELECT the Municipality",
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
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeControls();

            sent.IsChecked = true; ;
            packet.IsChecked = true;

        }

		public string IncidentName
        { get => incidentName.Text; }

        public string MunicipalityName
        { get => (municipalityName.SelectedIndex == 0 ? "" : municipalityName.SelectedItem as string); }

        public override string PacFormName => "XSC_OA_MuniStatus_v20130101";

        public override string PacFormType => "OA Municipal Status";

        public override string CreateSubject()
        {
            return (MessageNo + '_' + Severity?.ToUpper()[0] + '/' + HandlingOrder?.ToUpper()[0] + "_OAMuniStat_" + MunicipalityName + '_' + IncidentName);
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
                formControl.InputControl.BorderBrush = _redBrush;
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
                bool validationState;
                Control control = formControl.InputControl;
                switch (control.Name)
                {
                    case "municipalityName":
                        validationState = (control as ComboBox).SelectedIndex == 0;
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "eocOpen":
                        validationState = (control as ComboBox).SelectedIndex == 0;
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "activationLevel":
                        validationState = control.IsEnabled && (control as ComboBox).SelectedIndex == 0;
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "stateOfEmergency":
                        validationState = (control as ComboBox).SelectedIndex == 0;
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "howSent":
                        validationState = stateOfEmergency.SelectedIndex == 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsCommunications":
                        validationState = comboBoxCommunications.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsDebris":
                        validationState = comboBoxDebris.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsFlooding":
                        validationState = comboBoxFlooding.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsHazmat":
                        validationState = comboBoxHazmat.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsEmergencyServices":
                        validationState = comboBoxEmergencyServices.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsCasualties":
                        validationState = comboBoxCasualties.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsUtilitiesGas":
                        validationState = comboBoxUtilitiesGas.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsUtilitiesElectric":
                        validationState = comboBoxUtilitiesElectric.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsInfrastructurePower":
                        validationState = comboBoxInfrastructurePower.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsInfrastructureWater":
                        validationState = comboBoxInfrastructureWater.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsInfrastructureSewer":
                        validationState = comboBoxInfrastructureSewer.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsSearchAndRescue":
                        validationState = comboBoxSearchAndRescue.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsTransportationsRoads":
                        validationState = comboBoxTransportationsRoads.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsTransportationsBridges":
                        validationState = comboBoxTransportationsBridges.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsCivilUnrest":
                        validationState = comboBoxCivilUnrest.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
                        break;
                    case "commentsAnimalIssues":
                        validationState = comboBoxAnimalIssues.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, validationState);
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
            }
        }

    }
}

