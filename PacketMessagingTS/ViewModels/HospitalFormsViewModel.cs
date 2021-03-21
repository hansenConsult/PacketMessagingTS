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

using PacketMessagingTS.Helpers;

using SharedCode.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class HospitalFormsViewModel : FormsViewModel
    {
        public static HospitalFormsViewModel Instance { get; } = new HospitalFormsViewModel();

        public override int FormsPagePivotSelectedIndex
        {
            get => HospitalFormsPagePivotSelectedIndex;
            set => HospitalFormsPagePivotSelectedIndex = value;
        }

        private int hospitalFormsPagePivotSelectedIndex = -1;
        public int HospitalFormsPagePivotSelectedIndex
        {
            get
            {
                int index = GetProperty(ref hospitalFormsPagePivotSelectedIndex);
                if (index >= PublicData.FormControlAttributesInMenuOrderHospital.Length || index < 0)
                    index = 0;
                return index;
            }
            set => SetPropertyPrivate(ref hospitalFormsPagePivotSelectedIndex, value, true);
        }
    }
}
