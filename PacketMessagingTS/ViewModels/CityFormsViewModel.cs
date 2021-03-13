
using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : FormsViewModel
    {
        public static CityFormsViewModel Instance { get; } = new CityFormsViewModel();

        protected int cityFormsPagePivotSelectedIndex;
        public int CityFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref cityFormsPagePivotSelectedIndex);
                if (index >= PublicData.FormControlAttributesInMenuOrderCity.Length || index < 0)
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
