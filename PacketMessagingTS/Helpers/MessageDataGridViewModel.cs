using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.Views;
using SharedCode;
using SharedCode.Helpers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace PacketMessagingTS.Helpers
{
    public abstract class MessageDataGridViewModel : BaseViewModel
    {
        public List<PacketMessage> _messagesInFolder;

        protected StorageFolder _localFolder = ApplicationData.Current.LocalFolder;


        private ObservableCollection<PacketMessage> _dataGridSource;
        public ObservableCollection<PacketMessage> DataGridSource
        {
            get => _dataGridSource;
            set => Set(ref _dataGridSource, value);
        }

        private Dictionary<string, DataGridSortData> _DataGridSortDataDictionary;
        public Dictionary<string, DataGridSortData> DataGridSortDataDictionary => _DataGridSortDataDictionary ?? (_DataGridSortDataDictionary = new Dictionary<string, DataGridSortData>());

        private List<PacketMessage> _SelectedMessages;
        public List<PacketMessage> SelectedMessages => _SelectedMessages ?? (_SelectedMessages = new List<PacketMessage>());
        //{ get; set; }

        public PacketMessage SingleSelectedMessage { get; set; }

        public PacketMessage PacketMessageRightClicked { get; set; }

        protected abstract void RefreshDataGridAsync();

        protected abstract Task DeleteMessageAsync(PacketMessage packetMessage);

        private object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
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

        public void SortColumn(DataGridColumn column)
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
            DataGridSource = new ObservableCollection<PacketMessage>(sortedItems);
        }

        private ICommand _OpenMessageCommand;
        public ICommand OpenMessageCommand => _OpenMessageCommand ?? (_OpenMessageCommand = new RelayCommand(OpenMessage));

        public void OpenMessage()
        {
            //if (SelectedMessages != null && SelectedMessages.Count == 1)
            //{
            //    OpenMessage(SelectedMessages[0]);
            //}
            OpenMessage(SingleSelectedMessage);
        }

        public abstract void OpenMessage(PacketMessage packetMessage);

        protected void OpenMessage(string messageFolder, PacketMessage packetMessage)
        {
            if (string.IsNullOrEmpty(messageFolder))
                return;

            string packetMessagePath = Path.Combine(messageFolder, packetMessage.FileName);
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


        private ICommand _SendReceiveCommand;
        public ICommand SendReceiveCommand => _SendReceiveCommand ?? (_SendReceiveCommand = new RelayCommand(SendReceive));

        public void SendReceive()
        {
            CommunicationsService.CreateInstance().BBSConnectAsync2();
            RefreshDataGridAsync();
        }

        private ICommand _DeleteMessagesCommand;
        public ICommand DeleteMessagesCommand => _DeleteMessagesCommand ?? (_DeleteMessagesCommand = new RelayCommand(DeleteSelectedMessages));

        public async void DeleteSelectedMessages()
        {
            foreach (PacketMessage packetMessage in SelectedMessages)
            {
                await DeleteMessageAsync(packetMessage);
            }
            RefreshDataGridAsync();
        }

        private ICommand _OpenMessageFromContextMenuCommand;
        public ICommand OpenMessageFromContextMenuCommand => _OpenMessageFromContextMenuCommand ?? (_OpenMessageFromContextMenuCommand = new RelayCommand(OpenMessageGromContextMenu));

        public void OpenMessageGromContextMenu()
        {
            //OpenMessage(PacketMessageRightClicked);
            OpenMessage(SingleSelectedMessage);
        }

        private ICommand _MoveToArchiveFromContextMenuCommand;
        public ICommand MoveToArchiveFromContextMenuCommand => _MoveToArchiveFromContextMenuCommand ?? (_MoveToArchiveFromContextMenuCommand = new RelayCommand(MoveToArchiveFromContextMenu));

        protected abstract void MoveToArchiveFromContextMenu();
        //{
        //    TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderList[GetCustomFolderListIndex()];
        //    StorageFolder messageFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

        //    var file = await folder.CreateFileAsync(SingleSelectedMessage.FileName, CreationCollisionOption.OpenIfExists);
        //    await file?.MoveAsync(SharedData.ArchivedMessagesFolder);

        //    RefreshDataGridAsync();
        //}

        private ICommand _MoveToFolderFromContextMenuCommand;
        public ICommand MoveToFolderFromContextMenuCommand => _MoveToFolderFromContextMenuCommand ?? (_MoveToFolderFromContextMenuCommand = new RelayCommand<string>(MoveToFolderFromContextMenu));

        protected abstract void MoveToFolderFromContextMenu(string folder);
        //{
        //    StorageFolder storageFolder = MainPagePivotSelectedItem.Tag as StorageFolder;

        //    var file = await storageFolder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);
        //    StorageFolder _localFolder = ApplicationData.Current.LocalFolder;
        //    StorageFolder moveToFolder = await _localFolder.CreateFolderAsync(folder, CreationCollisionOption.OpenIfExists);
        //    await file?.MoveAsync(moveToFolder);

        //    RefreshDataGridAsync();
        //}

    }
}
