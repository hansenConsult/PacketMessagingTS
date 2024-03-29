﻿//*********************************************************
//
// Copyright (c) Hansen Consulting. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using FormControlBaseClass;

using MetroLog;

using PacketMessagingTS.Controls;

using SharedCode;
using SharedCode.Helpers;
using static PacketMessagingTS.Core.Helpers.MessageOriginHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using PacketMessagingTS.ViewModels;
using PacketMessagingTS.Core.Helpers;

namespace PacketMessagingTS.Helpers
{
    public abstract class BaseFormsPage : Page
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BaseFormsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        protected MessageOrigin _messageOrigin = MessageOrigin.New;

        protected Pivot _formsPagePivot;

        protected SendFormDataControl _packetAddressForm;
        protected FormControlBase _packetForm;
        protected SimpleMessagePivot _simpleMessagePivot;

        protected List<FormControlAttributes> _formControlAttributeList0 = new List<FormControlAttributes>();
        protected List<FormControlAttributes> _formControlAttributeList1 = new List<FormControlAttributes>();

        protected List<FormControlAttributes> _formControlAttributeList;

        private PacketMessage _packetMessage;
        public PacketMessage PacketMessage
        {
            get => _packetMessage;
            set => _packetMessage = value;
        }

        public Pivot FormsPagePivot => _formsPagePivot;

        public FormsViewModel ViewModel
        { get; set; }


        //public BaseFormsPage()
        //{
        //}

        protected virtual PivotItem CreatePivotItem(FormControlAttributes formControlAttributes)
        {
            PivotItem pivotItem = new PivotItem
            {
                Name = formControlAttributes.FormControlName,
                Header = formControlAttributes.FormControlMenuName
            };

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Margin = new Thickness(0, 12, -12, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = double.NaN
            };

            StackPanel stackpanel = new StackPanel
            {
                Name = pivotItem.Name + "Panel"
            };
            scrollViewer.Content = stackpanel;

            pivotItem.Content = scrollViewer;

            return pivotItem;
        }

        //protected virtual void PopulateFormsPagePivot()
        //{
        //    foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
        //    {
        //        if (string.IsNullOrEmpty(formControlAttribute.FormControlMenuName))
        //        {
        //            continue;
        //        }
        //        PivotItem pivotItem = CreatePivotItem(formControlAttribute);
        //        _formsPagePivot.Items.Add(pivotItem);
        //    }
        //}

        protected virtual void PopulateFormsPagePivot(List<FormControlAttributes> formControlAttributes, string[] formControlMenuNames)
        {
            //FormControlAttributes[] formControlAttributesInMenuOrder = new FormControlAttributes[formControlMenuNames.Length];
            for (int i = 0; i < formControlMenuNames.Length; i++)
            {
                foreach (FormControlAttributes formControlAttribute in formControlAttributes)
                {
                    if (formControlMenuNames[i] == formControlAttribute.FormControlMenuName)
                    {
                        PivotItem pivotItem = CreatePivotItem(formControlAttribute);
                        _formsPagePivot.Items.Add(pivotItem);
                    }
                }
            }
        }

        //protected virtual void PopulateFormsPagePivot(FormControlAttributes[] formControlAttributesInMenuOrder)
        //{
        //    // Create an array in menu index order
        //    foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
        //    {
        //        if (formControlAttribute.FormControlMenuIndex < 0)
        //        {
        //            _logHelper.Log(LogLevel.Warn, $"Menu index is undefined for {formControlAttribute.FormControlName}");
        //            continue;
        //        }
        //        formControlAttributesInMenuOrder[formControlAttribute.FormControlMenuIndex] = formControlAttribute;
        //    }

        //    for (int i = 0; i < formControlAttributesInMenuOrder.Length; i++)
        //    {
        //        if (string.IsNullOrEmpty(formControlAttributesInMenuOrder[i].FormControlMenuName))
        //        {
        //            _logHelper.Log(LogLevel.Warn, $"Menu name is undefined for {formControlAttributesInMenuOrder[i].FormControlName}");
        //            continue;
        //        }
        //        PivotItem pivotItem = CreatePivotItem(formControlAttributesInMenuOrder[i]);
        //        _formsPagePivot.Items.Add(pivotItem);
        //    }
        //}

