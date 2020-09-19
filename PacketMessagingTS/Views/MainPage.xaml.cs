using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MetroLog;

using Microsoft.Toolkit.Uwp.UI.Controls;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        public MainViewModel MainViewModel { get; } = Singleton<MainViewModel>.Instance;

        public static MainPage Current;

        //List<string> _bulletinList;


        public MainPage()
        {
            InitializeComponent();

            Current = this;
            MainViewModel.MainPagePivot = mainPagePivot;

            foreach (PivotItem item in mainPagePivot.Items)
            {
                int sortColumn = -1;
                DataGridSortDirection? sortDirection = DataGridSortDirection.Descending;
                switch (item.Name)
                {           
                    case "pivotItemInBox":
                        item.Tag = SharedData.ReceivedMessagesFolder;
                        sortColumn = 1;
                        dataGridInBox.Columns[sortColumn].SortDirection = sortDirection;
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
                    case "pivotItemPrint":
                        item.Tag = SharedData.PrintMessagesFolder;
                        sortDirection = null;   // No default sorting
                        break;
                }
                bool success = DataGridSortData.DataGridSortDataDictionary.TryAdd(item.Name, new DataGridSortData(item.Name, sortColumn, sortDirection));
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            List<PacketMessage> messages;

            foreach (PivotItem item in mainPagePivot.Items)
            {
                switch (item.Name)
                {
                    //case "pivotItemInBox":
                    //    messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                    //    MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                    //    break;
                    //case "pivotItemSent":
                    //    messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                    //    MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                    //    break;
                    case "pivotItemOutBox":
                        messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                        MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                        break;
                        //case "pivotItemDrafts":
                        //    messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                        //    MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                        //    break;
                        //case "pivotItemArchive":
                        //    messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                        //    MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                        //    break;
                        //case "pivotItemDeleted":
                        //    messages = await PacketMessage.GetPacketMessages(item.Tag as StorageFolder);
                        //    MainViewModel.UpdateHeaderMessageCount(item, messages.Count);
                        //    break;
                }
            }
            base.OnNavigatedTo(e);
        }

        //private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        //{
        //    int count = VisualTreeHelper.GetChildrenCount(obj);
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is T)
        //            return (T)child;
        //        else
        //        {
        //            T childOfChild = FindVisualChild<T>(child);
        //            if (childOfChild != null)
        //                return childOfChild;
        //        }
        //    }
        //    return null;
        //}

        //private void FillMoveLocations()
        //{
        //    var dataGrid = MainViewModel.FindDataGrid(MainViewModel.MainPagePivotSelectedItem);
        //    string menuFlyoutSubItemName = "moveMenu" + dataGrid.Name.Substring(8);
        //    MenuFlyoutSubItem moveSubMenu = dataGrid.FindName(menuFlyoutSubItemName) as MenuFlyoutSubItem;
        //    if (moveSubMenu is null)
        //        return;

        //    int itemsCount = moveSubMenu.Items.Count;
        //    for (int i = 0; i < itemsCount; i++)
        //    {
        //        if (moveSubMenu.Items[i] != null && (moveSubMenu.Items[i] as MenuFlyoutItem).Text.Contains("Archive"))
        //        {
        //            continue;
        //        }
        //        moveSubMenu.Items.Remove(moveSubMenu.Items[i]);
        //    }

        //    foreach (TabViewItemData tabView in CustomFoldersArray.Instance.CustomFolderList)
        //    {
        //        MenuFlyoutItem newMenuItem = new MenuFlyoutItem();
        //        newMenuItem.Text = tabView.Folder;
        //        //newMenuItem.Command = MainViewModel.MoveToFolderFromContextMenuCommand;
        //        newMenuItem.Click += OnMoveToFolderFromContextMenuCommand;      // To get the folder name
        //        moveSubMenu.Items.Add(newMenuItem);
        //    }
        //}

        //public DataGrid FindDataGrid(DependencyObject panelName)
        //{
        //    DataGrid dataGrid = null;

        //    var count = VisualTreeHelper.GetChildrenCount(panelName);
        //    DependencyObject control = VisualTreeHelper.GetChild(panelName, 0);

        //    count = VisualTreeHelper.GetChildrenCount(control);
        //    control = VisualTreeHelper.GetChild(control, 0);

        //    count = VisualTreeHelper.GetChildrenCount(control);
        //    control = VisualTreeHelper.GetChild(control, 0);

        //    if (control is DataGrid)
        //    {
        //        dataGrid = control as DataGrid;
        //    }
        //    return dataGrid;
        //}

        //private async Task RefreshDataGridAsync()
        //{
        //    try
        //    {
        //        MainViewModel._messagesInFolder = await PacketMessage.GetPacketMessages(MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder);

        //        MainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(MainViewModel._messagesInFolder);

        //        DataGrid dataGrid = MainViewModel.FindDataGrid(MainViewModel.MainPagePivotSelectedItem);
        //        int? sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber;
        //        if (sortColumnNumber == null || sortColumnNumber < 0)
        //            return;

        //        DataGridColumn sortColumn = dataGrid.Columns[(int)sortColumnNumber];
        //        MainViewModel.SortColumn(sortColumn);
        //    }
        //    catch (Exception e)
        //    {
        //        _logHelper.Log(LogLevel.Error, $"{e.Message}");
        //    }
        //}

        //private void OnMoveToFolderFromContextMenuCommand(object sender, RoutedEventArgs e)
        //{
        //    string folder = (sender as MenuFlyoutItem).Text;
        //    MainViewModel.MoveToFolderFromContextMenuCommand.Execute(folder);
        //}

        //private void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    MainViewModel.MainPagePivotSelectedItem = (PivotItem)e.AddedItems[0];
        //    //MainViewModel.PageDataGrid = MainViewModel.FindDataGrid((PivotItem)e.AddedItems[0]);
        //}


        //private async Task DeleteMessageAsync(PacketMessage packetMessage)
        //{
        //    if (packetMessage is null)
        //        return;

        //    StorageFolder folder = MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder;
        //    bool permanentlyDelete = false;
        //    if (folder == SharedData.DeletedMessagesFolder)
        //    {
        //        permanentlyDelete = true;
        //    }

        //    try
        //    {
        //        var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
        //        if (permanentlyDelete)
        //        {
        //            await file?.DeleteAsync();
        //        }
        //        else
        //        {
        //            await file?.MoveAsync(SharedData.DeletedMessagesFolder);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
        //        await file?.DeleteAsync();

        //        string s = ex.ToString();
        //    }
        //}

        //private async void AppBarMainPage_DeleteItemAsync(object sender, RoutedEventArgs e)
        //{
        //    foreach (PacketMessage packetMessage in _selectedMessages)
        //    {
        //        await DeleteMessageAsync(packetMessage);
        //    }
        //    await RefreshDataGridAsync();
        //}

        //private async void AppBarMainPage_SendReceiveAsync(object sender, RoutedEventArgs e)
        //{
        //    //// Using ViewLifetimeControl
        //    //await WindowManagerService.Current.TryShowAsStandaloneAsync("Connection Status", typeof(RxTxStatusPage));

        //    CommunicationsService.CreateInstance().BBSConnectAsync2();
        //    //communicationsService.BBSConnectAsync2(Dispatcher);

        //    await RefreshDataGridAsync();
        //}

        //private void AppBarMainPage_OpenMessage(object sender, RoutedEventArgs e)
        //{
        //     if (_selectedMessages != null && _selectedMessages.Count == 1)
        //    {
        //        MainViewModel.OpenMessage(_selectedMessages[0]);
        //    }
        //}

        //private async void AppBarMainPage_MoveToArchiveAsync(object sender, RoutedEventArgs e)
        //{
        //    StorageFolder folder = MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder;

        //    foreach (PacketMessage packetMessage in MainViewModel.SelectedMessages)
        //    {
        //        var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
        //        await file?.MoveAsync(SharedData.ArchivedMessagesFolder);
        //    }

        //     await RefreshDataGridAsync();
        //}

        //private void AppBarMainPage_OpenMessageFromContectMenu(object sender, RoutedEventArgs e)
        //{
        //    MainViewModel.OpenMessage(MainViewModel.PacketMessageRightClicked);
        //}

        //private async void AppBarMainPage_DeleteItemFromContectMenuAsync(object sender, RoutedEventArgs e)
        //{
        //    await MainViewModel.DeleteMessageAsync(MainViewModel.PacketMessageRightClicked);

        //    await RefreshDataGridAsync();
        //}

        private void AppBarMainPage_OpenInWebView(object sender, RoutedEventArgs e)
        {
            try
            {
                string folder = ((StorageFolder)((PivotItem)mainPagePivot.SelectedItem).Tag).Path;
                string packetMessagePath = Path.Combine(folder, MainViewModel.PacketMessageRightClicked.FileName);

                NavigationService.Navigate(typeof(WebViewPage), packetMessagePath);
            }
            catch
            {
                return;
            }
        }

        private void AppBarMainPage_TestStatusPage(object sender, RoutedEventArgs e)
        {
            //i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;

            //Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = $"\nTest text{i}";

            //CommunicationsService communicationsService = CommunicationsService.CreateInstance();
            //communicationsService.AddRxTxStatusAsync($"\nTest text{i}");
        }

        //public object GetDynamicSortProperty(object item, string propName)
        //{
        //    //Use reflection to get order type
        //    return item.GetType().GetProperty(propName).GetValue(item);
        //}

        //private void SortColumn(DataGridColumn column)
        //{
        //    if (column.SortDirection is null)
        //        return;

        //    IOrderedEnumerable<PacketMessage> sortedItems = null;
        //    if (column.SortDirection == DataGridSortDirection.Ascending)
        //    {
        //        sortedItems = from item in _messagesInFolder orderby GetDynamicSortProperty(item, column.Tag.ToString()) ascending select item;
        //    }
        //    else
        //    {
        //        sortedItems = from item in _messagesInFolder orderby GetDynamicSortProperty(item, column.Tag.ToString()) descending select item;
        //    }
        //    MainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(sortedItems);
        //}

        //private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        //{
        //    int sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber;
        //    if (sortColumnNumber < 0)
        //    {
        //        // There is no default sorting column for this data grid. Select current column.
        //        sortColumnNumber = e.Column.DisplayIndex;
        //    }
        //    if ((sender as DataGrid).Columns[sortColumnNumber].Header == e.Column.Header) // Sorting on same column, switch SortDirection
        //    {
        //        if (e.Column.SortDirection == DataGridSortDirection.Ascending)
        //            e.Column.SortDirection = DataGridSortDirection.Descending;
        //        else
        //            e.Column.SortDirection = DataGridSortDirection.Ascending;
        //    }
        //    else
        //    {
        //        // Sorting on a new column. Use that columns SortDirection
        //        e.Column.SortDirection = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortDirection;
        //    }

        //    MainViewModel.SortColumn(e.Column);

        //    // If sort column has changed remove the sort icon from the previous column
        //    if ((sender as DataGrid).Columns[sortColumnNumber].Header != e.Column.Header)
        //    {
        //        (sender as DataGrid).Columns[sortColumnNumber].SortDirection = null;
        //    }
        //    DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber = e.Column.DisplayIndex;
        //    DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortDirection = e.Column.SortDirection;
        //}

        //private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    foreach (PacketMessage packetMessage in e.AddedItems)
        //    {
        //        MainViewModel.SelectedMessages.Add(packetMessage);
        //    }
        //    foreach (PacketMessage packetMessage in e.RemovedItems)
        //    {
        //        MainViewModel.SelectedMessages.Remove(packetMessage);
        //    }
        //    if (MainViewModel.SelectedMessages.Count == 1)
        //    {
        //        MainViewModel.SingleSelectedMessage = MainViewModel.SelectedMessages[0];
        //    }
        //}

        //private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
        //    try
        //    {
        //        PacketMessage pktmsg = (e.OriginalSource as TextBlock)?.DataContext as PacketMessage;
        //        if (pktmsg != null)
        //        {
        //            MainViewModel.OpenMessage(pktmsg);
        //        }
        //    }
        //    catch
        //    {
        //        return;
        //    }
        //}

        //private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    PacketMessage packetMesage = e.Row.DataContext as PacketMessage;

        //    if (!(bool)packetMesage?.MessageOpened)
        //    {
        //        e.Row.Background = new SolidColorBrush(Colors.BlanchedAlmond);
        //    }
        //}

        //private void DataGrid_UnloadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    e.Row.Background = new SolidColorBrush(Colors.White);
        //}

        //private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        //{
        //    try
        //    {
        //        //PacketMessage msg = null;
        //        MainViewModel.PacketMessageRightClicked = (e.OriginalSource as TextBlock)?.DataContext as PacketMessage;
        //        MainViewModel.SingleSelectedMessage = MainViewModel.PacketMessageRightClicked;
        //        //if (MainViewModel.SelectedMessages.Count > 1)
        //        //{
        //        //    msg = MainViewModel.SelectedMessages?.FirstOrDefault(m => m.MessageNumber == MainViewModel.PacketMessageRightClicked?.MessageNumber);
        //        //}
        //        //if (msg is null)
        //        //{
        //        //    (sender as DataGrid).SelectedItem = MainViewModel.PacketMessageRightClicked;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        string messGE = ex.Message;
        //        return;
        //    }
        //}

    }
}
