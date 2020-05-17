using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : FormsViewModel
    {
        protected int cityFormsPagePivotSelectedIndex;
        public int CityFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref cityFormsPagePivotSelectedIndex);
            set => SetProperty(ref cityFormsPagePivotSelectedIndex, value, true);
        }

    }
}