        public virtual void ScanFormAttributes(FormControlAttribute.FormType[] formTypes)
        {
            foreach (Assembly assembly in SharedData.Assemblies)
            {
                Type[] expTypes = assembly.GetExportedTypes();
                //_logHelper.Log(LogLevel.Info, $"Assembly: {assembly}");
                Type[] types = assembly.GetTypes();
                //_logHelper.Log(LogLevel.Info, $"Types Count: {expTypes.Length}");
                foreach (Type classType in expTypes)
                {
                    //_logHelper.Log(LogLevel.Info, $"Type: {classType.Name}");
                    var attrib = classType.GetTypeInfo();
                    //foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                    foreach (CustomAttributeData customAttribute in attrib.CustomAttributes)
                    {
                        //_logHelper.Log(LogLevel.Info, $"CustomAttributeData: {customAttribute}");
                        //if (customAttribute.AttributeType.Name != nameof(FormControlAttribute))
                        //    continue;
                        //Type formControlAttributeType = typeof(FormControlAttribute);
                        //Type customAttributeType = customAttribute.AttributeType;
                        if (customAttribute.AttributeType != typeof(FormControlAttribute))
                            continue;
                        IList<CustomAttributeNamedArgument> namedArguments = customAttribute.NamedArguments;
                        //if (namedArguments.Count == 4)
                        if (namedArguments.Count == FormControlAttributes.AttributesCount)
                        {
                            bool formControlTypeFound = false;
                            string formControlName = "";
                            FormControlAttribute.FormType formControlType = FormControlAttribute.FormType.Undefined;
                            string formControlMenuName = "";
                            foreach (CustomAttributeNamedArgument arg in namedArguments)
                            {
                                if (arg.MemberName == "FormControlName")
                                {
                                    formControlName = arg.TypedValue.Value as string;
                                }
                                else if (arg.MemberName == "FormControlType")
                                {
                                    formControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), arg.TypedValue.Value.ToString());
                                    for (int i = 0; i < formTypes.Length; i++)
                                    {
                                        if (formControlType == formTypes[i])
                                        {
                                            formControlTypeFound = true;
                                            break;
                                        }
                                    }
                                }
                                else if (arg.MemberName == "FormControlMenuName")
                                {
                                    formControlMenuName = arg.TypedValue.Value as string;
                                }
                            }
                            if (formControlTypeFound)
                            {
                                FormControlAttributes formControlAttributes = new FormControlAttributes(formControlName, formControlMenuName, formControlType);
                                _formControlAttributeList.Add(formControlAttributes);
                            }
                        }
                    }
                }
            }

            //// Pick latest file version for each type
            //for (int i = 0; i<_formControlAttributeList.Count; i++)
            //{
            //    for (int j = i + 1; j<_formControlAttributeList.Count; j++)
            //    {
            //        if (_formControlAttributeList[i].FormControlName == _formControlAttributeList[j].FormControlName)
            //        {
            //            // Should be version rather than creation date
            //            if (_formControlAttributeList[i].FormControlFile.DateCreated > _formControlAttributeList[j].FormControlFile.DateCreated)
            //            {
            //                _formControlAttributeList.Remove(_formControlAttributeList[j]);
            //            }
            //            else
            //            {
            //                _formControlAttributeList.Remove(_formControlAttributeList[i]);
            //            }
            //        }
            //    }
            //}
            // Sort by form type
            foreach (FormControlAttributes formControlAttributes in _formControlAttributeList)
            {
                if (formControlAttributes.FormControlType == formTypes[0])
                {
                    _formControlAttributeList0.Add(formControlAttributes);
                }
                else if (formTypes.Length == 2 && formControlAttributes.FormControlType == formTypes[1])
                {
                    _formControlAttributeList1.Add(formControlAttributes);
                }
            }
            _formControlAttributeList.Clear();
        }

        //public virtual void ScanFormAttributes(FormControlAttribute.FormType[] formTypes)
        //{
        //    //_logHelper.Log(LogLevel.Info, $"Installed file count: {SharedData.FilesInInstalledLocation.Count}");
        //    //foreach (StorageFile file in SharedData.FilesInInstalledLocation)
        //    //{
        //    //    _logHelper.Log(LogLevel.Info, $"Installed files: {file.Name}");
        //    //}

        //    // Not in UWP
        //    //AppDomain currentDomain = AppDomain.CurrentDomain;
        //    //Assembly[] assems = currentDomain.GetAssemblies();
        //    //System.Reflection.Assembly[] AppDomain.GetAssemblies
        //    //foreach (Assembly assembly in assems)
        //    //{
        //    //    if (!assembly.FullName.Contains("FormControl"))
        //    //        continue;

        //    //    _logHelper.Log(LogLevel.Info, $"Assembly: {assembly.ToString()}");
        //    //    Type[] types = assembly.GetTypes();
        //    //    _logHelper.Log(LogLevel.Info, $"Types Count: {types.Length}");
        //    //    foreach (Type classType in types)
        //    //    {
        //    //        var attrib = classType.GetTypeInfo();
        //    //        foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
        //    //        {
        //    //            //if (!(customAttribute is FormControlAttribute))
        //    //            //    continue;
        //    //            var namedArguments = customAttribute.NamedArguments;
        //    //            if (namedArguments.Count == 3)
        //    //            {
        //    //                string formControlType = namedArguments[0].TypedValue.Value as string;
        //    //                FormControlAttribute.FormType FormControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), namedArguments[1].TypedValue.Value.ToString());
        //    //                string formControlMenuName = namedArguments[2].TypedValue.Value as string;
        //    //                //FormControlAttributes formControlAttributes = new FormControlAttributes(formControlType, formControlMenuName, FormControlType, file);
        //    //                //_formControlAttributeList.Add(formControlAttributes);
        //    //            }
        //    //        }
        //    //    }
        //    //    _logHelper.Log(LogLevel.Info, $"Attributelist Count: {_formControlAttributeList.Count}");
        //    //}

        //    IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
        //    if (files is null)
        //        return;

        //    foreach (StorageFile file in files.Where(file => file.FileType == ".dll" && file.Name.Contains("FormControl.dll")))
        //    {
        //        try
        //        {
        //            Assembly assembly = Assembly.Load(new AssemblyName(file.DisplayName));
        //            Type[] types = assembly.GetTypes();
        //            //_logHelper.Log(LogLevel.Info, $"Types count: {assembly.ManifestModule}, {types.Length}");
        //            foreach (Type classType in types)
        //            {
        //                var attrib = classType.GetTypeInfo();
        //                foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
        //                {
        //                    //if (!(customAttribute is FormControlAttribute))
        //                    //    continue;
        //                    var namedArguments = customAttribute.NamedArguments;
        //                    if (namedArguments.Count == 3)
        //                    {
        //                        string formControlType = namedArguments[0].TypedValue.Value as string;
        //                        FormControlAttribute.FormType FormControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), namedArguments[1].TypedValue.Value.ToString());
        //                        string formControlMenuName = namedArguments[2].TypedValue.Value as string;
        //                        FormControlAttributes formControlAttributes = new FormControlAttributes(formControlType, formControlMenuName, FormControlType, file);
        //                        _formControlAttributeList.Add(formControlAttributes);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //        }
        //    }
        //    //// Pick latest file version for each type
        //    //for (int i = 0; i < _formControlAttributeList.Count; i++)
        //    //{
        //    //    for (int j = i + 1; j < _formControlAttributeList.Count; j++)
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
        //            _attributeListTypeNone.Add(formControlAttributes);
        //        }
        //        else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CountyForm)
        //        {
        //            _attributeListTypeCounty.Add(formControlAttributes);
        //        }
        //        else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CityForm)
        //        {
        //            _attributeListTypeCity.Add(formControlAttributes);
        //        }
        //        else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.HospitalForm)
        //        {
        //            _attributeListTypeHospital.Add(formControlAttributes);
        //        }
        //        else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.TestForm)
        //        {
        //            _attributeListTypeTestForms.Add(formControlAttributes);
        //        }
        //    }
        //}

        //public abstract int FormsPagePivotSelectedIndex
        //{ get; set; }

        //protected virtual void SetAppBarSendIsEnabled(bool isEnabled) { }           

        protected static string ValidateSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return subject;

            try
            {
                return Regex.Replace(subject, @"[^\w\.@-\\%/\-\ ,()]", "~",
                                     RegexOptions.Singleline, TimeSpan.FromSeconds(1.0));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
            //catch (ArgumentException)
            //{
            //    return string.Empty;
            //}
            catch (ArgumentNullException)
            {
                return string.Empty;
            }
            catch (ArgumentOutOfRangeException)
            {
                return string.Empty;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //_logHelper.Log(LogLevel.Trace, "Entering OnNavigatedTo in BaseFormsPage");
            if (e.Parameter is null)
            {
                PacketMessage = null;
                ViewModel.LoadMessage = false;
                base.OnNavigatedTo(e);
                return;
            }

            // Open a form with content
            string packetMessagePath = e.Parameter as string;
            PacketMessage = PacketMessage.Open(packetMessagePath);
            if (PacketMessage is null)
            {
                _logHelper.Log(LogLevel.Error, $"Failed to open {packetMessagePath}");
                ViewModel.LoadMessage = false;
                base.OnNavigatedTo(e);
                return;
            }
            else
            {
                PacketMessage.MessageOpened = true;
                string directory = Path.GetDirectoryName(packetMessagePath);
                ViewModel.LoadMessage = true;

                int index = 0;
                foreach (PivotItem pivotItem in _formsPagePivot.Items)
                {
                    if (pivotItem.Name == PacketMessage.PacFormName) // If PacFormType is not set
                    {
                        //_formsPagePivot.SelectedIndex = index;
                        //FormsPagePivotSelectedIndex = index;
                        ViewModel.FormsPagePivotSelectedIndex = index;
                        break;
                    }
                    index++;
                }
                //// Show SimpleMessage header formatted by where the message came from
                //if (_packetMessage.PacFormName == "SimpleMessage")
                //{
                //    if (_packetMessage.MessageOrigin == MessageOrigin.Received || directory.Contains("Received"))
                //    {
                //        _messageOrigin = MessageOrigin.Received;
                //    }
                //    else if (_packetMessage.MessageOrigin == MessageOrigin.Sent || directory.Contains("Sent"))
                //    {
                //        _messageOrigin = MessageOrigin.Sent;
                //    }
                //    else
                //    {
                //        _messageOrigin = MessageOrigin.New;
                //    }
                //}
                //ViewModel.MessageOrigin = _messageOrigin;
                PacketMessage.Save(directory);
            }
            base.OnNavigatedTo(e);
        }

    }
}
