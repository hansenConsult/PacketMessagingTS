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
using System.ComponentModel;
using System.Runtime.CompilerServices;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PacketMessagingTS.Controls
{
    public sealed partial class SendFormDataControl : FormControlBasics
    {
        public SendFormDataControl()
        {
            this.InitializeComponent();

            ScanControls(messageInfo);

            MessageBBS = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.BBS;
            MessageTNC = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.TNC;
            MessageTo = Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.SendTo;
            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                MessageFrom = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
            }
            else
            {
                MessageFrom = Singleton<IdentityViewModel>.Instance.UserCallsign;
            }

        }

        private string messageSubject;
        public string MessageSubject
        {
            get => messageSubject;
            //set => Set(ref messageSubject, value);
            set
            {
                messageSubject = value;
                textBoxMessageSubject.Text = value ?? "";  // Ned to use invoke???
            }
        }

        private string messageBBS;
        public string MessageBBS
        {
            get => messageBBS;
            set => Set(ref messageBBS, value);
        }

        private string messageTNC;
        public string MessageTNC
        {
            get => messageTNC;
            set => Set(ref messageTNC, value);
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

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private void MessageTo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AddressBook.Instance.UserBBS = MessageBBS;

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
            {
                // Only get results when it was a user typing, 
                // otherwise assume the value got filled in by TextMemberPath 
                // or the handler for SuggestionChosen.
                if (string.IsNullOrEmpty(textBoxMessageTo.Text))
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
            }
        }

    }
}
