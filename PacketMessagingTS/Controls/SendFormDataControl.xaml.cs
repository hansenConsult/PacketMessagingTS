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
using FormControlBaseClass;
using PacketMessagingTS.Models;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class SendFormDataControl : FormControlBasics
    {
        public SendFormDataControl()
        {
            this.InitializeComponent();
        }

        public string MessageSubject
        {
            get; internal set;
        }
        public string MessageBBS { get; internal set; }
        public string MessageTNC { get; internal set; }
        public string MessageFrom { get; internal set; }
        public string MessageTo { get; internal set; }

        private void MessageTo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AddressBook.Instance.UserBBS = messageBBS.Text;

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
        }

        private void MessageTo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            {
                // Only get results when it was a user typing, 
                // otherwise assume the value got filled in by TextMemberPath 
                // or the handler for SuggestionChosen.
                if (string.IsNullOrEmpty(messageTo.Text))
                {
                    sender.ItemsSource = null;
                    return;
                }

                if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    //Set the ItemsSource to be your filtered dataset
                    if (toSelection.IsOn)
                        sender.ItemsSource = AddressBook.Instance.GetCallsigns(messageTo.Text);
                    else
                        sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames(messageTo.Text);
                }
            }
        }

    }
}
