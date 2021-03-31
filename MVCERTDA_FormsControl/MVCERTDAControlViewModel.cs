﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SharedCode.Models;
using SharedCode;

namespace MVCERTDA_FormControl
{
    public class MVCERTDAControlViewModel : UserControlViewModelBase
    {
        //public static MVCERTDAControlViewModel Instance { get; } = new MVCERTDAControlViewModel();

        public List<TacticalCall> CERTLocationTacticalCalls { get => TacticalCallsigns.CreateMountainViewCERTList(); }    // Must be sorted by Agency Name


        private string howReceivedSent;
        public string HowReceivedSent
        {
            get => howReceivedSent;
            set => SetProperty(ref howReceivedSent, value);
        }

        public override string TacticalCallsign
        {
            get => CERTLocationValue;
            set => CERTLocationValue = value;
        }

        private string tacticalCallsign;
        public string CERTLocationValue
        {
            get => tacticalCallsign;
            set => SetProperty(ref tacticalCallsign, value);
        }


    }
}
