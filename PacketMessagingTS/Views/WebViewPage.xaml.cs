using System;

using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class WebViewPage : Page
    {
        public WebViewViewModel ViewModel { get; } = new WebViewViewModel();

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }

        //For example, if the content of a web view named webView1 contains a function
        //named setDate that takes 3 parameters, you can invoke it like this.

        //C#
        //    string[] args = {"January", "1", "2000"};
        //    string returnValue = await webView1.InvokeScriptAsync("setDate", args);
    }
}
