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

        //private string _addressBookCallsign;
        public string AddressBookCallsign
        {
            get => addressBookCallsign.Text;
            set
            {
                addressBookCallsign.Text = value;//Set(ref _addressBookCallsign, value);
            }
        }

        //private string _addressBookName;
        public string AddressBookName
        {
            get => addressBookName.Text;
            set
            {
                addressBookName.Text = value;// Set(ref _addressBookName, value);
            }
        }

        //private string selectedPrimaryBBS;
        public string SelectedPrimaryBBS
        {
            get => addressBookPrimaryBBS.SelectedValue as string;
            set
            {
                addressBookPrimaryBBS.SelectedValue = value;
                //Set(ref selectedPrimaryBBS, value);
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
