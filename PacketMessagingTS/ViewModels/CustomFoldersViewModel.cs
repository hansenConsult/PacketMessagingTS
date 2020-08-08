using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using MetroLog;

using Microsoft.Toolkit.Uwp.UI.Controls;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Views;

using SharedCode;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

using WinUI = Microsoft.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class CustomFoldersViewModel : MessageDataGridViewModel
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CustomFoldersViewModel>();
        private static LogHelper _logHelper = new LogHelper(log);

        private static CustomFoldersArray _customFoldersInstance = CustomFoldersArray.Instance;

        public CustomFoldersPage CustomFoldersPage { get; set; }

        private int selectedTabIndex;
        public int SelectedTabIndex
        {
            get
            {
                GetProperty(ref selectedTabIndex);
                if (selectedTabIndex >= Tabs.Count)
                {
                    SelectedTabIndex = 0;
                }
                return selectedTabIndex;
            }
            set
            {
                SetProperty(ref selectedTabIndex, value, true);
                //FillMoveLocations();
            }
        }

        private TabViewItemData _selectedTab;
        public TabViewItemData SelectedTab
        {
            get
            {
                if (_selectedTab == null)
                {
                    _selectedTab = _customFoldersInstance.CustomFolderList[selectedTabIndex];
                    RefreshDataGridAsync();
                }
                return _selectedTab;
            }
            set
            {
                if (value != null)
                {
                    Set(ref _selectedTab, value);
                    //if ()
                    //{
                    //    CustomFoldersPage.FillMoveLocations(CustomFoldersDataGrid);
                    //}
                    RefreshDataGridAsync();
                }
            }
        }


        private RelayCommand _addTabCommand;
        private RelayCommand<WinUI.TabViewTabCloseRequestedEventArgs> _closeTabCommand;

        public RelayCommand AddTabCommand => _addTabCommand ?? (_addTabCommand = new RelayCommand(AddTabAsync));

        public RelayCommand<WinUI.TabViewTabCloseRequestedEventArgs> CloseTabCommand => _closeTabCommand ?? (_closeTabCommand = new RelayCommand<WinUI.TabViewTabCloseRequestedEventArgs>(CloseTab));

        public ObservableCollection<TabViewItemData> Tabs { get; } = new ObservableCollection<TabViewItemData>(_customFoldersInstance.CustomFolderList);


        public CustomFoldersViewModel()
        {
        }

        protected override void FillMoveLocations()
        {
            DataGrid dataGrid = PageDataGrid;
            dataGrid.ContextFlyout = new MenuFlyout();
            MenuFlyout data = dataGrid.ContextFlyout as MenuFlyout;

            MenuFlyoutItem newMenuItem = new MenuFlyoutItem()
            {
                Text = "Open",
                Command = OpenMessageFromContextMenuCommand,
            };
            data.Items.Add(newMenuItem);

            MenuFlyoutSubItem moveSubMenu = new MenuFlyoutSubItem()
            {
                Text = "Move",
            };
            data.Items.Add(moveSubMenu);

            newMenuItem = new MenuFlyoutItem()
            {
                Text = "Move To Archive",
                Command = MoveToArchiveFromContextMenuCommand,
            };
            moveSubMenu.Items.Add(newMenuItem);

            string currentFolder = SelectedTab.Folder;
            foreach (TabViewItemData tabView in CustomFoldersArray.Instance.CustomFolderList)
            {
                if (currentFolder != tabView.Folder)
                {
                    newMenuItem = new MenuFlyoutItem()
                    {
                        Text = tabView.Folder,
                        Command = MoveToFolderFromContextMenuCommand,
                        CommandParameter = tabView.Folder,
                    };
                    //newMenuItem.Click += OnMoveToFolderFromContextMenuCommand;
                    moveSubMenu.Items.Add(newMenuItem);
                }
            }
            newMenuItem = new MenuFlyoutItem()
            {
                Text = "Delete",
                Command = DeleteMessagesCommand,
            };
            data.Items.Add(newMenuItem);
        }

        private int GetCustomFolderListIndex()
        {
            int i = 0;
            for (; i < _customFoldersInstance.CustomFolderList.Count; i++)
            {
                if (_customFoldersInstance.CustomFolderList[i].Header == Tabs[selectedTabIndex].Header)
                {
                    break;
                }
            }
            return i;
        }

        protected override async void RefreshDataGridAsync()
        {
            try
            {
                int i = 0;
                for (; i < _customFoldersInstance.CustomFolderList.Count; i++)
                {
                    if (_customFoldersInstance.CustomFolderList[i].Header == Tabs[selectedTabIndex].Header)
                    {
                        break;
                    }
                }
                TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[i];
                StorageFolder itemFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);
                _messagesInFolder = await PacketMessage.GetPacketMessages(itemFolder);

                DataGridSource = new ObservableCollection<PacketMessage>(_messagesInFolder);

                //UpdateHeaderMessageCount(MainPagePivotSelectedItem, _messagesInFolder.Count);

                //DataGrid dataGrid = FindDataGrid(ContentArea);
                bool found = DataGridSortDataDictionary.TryGetValue(SelectedTab.Folder, out DataGridSortData sortData);
                int? sortColumnNumber = -1;
                if (found)
                {
                    sortColumnNumber = sortData.SortColumnNumber;
                }
                else
                {
                    bool success = DataGridSortDataDictionary.TryAdd(SelectedTab.Folder, new DataGridSortData(SelectedTab.Folder, -1, DataGridSortDirection.Descending));
                }

                sortColumnNumber = DataGridSortDataDictionary[tabViewItemData.Folder].SortColumnNumber;
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

        private async void AddTabAsync()
        {
            int newIndex = Tabs.Any() ? Tabs.Max(t => t.Index) + 1 : 1;
            string header = $"Folder {newIndex}";
            string folder = $"Folder {newIndex}";

            NewTabContentDialog addTabDialog = new NewTabContentDialog(header);
            ContentDialogResult result = await addTabDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                header = addTabDialog.NewTabName;
                folder = header;
            }
            TabViewItemData tabViewItem = new TabViewItemData()
            {
                Index = newIndex,
                Header = header,
                Folder = folder,
            };

            _customFoldersInstance.CustomFolderList.Add(tabViewItem);
            Tabs.Add(tabViewItem);
            await _localFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            await _customFoldersInstance.SaveAsync();

            bool success = DataGridSortDataDictionary.TryAdd(tabViewItem.Folder, new DataGridSortData(tabViewItem.Folder, 0, DataGridSortDirection.Descending));
        }

        private async void CloseTab(WinUI.TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is TabViewItemData item)
            {
                StorageFolder itemFolder = await _localFolder.CreateFolderAsync(item.Folder, CreationCollisionOption.OpenIfExists);
                await itemFolder.DeleteAsync();
                SelectedTabIndex = Math.Max(SelectedTabIndex - 1, 0);
                Tabs.Remove(item);
                _customFoldersInstance.CustomFolderList.Remove(item);
                await _customFoldersInstance.SaveAsync();

                DataGridSortDataDictionary.Remove(item.Folder);
            }
        }

        //private RelayCommand<DataGridColumnEventArgs> _SortingCommand;
        //public RelayCommand<DataGridColumnEventArgs> SortingCommand => _SortingCommand ?? (_SortingCommand = new RelayCommand<DataGridColumnEventArgs>(DataGridSorting));
        protected override void DataGridSorting(DataGridColumnEventArgs args)
        {
            bool found = DataGridSortDataDictionary.TryGetValue(SelectedTab.Folder, out DataGridSortData sortData);
            int sortColumnNumber = -1;
            if (found)
            {
                sortColumnNumber = sortData.SortColumnNumber;
            }
            else
            {
                bool success = DataGridSortDataDictionary.TryAdd(SelectedTab.Folder, new DataGridSortData(SelectedTab.Folder, -1, DataGridSortDirection.Descending));
            }
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
                args.Column.SortDirection = DataGridSortDataDictionary[SelectedTab.Header].SortDirection;
            }

            SortColumn(args.Column);

            // If sort column has changed remove the sort icon from the previous column
            if (PageDataGrid.Columns[sortColumnNumber].Header != args.Column.Header)
            {
                PageDataGrid.Columns[sortColumnNumber].SortDirection = null;
            }
            DataGridSortDataDictionary[SelectedTab.Header].SortColumnNumber = args.Column.DisplayIndex;
            DataGridSortDataDictionary[SelectedTab.Header].SortDirection = args.Column.SortDirection;
        }

        public override async void OpenMessage(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[GetCustomFolderListIndex()];
            StorageFolder messageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

            OpenMessage(messageFolder.Path, packetMessage);
        }

        protected override async void MoveToArchiveFromContextMenu()
        {
            TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[GetCustomFolderListIndex()];
            StorageFolder messageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

            var file = await messageFolder.CreateFileAsync(SingleSelectedMessage.FileName, CreationCollisionOption.OpenIfExists);
            await file?.MoveAsync(SharedData.ArchivedMessagesFolder);

            RefreshDataGridAsync();
        }

        protected override async void MoveToFolderFromContextMenu(string folder)
        {
            TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[GetCustomFolderListIndex()];
            StorageFolder messageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

            var file = await messageFolder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
            StorageFolder moveToFolder = await _localFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
            await file?.MoveAsync(moveToFolder);

            RefreshDataGridAsync();
        }

        protected override async Task DeleteMessageAsync(PacketMessage packetMessage)
        {
            if (packetMessage is null)
                return;

            int i = 0;
            for (; i < _customFoldersInstance.CustomFolderList.Count; i++)
            {
                if (_customFoldersInstance.CustomFolderList[i].Header == SelectedTab.Header as string)
                {
                    break;
                }
            }
            TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[i];
            StorageFolder folder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

            //StorageFolder folder = MainPagePivotSelectedItem.Tag as StorageFolder;
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
            _messagesInFolder = await PacketMessage.GetPacketMessages(folder);
        }

        private ICommand _EditFolderNameCommand;
        public ICommand EditFolderNameCommand => _EditFolderNameCommand ?? (_EditFolderNameCommand = new RelayCommand(EditFolderNameAsync));
        public async void EditFolderNameAsync()
        {
            //SelectedTab.
            string header = _selectedTab.Header;
            NewTabContentDialog addTabDialog = new NewTabContentDialog(_selectedTab.Header);
            ContentDialogResult result = await addTabDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    _selectedTab.Header = addTabDialog.NewTabName;
                    // Rename folder
                    StorageFolder itemFolder = await _localFolder.CreateFolderAsync(_selectedTab.Folder, CreationCollisionOption.OpenIfExists);
                    await itemFolder.RenameAsync(_selectedTab.Header);
                    _selectedTab.Folder = _selectedTab.Header;
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Warn, $"Rename folder failed. {_selectedTab.Header}, {e.Message}");
                }
            }
        }

    }
}
