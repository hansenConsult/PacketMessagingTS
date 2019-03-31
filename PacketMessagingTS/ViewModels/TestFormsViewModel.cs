using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;


namespace PacketMessagingTS.ViewModels
{
    public class TestFormsViewModel : BaseFormsViewModel
    {
        public int TestFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref formsPagePivotSelectedIndex);
            set
            {
                SetProperty(ref formsPagePivotSelectedIndex, value, true);
            }
        }

    }
}
