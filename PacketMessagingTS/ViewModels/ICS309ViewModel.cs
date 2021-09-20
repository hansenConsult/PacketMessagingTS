using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.Input;

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

        private readonly int _linesPerPage = 22;


        public async Task InitializeAsync()
        {
            FromOpenFile = false;

            _CommLog = new CommLog();
            _CommLog.OperationalPeriodFrom = DateTime.Today;
            _CommLog.OperationalPeriodTo = DateTime.Now;
            _CommLog.OperatorNameCallsign = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
            _CommLog.DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
            await BuildLogDataSetAsync(_CommLog.OperationalPeriodFrom, _CommLog.OperationalPeriodTo);
        }

        //private string incidentNameActivationNumber;
        //public string IncidentNameActivationNumber
        //{
        //    get => incidentNameActivationNumber;
        //    set => SetProperty(ref incidentNameActivationNumber, value);
        //}

        //private string _incidentName;
        //public string IncidentName
        //{
        //    get => _incidentName;
        //    set => SetProperty(ref _incidentName, value);
        //}

        //private string _activationNumber;
        //public string ActivationNumber
        //{
        //    get => _activationNumber;
        //    set => SetProperty(ref _activationNumber, value);
        //}

        //private DateTime _operationalPeriodStart;
        //public DateTime OperationalPeriodStart
        //{
        //    get => _operationalPeriodStart;
        //    set => _operationalPeriodStart = value;
        //}

        //private string _radioNetName;
        //public string RadioNetName
        //{
        //    get => _radioNetName;
        //    set => SetProperty(ref _radioNetName, value);
        //}

        //private DateTime _operationalPeriodEnd;
        //public DateTime OperationalPeriodEnd
        //{
        //    get => _operationalPeriodEnd;
        //    set => _operationalPeriodEnd = value;
        //}

        //private async void OperationalPeriod_TextChangedAsync(string operationalPeriod)
        //{
        //    string opPeriod = operationalPeriod;
        //    var startStop = opPeriod.Split(new string[] { "to", " " }, StringSplitOptions.RemoveEmptyEntries);
        //    if (startStop != null && (startStop.Count() != 3 && startStop.Count() != 4))
        //        return;

        //    int endTimeIndex = 3;
        //    if (startStop.Count() == 3)
        //    {
        //        endTimeIndex = 2;
        //    }

        //    if (startStop[1].Length != 5)
        //        return;

        //    if (startStop[endTimeIndex].Length != 5)
        //        return;

        //    string dateTime = startStop[0] + " " + startStop[1];

        //    if (!DateTime.TryParse(dateTime, out DateTime operationalPeriodStart))
        //        return;

        //    if (startStop.Count() == 3)
        //    {
        //        dateTime = startStop[0] + " " + startStop[endTimeIndex];
        //    }
        //    else
        //    {
        //        dateTime = startStop[2] + " " + startStop[endTimeIndex];
        //    }

        //    if (!DateTime.TryParse(dateTime, out DateTime operationalPeriodEnd))
        //        return;

        //    if (operationalPeriodEnd < operationalPeriodStart)
        //        return;

        //    OperationalPeriodStart = operationalPeriodStart;
        //    OperationalPeriodEnd = operationalPeriodEnd;

        //    if (operationalPeriodEnd - operationalPeriodStart > new TimeSpan(0, 0, 0))
        //    {
        //        await BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
        //    }
        //}

        //private string _operationalPeriod;
        //public string OperationalPeriod
        //{
        //    get => _operationalPeriod;
        //    set
        //    {
        //        if (!_fromOpenFile)
        //        {
        //            OperationalPeriod_TextChangedAsync(value);
        //        }
        //        SetProperty(ref _operationalPeriod, $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}");
        //    }
        //}

        private CommLog _CommLog;
        public CommLog CommLog
        {
            get => _CommLog;
            set => SetProperty(ref _CommLog, value);
        }

        public bool FromOpenFile
        { get; set; }

        private bool _ics309PrintButtonVisible;
        public bool ICS309PrintButtonVisible
        {
            get => _ics309PrintButtonVisible;
            set => SetProperty(ref _ics309PrintButtonVisible, value);
        }

        private List<CommLogEntry> _commLogEntriesByPage;
        public List<CommLogEntry> CommLogEntriesByPage
        {
            get => _commLogEntriesByPage;
            set => _commLogEntriesByPage = value;
        }

        private ObservableCollection<CommLogEntry> _commLogEntryCollection;
        public ObservableCollection<CommLogEntry> CommLogEntryCollection
        {
            get => _commLogEntryCollection ?? (_commLogEntryCollection = new ObservableCollection<CommLogEntry>());
            set => SetProperty(ref _commLogEntryCollection, value);
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

        public async Task BuildLogDataSetAsync(DateTime startTime, DateTime endTime)
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

            int commLogEntryCount = _CommLog.CommLogEntryList.Count;
            int pages = Math.DivRem(commLogEntryCount, _linesPerPage, out int remainder);
            ICS309FooterViewModel.Instance.TotalPages = remainder == 0 ? pages : pages + 1;
        }

        //pageNo is 0-based
        public void GetCommLogEntriesByPage(int pageNo)
        {
            int startIndex = _linesPerPage * pageNo;
            int lineCount = _linesPerPage;
            if (pageNo + 1 == ICS309FooterViewModel.Instance.TotalPages)
            {
                lineCount = _CommLog.CommLogEntryList.Count - (pageNo * _linesPerPage);
            }
            CommLogEntriesByPage = _CommLog.CommLogEntryList.GetRange(startIndex, lineCount);
            CommLogEntryCollection = new ObservableCollection<CommLogEntry>(CommLogEntriesByPage);
        }

        public void FillFormFromCommLog()
        {
            if (_CommLog is null)
            {
                return;
            }
            ICS309HeaderViewModel.Instance.IncidentName = _CommLog.IncidentName;
            ICS309HeaderViewModel.Instance.ActivationNumber = _CommLog.ActivationNumber;
            ICS309HeaderViewModel.Instance.OperationalPeriod = $"{DateTimeStrings.DateTimeString(_CommLog.OperationalPeriodFrom)} to {DateTimeStrings.DateTimeString(_CommLog.OperationalPeriodTo)}";
            ICS309HeaderViewModel.Instance.RadioNetName = _CommLog.RadioNetName;
            ICS309HeaderViewModel.Instance.OperatorNameCallsign = _CommLog.OperatorNameCallsign;
            ICS309FooterViewModel.Instance.OperatorNameCallsign = _CommLog.OperatorNameCallsign;
            ICS309FooterViewModel.Instance.DateTimePrepared = _CommLog.DateTimePrepared;
            CommLogEntryCollection = new ObservableCollection<CommLogEntry>(_CommLog.CommLogEntryList);
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
            FromOpenFile = true;
            FillFormFromCommLog();
        }

        private ICommand _SaveICS309Command;
        public ICommand SaveICS309Command => _SaveICS309Command ?? (_SaveICS309Command = new RelayCommand(SaveICS309));

        public async void SaveICS309()
        {
            _CommLog.IncidentName = ICS309HeaderViewModel.Instance.IncidentName;
            _CommLog.ActivationNumber = ICS309HeaderViewModel.Instance.ActivationNumber;
            _CommLog.RadioNetName = ICS309HeaderViewModel.Instance.RadioNetName;
            _CommLog.OperatorNameCallsign = ICS309HeaderViewModel.Instance.OperatorNameCallsign;
            _CommLog.DateTimePrepared = ICS309FooterViewModel.Instance.DateTimePrepared;
            await _CommLog.SaveAsync();
        }

    }
} 
