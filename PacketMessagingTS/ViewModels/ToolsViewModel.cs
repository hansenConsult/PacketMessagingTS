using System;
using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel : BaseViewModel
    {
        public static ToolsViewModel Instance { get; } = new ToolsViewModel();

        private int toolsPivotSelectedIndex;
        public int ToolsPivotSelectedIndex
        {
            get => GetProperty(ref toolsPivotSelectedIndex);
            set => SetProperty(ref toolsPivotSelectedIndex, value, true);
        }

    }
}
