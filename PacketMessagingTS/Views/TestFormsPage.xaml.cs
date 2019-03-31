using MetroLog;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;
using SharedCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestFormsPage : BaseFormsPage
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TestFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        public TestFormsViewModel _testFormsViewModel { get; } = Singleton<TestFormsViewModel>.Instance;

        public TestFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            _formControlAttributeList.AddRange(_attributeListTypeHospital);
            PopulateFormsPagePivot();
        }

        protected override int GetFormsPagePivotSelectedIndex()
        {
            return _testFormsViewModel.TestFormsPagePivotSelectedIndex;
        }

        protected override void SetFormsPagePivotSelectedIndex(int index)
        {
            _testFormsViewModel.TestFormsPagePivotSelectedIndex = index;
        }

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _testFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }
    }
}
