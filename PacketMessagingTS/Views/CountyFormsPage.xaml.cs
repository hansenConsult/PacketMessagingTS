using System;
using System.Collections.Generic;
using System.Reflection;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class CountyFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CountyFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public CountyFormsViewModel _CountyFormsViewModel { get; } = Singleton<CountyFormsViewModel>.Instance;


        public CountyFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            if (SharedData.FormControlAttributeCountyList == null || SharedData.FormControlAttributeCountyList.Count == 0)
            {
                _formControlAttributeList = new List<FormControlAttributes>();
                ScanFormAttributes(new FormControlAttribute.FormType[2] { FormControlAttribute.FormType.None, FormControlAttribute.FormType.CountyForm });

                _formControlAttributeList.AddRange(_formControlAttributeList0);
                _formControlAttributeList.AddRange(_formControlAttributeList1);

                SharedData.FormControlAttributeCountyList = _formControlAttributeList;
                PublicData.FormControlAttributeCountyList = new List<FormControlAttributes2>(); //Testing
                foreach (FormControlAttributes attr in _formControlAttributeList)
                {
                    FormControlAttributes2 attr2 = new FormControlAttributes2(attr.FormControlName, attr.FormControlMenuName, attr.FormControlType, attr.FormControlFile);
                    PublicData.FormControlAttributeCountyList.Add(attr2);  // Testing
                }
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeCountyList;
            }
            PopulateFormsPagePivot();

            _CountyFormsViewModel.FormsPage = this;
            ViewModel = _CountyFormsViewModel;
        }

    }
}
