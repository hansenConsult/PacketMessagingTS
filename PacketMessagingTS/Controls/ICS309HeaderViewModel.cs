using System;
using System.Linq;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

namespace PacketMessagingTS.Controls
{
    public class ICS309HeaderViewModel : ObservableObject
    {
        public static ICS309HeaderViewModel Instance { get; } = new ICS309HeaderViewModel();

        ICS309HeaderViewModel()
        {
            if (ICS309ViewModel.Instance.CommLog == null)
            {
                //ICS309ViewModel.Instance.InitializeAsync();
                InitializeCommLog();
            }
            OperationalPeriod = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            //RadioNetName = IdentityViewModel.Instance.UseTacticalCallsign ? $"{IdentityViewModel.Instance.TacticalCallsign}" : "";
            OperatorNameCallsign = ICS309ViewModel.Instance.CommLog.OperatorNameCallsign;
        }

        private async void InitializeCommLog()
        {
            await ICS309ViewModel.Instance.InitializeAsync();
        }

        private string _incidentName;
        public string IncidentName
        {
            get => _incidentName;
            set
            {
                ICS309ViewModel.Instance.CommLog.IncidentName = value;
                SetProperty(ref _incidentName, value);
            }
        }

        private string _activationNumber;
        public string ActivationNumber
        {
            get => _activationNumber;
            set
            {
                ICS309ViewModel.Instance.CommLog.ActivationNumber = value;
                SetProperty(ref _activationNumber, value);
            }
        }

        public DateTime OperationalPeriodStart
        {
            get => ICS309ViewModel.Instance.CommLog.OperationalPeriodFrom;
            set => ICS309ViewModel.Instance.CommLog.OperationalPeriodFrom = value;
        }

        private string _radioNetName;
        public string RadioNetName
        {
            get => _radioNetName;
            set
            {
                ICS309ViewModel.Instance.CommLog.RadioNetName = value;
                SetProperty(ref _radioNetName, value);
            }
        }

        public DateTime OperationalPeriodEnd
        {
            get => ICS309ViewModel.Instance.CommLog.OperationalPeriodTo;
            set => ICS309ViewModel.Instance.CommLog.OperationalPeriodTo = value;
        }

        private async void OperationalPeriod_TextChangedAsync(string operationalPeriod)
        {
            string opPeriod = operationalPeriod;
            string[] startStop = opPeriod.Split(new string[] { "to", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (startStop != null && startStop.Count() != 3 && startStop.Count() != 4)
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

            if (!DateTime.TryParse(dateTime, out DateTime operationalPeriodStart))
                return;

            if (startStop.Count() == 3)
            {
                dateTime = startStop[0] + " " + startStop[endTimeIndex];
            }
            else
            {
                dateTime = startStop[2] + " " + startStop[endTimeIndex];
            }

            if (!DateTime.TryParse(dateTime, out DateTime operationalPeriodEnd))
                return;

            if (operationalPeriodEnd < operationalPeriodStart)
                return;

            ICS309ViewModel.Instance.CommLog.OperationalPeriodFrom = operationalPeriodStart;
            ICS309ViewModel.Instance.CommLog.OperationalPeriodTo = operationalPeriodEnd;

            if (operationalPeriodEnd - operationalPeriodStart > new TimeSpan(0, 0, 0))
            {
                await ICS309ViewModel.Instance.BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
            }
        }

        private async void OperationalPeriod_TextChangedFromFileAsync(string operationalPeriod)
        {
            if (OperationalPeriodEnd - OperationalPeriodStart > new TimeSpan(0, 0, 0))
            {
                await ICS309ViewModel.Instance.BuildLogDataSetAsync(OperationalPeriodStart, OperationalPeriodEnd);
            }
        }

        private string _operationalPeriod;
        public string OperationalPeriod
        {
            //get => _operationalPeriod;
            get => $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            set
            {
                if (!ICS309ViewModel.Instance.FromOpenFile)
                {
                    OperationalPeriod_TextChangedAsync(value);
                }
                else
                {
                    OperationalPeriod_TextChangedFromFileAsync(value);
                }
                SetProperty(ref _operationalPeriod, $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}");
            }
        }

        private string _OperatorNameCallsign;
        public string OperatorNameCallsign
        {
            get => _OperatorNameCallsign;
            set => SetProperty(ref _OperatorNameCallsign, value);
        }

    }
}
