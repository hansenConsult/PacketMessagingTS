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

        //public CountyFormsViewModel _CountyFormsViewModel { get; } = Singleton<CountyFormsViewModel>.Instance;
        private CountyFormsViewModel CountyFormsViewModel = CountyFormsViewModel.Instance;


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

                //SortAttributesByMenuIndex(PublicData.FormControlAttributesInMenuOrderCounty);
                //PublicData.FormControlAttributeCountyList = new List<FormControlAttributes>(); //Testing
                //foreach (FormControlAttributes attr in _formControlAttributeList)
                //{
                //    FormControlAttributes attr2 = new FormControlAttributes(attr.FormControlName, attr.FormControlMenuName, attr.FormControlType, attr.FormControlMenuIndex, attr.FormControlFile);
                //    PublicData.FormControlAttributeCountyList.Add(attr2);  // Testing
                //}
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeCountyList;
            }
            int indexCount = _formControlAttributeList.Count;
            PublicData.FormControlAttributesInMenuOrderCounty = new FormControlAttributes[indexCount];

            PopulateFormsPagePivot(PublicData.FormControlAttributesInMenuOrderCounty);

            CountyFormsViewModel.FormsPage = this;
            ViewModel = CountyFormsViewModel;
        }

        //private void SortAttributesByMenuIndex(FormControlAttributes[] formControlAttributesInMenuOrder)
        //{
        //    // Get a list of menuItems in order
        //    foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
        //    {
        //        if (formControlAttribute.FormControlMenuIndex < 0)
        //        {
        //            _logHelper.Log(LogLevel.Warn, $"Menu index is undefined for {formControlAttribute.FormControlName}");
        //            continue;
        //        }
        //        formControlAttributesInMenuOrder[formControlAttribute.FormControlMenuIndex] = formControlAttribute;
        //    }
        //}
    }
}
