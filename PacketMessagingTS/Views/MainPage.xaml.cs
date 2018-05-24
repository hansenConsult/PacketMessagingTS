using System;
using System.Collections.Generic;
using FormControlBaseClass;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MetroLog;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using PacketMessagingTS.Services;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        private ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

        public MainViewModel _mainViewModel { get; } = new MainViewModel();

        List<PacketMessage> _selectedMessages = new List<PacketMessage>();

        public MainPage()
        {
            InitializeComponent();

            foreach (PivotItem item in MainPagePivot.Items)
            {
                switch (item.Name)
                {
                    case "pivotItemInBox":
                        item.Tag = SharedData.ReceivedMessagesFolder;
                        break;
                    case "pivotItemSent":
                        item.Tag = SharedData.SentMessagesFolder;
                        break;
                    case "pivotItemOutBox":
                        item.Tag = SharedData.UnsentMessagesFolder;
                        break;
                    case "pivotItemDrafts":
                        item.Tag = SharedData.DraftMessagesFolder;
                        break;
                    case "pivotItemArchive":
                        item.Tag = SharedData.ArchivedMessagesFolder;
                        break;
                    case "pivotItemDeleted":
                        item.Tag = SharedData.DeletedMessagesFolder;
                        break;
                }
            }

        }

        private async Task RefreshDataGridAsync(PivotItem pivotItem)
        {
            List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages((StorageFolder)pivotItem.Tag);

            //_mainViewModel.Source = new ObservableCollection<PacketMessage>(messagesInFolder);
            switch (pivotItem.Name)
            {
                case "pivotItemInBox":
                    dataGridInbox.ItemsSource = messagesInFolder;
                    break;
                case "pivotItemSent":
                    dataGridSent.ItemsSource = messagesInFolder;
                    break;
                case "pivotItemOutBox":
                    dataGridOutbox.ItemsSource = messagesInFolder;
                    break;
                case "pivotItemDrafts":
                    dataGridDrafts.ItemsSource = messagesInFolder;
                    break;
                case "pivotItemArchive":
                    dataGridArchived.ItemsSource = messagesInFolder;
                    break;
                case "pivotItemDeleted":
                    dataGridDeleted.ItemsSource = messagesInFolder;
                    break;
            }
        }

        private async void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMessages.Clear();

            PivotItem pivotItem = (PivotItem)e.AddedItems[0];

            await RefreshDataGridAsync(pivotItem);
        }

        //private void DataGrid_DoubleClicked(object sender, )
        //{

        //}

        private void DataGrid_SelectionChanged(object sender, Telerik.UI.Xaml.Controls.Grid.DataGridSelectionChangedEventArgs e)
        {
            foreach (PacketMessage packetMessage in e.RemovedItems)
            {
                _selectedMessages.Remove(packetMessage);
            }
            //_selectedMessages = e.AddedItems as List<PacketMessage>;
            foreach (PacketMessage packetMessage in e.AddedItems)
            {
                _selectedMessages.Add(packetMessage);
            }
        }

        private void OpenMessage()
        {
            if (_selectedMessages.Count == 1)
            {
                string folder = ((StorageFolder)((PivotItem)MainPagePivot.SelectedItem).Tag).Path;
                //string packetMessagePath = folder + @"\" + _selectedMessages[0].FileName;
                string packetMessagePath = Path.Combine(folder, _selectedMessages[0].FileName);

                NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
                //var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();
                //nav.Navigate(typeof(FormsPage), packetMessagePath);
            }
        }

        private async void AppBarMainPage_DeleteItemAsync(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)MainPagePivot.SelectedItem;
            StorageFolder folder = (StorageFolder)pivotItem.Tag;
            bool permanentlyDelete = false;
            if (folder == SharedData.DeletedMessagesFolder)
            {
                permanentlyDelete = true;
            }

            //IList<Object> selectedMessages = null;
            //     selectedMessages = _currentListView.SelectedItems;

            foreach (PacketMessage packetMessage in _selectedMessages)
            {
                try
                {
                    var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                    if (permanentlyDelete)
                    {
                        await file?.DeleteAsync();
                    }
                    else
                    {
                        await file?.MoveAsync(SharedData.DeletedMessagesFolder);
                    }
                }
                catch (Exception ex)
                {
                    var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                    await file?.DeleteAsync();

                    string s = ex.ToString();
                    continue;
                }
            }
            await RefreshDataGridAsync(pivotItem);
        }

        private void AppBarMainPage_SendReceive(object sender, RoutedEventArgs e)
        {
            Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            communicationsService.BBSConnectAsync();
        }

        private void AppBarMainPage_OpenMessage(object sender, RoutedEventArgs e)
        {
            OpenMessage();
        }

        private void DataGrid_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            OpenMessage();
        }
    }
}
