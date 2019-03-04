﻿//*********************************************************
//
// Copyright (c) Hansen Consulting. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using PacketMessagingTS.Helpers;


namespace PacketMessagingTS.ViewModels
{
    public class HospitalFormsViewModel : BaseViewModel
    {
        private int hospitalFormsPagePivotSelectedIndex;
        public int HospitalFormsPagePivotSelectedIndex
        {
            get => GetProperty(ref hospitalFormsPagePivotSelectedIndex);
            set
            {
                SetProperty(ref hospitalFormsPagePivotSelectedIndex, value, true);
            }
        }

    }
}