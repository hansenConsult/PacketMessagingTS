using System;
using System.Threading.Tasks;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace PacketMessagingTS.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : BaseViewModel
    {
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

        private ICommand _switchThemeCommand;
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand is null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        public SettingsViewModel()
        {
        }

        public void Initialize()
        {
            VersionDescription = GetVersionDescription();
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
            get
            {
                GetProperty(ref w1XSCStatusUp);
                //AddressBook.Instance.UpdateForBBSStatusChange("W1XSC", w1XSCStatusUp);
                return w1XSCStatusUp;
            }
            set
            {
                SetProperty(ref w1XSCStatusUp, value, true);
                AddressBook.Instance.UpdateForBBSStatusChange("W1XSC", w1XSCStatusUp);
            }
        }

        public bool w2XSCStatusUp = true;
        public bool W2XSCStatusUp
        {
            get
            {
                GetProperty(ref w2XSCStatusUp);
                //AddressBook.Instance.UpdateForBBSStatusChange("W2XSC", temp);
                return w2XSCStatusUp;
            }
            set
            {
                SetProperty(ref w2XSCStatusUp, value, true);
                AddressBook.Instance.UpdateForBBSStatusChange("W2XSC", w2XSCStatusUp);
            }
        }

        public bool w3XSCStatusUp = true;
        public bool W3XSCStatusUp
        {
            get
            {
                GetProperty(ref w3XSCStatusUp);
                //AddressBook.Instance.UpdateForBBSStatusChange("W3XSC", temp);
                return w3XSCStatusUp;
            }
            set
            {
                SetProperty(ref w3XSCStatusUp, value, true);
                AddressBook.Instance.UpdateForBBSStatusChange("W3XSC", W3XSCStatusUp);
            }
        }

        public bool w4XSCStatusUp = true;
        public bool W4XSCStatusUp
        {
            get
            {
                GetProperty(ref w4XSCStatusUp);
                //AddressBook.Instance.UpdateForBBSStatusChange("W4XSC", temp);
                return w4XSCStatusUp;
            }
            set
            {
                SetProperty(ref w4XSCStatusUp, value, true);
                AddressBook.Instance.UpdateForBBSStatusChange("W4XSC", W4XSCStatusUp);
            }
        }
        public bool w5XSCStatusUp = true;
        public bool W5XSCStatusUp
        {
            get
            {
                GetProperty(ref w5XSCStatusUp);
                //AddressBook.Instance.UpdateForBBSStatusChange("W5XSC", temp);
                return w5XSCStatusUp;
            }
            set
            {
                SetProperty(ref w5XSCStatusUp, value, true);
                AddressBook.Instance.UpdateForBBSStatusChange("W5XSC", W5XSCStatusUp);
            }
        }

    }
}
