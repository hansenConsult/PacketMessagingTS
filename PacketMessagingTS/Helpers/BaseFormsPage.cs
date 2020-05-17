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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FormControlBaseClass;
using FormControlBasicsNamespace;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;
using static SharedCode.Helpers.MessageOriginHelper;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using HavBedReportFormControl;
//using Microsoft.Toolkit.Uwp.Helpers;

namespace PacketMessagingTS.Helpers
{
    public class FormControlAttributes
    {
        public string FormControlName
        { get; private set; }

        public string FormControlMenuName
        { get; private set; }

        public FormControlAttribute.FormType FormControlType
        { get; private set; }

        public StorageFile FormControlFileName
        { get; set; }

        public FormControlAttributes(string formControlType, string formControlMenuName, FormControlAttribute.FormType formType, StorageFile formControlFileName)
        {
            FormControlName = formControlType;
            FormControlMenuName = formControlMenuName;
            FormControlType = formType;
            FormControlFileName = formControlFileName;
        }
    }

    public abstract class BaseFormsPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BaseFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        //public FormsViewModel _formsViewModel { get; } = Singleton<FormsViewModel>.Instance;
        //public FormsViewModel _formsViewModel { get; set; }

        protected MessageOrigin _messageOrigin = MessageOrigin.New;

        protected Pivot _formsPagePivot;
        protected PacketMessage _packetMessage;
        protected bool _loadMessage = false;

        protected SendFormDataControl _packetAddressForm;

        protected List<FormControlAttributes> _attributeListTypeNone = new List<FormControlAttributes>();
        protected List<FormControlAttributes> _attributeListTypeCounty = new List<FormControlAttributes>();
        protected List<FormControlAttributes> _attributeListTypeCity = new List<FormControlAttributes>();
        protected List<FormControlAttributes> _attributeListTypeHospital = new List<FormControlAttributes>();
        protected List<FormControlAttributes> _attributeListTypeTestForms = new List<FormControlAttributes>();

        protected List<FormControlAttributes> _formControlAttributeList;

        FormControlBase _packetForm;
        public FormControlBase PacketForm
        {
            get => _packetForm;
        }

