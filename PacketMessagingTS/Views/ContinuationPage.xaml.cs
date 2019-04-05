using System;

using ICS309UserControl;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// A paged used to flow text from a given text container
    /// Usage: Output scenarios 1-4 might not fit entirely on a given "printer page"
    /// In that case simply add new continuation pages of the given size until all the content can be displayed
    /// </summary>
    public sealed partial class ContinuationPage : Page
    {
        public ContinuationViewModel _continuationViewModel { get; } = new ContinuationViewModel();


        public ContinuationPage()
        {
            InitializeComponent();

            ICS309UserControl.ICS309Control ics309Control = new ICS309UserControl.ICS309Control();
            formControl.Children.Add(ics309Control);

            ics309Control.IncidentName = _continuationViewModel.IncidentName;
            //operationalPeriod.Text = FormatDateTime(_toolsViewModel.OperationalPeriodStart) + " to " + FormatDateTime(_toolsViewModel.OperationalPeriodEnd);
            ics309Control.RadioNetName = _continuationViewModel.RadioNetName;
            //ics309Control. = $"{Singleton<IdentityViewModel>.Instance.UserName}, {Singleton<IdentityViewModel>.Instance.UserCallsign}";
            //ics309Control.DateTimePrepared = DateTime.Now;
            ////dateTimePrepared.Text = FormatDateTime(_toolsViewModel.DateTimePrepared);
            ////preparedByNameCallsign.Text = radioOperator.Text;
            //ics309Control.AddToMessageList();
        }

        public ContinuationPage(FormsPage formsControl)
        {
            InitializeComponent();

            formControl.Children.Add(formsControl);
        }

        ///// <summary>
        ///// Creates a continuation page and links text-flow to a text flow container
        ///// </summary>
        ///// <param name="textLinkContainer">Text link container which will flow text into this page</param>
        //public ContinuationPage(RichTextBlockOverflow textLinkContainer)
        //{
        //    InitializeComponent();
        //    //textLinkContainer.OverflowContentTarget = ContinuationPageLinkedContainer;
        //}
    }
}
