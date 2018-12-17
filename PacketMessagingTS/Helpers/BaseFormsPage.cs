//*********************************************************
//
// Copyright (c) Hansen Consulting. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using PacketMessagingTS.ViewModels;
using SharedCode;
using System.IO;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PacketMessagingTS.Helpers
{
    public class BaseFormsPage : Page
    {
        Pivot _formsPagePivot;
        PacketMessage _packetMessage;
        bool _loadMessage = false;

        protected FormsViewModel _formsViewModel;

        protected PrintHelper printHelper;

        public BaseFormsPage()
        {
            _formsViewModel = new FormsViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Initialize common helper class and register for printing
            printHelper = new PrintHelper(this);
            printHelper.RegisterForPrinting();

            if (e.Parameter is null)
            {
                // Open an empty form
                _formsPagePivot.SelectedIndex = _formsViewModel.FormsPagePivotSelectedIndex;
                return;
            }

            // Open a form with content
            int index = 0;
            string packetMessagePath = e.Parameter as string;
            _packetMessage = PacketMessage.Open(packetMessagePath);
            _packetMessage.MessageOpened = true;
            string directory = Path.GetDirectoryName(packetMessagePath);
            _loadMessage = true;
            foreach (PivotItem pivotItem in _formsPagePivot.Items)
            {
                if (pivotItem.Name == _packetMessage.PacFormName) // If PacFormType is not set
                {
                    _formsPagePivot.SelectedIndex = index;
                    break;
                }
                index++;
            }
            // Show SimpleMessage header formatted by where the message came from
            _packetMessage.Save(directory);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (printHelper != null)
            {
                printHelper.UnregisterForPrinting();
            }
            _formsViewModel.FormsPagePivotSelectedIndex = _formsPagePivot.SelectedIndex;

            base.OnNavigatedFrom(e);
        }
    }
}
