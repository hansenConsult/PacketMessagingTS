using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Views;

using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private IList<KeyboardAccelerator> _keyboardAccelerators;
        private NavigationView _navigationView;
        private NavigationViewItem _selected;
        private ICommand _loadedCommand;
        private ICommand _itemInvokedCommand;

        public NavigationViewItem Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ShellViewModel()
        {
        }

        public void Initialize(Frame frame, NavigationView navigationView, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            _navigationView = navigationView;
            _keyboardAccelerators = keyboardAccelerators;
            NavigationService.Frame = frame;
            NavigationService.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;
        }

        private void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavigationService.Navigate(typeof(SettingsPage));
                return;
            }

            var item = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);
            var pageType = item.GetValue(NavHelper.NavigateToProperty) as Type;
            NavigationService.Navigate(pageType);
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.GoBack();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = _navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            Selected = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private ObservableCollection<string> collectionOfSerialDevices;
        public ObservableCollection<string> CollectionOfSerialDevices
        {
            get => collectionOfSerialDevices;
            set => collectionOfSerialDevices = value;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            KeyboardAccelerator keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            bool result = NavigationService.GoBack();
            args.Handled = result;
        }

    }
}
