using PacketMessagingTS.Helpers;

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
                if (index >= SharedData.FormControlAttributeCityList.Count)
                    index = 0;
                FormsPagePivotSelectionChangedAsync(index);
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
