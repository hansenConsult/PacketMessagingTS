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
        public override int FormsPagePivotSelectedIndex
        {
            get => TestFormsPagePivotSelectedIndex;
            set => TestFormsPagePivotSelectedIndex = value;
        }

        protected int _testFormsPagePivotSelectedIndex;
        public int TestFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref _testFormsPagePivotSelectedIndex);
                if (index >= SharedData.FormControlAttributeTestList.Count)
                    index = 0;
                FormsPagePivotSelectionChangedAsync(index);
                return index;
            }
            set => SetProperty(ref _testFormsPagePivotSelectedIndex, value, true);
        }

    }
}
