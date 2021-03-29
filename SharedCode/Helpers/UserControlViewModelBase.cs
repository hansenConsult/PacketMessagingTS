using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace SharedCode.Helpers
{
    public class UserControlViewModelBase : ObservableRecipient
    {
        protected ObservableRecipient UserControlViewModel
        { get; set; }

        private string operatorName;
        public string OperatorName
        {
            get => operatorName;
            set => SetProperty(ref operatorName, value);
        }

        private string operatorCallsign;
        public string OperatorCallsign
        {
            get => operatorCallsign;
            set => SetProperty(ref operatorCallsign, value);
        }

    }
}
