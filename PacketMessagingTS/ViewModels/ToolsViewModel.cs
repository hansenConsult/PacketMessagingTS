using System;
using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel : BaseViewModel
    {
        private int toolsPivotSelectedIndex;
        public int ToolsPivotSelectedIndex
        {
            get => GetProperty(ref toolsPivotSelectedIndex);
            set => SetProperty(ref toolsPivotSelectedIndex, value, true);
        }

    }
}
