using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class RxTxStatusViewModel : BaseViewModel
    {
        private string rxTxStatus;
        public string RxTxStatus
        {
            get => rxTxStatus;
            set => rxTxStatus = value;
        }
    }
}
