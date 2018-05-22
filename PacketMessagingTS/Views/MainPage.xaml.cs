using System;
using System.Collections.Generic;
using FormControlBaseClass;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();

        }

        private void OpenMessage()
        {
            //if (_selectedMessages.Count == 1)
            //{
            //    string folder = ((StorageFolder)((PivotItem)MainPagePivot.SelectedItem).Tag).Path;
            //    string packetMessagePath = folder + @"\" + _selectedMessages[0].FileName;
            //    var nav = WindowWrapper.Current().NavigationServices.FirstOrDefault();
            //    nav.Navigate(typeof(FormsPage), packetMessagePath);
            //}
        }

        private async void AppBarMainPage_DeleteItemAsync(object sender, RoutedEventArgs e)
        {
            //PivotItem pivotItem = (PivotItem)MainPagePivot.SelectedItem;
            //StorageFolder folder = (StorageFolder)pivotItem.Tag;
            //bool permanentlyDelete = false;
            //if (folder == _deletedMessagesFolder)
            //{
            //    permanentlyDelete = true;
            //}

            //IList<Object> selectedMessages = null;
            //if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            //{
            //    if (pivotItem.Name == "Drafts")
            //    {
            //        //selectedMessages = _currentGridView.SelectedItems;
            //        selectedMessages = new List<Object>();

            //        foreach (var message in _selectedMessages)
            //        {
            //            selectedMessages.Add(message);
            //        }
            //    }
            //    else
            //        selectedMessages = (IList<Object>)_currentListView.SelectedItems;
            //}
            //else
            //{
            //    selectedMessages = _currentListView.SelectedItems;
            //}
            //foreach (PacketMessage packetMessage in selectedMessages)
            //{
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
            //        continue;
            //    }
            //}
            //RefreshListViewAsync(pivotItem);
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

    }
}
