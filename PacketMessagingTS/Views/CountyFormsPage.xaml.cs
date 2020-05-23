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

        protected override int FormsPagePivotSelectedIndex
        {
            get => _CountyFormsViewModel.CountyFormsPagePivotSelectedIndex;
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

        //public override void ScanFormAttributes(FormControlAttribute.FormType[] formTypes)
        //{
        //    //AppDomain currentDomain = AppDomain.CurrentDomain;
        //    //Assembly[] assemblies = currentDomain.GetAssemblies();
        //    //System.Reflection.Assembly[] AppDomain.GetAssemblies
        //    //foreach (Assembly assembly in assemblies)
        //    foreach (Assembly assembly in SharedData.Assemblies)
        //    {
        //        //if (!assembly.FullName.Contains("FormControl"))
        //        //    continue;

        //        Type[] expTypes =  assembly.GetExportedTypes();
        //        _logHelper.Log(LogLevel.Info, $"Assembly: {assembly}");
        //        Type[] types = assembly.GetTypes();
        //        _logHelper.Log(LogLevel.Info, $"Types Count: {expTypes.Length}");
        //        foreach (Type classType in expTypes)
        //        {
        //            _logHelper.Log(LogLevel.Info, $"Type: {classType.Name}");
        //            var attrib = classType.GetTypeInfo();
        //            //foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
        //            foreach (CustomAttributeData customAttribute in attrib.CustomAttributes)
        //            {
        //                _logHelper.Log(LogLevel.Info, $"CustomAttributeData: {customAttribute}");
        //                //if (!(customAttribute is FormControlAttribute))
        //                //    continue;
        //                IList<CustomAttributeNamedArgument> namedArguments = customAttribute.NamedArguments;
        //                if (namedArguments.Count == 3)
        //                {
        //                    bool formControlTypeFound = false;
        //                    string formControlName = "";
        //                    FormControlAttribute.FormType formControlType = FormControlAttribute.FormType.Undefined;
        //                    string formControlMenuName = "";
        //                    foreach (CustomAttributeNamedArgument arg in namedArguments)
        //                    {
        //                        if (arg.MemberName == "FormControlName")
        //                        {
        //                            formControlName = arg.TypedValue.Value as string;
        //                        }
        //                        else if (arg.MemberName == "FormControlType")
        //                        {
        //                            formControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), arg.TypedValue.Value.ToString());
        //                            for (int i = 0; i < formTypes.Length; i++)
        //                            {
        //                                if (formControlType == formTypes[i])
        //                                {
        //                                    formControlTypeFound = true;
        //                                    break;
        //                                }
        //                            }
        //                            //if (!formControlTypeFound)
        //                            //    continue;
        //                        }
        //                        else if (arg.MemberName == "FormControlMenuName")
        //                        {
        //                            formControlMenuName = arg.TypedValue.Value as string;
        //                        }
        //                    }
        //                    //formControlName = namedArguments[0].TypedValue.Value as string;
        //                    //string formControlName = namedArguments[0].TypedValue.Value as string;
        //                    //_logHelper.Log(LogLevel.Info, $"formControlName: {formControlName}");
        //                    //_logHelper.Log(LogLevel.Info, $"formControlType: {namedArguments[1].TypedValue.Value as string}");
        //                    //FormControlAttribute.FormType formControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), namedArguments[1].TypedValue.Value.ToString());
        //                    //FormControlAttribute.FormType formControlType = FormControlAttribute.FormType.CountyForm;
        //                    //bool formControlTypeFound = false;
        //                    //for (int i = 0; i < formTypes.Length; i++)
        //                    //{
        //                    //    if (formControlType == formTypes[i])
        //                    //    {
        //                    //        formControlTypeFound = true;
        //                    //        break;
        //                    //    }
        //                    //}
        //                    if (!formControlTypeFound)
        //                        continue;
        //                    //string formControlMenuName = namedArguments[2].TypedValue.Value as string;
        //                    FormControlAttributes formControlAttributes = new FormControlAttributes(formControlName, formControlMenuName, formControlType, null);
        //                    _formControlAttributeList.Add(formControlAttributes);
        //                }
        //            }
        //        }
        //        //_logHelper.Log(LogLevel.Info, $"Attributelist Count: {_formControlAttributeList.Count}");
        //    }

        //    //// Pick latest file version for each type
        //    //for (int i = 0; i<_formControlAttributeList.Count; i++)
        //    //{
        //    //    for (int j = i + 1; j<_formControlAttributeList.Count; j++)
        //    //    {
        //    //        if (_formControlAttributeList[i].FormControlName == _formControlAttributeList[j].FormControlName)
        //    //        {
        //    //            // Should be version rather than creation date
        //    //            if (_formControlAttributeList[i].FormControlFile.DateCreated > _formControlAttributeList[j].FormControlFile.DateCreated)
        //    //            {
        //    //                _formControlAttributeList.Remove(_formControlAttributeList[j]);
        //    //            }
        //    //            else
        //    //            {
        //    //                _formControlAttributeList.Remove(_formControlAttributeList[i]);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    // Sort by menu type
        //    foreach (FormControlAttributes formControlAttributes in _formControlAttributeList)
        //    {
        //        if (formControlAttributes.FormControlType == FormControlAttribute.FormType.None)
        //        {
        //            _formControlAttributeList0.Add(formControlAttributes);
        //        }
        //        else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CountyForm)
        //        {
        //            _formControlAttributeList1.Add(formControlAttributes);
        //        }
        //    }
        //    _formControlAttributeList.Clear();
        //}

        protected override void SetAppBarSendIsEnabled(bool isEnabled)
        {
            _CountyFormsViewModel.IsAppBarSendEnabled = isEnabled;
        }
    }
}
