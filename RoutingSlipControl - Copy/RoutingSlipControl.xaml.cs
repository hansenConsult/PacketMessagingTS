using FormControlBaseClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RoutingSlipControl
{

    public sealed partial class RoutingSlipControl : FormControlBasics
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

        public RoutingSlipControl()
        {
            this.InitializeComponent();
        }

        private string formName;
        public string FormName
        {
            get => formName;
            set => formName = value; //Set(ref formName, value);
        }

        private string originMsgNumber;
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

        private string toICSPosition;
        public string ToICSPosition
        {
            get => toICSPosition;
            set => formName = value; //Set(ref toICSPosition, value);
        }

        private string fromICSPosition;
        public string FromICSPosition
        {
            get => fromICSPosition;
            set => formName = value;//Set(ref fromICSPosition, value);
        }

        private string toLocation;
            public string ToLocation
            {
                get => toLocation;
                set => formName = value; //Set(ref toLocation, value);
            }

            private string fromLocation;
            public string FromLocation
            {
                get => fromLocation;
                set => formName = value; //Set(ref fromLocation, value);
            }

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
                //ComboBoxRequired_SelectionChanged(sender, e);
            }

        }
    }

