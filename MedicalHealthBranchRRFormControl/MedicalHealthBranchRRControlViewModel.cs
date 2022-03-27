using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace MedicalHealthBranchRRFormControl
{
    public class MedicalHealthBranchRRControlViewModel : UserControlViewModelBase
    {
        //public static MedicalHealthBranchRRControlViewModel Instance { get; } = new MedicalHealthBranchRRControlViewModel();
        //HandlingOrder = "priority;"

        public override string MsgDate
        {
            get => _msgDate;
            set => RequestMsgDate = value;
        }

        private string requestMsgDate;
        public string RequestMsgDate
        {
            get => requestMsgDate;
            set => SetProperty(ref requestMsgDate, value);
        }

    }
}
