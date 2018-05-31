using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;


namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel :  BaseViewModel
    {
        public string incidentName = "";
        public string IncidentName
        {
            get => GetProperty(ref incidentName);
            set => SetProperty(ref incidentName, value, true);
        }

        public DateTime operationalPeriodStart = DateTime.Now;
        public DateTime OperationalPeriodStart
        {
            get => GetProperty(ref operationalPeriodStart);
            set => SetProperty(ref operationalPeriodStart, value, true);
        }

        public string radioNetName = "";
        public string RadioNetName
        {
            get => GetProperty(ref radioNetName);
            set => SetProperty(ref radioNetName, value, true);
        }

        public DateTime operationalPeriodEnd = DateTime.Now;
        public DateTime OperationalPeriodEnd
        {
            get => GetProperty(ref operationalPeriodEnd);
            set => SetProperty(ref operationalPeriodEnd, value, true);
        }

        public string radioOperator;
        public string RadioOperator
        {
            get => GetProperty(ref radioOperator);
            set
            {
                SetProperty(ref radioOperator, value, true);
            }
        }

        public DateTime dateTimePrepared;
        public DateTime DateTimePrepared
        {
            get => GetProperty(ref dateTimePrepared);
            set
            {
                SetProperty(ref dateTimePrepared, value, true);
            }
        }

        public int totalPages;
        public int TotalPages
        {
            get => totalPages;
            set
            {
                SetProperty(ref totalPages, value);
            }
        }

        public int pageNo;
        public int PageNo
        {
            get => pageNo;
            set
            {
                SetProperty(ref pageNo, value);
                PageNoAsString = PageNo.ToString();
            }
        }

        public string pageNoAsString;
        public string PageNoAsString
        {
            get => pageNoAsString;
            set
            {
                pageNoAsString = $"Page {value} of {TotalPages.ToString()}";
            }
        }

    }
}
