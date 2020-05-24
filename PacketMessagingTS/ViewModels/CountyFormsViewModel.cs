using System;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CountyFormsViewModel : FormsViewModel
    {
        private int countyFormsPagePivotSelectedIndex;
        public int CountyFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref countyFormsPagePivotSelectedIndex);
            set => SetProperty(ref countyFormsPagePivotSelectedIndex, value, true);
        }

    }
}
