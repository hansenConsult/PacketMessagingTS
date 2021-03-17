using System;
using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel : ViewModelBase
    {
        public static ToolsViewModel Instance { get; } = new ToolsViewModel();

        private int toolsPivotSelectedIndex;
        public int ToolsPivotSelectedIndex
        {
            //get => toolsPivotSelectedIndex;
            get => GetProperty(ref toolsPivotSelectedIndex);
            set => SetPropertyPrivate(ref toolsPivotSelectedIndex, value, true);
        }

        //public ToolsViewModel()
        //{
        //    object o = App.Properties[nameof(ToolsPivotSelectedIndex)];
        //    toolsPivotSelectedIndex = Convert.ToInt32(o);
        //}

    }
}
