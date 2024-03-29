﻿using System;
using System.Collections.Generic;

using FormControlBaseClass;

using FormControlBaseMvvmNameSpace;

using FormControlBasicsNamespace;

using PacketMessagingTS.Core.Helpers;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace HospitalRollCallFormControl
{
    [FormControl(
        FormControlName = "hospital_roll_call",
        FormControlMenuName = "Hospital RollCall",
        FormControlType = FormControlAttribute.FormType.HospitalForm
        )
    ]

    public sealed partial class HospitalRollCallControl : FormControlBase
    {
        HospitalRollCallControlViewModel ViewModel = HospitalRollCallControlViewModel.Instance;

        public HospitalRollCallControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            GetFormDataFromAttribute(GetType());

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            //InitializeToggleButtonGroups();
        }

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "ICS-213 Test";

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

        public override void AppendDrillTraffic() {}

        public override string CreateOutpostData(ref PacketMessage packetMessage) => throw new NotImplementedException();

        public override string CreateSubject()
        {
            //return $"{messageNo.Text}_{HandlingOrder?.ToUpper()[0]}_ICS213_{subject.Text}";
            //return $"_{ViewModelBase.HandlingOrder?.ToUpper()[0]}_ICS213_";
            return "";
        }

        //public ObservableCollection<Hospital> DataGridSource => new ObservableCollection<Hospital>(HospitalRollCall.Instance.HospitalList);
    }

}
