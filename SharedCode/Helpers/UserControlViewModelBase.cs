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

        private bool namePanel1Visibility = true;
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

        public virtual string MessageBody
        { get; set; }

        protected bool inBoxHeaderVisibility;
        public virtual bool InBoxHeaderVisibility
        { get; set; }

        protected bool sentHeaderVisibility;
        public virtual bool SentHeaderVisibility
        { get; set; }

        protected bool newHeaderVisibility;
        public virtual bool NewHeaderVisibility
        { get; set; }


    }
}