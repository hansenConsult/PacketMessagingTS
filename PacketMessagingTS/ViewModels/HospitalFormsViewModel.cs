using PacketMessagingTS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PacketMessagingTS.ViewModels
{
    public class HospitalFormsViewModel : BaseViewModel
    {
        private int hospitalFormsPagePivotSelectedIndex;
        public int HospitalFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref hospitalFormsPagePivotSelectedIndex);
            set
            {
                SetProperty(ref hospitalFormsPagePivotSelectedIndex, value, true);
            }
        }

    }
}
