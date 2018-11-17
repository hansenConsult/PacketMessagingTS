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

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Views
{
    public sealed partial class ICS309ControlForPrint : Page
    {
        public ToolsViewModel _toolsViewModel { get; } = Singleton<ToolsViewModel>.Instance;

        public ICS309ControlForPrint()
        {
            this.InitializeComponent();
        }
    }
}
