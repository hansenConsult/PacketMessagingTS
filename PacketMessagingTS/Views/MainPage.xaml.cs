﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FormControlBaseClass;
using MetroLog;

using Microsoft.Toolkit.Uwp.UI.Controls;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        public MainViewModel MainViewModel { get; } = Singleton<MainViewModel>.Instance;

        public static MainPage Current;
        //private readonly object _lock = new object();
        //PivotItem _currentPivotItem;

        //List<string> _bulletinList;
        List<PacketMessage> _messagesInFolder;
        List<PacketMessage> _selectedMessages = new List<PacketMessage>();
        PacketMessage _packetMessageRightClicked;

        //private AppWindow appWindow = null;
        //private Frame appWindowFrame = new Frame();


        public MainPage()
        {
            InitializeComponent();

            Current = this;
            MainViewModel.MainPagePivot = MainPagePivot;

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
            try
            {
                _messagesInFolder = await PacketMessage.GetPacketMessages(MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder);
                //_messagesInFolder = await PacketMessage.GetPacketMessages((MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder).Path);

                MainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(_messagesInFolder);

                //string header = _currentPivotItem.Header as string;
                //int index = header.IndexOf(" (");
                //header = (_currentPivotItem.Header as string).Substring(0, index < 0 ? header.Length : index);
                //_currentPivotItem.Header = $"{header} ({_messagesInFolder.Count})";

                DataGridColumn sortColumn = null;
                //switch (_currentPivotItem.Name)
                //{
                //    case "pivotItemInBox":
                //        sortColumn = dataGridInbox.Tag as DataGridColumn;
                //        break;
                //    case "pivotItemSent":
                //        sortColumn = dataGridSent.Tag as DataGridColumn;
                //        break;
                //    case "pivotItemOutBox":
                //        sortColumn = dataGridOutbox.Tag as DataGridColumn;
                //        break;
                //    case "pivotItemDrafts":
                //        sortColumn = dataGridDrafts.Tag as DataGridColumn;
                //        break;
                //    case "pivotItemArchive":
                //        break;
                //    case "pivotItemDeleted":
                //        break;
                //}
                //if (sortColumn != null)
                //{
                //    SortColumn(sortColumn);
                //}
                DataGrid dataGrid = FindDataGrid(MainViewModel.MainPagePivotSelectedItem);
                int? sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber;
                if (sortColumnNumber == null || sortColumnNumber < 0)
                    return;

                sortColumn = dataGrid.Columns[(int)sortColumnNumber];
                SortColumn(sortColumn);
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"{e.Message}");
            }
        }

        private async void MainPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMessages.Clear();

            //_currentPivotItem = (PivotItem)e.AddedItems[0];
            MainViewModel.MainPagePivotSelectedItem = (PivotItem)e.AddedItems[0];

            //if (!(appWindow is null))
            //{
            //    await appWindow.CloseAsync();      // TODO remove testing only
            //}

            await RefreshDataGridAsync();
        }

        public void AddTextToStatusWindow(string text)
        {
            Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = text;
        }

        // private void OpenMessage(PacketMessage packetMessage)
        // {
        //     string folder = (((PivotItem)MainPagePivot.SelectedItem).Tag as StorageFolder).Path;
        //     string packetMessagePath = Path.Combine(folder, packetMessage.FileName);
        //     Type type = typeof(FormsPage);
        //     switch (packetMessage.FormControlType)
        //     {
        //         case FormControlAttribute.FormType.Undefined:
        //             type = typeof(FormsPage);
        //             break;
        //         case FormControlAttribute.FormType.None:
        //             type = typeof(FormsPage);
        //             break;
        //case FormControlAttribute.FormType.CountyForm:
        //             type = typeof(FormsPage);
        //             break;
        //case FormControlAttribute.FormType.CityForm:
        //             type = typeof(CityFormsPage);
        //             break;
        //case FormControlAttribute.FormType.HospitalForm:
        //             type = typeof(HospitalFormsPage);
        //             break;
        //         case FormControlAttribute.FormType.TestForm:
        //             type = typeof(TestFormsPage);
        //             break;
        //     }
        //     NavigationService.Navigate(type, packetMessagePath);
        // }

        private async Task DeleteMessageAsync(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            StorageFolder folder = MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder;
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
            //if (appWindow == null)
            //{
            //    // Create a new window
            //    appWindow = await AppWindow.TryCreateAsync();
            //    // Make sure we release the reference to this window, and release XAML resources, when it's closed
            //    appWindow.Closed += delegate { appWindow = null; appWindowFrame.Content = null; };
            //    // Navigate the frame to the page we want to show in the new window
            //    appWindowFrame.Navigate(typeof(RxTxStatusPage), appWindow);
            //}
            //// Request the size of our window
            //appWindow.RequestSize(new Size(500, 320));
            //// Attach the XAML content to our window
            //ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);

            //AppWindowPlacement placement = appWindow.GetPlacement();
            //double width = placement.Size.Width;
            //Point offset = new Point(-width, 50);
            //appWindow.RequestMoveRelativeToCurrentViewContent(offset);

            //// Now show the window
            //await appWindow.TryShowAsync();
            //AppWindowPlacement placement = appWindow.GetPlacement();
            //double width = placement.Size.Width;
            //Point offset = new Point(-width, 50);
            //appWindow.RequestMoveRelativeToCurrentViewContent(offset);


            // Using ViewLifetimeControl
            await WindowManagerService.Current.TryShowAsStandaloneAsync("Connection Status", typeof(RxTxStatusPage));

            CommunicationsService communicationsService = CommunicationsService.CreateInstance();

            communicationsService.BBSConnectAsync2();
            //communicationsService.BBSConnectAsync2(Dispatcher);

            await RefreshDataGridAsync();
        }

        private void AppBarMainPage_OpenMessage(object sender, RoutedEventArgs e)
        {
             if (_selectedMessages != null && _selectedMessages.Count == 1)
            {
                MainViewModel.OpenMessage(_selectedMessages[0]);
            }
        }

        private async void AppBarMainPage_MoveToArchiveAsync(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = MainViewModel.MainPagePivotSelectedItem.Tag as StorageFolder;

            //if (_mainViewModel.SelectedItems.Count > 1)
            //{
                foreach (PacketMessage packetMessage in MainViewModel.SelectedItems)
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

            MainViewModel.OpenMessage(_packetMessageRightClicked);
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

        int i = 0;
        private void AppBarMainPage_TestStatusPage(object sender, RoutedEventArgs e)
        {
            //i++;
            //textBoxText += $" \nTest text{i}";
            //textBoxStatus.Text = textBoxText;

            //Singleton<RxTxStatusViewModel>.Instance.AddRxTxStatus = $"\nTest text{i}";

            CommunicationsService communicationsService = CommunicationsService.CreateInstance();
            //communicationsService.AddRxTxStatusAsync($"\nTest text{i}");
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
            MainViewModel.DataGridSource = new ObservableCollection<PacketMessage>(sortedItems);
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            int sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber;
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
                e.Column.SortDirection = DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortDirection;
            }

            SortColumn(e.Column);

            // If sort column has changed remove the sort icon from the previous column
            if ((sender as DataGrid).Columns[sortColumnNumber].Header != e.Column.Header)
            {
                (sender as DataGrid).Columns[sortColumnNumber].SortDirection = null;
            }
            DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortColumnNumber = e.Column.DisplayIndex;
            DataGridSortData.DataGridSortDataDictionary[MainViewModel.MainPagePivotSelectedItem.Name].SortDirection = e.Column.SortDirection;
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
            MainViewModel.SelectedItems = _selectedMessages;
        }

        private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                PacketMessage pktmsg = (e.OriginalSource as TextBlock).DataContext as PacketMessage;
                MainViewModel.OpenMessage(pktmsg);
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
