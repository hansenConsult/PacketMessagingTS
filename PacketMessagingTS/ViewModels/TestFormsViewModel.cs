using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class TestFormsViewModel : FormsViewModel
    {
        public static TestFormsViewModel Instance { get; } = new TestFormsViewModel();


        public override int FormsPagePivotSelectedIndex
        {
            get => TestFormsPagePivotSelectedIndex;
            set => TestFormsPagePivotSelectedIndex = value;
        }

        protected int _testFormsPagePivotSelectedIndex = -1;
        public int TestFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref _testFormsPagePivotSelectedIndex);
                if (index >= FormMenuIndexDefinitions.Instance.OtherFormsMenuNames.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref _testFormsPagePivotSelectedIndex, value, true);
        }

    }
}
