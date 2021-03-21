using System;

using PacketMessagingTS.Helpers;

using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

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

        private int countyFormsPagePivotSelectedIndex = -1;
        public int CountyFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref countyFormsPagePivotSelectedIndex);
                if (index >= PublicData.FormControlAttributesInMenuOrderCounty.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref countyFormsPagePivotSelectedIndex, value, true);
        }


    }
}
