
using SharedCode.Helpers;

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

        //public override string SenderMsgNo
        //{ get; set; }

    }
}
