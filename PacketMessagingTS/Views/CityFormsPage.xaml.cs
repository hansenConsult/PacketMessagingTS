﻿using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CityFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CityFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public CityFormsViewModel _cityFormsViewModel { get; } = Singleton<CityFormsViewModel>.Instance;

        public CityFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeCity);
            PopulateFormsPagePivot();
        }

        protected override int FormsPagePivotSelectedIndex
        {
            get => _cityFormsViewModel.CityFormsPagePivotSelectedIndex;
            set => _cityFormsViewModel.CityFormsPagePivotSelectedIndex = value;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _cityFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
