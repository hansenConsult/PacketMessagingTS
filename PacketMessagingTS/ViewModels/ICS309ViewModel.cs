using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;

using SharedCode;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class ICS309ViewModel : ViewModelBase
    {
        public static ICS309ViewModel Instance { get; } = new ICS309ViewModel();

        private CommLog _CommLog;
        private bool _fromOpenFile;


        public void Initialize()
        {
            _fromOpenFile = false;
            _CommLog = new CommLog();
            //incidentName.Text = IncidentName;
            OperationalPeriodStart = DateTime.Today;
            OperationalPeriodEnd = DateTime.Now;
            OperationalPeriod = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            RadioNetName = IdentityViewModel.Instance.UseTacticalCallsign ? $"{IdentityViewModel.Instance.TacticalCallsign}" : "";
            OperatorNameCallsign = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
            DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
            TotalPages = 1;
            PageNo = 1;
        }

        //private string incidentNameActivationNumber;
        //public string IncidentNameActivationNumber
        //{
        //    get => incidentNameActivationNumber;
        //    set => SetProperty(ref incidentNameActivationNumber, value);
        //}

        private string incidentName;
        public string IncidentName
        {
            get => incidentName;
            set => SetProperty(ref incidentName, value);
        }

        private string activationNumber;
        public string ActivationNumber
        {
            get => activationNumber;
            set => SetProperty(ref activationNumber, value);
        }

        private DateTime operationalPeriodStart;
        public DateTime OperationalPeriodStart
        {
            get => operationalPeriodStart;
            set => operationalPeriodStart = value;
        }

        private string radioNetName;
        public string RadioNetName
        {
            get => radioNetName;
            set => SetProperty(ref radioNetName, value);
        }

        private DateTime operationalPeriodEnd;
        public DateTime OperationalPeriodEnd
        {
            get => operationalPeriodEnd;
            set => operationalPeriodEnd = value;
        }

        private async void OperationalPeriod_TextChangedAsync(string operationalPeriod)
        {
            string opPeriod = operationalPeriod;
            var startStop = opPeriod.Split(new string[] { "to", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (startStop != null && (startStop.Count() != 3 && startStop.Count() != 4))
                return;

            int endTimeIndex = 3;
            if (startStop.Count() == 3)
            {
                endTimeIndex = 2;
            }

            if (startStop[1].Length != 5)
                return;

            if (startStop[endTimeIndex].Length != 5)
                return;

            string dateTime = startStop[0] + " " + startStop[1];

            DateTime operationalPeriodStart;
            if (!DateTime.TryParse(dateTime, out operationalPeriodStart))
                return;

            if (startStop.Count() == 3)
            {
                dateTime = startStop[0] + " " + startStop[endTimeIndex];
            }
            else
            {
                dateTime = startStop[2] + " " + startStop[endTimeIndex];
            }

            DateTime operationalPeriodEnd;
            if (!DateTime.TryParse(dateTime, out operationalPeriodEnd))
                return;

            if (operationalPeriodEnd < operationalPeriodStart)
                return;

            OperationalPeriodStart = operationalPeriodStart;
            OperationalPeriodEnd = operationalPeriodEnd;

            if (operationalPeriodEnd - operationalPeriodStart > new TimeSpan(0, 0, 0))
            {
                await BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
            }
        }

        private string operationalPeriod;
        public string OperationalPeriod
        {
            get => operationalPeriod;
            set
            {
                if (!_fromOpenFile)
                {
                    OperationalPeriod_TextChangedAsync(value);
                }
                SetProperty(ref operationalPeriod, $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}");
            }
        }

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

        private ObservableCollection<CommLogEntry> commLogEntryCollection;
        public ObservableCollection<CommLogEntry> CommLogEntryCollection
        {
            get => commLogEntryCollection ?? (commLogEntryCollection = new ObservableCollection<CommLogEntry>());
            set => SetProperty(ref commLogEntryCollection, value);
        }

        public object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        public List<T> Sort_List<T>(List<T> data)
        {
            List<T> data_sorted = new List<T>();

            data_sorted = (from n in data
                           orderby GetDynamicSortProperty(n, "Time") ascending
                           select n).ToList();
            return data_sorted;
        }

        private async Task BuildLogDataSetAsync(DateTime startTime, DateTime endTime)
        {
            _CommLog.CommLogEntryList.Clear();
            // Get messages in the InBox and the Sent Messages folder
            List<PacketMessage> messagesInReceivedFolder = await PacketMessage.GetPacketMessages(SharedData.ReceivedMessagesFolder.Path);
            foreach (PacketMessage packetMessage in messagesInReceivedFolder)
            {
                _CommLog.AddCommLogEntry(packetMessage, startTime, endTime);
            }
            List<PacketMessage> messagesInSentFolder = await PacketMessage.GetPacketMessages(SharedData.SentMessagesFolder.Path);
            foreach (PacketMessage packetMessage in messagesInSentFolder)
            {
                _CommLog.AddCommLogEntry(packetMessage, startTime, endTime);
            }
            List<CommLogEntry> sortedList = Sort_List(_CommLog.CommLogEntryList);
            _CommLog.CommLogEntryList = sortedList;

            CommLogEntryCollection = new ObservableCollection<CommLogEntry>(sortedList);
        }

        private void FillFormFromCommLog()
        {
            if (_CommLog is null)
                return;

            _fromOpenFile = true;
            IncidentName = _CommLog.IncidentName;
            ActivationNumber = _CommLog.ActivationNumber;
            OperationalPeriodStart = _CommLog.OperationalPeriodFrom;
            OperationalPeriodEnd = _CommLog.OperationalPeriodTo;
            OperationalPeriod = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            RadioNetName = _CommLog.RadioNetName;
            OperatorNameCallsign = _CommLog.OperatorNameCallsign;
            CommLogEntryCollection = new ObservableCollection<CommLogEntry>(_CommLog.CommLogEntryList);
            DateTimePrepared = _CommLog.DateTimePrepared;
        }

        private ICommand _OpenICS309Command;
        public ICommand OpenICS309Command => _OpenICS309Command ?? (_OpenICS309Command = new RelayCommand(OpenICS309));

        public async void OpenICS309()
        {
            StorageFile file;
            ContentDialogOpenICS309 selectFile = new ContentDialogOpenICS309();
            selectFile.ICS309Files = await selectFile.GetFilesAsync(".xml");
            ContentDialogResult result = await selectFile.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                file = selectFile.ICS309Files[selectFile.FilesSelectedIndex];
            }
            else
            {
                return;
            }

            if (file is null)
                return;

            _CommLog = CommLog.Open(file);
            FillFormFromCommLog();
        }

        private ICommand _SaveICS309Command;
        public ICommand SaveICS309Command => _SaveICS309Command ?? (_SaveICS309Command = new RelayCommand(SaveICS309));

        public async void SaveICS309()
        {
            _CommLog.IncidentName = IncidentName;
            _CommLog.ActivationNumber = ActivationNumber;
            _CommLog.OperationalPeriodFrom = OperationalPeriodStart;
            _CommLog.OperationalPeriodTo = OperationalPeriodEnd;
            _CommLog.RadioNetName = RadioNetName;
            _CommLog.OperatorNameCallsign = OperatorNameCallsign;
            _CommLog.DateTimePrepared = DateTimePrepared;
            await _CommLog.SaveAsync();
        }

        //private ICommand _PrintICS309Command;
        //public ICommand PrintICS309Command => _PrintICS309Command ?? (_PrintICS309Command = new RelayCommand(PrintICS309));

        //public async void PrintICS309()
        //{
        //    await printHelper.ShowPrintUIAsync("ICS 309", true);
        //    //await PrintManager.ShowPrintUIAsync();
        //}

    }
} 
