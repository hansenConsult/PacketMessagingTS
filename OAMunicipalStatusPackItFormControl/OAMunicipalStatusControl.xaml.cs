using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OAMunicipalStatusPackItFormControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    [FormControl(
        FormControlName = "form-oa-muni-status",
        FormControlMenuName = "OA Municipal Status",
        FormControlType = FormControlAttribute.FormType.TestForm)
    ]

    public sealed partial class OAMunicipalStatusControl : FormControlBase
    {
        List<ComboBoxPackItItem> Municipalities = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem("Campbell"),
                new ComboBoxPackItItem("Cupertino"),
                new ComboBoxPackItItem("Gilroy"),
                new ComboBoxPackItItem("Los Altos"),
                new ComboBoxPackItItem("Los Altos Hills"),
                new ComboBoxPackItItem("Los Gatos"),
                new ComboBoxPackItItem("Milpitas"),
                new ComboBoxPackItItem("Monte Sereno"),
                new ComboBoxPackItItem("Morgan Hill"),
                new ComboBoxPackItItem("Mountain View"),
                new ComboBoxPackItItem("Palo Alto"),
                new ComboBoxPackItItem("San Jose"),
                new ComboBoxPackItItem("Santa Clara"),
                new ComboBoxPackItItem("Saratoga"),
                new ComboBoxPackItItem("Sunnyvale"),
                new ComboBoxPackItItem("*Unincorporated County Areas*", "Unincorporated")
        };

        List<ComboBoxPackItItem> OfficeStatus = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Unknown", LightGrayBrush),
                new ComboBoxPackItItem("Open", LightGreenBrush),
                new ComboBoxPackItItem("Closed", PinkBrush),
        };

        List<ComboBoxPackItItem> UnknownYesNo = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Unknown", LightGrayBrush),
                new ComboBoxPackItItem("Yes", PinkBrush),
                new ComboBoxPackItItem("No", LightGreenBrush),
        };

        List<ComboBoxPackItItem> ActivationLevel = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Normal", LightGreenBrush),
                new ComboBoxPackItItem("Duty Officer", YellowBrush),
                new ComboBoxPackItItem("Monitor", OrangeBrush),
                new ComboBoxPackItItem("Partial", PinkBrush),
                new ComboBoxPackItItem("Full", PinkBrush)
        };

        List<ComboBoxPackItItem> CurrentSituation = new List<ComboBoxPackItItem>
        {
                new ComboBoxPackItItem(null, ""),
                new ComboBoxPackItItem("Unknown", LightGrayBrush),
                new ComboBoxPackItItem("Normal", LightGreenBrush),
                new ComboBoxPackItItem("Problem", YellowBrush),
                new ComboBoxPackItItem("Failure", PinkBrush),
                new ComboBoxPackItItem("Delayed", WhiteBrush),
                new ComboBoxPackItItem("Closed,", WhiteBrush),
                new ComboBoxPackItItem("Early Out", WhiteBrush),
        };

        public OAMunicipalStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();
        }

        public string JurisdictionName
        {
            get => jurisdictionName.SelectedValue as string;
            set => jurisdictionName.SelectedValue = value;
        }

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-oa-muni-status";

        public override string PacFormType => "OA Municipal Status";

        public override string CreateSubject()
        {
            return $"{MessageNo}_{HandlingOrder?.ToUpper()[0]}_OAMuniStat_{JurisdictionName}";
        }

        
        public string ReportType
        { get; set; }

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
                        notUpdated = control.IsEnabled && (control as ComboBox).SelectedIndex == 0;
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "stateOfEmergency":
                        //notUpdated = (control as ComboBox).SelectedIndex == -1;
                        //UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "howSent":
                        notUpdated = stateOfEmergency.SelectedItem as string == "Yes" && string.IsNullOrEmpty((control as TextBox).Text);
                        UpdateControlValidationInfo(formControl, notUpdated);
                        break;
                    case "commentsCommunications":
                        //notUpdated = comboBoxCommunications.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
                        //UpdateControlValidationInfo(formControl, notUpdated);
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
                "!SCCoPIFO!",
                "#T: form-oa-muni-status.html",
                "#V: 2.17-2.1",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        {
            if (formField.ControlName == "municipalityName")
            {
                string outpostData = formField.ControlContent;
                if (outpostData.Contains("*"))
                {
                    outpostData = "Unincorporated";
                }
                return $"{id}: [{outpostData}]";
            }
            else
            {
                return base.CreateComboBoxOutpostDataString(formField, id);
            }
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

        private void EocOpen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (activationLevel != null)
            {
                activationLevel.IsEnabled = ((sender as ComboBox).SelectedItem as ComboBoxPackItItem)?.Item == "Yes";
                if (activationLevel.IsEnabled)
                {
                    activationLevel.SelectedIndex = 0;
                }
                ComboBox_SelectionChanged(sender, e);
            }
        }

        private void UpdateCommentsTag(ComboBox comboBox, TextBox textBox)
        {
            return;
            if (comboBox.SelectedIndex > 1)
            {
                textBox.Tag = (textBox.Tag as string).Replace("conditionallyrequired", "required");
            }
            else
            {
                textBox.Tag = (textBox.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            ValidateForm();
        }

        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //switch ((sender as ComboBox).Name)
            //{
            //    case "comboBoxCommunications":
            //        UpdateCommentsTag(sender as ComboBox, commentsCommunications);
            //        break;
            //    case "comboBoxDebris":
            //        UpdateCommentsTag(sender as ComboBox, commentsDebris);
            //        break;
            //    case "comboBoxFlooding":
            //        UpdateCommentsTag(sender as ComboBox, commentsFlooding);
            //        break;
            //    case "comboBoxHazmat":
            //        UpdateCommentsTag(sender as ComboBox, commentsHazmat);
            //        break;
            //    case "comboBoxEmergencyServices":
            //        UpdateCommentsTag(sender as ComboBox, commentsEmergencyServices);
            //        break;
            //    case "comboBoxCasualties":
            //        UpdateCommentsTag(sender as ComboBox, commentsCasualties);
            //        break;
            //    case "comboBoxUtilitiesGas":
            //        UpdateCommentsTag(sender as ComboBox, commentsUtilitiesGas);
            //        break;
            //    case "comboBoxUtilitiesElectric":
            //        UpdateCommentsTag(sender as ComboBox, commentsUtilitiesElectric);
            //        break;
            //    case "comboBoxInfrastructurePower":
            //        UpdateCommentsTag(sender as ComboBox, commentsInfrastructurePower);
            //        break;
            //    case "comboBoxInfrastructureWater":
            //        UpdateCommentsTag(sender as ComboBox, commentsInfrastructureWater);
            //        break;
            //    case "comboBoxInfrastructureSewer":
            //        UpdateCommentsTag(sender as ComboBox, commentsInfrastructureSewer);
            //        break;
            //    case "comboBoxSearchAndRescue":
            //        UpdateCommentsTag(sender as ComboBox, commentsSearchAndRescue);
            //        break;
            //    case "comboBoxTransportationsRoads":
            //        UpdateCommentsTag(sender as ComboBox, commentsTransportationsRoads);
            //        break;
            //    case "comboBoxTransportationsBridges":
            //        UpdateCommentsTag(sender as ComboBox, commentsTransportationsBridges);
            //        break;
            //    case "comboBoxCivilUnrest":
            //        UpdateCommentsTag(sender as ComboBox, commentsCivilUnrest);
            //        break;
            //    case "comboBoxAnimalIssues":
            //        UpdateCommentsTag(sender as ComboBox, commentsAnimalIssues);
            //        break;
            //}
            ComboBox_SelectionChanged(sender, e);
        }

        private void StateOfEmergency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e == null || e.AddedItems.Count == 0)
                return;

            if ((e.AddedItems[0] as ComboBoxPackItItem).Item == "Yes" || (bool)complete.IsChecked)
            {
                howSent.Tag = (howSent.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            else
            {
                howSent.Tag = (howSent.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            foreach (FormControl formControl in _formControlsList)
            {
                if (howSent.Name == formControl.InputControl.Name)
                {
                    if (string.IsNullOrEmpty(howSent.Text) && IsFieldRequired(howSent))
                    {
                        howSent.BorderBrush = formControl.RequiredBorderBrush;
                        howSent.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        howSent.BorderBrush = formControl.BaseBorderColor;
                        howSent.BorderThickness = new Thickness(1);
                    }
                }
            }

            (sender as ComboBox).Background = (e.AddedItems[0] as ComboBoxPackItItem).BackgroundBrush;

            //UpdateFormFieldsRequiredColors();
            //ValidateForm();
            ComboBox_SelectionChanged(sender, e);
        }

        protected override void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
                eocPhone.Tag = (eocPhone.Tag as string).Replace(",required", ",conditionallyrequired");
                primEMContactName.Tag = (primEMContactName.Tag as string).Replace(",required", ",conditionallyrequired");
                primEMContactPhone.Tag = (primEMContactPhone.Tag as string).Replace(",required", ",conditionallyrequired");
                officeStatus.Tag = (officeStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                eocOpen.Tag = (eocOpen.Tag as string).Replace(",required", ",conditionallyrequired");
                activationLevel.Tag = (activationLevel.Tag as string).Replace(",required", ",conditionallyrequired");
                stateOfEmergency.Tag = (stateOfEmergency.Tag as string).Replace(",required", ",conditionallyrequired");
                howSent.Tag = (howSent.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            else
            {
                eocPhone.Tag = (eocPhone.Tag as string).Replace(",conditionallyrequired", ",required");
                primEMContactName.Tag = (primEMContactName.Tag as string).Replace(",conditionallyrequired", ",required");
                primEMContactPhone.Tag = (primEMContactPhone.Tag as string).Replace(",conditionallyrequired", ",required");
                officeStatus.Tag = (officeStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                eocOpen.Tag = (eocOpen.Tag as string).Replace(",conditionallyrequired", ",required");
                activationLevel.Tag = (activationLevel.Tag as string).Replace(",conditionallyrequired", ",required");
                stateOfEmergency.Tag = (stateOfEmergency.Tag as string).Replace(",conditionallyrequired", ",required");
                howSent.Tag = (howSent.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            string minval = "41.";
            bool startCurrentStatus = false;
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.InputControl is ComboBox comboBox)
                {
                    if ((comboBox.Tag as string).Contains(minval))
                    {
                        startCurrentStatus = true;
                    }
                    if (startCurrentStatus)
                    {
                        if (required)
                        {
                            comboBox.Tag = (comboBox.Tag as string).Replace(",conditionallyrequired", ",required");
                        }
                        else
                        {
                            comboBox.Tag = (comboBox.Tag as string).Replace(",required", ",conditionallyrequired");
                        }
                    }
                }
            }
            UpdateFormFieldsRequiredColors();
        }

    }
}

