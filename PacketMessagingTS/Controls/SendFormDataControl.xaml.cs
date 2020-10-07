using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FormControlBaseClass;
using FormControlBasicsNamespace;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

//using SharedCode.Helpers;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class SendFormDataControl : FormControlBasics
    {
        public static readonly DependencyProperty MessageSubjectProperty =
                DependencyProperty.Register(
                "MessageSubject",
                typeof(string),
                typeof(SendFormDataControl),
                null);

        public SendFormDataControl()
        {
            InitializeComponent();

            ScanControls(messageInfo);

            //(string bbs, string tnc, string from) = Utilities.GetProfileData();
            (string bbs, string tnc, string from) = Utilities.GetProfileDataBBSStatusChecked();
            MessageBBS = bbs; //Utilities.GetBBSName(out string from, out string tnc);
            //AddressBook.Instance.UserBBS = MessageBBS;
            MessageTNC = tnc;
            MessageTo = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.SendTo;
            MessageFrom = from;
        }

        public override FormControlBasics RootPanel => rootPanel;

        private string messageSubject;
        public string MessageSubject
        {
            get => messageSubject;
            //set => Set(ref messageSubject, value);
            set
            {
                string validatedSubject = ValidateSubject(value);
                Set(ref messageSubject, validatedSubject ?? "");
            }
        }

        //public string MessageSubject
        //{
        //    get { return (string)GetValue(MessageSubjectProperty); }
        //    set { SetValue(MessageSubjectProperty, value ?? ""); }
        //}

        public ObservableCollection<BBSData> BBSArray => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataArray);

        private string originalBBS;

        private string messageBBS;
        public string MessageBBS
        {
            get => messageBBS;
            set
            {
                Set(ref messageBBS, value);
                //Utilities.SetApplicationTitle(messageBBS);
            }
        }

        public ObservableCollection<TNCDevice> DeviceList => new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);

        private string messageTNC;
        public string MessageTNC
        {
            get => messageTNC;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                Set(ref messageTNC, value);

                if (messageTNC.Contains(SharedData.EMail))
                {
                    if (!string.IsNullOrEmpty(MessageBBS))
                    {
                        originalBBS = MessageBBS;
                    }
                    MessageBBS = comboBoxMessageBBS.Text = "";    // TODO fix this
                }
                else
                {
                    if (!string.IsNullOrEmpty(originalBBS))
                    {
                        MessageBBS = comboBoxMessageBBS.Text = originalBBS;
                    }
                }
            }
        }

        private string messageFrom;
        public string MessageFrom
        {
            get => messageFrom;
            set => Set(ref messageFrom, value);
        }

        private string messageTo;
        public string MessageTo
        {
            get => messageTo;
            set => Set(ref messageTo, value);
        }

        private bool isToIndividuals = true;
        public bool IsToIndividuals
        {
            get => isToIndividuals;
            set => Set(ref isToIndividuals, value);
        }

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
                        TextBox autoSuggestBoxAsTextBox = FindName($"{autoSuggestBox.Name}TextBox") as TextBox;
                        if (autoSuggestBoxAsTextBox != null)
                        {
                            autoSuggestBoxAsTextBox.Text = FormPacketMessage.MessageTo;
                        }
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    TextBox comboBoxAsTextBox = FindName($"{comboBox.Name}TextBox") as TextBox;
                    if (comboBoxAsTextBox != null)
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

        private static string ValidateSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return string.Empty;

            try
            {
                return Regex.Replace(subject, @"[^\w\.@-\\%/\-\ ,()]", "~",
                                     RegexOptions.Singleline, TimeSpan.FromSeconds(1.0));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }

        private void MessageTo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!toSelection.IsOn)
            {
                // Distribution list selected
                string[] listItems = DistributionListArray.Instance.GetDistributionListItems(args.SelectedItem.ToString());
                sender.Text = AddressBook.Instance.GetAddress(listItems[0]);
                for (int i = 1; i < listItems.Length; i++)
                {
                    sender.Text += $", {AddressBook.Instance.GetAddress(listItems[i])}";
                }
            }
            else
            {
                sender.Text = AddressBook.Instance.GetAddress(args.SelectedItem.ToString());
            }
            MessageTo = sender.Text;
        }

        private void MessageTo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (string.IsNullOrEmpty(sender.Text))
            {
                sender.ItemsSource = null;
                return;
            }

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                if (IsToIndividuals)
                    sender.ItemsSource = AddressBook.Instance.GetCallsigns(textBoxMessageTo.Text);
                else
                    sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames(textBoxMessageTo.Text);
            }
            else
            {
                string messageTo = AddressBook.Instance.GetAddress(textBoxMessageTo.Text);
                sender.Text = messageTo ?? textBoxMessageTo.Text;
            }
        }

    }
}
