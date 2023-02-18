using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;

using PacketMessagingTS.Services;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.Views;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using PacketMessagingTS.Core.Helpers;

namespace PacketMessagingTS.Helpers
{
    public abstract class MessageDataGridViewModel : ViewModelBase
    {
        public List<PacketMessage> _messagesInFolder;

        protected StorageFolder _localFolder = ApplicationData.Current.LocalFolder;

        public DataGrid PageDataGrid
        { get; set; }

        private ObservableCollection<PacketMessage> _dataGridSource;
        public ObservableCollection<PacketMessage> DataGridSource
        {
            get => _dataGridSource;
            set => SetProperty(ref _dataGridSource, value);
        }

        private Dictionary<string, DataGridSortData> _DataGridSortDataDictionary;
        public Dictionary<string, DataGridSortData> DataGridSortDataDictionary => _DataGridSortDataDictionary ?? (_DataGridSortDataDictionary = new Dictionary<string, DataGridSortData>());

        private List<PacketMessage> _SelectedMessages;
        public List<PacketMessage> SelectedMessages => _SelectedMessages ?? (_SelectedMessages = new List<PacketMessage>());

        public PacketMessage SingleSelectedMessage { get; set; }

        private PacketMessage _SelectedMessage;
        public PacketMessage SelectedMessage
        {
            get => _SelectedMessage;
            set
            {
                SetProperty(ref _SelectedMessage, value);

                if (!(_SelectedMessage is null))
                {
                    _SelectedMessages = new List<PacketMessage>();
                    for (int i = 0; i < PageDataGrid.SelectedItems.Count; i++)
                    {
                        _SelectedMessages.Add(PageDataGrid.SelectedItems[i] as PacketMessage);
                    }
                }
            }
        }

        public PacketMessage PacketMessageRightClicked { get; set; }

        protected abstract void RefreshDataGridAsync();

        protected abstract Task DeleteMessageAsync(PacketMessage packetMessage);

        protected abstract void FillMoveLocations();

        private object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        protected void SortColumn(DataGridColumn column)
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

        private bool _IsAppBarOpenDeleteEnabled;
        public bool IsAppBarOpenDeleteEnabled
        {
            get => (_messagesInFolder != null && _messagesInFolder.Count > 0);
            set => SetProperty(ref _IsAppBarOpenDeleteEnabled, value);
        }

        private ICommand _OpenMessageCommand;
        public ICommand OpenMessageCommand => _OpenMessageCommand ?? (_OpenMessageCommand = new RelayCommand(OpenMessage));

