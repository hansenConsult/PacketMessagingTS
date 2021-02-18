using System;

using PacketMessagingTS.Helpers;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CountyFormsViewModel : FormsViewModel
    {
        public override int FormsPagePivotSelectedIndex
        {
            get => CountyFormsPagePivotSelectedIndex;
            set => CountyFormsPagePivotSelectedIndex = value;
        }

        private int countyFormsPagePivotSelectedIndex;
        public int CountyFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref countyFormsPagePivotSelectedIndex);
                if (index >= PublicData.FormControlAttributesInMenuOrderCounty.Length)
                    index = 0;
                if (!FirstTimeFormOpened)
                    LoadMessage = false;
                FormsPagePivotSelectionChangedAsync(index);
                FirstTimeFormOpened = false;
                return index;
            }
            set => SetProperty(ref countyFormsPagePivotSelectedIndex, value, true);
        }

    }
}
