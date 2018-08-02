using System;
using System.Collections.Generic;
using FormControlBaseClass;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MetroLog;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using PacketMessagingTS.Services;
using SharedCode;
//using Telerik.UI.Xaml.Controls.Grid;

using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Linq;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        private ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

        public MainViewModel _mainViewModel { get; } = Singleton<MainViewModel>.Instance;

        List<PacketMessage> _messagesInFolder;
        List<PacketMessage> _selectedMessages = new List<PacketMessage>();
        PacketMessage _packetMessageClicked;

        //ObservableCollection<PacketMessage> _messageObservableCollection;
        //ObservableCollection<PacketMessage> messageFolderCollection;

        public MainPage()
        {
            InitializeComponent();

            _mainViewModel.MainPagePivot = MainPagePivot;

            foreach (PivotItem item in MainPagePivot.Items)
            {
                switch (item.Name)
                {
                    case "pivotItemInBox":
                        item.Tag = SharedData.ReceivedMessagesFolder;
                        dataGridInbox.Columns[1].SortDirection = DataGridSortDirection.Descending;
                        dataGridInbox.Tag = dataGridInbox.Columns[1];   // Default sort column
                        break;
                    case "pivotItemSent":
                        item.Tag = SharedData.SentMessagesFolder;
                        dataGridSent.Columns[1].SortDirection = DataGridSortDirection.Descending;
                        dataGridSent.Tag = dataGridSent.Columns[1];
                        break;
                    case "pivotItemOutBox":
                        item.Tag = SharedData.UnsentMessagesFolder;
                        dataGridOutbox.Columns[0].SortDirection = DataGridSortDirection.Descending;
                        dataGridOutbox.Tag = dataGridDrafts.Columns[0];
                        break;
                    case "pivotItemDrafts":
                        item.Tag = SharedData.DraftMessagesFolder;
                        dataGridDrafts.Columns[0].SortDirection = DataGridSortDirection.Descending;
                        dataGridDrafts.Tag = dataGridDrafts.Columns[0];
                        break;
                    case "pivotItemArchive":
                        item.Tag = SharedData.ArchivedMessagesFolder;
                        // No default sorting
                        break;
                    case "pivotItemDeleted":
                        item.Tag = SharedData.DeletedMessagesFolder;
                        // No default sorting
                        break;
                }
            }

        }

        private async Task RefreshDataGridAsync(PivotItem pivotItem)
        {
            _messagesInFolder = await PacketMessage.GetPacketMessages((StorageFolder)pivotItem.Tag);

            //ObservableCollection<PacketMessage> messageObservableCollection = new ObservableCollection<PacketMessage>(messagesInFolder);
            //_mainViewModel.Source = new ObservableCollection<PacketMessage>(_messagesInFolder);
            _mainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(_messagesInFolder);

            DataGridColumn sortColumn = null;
            switch (pivotItem.Name)
            {
                case "pivotItemInBox":
                    sortColumn = dataGridInbox.Tag as DataGridColumn;
                    break;
                case "pivotItemSent":
                    sortColumn = dataGridSent.Tag as DataGridColumn;
                    break;
                case "pivotItemOutBox":
                    sortColumn = dataGridOutbox.Tag as DataGridColumn;
                    break;
                case "pivotItemDrafts":
                    sortColumn = dataGridDrafts.Tag as DataGridColumn;
                    break;
                case "pivotItemArchive":
                    //dataGridArchived.ItemsSource = _messageObservableCollection;
                    break;
                case "pivotItemDeleted":
                    break;
            }
            if (sortColumn != null)
            {
                SortColumn(sortColumn);
            }

 
        }

        private async void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMessages.Clear();

            PivotItem pivotItem = (PivotItem)e.AddedItems[0];

            await RefreshDataGridAsync(pivotItem);
            //_mainViewModel.RefreshDataGridAsync(); // problem on startup because MainPagePivotSelectedItem is null
        }

        //private void DataGrid_SelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        //{
        //    foreach (PacketMessage packetMessage in e.AddedItems)
        //    {
        //        _selectedMessages.Add(packetMessage);
        //    }
        //    foreach (PacketMessage packetMessage in e.RemovedItems)
        //    {
        //        _selectedMessages.Remove(packetMessage);
        //    }
        //    _mainViewModel.SelectedItems = _selectedMessages;
        //    //ObservableCollection<object> selectedItemsCollection = ((RadDataGrid)sender).SelectedItems;
        //}

        private void OpenMessage()
        {
            if (_selectedMessages.Count == 1)
            {
                string folder = ((StorageFolder)((PivotItem)MainPagePivot.SelectedItem).Tag).Path;
                string packetMessagePath = Path.Combine(folder, _selectedMessages[0].FileName);

                NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
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
            communicationsService.BBSConnectAsync2();
        }

        private void AppBarMainPage_OpenMessage(object sender, RoutedEventArgs e)
        {
            _mainViewModel.OpenMessageFromDoubleClick();
        }

        //private void DataGridInbox_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        //{
        //    var physicalPoint = e.GetCurrentPoint(sender as RadDataGrid);
        //    var point = new Point { X = physicalPoint.Position.X, Y = physicalPoint.Position.Y };
        //    _packetMessageClicked = (sender as RadDataGrid).HitTestService.RowItemFromPoint(point) as PacketMessage;
        //}

        private async void AppBarMailPage_MoveToArchiveAsync(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)MainPagePivot.SelectedItem;
            StorageFolder folder = (StorageFolder)pivotItem.Tag;

            var file = await folder.CreateFileAsync(_packetMessageClicked.FileName, CreationCollisionOption.OpenIfExists);

            await file?.MoveAsync(SharedData.ArchivedMessagesFolder);

            await RefreshDataGridAsync(pivotItem);
        }

        private void AppBarMainPage_OpenMessageFromContectMenu(object sender, RoutedEventArgs e)
        {
            //DataGrid dataGrid = ((UIElement)sender).p
            if (_packetMessageClicked == null && _mainViewModel.SelectedItems != null)
            {
                _packetMessageClicked = _mainViewModel.SelectedItems[0];
            }
            _mainViewModel.OpenMessageFromDoubleClick(_packetMessageClicked);
        }

        private async void AppBarMainPage_DeleteItemFromContectMenuAsync(object sender, RoutedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)MainPagePivot.SelectedItem;
            StorageFolder folder = (StorageFolder)pivotItem.Tag;
            bool permanentlyDelete = false;
            if (folder == SharedData.DeletedMessagesFolder)
            {
                permanentlyDelete = true;
            }
            try
            {
                var file = await folder.CreateFileAsync(_packetMessageClicked.FileName, CreationCollisionOption.OpenIfExists);
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
                var file = await folder.CreateFileAsync(_packetMessageClicked.FileName, CreationCollisionOption.OpenIfExists);
                await file?.DeleteAsync();

                string s = ex.ToString();
            }
            await RefreshDataGridAsync(pivotItem);
        }

        public object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        private void SortColumn(DataGridColumn column)
        {
            IOrderedEnumerable<PacketMessage> sortedItems = null;
            if (column.SortDirection == DataGridSortDirection.Ascending)
            {
                sortedItems = from item in _messagesInFolder orderby GetDynamicSortProperty(item, column.Tag.ToString()) ascending select item;
            }
            else
            {
                sortedItems = from item in _messagesInFolder orderby GetDynamicSortProperty(item, column.Tag.ToString()) descending select item;
            }
            _mainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(sortedItems);
        }

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if ((((DataGrid)sender).Tag as DataGridColumn).Tag == e.Column.Tag) // Sorting on same column
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                else
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            }

            SortColumn(e.Column);

            // If sort column has changed remove the sort icon from the previous column
            if ((((DataGrid)sender).Tag as DataGridColumn).Tag != e.Column.Tag)
            {
                (((DataGrid)sender).Tag as DataGridColumn).SortDirection = null;
            }
            ((DataGrid)sender).Tag = e.Column;
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            foreach (PacketMessage packetMessage in e.AddedItems)
            {
                _selectedMessages.Add(packetMessage);
            }
            foreach (PacketMessage packetMessage in e.RemovedItems)
            {
                _selectedMessages.Remove(packetMessage);
            }
            _mainViewModel.SelectedItems = _selectedMessages;
        }

        private void draftsDataGrid_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            _mainViewModel.OpenMessageFromDoubleClick(((DataGrid)sender).SelectedItem as PacketMessage);
        }


    }
}
