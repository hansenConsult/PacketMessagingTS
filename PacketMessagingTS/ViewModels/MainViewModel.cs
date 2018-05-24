using System;
using System.Collections.ObjectModel;
using PacketMessagingTS.Services;

using FormControlBaseClass;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;


namespace PacketMessagingTS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
        }

        ObservableCollection<PacketMessage> source;
        public ObservableCollection<PacketMessage> Source
        {
            get
            {
                return source;
                // TODO WTS: Replace this with your actual data
                //return SampleDataService.GetGridSampleData();
            }
            set => SetProperty(ref source, value);

        }

    }
}
