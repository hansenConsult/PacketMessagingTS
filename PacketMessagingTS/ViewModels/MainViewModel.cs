﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using MetroLog;

using Microsoft.Toolkit.Uwp.UI.Controls;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;
using PacketMessagingTS.Views;

using SharedCode;
using SharedCode.Helpers;

using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PacketMessagingTS.ViewModels
{
    public class MainViewModel : MessageDataGridViewModel
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainViewModel>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public static MainViewModel Instance { get; } = new MainViewModel();

        public MainViewModel()
        {
        }

        public override void OpenMessage(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            string folder = (((PivotItem)MainPagePivot.SelectedItem).Tag as StorageFolder).Path;
            // Change state so the message will not be sent if it is open for editing
            if (((PivotItem)MainPagePivot.SelectedItem).Tag as StorageFolder == SharedData.UnsentMessagesFolder)
            {
                packetMessage.MessageState = MessageState.Edit;
                packetMessage.Save(folder);
            }
            string packetMessagePath = Path.Combine(folder, packetMessage.FileName);
            Type type = typeof(CountyFormsPage);
            switch (packetMessage.FormControlType)
            {
                case FormControlAttribute.FormType.Undefined:
                    type = typeof(CountyFormsPage);
                    break;
                case FormControlAttribute.FormType.None:
                    type = typeof(CountyFormsPage);
                    break;
                case FormControlAttribute.FormType.CountyForm:
                    type = typeof(CountyFormsPage);
                    break;
                case FormControlAttribute.FormType.CityForm:
                    type = typeof(CityFormsPage);
                    break;
                case FormControlAttribute.FormType.HospitalForm:
                    type = typeof(HospitalFormsPage);
                    break;
                case FormControlAttribute.FormType.TestForm:
                    type = typeof(TestFormsPage);
                    break;
            }
            NavigationService.Navigate(type, packetMessagePath);

        }

        public Pivot MainPagePivot { get; set; }

        //private ObservableCollection<PacketMessage> source;
        //public ObservableCollection<PacketMessage> Source
        //{
        //    get => source;
        //    set => SetProperty(ref source, value);
        //}

        private int mainPagePivotSelectedIndex;
        public int MainPagePivotSelectedIndex
        {
            get => GetProperty(ref mainPagePivotSelectedIndex);
            //{
            //    GetProperty(ref mainPagePivotSelectedIndex);
            //    MainPagePivotSelectedItem = MainPagePivot.Items[mainPagePivotSelectedIndex] as PivotItem;
            //    RefreshDataGridAsync();
            //    return mainPagePivotSelectedIndex;
            //}
            set
            {
                bool indexChanged = SetProperty(ref mainPagePivotSelectedIndex, value, true);

                if (indexChanged)
                {
                    //MainPagePivotSelectedItem = MainPagePivot.Items[mainPagePivotSelectedIndex] as PivotItem;
                    SelectedMessages.Clear();
                }
            }
        }

        private PivotItem _MainPagePivotSelectedItem;
        public PivotItem MainPagePivotSelectedItem
        {
            get
            {
                if (_MainPagePivotSelectedItem is null)
                {
                    _MainPagePivotSelectedItem = (PivotItem)MainPagePivot.Items[mainPagePivotSelectedIndex];
                    RefreshDataGridAsync();
                }
                //FillMoveLocations();
                return _MainPagePivotSelectedItem;
            }
            set
            {
                Set(ref _MainPagePivotSelectedItem, value);
                PageDataGrid = null;
                RefreshDataGridAsync();
            }
        }

        public static DataGrid FindDataGrid(DependencyObject panelName)
        {
            DataGrid dataGrid = null;

            //var count = VisualTreeHelper.GetChildrenCount(panelName);
            DependencyObject control = VisualTreeHelper.GetChild(panelName, 0);

            //count = VisualTreeHelper.GetChildrenCount(control);
            control = VisualTreeHelper.GetChild(control, 0);

            //count = VisualTreeHelper.GetChildrenCount(control);
            control = VisualTreeHelper.GetChild(control, 0);

            if (control is DataGrid)
            {
                dataGrid = control as DataGrid;
            }
            return dataGrid;
        }


        public async Task UpdateDownloadedBulletinsAsync()
        {
            PacketSettingsViewModel packetSettingsViewModel = PacketSettingsViewModel.Instance;
            string[] areas = packetSettingsViewModel.AreaString.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            BulletinHelpers.BulletinDictionary = new Dictionary<string, List<string>>();

            foreach (PivotItem pivotItem in MainPagePivot.Items)
            {
                if (pivotItem.Name == "pivotItemInBox" || pivotItem.Name == "pivotItemArchive")
                {
                    List<PacketMessage> messagesInFolder = await PacketMessage.GetPacketMessages((pivotItem.Tag as StorageFolder).Path);
                    foreach (PacketMessage packetMessage in messagesInFolder)
                    {
                        foreach (string area in areas)
                        {
                            if (packetMessage.Area == area)
                            {
                                if (!BulletinHelpers.BulletinDictionary.TryGetValue(area, out _))
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

        public static void UpdateHeaderMessageCount(PivotItem pivotItem, int messageCount)
        {
            if (!(pivotItem.Name as string).Contains("pivotItemOutBox"))
                return;

            if ((pivotItem.Header as string).IndexOf('(') != -1)
            {
                int startIndex = (pivotItem.Header as string).IndexOf('(');
                int stopIndex = (pivotItem.Header as string).IndexOf(')');
                int oldCount = Convert.ToInt32((pivotItem.Header as string).Substring(startIndex + 1, stopIndex - startIndex - 1));
                if (messageCount != 0)
                {
                    if (oldCount != messageCount)
                    {
                        pivotItem.Header = (pivotItem.Header as string).Replace($"({oldCount})", $"({messageCount})");
                    }
                }
                else
                {
                    pivotItem.Header = (pivotItem.Header as string).Replace($" ({oldCount})", "");
                }
            }
            else
            {
                if (messageCount != 0)
                {
                    pivotItem.Header = $"{pivotItem.Header} ({messageCount})";
                }
            }
        }

        protected override void DataGridSorting(DataGridColumnEventArgs args)
        {
            int sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainPagePivotSelectedItem.Name].SortColumnNumber;
            if (sortColumnNumber < 0)
            {
                // There is no default sorting column for this data grid. Select current column.
                sortColumnNumber = args.Column.DisplayIndex;
            }
            if (PageDataGrid.Columns[sortColumnNumber].Header == args.Column.Header) // Sorting on same column, switch SortDirection
            {
                if (args.Column.SortDirection == DataGridSortDirection.Ascending)
                    args.Column.SortDirection = DataGridSortDirection.Descending;
                else
                    args.Column.SortDirection = DataGridSortDirection.Ascending;
            }
            else
            {
                // Sorting on a new column. Use that columns SortDirection
                args.Column.SortDirection = DataGridSortData.DataGridSortDataDictionary[MainPagePivotSelectedItem.Name].SortDirection;
            }

            SortColumn(args.Column);

            // If sort column has changed remove the sort icon from the previous column
            if (PageDataGrid.Columns[sortColumnNumber].Header != args.Column.Header)
            {
                PageDataGrid.Columns[sortColumnNumber].SortDirection = null;
            }
            DataGridSortData.DataGridSortDataDictionary[MainPagePivotSelectedItem.Name].SortColumnNumber = args.Column.DisplayIndex;
            DataGridSortData.DataGridSortDataDictionary[MainPagePivotSelectedItem.Name].SortDirection = args.Column.SortDirection;
        }

        protected override async void RefreshDataGridAsync()
        {
            try
            {
                _messagesInFolder = await PacketMessage.GetPacketMessages(MainPagePivotSelectedItem.Tag as StorageFolder);

                DataGridSource = new ObservableCollection<PacketMessage>(_messagesInFolder);

                if (_messagesInFolder.Count == 0)
                    return;

                UpdateHeaderMessageCount(MainPagePivotSelectedItem, _messagesInFolder.Count);

                if (PageDataGrid is null)
                {
                    PageDataGrid = FindDataGrid(MainPagePivotSelectedItem);
                }

                int? sortColumnNumber = DataGridSortData.DataGridSortDataDictionary[MainPagePivotSelectedItem.Name].SortColumnNumber;
                if (sortColumnNumber == null || sortColumnNumber < 0)
                    return;

                DataGridColumn sortColumn = PageDataGrid.Columns[(int)sortColumnNumber];
                SortColumn(sortColumn);
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"{e.Message}");
            }
        }

        protected override void FillMoveLocations()
        {
            PageDataGrid = FindDataGrid(MainPagePivotSelectedItem);
            string menuFlyoutSubItemName = "moveMenu" + PageDataGrid?.Name.Substring(8);
            //MenuFlyoutSubItem moveSubMenu = PageDataGrid?.FindName(menuFlyoutSubItemName) as MenuFlyoutSubItem;
            if (!(PageDataGrid?.FindName(menuFlyoutSubItemName) is MenuFlyoutSubItem moveSubMenu))
                return;

            int itemsCount = moveSubMenu.Items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                if (moveSubMenu.Items[i] != null && (moveSubMenu.Items[i] as MenuFlyoutItem).Text.Contains("Archive"))
                {
                    continue;
                }
                moveSubMenu.Items.Remove(moveSubMenu.Items[i]);
            }

            foreach (TabViewItemData tabView in CustomFoldersArray.Instance.CustomFolderDataList)
            {
                MenuFlyoutItem newMenuItem = new MenuFlyoutItem()
                {
                    Text = tabView.Folder,
                    Command = MoveToFolderFromContextMenuCommand,
                    CommandParameter = tabView.Folder,
                };
                moveSubMenu.Items.Add(newMenuItem);
            }
        }

        protected override async Task DeleteMessageAsync(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            StorageFolder folder = MainPagePivotSelectedItem.Tag as StorageFolder;
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
            catch (FileNotFoundException)
            {
                var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                await file?.DeleteAsync();

                //string s = ex.ToString();
            }
            catch (Exception e)
            {
                _logHelper.Log(LogLevel.Error, $"{e.Message}");
            }
            _messagesInFolder = await PacketMessage.GetPacketMessages(folder);
        }

        //private ICommand _OpenMessageCommand;
        //public ICommand OpenMessageCommand => _OpenMessageCommand ?? (_OpenMessageCommand = new RelayCommand(OpenMessage));

        //public void OpenMessage()
        //{
        //    if (SelectedMessages != null && SelectedMessages.Count == 1)
        //    {
        //        OpenMessage(SelectedMessages[0]);
        //    }
        //}

        //private ICommand _SendReceiveCommand;
        //public ICommand SendReceiveCommand => _SendReceiveCommand ?? (_SendReceiveCommand = new RelayCommand(SendReceive));

        //public async void SendReceive()
        //{
        //    CommunicationsService.CreateInstance().BBSConnectAsync2();
        //    RefreshDataGridAsync();
        //}

        //private ICommand _DeleteMessagesCommand;
        //public ICommand DeleteMessagesCommand => _DeleteMessagesCommand ?? (_DeleteMessagesCommand = new RelayCommand(DeleteSelectedMessages));

        //public async void DeleteSelectedMessages()
        //{
        //    foreach (PacketMessage packetMessage in SelectedMessages)
        //    {
        //        await DeleteMessageAsync(packetMessage);
        //    }
        //    RefreshDataGridAsync();
        //}
        private RelayCommand<PivotItemEventArgs> _PivotItemLoadedCommand;
        public RelayCommand<PivotItemEventArgs> PivotItemLoadedCommand => _PivotItemLoadedCommand ?? (_PivotItemLoadedCommand = new RelayCommand<PivotItemEventArgs>(PivotItemLoaded));

        public void PivotItemLoaded(PivotItemEventArgs args)
        {
            PageDataGrid = FindDataGrid(MainPagePivotSelectedItem);
            //RefreshDataGridAsync();
            FillMoveLocations();
        }

        private RelayCommand<SelectionChangedEventArgs> _SelectionChangedCommand;
        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand => _SelectionChangedCommand ?? (_SelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>(SelectionChanged));

        public void SelectionChanged(SelectionChangedEventArgs args)
        {
            MainPagePivotSelectedItem = (PivotItem)args.AddedItems[0];
            //FillMoveLocations();
        }

        protected RelayCommand<DataGridRowEventArgs> _LandingRowCommand;
        public RelayCommand<DataGridRowEventArgs> LandingRowCommand => _LandingRowCommand ?? (_LandingRowCommand = new RelayCommand<DataGridRowEventArgs>(LandingRow));

        protected static void LandingRow(DataGridRowEventArgs args)
        {
            PacketMessage packetMesage = args.Row.DataContext as PacketMessage;

            if (!(bool)packetMesage?.MessageOpened)
            {
                args.Row.Background = new SolidColorBrush(Colors.BlanchedAlmond);
            }
        }

        protected RelayCommand<DataGridRowEventArgs> _UnloadingRowCommand;
        public RelayCommand<DataGridRowEventArgs> UnloadingRowCommand => _UnloadingRowCommand ?? (_UnloadingRowCommand = new RelayCommand<DataGridRowEventArgs>(UnloadingRow));

        protected static void UnloadingRow(DataGridRowEventArgs args)
        {
            args.Row.Background = new SolidColorBrush(Colors.White);
        }

        private ICommand _MoveToArchiveCommand;
        public ICommand MoveToArchiveCommand => _MoveToArchiveCommand ?? (_MoveToArchiveCommand = new RelayCommand(MoveToArchive));

        public async void MoveToArchive()
        {
            StorageFolder folder = MainPagePivotSelectedItem.Tag as StorageFolder;

            if (SelectedMessages.Count > 0)
            {
                foreach (PacketMessage packetMessage in SelectedMessages)
                {
                    var file = await folder.CreateFileAsync(packetMessage.FileName, CreationCollisionOption.OpenIfExists);
                    await file?.MoveAsync(SharedData.ArchivedMessagesFolder);
                }
            }
            RefreshDataGridAsync();
        }

        private ICommand _DeleteMessagesFromContextMenuCommand;
        public ICommand DeleteMessagesFromContextMenuCommand => _DeleteMessagesFromContextMenuCommand ?? (_DeleteMessagesFromContextMenuCommand = new RelayCommand(DeleteMessageFromContextMenu));

        public async void DeleteMessageFromContextMenu()
        {
            await DeleteMessageAsync(PacketMessageRightClicked);
            RefreshDataGridAsync();
        }

        protected override async void MoveToFolderFromContextMenu(string folder)
        {
            if (PacketMessageRightClicked == null)
                return;

            StorageFolder storageFolder = MainPagePivotSelectedItem.Tag as StorageFolder;

            PacketMessageRightClicked.MovedFromFolder = storageFolder.Name;
            PacketMessageRightClicked.Save(storageFolder.Path);

            var file = await storageFolder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
            StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder moveToFolder = await _localFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            await file?.MoveAsync(moveToFolder);

            RefreshDataGridAsync();
        }

        protected override async void MoveToArchiveFromContextMenu()
        {
            if (PacketMessageRightClicked == null)
                return;

            StorageFolder folder = MainPagePivotSelectedItem.Tag as StorageFolder;

            PacketMessageRightClicked.MovedFromFolder = folder.Name;
            PacketMessageRightClicked.Save(folder.Path);

            var file = await folder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
            await file?.MoveAsync(SharedData.ArchivedMessagesFolder);

            RefreshDataGridAsync();
        }

        private ICommand _PrintFromContextMenuCommand;
        public ICommand PrintFromContextMenuCommand => _PrintFromContextMenuCommand ?? (_PrintFromContextMenuCommand = new RelayCommand(PrintFromContextMenu));

        public object Utility { get; private set; }

        public async void PrintFromContextMenu()
        {
            StorageFolder folder = MainPagePivotSelectedItem.Tag as StorageFolder;

            await folder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
            //await file?.MoveAsync(SharedData.ArchivedMessagesFolder);
        }

    }
}
