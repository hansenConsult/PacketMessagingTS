using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using ToggleButtonGroupControl;
using FormControlBaseClass;
using MetroLog;
using SharedCode;
using MessageFormControl;
using static SharedCode.Helpers.FormProvidersHelper;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HospitalFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<HospitalFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public HospitalFormsViewModel _hospitalFormsViewModel { get; } = Singleton<HospitalFormsViewModel>.Instance;


        public HospitalFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeHospital);
            PopulateFormsPagePivot();
        }

        protected override int GetFormsPagePivotSelectedIndex()
        {
            return _hospitalFormsViewModel.HospitalFormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _hospitalFormsViewModel.HospitalFormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _hospitalFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }

    }
}
