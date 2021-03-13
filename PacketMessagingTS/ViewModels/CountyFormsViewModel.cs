using System;

using PacketMessagingTS.Helpers;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CountyFormsViewModel : FormsViewModel
    {
        public static CountyFormsViewModel Instance { get; } = new CountyFormsViewModel();

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
                if (index >= PublicData.FormControlAttributesInMenuOrderCounty.Length || index < 0)
                    index = 0;
                FormsPagePivotSelectionChangedAsync(index);
                return index;
            }
            set => SetProperty(ref countyFormsPagePivotSelectedIndex, value, true);
        }

    }
}
