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

using System.Windows.Input;
using FormControlBaseClass;
using PacketMessagingTS.Controls;
using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class FormsViewModel : BaseViewModel
    {
        protected int formsPagePivotSelectedIndex;
        public int FormsPagePivotSelectedIndex
        {
            get => GetProperty(ref formsPagePivotSelectedIndex);
            set => SetProperty(ref formsPagePivotSelectedIndex, value, true);
        }

        public FormControlBase PacketForm
        { get; set; }

        private ICommand _PrintFormCommand;
        public ICommand PrintFormCommand => _PrintFormCommand ?? (_PrintFormCommand = new RelayCommand(PrintForm));

        public void PrintForm()
        {
            PacketForm.PrintForm();
        }

    }
}
