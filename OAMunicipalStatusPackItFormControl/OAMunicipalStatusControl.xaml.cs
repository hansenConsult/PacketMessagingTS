﻿using System.Collections.Generic;

using FormControlBaseClass;
using FormControlBasicsNamespace;
using FormUserControl;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using FormControlBaseMvvmNameSpace;
using PacketMessagingTS.Core.Helpers;
using System.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OAMunicipalStatusPackItFormControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    [FormControl(
        FormControlName = "form-oa-muni-status",
        FormControlMenuName = "OA Jurisdiction Status",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]

    public sealed partial class OAMunicipalStatusControl : FormControlBase
    {
        readonly new List<ComboBoxItem> ToICSPositionItems = new List<ComboBoxItem>
        {
            new ComboBoxItem() {Content = "Situation Analysis Unit"},
            new ComboBoxItem() {Content = "Planning Section"},
        };

        List<ComboBoxItem> _Municipalities = new List<ComboBoxItem>();
        //{
        //        new ComboBoxItem() {Content = "Campbell"},
        //        new ComboBoxItem() {Content = "Cupertino"},
        //        new ComboBoxItem() {Content = "Gilroy"},
        //        new ComboBoxItem() {Content = "Los Altos"},
        //        new ComboBoxItem() {Content = "Los Altos Hills"},
        //        new ComboBoxItem() {Content = "Los Gatos"},
        //        new ComboBoxItem() {Content = "Milpitas"},
        //        new ComboBoxItem() {Content = "Monte Sereno"},
        //        new ComboBoxItem() {Content = "Morgan Hill"},
        //        new ComboBoxItem() {Content = "Mountain View"},
        //        new ComboBoxItem() {Content = "Palo Alto"},
        //        new ComboBoxItem() {Content = "San Jose"},
        //        new ComboBoxItem() {Content = "Santa Clara"},
        //        new ComboBoxItem() {Content = "Saratoga"},
        //        new ComboBoxItem() {Content = "Sunnyvale"},
        //        //new ComboBoxItem() {Content = "*Unincorporated County Areas*", Tag = "Unincorporated"}
        //};
        readonly List<ComboBoxItem> OfficeStatus = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = null, Tag = "" },
                new ComboBoxItem() {Content = "Unknown", Background = LightGrayBrush},
                new ComboBoxItem() {Content = "Open", Background = LightGreenBrush},
                new ComboBoxItem() {Content = "Closed", Background = PinkBrush},
        };
        readonly List<ComboBoxItem> UnknownYesNo = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = null, Tag = "" },
                new ComboBoxItem() {Content = "Unknown", Background = LightGrayBrush },
                new ComboBoxItem() {Content = "Yes", Background = PinkBrush },
                new ComboBoxItem() {Content = "No", Background = LightGreenBrush },
        };
        readonly List<ComboBoxItem> ActivationLevel = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = null, Tag = ""},
                new ComboBoxItem() {Content = "Normal", Background = LightGreenBrush},
                new ComboBoxItem() {Content = "Duty Officer", Background = YellowBrush},
                new ComboBoxItem() {Content = "Monitor", Background = OrangeBrush},
                new ComboBoxItem() {Content = "Partial", Background = PinkBrush},
                new ComboBoxItem() {Content = "Full", Background = PinkBrush}
        };

        readonly List<ComboBoxItem> Communications = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = null, Tag = "" },
                new ComboBoxItem() {Content = "Unknown", Background = LightGrayBrush },
                new ComboBoxItem() {Content = "Normal", Background = LightGreenBrush },
                new ComboBoxItem() {Content = "Problem", Background = YellowBrush },
                new ComboBoxItem() {Content = "Failure", Background = PinkBrush },
                new ComboBoxItem() {Content = "Delayed", Background = WhiteBrush },
                new ComboBoxItem() {Content = "Closed", Background = WhiteBrush },
                new ComboBoxItem() {Content = "Early Out", Background = WhiteBrush },
        };

        readonly List<ComboBoxItem> UnknownYesNo1 = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> UnknownYesNo2 = new List<ComboBoxItem>();


        //public OAMunicipalStatusControlViewModel ViewModel = OAMunicipalStatusControlViewModel.Instance;
        public OAMunicipalStatusControlViewModel ViewModel = new OAMunicipalStatusControlViewModel();
        readonly List<ComboBoxItem> Debris = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> Flooding = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> Hazmat = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> EmergencyServices = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> Casualties = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> UtilitiesGas = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> UtilitiesElectric = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> InfrastructurePower = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> InfrastructureWater = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> InfrastructureSewer = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> SearchAndRescue = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> TransportationsRoads = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> TransportationsBridges = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> CivilUnrest = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> AnimalIssues = new List<ComboBoxItem>();


        public OAMunicipalStatusControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            FormHeaderControl.ViewModel.HeaderString1 = "Santa Clara OA Jurisdiction Status";
            FormHeaderControl.ViewModel.HeaderSubstring = "WebEOC: 20190327";
            FormHeaderControl.ViewModel.PIF = "2.2";

            FormHeaderControl.SetToICSPosition(ToICSPositionItems);
            FormHeaderControl.SetToLocation("County EOC");

            CreateComboBoxList(UnknownYesNo1, UnknownYesNo);
            CreateComboBoxList(UnknownYesNo2, UnknownYesNo);

            CreateComboBoxList(Debris, Communications);
            CreateComboBoxList(Flooding, Communications);
            CreateComboBoxList(Hazmat, Communications);
            CreateComboBoxList(EmergencyServices, Communications);
            CreateComboBoxList(Casualties, Communications);
            CreateComboBoxList(UtilitiesGas, Communications);
            CreateComboBoxList(UtilitiesElectric, Communications);
            CreateComboBoxList(InfrastructurePower, Communications);
            CreateComboBoxList(InfrastructureWater, Communications);
            CreateComboBoxList(InfrastructureSewer, Communications);
            CreateComboBoxList(SearchAndRescue, Communications);
            CreateComboBoxList(TransportationsRoads, Communications);
            CreateComboBoxList(TransportationsBridges, Communications);
            CreateComboBoxList(CivilUnrest, Communications);
            CreateComboBoxList(AnimalIssues, Communications);

            _Municipalities = base.Municipalities;

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            FormHeaderControl.SetHandlingOrder(0);

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "OA Municipal Status";

        public override void AppendDrillTraffic()
        { }

        public override void SetPracticeField(string practiceField)
        {
            //reportType.SelectedIndex = 0;

            //ComboBoxItem practiceComboBoxItem = new ComboBoxItem() { Content = practiceField };
            ////if (jurisdictionName.Items.Count == 0 || (base.Municipalities.Count == _Municipalities.Count))
            ////if (practiceComboBoxItem.Content != jurisdictionName.Items[_Municipalities.Count - 1].Content)
            //{
            //    _Municipalities.Add(practiceComboBoxItem);
            //    //jurisdictionName.ItemsSource = _Municipalities;
            //    //if (jurisdictionName.Items.Count == 16)
            //    {
            //        try
            //        {
            //            //jurisdictionName.Text = practiceField;
            //            jurisdictionName.SelectedIndex = _Municipalities.Count - 1;
            //        }
            //        catch
            //        {
            //            jurisdictionName.SelectedIndex = -1;
            //        }
            //    }
            //}
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1, printPage2 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            //return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_JurisStat_{(jurisdictionName.SelectedValue as ComboBoxItem)?.Content}";
            return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_JurisStat_{jurisdictionName.Text}";
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formControl"></param>
        /// <param name="validationState">true - invalid, false - Valid </param>
        //void UpdateControlValidationInfo(FormControl formControl, bool validationState)
        //{
        //    if (validationState)
        //    {
        //        AddToErrorString(GetTagErrorMessage(formControl.InputControl));
        //        formControl.InputControl.BorderBrush = formControl.RequiredBorderBrush;
        //    }
        //    else
        //    {
        //        formControl.InputControl.BorderBrush = formControl.BaseBorderColor;
        //    }
        //}

        //public override string ValidateForm(string errorText = "")
        //{
        //    base.ValidateForm(errorText);

        //    foreach (FormControl formControl in FormControlsList)
        //    {
        //        bool notUpdated;
        //        Control control = formControl.InputControl;
        //        switch (control.Name)
        //        {
        //            case "municipalityName":
        //                notUpdated = (control as ComboBox).SelectedIndex == -1;
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "eocOpen":
        //                notUpdated = (control as ComboBox).SelectedIndex == -1;
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "activationLevel":
        //                notUpdated = control.IsEnabled && (control as ComboBox).SelectedIndex == 0;
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "stateOfEmergency":
        //                //notUpdated = (control as ComboBox).SelectedIndex == -1;
        //                //UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "howSent":
        //                notUpdated = stateOfEmergency.SelectedItem as string == "Yes" && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsCommunications":
        //                //notUpdated = comboBoxCommunications.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                //UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsDebris":
        //                notUpdated = comboBoxDebris.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsFlooding":
        //                notUpdated = comboBoxFlooding.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsHazmat":
        //                notUpdated = comboBoxHazmat.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsEmergencyServices":
        //                notUpdated = comboBoxEmergencyServices.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsCasualties":
        //                notUpdated = comboBoxCasualties.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsUtilitiesGas":
        //                notUpdated = comboBoxUtilitiesGas.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsUtilitiesElectric":
        //                notUpdated = comboBoxUtilitiesElectric.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsInfrastructurePower":
        //                notUpdated = comboBoxInfrastructurePower.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsInfrastructureWater":
        //                notUpdated = comboBoxInfrastructureWater.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsInfrastructureSewer":
        //                notUpdated = comboBoxInfrastructureSewer.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsSearchAndRescue":
        //                notUpdated = comboBoxSearchAndRescue.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsTransportationsRoads":
        //                notUpdated = comboBoxTransportationsRoads.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsTransportationsBridges":
        //                notUpdated = comboBoxTransportationsBridges.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsCivilUnrest":
        //                notUpdated = comboBoxCivilUnrest.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //            case "commentsAnimalIssues":
        //                notUpdated = comboBoxAnimalIssues.SelectedIndex > 1 && string.IsNullOrEmpty((control as TextBox).Text);
        //                UpdateControlValidationInfo(formControl, notUpdated);
        //                break;
        //        }
        //    }
        //    return ValidationResultMessage;
        //}

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false;
        //    foreach (FormField formField in formFields)
        //    {
        //        FrameworkElement control = GetFrameworkElement(formField);

        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is ToggleButtonGroup toggleButtonGroup)
        //        {
        //            switch (control.Name)
        //            {
        //                case "reportType":
        //                    ReportType = formField.ControlContent;
        //                    found1 = true;
        //                    break;
        //                case null:
        //                    continue;
        //            }
        //        }
        //        if (found1)
        //            break;
        //    }
        //    base.FillFormFromFormFields(formFields);

        //    UpdateFormFieldsRequiredColors();
        //}

        //private void EocOpen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (activationLevel != null)
        //    {
        //        activationLevel.IsEnabled = ((sender as ComboBox).SelectedItem as ComboBoxPackItItem)?.Item == "Yes";
        //        if (activationLevel.IsEnabled)
        //        {
        //            activationLevel.SelectedIndex = 0;
        //        }
        //        ComboBox_SelectionChanged(sender, e);
        //    }
        //}

        //private void UpdateCommentsTag(ComboBox comboBox, TextBox textBox)
        //{
        //    if (comboBox.SelectedIndex > 1)
        //    {
        //        textBox.Tag = (textBox.Tag as string).Replace("conditionallyrequired", "required");
        //    }
        //    else
        //    {
        //        textBox.Tag = (textBox.Tag as string).Replace(",required", ",conditionallyrequired");
        //    }
        //    ValidateForm();
        //}

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

            if (((e.AddedItems[0] is ComboBoxPackItItem) && ((e.AddedItems[0] as ComboBoxPackItItem).Item == "Yes")) 
                || ((e.AddedItems[0] is ComboBoxItem) && (((e.AddedItems[0] as ComboBoxItem).Content as string) == "Yes"))
                || (bool)complete.IsChecked)
            {
                howSent.Tag = (howSent.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            else
            {
                howSent.Tag = (howSent.Tag as string).Replace(",required", ",conditionallyrequired");
            }

            //FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == howSent.Name);

            //foreach (FormControl formControl in _formControlsList)
            //{
            //    if (howSent.Name == formControl.InputControl.Name)
            //    {
            //        if (string.IsNullOrEmpty(howSent.Text) && IsFieldRequired(howSent))
            //        {
            //            howSent.BorderBrush = formControl.RequiredBorderBrush;
            //            howSent.BorderThickness = new Thickness(2);
            //        }
            //        else
            //        {
            //            howSent.BorderBrush = formControl.BaseBorderColor;
            //            howSent.BorderThickness = new Thickness(1);
            //        }
            //    }
            //}

            //(sender as ComboBox).Background = (e.AddedItems[0] as ComboBoxPackItItem).BackgroundBrush;

            UpdateFormFieldsRequiredColors();

            ComboBox_SelectionChanged(sender, e);
        }

        void JurisdictionName_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;            
            //if (comboBox.SelectedIndex < 0)
            //{
                jurisdictionNameBaseTextBox.Text = comboBox.Text;
            //}
            //else
            //{
            //    jurisdictionNameBaseTextBox.Text = "Unincorporated";
            //}

            Subject_Changed(sender, e);
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

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        //{

        //}
    }
}

