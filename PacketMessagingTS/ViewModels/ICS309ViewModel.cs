using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using SharedCode;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace PacketMessagingTS.ViewModels
{
    public class ICS309ViewModel : FormsBaseViewModel
    {
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
            RadioNetName = Singleton<IdentityViewModel>.Instance.UseTacticalCallsign ? $"{Singleton<IdentityViewModel>.Instance.TacticalCallsign}" : "";
            OperatorNameCallsign = $"{Singleton<IdentityViewModel>.Instance.UserName}, {Singleton<IdentityViewModel>.Instance.UserCallsign}";
            DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
            TotalPages = 1;
            PageNo = 1;
            //PageNoOf = PageNoAsString;

            //if (OperationalPeriodStart != null && OperationalPeriodEnd != null)
            //{
            //    await BuildLogDataSetAsync(OperationalPeriodStart, OperationalPeriodEnd);
            //}

        }

        private string incidentNameActivationNumber;
        public string IncidentNameActivationNumber
        {
            get => incidentNameActivationNumber;
            //set => SetProperty(ref incidentName, value, true);
            set
            {
                //CommLog.Instance.IncidentNameActivationNumber = value;
                SetProperty(ref incidentNameActivationNumber, value);
            }
        }

        private DateTime operationalPeriodStart;
        public DateTime OperationalPeriodStart
        {
            //get => GetProperty(ref operationalPeriodStart);
            get => operationalPeriodStart;
            //set => SetProperty(ref operationalPeriodStart, value, true);
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
            //get => GetProperty(ref operationalPeriodEnd);
            get => operationalPeriodEnd;
            //set => SetProperty(ref operationalPeriodEnd, value, true);
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
            get => operationalPeriod;// = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            set
            {
                if (!_fromOpenFile)
                {
                    OperationalPeriod_TextChangedAsync(value);
                }
                //operationalPeriod = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
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

        private ObservableCollection<CommLogEntry> commLogEntryCollection = new ObservableCollection<CommLogEntry>();
        public ObservableCollection<CommLogEntry> CommLogEntryCollection
        {
            get => commLogEntryCollection;
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
            //            _toolsViewModel.ToolsPageCommLogPartViewModel viewModel = ToolsPageViewModel.toolsPageCommLogPartViewModel;
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
            _fromOpenFile = true;
            IncidentNameActivationNumber = _CommLog.IncidentNameActivationNumber;
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
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            openPicker.FileTypeFilter.Add(".xml");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file is null)
                return;

            _CommLog = CommLog.Open(file);
            FillFormFromCommLog();
        }

        private void FillCommLogFromForm()
        {
            _CommLog.IncidentNameActivationNumber = IncidentNameActivationNumber;
            _CommLog.OperationalPeriodFrom = OperationalPeriodStart;
            _CommLog.OperationalPeriodTo = OperationalPeriodEnd;
            //_CommLog.CommLogEntryList = CommLogEntryCollection = new ObservableCollection<CommLogEntry>();
            _CommLog.RadioNetName = RadioNetName;
            _CommLog.OperatorNameCallsign = OperatorNameCallsign;
            _CommLog.DateTimePrepared = DateTimePrepared;
        }

        private ICommand _SaveICS309Command;
        public ICommand SaveICS309Command => _SaveICS309Command ?? (_SaveICS309Command = new RelayCommand(SaveICS309));

        public async void SaveICS309()
        {
            FillCommLogFromForm();
            await _CommLog.SaveAsync();
        }

    }
}