        public string MessageNo
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.MessageNo;
                else
                    return _packetForm.MessageNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.MessageNo = value;
                    _packetForm.MessageNo = value;
                }
                else
                    _packetForm.MessageNo = value;
            }
        }

        public string DestinationMsgNo
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.DestinationMsgNo;
                else
                    return _packetForm.DestinationMsgNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                    _packetForm.FormHeaderControl.DestinationMsgNo = value;
                else
                    _packetForm.DestinationMsgNo = value;
            }
        }

        public string OriginMsgNo
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.OriginMsgNo;
                else
                    return _packetForm.OriginMsgNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                    _packetForm.FormHeaderControl.OriginMsgNo = value;
                else
                    _packetForm.OriginMsgNo = value;
            }
        }
        
        public string HandlingOrder
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.HandlingOrder;
                else
                    return _packetForm.HandlingOrder;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                    _packetForm.FormHeaderControl.HandlingOrder = value;
                else
                    _packetForm.HandlingOrder = value;
            }
        }

        public string MsgDate
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.MsgDate;
                else
                    return _packetForm.MsgDate;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.MsgDate = value;
                    _packetForm.MsgDate = value;
                }
                else
                    _packetForm.MsgDate = value;
            }
        }

        public string OperatorName
        {
            get => Singleton<IdentityViewModel>.Instance.UserName;
            set
            {
                if (_packetForm.RadioOperatorControl != null)
                    _packetForm.RadioOperatorControl.OperatorName = value;
                else
                    _packetForm.OperatorName = value;
            }
        }

        public string OperatorCallsign
        {
            get => Singleton<IdentityViewModel>.Instance.UserCallsign;
            set
            {
                if (_packetForm.RadioOperatorControl != null)
                    _packetForm.RadioOperatorControl.OperatorCallsign = value;
                else
                    _packetForm.OperatorCallsign = value;
            }
        }

        public BaseFormsPage()
        {
            _formControlAttributeList = new List<FormControlAttributes>();
            ScanFormAttributes();
            _formControlAttributeList.Clear();
        }

        private PivotItem CreatePivotItem(FormControlAttributes formControlAttributes)
        {
            PivotItem pivotItem = new PivotItem();
            pivotItem.Name = formControlAttributes.FormControlName;
            pivotItem.Header = formControlAttributes.FormControlMenuName;

            ScrollViewer scrollViewer = new ScrollViewer
            {
                Margin = new Thickness(0, 12, -12, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = double.NaN
            };

            StackPanel stackpanel = new StackPanel();
            stackpanel.Name = pivotItem.Name + "Panel";
            scrollViewer.Content = stackpanel;

            pivotItem.Content = scrollViewer;

            return pivotItem;
        }

        protected void PopulateFormsPagePivot()
        {
            foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
            {
                if (string.IsNullOrEmpty(formControlAttribute.FormControlMenuName))
                {
                    continue;
                }
                PivotItem pivotItem = CreatePivotItem(formControlAttribute);
                _formsPagePivot.Items.Add(pivotItem);
            }
        }

        private void ScanFormAttributes()
        {
            IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
            if (files is null)
                return;

            foreach (StorageFile file in files.Where(file => file.FileType == ".dll" && file.Name.Contains("FormControl.dll")))
            {
                try
                {
                    Assembly assembly = Assembly.Load(new AssemblyName(file.DisplayName));
                    foreach (Type classType in assembly.GetTypes())
                    {
                        var attrib = classType.GetTypeInfo();
                        foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                        {
                            //if (!(customAttribute is FormControlAttribute))
                            //    continue;
                            var namedArguments = customAttribute.NamedArguments;
                            if (namedArguments.Count == 3)
                            {
                                string formControlType = namedArguments[0].TypedValue.Value as string;
                                FormControlAttribute.FormType FormControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), namedArguments[1].TypedValue.Value.ToString());
                                string formControlMenuName = namedArguments[2].TypedValue.Value as string;
                                FormControlAttributes formControlAttributes = new FormControlAttributes(formControlType, formControlMenuName, FormControlType, file);
                                _formControlAttributeList.Add(formControlAttributes);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            // Pick latest file version for each type
            for (int i = 0; i < _formControlAttributeList.Count; i++)
            {
                for (int j = i + 1; j < _formControlAttributeList.Count; j++)
                {
                    if (_formControlAttributeList[i].FormControlName == _formControlAttributeList[j].FormControlName)
                    {
                        // Should be version rather than creation date
                        if (_formControlAttributeList[i].FormControlFileName.DateCreated > _formControlAttributeList[j].FormControlFileName.DateCreated)
                        {
                            _formControlAttributeList.Remove(_formControlAttributeList[j]);
                        }
                        else
                        {
                            _formControlAttributeList.Remove(_formControlAttributeList[i]);
                        }
                    }
                }
            }
            // Sort by menu type
            foreach (FormControlAttributes formControlAttributes in _formControlAttributeList)
            {
                if (formControlAttributes.FormControlType == FormControlAttribute.FormType.None)
                {
                    _attributeListTypeNone.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CountyForm)
                {
                    _attributeListTypeCounty.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CityForm)
                {
                    _attributeListTypeCity.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.HospitalForm)
                {
                    _attributeListTypeHospital.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.TestForm)
                {
                    _attributeListTypeTestForms.Add(formControlAttributes);
                }
            }
        }

        protected abstract int FormsPagePivotSelectedIndex
        { get; set; }

        protected abstract void SetAppBarSendIsEnabled(bool isEnabled);

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
        }

        protected string CreateSubject()
        {
            return ValidateSubject(_packetForm.CreateSubject());
        }

        private void CreatePacketMessage(MessageState messageState = MessageState.Locked, FormProviders formProvider = FormProviders.PacForm)
        {
            _packetMessage = new PacketMessage()
            {
                FormControlType = _packetForm.FormControlType,
                BBSName = _packetAddressForm.MessageBBS,
                TNCName = _packetAddressForm.MessageTNC,
                FormFieldArray = _packetForm.CreateFormFieldsInXML(),
                FormProvider = _packetForm.FormProvider,
                PacFormName = _packetForm.PacFormName,
                PacFormType = _packetForm.PacFormType,
                MessageFrom = _packetAddressForm.MessageFrom,
                MessageTo = _packetAddressForm.MessageTo,
                CreateTime = DateTime.Now,
                MessageState = messageState,
            };

            //if (_packetForm.FormHeaderControl != null)
            //    _packetMessage.MessageNumber = _packetForm.FormHeaderControl.OriginMsgNo;
            _packetMessage.MessageNumber = OriginMsgNo;
            //else
            //    _packetMessage.MessageNumber = _packetForm.OriginMsgNo;


            UserAddressArray.Instance.AddAddressAsync(_packetMessage.MessageTo);
            //string subject = ValidateSubject(_packetForm.CreateSubject());  // TODO use CreateSubject
            string subject = CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            _packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            if (!_packetMessage.CreateFileName())
            {
                throw new Exception();
            }
        }

        public void FillFormFromPacketMessage()
        {
            //_packetForm.FormProvider = _packetMessage.FormProvider;
            _packetAddressForm.MessageBBS = _packetMessage.BBSName;
            _packetAddressForm.MessageTNC = _packetMessage.TNCName;
            _packetForm.FillFormFromFormFields(_packetMessage.FormFieldArray);
            _packetAddressForm.MessageFrom = _packetMessage.MessageFrom;
            _packetAddressForm.MessageTo = _packetMessage.MessageTo;
            _packetAddressForm.MessageSubject = _packetMessage.Subject;

            //string opcall = _packetForm.OperatorCallsign;//test
            // Special handling for SimpleMessage
            //_packetForm.MessageNo = _packetMessage.MessageNumber;
            MessageNo = _packetMessage.MessageNumber;
            _packetForm.MessageReceivedTime = _packetMessage.ReceivedTime;
            if (_packetMessage.MessageOrigin == MessageOrigin.Received)
            {
                _packetForm.MessageSentTime = _packetMessage.JNOSDate;
                _packetForm.ReceivedOrSent = "Receiver";
                if (_packetForm.FormProvider == FormProviders.PacItForm && _packetForm.PacFormType == "ICS213")
                {
                    MessageNo = _packetMessage.MessageNumber;
                    _packetForm.SenderMsgNo = _packetMessage.SenderMessageNumber;
                }
                else
                {
                    //_packetForm.DestinationMsgNo = _packetMessage.MessageNumber;
                    DestinationMsgNo = _packetMessage.MessageNumber;
                    //_packetForm.OriginMsgNo = _packetMessage.SenderMessageNumber;
                    OriginMsgNo = _packetMessage.SenderMessageNumber;
                }
            }
            else if (_packetMessage.MessageOrigin == MessageOrigin.Sent)
            {
                _packetForm.MessageSentTime = _packetMessage.SentTime;
                //_packetForm.DestinationMsgNo = _packetMessage.ReceiverMessageNumber;
                DestinationMsgNo = _packetMessage.ReceiverMessageNumber;
                //_packetForm.OriginMsgNo = _packetMessage.MessageNumber;
                OriginMsgNo = _packetMessage.MessageNumber;
                _packetForm.ReceivedOrSent = "Sender";

            }
            else if (_packetMessage.MessageOrigin == MessageOrigin.New)
            {
                _packetForm.MessageSentTime = null;
                _packetForm.MessageReceivedTime = _packetMessage.CreateTime;
                //_packetForm.OriginMsgNo = _packetMessage.MessageNumber;
                OriginMsgNo = _packetMessage.MessageNumber;
            }
        }

        public static FormControlBase CreateFormControlInstance(string controlName)
        {
            FormControlBase formControl = null;
            IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
            if (files is null)
                return null;

            Type foundType = null;
            foreach (var file in files.Where(file => file.FileType == ".dll" && file.Name.Contains("FormControl.dll")))
            {
                try
                {
                    Assembly assembly = Assembly.Load(new AssemblyName(file.DisplayName));
                    foreach (Type classType in assembly.GetTypes())
                    {
                        var attrib = classType.GetTypeInfo();
                        foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                        {
                            var namedArguments = customAttribute.NamedArguments;
                            if (namedArguments.Count == 3)
                            {
                                var formControlType = namedArguments[0].TypedValue.Value as string;
                                if (formControlType == controlName)
                                {
                                    foundType = classType;
                                    break;
                                }
                            }
                        }
                        if (foundType != null)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (foundType != null)
                    break;
            }

            if (foundType != null)
            {
                try
                {
                    formControl = (FormControlBase)Activator.CreateInstance(foundType);
                }
                catch (Exception e)
                {
                    string message = e.Message;
                }
            }
            return formControl;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is null)
            {
                // Open last used empty form
                //_formsPagePivot.SelectedIndex = GetFormsPagePivotSelectedIndex();
                _formsPagePivot.SelectedIndex = FormsPagePivotSelectedIndex;
                _packetMessage = null;
                return;
            }

            // Open a form with content
            int index = 0;
            string packetMessagePath = e.Parameter as string;
            _packetMessage = PacketMessage.Open(packetMessagePath);
            if (_packetMessage is null)
            {
                _logHelper.Log(LogLevel.Error, $"Failed to open {packetMessagePath}");
                return;
            }
            else
            {
                _packetMessage.MessageOpened = true;
                string directory = Path.GetDirectoryName(packetMessagePath);
                _loadMessage = true;
                foreach (PivotItem pivotItem in _formsPagePivot.Items)
                {
                    if (pivotItem.Name == _packetMessage.PacFormName) // If PacFormType is not set
                    {
                        _formsPagePivot.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                // Show SimpleMessage header formatted by where the message came from
                if (_packetMessage.PacFormName == "SimpleMessage")
                {
                    if (directory.Contains("Received"))
                    {
                        _messageOrigin = MessageOrigin.Received;
                    }
                    else if (directory.Contains("Sent"))
                    {
                        _messageOrigin = MessageOrigin.Sent;
                    }
                    else
                    {
                        _messageOrigin = MessageOrigin.New;
                    }
                }
                _packetMessage.Save(directory);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //SetFormsPagePivotSelectedIndex(_formsPagePivot.SelectedIndex);
            FormsPagePivotSelectedIndex = _formsPagePivot.SelectedIndex;

            base.OnNavigatedFrom(e);
        }

        private async Task InitializeFormControlAsync()
        {
            PivotItem pivotItem = _formsPagePivot.SelectedItem as PivotItem;
            string pivotItemName = pivotItem.Name;

            _packetAddressForm = new SendFormDataControl();
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");

                return;
            }
            //_formsViewModel.PacketForm = _packetForm;

            _packetForm.UpdateFormFieldsRequiredColors(true);

            MessageNo = Utilities.GetMessageNumberPacket();
            OriginMsgNo = _packetForm.MessageNo;

            DateTime now = DateTime.Now;
            MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
            //_packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
            OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
            OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                _packetForm.TacticalCallsign = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
            }

            StackPanel stackPanel = ((ScrollViewer)pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                stackPanel.Children.Insert(0, _packetAddressForm);
                stackPanel.Children.Insert(1, _packetForm);

                _packetAddressForm.MessageSubject = $"{MessageNo}_R_<subject>";

                (_packetForm as MessageControl).NewHeaderVisibility = true;
                _packetForm.MessageReceivedTime = DateTime.Now;
            }
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            }
        }

        void FormControl_MsgTimeChanged(object sender, FormEventArgs e)
        {
            _packetForm.MsgTimeChanged(e.SubjectLine);
        }

        void FormControl_SubjectChange(object sender, FormEventArgs e)
        {
            if (e?.SubjectLine?.Length > 0) // Why this test?
            {
                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
                if (_packetMessage != null)
                {
                    _packetMessage.Subject = _packetAddressForm.MessageSubject;
                }
            }
        }

        // TODO insert InitializeFormControlAsync, maybe
        public async void FormsPagePivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _packetAddressForm = new SendFormDataControl(_loadMessage);

            string practiceSubject = Singleton<PacketSettingsViewModel>.Instance.DefaultSubject;

            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            string pivotItemName = pivotItem.Name;
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }
            //_formsViewModel.PacketForm = _packetForm;

            _packetForm.FormPacketMessage = _packetMessage;
            MessageNo = Utilities.GetMessageNumberPacket();
            OriginMsgNo = MessageNo;

            StackPanel stackPanel = ((ScrollViewer)pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                stackPanel.Children.Insert(0, _packetAddressForm);
                stackPanel.Children.Insert(1, _packetForm);

                _packetAddressForm.MessageSubject = $"{MessageNo}_R_";
                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {
                    _packetAddressForm.MessageSubject += practiceSubject;
                }
                else if (Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.Name == "Check-In"
                    || Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.Name == "Check-Out")
                {
                    string chechInOut;
                    if (Singleton<PacketSettingsViewModel>.Instance.CurrentProfile.Name == "Check-In")
                        chechInOut = "Check-In";
                    else
                        chechInOut = "Check-Out";
                    string userCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
                    string userName = Singleton<IdentityViewModel>.Instance.UserName;
                    if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
                    {
                        string tacticalCallsign = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
                        string tacticalAgencyName = Singleton<IdentityViewModel>.Instance.TacticalAgencyName;
                        string messageSubject = $"{chechInOut} {tacticalCallsign}, {tacticalAgencyName}";
                        _packetAddressForm.MessageSubject += messageSubject;
                        _packetForm.MessageBody = $"{chechInOut} { tacticalCallsign}, {tacticalAgencyName} \r\nPresent are:\r\n{userCallsign}, {userName}\r\n";
                    }
                    else
                    {
                        string messageSubject = $"{chechInOut} {userCallsign}, {userName}";
                        _packetAddressForm.MessageSubject += messageSubject;
                        _packetForm.MessageBody = $"{chechInOut} {userCallsign}, {userName} \r\n";
                    }
                }
                _packetForm.MessageReceivedTime = DateTime.Now;
                switch (_messageOrigin)
                {
                    case MessageOrigin.Received:
                        (_packetForm as MessageControl).InBoxHeaderVisibility = true;
                        break;
                    case MessageOrigin.Sent:
                        (_packetForm as MessageControl).SentHeaderVisibility = true;
                        break;
                    default:
                        (_packetForm as MessageControl).NewHeaderVisibility = true;
                        break;
                }
            }
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            }

            if (!_loadMessage)
            {
                _packetForm.EventSubjectChanged += FormControl_SubjectChange;
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.EventSubjectChanged += FormControl_SubjectChange;
                    _packetForm.FormHeaderControl.EventMsgTimeChanged += FormControl_MsgTimeChanged;
                }

                DateTime now = DateTime.Now;
                MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                //_packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
                OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
                OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
                if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
                {
                    _packetForm.TacticalCallsign = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
                }

    //            _packetMessage.FormFieldArray = _packetForm.CreateFormFieldsInXML();
                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {                    
                    HandlingOrder = "routine";
                    switch (_packetForm.PacFormType)
                    {
                        case "ICS213":
                            _packetForm.Severity = "other";
                            _packetForm.Subject = practiceSubject;
                            break;
                        case "XSC_EOC_213RR":
                            _packetForm.IncidentName = practiceSubject;
                            break;
                        case "OA Municipal Status":
                            // Use Jurisdiction Name
                            break;
                        case "OAShelterStat":
                            _packetForm.ShelterName = practiceSubject;
                            break;
                        case "Allied_Health_Status":
                            _packetForm.FacilityName = practiceSubject;
                            break;
                    }                   
                }
            }
            else
            {
                FillFormFromPacketMessage();
                //appBarSend.IsEnabled = !(_packetMessage.MessageState == MessageState.Locked);
                SetAppBarSendIsEnabled(!(_packetMessage.MessageState == MessageState.Locked));

                _loadMessage = false;
            }

            //SetFormsPagePivotSelectedIndex(((Pivot)sender).SelectedIndex);
            FormsPagePivotSelectedIndex = ((Pivot)sender).SelectedIndex;
        }

        public async void AppBarClearForm_ClickAsync(object sender, RoutedEventArgs e)
        {
            await InitializeFormControlAsync();
        }

        public async void AppBarViewOutpostData_ClickAsync(object sender, RoutedEventArgs e)
        {
            //CreatePacketMessage();
            PacketMessage packetMessage = new PacketMessage()
            {
                FormFieldArray = _packetForm.CreateFormFieldsInXML(),
                FormProvider = _packetForm.FormProvider,
                PacFormName = _packetForm.PacFormName,
                PacFormType = _packetForm.PacFormType,
                //MessageNumber = _packetForm.MessageNo,
                MessageNumber = MessageNo,
                CreateTime = DateTime.Now,
            };
            string subject = CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            packetMessage.MessageBody = _packetForm.CreateOutpostData(ref packetMessage);

            bool copy = await ContentDialogs.ShowDualButtonMessageDialogAsync(packetMessage.MessageBody, "Copy", "Close", "Outpost Message");
            if (copy)
            {
                DataPackage dataPackage = new DataPackage
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(_packetMessage.MessageBody);
                Clipboard.SetContent(dataPackage);
            }
        }

        public void AppBarSave_Click(object sender, RoutedEventArgs e)
        {
            if (_packetMessage != null)     // Not a new message
            {
                // if the message state was locked it means it was previously sent or received.
                // We must assign a new message number
                if (_packetMessage.MessageState == MessageState.Locked)
                {
                    MessageNo = Utilities.GetMessageNumberPacket();
                    _packetMessage.MessageNumber = MessageNo;
                    _packetMessage.MessageState = MessageState.Edit;
                }
                _packetMessage.FormFieldArray = _packetForm.CreateFormFieldsInXML();       // Update fields
            }
            else
            {
                CreatePacketMessage(MessageState.None);
            }

            //_packetMessage.FormFieldArray = _packetForm.CreateFormFieldsInXML();       // Update fields.  Done above
            //if (_packetMessage.PacFormName == "SimpleMessage")
            //{
            //    FormField mesageBody = _packetMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == "messageBody");
            //    FormField richTextMessageBody = _packetMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == "richTextMessageBody");
            //    if (string.IsNullOrEmpty(richTextMessageBody.ControlContent))
            //    {
            //        richTextMessageBody.ControlContent = mesageBody.ControlContent;
            //    }
            //}
            _packetMessage.Save(SharedData.DraftMessagesFolder.Path);
            Utilities.MarkMessageNumberAsUsed();

            // Initialize to an empty form
            //await InitializeFormControlAsync();
        }

        public async void AppBarSend_ClickAsync(object sender, RoutedEventArgs e)
        {
            string validationResult = _packetForm.ValidateForm();
            validationResult = _packetAddressForm.ValidateForm(validationResult);
            if (!string.IsNullOrEmpty(validationResult))
            {
                validationResult += "\n\nAdd the missing information and press \"Send\" to continue.";
                await ContentDialogs.ShowSingleButtonContentDialogAsync(validationResult, "Close", "Missing input fields");
                return;
            }

            // append "Drill Traffic" if requested
            if (Singleton<PacketSettingsViewModel>.Instance.IsDrillTraffic)
            {
                _packetForm.AppendDrillTraffic();
            }

            CreatePacketMessage(MessageState.Edit);
            DateTime now = DateTime.Now;

            var operatorDateField = _packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
            if (operatorDateField != null)
            {
                operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{(now.Year):d4}";
            }
            var operatorTimeField = _packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
            if (operatorTimeField != null)
                operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

            Utilities.MarkMessageNumberAsUsed();
            _packetMessage.MessageOrigin = MessageOrigin.Sent;
            if (_packetMessage.PacFormName == "SimpleMessage")
            {
                // Copy messageBody to facilitate printing for long messages
                FormField formField = new FormField();
                formField.ControlName = "richTextMessageBody";
                formField.ControlContent = _packetMessage.FormFieldArray[0].ControlContent;
                _packetMessage.FormFieldArray[1] = formField;
            }

            //string subject = CreateSubject();
            //_packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            //_packetMessage.MessageBody = _packetForm.CreateOutpostData(ref _packetMessage);

            _packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

            Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            communicationsService.BBSConnectAsync2();

            // Create an empty form
            await InitializeFormControlAsync();
        }

        #region Print

        public void AppBarPrint_Click(object sender, RoutedEventArgs e)
        {
            _packetForm.PrintForm();
        }
        #endregion Print
    }
}
