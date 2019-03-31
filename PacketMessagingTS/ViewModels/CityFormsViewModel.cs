using PacketMessagingTS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketMessagingTS.ViewModels
{
    public class CityFormsViewModel : BaseViewModel
    {
        private int cityFormsPagePivotSelectedIndex;
        public int CityFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref cityFormsPagePivotSelectedIndex);
            set
            {
                SetProperty(ref cityFormsPagePivotSelectedIndex, value, true);
            }
        }

    }
}
