using System.Collections.Generic;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;


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

        private HospitalFormsViewModel HospitalFormsViewModel = HospitalFormsViewModel.Instance;


        public HospitalFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            if (SharedData.FormControlAttributeHospitalList == null || SharedData.FormControlAttributeHospitalList.Count == 0)
            {
                _formControlAttributeList = new List<FormControlAttributes>();
                ScanFormAttributes(new FormControlAttribute.FormType[1] { FormControlAttribute.FormType.HospitalForm});

                _formControlAttributeList.AddRange(_formControlAttributeList0);

                SharedData.FormControlAttributeHospitalList = _formControlAttributeList;
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeHospitalList;
            }
            //int indexCount = _formControlAttributeList.Count;
            //PublicData.FormControlAttributesInMenuOrderHospital = new FormControlAttributes[indexCount];

            //PopulateFormsPagePivot(PublicData.FormControlAttributesInMenuOrderHospital);
            PopulateFormsPagePivot(SharedData.FormControlAttributeHospitalList, FormMenuIndexDefinitions.Instance.HospitalFormsMenuNames);


            HospitalFormsViewModel.FormsPage = this;
            ViewModel = HospitalFormsViewModel;
        }

    }
}
