using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

using FormControlBaseClass;

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

namespace PacketMessagingTS.Controls
{
    public sealed partial class RoutingSlip : FormControlBasics
    {
        public string[] ICSPosition = new string[] {
                "Incident Commander",
                "Operations",
                "Planning",
                "Logistics",
                "Finance",
                "Public Info. Officer",
                "Liaison Officer",
                "Safety Officer"
        };

        public RoutingSlip()
        {
            this.InitializeComponent();
        }

        private string formName;
        public string FormName
        {
            get;
            set;
        }

        private string originMsgNumber = "MSG-12345P";
        public string OriginMsgNumber
        {
            get;
            set;
        }

        private string destinationMsgNumber;
        public string DestinationMsgNumber
        {
            get;
            set;
        }

        private string tacticalCallsign;
        public string CERTLocationValue
        {
            get => tacticalCallsign;
            set => Set(ref tacticalCallsign, value);
        }

        public string TacticalCallsign
        {
            get => CERTLocationValue;
            set => CERTLocationValue = value;
        }


        private string toICSPosition;
        public string ToICSPosition
        {
            get => toICSPosition;
            set => Set(ref toICSPosition, value);
        }

        private string toLocation;
        public string ToLocation
        {
            get => toLocation;
            set => Set(ref toLocation, value);
        }

        private string fromLocation;
        public string FromLocation
        {
            get => fromLocation;
            set => Set(ref fromLocation, value);
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

        //private void ComboBoxFromLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count < 1)
        //    {
        //        return;
        //    }

        //    if ((sender as ComboBox).Name == "comboBoxFromLocation")
        //    {
        //        if (comboBoxFromLocation.SelectedIndex < 0 && comboBoxFromLocation.IsEditable)
        //        {
        //            textBoxFromLocation.Text = comboBoxFromLocation.Text;
        //            FromLocation = comboBoxFromLocation.Text;
        //        }
        //        else
        //        {
        //            textBoxFromLocation.Text = comboBoxFromLocation.SelectedItem.ToString();
        //        }
        //    }
        //    ComboBoxRequired_SelectionChanged(sender, e);
        //}

        private void ICSPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).Name == "comboBoxToICSPosition")
            {
                if (comboBoxToICSPosition.SelectedIndex < 0 && comboBoxToICSPosition.IsEditable)
                {
                    textBoxToICSPosition.Text = comboBoxToICSPosition.Text;
                }
                else
                {
                    textBoxToICSPosition.Text = comboBoxToICSPosition.SelectedItem.ToString();
                }
            }
            else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
            {
                if (comboBoxFromICSPosition.SelectedIndex < 0 && comboBoxFromICSPosition.IsEditable)
                {
                    textBoxFromICSPosition.Text = comboBoxFromICSPosition.Text;
                }
                else
                {
                    textBoxFromICSPosition.Text = comboBoxFromICSPosition.SelectedItem.ToString();
                }
            }
            ComboBoxRequired_SelectionChanged(sender, e);
        }

    }
}
