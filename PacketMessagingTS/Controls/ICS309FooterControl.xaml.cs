using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class ICS309FooterControl : UserControl
    {
        public readonly ICS309FooterViewModel ics309FooterViewModel = ICS309FooterViewModel.Instance;

        public ICS309FooterControl()
        {
            InitializeComponent();

            ics309FooterViewModel.OperatorNameCallsignFooter = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
            ics309FooterViewModel.DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
        }

        public ICS309FooterControl(int pageNo)
        {
            InitializeComponent();

            ics309FooterViewModel.OperatorNameCallsignFooter = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
            ics309FooterViewModel.DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
            ics309FooterViewModel.PageNoAsString = $"Page {pageNo} of {ics309FooterViewModel.TotalPages}";
            //ics309FooterViewModel.PageNo = pageNo;
        }

        public void PageNoOf(int pageNo)
        {
            pageNoOf.Text = $"Page {pageNo} of {ics309FooterViewModel.TotalPages}";
        }
    }
}
