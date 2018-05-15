using System;

using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class TabbedPage : Page
    {
        public TabbedViewModel ViewModel { get; } = new TabbedViewModel();

        public TabbedPage()
        {
            InitializeComponent();
        }
    }
}
