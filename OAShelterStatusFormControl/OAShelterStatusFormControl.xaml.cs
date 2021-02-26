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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OAShelterStatusFormControl
{
    [FormControl(
        FormControlName = "form-oa-shelter-status",
        FormControlMenuName = "OA Shelter Status",
        FormControlType = FormControlAttribute.FormType.CountyForm,
        FormControlMenuIndex = 4)
    ]

    public sealed partial class OAShelterStatusControl : FormControlBase
    {
        readonly List<ComboBoxItem> Municipalities = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = "Campbell" },
                new ComboBoxItem() { Content = "Cupertino" },
                new ComboBoxItem() { Content = "Gilroy" },
                new ComboBoxItem() { Content = "Los Altos" },
                new ComboBoxItem() { Content = "Los Altos Hills" },
                new ComboBoxItem() { Content = "Los Gatos" },
                new ComboBoxItem() { Content = "Milpitas" },
                new ComboBoxItem() { Content = "Monte Sereno" },
                new ComboBoxItem() { Content = "Morgan Hill" },
                new ComboBoxItem() { Content = "Mountain View" },
                new ComboBoxItem() { Content = "Palo Alto" },
                new ComboBoxItem() { Content = "San Jose" },
                new ComboBoxItem() { Content = "Santa Clara" },
                new ComboBoxItem() { Content = "Saratoga" },
                new ComboBoxItem() { Content = "Sunnyvale" },
                new ComboBoxItem() { Content = "Unincorporated Areas", Tag = "Unincorporated" }
        };

        public List<ComboBoxItem> ShelterTypes = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = "Type 1" },
                new ComboBoxItem() { Content = "Type 2" },
                new ComboBoxItem() { Content = "Type 3" },
                new ComboBoxItem() { Content = "Type 4" },
        };


        //public IList<ComboBoxPackItItem> ShelterStatuses = new List<ComboBoxPackItItem>
        //{
        //        new ComboBoxPackItItem("Open", LightGreenBrush),
        //        new ComboBoxPackItItem("Closed", PinkBrush),
        //        new ComboBoxPackItItem("Full", YellowBrush),
        //};
        public IList<ComboBoxItem> ShelterStatuses = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = "Open", Background = LightGreenBrush },
                new ComboBoxItem() { Content = "Closed", Background = PinkBrush },
                new ComboBoxItem() { Content = "Full", Background = YellowBrush },
        };


        public IList<ComboBoxItem> ShelterStatusStrings = new List<ComboBoxItem>();

        public List<ComboBoxItem> YesNoContentPet = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null, Tag = ""},
                new ComboBoxItem() { Content = "Yes", Tag = "checked" },
                new ComboBoxItem() { Content = "No", Tag = "false" },
        };

        public List<ComboBoxItem> YesNoContentSafety = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null, Tag = ""},
                new ComboBoxItem() { Content = "Yes", Tag = "checked" },
                new ComboBoxItem() { Content = "No", Tag = "false" },
        };

        public List<ComboBoxItem> YesNoContentArt20 = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null, Tag = ""},
                new ComboBoxItem() { Content = "Yes", Tag = "checked" },
                new ComboBoxItem() { Content = "No", Tag = "false" },
        };

        readonly List<ComboBoxItem> Managers = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = "American Red Cross" },
                new ComboBoxItem() { Content = "Private" },
                new ComboBoxItem() { Content = "Community" },
                new ComboBoxItem() { Content = "Government" },
                new ComboBoxItem() { Content = "Other" },
        };

        public OAShelterStatusControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            state.Text = "California";

            FormHeaderControl.HeaderString1 = "Santa Clara OA Shelter Status";
            FormHeaderControl.HeaderSubstring = "WebEOC: 20130814";

            UpdateFormFieldsRequiredColors();
        }

    public override FormControlBasics RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override string GetPacFormName() => "form-oa-shelter-status";

        public override string PacFormType => "OAShelterStat";

        public override void AppendDrillTraffic()
        { }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1, printPage2 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;


        public override string CreateSubject()
        {
            return $"{formHeaderControl.OriginMsgNo}_{formHeaderControl.HandlingOrder?.ToUpper()[0]}_OAShelterStat_{shelterName.Text}";
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                "#T: form-oa-shelter-status.html",
                $"#V: {PackItFormVersion}-{FormHeaderControl.PIF}",
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
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
            bool found1 = false, found2 = false, found3 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "shelterName":
                            ShelterName = formField.ControlContent;
                            found1 = true;
                            break;
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

        protected override void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            TextBox textBox = FindName($"{comboBox.Name}TextBox") as TextBox;

            if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxPackItItem>)
            {
                if (FormPacketMessage.MessageState != MessageState.Locked)
                    return;

                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    //if (formField.ControlName == comboBox.Name && formField.ControlComboxContent.SelectedIndex < 0)
                    if (formField.ControlName == comboBox.Name)
                    {
                        //comboBox.SelectedValue = formField.ControlContent;
                        int index = 0;
                        foreach (ComboBoxPackItItem packItItem in comboBox.ItemsSource as List<ComboBoxPackItItem>)
                        {
                            if (packItItem.PacketData == formField.ControlContent)
                            {
                                packItItem.SelectedIndex = index;
                                comboBox.SelectedIndex = index;
                                //if (FormPacketMessage.MessageState == MessageState.Locked)
                                //{
                                //    textBox = FindName($"{comboBox.Name}TextBox") as TextBox;
                                    textBox.Background = packItItem.BackgroundBrush;
                                //}
                                break;
                            }
                            index++;
                        }
                        break;
                    }
                }
            }
            if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxItem>)
            {
                if (FormPacketMessage.MessageState != MessageState.Locked)
                    return;

                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    if (formField.ControlName == comboBox.Name && !string.IsNullOrEmpty(formField.ControlContent))
                    {
                        var co = comboBox.Items.Count;
                        bool tagFnd = false;
                        bool backgroundColorFound = false;
                        foreach (ComboBoxItem comboBoxItem in comboBox.Items)
                        {
                            if ((comboBoxItem.Tag as string) == formField.ControlContent)
                            {
                                tagFnd = true;
                                comboBox.SelectedItem = comboBoxItem;
                                textBox.Text = comboBoxItem.Content as string;
                                if (comboBoxItem.Background != null)
                                {
                                    backgroundColorFound = true;
                                    textBox.Background = (comboBox.SelectedItem as ComboBoxItem).Background;
                                    break;
                                }
                            }
                            if ((comboBoxItem.Content as string) == formField.ControlContent)
                            {
                                if (comboBoxItem.Background != null)
                                {
                                    backgroundColorFound = true;
                                    comboBox.SelectedItem = comboBoxItem;
                                    textBox.Background = (comboBox.SelectedItem as ComboBoxItem).Background;
                                    //break;
                                }
                            }
                        }
                        if (!tagFnd && !backgroundColorFound)
                        {
                            comboBox.SelectedValue = formField.ControlContent;
                        }
                        break;
                    }
                }

            }
        }

    }
}
