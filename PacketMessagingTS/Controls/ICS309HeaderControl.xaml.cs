using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

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
    public sealed partial class ICS309HeaderControl : UserControl
    {
        public readonly ICS309HeaderViewModel ViewModel = ICS309HeaderViewModel.Instance;

        public ICS309HeaderControl()
        {
            this.InitializeComponent();

            if (!ICS309ViewModel.Instance.FromOpenFile)
            {
                ViewModel.OperationalPeriodStart = DateTime.Today;
                ViewModel.OperationalPeriodEnd = DateTime.Now;
                ViewModel.OperationalPeriod = $"{DateTimeStrings.DateTimeString(ViewModel.OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(ViewModel.OperationalPeriodEnd)}";
                ViewModel.RadioNetName = IdentityViewModel.Instance.UseTacticalCallsign ? $"{IdentityViewModel.Instance.TacticalCallsign}" : "";
                ViewModel.OperatorNameCallsign = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
            }

        }
    }
}
