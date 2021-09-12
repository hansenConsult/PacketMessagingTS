using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

namespace PacketMessagingTS.Controls
{
    public class ICS309FooterViewModel : ObservableObject
    {
        public static ICS309FooterViewModel Instance { get; } = new ICS309FooterViewModel();


        private string _OperatorNameCallsignFooter;
        public string OperatorNameCallsignFooter
        {
            get => _OperatorNameCallsignFooter;
            set => SetProperty(ref _OperatorNameCallsignFooter, value);
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
                PageNoAsString = $"Page {_pageNo} of {_totalPages}";
            }
        }

        // 1-based page number
        private int _pageNo = 1;
        public int PageNo
        {
            get => _pageNo;
            set
            {
                _pageNo = value;
                //PageNoAsString = $"Page {_pageNo} of {TotalPages}";
            }
        }

        private string _pageNoAsString;
        public string PageNoAsString
        {
            get => _pageNoAsString;
            set => SetProperty(ref _pageNoAsString, value);
        }

    }
}
