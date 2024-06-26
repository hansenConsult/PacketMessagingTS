﻿using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBaseMvvmNameSpace;

using FormUserControl;

using Microsoft.Toolkit.Uwp.UI;

using PacketMessagingTS.Core.Helpers;

using SharedCode;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HavBedReportFormControl
{
    [FormControl(
        FormControlName = "form-mhoc-beds-status",
        FormControlMenuName = "XSC HAvBed Report",
        FormControlType = FormControlAttribute.FormType.HospitalForm
        )
    ]

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HavBedReportControl : FormControlBase
    {
        private const string Key = "HAvBedNumberTextBox";


        readonly List<ComboBoxItem> CommandCenterStatus = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = null, Tag = "" },
            new ComboBoxItem() { Content = "Available", Background = LightGreenBrush },
            new ComboBoxItem() { Content = "Drill or Exercise", Background = LightGrayBrush },
            new ComboBoxItem() { Content = "Full Activation", Background = PinkBrush },
            new ComboBoxItem() { Content = "Monitoring", Background = OrangeBrush },
            new ComboBoxItem() { Content = "Not Activated", Background = LightGreenBrush },
            new ComboBoxItem() { Content = "Unvailable", Background = PinkBrush },
            new ComboBoxItem() { Content = "Limited Activation", Background = PinkBrush },
        };

        readonly List<ComboBoxItem> Decon = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = null, Tag = "" },
            new ComboBoxItem() { Content = "Exceeded", Background = BlackBrush, Foreground = WhiteBrush },
            new ComboBoxItem() { Content = "Full", Background = PinkBrush },
            new ComboBoxItem() { Content = "Inactive", Background = YellowBrush },
            new ComboBoxItem() { Content = "Open", Background = LightGreenBrush },
        };

        private readonly HavBedReportControlViewModel ViewModel = new HavBedReportControlViewModel();



        public HavBedReportControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            FormHeaderControl.ViewModel.HeaderString1 = "SCCo Medical Health Branch - HAvBed Report";
            FormHeaderControl.ViewModel.HeaderSubstring = "EMResource: c190320";
            FormHeaderControl.ViewModel.PIF = "2.2";

            FormHeaderControl.SetToICSPosition(ToICSPositionItems);
            //formHeaderControl.SetToLocation(ToICSLocationItems);

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            FormHeaderControl.SetHandlingOrder(0);

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "HAVBEDSTATUS";

        public override void AppendDrillTraffic()
        {
            //specialInstructions.Text += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels => new List<Panel> { printPage1 };

        public override FormHeaderUserControl FormHeaderControl => formHeaderControl;

        public override RadioOperatorUserControl RadioOperatorControl => radioOperatorControl;

        public override string CreateSubject()
        {
            return $"{formHeaderControl.ViewModelBase.OriginMsgNo}_{formHeaderControl.ViewModelBase.HandlingOrder?.ToUpper()[0]}_HAvBed_{(hospitalName.SelectedValue as ComboBoxItem)?.Content}";
        }

        //public override string CreateOutpostData(ref PacketMessage packetMessage)
        //{
        //    _outpostData = new List<string>
        //    {
        //        "!SCCoPIFO!",
        //        "#T: form-mhoc-beds-status.html",
        //        $"#V: {PackItFormVersion}-{PIF}",
        //    };
        //    CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

        //    return CreateOutpostMessageBody(_outpostData);
        //}

        public override void FormatTextBoxes()
        {
            if (FormPacketMessage is null)
                return;

            if (FormPacketMessage.MessageState == MessageState.Locked)
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    (string id, FrameworkElement control) = GetTagIndex(formField);
                    if (control is TextBox textBox)
                    {
                        if (string.Compare(id, "50.") <= 0 && string.Compare(id, "41.") >= 0)
                        {
                            textBox.TextAlignment = TextAlignment.Left;
                        }
                    }
                }
            }
        }

    }
}
