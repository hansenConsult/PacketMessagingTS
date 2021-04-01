using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace OA_Allied_HealthStatusFormControl
{
    public class OAAlliedHealthStatusControlViewModel : UserControlViewModelBase
    {
        //public static OAAlliedHealthStatusControlViewModel Instance { get; } = new OAAlliedHealthStatusControlViewModel();


        public override string MsgDate
        {
            get => _msgDate;
            set => FacilityDate = value;
        }

        private string facilityDate;
        public string FacilityDate
        {
            get => facilityDate;
            set => SetProperty(ref facilityDate, value);
        }

    }
}
