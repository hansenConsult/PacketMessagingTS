using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;


namespace PacketMessagingTS.Controls
{
    public sealed partial class ContentDialogAddressBookEntry : ContentDialog
    {
        public ContentDialogAddressBookEntry()
        {
            this.InitializeComponent();
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
                //addressBookCallsign.Text = value;
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
                        AddressBookPrefix = addressBookCallsign.Substring(addressBookCallsign.Length - 3, 3);
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
                //addressBookName.Text = value;
                Set(ref addressBookName, value);
            }
        }

        //private string _addressBookCity;
        public string AddressBookCity
        {
            get => addressBookCity.Text;
            set
            {
                addressBookCity.Text = value;// Set(ref _addressBookName, value);
            }
        }

        private string addressBookPrefix;
        public string AddressBookPrefix
        {
            get => addressBookPrefix;
            set
            {
                //addressBookPrefix.Text = value;
                Set(ref addressBookPrefix, value);
            }
        }

        private bool emailNotEntered = true;
        public bool EmailNotEntered
        {
            get => emailNotEntered;
            set
            {
                Set(ref emailNotEntered, value);
                textBoxPrefix.IsEnabled = emailNotEntered;
                addressBookPrimaryBBS.IsEnabled = emailNotEntered;
                addressBookSecondaryBBS.IsEnabled = emailNotEntered;
            }
        }

        private string selectedPrimaryBBS;
        public string SelectedPrimaryBBS
        {
            //get => addressBookPrimaryBBS.SelectedValue as string;
            get => selectedPrimaryBBS;
            set
            {
                //addressBookPrimaryBBS.SelectedValue = value;
                Set(ref selectedPrimaryBBS, value);
            }
        }

        //private string selectedSecondaryBBS;
        public string SelectedSecondaryBBS
        {
            get => addressBookSecondaryBBS.SelectedValue as string; //selectedSecondaryBBS;
            set
            {
                addressBookSecondaryBBS.SelectedValue = value;//Set(ref selectedSecondaryBBS, value);
            }
        }
    }
}
