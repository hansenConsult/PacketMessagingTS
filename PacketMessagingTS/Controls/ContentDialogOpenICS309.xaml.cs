using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
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
    public sealed partial class ContentDialogOpenICS309 : ContentDialog
    {

        public ContentDialogOpenICS309()
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


        private int fileSelectedIndex = -1;
        public int FilesSelectedIndex
        {
            get => fileSelectedIndex;
            set
            {
                Set(ref fileSelectedIndex, value);
                if (fileSelectedIndex >= 0)
                {
                    IsPrimaryButtonEnabled = true;
                    IsSecondaryButtonEnabled = true;
                }
            }
        }

        private List<StorageFile> ics309Files = new List<StorageFile>();
        public List<StorageFile> ICS309Files
        {
            get => ics309Files;
            set
            {
                Set(ref ics309Files, value);
                ShowICS309Files = new ObservableCollection<StorageFile>(ics309Files);
            }
        }

        private ObservableCollection<StorageFile> showICS309Files = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFile> ShowICS309Files
        {
            get => showICS309Files;
            set => Set(ref showICS309Files, value);
        }

        public async Task<List<StorageFile>> GetFilesAsync(string extension)
        {
            List<string> fileTypeFilter = new List<string>() { ".xml" };
            fileTypeFilter.Add(".txt");
            fileTypeFilter.Add(".csv");
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the Outbox folder
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFileQueryResult results = localFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();

            List<StorageFile> ics309Files = new List<StorageFile>();
            foreach (StorageFile file in files)
            {
                if (file.Name.StartsWith("ICS309"))
                {
                    ics309Files.Add(file);
                }
            }
            return ics309Files;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async void FileType_Checked(object sender, RoutedEventArgs e)
        {
            //ICS309Files.Clear();
            switch ((sender as RadioButton).Name)
            {
                case "xmlFile":
                    ICS309Files = await GetFilesAsync(".xml");
                    break;
                case "txtFile":
                    ICS309Files = await GetFilesAsync(".txt");
                    break;
                case "csvFile":
                    ICS309Files = await GetFilesAsync(".csv");
                    break;
            }
        }

        private async void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            //ICS309Files = await GetFilesAsync(".xml");
        }
    }
}
