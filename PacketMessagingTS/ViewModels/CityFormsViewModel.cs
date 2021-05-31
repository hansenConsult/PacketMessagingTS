
using PacketMessagingTS.Models;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : FormsViewModel
    {
        public static CityFormsViewModel Instance { get; } = new CityFormsViewModel();

        private int _cityFormsPagePivotSelectedIndex = -1;
        public int CityFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref _cityFormsPagePivotSelectedIndex);
                if (index >= FormMenuIndexDefinitions.Instance.CityFormsMenuNames.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref _cityFormsPagePivotSelectedIndex, value, true);
        }

        public override int FormsPagePivotSelectedIndex
        {
            get => CityFormsPagePivotSelectedIndex;
            set => CityFormsPagePivotSelectedIndex = value;
        }

    }
}
