using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace PacketMessagingTS.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : BaseViewModel
    {
        private int settingsPivotSelectedIndex;
        public int SettingsPivotSelectedIndex
        {
            get => GetProperty(ref settingsPivotSelectedIndex);
            set => SetProperty(ref settingsPivotSelectedIndex, value, true);
        }

        private ElementTheme _elementTheme = ThemeSelectorService.Theme;
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;
        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }

        //private ICommand _switchThemeCommand;
        //public ICommand SwitchThemeCommand
        //{
        //    get
        //    {
        //        if (_switchThemeCommand is null)
        //        {
        //            _switchThemeCommand = new RelayCommand<ElementTheme>(
        //                async (param) =>
        //                {
        //                    ElementTheme = param;
        //                    await ThemeSelectorService.SetThemeAsync(param);
        //                });
        //        }

        //        return _switchThemeCommand;
        //    }
        //}

        public SettingsViewModel()
        {
        }

        //public void Initialize()
        //{
        //    VersionDescription = GetVersionDescription();
        //}

        //private string GetVersionDescription()
        //{
        //    var appName = "AppDisplayName".GetLocalized();
        //    var package = Package.Current;
        //    var packageId = package.Id;
        //    var version = packageId.Version;

        //    return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        //}

        private bool w1XSCStatusUp = true;
        public bool W1XSCStatusUp
        {
            get => GetProperty(ref w1XSCStatusUp);
            set
            {
                if (SetProperty(ref w1XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W1XSC", w1XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool w2XSCStatusUp = true;
        public bool W2XSCStatusUp
        {
            get => GetProperty(ref w2XSCStatusUp);
            set
            {
                if (SetProperty(ref w2XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W2XSC", w2XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool w3XSCStatusUp = true;
        public bool W3XSCStatusUp
        {
            get => GetProperty(ref w3XSCStatusUp);
            set
            {
                if (SetProperty(ref w3XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W3XSC", W3XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool w4XSCStatusUp = true;
        public bool W4XSCStatusUp
        {
            get => GetProperty(ref w4XSCStatusUp);
            set
            {
                if (SetProperty(ref w4XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W4XSC", W4XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }
        public bool w5XSCStatusUp = true;
        public bool W5XSCStatusUp
        {
            get => GetProperty(ref w5XSCStatusUp);
            set
            {
                if (SetProperty(ref w5XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W5XSC", W5XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool IsBBSUp(string bbs)
        {
            bool statusUp = false;
            switch (bbs.ToLower())
            {
                case "w1xsc":
                    if (w1XSCStatusUp)
                        statusUp = true;
                    break;
                case "w2xsc":
                    if (w2XSCStatusUp)
                        statusUp = true;
                    break;
                case "w3xsc":
                    if (w3XSCStatusUp)
                        statusUp = true;
                    break;
                case "w4xsc":
                    if (w4XSCStatusUp)
                        statusUp = true;
                    break;
                case "w5xsc":
                    if (w5XSCStatusUp)
                        statusUp = true;
                    break;
            }
            return statusUp;
        }

        private string _selectedPrinter;
        public string SelectedPrinter
        {
            get => GetProperty(ref _selectedPrinter);
            set => SetProperty(ref _selectedPrinter, value, true);
        }

        private bool _printReceivedMessages;
        public bool PrintReceivedMessages
        {
            get => GetProperty(ref _printReceivedMessages);
            set => SetProperty(ref _printReceivedMessages, value, true);
        }

        private int _receivedCopyCount;
        public int ReceivedCopyCount
        {
            get => GetProperty(ref _receivedCopyCount);
            set
            {
                SetProperty(ref _receivedCopyCount, value, true);
            }
        }

        private string _selectedReceiveDest1;
        public string SelectedReceiveDest1
        {
            get => GetProperty(ref _selectedReceiveDest1);
            set => SetProperty(ref _selectedReceiveDest1, value, true);
        }

        private string _selectedReceiveDest2;
        public string SelectedReceiveDest2
        {
            get => GetProperty(ref _selectedReceiveDest2);
            set => SetProperty(ref _selectedReceiveDest2, value, true);
        }

        private string _selectedReceiveDest3;
        public string SelectedReceiveDest3
        {
            get => GetProperty(ref _selectedReceiveDest3);
            set => SetProperty(ref _selectedReceiveDest3, value, true);
        }

        private string _selectedReceiveDest4;
        public string SelectedReceiveDest4
        {
            get => GetProperty(ref _selectedReceiveDest4);
            set => SetProperty(ref _selectedReceiveDest4, value, true);
        }

        public string[] ReceivePrintDestinations
        {
            get
            {
                string[] receivePrintDestinations = new string[ReceivedCopyCount];
                for (int i = 0; i < ReceivedCopyCount; i++)
                {
                    if (i == 0)
                        receivePrintDestinations[i] = SelectedReceiveDest1;
                    if (i == 1)
                        receivePrintDestinations[i] = SelectedReceiveDest2;
                    if (i == 2)
                        receivePrintDestinations[i] = SelectedReceiveDest3;
                    if (i == 3)
                        receivePrintDestinations[i] = SelectedReceiveDest4;
                }

                return receivePrintDestinations;
            }
        }

        private bool _printSentMessages;
        public bool PrintSentMessages
        {
            get => GetProperty(ref _printSentMessages);
            set => SetProperty(ref _printSentMessages, value, true);
        }

        private int _sentCopyCount;
        public int SentCopyCount
        {
            get => GetProperty(ref _sentCopyCount);
            set => SetProperty(ref _sentCopyCount, value, true);
        }

        private string _selectedSentDest1;
        public string SelectedSentDest1
        {
            get => GetProperty(ref _selectedSentDest1);
            set => SetProperty(ref _selectedSentDest1, value, true);
        }

        private string _selectedSentDest2;
        public string SelectedSentDest2
        {
            get => GetProperty(ref _selectedSentDest2);
            set => SetProperty(ref _selectedSentDest2, value, true);
        }

        private string _selectedSentDest3;
        public string SelectedSentDest3
        {
            get => GetProperty(ref _selectedSentDest3);
            set => SetProperty(ref _selectedSentDest3, value, true);
        }

        private string _selectedSentDest4;
        public string SelectedSentDest4
        {
            get => GetProperty(ref _selectedSentDest4);
            set => SetProperty(ref _selectedSentDest4, value, true);
        }

        public string[] SentPrintDestinations
        {
            get
            {
                string[] sentPrintDestinations = new string[SentCopyCount];
                for (int i = 0; i < SentCopyCount; i++)
                {
                    if (i == 0)
                        sentPrintDestinations[i] = SelectedSentDest1;
                    if (i == 1)
                        sentPrintDestinations[i] = SelectedSentDest2;
                    if (i == 2)
                        sentPrintDestinations[i] = SelectedSentDest3;
                    if (i == 3)
                        sentPrintDestinations[i] = SelectedSentDest4;
                }
                return sentPrintDestinations;
            }
        }

        public string DataPath
        {
            get => ApplicationData.Current.LocalFolder.Path.ToString();
        }

    }
}