        public void OpenMessage()
        {
            if (SelectedMessages != null && SelectedMessages.Count == 1)
            {
                OpenMessage(SelectedMessages[0]);
            }
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

        public async void SendReceive()
        {
            CommunicationsService communicationsService = new CommunicationsService();
            await communicationsService.BBSConnectAsync2();
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
        public ICommand OpenMessageFromContextMenuCommand => _OpenMessageFromContextMenuCommand ?? (_OpenMessageFromContextMenuCommand = new RelayCommand(OpenMessageFromContextMenu));

        public void OpenMessageFromContextMenu()
        {
            OpenMessage(PacketMessageRightClicked);
        }

        private ICommand _MoveToArchiveFromContextMenuCommand;
        public ICommand MoveToArchiveFromContextMenuCommand => _MoveToArchiveFromContextMenuCommand ?? (_MoveToArchiveFromContextMenuCommand = new RelayCommand(MoveToArchiveFromContextMenu));

        protected abstract void MoveToArchiveFromContextMenu();

        private ICommand _UndoMoveToArchiveFromContextMenuCommand;
        public ICommand UndoMoveToArchiveFromContextMenuCommand => _UndoMoveToArchiveFromContextMenuCommand ?? (_UndoMoveToArchiveFromContextMenuCommand = new RelayCommand(UndoMoveToArchiveFromContextMenu));

        protected async void UndoMoveToArchiveFromContextMenu()
        {
            if (PacketMessageRightClicked == null)
                return;

            string fromFolderName = PacketMessageRightClicked.MovedFromFolder;
            PacketMessageRightClicked.MovedFromFolder = "";
            PacketMessageRightClicked.Save(SharedData.ArchivedMessagesFolder.Path);

            StorageFile storageFile = await SharedData.ArchivedMessagesFolder.CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);

            StorageFolder movedFromfolder = await _localFolder.CreateFolderAsync(fromFolderName, CreationCollisionOption.OpenIfExists);
            await storageFile?.MoveAsync(movedFromfolder);
                      
            RefreshDataGridAsync();
        }

        private ICommand _MoveToFolderFromContextMenuCommand;
        public ICommand MoveToFolderFromContextMenuCommand => _MoveToFolderFromContextMenuCommand ?? (_MoveToFolderFromContextMenuCommand = new RelayCommand<string>(MoveToFolderFromContextMenu));

        protected abstract void MoveToFolderFromContextMenu(string folder);

        protected RelayCommand<DoubleTappedRoutedEventArgs> _doubleTappedCommand;
        public RelayCommand<DoubleTappedRoutedEventArgs> DoubleTappedCommand => _doubleTappedCommand ?? (_doubleTappedCommand = new RelayCommand<DoubleTappedRoutedEventArgs>(DoubleTapped));

        private ICommand _UndoMoveFromContextMenuCommand;
        public ICommand UndoMoveFromContextMenuCommand => _UndoMoveFromContextMenuCommand ?? (_UndoMoveFromContextMenuCommand = new RelayCommand(UndoMoveFromContextMenu));

        protected virtual void UndoMoveFromContextMenu()
        {
            if (PacketMessageRightClicked == null)
                return;

        //    int i = GetCustomFolderListIndex();
        //    TabViewItemData tabViewItemData = _customFoldersInstance.CustomFolderDataList[i];
        //    StorageFolder itemFolder = await _localFolder.CreateFolderAsync(tabViewItemData.Folder, CreationCollisionOption.OpenIfExists);

            string fromFolderName = PacketMessageRightClicked.MovedFromFolder;
            PacketMessageRightClicked.MovedFromFolder = "";
        //    PacketMessageRightClicked.Save(.Path);

        //    StorageFile storageFile = await .CreateFileAsync(PacketMessageRightClicked.FileName, CreationCollisionOption.OpenIfExists);

        //    StorageFolder movedFromfolder = await _localFolder.CreateFolderAsync(fromFolderName, CreationCollisionOption.OpenIfExists);
        //    await storageFile?.MoveAsync(movedFromfolder);

        //    RefreshDataGridAsync();
        }

        protected void DoubleTapped(DoubleTappedRoutedEventArgs args)
        {
            try
            {
                PacketMessage pktmsg = (args.OriginalSource as TextBlock)?.DataContext as PacketMessage;
                if (pktmsg != null || (SelectedMessages != null && SelectedMessages.Count == 1))
                {
                    OpenMessage(pktmsg);
                }
            }
            catch
            {
                return;
            }
        }

        protected RelayCommand<RightTappedRoutedEventArgs> _rightTappedCommand;
        public RelayCommand<RightTappedRoutedEventArgs> RightTappedCommand => _rightTappedCommand ?? (_rightTappedCommand = new RelayCommand<RightTappedRoutedEventArgs>(RightTapped));

        protected void RightTapped(RightTappedRoutedEventArgs args)
        {
            try
            {
                PacketMessageRightClicked = (args.OriginalSource as TextBlock)?.DataContext as PacketMessage;
                SingleSelectedMessage = PacketMessageRightClicked;
            }
            catch (Exception ex)
            {
                string messGE = ex.Message;
                return;
            }
        }

        private RelayCommand<DataGridColumnEventArgs> _SortingCommand;
        public RelayCommand<DataGridColumnEventArgs> SortingCommand => _SortingCommand ?? (_SortingCommand = new RelayCommand<DataGridColumnEventArgs>(DataGridSorting));
        protected abstract void DataGridSorting(DataGridColumnEventArgs args);

    }
}
