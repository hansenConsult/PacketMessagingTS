using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        //private PivotItem settingsPivotSelectedItem;
        //public PivotItem SettingsPivotSelectedItem
        //{
        //    //get => settingsPivotSelectedItem;
        //    get
        //    {
        //        switch (settingsPivotSelectedItem.Name)
        //        {
        //            case "pivotSettings":
        //            break;
        //        }
        //        return settingsPivotSelectedItem;
        //    }
        //set => SetProperty(ref settingsPivotSelectedItem, value);
        //}

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
        public bool w5XSCStatusUp = false;
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
                if (value > 9)
                    value = 9;
                SetProperty(ref _receivedCopyCount, value, true);
                ReceivedCopyNames = ReceivedCopyNamesArray[_receivedCopyCount];
            }
        }

        private string[] receivedCopyNamesArray;
        public string[] ReceivedCopyNamesArray
        {
            get
            {
                if (receivedCopyNamesArray is null)
                {
                    GetProperty(ref receivedCopyNamesArray);
                    if (receivedCopyNamesArray is null)
                    {
                        receivedCopyNamesArray = new string[10];
                        for (int i = 0; i < receivedCopyNamesArray.Length; i++)
                        {
                            receivedCopyNamesArray[i] = "";
                        }
                    }
                }
                return receivedCopyNamesArray;
            }
            set => SetProperty(ref receivedCopyNamesArray, value, true);
        }

        private string receivedCopyNames;
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
                    receivedCopyNames = value;  // To make sure an update happens
                }
                string[] copyNamesArray = new string[10];
                ReceivedCopyNamesArray.CopyTo(copyNamesArray, 0);
                copyNamesArray[ReceivedCopyCount] = newValue;
                ReceivedCopyNamesArray = copyNamesArray;

                SetProperty(ref receivedCopyNames, newValue);
            }
        }

        public string[] ReceivedCopyNamesAsArray() => ReceivedCopyNames.Split(new char[] { '\r', '\n' });

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
            set
            {
                if (value > 9)
                    value = 9;
                SetProperty(ref _sentCopyCount, value, true);
            }
        }

        private string[] sentCopyNamesArray;
        public string[] SentCopyNamesArray
        {
            get
            {
                if (sentCopyNamesArray is null)
                {
                    GetProperty(ref sentCopyNamesArray);
                    if (sentCopyNamesArray is null)
                    {
                        sentCopyNamesArray = new string[10];
                        for (int i = 0; i < sentCopyNamesArray.Length; i++)
                        {
                            sentCopyNamesArray[i] = "";
                        }
                    }
                }
                return sentCopyNamesArray;
            }
            set => SetProperty(ref sentCopyNamesArray, value, true);
        }

        private string sentCopyNames;
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
                    sentCopyNames = value;  // To make sure an update happens
                }
                string[] copyNamesArray = new string[10];
                SentCopyNamesArray.CopyTo(copyNamesArray, 0);
                copyNamesArray[SentCopyCount] = newValue;
                SentCopyNamesArray = copyNamesArray;

                SetProperty(ref sentCopyNames, newValue);
            }
        }

        public string[] SentCopyNamesAsArray() => SentCopyNames.Split(new char[] { '\r', '\n' });

        public string DataPath
        {
            get => ApplicationData.Current.LocalFolder.Path.ToString();
        }

    }
}
