using System.Collections.Generic;
using System.IO;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestFormsPage : BaseFormsPage
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<TestFormsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        public TestFormsViewModel _TestFormsViewModel { get; } = Singleton<TestFormsViewModel>.Instance;


        public TestFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            if (SharedData.FormControlAttributeTestList == null || SharedData.FormControlAttributeTestList.Count == 0)
            {
                _formControlAttributeList = new List<FormControlAttributes>();
                ScanFormAttributes(new FormControlAttribute.FormType[1] { FormControlAttribute.FormType.TestForm });

                _formControlAttributeList.AddRange(_formControlAttributeList0);

                SharedData.FormControlAttributeTestList = _formControlAttributeList;
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeTestList;
            }

            PopulateFormsPagePivot();

            _TestFormsViewModel.FormsPage = this;
            ViewModel = _TestFormsViewModel;
        }

    }
}
