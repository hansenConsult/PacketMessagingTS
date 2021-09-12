using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Toolkit.Mvvm.ComponentModel;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;

namespace PacketMessagingTS.Controls
{
    public class ICS309HeaderViewModel : ObservableObject
    {
        public static ICS309HeaderViewModel Instance { get; } = new ICS309HeaderViewModel();

        ICS309HeaderViewModel()
        {
            OperationalPeriodStart = DateTime.Today;
            OperationalPeriodEnd = DateTime.Now;
            OperationalPeriod = $"{DateTimeStrings.DateTimeString(OperationalPeriodStart)} to {DateTimeStrings.DateTimeString(OperationalPeriodEnd)}";
            RadioNetName = IdentityViewModel.Instance.UseTacticalCallsign ? $"{IdentityViewModel.Instance.TacticalCallsign}" : "";
            OperatorNameCallsign = $"{IdentityViewModel.Instance.UserName}, {IdentityViewModel.Instance.UserCallsign}";
        }

        private string _incidentName;
        public string IncidentName
        {
            get => _incidentName;
            set => SetProperty(ref _incidentName, value);
        }

        private string _activationNumber;
        public string ActivationNumber
        {
            get => _activationNumber;
            set => SetProperty(ref _activationNumber, value);
        }

        private DateTime _operationalPeriodStart;
        public DateTime OperationalPeriodStart
        {
            get => _operationalPeriodStart;
            set => _operationalPeriodStart = value;
        }

        private string _radioNetName;
        public string RadioNetName
        {
            get => _radioNetName;
            set => SetProperty(ref _radioNetName, value);
        }

        private DateTime _operationalPeriodEnd;
        public DateTime OperationalPeriodEnd
        {
            get => _operationalPeriodEnd;
            set => _operationalPeriodEnd = value;
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

            OperationalPeriodStart = operationalPeriodStart;
            OperationalPeriodEnd = operationalPeriodEnd;

            if (operationalPeriodEnd - operationalPeriodStart > new TimeSpan(0, 0, 0))
            {
                await ICS309ViewModel.Instance.BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
            }
        }

        private string _operationalPeriod;
        public string OperationalPeriod
        {
            get => _operationalPeriod;
            set
            {
                if (!ICS309ViewModel.Instance.FromOpenFile)
                {
                    OperationalPeriod_TextChangedAsync(value);
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
