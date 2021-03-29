﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace FormUserControl
{
    public class RadioOperatorUserControlViewModel : ObservableRecipient
    {
        public static RadioOperatorUserControlViewModel Instance { get; } = new RadioOperatorUserControlViewModel();


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
