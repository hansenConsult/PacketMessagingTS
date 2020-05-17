using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class TestFormsViewModel : FormsViewModel
    {
        protected int testFormsPagePivotSelectedIndex;
        public int TestFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref testFormsPagePivotSelectedIndex);
            set => SetProperty(ref testFormsPagePivotSelectedIndex, value, true);
        }

    }
}
