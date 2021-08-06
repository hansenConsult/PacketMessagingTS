﻿
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

namespace ICS213PackItFormControl
{
    public class ICS213PackItControlViewModel : UserControlViewModelBase
    {
        private string howReceivedSent;
        public string HowReceivedSent
        {
            get => howReceivedSent;
            set => SetProperty(ref howReceivedSent, value);
        }

    }
}
