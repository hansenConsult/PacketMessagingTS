
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomFoldersPage : Page
    {
        //public CustomFoldersViewModel ViewModel { get; } = Singleton<CustomFoldersViewModel>.Instance;
        private CustomFoldersViewModel ViewModel = CustomFoldersViewModel.Instance;

        //public event RoutedEventHandler OnMoveToFolderFromContextMenuCommand;

        public CustomFoldersPage()
        {
            InitializeComponent();

            //ViewModel.CustomFoldersPage = this;
            ViewModel.PageDataGrid = dataGrid;
        }

        //public void FillMoveLocations(DataGrid dataGrid1)
        //{
        //    DataGrid dataGrid = Utilities.FindVisualChild<DataGrid>(ContentArea);
        //    dataGrid.ContextFlyout = new MenuFlyout();
        //    MenuFlyout data = dataGrid.ContextFlyout as MenuFlyout;

        //    MenuFlyoutItem newMenuItem = new MenuFlyoutItem()
        //    {
        //        Text = "Open",
        //        Command = ViewModel.OpenMessageFromContextMenuCommand,
        //    };
        //    data.Items.Add(newMenuItem);

        //    MenuFlyoutSubItem moveSubMenu = new MenuFlyoutSubItem()
        //    {
        //        Text = "Move",
        //    };
        //    data.Items.Add(moveSubMenu);

        //    newMenuItem = new MenuFlyoutItem()
        //    {
        //        Text = "Move To Archive",
        //        Command = ViewModel.MoveToArchiveFromContextMenuCommand,
        //    };
        //    moveSubMenu.Items.Add(newMenuItem);

        //    string currentFolder = ViewModel.SelectedTab.Folder;
        //    foreach (TabViewItemData tabView in CustomFoldersArray.Instance.CustomFolderList)
        //    {
        //        if (currentFolder != tabView.Folder)
        //        {
        //            newMenuItem = new MenuFlyoutItem()
        //            {
        //                Text = tabView.Folder,
        //                Command = ViewModel.MoveToFolderFromContextMenuCommand,
        //                CommandParameter = tabView.Folder,
        //            };
        //            //newMenuItem.Click += OnMoveToFolderFromContextMenuCommand;
        //            moveSubMenu.Items.Add(newMenuItem);
        //        }
        //    }
        //    newMenuItem = new MenuFlyoutItem()
        //    {
        //        Text = "Delete",
        //        Command = ViewModel.DeleteMessagesCommand,
        //    };
        //    data.Items.Add(newMenuItem);

        //}

        //private void OnMoveToFolderFromContextMenuCommand(object sender, RoutedEventArgs e)
        //{
        //    string folder = (sender as MenuFlyoutItem).Text;
        //    ViewModel.MoveToFolderFromContextMenuCommand.Execute(folder);
        //}

        //private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        //{
        //    //int currentColumn = ViewModel.PageDataGrid.CurrentColumn.DisplayIndex;

        //    bool found = ViewModel.DataGridSortDataDictionary.TryGetValue(ViewModel.SelectedTab.Folder, out DataGridSortData sortData);
        //    int sortColumnNumber = -1;
        //    if (found)
        //    {
        //        sortColumnNumber = sortData.SortColumnNumber;
        //    }
        //    else
        //    {
        //        bool success = ViewModel.DataGridSortDataDictionary.TryAdd(ViewModel.SelectedTab.Folder, new DataGridSortData(ViewModel.SelectedTab.Folder, -1, DataGridSortDirection.Descending));
        //    }
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
        //        e.Column.SortDirection = ViewModel.DataGridSortDataDictionary[ViewModel.SelectedTab.Header].SortDirection;
        //    }

        //    ViewModel.SortColumn(e.Column);

        //    // If sort column has changed remove the sort icon from the previous column
        //    if ((sender as DataGrid).Columns[sortColumnNumber].Header != e.Column.Header)
        //    {
        //        (sender as DataGrid).Columns[sortColumnNumber].SortDirection = null;
        //    }
        //    ViewModel.DataGridSortDataDictionary[ViewModel.SelectedTab.Header].SortColumnNumber = e.Column.DisplayIndex;
        //    ViewModel.DataGridSortDataDictionary[ViewModel.SelectedTab.Header].SortDirection = e.Column.SortDirection;
        //}

        //private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    foreach (PacketMessage packetMessage in e.AddedItems)
        //    {
        //        ViewModel.SelectedMessages.Add(packetMessage);
        //    }
        //    foreach (PacketMessage packetMessage in e.RemovedItems)
        //    {
        //        ViewModel.SelectedMessages.Remove(packetMessage);
        //    }
        //    if (ViewModel.SelectedMessages.Count == 1)
        //    {
        //        ViewModel.SingleSelectedMessage = ViewModel.SelectedMessages[0];
        //    }
        //    IList SelectedMessages = (sender as DataGrid).SelectedItems;
        //    int a = (sender as DataGrid).SelectedItems.Count;
        //    var packetMessage1 = SelectedMessages[0] as PacketMessage;
        //}

        //private void DataGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        //{
        //    try
        //    {
        //        PacketMessage pktmsg = (e.OriginalSource as TextBlock)?.DataContext as PacketMessage;
        //        if (pktmsg != null)
        //        {
        //            ViewModel.OpenMessage(pktmsg);
        //        }
        //    }
        //    catch
        //    {
        //        return;
        //    }
        //}

        //private void DataGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        //{
        //    try
        //    {
        //        ViewModel.PacketMessageRightClicked = (e.OriginalSource as TextBlock)?.DataContext as PacketMessage;
        //        ViewModel.SingleSelectedMessage = ViewModel.PacketMessageRightClicked;
        //        FillMoveLocations(ViewModel.PageDataGrid);
        //    }
        //    catch (Exception ex)
        //    {
        //        string messGE = ex.Message;
        //        return;
        //    }
        //}

    }
}

