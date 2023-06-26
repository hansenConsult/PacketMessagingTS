using System;
using System.Collections.Generic;

using FormControlBaseClass;
using FormUserControl;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using FormControlBasicsNamespace;
using Windows.UI.Xaml.Media;
using System.Drawing;
using FormControlBaseMvvmNameSpace;
using PacketMessagingTS.Core.Helpers;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAShelterStatusFormControl
{
    [FormControl(
        FormControlName = "form-oa-shelter-status",
        FormControlMenuName = "OA Shelter Status",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]

    public sealed partial class OAShelterStatusControl : FormControlBase
    {
        readonly OAShelterStatusFormControlViewModel ViewModel = new OAShelterStatusFormControlViewModel();

        readonly new List<ComboBoxItem> ToICSPositionItems = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = "Mass Care and Shelter Unit" },
            new ComboBoxItem() { Content = "Care and Shelter Branch" },
            new ComboBoxItem() { Content = "Operations Section" },
        };

        //readonly List<ComboBoxItem> Municipalities = new List<ComboBoxItem>
        //{
        //        new ComboBoxItem() { Content = "Campbell" },
        //        new ComboBoxItem() { Content = "Cupertino" },
        //        new ComboBoxItem() { Content = "Gilroy" },
        //        new ComboBoxItem() { Content = "Los Altos" },
        //        new ComboBoxItem() { Content = "Los Altos Hills" },
        //        new ComboBoxItem() { Content = "Los Gatos" },
        //        new ComboBoxItem() { Content = "Milpitas" },
        //        new ComboBoxItem() { Content = "Monte Sereno" },
        //        new ComboBoxItem() { Content = "Morgan Hill" },
        //        new ComboBoxItem() { Content = "Mountain View" },
        //        new ComboBoxItem() { Content = "Palo Alto" },
        //        new ComboBoxItem() { Content = "San Jose" },
        //        new ComboBoxItem() { Content = "Santa Clara" },
        //        new ComboBoxItem() { Content = "Saratoga" },
        //        new ComboBoxItem() { Content = "Sunnyvale" },
        //        //new ComboBoxItem() { Content = "Unincorporated Areas", Tag = "Unincorporated" }
        //};


        //public IList<ComboBoxItem> ShelterStatusStrings = new List<ComboBoxItem>();

        public OAShelterStatusControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            //InitializeToggleButtonGroups();

            state.Text = "California";

            FormHeaderControl.ViewModel.HeaderString1 = "Santa Clara OA Shelter Status";
            FormHeaderControl.ViewModel.HeaderSubstring = "WebEOC: 20130814";
            FormHeaderControl.ViewModelBase.PIF = "2.3";

            FormHeaderControl.SetToICSPosition(ToICSPositionItems);

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            FormHeaderControl.SetHandlingOrder(1);

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        //public override string GetPacFormName() => "form-oa-shelter-status";

        public override string PacFormType => "OAShelterStat";

        public override void AppendDrillTraffic()
        { }

        public override void SetPracticeField(string practiceField) 
        {
            shelterName.Text = practiceField;
            shelterDetailsName.Text = practiceField;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1, printPage2 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;


        public override string CreateSubject()
        {
            return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_SheltStat_{shelterName.Text}";
        }

        private void UpdateAvailability()
        {
            try
            {
                int occupancyInt = string.IsNullOrEmpty(occupancy.Text) ? 0 : Convert.ToInt32(occupancy.Text);
                int capacityInt = string.IsNullOrEmpty(capacity.Text) ? 0 : Convert.ToInt32(capacity.Text);
                availablity.Text = (capacityInt - occupancyInt).ToString();
            }
            //catch (FormatException fe)
            catch
            {
                return;
            }

        }

        private void Capacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_IntegerChanged(sender, e);

            UpdateAvailability();
        }

        protected override void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
                shelterType.Tag = (shelterType.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterStatus.Tag = (shelterStatus.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterAddress.Tag = (shelterAddress.Tag as string).Replace(",required", ",conditionallyrequired");
                shelterCity.Tag = (shelterCity.Tag as string).Replace(",required", ",conditionallyrequired");
                state.Tag = (state.Tag as string).Replace(",required", ",conditionallyrequired");
                capacity.Tag = (capacity.Tag as string).Replace(",required", ",conditionallyrequired");
                occupancy.Tag = (occupancy.Tag as string).Replace(",required", ",conditionallyrequired");
                managedBy.Tag = (managedBy.Tag as string).Replace(",required", ",conditionallyrequired");
                primaryContact.Tag = (primaryContact.Tag as string).Replace(",required", ",conditionallyrequired");
                primaryContactPhone.Tag = (primaryContactPhone.Tag as string).Replace(",required", ",conditionallyrequired");
            }
            else
            {
                shelterType.Tag = (shelterType.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterStatus.Tag = (shelterStatus.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterAddress.Tag = (shelterAddress.Tag as string).Replace(",conditionallyrequired", ",required");
                shelterCity.Tag = (shelterCity.Tag as string).Replace(",conditionallyrequired", ",required");
                state.Tag = (state.Tag as string).Replace(",conditionallyrequired", ",required");
                capacity.Tag = (capacity.Tag as string).Replace(",conditionallyrequired", ",required");
                occupancy.Tag = (occupancy.Tag as string).Replace(",conditionallyrequired", ",required");
                managedBy.Tag = (managedBy.Tag as string).Replace(",conditionallyrequired", ",required");
                primaryContact.Tag = (primaryContact.Tag as string).Replace(",conditionallyrequired", ",required");
                primaryContactPhone.Tag = (primaryContactPhone.Tag as string).Replace(",conditionallyrequired", ",required");
            }
            UpdateFormFieldsRequiredColors();
        }

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = true, found2 = false, found3 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        //case "shelterName":
                        //    shelterDetailsName.Text = formField.ControlContent;
                        //    //    ShelterName = formField.ControlContent;
                        //        found1 = true;
                        //    break;
                        case "capacity":
                            capacity.Text = formField.ControlContent;
                            UpdateAvailability();
                            found2 = true;
                            break;
                        case "occupancy":
                            occupancy.Text = formField.ControlContent;
                            UpdateAvailability();
                            found3 = true;
                            break;
                        case null:
                            continue;
                    }
                }
                if (found1 && found2 && found3)
                    break;
            }
            base.FillFormFromFormFields(formFields);
        }

        void ShelterName_Changed(object sender, RoutedEventArgs e)
        {
            TextBox shelterName = sender as TextBox;
            shelterDetailsName.Text = shelterName.Text;

            Subject_Changed(sender, e);
        }

        void ShelterCity_Changed(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedIndex >= 0)
            {
                shelterCityBaseTextBox.Text = comboBox.Text;
            }
            //else
            //{
            //    shelterCityBaseTextBox.Text = "Unincorporated";
            //}

            Subject_Changed(sender, e);
        }

        void ManagedBy_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RadioButtons managByButtons = sender as RadioButtons;
            if (managByButtons.SelectedItem == null)
                return;
            if ((managByButtons.SelectedItem as RadioButton).Name == "managedByOther")
            {
                textBoxOther.Text = "";
                textBoxOther.Visibility = Visibility.Visible;
            }
            else
            {
                textBoxOther.Visibility = Visibility.Collapsed;
                textBoxOther.Text = (string)(managByButtons.SelectedItem as RadioButton).Content;
            }
        }

    }
}
