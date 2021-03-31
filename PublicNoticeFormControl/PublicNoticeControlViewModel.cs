using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedCode.Helpers;

namespace PublicNoticeFormControl
{
    public class PublicNoticeControlViewModel : UserControlViewModelBase
    {
        //public static PublicNoticeControlViewModel Instance { get; } = new PublicNoticeControlViewModel();


        private bool _pageVisibility = true;
        public bool PageVisibility
        {
            get => _pageVisibility;
            set => SetProperty(ref _pageVisibility, value);
        }

        private bool _noticeVisibility = false;
        public bool NoticeVisibility
        {
            get => _noticeVisibility;
            set => SetProperty(ref _noticeVisibility, value);
        }

        private string _noticeType;
        public string NoticeType
        {
            get => _noticeType;
            set => SetProperty(ref _noticeType, value);
        }

        private int _typeFontSize = 50;
        public int TypeFontSize
        {
            get => _typeFontSize;
            set => SetProperty(ref _typeFontSize, value);
        }

        private string _topic;
        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        private int _topicFontSize = 35;
        public int TopicFontSize
        {
            get => _topicFontSize;
            set => SetProperty(ref _topicFontSize, value);
        }

        private string _issuedBy;
        public string IssuedBy
        {
            get => _issuedBy;
            set => SetProperty(ref _issuedBy, value);
        }

        private string _effectiveDate;
        public string EffectiveDate
        {
            get => _effectiveDate;
            set => SetProperty(ref _effectiveDate, value);
        }

        private string _expires;
        public string Expires
        {
            get => _expires;
            set => SetProperty(ref _expires, value);
        }

        private string _notice;
        public string Notice
        {
            get => _notice;
            set => SetProperty(ref _notice, value);
        }

        private string _signed;
        public string Signed
        {
            get => _signed;
            set => SetProperty(ref _signed, value);
        }

    }
}
