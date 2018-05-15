using System;

using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class BlankPage : Page
    {
        public BlankViewModel ViewModel { get; } = new BlankViewModel();

        public BlankPage()
        {
            InitializeComponent();
        }
    }
}
