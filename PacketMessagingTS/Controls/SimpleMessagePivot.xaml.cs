using System;

using FormControlBasicsNamespace;

using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class SimpleMessagePivot : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventMessageChanged;
        public event EventHandler<FormEventArgs> EventSimpleMsgSubjectChanged;

        public int SelectedIndex
        { get; set; }

        public string Message
        { get; set; }

        public string Subject
        { get;
            set; }


        public SimpleMessagePivot()
        {
            this.InitializeComponent();
        }

        private void SimpleMessageTypePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string header = "";
            if (e.AddedItems.Count == 1)
            {
                header = (string)(e.AddedItems[0] as PivotItem).Header;
            }
            SelectedIndex = (sender as Pivot).SelectedIndex;

            string chechInOut = "";
            if (SelectedIndex == 0)
            {
                Subject = $"{PacketSettingsViewModel.Instance.DefaultSubject}";
                Message = PacketSettingsViewModel.Instance.DefaultMessage;
            }
            else if (SelectedIndex == 1)    // Check in
            {
                chechInOut = "Check-In";
            }
            else if (SelectedIndex == 2)    // Check out
            {
                chechInOut = "Check-Out";
            }
            if (SelectedIndex > 0)
            {
                //string userCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
                string userCallsign = IdentityViewModel.Instance.UserCallsign;
                string userName = IdentityViewModel.Instance.UserName;
                if (IdentityViewModel.Instance.UseTacticalCallsign)
                {
                    string tacticalCallsign = IdentityViewModel.Instance.TacticalCallsign;
                    string tacticalAgencyName = IdentityViewModel.Instance.TacticalAgencyName;
                    Subject = $"{chechInOut} {tacticalCallsign}, {tacticalAgencyName}";
                    //Message = $"{chechInOut} {tacticalCallsign}, {tacticalAgencyName} \r\nPresent are:\r\n{userCallsign}, {userName}\r\n";
                    Message = $"{chechInOut} {tacticalCallsign}, {tacticalAgencyName} \r\n{userCallsign}, {userName}\r\n";
                }
                else
                {
                    Subject = $"{chechInOut} {userCallsign}, {userName}";
                    Message = $"{chechInOut} {userCallsign}, {userName} \r\n";
                }
            }

            // Create event Message changed
            EventHandler<FormEventArgs> OnMessageChange = EventMessageChanged;
            FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = Message };
            OnMessageChange?.Invoke(this, formEventArgs);

            EventHandler<FormEventArgs> OnSubjectChange = EventSimpleMsgSubjectChanged;
            formEventArgs = new FormEventArgs() { SubjectLine = Subject };
            OnSubjectChange?.Invoke(this, formEventArgs);
        }
    }
}
