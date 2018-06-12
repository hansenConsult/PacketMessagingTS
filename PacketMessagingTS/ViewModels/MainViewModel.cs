using System;
using System.Collections.ObjectModel;
using PacketMessagingTS.Services;

using FormControlBaseClass;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using System.IO;
using PacketMessagingTS.Views;

namespace PacketMessagingTS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        
        public MainViewModel()
        {
        }

        public async Task RefreshDataGridAsync()
        {
            if (MainPagePivotSelectedItem == null)
                return;

            List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages((StorageFolder)MainPagePivotSelectedItem.Tag);
            //var task = Task<List<PacketMessage>>.Run(async () => await PacketMessage.GetPacketMessages((StorageFolder)MainPagePivotSelectedItem.Tag));
            //task.Wait();
            //List<PacketMessage> messagesInFolder = task.Result;

            Source = new ObservableCollection<PacketMessage>(messagesInFolder);
        }

        public void OpenMessageFromDoubleClick()
        {
            //PivotItem pivotItem = MainPagePivot.Items[_mainViewModel.PivotSelectedIndex] as PivotItem;
            //string folder = ((StorageFolder)((PivotItem)MainPagePivot.SelectedItem).Tag).Path;
            //PivotItem pivotItem = (MainPagePivot.Items[_mainViewModel.MainPagePivotSelectedIndex] as PivotItem);
            string folder = ((StorageFolder)MainPagePivotSelectedItem.Tag).Path;
            //string packetMessagePath = Path.Combine(folder, packetMessage.FileName);
            string packetMessagePath = Path.Combine(folder, SelectedMessage.FileName);

            NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
        }

        public Pivot MainPagePivot { get; set; }

        private ObservableCollection<PacketMessage> source;
        public ObservableCollection<PacketMessage> Source
        {
            get => source;
            set => SetProperty(ref source, value);
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

        private PacketMessage selectedMessage;
        public PacketMessage SelectedMessage
        {
            get => selectedMessage;
            set => SetProperty(ref selectedMessage, value);
        }
    }
}
