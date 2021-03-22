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
        protected string _FormControlName;
        public string FormControlName
        { 
            get => _FormControlName; 
            set => _FormControlName = value; 
        }

        protected string _FormControlMenuName;
        public string FormControlMenuName
        {  
            get => _FormControlMenuName;  
            set => _FormControlMenuName = value; 
        }

        //protected int _FormControlMenuIndex = -1;
        //public int FormControlMenuIndex
        //{
        //    get => _FormControlMenuIndex;
        //    set => _FormControlMenuIndex = value;
        //}

        protected FormControlAttribute.FormType _FormControlType;
        public FormControlAttribute.FormType FormControlType
        { 
            get => _FormControlType; 
            set => _FormControlType = value; 
        }

        public static int AttributesCount => 3;

        //public FormControlAttributes(string formControlName, string formControlMenuName, FormControlAttribute.FormType formType, int formControlMenuIndex)
        public FormControlAttributes(string formControlName, string formControlMenuName, FormControlAttribute.FormType formType)
        {
            _FormControlName = formControlName;
            _FormControlMenuName = formControlMenuName;
            //_FormControlMenuIndex = formControlMenuIndex;
            _FormControlType = formType;
        }

        //public FormControlAttributes(string formControlType, string formControlMenuName, FormControlAttribute.FormType formType, int formControlMenuIndex, StorageFile formControlFile)
        //{
        //    FormControlName = formControlType;
        //    FormControlMenuName = formControlMenuName;
        //    FormControlMenuIndex = formControlMenuIndex;
        //    FormControlType = formType;
        //    FormControlFile = formControlFile;
        //}
    }
}
