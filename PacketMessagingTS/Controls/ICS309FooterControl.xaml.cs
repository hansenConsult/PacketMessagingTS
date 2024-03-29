﻿
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class ICS309FooterControl : UserControl
    {
        public readonly ICS309FooterViewModel ics309FooterViewModel = ICS309FooterViewModel.Instance;

        public ICS309FooterControl()
        {
            InitializeComponent();

            pageNoOf.Text = $"Page 1 of 1";
        }

        public ICS309FooterControl(int pageNo)
        {
            InitializeComponent();

            pageNoOf.Text = $"Page {pageNo} of {ics309FooterViewModel.TotalPages}";
        }
    }
}
