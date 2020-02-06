using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS309FormControl
{
    [FormControl(
    FormControlName = "form-ics309",
    FormControlMenuName = "ICS-409",
    FormControlType = FormControlAttribute.FormType.TestForm)
]

    public sealed partial class ICS309Control : FormControlBase
    {


        //List<PacketMessage> _messageList;
        //CommLog _commLog;

        //public DateTime OperationalPeriodStart
        //{ get; set; }

        //public DateTime OperationalPeriodEnd
        //{ get; set; }

        //public string IncidentName
        //{
        //    get => incidentName.Text;
        //    set => incidentName.Text = value;
        //}

        //public string RadioNetName 
        //{
        //    get => radioNetName.Text;
        //    set => radioNetName.Text = value;
        //}

        //public int TotalPages
        //{ get; set; }

        //public int PageNo
        //{ get; set; }

        //public string PageNoOf
        //{
        //    set
        //    {
        //        pageNoOf.Text = $"Page {value} of {TotalPages.ToString()}"; ;
        //    }
        //}


        public ICS309Control()
        {
            this.InitializeComponent();

            //_commLog = new CommLog();

            //operationalPeriod.Text = FormatDateTime(_toolsViewModel.OperationalPeriodStart) + " to " + FormatDateTime(_toolsViewModel.OperationalPeriodEnd);
            //radioOperator.Text = $"{Singleton<IdentityViewModel>.Instance.UserName}, {Singleton<IdentityViewModel>.Instance.UserCallsign}";
            //_toolsViewModel.DateTimePrepared = DateTime.Now;
            //dateTimePrepared.Text = FormatDateTime(_toolsViewModel.DateTimePrepared);
            //preparedByNameCallsign.Text = radioOperator.Text;

            //await BuildLogDataSetAsync(_toolsViewModel.OperationalPeriodStart, _toolsViewModel.OperationalPeriodEnd);

        }


        public override FormProviders FormProvider => FormProviders.PacForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string PacFormName => "form-ics213";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override void AppendDrillTraffic()
        {            
        }

        public override string CreateSubject()
        {
            return ($"");
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            return null;
        }


        private async void OperationalPeriod_TextChangedAsync(object sender, TextChangedEventArgs e)
        {
            string opPeriod = operationalPeriod.Text;
            var startStop = opPeriod.Split(new string[] { "to", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (startStop != null && (startStop.Count() != 3 && startStop.Count() != 4))
                return;

            int endTimeIndex = 3;
            if (startStop.Count() == 3)
            {
                endTimeIndex = 2;
            }

            if (startStop[1].Length != 4)
                return;

            if (startStop[endTimeIndex].Length != 4)
                return;

            string dateTime = startStop[0] + " " + startStop[1].Insert(2, ":");

            DateTime operationalPeriodStart;
            if (!DateTime.TryParse(dateTime, out operationalPeriodStart))
                return;

            if (startStop.Count() == 3)
            {
                dateTime = startStop[0] + " " + startStop[endTimeIndex].Insert(2, ":");
            }
            else
            {
                dateTime = startStop[2] + " " + startStop[endTimeIndex].Insert(2, ":");
            }

            DateTime operationalPeriodEnd;
            if (!DateTime.TryParse(dateTime, out operationalPeriodEnd))
                return;

            if (operationalPeriodEnd < operationalPeriodStart)
                return;

            _toolsViewModel.OperationalPeriodStart = operationalPeriodStart;
            _toolsViewModel.OperationalPeriodEnd = operationalPeriodEnd;

            if (operationalPeriodEnd - operationalPeriodStart > new TimeSpan(0, 0, 0))
            {
                await BuildLogDataSetAsync(operationalPeriodStart, operationalPeriodEnd);
            }
        }

        private void IncidentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            IncidentName = incidentName.Text;
        }

        private void RadioNetName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RadioNetName = radioNetName.Text;
        }

        public void AddToMessageList(List<PacketMessage> messageList, DateTime startTime, DateTime endTime)
        {
            _messageList.AddRange(messageList);
            BuildLogDataSet(startTime, endTime);
        }

        private object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        private List<T> Sort_List<T>(List<T> data)
        {
            List<T> data_sorted = new List<T>();

            data_sorted = (from n in data
                           orderby GetDynamicSortProperty(n, "Time") ascending
                           select n).ToList();
            return data_sorted;
        }

        private void BuildLogDataSet(DateTime startTime, DateTime endTime)
        {
            _commLog.CommLogEntryList.Clear();

            foreach (PacketMessage packetMessage in _messageList)
            {
                _commLog.AddCommLogEntry(packetMessage, startTime, endTime);
            }
            List<CommLogEntry> sortedList = Sort_List(_commLog.CommLogEntryList);

            CommLogMessagesCollection.Source = new ObservableCollection<CommLogEntry>(sortedList);
        }

    }
}
