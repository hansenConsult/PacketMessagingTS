using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace ResourceRequestPackItFormControl
{
    public class ResourceRequestPackItControlViewModel : UserControlViewModelBase
    {
        public override string MsgDate
        {
            get => _msgDate;
            set =>
                //SetProperty(ref _msgDate, value);
                InitiatedDate = value;
        }

        private string _initiatedDate;
        public string InitiatedDate
        {
            get => _initiatedDate;
            set => SetProperty(ref _initiatedDate, value);
        }
    }
}
