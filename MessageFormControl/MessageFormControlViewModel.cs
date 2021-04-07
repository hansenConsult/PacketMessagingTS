using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace MessageFormControl
{
    public class MessageFormControlViewModel : UserControlViewModelBase
    {
        public static MessageFormControlViewModel Instance { get; } = new MessageFormControlViewModel();

        public enum HeaderVisibility
        {
            None,
            InboxHeader,
            SentHeader,
            NewHeader,
            PrintHeader,
            FixedContent,
        }

        public HeaderVisibility _visibleHeader = HeaderVisibility.None;
        public HeaderVisibility _previousVisibleHeader = HeaderVisibility.None;


        private bool inBoxHeaderVisibility;
        public bool InBoxHeaderVisibility
        {
            get => inBoxHeaderVisibility;
            set
            {
                SetProperty(ref inBoxHeaderVisibility, value);
                if (value)
                {
                    _visibleHeader = HeaderVisibility.InboxHeader;
                    FixedContentVisibility = true;
                    SentHeaderVisibility = false;
                    NewHeaderVisibility = false;
                    PrintHeaderVisibility = false;
                }
            }
        }

        private bool sentHeaderVisibility;
        public bool SentHeaderVisibility
        {
            get => sentHeaderVisibility;
            set
            {
                SetProperty(ref sentHeaderVisibility, value);
                if (value)
                {
                    _visibleHeader = HeaderVisibility.SentHeader;
                    FixedContentVisibility = true;
                    InBoxHeaderVisibility = false;
                    NewHeaderVisibility = false;
                    PrintHeaderVisibility = false;
                }
            }
        }

        private bool newHeaderVisibility;
        public  bool NewHeaderVisibility
        {
            get => newHeaderVisibility;
            set
            {
                SetProperty(ref newHeaderVisibility, value);
                if (value)
                {
                    _visibleHeader = HeaderVisibility.NewHeader;
                    FixedContentVisibility = false;
                    InBoxHeaderVisibility = false;
                    SentHeaderVisibility = false;
                    PrintHeaderVisibility = false;
                }
            }
        }

        private bool printHeaderVisibility;
        public bool PrintHeaderVisibility
        {
            get => printHeaderVisibility;
            set
            {
                SetProperty(ref printHeaderVisibility, value);
                if (value)
                {
                    _visibleHeader = HeaderVisibility.PrintHeader;
                    FixedContentVisibility = true;
                    NewHeaderVisibility = false;
                    InBoxHeaderVisibility = false;
                    SentHeaderVisibility = false;
                }
            }
        }

        private bool fixedContentVisibility;
        public bool FixedContentVisibility
        {
            get => fixedContentVisibility;
            set
            {
                SetProperty(ref fixedContentVisibility, value);
            }
        }

        public void SetHeaderVisibility()
        {
            switch (_visibleHeader)
            {
                case HeaderVisibility.None:
                    break;
                case HeaderVisibility.InboxHeader:
                    InBoxHeaderVisibility = true;
                    break;
                case HeaderVisibility.SentHeader:
                    SentHeaderVisibility = true;
                    break;
                case HeaderVisibility.NewHeader:
                    NewHeaderVisibility = true;
                    break;
                case HeaderVisibility.PrintHeader:
                    PrintHeaderVisibility = true;
                    break;
                    //case HeaderVisibility.FixedContent:
                    //    FixedContentVisibility = true;
                    //    break;
            }
        }

        private string _messageBody;
        public  string MessageBody
        {
            get => _messageBody;
            set => SetProperty(ref _messageBody, value);
        }

        private DateTime? messageReceivedTime;
        public DateTime? MessageReceivedTime
        {
            get => messageReceivedTime;
            set => SetProperty(ref messageReceivedTime, value);
            //set => messageReceivedTime = value;
        }

        private DateTime? messageSentTime = null;
        public DateTime? MessageSentTime
        {
            get => messageSentTime;
            set => SetProperty(ref messageSentTime, value);
            //set => messageSentTime = value;
        }

    }
}
