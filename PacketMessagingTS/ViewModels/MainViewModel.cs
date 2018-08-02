using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using System.IO;

using SharedCode;
using PacketMessagingTS.Views;
using PacketMessagingTS.Services;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;


namespace PacketMessagingTS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        
        public MainViewModel()
        {
        }

        //public async Task RefreshDataGridAsync()
        //{
        //    if (MainPagePivotSelectedItem == null)
        //        return;

        //    List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages((StorageFolder)MainPagePivotSelectedItem.Tag);
        //    //var task = Task<List<PacketMessage>>.Run(async () => await PacketMessage.GetPacketMessages((StorageFolder)MainPagePivotSelectedItem.Tag));
        //    //task.Wait();
        //    //List<PacketMessage> messagesInFolder = task.Result;

        //    switch (MainPagePivotSelectedItem.Name)
        //    {
        //        case "":
        //            DraftsSource = new ObservableCollection<PacketMessage>(messagesInFolder);
        //            break;
        //        default:
        //            Source = new ObservableCollection<PacketMessage>(messagesInFolder);
        //            break;
        //    }
        //}

        public void OpenMessageFromDoubleClick(PacketMessage packetMessage)
        {
            if (packetMessage == null)
                return;

            string folder = ((StorageFolder)MainPagePivotSelectedItem.Tag).Path;
            string packetMessagePath = Path.Combine(folder, packetMessage.FileName);

            NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
        }

        public void OpenMessageFromDoubleClick()
        {
            if (SelectedItems != null && SelectedItems.Count == 1)
            {
                OpenMessageFromDoubleClick(SelectedItems[0]);
            }
        }

        public Pivot MainPagePivot { get; set; }

        private ObservableCollection<PacketMessage> source;
        public ObservableCollection<PacketMessage> Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }

        private ObservableCollection<PacketMessage> dataGridSource;
        public ObservableCollection<PacketMessage> DataGridSource
        {
            get => dataGridSource;
            set => SetProperty(ref dataGridSource, value);
        }

        private int mainPagePivotSelectedIndex;
        public int MainPagePivotSelectedIndex
        {
            get => GetProperty(ref mainPagePivotSelectedIndex);
            set
            {
                SetProperty(ref mainPagePivotSelectedIndex, value, true);

                MainPagePivotSelectedItem = MainPagePivot.Items[mainPagePivotSelectedIndex] as PivotItem;
            }
        }
        public PivotItem MainPagePivotSelectedItem { get; set; }

        public IList<PacketMessage> SelectedItems { get; set; }


    }
}
