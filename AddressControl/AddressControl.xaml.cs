using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using FormControlBasicsNamespace;

using SharedCode.Helpers;

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

namespace AddressControl
{
    public sealed partial class AddressControl : FormControlBasics
    {
        AddressControlViewModel AddressControlViewModel = AddressControlViewModel.Instance;


        public AddressControl()
        {
            this.InitializeComponent();

            ScanControls(messageInfo);

            (string bbs, string tnc, string from) = Utilities.GetProfileDataBBSStatusChecked();
            MessageBBS = bbs; //Utilities.GetBBSName(out string from, out string tnc);
            //AddressBook.Instance.UserBBS = MessageBBS;
            MessageTNC = tnc;
            MessageTo = PacketSettingsViewModel.Instance.CurrentProfile.SendTo;
            MessageFrom = from;
        }

        public override FormControlBasics RootPanel => rootPanel;


        public ObservableCollection<BBSData> BBSArray => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataArray);

        private string originalBBS;

        private string messageBBS;
        public string MessageBBS
        {
            get => messageBBS;
            set => SetProperty(ref messageBBS, value);
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

                SetProperty(ref messageTNC, value);

                if (messageTNC.Contains(PublicData.EMail))
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
