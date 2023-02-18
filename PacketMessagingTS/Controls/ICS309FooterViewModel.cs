
//using Microsoft.Toolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PacketMessagingTS.Controls
{
    public class ICS309FooterViewModel : ObservableObject
    {
        public static ICS309FooterViewModel Instance { get; } = new ICS309FooterViewModel();


        private string _OperatorNameCallsign;
        public string OperatorNameCallsign
        {
            get => _OperatorNameCallsign;
            set => SetProperty(ref _OperatorNameCallsign, value);
        }

        private string _DateTimePrepared;
        public string DateTimePrepared
        {
            get => _DateTimePrepared;
            set => SetProperty(ref _DateTimePrepared, value);
        }

        private int _totalPages = 1;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                //PageNoAsString = $"Page {_pageNo} of {_totalPages}";
            }
        }

        //// 1-based page number
        //private int _pageNo = 1;
        //public int PageNo
        //{
        //    get => _pageNo;
        //    set
        //    {
        //        _pageNo = value;
        //        //PageNoAsString = $"Page {_pageNo} of {TotalPages}";
        //    }
        //}

        //private string _pageNoAsString;
        //public string PageNoAsString
        //{
        //    get => _pageNoAsString;
        //    set => SetProperty(ref _pageNoAsString, value);
        //}

    }
}
