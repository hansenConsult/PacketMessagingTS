using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Views;

using SharedCode;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        
        public MainViewModel()
        {
            SelectedItems = new List<PacketMessage>();
        }

        //public async Task RefreshDataGridAsync()
        //{
        //    if (MainPagePivotSelectedItem is null)
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

        public void OpenMessage(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            string folder = ((StorageFolder)MainPagePivotSelectedItem.Tag).Path;
            string packetMessagePath = Path.Combine(folder, packetMessage.FileName);

            NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
        }

        //public void OpenMessageFromDoubleClick()
        //{
        //    if (SelectedItems != null && SelectedItems.Count == 1)
        //    {
        //        OpenMessage(SelectedItems[0]);
        //    }
        //}

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

        private int mainPagePivotSelectedIndex = Utilities.GetProperty("MainPagePivotSelectedIndex");
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

        public async Task UpdateDownloadedBulletinsAsync()
        {
            PacketSettingsViewModel packetSettingsViewModel = Singleton<PacketSettingsViewModel>.Instance;
            string[] areas = packetSettingsViewModel.AreaString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            BulletinHelpers.BulletinDictionary = new Dictionary<string, List<string>>();

            foreach (PivotItem pivotItem in MainPagePivot.Items)
            {
                if (pivotItem.Name == "pivotItemInBox" || pivotItem.Name == "pivotItemArchive")
                {
                    List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages(pivotItem.Tag as StorageFolder);
                    foreach (PacketMessage packetMessage in messagesInFolder)
                    {
                        foreach (string area in areas)
                        {
                            if (packetMessage.Area == area)
                            {
                                if (!BulletinHelpers.BulletinDictionary.TryGetValue(area, out List<string> bulletinList))
                                {
                                    BulletinHelpers.BulletinDictionary[area] = new List<string>();
                                }
                                BulletinHelpers.BulletinDictionary[area].Add(packetMessage.Subject);
                            }
                        }
                    }
                }
            }
            // Save lists
            BulletinHelpers.SaveBulletinDictionary(areas);
        }

    }
}
