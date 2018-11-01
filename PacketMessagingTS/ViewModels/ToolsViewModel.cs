using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

namespace PacketMessagingTS.ViewModels
{
    public class ToolsViewModel : BaseViewModel
    {
        private ObservableCollection<CommLogEntry> commLogEntryCollection = new ObservableCollection<CommLogEntry>();
        public ObservableCollection<CommLogEntry> CommLogEntryCollection
        {
            get => commLogEntryCollection;
            set => SetProperty(ref commLogEntryCollection, value);
        }

        private string incidentName = "";
        public string IncidentName
        {
            get => GetProperty(ref incidentName);
            set => SetProperty(ref incidentName, value, true);
        }

        private DateTime operationalPeriodStart = DateTime.Now;
        public DateTime OperationalPeriodStart
        {
            get => GetProperty(ref operationalPeriodStart);
            set => SetProperty(ref operationalPeriodStart, value, true);
        }

        private string radioNetName = "";
        public string RadioNetName
        {
            get => GetProperty(ref radioNetName);
            set => SetProperty(ref radioNetName, value, true);
        }

        private DateTime operationalPeriodEnd = DateTime.Now;
        public DateTime OperationalPeriodEnd
        {
            get => GetProperty(ref operationalPeriodEnd);
            set => SetProperty(ref operationalPeriodEnd, value, true);
        }

        private string radioOperator;
        public string RadioOperator
        {
            get => GetProperty(ref radioOperator);
            set
            {
                SetProperty(ref radioOperator, value, true);
            }
        }

        private DateTime dateTimePrepared;
        public DateTime DateTimePrepared
        {
            get => GetProperty(ref dateTimePrepared);
            set
            {
                SetProperty(ref dateTimePrepared, value, true);
            }
        }

        private int totalPages;
        public int TotalPages
        {
            get => totalPages;
            set
            {
                SetProperty(ref totalPages, value);
            }
        }

        private int pageNo;
        public int PageNo
        {
            get => pageNo;
            set
            {
                SetProperty(ref pageNo, value);
                PageNoAsString = PageNo.ToString();
            }
        }

        private string pageNoAsString;
        public string PageNoAsString
        {
            get => pageNoAsString;
            set
            {
                pageNoAsString = $"Page {value} of {TotalPages.ToString()}";
            }
        }

        private bool ics309PrintButtonVisible;
        public bool ICS309PrintButtonVisible
        {
            get => ics309PrintButtonVisible;
            set => SetProperty(ref ics309PrintButtonVisible, value);
        }

    }
}
