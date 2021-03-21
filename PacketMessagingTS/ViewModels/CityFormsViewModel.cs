
using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : FormsViewModel
    {
        public static CityFormsViewModel Instance { get; } = new CityFormsViewModel();

        private int cityFormsPagePivotSelectedIndex = -1;
        public int CityFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref cityFormsPagePivotSelectedIndex);
                if (index >= PublicData.FormControlAttributesInMenuOrderCity.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref cityFormsPagePivotSelectedIndex, value, true);
        }

        public override int FormsPagePivotSelectedIndex
        {
            get => CityFormsPagePivotSelectedIndex;
            set => CityFormsPagePivotSelectedIndex = value;
        }

    }
}
