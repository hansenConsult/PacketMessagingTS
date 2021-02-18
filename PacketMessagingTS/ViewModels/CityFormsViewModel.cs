using PacketMessagingTS.Helpers;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : FormsViewModel
    {
        protected int cityFormsPagePivotSelectedIndex;
        public int CityFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref cityFormsPagePivotSelectedIndex);
                //if (index >= SharedData.FormControlAttributeCityList.Count)
                if (index >= PublicData.FormControlAttributesInMenuOrderCity.Length)
                    index = 0;
                if (!FirstTimeFormOpened)
                    LoadMessage = false;
                FormsPagePivotSelectionChangedAsync(index);
                FirstTimeFormOpened = false;
                return index;
            }
            set => SetProperty(ref cityFormsPagePivotSelectedIndex, value, true);
        }

        public override int FormsPagePivotSelectedIndex
        {
            get => CityFormsPagePivotSelectedIndex;
            set => CityFormsPagePivotSelectedIndex = value;
        }

    }
}
