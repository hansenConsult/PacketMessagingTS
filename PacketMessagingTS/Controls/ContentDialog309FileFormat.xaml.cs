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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Controls
{
    public sealed partial class ContentDialog309FileFormat : ContentDialog
    {
        public ContentDialog309FileFormat()
        {
            this.InitializeComponent();

            XmlFormat = true;
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


        private bool _xmlFormat;
        public bool XmlFormat
        {
            get => _xmlFormat;
            set
            {
                Set(ref _xmlFormat, value);
            }
        }

        private bool _txtFormat;
        public bool TxtFormat
        {
            get => _txtFormat;
            set
            {
                Set(ref _txtFormat, value);
            }
        }

        private bool _csvFormat;
        public bool CsvFormat
        {
            get => _csvFormat;
            set
            {
                Set(ref _csvFormat, value);
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void TextBlock_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }
    }
}
