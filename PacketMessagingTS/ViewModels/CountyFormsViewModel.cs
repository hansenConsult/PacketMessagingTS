using System;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CountyFormsViewModel : FormsViewModel
    {
        //public override int FormsPagePivotSelectedIndex
        //{
        //    get => CountyFormsPagePivotSelectedIndex;
        //    set => CountyFormsPagePivotSelectedIndex = value;
        //}

        private int countyFormsPagePivotSelectedIndex;
        public int CountyFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref countyFormsPagePivotSelectedIndex);
                if (index >= SharedData.FormControlAttributeCountyList.Count)
                    index = 0;
                FormsPagePivotSelectionChangedAsync(index);
                return index;
            }
            set => SetProperty(ref countyFormsPagePivotSelectedIndex, value, true);
        }

    }
}
