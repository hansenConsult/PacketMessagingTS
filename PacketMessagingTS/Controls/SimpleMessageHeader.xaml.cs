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

namespace PacketMessagingTS.Controls
{
    public sealed partial class SimpleMessageHeader : UserControl
    {
        public SimpleMessageHeader()
        {
            this.InitializeComponent();
        }

        private string messageReceivedTime;
        public string MessageReceivedTime
        {
            get => messageReceivedTime;
            set => Set(ref messageReceivedTime, value);
        }

        private string messageSentTime;
        public string MessageSentTime
        {
            get => messageSentTime;
            set => Set(ref messageSentTime, value);
        }

        private string messageNumber;
        public string MessageNumber
        {
            get => messageNumber;
            set => Set(ref messageNumber, value);
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

    }
}
