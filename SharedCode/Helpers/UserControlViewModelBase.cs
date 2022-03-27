
using Microsoft.Toolkit.Mvvm.ComponentModel;

using Windows.UI.Xaml.Controls;

namespace SharedCode.Helpers
{
    public class UserControlViewModelBase : ObservableObject
    {
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

        private string _headerPIF;
        public string HeaderPIF
        {
            get => $"PIF: {_pif}";
            set => SetProperty(ref _headerPIF, value);
        }

        public virtual string MessageNo
        { get; set; }

        private string _originMsgNo;
        public virtual string OriginMsgNo
        {
            get => _originMsgNo;
            set => SetProperty(ref _originMsgNo, value);
        }

        private string _destinationMsgNo;
        public virtual string DestinationMsgNo
        {
            get => _destinationMsgNo;
            set => SetProperty(ref _destinationMsgNo, value);
        }

        private string _senderMsgNo;
        public virtual string SenderMsgNo
        { 
            get => _senderMsgNo; 
            set => SetProperty(ref _senderMsgNo, value);
        }

        protected string _msgDate;
        public virtual string MsgDate
        {
            get => _msgDate;
            set => SetProperty(ref _msgDate, value);
        }

        protected string _msgTime;
        public virtual string MsgTime
        { 
            get => _msgTime;
            set => SetProperty(ref _msgTime, value); 
        }

        protected string _handlingOrder;
        public string HandlingOrder
        {
            get => _handlingOrder;
            set => SetProperty(ref _handlingOrder, value == "" ? null : value);
        }

        private string _operatorName;
        public string OperatorName
        {
            get => _operatorName;
            set => SetProperty(ref _operatorName, value);
        }

        private string _operatorCallsign;
        public string OperatorCallsign
        {
            get => _operatorCallsign;
            set => SetProperty(ref _operatorCallsign, value);
        }

        private string _receivedOrSent;
        public string ReceivedOrSent
        {
            get => _receivedOrSent;
            set => SetProperty(ref _receivedOrSent, value);
        }

        public virtual string TacticalCallsign
        { get; set; }

    }
}