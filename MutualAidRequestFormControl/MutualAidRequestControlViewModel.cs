using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace MutualAidRequestFormControl
{
    public class MutualAidRequestControlViewModel : UserControlViewModelBase
    {
        public static MutualAidRequestControlViewModel Instance { get; } = new MutualAidRequestControlViewModel();


        public override string MsgDate
        {
            get => _msgDate;
            set
            {
                //SignedDate = value;
                //SetProperty(ref _msgDate, value);
                SignedDate = value;
                //UpdateFormFieldsRequiredColors();
            }
        }

        private string _SignedDate;
        public string SignedDate
        {
            get => _SignedDate;
            set => SetProperty(ref _SignedDate, value);
        }

    }
}
