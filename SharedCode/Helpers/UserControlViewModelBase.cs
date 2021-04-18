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

        //protected ObservableRecipient UserControlViewModel
        //{ get; set; }


        public virtual string PackItFormVersion => "3.2";

        protected string _pif = "2.1";
        public virtual string PIF
        {
            get => _pif;
            set
            {
                _pif = value;
                HeaderPIF = $"PIF: {_pif}";
            }
        }

        private string headerPIF;
        public string HeaderPIF
        {
            get => $"PIF: {_pif}";
            set => SetProperty(ref headerPIF, value);
        }

        public virtual string MessageNo
        { get; set; }

        private string originMsgNo;
        public virtual string OriginMsgNo
        {
            get => originMsgNo;
            set => SetProperty(ref originMsgNo, value);
        }

        private string destinationMsgNo;
        public virtual string DestinationMsgNo
        {
            get => destinationMsgNo;
            set => SetProperty(ref destinationMsgNo, value);
        }

        private string senderMsgNo;
        public virtual string SenderMsgNo
        { 
            get => senderMsgNo; 
            set => SetProperty(ref senderMsgNo, value);
        }

        protected string _msgDate;
        public virtual string MsgDate
        {
            get => _msgDate;
            set => SetProperty(ref _msgDate, value);
        }

        public virtual string MsgTime
        { get; set; }

        private string handlingOrder;
        public string HandlingOrder
        {
            get => handlingOrder;
            set => SetProperty(ref handlingOrder, value == "" ? null : value);
        }

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

        private string receivedOrSent;
        public string ReceivedOrSent
        {
            get => receivedOrSent;
            set => SetProperty(ref receivedOrSent, value);
        }

        public virtual string TacticalCallsign
        { get; set; }

        //public virtual string MessageBody
        //{ get; set; }


    }
}