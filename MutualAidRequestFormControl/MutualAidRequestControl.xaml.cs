using System;
using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using FormControlBaseMvvmNameSpace;
using PacketMessagingTS.Core.Helpers;
using System.Reflection;
using System.Security.AccessControl;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MutualAidRequestFormControl
{
    [FormControl(
        FormControlName = "form-oa-mutual-aid-request-v2",
        FormControlMenuName = "RACES Mutual Aid Request",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]


    public sealed partial class MutualAidRequestControl : FormControlBase
    {
        readonly MutualAidRequestControlViewModel ViewModel = MutualAidRequestControlViewModel.Instance;

        readonly new List<ComboBoxItem> ToICSPositionItems = new List<ComboBoxItem>
        {
            new ComboBoxItem() {Content = "RACES Chief Radio Officer"},
            new ComboBoxItem() {Content = "RACES Unit"},
            new ComboBoxItem() {Content = "Operations Section"},
        };

        readonly List<ComboBoxItem> ResourceRole = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = null },
            new ComboBoxItem() { Content = "Field Communicator" },
            new ComboBoxItem() { Content = "Net Control Operator" },
            new ComboBoxItem() { Content = "Packet Operator" },
            new ComboBoxItem() { Content = "Shadow Communicator" },
        };

        readonly List<ComboBoxItem> TypeFieldCom = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null },
                new ComboBoxItem() { Content = "F1" },
                new ComboBoxItem() { Content = "F2" },
                new ComboBoxItem() { Content = "F3" },
                new ComboBoxItem() { Content = "Type IV" },
                new ComboBoxItem() { Content = "Type V" },
        };

        readonly List<ComboBoxItem> TypePacket = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null },
                new ComboBoxItem() { Content = "P1" },
                new ComboBoxItem() { Content = "P2" },
                new ComboBoxItem() { Content = "P3" },
                new ComboBoxItem() { Content = "Type IV" },
                new ComboBoxItem() { Content = "Type V" },
        };

        readonly List<ComboBoxItem> TypeNetCtrl = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null },
                new ComboBoxItem() { Content = "N1" },
                new ComboBoxItem() { Content = "N2" },
                new ComboBoxItem() { Content = "N3" },
                new ComboBoxItem() { Content = "Type IV" },
                new ComboBoxItem() { Content = "Type V" },
        };

        readonly List<ComboBoxItem> TypeShadow = new List<ComboBoxItem>
        {
                new ComboBoxItem() { Content = null },
                new ComboBoxItem() { Content = "S1" },
                new ComboBoxItem() { Content = "S2" },
                new ComboBoxItem() { Content = "S3" },
                new ComboBoxItem() { Content = "Type IV" },
                new ComboBoxItem() { Content = "Type V" },
        };

        readonly List<ComboBoxItem> ResourceRole1 = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> ResourceRole2 = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> ResourceRole3 = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> ResourceRole4 = new List<ComboBoxItem>();
        readonly List<ComboBoxItem> ResourceRole5 = new List<ComboBoxItem>();

        List<ComboBoxItem> ResourceType = new List<ComboBoxItem>();

        List<ComboBoxItem> ResourceType1P = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType1M = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType2P = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType2M = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType3P = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType3M = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType4P = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType4M = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType5P = new List<ComboBoxItem>();
        List<ComboBoxItem> ResourceType5M = new List<ComboBoxItem>();


        public MutualAidRequestControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            FormHeaderControl.ViewModel.HeaderString1 = "Santa Clara County RACES -- Mutual Aid Request";
            FormHeaderControl.ViewModel.HeaderSubstring = "Version: 20220129";            

            FormHeaderControl.SetToICSPosition(ToICSPositionItems);
            FormHeaderControl.SetToLocation("County EOC");

            CreateComboBoxList(ResourceRole1, ResourceRole);
            CreateComboBoxList(ResourceRole2, ResourceRole);
            CreateComboBoxList(ResourceRole3, ResourceRole);
            CreateComboBoxList(ResourceRole4, ResourceRole);
            CreateComboBoxList(ResourceRole5, ResourceRole);

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            FormHeaderControl.SetHandlingOrder(2);
            FormHeaderControl.SetToLocation("County EOC");

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "RACES-MAR";

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override void SetPracticeField(string practiceField)
        {
            agencyName.Text = practiceField;

            UpdateFormFieldsRequiredColors();
        }

        // PEH-1316P_R_RACES-MAR_Agency
        public override string CreateSubject()
        {
            return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_RACES-MAR_{agencyName.Text}";
        }

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        void UpdateRequiredResources(int resourceNo, bool roleChanged = false)
        {
            switch (resourceNo)
            {
                case 1:
                    if (roleChanged)
                    {
                        prefType1.Tag = prefType1.Tag.ToString().Replace("conditionallyrequired", "required");
                        minType1.Tag = minType1.Tag.ToString().Replace("conditionallyrequired", "required");
                    }
                    break;
                case 2:
                    radioQty2.Tag = radioQty2.Tag.ToString().Replace("conditionallyrequired", "required");
                    radioRole2.Tag = radioRole2.Tag.ToString().Replace("conditionallyrequired", "required");
                    if (roleChanged)
                    {
                        prefType2.Tag = prefType2.Tag.ToString().Replace("conditionallyrequired", "required");
                        radioMinType2.Tag = radioMinType2.Tag.ToString().Replace("conditionallyrequired", "required");
                    }
                    break;
                case 3:
                    radioQty3.Tag = radioQty3.Tag.ToString().Replace("conditionallyrequired", "required");
                    radioRole3.Tag = radioRole3.Tag.ToString().Replace("conditionallyrequired", "required");
                    if (roleChanged)
                    {
                        prefType3.Tag = prefType3.Tag.ToString().Replace("conditionallyrequired", "Required");
                        radioMinType3.Tag = radioMinType3.Tag.ToString().Replace("conditionallyrequired", "required");
                    }
                    break;
                case 4:
                    radioQty4.Tag = radioQty4.Tag.ToString().Replace("conditionallyrequired", "required");
                    radioRole4.Tag = radioRole4.Tag.ToString().Replace("conditionallyrequired", "required");
                    if (roleChanged)
                    {
                        prefType4.Tag = prefType4.Tag.ToString().Replace("conditionallyrequired", "required");
                        radioMinType4.Tag = radioMinType4.Tag.ToString().Replace("conditionallyrequired", "required");
                    }
                    break;
                case 5:
                    radioQty5.Tag = radioQty5.Tag.ToString().Replace("conditionallyrequired", "required");
                    radioRole5.Tag = radioRole5.Tag.ToString().Replace("conditionallyrequired", "required");
                    if (roleChanged)
                    {
                        prefType5.Tag = prefType5.Tag.ToString().Replace("conditionallyrequired", "required");
                        radioMinType5.Tag = radioMinType5.Tag.ToString().Replace("conditionallyrequired", "required");
                    }
                    break;
            }
            UpdateFormFieldsRequiredColors();
        }

        void Qty_TextChanged(object sender, TextChangedEventArgs e)
        {          
            TextBox qtyTextBox = sender as TextBox;
            int resourceNo = Convert.ToInt32(qtyTextBox.Name.Substring(8));       //radioQty
            UpdateRequiredResources(resourceNo);

            TextBox_TextChanged(sender, e);
        }


        void Role_Changed(object sender, SelectionChangedEventArgs e)
        {
            ResourceType = new List<ComboBoxItem>();
            ComboBox comboBox = sender as ComboBox;

            ComboBoxItem resoueceRole = e.AddedItems[0] as ComboBoxItem;

            switch (resoueceRole.Content)
            {
                case "Field Communicator":
                    CreateComboBoxList(ResourceType, TypeFieldCom);
                    break;

                case "Net Control Operator":
                    CreateComboBoxList(ResourceType, TypeNetCtrl);
                    break;

                case "Packet Operator":
                    CreateComboBoxList(ResourceType, TypePacket);
                    break;

                case "Shadow Communicator":
                    CreateComboBoxList(ResourceType, TypeShadow);
                    break;

                case "":
                    break;
            }

            int resourceNo = 0;
            switch (comboBox.Name)
            {
                case "radioRole1":
                    prefType1.ItemsSource = ResourceType;
                    ResourceType1M = new List<ComboBoxItem>();
                    CreateComboBoxList(ResourceType1M, ResourceType);
                    minType1.ItemsSource = ResourceType1M;
                    rolePosition1.Text = $"{resoueceRole.Content} / {radioPosition1.Text}";
                    resourceNo = 1;
                    break;

                case "radioRole2":
                    //CreateComboBoxList(ResourceType2P, ResourceType);
                    prefType2.ItemsSource = ResourceType;
                    ResourceType2M = new List<ComboBoxItem>();
                    CreateComboBoxList(ResourceType2M, ResourceType);
                    radioMinType2.ItemsSource = ResourceType2M;
                    rolePosition2.Text = $"{resoueceRole.Content} / {radioPosition2.Text}";
                    resourceNo = 2;
                    break;

                case "radioRole3":
                    //CreateComboBoxList(ResourceType3P, ResourceType);
                    prefType3.ItemsSource = ResourceType;
                    ResourceType3M = new List<ComboBoxItem>();
                    CreateComboBoxList(ResourceType3M, ResourceType);
                    radioMinType3.ItemsSource = ResourceType3M;
                    rolePosition3.Text = $"{resoueceRole.Content} / {radioPosition3.Text}";
                    resourceNo = 3;
                    break;

                case "radioRole4":
                    //CreateComboBoxList(ResourceType4P, ResourceType);
                    prefType4.ItemsSource = ResourceType;
                    ResourceType4M = new List<ComboBoxItem>();
                    CreateComboBoxList(ResourceType4M, ResourceType);
                    radioMinType4.ItemsSource = ResourceType4M;
                    rolePosition4.Text = $"{resoueceRole.Content} / {radioPosition4.Text}";
                    resourceNo = 4;
                    break;

                case "radioRole5":
                    //CreateComboBoxList(ResourceType5P, ResourceType);
                    prefType5.ItemsSource = ResourceType;
                    ResourceType5M = new List<ComboBoxItem>();
                    CreateComboBoxList(ResourceType5M, ResourceType);
                    radioMinType5.ItemsSource = ResourceType5M;
                    rolePosition5.Text = $"{resoueceRole.Content} / {radioPosition5.Text}";
                    resourceNo = 5;
                    break;
            }
            UpdateRequiredResources(resourceNo, true);

            ComboBox_SelectionChanged(sender, e);
            UpdateFormFieldsRequiredColors();
        }

        void position_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            switch (textBox.Name)
            {
                case "radioPosition1":
                    rolePosition1.Text = $"{(radioRole1.SelectedItem as ComboBoxItem)?.Content} / {radioPosition1.Text}";
                    break;

                case "radioPosition2":
                    rolePosition2.Text = $"{(radioRole2.SelectedItem as ComboBoxItem)?.Content} / {radioPosition2.Text}";
                    break;

                case "radioPosition3":
                    rolePosition3.Text = $"{(radioRole3.SelectedItem as ComboBoxItem)?.Content} / {radioPosition3.Text}";
                    break;

                case "radioPosition4":
                    rolePosition4.Text = $"{(radioRole4.SelectedItem as ComboBoxItem)?.Content} / {radioPosition4.Text}";
                    break;

                case "radioPosition5":
                    rolePosition5.Text = $"{(radioRole5.SelectedItem as ComboBoxItem)?.Content} / {radioPosition5.Text}";
                    break;
            }

        }

        private void formHeaderControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //public override string MsgDate
        //{
        //    get => _msgDate;
        //    set
        //    {
        //        //SignedDate = value;
        //        SetProperty(ref _msgDate, value);
        //        signedDate.Text = value;
        //        UpdateFormFieldsRequiredColors();
        //    }
        //}

        //private string _SignedDate;
        //public string SignedDate
        //{
        //    get => _SignedDate;
        //    set => SetProperty(ref _SignedDate, value);
        //}

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false;
        //    foreach (FormField formField in formFields)
        //    {
        //        FrameworkElement control = GetFrameworkElement(formField);

        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is TextBox textBox)
        //        {
        //            switch (control.Name)
        //            {
        //                case "signedDate":
        //                    SignedDate = formField.ControlContent;
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
        //}

    }
}
