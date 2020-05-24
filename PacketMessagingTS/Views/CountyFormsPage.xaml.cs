using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FormControlBaseClass;

using MetroLog;

using Microsoft.Toolkit.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;

using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class CountyFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CountyFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public CountyFormsViewModel _CountyFormsViewModel { get; } = Singleton<CountyFormsViewModel>.Instance;
        //public FormsViewModel FormViewModel { get; } = Singleton<CountyFormsViewModel>.Instance;

        public override int FormsPagePivotSelectedIndex
        {
            get
            {
                int index = _CountyFormsViewModel.CountyFormsPagePivotSelectedIndex;
                //int index = FormViewModel.CountyFormsPagePivotSelectedIndex;
                if (index >= _formControlAttributeList.Count)
                    index = 0;
                return index;
            }
            set => _CountyFormsViewModel.CountyFormsPagePivotSelectedIndex = value;
        }


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
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeCountyList;
            }
            PopulateFormsPagePivot();
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _CountyFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }


    }
}
