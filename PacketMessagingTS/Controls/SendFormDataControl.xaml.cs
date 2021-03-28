using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FormControlBaseClass;

using FormControlBaseMvvmNameSpace;
//using FormControlBasicsNamespace;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public partial class SendFormDataControl : FormControlBaseMvvm
    //public partial class SendFormDataControl : FormControlBasics
    {
        public static readonly DependencyProperty MessageSubjectProperty =
                DependencyProperty.Register(
                "MessageSubject",
                typeof(string),
                typeof(SendFormDataControl),
                null);

        SendFormDataControlViewModel SendFormDataControlViewModel =  SendFormDataControlViewModel.Instance;

        public SendFormDataControl()
        {
            InitializeComponent();

            ScanControls(messageInfo);

            //(string bbs, string tnc, string from) = Utilities.GetProfileDataBBSStatusChecked();
            //SendFormDataControlViewModel.MessageBBS = bbs; //Utilities.GetBBSName(out string from, out string tnc);
            SendFormDataControlViewModel.MessageBBS = Utilities.GetSenderBBSStatusChecked();
            //AddressBook.Instance.UserBBS = MessageBBS;
            SendFormDataControlViewModel.MessageTNC = PacketSettingsViewModel.Instance.CurrentProfile.TNC;
            SendFormDataControlViewModel.MessageTo = PacketSettingsViewModel.Instance.CurrentProfile.SendTo;
            //SendFormDataControlViewModel.MessageFrom = from;
            IdentityViewModel instance = IdentityViewModel.Instance;
            SendFormDataControlViewModel.MessageFrom = instance.UseTacticalCallsign ? instance.TacticalCallsign : instance.UserCallsign;
        }

        //public override FormControlBasics RootPanel => rootPanel;
        public override FormControlBaseMvvm RootPanel => rootPanel;

        //private string messageSubject;
        //public string MessageSubject
        //{
        //    get => messageSubject;
        //    set
        //    {
        //        string validatedSubject = ValidateSubject(value);
        //        SetProperty(ref messageSubject, validatedSubject);
        //    }
        //}

        //public ObservableCollection<BBSData> BBSArray => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataArray);

        //private string originalBBS;

        //private string messageBBS;
        //public string MessageBBS
        //{
        //    get => messageBBS;
        //    set => SetProperty(ref messageBBS, value);
        //}

        //public ObservableCollection<TNCDevice> DeviceList => new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);

        //private string messageTNC;
        //public string MessageTNC
        //{
        //    get => messageTNC;
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            return;

        //        SetProperty(ref messageTNC, value);

        //        if (messageTNC.Contains(PublicData.EMail))
        //        {
        //            if (!string.IsNullOrEmpty(SendFormDataControlViewModel.MessageBBS))
        //            {
        //                originalBBS = SendFormDataControlViewModel.MessageBBS;
        //            }
        //            SendFormDataControlViewModel.MessageBBS = comboBoxMessageBBS.Text = "";    // TODO fix this
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(originalBBS))
        //            {
        //                SendFormDataControlViewModel.MessageBBS = comboBoxMessageBBS.Text = originalBBS;
        //            }
        //        }
        //    }
        //}

        //private string messageFrom;
        //public string MessageFrom
        //{
        //    get => messageFrom;
        //    set => SetProperty(ref messageFrom, value);
        //}

        //private string messageTo;
        //public string MessageTo
        //{
        //    get => messageTo;
        //    set => SetProperty(ref messageTo, value);
        //}

        //private bool isToIndividuals = true;
        //public bool IsToIndividuals
        //{
        //    get => isToIndividuals;
        //    set => SetProperty(ref isToIndividuals, value);
        //}

        public override void LockForm()
        {
            base.LockForm();

            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;

                if (control is AutoSuggestBox autoSuggestBox)
                {
                    if (autoSuggestBox.Name == "textBoxMessageTo")
                    {
                        if (FindName($"{autoSuggestBox.Name}TextBox") is TextBox autoSuggestBoxAsTextBox)
                        {
                            autoSuggestBoxAsTextBox.Text = FormPacketMessage.MessageTo;
                        }
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (FindName($"{comboBox.Name}TextBox") is TextBox comboBoxAsTextBox)
                    {
                        if (comboBox.Name == "comboBoxMessageBBS")
                        {
                            comboBoxAsTextBox.Text = FormPacketMessage.BBSName;
                        }
                        else if (comboBox.Name == "comboBoxMessageTNC")
                        {
                            comboBoxAsTextBox.Text = FormPacketMessage.TNCName;
                        }
                    }
                }
            }
        }

        //private static string ValidateSubject(string subject)
        //{
        //    if (string.IsNullOrEmpty(subject))
        //        return string.Empty;

        //    try
        //    {
        //        return Regex.Replace(subject, @"[^\w\.@-\\%/\-\ ,()]", "~",
        //                             RegexOptions.Singleline, TimeSpan.FromSeconds(1.0));
        //    }
        //    // If we timeout when replacing invalid characters, 
        //    // we should return Empty.
        //    catch (RegexMatchTimeoutException)
        //    {
        //        return string.Empty;
        //    }
        //}

        //private void MessageTo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    if (!toSelection.IsOn)
        //    {
        //        // Distribution list selected
        //        string[] listItems = DistributionListArray.Instance.GetDistributionListItems(args.SelectedItem.ToString());
        //        sender.Text = AddressBook.Instance.GetAddress(listItems[0]);
        //        for (int i = 1; i < listItems.Length; i++)
        //        {
        //            sender.Text += $", {AddressBook.Instance.GetAddress(listItems[i])}";
        //        }
        //    }
        //    else
        //    {
        //        sender.Text = AddressBook.Instance.GetAddress(args.SelectedItem.ToString());
        //    }
        //    SendFormDataControlViewModel.MessageTo = sender.Text;
        //}

        //private void MessageTo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (string.IsNullOrEmpty(sender.Text))
        //    {
        //        sender.ItemsSource = null;
        //        return;
        //    }

        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        //Set the ItemsSource to be your filtered dataset
        //        if (SendFormDataControlViewModel.IsToIndividuals)
        //            sender.ItemsSource = AddressBook.Instance.GetCallsigns(textBoxMessageTo.Text);
        //        else
        //            sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames(textBoxMessageTo.Text);
        //    }
        //    else
        //    {
        //        string messageTo = AddressBook.Instance.GetAddress(textBoxMessageTo.Text);
        //        sender.Text = messageTo ?? textBoxMessageTo.Text;
        //    }
        //}

    }
}
