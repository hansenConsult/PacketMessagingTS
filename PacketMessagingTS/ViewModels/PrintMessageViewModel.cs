using System;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using Windows.UI.Core;

namespace PacketMessagingTS.ViewModels
{
    public class PrintMessageViewModel : FormsViewModel
    {
        public static PrintMessageViewModel Instance { get; } = new PrintMessageViewModel();

        private ViewLifetimeControl _viewLifetimeControl;


        public PrintMessageViewModel()
        {
        }

        public void Initialize(ViewLifetimeControl viewLifetimeControl)
        {
            _viewLifetimeControl = viewLifetimeControl;
            _viewLifetimeControl.Released += OnViewLifetimeControlReleased;
        }

        private async void OnViewLifetimeControlReleased(object sender, EventArgs e)
        {
            _viewLifetimeControl.Released -= OnViewLifetimeControlReleased;
            await WindowManagerService.Current.MainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                WindowManagerService.Current.SecondaryViews.Remove(_viewLifetimeControl);
            });
        }
    }
}
