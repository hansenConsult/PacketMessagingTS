using System;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

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

        private int _countyFormsPagePivotSelectedIndex = -1;
        public int CountyFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref _countyFormsPagePivotSelectedIndex);
                if (index >= FormMenuIndexDefinitions.Instance.CountyFormsMenuNames.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref _countyFormsPagePivotSelectedIndex, value, true);
        }


    }
}
