using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Controls
{
    public sealed partial class ContentDialogAddressBookEntry : ContentDialog
    {
        public static readonly DependencyProperty EmailNotEnteredProperty =
            DependencyProperty.Register(
                "EmailNotEntered",
                typeof(bool),
                typeof(ContentDialogAddressBookEntry),
                null);

        public ContentDialogAddressBookEntry()
        {
            InitializeComponent();

            EmailNotEntered = true;
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

        private string addressBookCallsign;
        public string AddressBookCallsign
        {
            get => addressBookCallsign;
            set
            {
                Set(ref addressBookCallsign, value);
                if (addressBookCallsign.Contains("@"))
                {
                    EmailNotEntered = false;
                }
                else
                {
                    EmailNotEntered = true;
                    if (!string.IsNullOrEmpty(addressBookCallsign) && addressBookCallsign.Length > 3)
                    {
                        textBoxPrefix.Text = addressBookCallsign.Substring(addressBookCallsign.Length - 3, 3);
                    }
                }
            }
        }

        private string addressBookName;
        public string AddressBookName
        {
            get => addressBookName;
            set
            {
                Set(ref addressBookName, value);
            }
        }

        private string addressBookCity;
        public string AddressBookCity
        {
            get => addressBookCity;
            set => Set(ref addressBookCity, value);
        }

        private string addressBookPrefix;
        public string AddressBookPrefix
        {
            get => addressBookPrefix;
            set => Set(ref addressBookPrefix, value);
        }

        public bool EmailNotEntered
        {
            get { return (bool)GetValue(EmailNotEnteredProperty); }
            set { SetValue(EmailNotEnteredProperty, value); }
        }
        //private bool emailNotEntered = true;
        //public bool EmailNotEntered
        //{
        //    get => emailNotEntered;
        //    set
        //    {
        //        Set(ref emailNotEntered, value);
        //        textBoxPrefix.IsEnabled = emailNotEntered;
        //        addressBookPrimaryBBS.IsEnabled = emailNotEntered;
        //        addressBookSecondaryBBS.IsEnabled = emailNotEntered;
        //    }
        //}

        private string selectedPrimaryBBS;
        public string SelectedPrimaryBBS
        {
            get => selectedPrimaryBBS;
            set => Set(ref selectedPrimaryBBS, value);
        }

        private string selectedSecondaryBBS;
        public string SelectedSecondaryBBS
        {
            get => selectedSecondaryBBS;
            set => Set(ref selectedSecondaryBBS, value);
        }

        private string selectedLastUsedBBS;
        public string SelectedLastUsedBBS
        {
            get => selectedLastUsedBBS;
            set => Set(ref selectedLastUsedBBS, value);
        }

        private DateTime lastUsedBBSDate;
        public DateTime LastUsedBBSDate
        {
            get => lastUsedBBSDate;
            set => Set(ref lastUsedBBSDate, value);
        }
    }
}
