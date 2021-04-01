using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using SharedCode.Helpers;

namespace ICS213PackItFormControl
{
    public class ICS213PackItControlViewModel : UserControlViewModelBase
    {
        //public static ICS213PackItControlViewModel Instance { get; } = new ICS213PackItControlViewModel();

        
        private string howReceivedSent;
        public string HowReceivedSent
        {
            get => howReceivedSent;
            set => SetProperty(ref howReceivedSent, value);
        }

    }
}
