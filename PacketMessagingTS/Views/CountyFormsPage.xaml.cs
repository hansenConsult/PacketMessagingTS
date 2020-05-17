using System;
using FormControlBaseClass;
using Microsoft.Toolkit.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Views
{
    public sealed partial class CountyFormsPage : BaseFormsPage
    {
        public CountyFormsViewModel _CountyFormsViewModel { get; } = Singleton<CountyFormsViewModel>.Instance;

        protected override int FormsPagePivotSelectedIndex
        {
            get => _CountyFormsViewModel.CountyFormsPagePivotSelectedIndex;
            set => _CountyFormsViewModel.CountyFormsPagePivotSelectedIndex = value;
        }


        public CountyFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeNone);
            _formControlAttributeList.AddRange(_attributeListTypeCounty);
            PopulateFormsPagePivot();
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _CountyFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }
    }
}
