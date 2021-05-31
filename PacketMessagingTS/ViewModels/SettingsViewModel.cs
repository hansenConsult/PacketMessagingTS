using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;

using SharedCode.Models;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace PacketMessagingTS.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : ViewModelBase
    {
        public static SettingsViewModel Instance { get; } = new SettingsViewModel();

        private int _settingsPivotSelectedIndex;
        public int SettingsPivotSelectedIndex
        {
            get => GetProperty(ref _settingsPivotSelectedIndex);
            set => SetPropertyPrivate(ref _settingsPivotSelectedIndex, value, true);
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

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private bool _w1XSCStatusUp = true;
        public bool W1XSCStatusUp
        {
            get => GetProperty(ref _w1XSCStatusUp);
            set
            {
                if (SetPropertyPrivate(ref _w1XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W1XSC", _w1XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool _w2XSCStatusUp = true;
        public bool W2XSCStatusUp
        {
            get => GetProperty(ref _w2XSCStatusUp);
            set
            {
                if (SetPropertyPrivate(ref _w2XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W2XSC", _w2XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool _w3XSCStatusUp = true;
        public bool W3XSCStatusUp
        {
            get => GetProperty(ref _w3XSCStatusUp);
            set
            {
                if (SetPropertyPrivate(ref _w3XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W3XSC", W3XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool _w4XSCStatusUp = true;
        public bool W4XSCStatusUp
        {
            get => GetProperty(ref _w4XSCStatusUp);
            set
            {
                if (SetPropertyPrivate(ref _w4XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W4XSC", W4XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }
        public bool _w5XSCStatusUp = false;
        public bool W5XSCStatusUp
        {
            get => GetProperty(ref _w5XSCStatusUp);
            set
            {
                if (SetPropertyPrivate(ref _w5XSCStatusUp, value, true))
                {
                    AddressBook.Instance.UpdateForBBSStatusChange("W5XSC", W5XSCStatusUp);
                    Utilities.SetApplicationTitle();
                }
            }
        }

        public bool? IsBBSUp(string bbs)
        {
            bool? statusUp = null;
            if (!string.IsNullOrEmpty(bbs))
            {
                statusUp = false;
                switch (bbs.ToLower())
                {
                    case "w1xsc":
                        if (_w1XSCStatusUp)
                            statusUp = true;
                        break;
                    case "w2xsc":
                        if (_w2XSCStatusUp)
                            statusUp = true;
                        break;
                    case "w3xsc":
                        if (_w3XSCStatusUp)
                            statusUp = true;
                        break;
                    case "w4xsc":
                        if (_w4XSCStatusUp)
                            statusUp = true;
                        break;
                    case "w5xsc":
                        if (_w5XSCStatusUp)
                            statusUp = true;
                        break;
                }
            }
            return statusUp;
        }

        private string _selectedPrinter;
        public string SelectedPrinter
        {
            get => GetProperty(ref _selectedPrinter);
            set => SetPropertyPrivate(ref _selectedPrinter, value, true);
        }

        private bool _printReceivedMessages;
        public bool PrintReceivedMessages
        {
            get => GetProperty(ref _printReceivedMessages);
            set => SetPropertyPrivate(ref _printReceivedMessages, value, true);
        }

        private int _receivedCopyCount;
        public int ReceivedCopyCount
        {
            get => GetProperty(ref _receivedCopyCount);
            set
            {
                if (value > 9)
                    value = 9;
                SetPropertyPrivate(ref _receivedCopyCount, value, true);
                ReceivedCopyNames = ReceivedCopyNamesArray[_receivedCopyCount];
            }
        }

        private string[] _receivedCopyNamesArray;
        public string[] ReceivedCopyNamesArray
        {
            get
            {
                if (_receivedCopyNamesArray is null)
                {
                    GetProperty(ref _receivedCopyNamesArray);
                    if (_receivedCopyNamesArray is null)
                    {
                        _receivedCopyNamesArray = new string[10];
                        for (int i = 0; i < _receivedCopyNamesArray.Length; i++)
                        {
                            _receivedCopyNamesArray[i] = "";
                        }
                    }
                }
                return _receivedCopyNamesArray;
            }
            set => SetPropertyPrivate(ref _receivedCopyNamesArray, value, true);
        }

        private string _receivedCopyNames;
        public string ReceivedCopyNames
        {
            get => ReceivedCopyNamesArray[ReceivedCopyCount];
            set
            {
                string[] copynames = value.Split(new char[] { '\r', '\n' });
                int excessCopyNames = copynames.Length - ReceivedCopyCount;
                string newValue = value;
                if (excessCopyNames > 0)
                {
                    int lastIndex;
                    for (int i = 0; i < excessCopyNames; i++)
                    {
                        lastIndex = newValue.LastIndexOf('\r');
                        if (lastIndex < 0)
                            break;
                        newValue = newValue.Substring(0, lastIndex);
                    }
                    _receivedCopyNames = value;  // To make sure an update happens
                }
                string[] copyNamesArray = new string[10];
                ReceivedCopyNamesArray.CopyTo(copyNamesArray, 0);
                copyNamesArray[ReceivedCopyCount] = newValue;
                ReceivedCopyNamesArray = copyNamesArray;

                SetProperty(ref _receivedCopyNames, newValue);
            }
        }

        public string[] ReceivedCopyNamesAsArray() => ReceivedCopyNames.Split(new char[] { '\r', '\n' });

        private bool _printSentMessages;
        public bool PrintSentMessages
        {
            get => GetProperty(ref _printSentMessages);
            set => SetPropertyPrivate(ref _printSentMessages, value, true);
        }

        private int _sentCopyCount;
        public int SentCopyCount
        {
            get => GetProperty(ref _sentCopyCount);
            set
            {
                if (value > 9)
                    value = 9;
                SetPropertyPrivate(ref _sentCopyCount, value, true);
            }
        }

        private string[] _sentCopyNamesArray;
        public string[] SentCopyNamesArray
        {
            get
            {
                if (_sentCopyNamesArray is null)
                {
                    GetProperty(ref _sentCopyNamesArray);
                    if (_sentCopyNamesArray is null)
                    {
                        _sentCopyNamesArray = new string[10];
                        for (int i = 0; i < _sentCopyNamesArray.Length; i++)
                        {
                            _sentCopyNamesArray[i] = "";
                        }
                    }
                }
                return _sentCopyNamesArray;
            }
            set => SetPropertyPrivate(ref _sentCopyNamesArray, value, true);
        }

        private string _sentCopyNames;
        public string SentCopyNames
        {
            get => SentCopyNamesArray[SentCopyCount];
            set
            {
                string[] copynames = value.Split(new char[] { '\r', '\n' });
                int excessCopyNames = copynames.Length - ReceivedCopyCount;
                string newValue = value;
                if (excessCopyNames > 0)
                {
                    int lastIndex;
                    for (int i = 0; i < excessCopyNames; i++)
                    {
                        lastIndex = newValue.LastIndexOf('\r');
                        newValue = newValue.Substring(0, lastIndex);
                    }
                    _sentCopyNames = value;  // To make sure an update happens
                }
                string[] copyNamesArray = new string[10];
                SentCopyNamesArray.CopyTo(copyNamesArray, 0);
                copyNamesArray[SentCopyCount] = newValue;
                SentCopyNamesArray = copyNamesArray;

                SetProperty(ref _sentCopyNames, newValue);
            }
        }

        public string[] SentCopyNamesAsArray() => SentCopyNames.Split(new char[] { '\r', '\n' });

        public string DataPath
        {
            get => ApplicationData.Current.LocalFolder.Path.ToString();
        }

    }
}
