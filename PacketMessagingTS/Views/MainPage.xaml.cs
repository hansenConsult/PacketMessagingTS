﻿using System;
using System.Collections.Generic;
using FormControlBaseClass;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using MetroLog;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using PacketMessagingTS.Services;
using SharedCode;

using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Threading;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        public MainViewModel _mainViewModel { get; } = Singleton<MainViewModel>.Instance;

        private readonly object _lock = new object();
        PivotItem _currentPivotItem;

        //List<string> _bulletinList;
        List<PacketMessage> _messagesInFolder;
        List<PacketMessage> _selectedMessages = new List<PacketMessage>();
        PacketMessage _packetMessageRightClicked;


        public MainPage()
        {
            InitializeComponent();

            _mainViewModel.MainPagePivot = MainPagePivot;

            foreach (PivotItem item in MainPagePivot.Items)
            {
                int sortColumn = -1;
                DataGridSortDirection? sortDirection = DataGridSortDirection.Descending;
                switch (item.Name)
                {           
                    case "pivotItemInBox":
                        item.Tag = SharedData.ReceivedMessagesFolder;
                        sortColumn = 1;
                        dataGridInbox.Columns[sortColumn].SortDirection = sortDirection;
                        break;
                    case "pivotItemSent":
                        item.Tag = SharedData.SentMessagesFolder;
                        sortColumn = 1;
                        dataGridSent.Columns[sortColumn].SortDirection = sortDirection;
                        break;
                    case "pivotItemOutBox":
                        item.Tag = SharedData.UnsentMessagesFolder;
                        sortColumn = 0;
                        dataGridOutbox.Columns[sortColumn].SortDirection = sortDirection;
                        break;
                    case "pivotItemDrafts":
                        item.Tag = SharedData.DraftMessagesFolder;
                        sortColumn = 0;
                        dataGridDrafts.Columns[sortColumn].SortDirection = sortDirection;
                        break;
                    case "pivotItemArchive":
                        item.Tag = SharedData.ArchivedMessagesFolder;
                        sortDirection = null;   // No default sorting
                        break;
                    case "pivotItemDeleted":
                        item.Tag = SharedData.DeletedMessagesFolder;
                        sortDirection = null;   // No default sorting
                        break;
                }
                bool success = DataGridSortData.DataGridSortDataDictionary.TryAdd(item.Name, new DataGridSortData(item.Name, sortColumn, sortDirection));
            }

        }

        public DataGrid FindDataGrid(DependencyObject panelName)
        {
            DataGrid dataGrid = null;

            var count = VisualTreeHelper.GetChildrenCount(panelName);
            DependencyObject control = VisualTreeHelper.GetChild(panelName, 0);

            count = VisualTreeHelper.GetChildrenCount(control);
            control = VisualTreeHelper.GetChild(control, 0);

            count = VisualTreeHelper.GetChildrenCount(control);
            control = VisualTreeHelper.GetChild(control, 0);

            if (control is DataGrid)
            {
                dataGrid = control as DataGrid;
            }
            return dataGrid;
        }

        private async Task RefreshDataGridAsync()
        {
            _messagesInFolder = await PacketMessage.GetPacketMessages(_currentPivotItem.Tag as StorageFolder);

            _mainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(_messagesInFolder);

            DataGridColumn sortColumn = null;
            DataGrid dataGrid = FindDataGrid(_currentPivotItem);
            int sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[_currentPivotItem.Name].SortColumnNumber;
            if (sortColumnNumber < 0)
                return;

            sortColumn = dataGrid.Columns[sortColumnNumber];
            SortColumn(sortColumn);
        }

        private async void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMessages.Clear();

            _currentPivotItem = (PivotItem)e.AddedItems[0];

            await RefreshDataGridAsync();
            //_mainViewModel.RefreshDataGridAsync(); // problem on startup because MainPagePivotSelectedItem is null
        }

        private void OpenMessage(PacketMessage packetMessage)
        {
            string folder = (((PivotItem)MainPagePivot.SelectedItem).Tag as StorageFolder).Path;
            string packetMessagePath = Path.Combine(folder, packetMessage.FileName);

            NavigationService.Navigate(typeof(FormsPage), packetMessagePath);
        }

        private async Task DeleteMessageAsync(PacketMessage packetMessage)
        {
            StorageFolder folder = _currentPivotItem.Tag as StorageFolder;
            bool permanentlyDelete = false;
            if (folder == SharedData.DeletedMessagesFolder)
            {
                permanentlyDelete = true;
            }

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
            }
        }

        private async void AppBarMainPage_DeleteItemAsync(object sender, RoutedEventArgs e)
        {
            foreach (PacketMessage packetMessage in _selectedMessages)
            {
                await DeleteMessageAsync(packetMessage);
            }
            await RefreshDataGridAsync();
        }

        private async void AppBarMainPage_SendReceiveAsync(object sender, RoutedEventArgs e)
        {
            Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            communicationsService.BBSConnectAsync2();

            await RefreshDataGridAsync();
        }

        private void AppBarMainPage_OpenMessage(object sender, RoutedEventArgs e)
        {
            if (_selectedMessages != null && _selectedMessages.Count == 1)
            {
                OpenMessage(_selectedMessages[0]);
            }
        }

        private async void AppBarMainPage_MoveToArchiveAsync(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = (StorageFolder)_currentPivotItem.Tag;

            //if (_mainViewModel.SelectedItems.Count > 1)
            //{
                foreach (PacketMessage packetMessage in _mainViewModel.SelectedItems)
                {
                    var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                    await file?.MoveAsync(SharedData.ArchivedMessagesFolder);
                }
            //}
            //else
            //{
            //    var file = await folder.CreateFileAsync(_packetMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
            //    await file?.MoveAsync(SharedData.ArchivedMessagesFolder);
            //}

             await RefreshDataGridAsync();
        }

        private void AppBarMainPage_OpenMessageFromContectMenu(object sender, RoutedEventArgs e)
        {
            if (_packetMessageRightClicked == null)
                return;

            OpenMessage(_packetMessageRightClicked);
        }

        private async void AppBarMainPage_DeleteItemFromContectMenuAsync(object sender, RoutedEventArgs e)
        {
            await DeleteMessageAsync(_packetMessageRightClicked);

            await RefreshDataGridAsync();
        }

        private void AppBarMainPage_OpenInWebView(object sender, RoutedEventArgs e)
        {
            try
            {
                string folder = ((StorageFolder)((PivotItem)MainPagePivot.SelectedItem).Tag).Path;
                string packetMessagePath = Path.Combine(folder, _packetMessageRightClicked.FileName);

                NavigationService.Navigate(typeof(WebViewPage), packetMessagePath);
            }
            catch
            {
                return;
            }

        }

        public object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        private void SortColumn(DataGridColumn column)
        {
            if (column.SortDirection is null)
                return;

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

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            int sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[_currentPivotItem.Name].SortColumnNumber;
            if (sortColumnNumber < 0)
            {
                // There is no default sorting column for this data grid. Select current column.
                sortColumnNumber = e.Column.DisplayIndex;
            }
            if ((sender as DataGrid).Columns[sortColumnNumber].Header == e.Column.Header) // Sorting on same column, switch SortDirection
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                else
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                // Sorting on a new column. Use that columns SortDirection
                e.Column.SortDirection = DataGridSortData.DataGridSortDataDictionary[_currentPivotItem.Name].SortDirection;
            }

            SortColumn(e.Column);

            // If sort column has changed remove the sort icon from the previous column
            if ((sender as DataGrid).Columns[sortColumnNumber].Header != e.Column.Header)
            {
                (sender as DataGrid).Columns[sortColumnNumber].SortDirection = null;
            }
            DataGridSortData.DataGridSortDataDictionary[_currentPivotItem.Name].SortColumnNumber = e.Column.DisplayIndex;
            DataGridSortData.DataGridSortDataDictionary[_currentPivotItem.Name].SortDirection = e.Column.SortDirection;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                TextBlock grid = e.OriginalSource as TextBlock;
                PacketMessage pktmsg = (e.OriginalSource as TextBlock).DataContext as PacketMessage;
                _mainViewModel.OpenMessageFromDoubleClick(pktmsg);
            }
            catch
            {
                return;
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PacketMessage packetMesage = e.Row.DataContext as PacketMessage;

            if (!(bool)packetMesage?.MessageOpened)
            {
                e.Row.Background = new SolidColorBrush(Colors.BlanchedAlmond);
            }
        }

        private void DataGrid_UnloadingRow(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowEventArgs e)
        {
            e.Row.Background = new SolidColorBrush(Colors.White);
        }

        private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                _packetMessageRightClicked = (e.OriginalSource as TextBlock).DataContext as PacketMessage;
            }
            catch
            {
                return;
            }
        }

    }
}
