using System;
using System.Collections.ObjectModel;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel : ViewModelBase
    {
        public static ToolsViewModel Instance { get; } = new ToolsViewModel();

        public Pivot ToolsPagePivot { get; set; }

        private int toolsPivotSelectedIndex = -1;
        public int ToolsPivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref toolsPivotSelectedIndex);
                //if (index >= PublicData.FormControlAttributesInMenuOrderOther.Length || index < 0)
                if ( index >= ToolsPagePivot.Items.Count || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref toolsPivotSelectedIndex, value, true);
        }

    }
}
