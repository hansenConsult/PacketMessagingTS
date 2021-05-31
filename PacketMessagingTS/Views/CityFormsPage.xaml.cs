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
    public sealed partial class CityFormsPage : BaseFormsPage
    {
        //private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<CityFormsPage>();
        //private static readonly LogHelper _logHelper = new LogHelper(log);

        private readonly CityFormsViewModel CityFormsViewModel = CityFormsViewModel.Instance;

        public CityFormsPage()
        {
            InitializeComponent();

            _formsPagePivot = formsPagePivot;

            if (SharedData.FormControlAttributeCityList == null || SharedData.FormControlAttributeCityList.Count == 0)
            {
                _formControlAttributeList = new List<FormControlAttributes>();
                ScanFormAttributes(new FormControlAttribute.FormType[1] { FormControlAttribute.FormType.CityForm });

                _formControlAttributeList.AddRange(_formControlAttributeList0);

                SharedData.FormControlAttributeCityList = _formControlAttributeList;
            }
            else
            {
                _formControlAttributeList = SharedData.FormControlAttributeCityList;
            }
            //int indexCount = _formControlAttributeList.Count;
            //PublicData.FormControlAttributesInMenuOrderCity = new FormControlAttributes[indexCount];

            //PopulateFormsPagePivot(PublicData.FormControlAttributesInMenuOrderCity);
            PopulateFormsPagePivot(SharedData.FormControlAttributeCityList, FormMenuIndexDefinitions.Instance.CityFormsMenuNames);

            CityFormsViewModel.FormsPage = this;
            ViewModel = CityFormsViewModel;
            //ViewModel = _cityFormsViewModel;
            //ViewModel.FormsPage = this;
        }

    }
}
