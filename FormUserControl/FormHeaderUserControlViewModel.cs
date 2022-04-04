
using SharedCode.Helpers;

using System.Collections.Generic;

namespace FormUserControl
{
    public class FormHeaderUserControlViewModel : UserControlViewModelBase
    {
        //private bool namePanel1Visibility = true;
        private bool namePanel1Visibility = false;
        public bool NamePanel1Visibility
        {
            get => namePanel1Visibility;
            set => SetProperty(ref namePanel1Visibility, value);
        }

        public bool NamePanel2Visibility => !NamePanel1Visibility;

        private string headerString1;
        public string HeaderString1
        {
            get => headerString1;
            set => SetProperty(ref headerString1, value);
        }

        private string headerString2;
        public string HeaderString2
        {
            get => headerString2;
            set => SetProperty(ref headerString2, $" {value}");
        }

        private string headerSubstring;
        public string HeaderSubstring
        {
            get => headerSubstring;
            set => SetProperty(ref headerSubstring, value);
        }


    }
}
