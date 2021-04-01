using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using PacketMessagingTS.Models;

using SharedCode.Helpers;
using SharedCode.Models;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Controls
{
    class SendFormDataControlViewModel : UserControlViewModelBase
    {
        public static SendFormDataControlViewModel Instance { get; } = new SendFormDataControlViewModel();

        public ObservableCollection<BBSData> BBSArray => new ObservableCollection<BBSData>(BBSDefinitions.Instance.BBSDataArray);
        public ObservableCollection<TNCDevice> DeviceList => new ObservableCollection<TNCDevice>(TNCDeviceArray.Instance.TNCDeviceList);


        private string messageBBS;
        public string MessageBBS
        {
            get => messageBBS;
            set => SetProperty(ref messageBBS, value);
        }

        private string originalBBS;

        private string messageTNC = "";
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
                    MessageBBS = "";
                }
                else
                {
                    if (!string.IsNullOrEmpty(originalBBS))
                    {
                        MessageBBS = originalBBS;
                    }
                }
            }
        }

        private string messageFrom;
        public string MessageFrom
        {
            get => messageFrom;
            set => SetProperty(ref messageFrom, value);
        }

        private string messageTo;
        public string MessageTo
        {
            get => messageTo;
            set => SetProperty(ref messageTo, value);
        }

        private bool isToIndividuals = true;
        public bool IsToIndividuals
        {
            get => isToIndividuals;
            set => SetProperty(ref isToIndividuals, value);
        }

        private string messageSubject;
        public string MessageSubject
        {
            get => messageSubject;
            set
            {
                string validatedSubject = ValidateSubject(value);
                SetProperty(ref messageSubject, validatedSubject);
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

        public void MessageTo_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (IsToIndividuals)
            {
                sender.Text = AddressBook.Instance.GetAddress(args.SelectedItem.ToString());
            }
            else
            {
                // Distribution list selected
                string[] listItems = DistributionListArray.Instance.GetDistributionListItems(args.SelectedItem.ToString());
                sender.Text = AddressBook.Instance.GetAddress(listItems[0]);
                for (int i = 1; i < listItems.Length; i++)
                {
                    sender.Text += $", {AddressBook.Instance.GetAddress(listItems[i])}";
                }
            }
            MessageTo = sender.Text;
        }

        public void MessageTo_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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
                    sender.ItemsSource = AddressBook.Instance.GetCallsigns(MessageTo);
                else
                    sender.ItemsSource = DistributionListArray.Instance.GetDistributionListNames(MessageTo);
            }
            else
            {
                string messageTo = AddressBook.Instance.GetAddress(MessageTo);
                sender.Text = messageTo ?? MessageTo;
            }
        }

    }
}
