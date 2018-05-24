﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
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
                if (_switchThemeCommand == null)
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

        //private string firstMessageNumber = "100";
        //public string FirstMessageNumber
        //{
        //    get
        //    {
        //        //bool found = App.Properties.TryGetValue("MessageNumber", out object first);
        //        //if (!found)
        //        //{
        //        //    App.Properties["MessageNumber"] = firstMessageNumber;
        //        //}
        //        //firstMessageNumber = Convert.ToInt32(first);
        //        return firstMessageNumber;
        //    }
        //    set
        //    {
        //        //Utilities.MarkMessageNumberAsUsed(firstMessageNumber);
        //        SetProperty(ref firstMessageNumber, value);
        //    }
        //}

        private int firstMessageNumber;
        public int FirstMessageNumber
        {
            get
            {
                bool found = App.Properties.TryGetValue("MessageNumber", out object first);
                if (!found)
                {
                    App.Properties["MessageNumber"] = 100;
                }
                firstMessageNumber = Convert.ToInt32(App.Properties["MessageNumber"]);
                return firstMessageNumber;
            }
            set
            {
                Utilities.MarkMessageNumberAsUsed(value);
                SetProperty(ref firstMessageNumber, value);
            }
        }

        private bool sendReceipt;
        public bool SendReceipt
        {
            get => GetProperty(ref sendReceipt);
            set => SetProperty(ref sendReceipt, value, true);
        }
    }
}
