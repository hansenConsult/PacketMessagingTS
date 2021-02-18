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

using SharedCode.Helpers;

using Windows.Storage;


namespace SharedCode.Helpers
{
    public class FormControlAttributes
    {
        public string FormControlName
        { get; private set; }

        public string FormControlMenuName
        { get; private set; }

        private int _FormControlMenuIndex = -1;
        public int FormControlMenuIndex
        {
            get => _FormControlMenuIndex;
            private set => _FormControlMenuIndex = value;
        }

        public FormControlAttribute.FormType FormControlType
        { get; private set; }

        public StorageFile FormControlFile
        { get; set; }

        public static int AttributesCount => 4;

        public FormControlAttributes(string formControlType, string formControlMenuName, FormControlAttribute.FormType formType, int formControlMenuIndex, StorageFile formControlFile)
        {
            FormControlName = formControlType;
            FormControlMenuName = formControlMenuName;
            FormControlMenuIndex = formControlMenuIndex;
            FormControlType = formType;
            FormControlFile = formControlFile;
        }
    }
}
