using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FormControlBaseClass;
using PacketMessagingTS.Core.Helpers;
using SharedCode;
using SharedCode.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PublicNoticeFormControl
{
    [FormControl(
    FormControlName = "PublicNotice",
    FormControlMenuName = "SCCo Public Notice",
    FormControlType = FormControlAttribute.FormType.TestForm)
]


    public sealed partial class PublicNoticeControl : FormControlBase
    {
        public PublicNoticeControl()
        {
            this.InitializeComponent();
        }

        public override FormProvidersHelper.FormProviders FormProvider => FormProviders.PacForm;

        public override string PacFormName => "PublicNotice";

        public override string PacFormType => "PublicNoticeForm";

        public override Panel CanvasContainer => throw new NotImplementedException();

        public override Panel DirectPrintContainer => throw new NotImplementedException();

        public override List<Panel> PrintPanels => throw new NotImplementedException();

        public override void AppendDrillTraffic() => throw new NotImplementedException();

        public override string CreateOutpostData(ref PacketMessage packetMessage) => throw new NotImplementedException();

        public override string CreateSubject() => throw new NotImplementedException();
    }
}
