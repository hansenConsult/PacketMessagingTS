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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using FormControlBaseClass;

using FormControlBasicsNamespace;

using MessageFormControl;

using CommunityToolkit.Mvvm.Input;

using MetroLog;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Services.CommunicationsService;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using Windows.UI.Xaml.Documents;

namespace PacketMessagingTS.ViewModels
{
    public class FormsViewModel : ViewModelBase
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BaseFormsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        protected PivotItem _pivotItem;
        protected PacketMessage _packetMessage;
        protected SendFormDataControl _packetAddressForm;
        protected FormControlBase _packetForm;
        protected SimpleMessagePivot _simpleMessagePivot;


        public string MessageNo
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.MessageNo
                    : _packetForm.ViewModelBase.MessageNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.MessageNo = value;
                    _packetForm.ViewModelBase.MessageNo = value;
                }
                else
                    _packetForm.ViewModelBase.MessageNo = value;
            }
        }

        public string DestinationMsgNo
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.DestinationMsgNo
                    : _packetForm.ViewModelBase.DestinationMsgNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.DestinationMsgNo = value;
                }
                else
                {
                    _packetForm.ViewModelBase.DestinationMsgNo = value;
                }
            }
        }

        public string OriginMsgNo
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.OriginMsgNo
                    : _packetForm.ViewModelBase.OriginMsgNo;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.OriginMsgNo = value;
                }
                else
                {
                    _packetForm.ViewModelBase.OriginMsgNo = value;
                }
            }
        }

        public string MsgDate
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.MsgDate
                    : _packetForm.ViewModelBase.MsgDate;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.MsgDate = value;
                    if (_packetForm.ViewModelBase != null)
                    {
                        _packetForm.ViewModelBase.MsgDate = value;
                    }
                }
                else
                {
                    _packetForm.ViewModelBase.MsgDate = value;
                }
            }
        }

        public string MsgTime
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.MsgTime
                    : _packetForm.ViewModelBase.MsgTime;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.MsgTime = value;
                }
                else
                {
                    _packetForm.ViewModelBase.MsgTime = value;
                }
            }
        }

        public string OperatorName
        {
            get => IdentityViewModel.Instance.UserName;
            set
            {
                if (_packetForm.RadioOperatorControl != null)
                {
                    _packetForm.RadioOperatorControl.ViewModel.OperatorName = value;
                }
                else
                {
                    _packetForm.ViewModelBase.OperatorName = value;
                }
            }
        }

        public string OperatorCallsign
        {
            get => IdentityViewModel.Instance.UserCallsign;
            set
            {
                if (_packetForm.RadioOperatorControl != null)
                {
                    _packetForm.RadioOperatorControl.ViewModel.OperatorCallsign = value;
                }
                else
                {
                    _packetForm.ViewModelBase.OperatorCallsign = value;
                }
            }
        }

        public string HandlingOrder
        {
            get
            {
                return _packetForm.FormHeaderControl != null
                    ? _packetForm.FormHeaderControl.ViewModel.HandlingOrder
                    : _packetForm.ViewModelBase.HandlingOrder;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.ViewModel.HandlingOrder = value;
                }
                else
                {
                    _packetForm.ViewModelBase.HandlingOrder = value;
                }
            }
        }

        public override bool IsAppBarSendEnabled
        {
            //get => isAppBarSendEnabled;
            get
            {
                IsAppBarSendEnabled = _packetMessage is null || !(_packetMessage.MessageState == MessageState.Locked);

                return _isAppBarSendEnabled;
            }
            set => SetProperty(ref _isAppBarSendEnabled, value);
        }
        
        public BaseFormsPage FormsPage
        { get; set; }

        public bool LoadMessage { get; set; }

        //private MessageOriginHelper.MessageOrigin _messageOrigin = MessageOriginHelper.MessageOrigin.New;
        //public MessageOriginHelper.MessageOrigin MessageOrigin
        //{
        //    get => _messageOrigin;
        //    set => _messageOrigin = value;
        //}

        public FormControlBase PacketForm => _packetForm;

        public SendFormDataControl PacketAddressForm => _packetAddressForm;

        public virtual int FormsPagePivotSelectedIndex
        { get; set; }

        public static FormControlBase CreateFormControlInstance(string formControlName)
        {
            FormControlBase formControl = null;

            foreach (Assembly assembly in SharedData.Assemblies)
            {
                try
                {
                    foreach (Type classType in assembly.GetTypes())
                    {
                        Type attrib = classType.GetTypeInfo();

                        CustomAttributeData customAttribute = attrib.CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(FormControlAttribute));
                        if (customAttribute == null)
                        {
                            continue;
                        }

                        CustomAttributeNamedArgument arg = customAttribute.NamedArguments.FirstOrDefault(a => a.MemberName == "FormControlName");
                        if (formControlName == arg.TypedValue.Value as string)
                        {
                            formControl = (FormControlBase)Activator.CreateInstance(classType);
                            return formControl;
                        }
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    _logHelper.Log(LogLevel.Info, $"Exception: {ex.Message}");
                }
            }
            return formControl;

                //    //foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                //    foreach (CustomAttributeData customAttribute in attrib.CustomAttributes)
                //    {
                //        if (customAttribute.AttributeType != typeof(FormControlAttribute))
                //            continue;

                //        var namedArguments = customAttribute.NamedArguments;
                //        if (namedArguments.Count == FormControlAttributes.AttributesCount)
                //        {
                //            foreach (CustomAttributeNamedArgument arg in namedArguments)
                //            {
                //                if (arg.MemberName == "FormControlName")
                //                {
                //                    if (formControlName == arg.TypedValue.Value as string)
                //                    {
                //                        foundType = classType;
                //                        break;
                //                    }
                //                }
                //            }

                //            //    var formControlType = namedArguments[0].TypedValue.Value as string;
                //            //    if (formControlType == formControlName)
                //            //    {
                //            //        foundType = classType;
                //            //        break;
                //            //    }
                //            //}
                //        }
                //        if (foundType != null)
                //            break;
                //    }
                //}
            //}
            //    catch (Exception ex)
            //    {
            //        _logHelper.Log(LogLevel.Info, $"Exception: {ex.Message}");
            //        continue;
            //    }
            //    if (foundType != null)
            //        break;
            //}

            //if (foundType != null)
            //{
            //    try
            //    {
            //        formControl = (FormControlBase)Activator.CreateInstance(foundType);
            //        //object[] parameters = new object[] { messageState };
            //        //formControl = (FormControlBase)Activator.CreateInstance(foundType, parameters);
            //    }
            //    catch (Exception e)
            //    {
            //        _logHelper.Log(LogLevel.Info, $"Exception: {e.Message}");
            //    }
            //}
            //return formControl;
        }

        public async void InitializeFormControlAsync()
        {
            LoadMessage = false;
            await ShowPacketFormAsync();
            return;
        }

        public void FillFormFromPacketMessage()
        {
            //_packetForm.FormProvider = _packetMessage.FormProvider;
            //_packetAddressForm.MessageBBS = _packetMessage.BBSName;
            SendFormDataControlViewModel.Instance.MessageBBS = _packetMessage.BBSName;
            SendFormDataControlViewModel.Instance.MessageTNC = _packetMessage.TNCName;
            if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.Received)
            {
                _packetForm.ViewModelBase.ReceivedOrSent = "Receiver";
            }
            else
            {
                _packetForm.ViewModelBase.ReceivedOrSent = "Sender";
            }
            _packetForm.FillFormFromFormFields(_packetMessage.FormFieldArray);
            SendFormDataControlViewModel.Instance.MessageFrom = _packetMessage.MessageFrom;
            SendFormDataControlViewModel.Instance.MessageTo = _packetMessage.MessageTo;
            SendFormDataControlViewModel.Instance.MessageSubject = _packetMessage.Subject;

            //string opcall = _packetForm.OperatorCallsign;//test
            // Special handling for SimpleMessage
            MessageNo = _packetMessage.MessageNumber;
            if (_packetForm.PacFormType == "SimpleMessage")
            {
                (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageReceivedTime = _packetMessage.ReceivedTime;
                (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageCreatedTime = _packetMessage.CreateTime;
            }
            //_packetForm.MessageReceivedTime = _packetMessage.ReceivedTime;
            if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.Received)
            {
                if (_packetForm.PacFormType == "SimpleMessage")
                {
                    (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageSentTime = _packetMessage.JNOSDate;
                }
                _packetForm.ViewModelBase.ReceivedOrSent = "Receiver";
                if (_packetForm.FormProvider == FormProvidersHelper.FormProviders.PacItForm)
                {
                    DestinationMsgNo = _packetMessage.MessageNumber;
                    OriginMsgNo = _packetMessage.SenderMessageNumber;
                }
                else
                {
                    MessageNo = _packetMessage.MessageNumber;
                    _packetForm.ViewModelBase.SenderMsgNo = _packetMessage.SenderMessageNumber;
                }
                //if (_packetForm.FormProvider == FormProvidersHelper.FormProviders.PacItForm && _packetForm.PacFormType == "ICS213")
                //{
                //    MessageNo = _packetMessage.MessageNumber;
                //    _packetForm.ViewModelBase.SenderMsgNo = _packetMessage.SenderMessageNumber;
                //}
                //else
                //{
                //    DestinationMsgNo = _packetMessage.MessageNumber;
                //    OriginMsgNo = _packetMessage.SenderMessageNumber;
                //}
            }
            else if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.Sent)
            {
                if (_packetForm.PacFormType == "SimpleMessage")
                {
                    (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageSentTime = _packetMessage.SentTime;
                }
                DestinationMsgNo = _packetMessage.ReceiverMessageNumber;
                OriginMsgNo = _packetMessage.MessageNumber;
                _packetForm.ViewModelBase.ReceivedOrSent = "Sender";
            }
            else if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.New)
            {
                (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageSentTime = null;
                (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageReceivedTime = _packetMessage.ReceivedTime;
                OriginMsgNo = _packetMessage.MessageNumber;
            }
            if (_packetMessage.MessageState == MessageState.Locked)
            {
                _packetForm.LockForm();
                _packetAddressForm.LockForm();
            }
        }

        void FormControl_SubjectChange(object sender, FormEventArgs e)
        {
            if (e?.SubjectLine?.Length > 0) // Why this test?
            {
                SendFormDataControlViewModel.Instance.MessageSubject = _packetForm.CreateSubject();
                if (_packetMessage != null)
                {
                    _packetMessage.Subject = SendFormDataControlViewModel.Instance.MessageSubject;
                }
            }
        }

        void SimpleMessage_SubjectChange(object sender, FormEventArgs e)
        {
            SendFormDataControlViewModel.Instance.MessageSubject = $"{MessageNo}_R_{e?.SubjectLine}";
            
            if (_packetMessage != null)
            {
                _packetMessage.Subject = SendFormDataControlViewModel.Instance.MessageSubject;
            }
        }

        void FormControl_MessageChanged(object sender, FormEventArgs e)
        {
            _packetForm.MessageChanged(e.SubjectLine);
        }

        void FormControl_MsgTimeChanged(object sender, FormEventArgs e)
        {
            _packetForm.MsgTimeChanged(e.SubjectLine);
        }

        public void FormsPagePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int addedCount = e.AddedItems.Count;
            //int removedCount = e.RemovedItems.Count;
            //var item = e.AddedItems[0];

            int index = FormsPagePivotSelectedIndex;
            FormsPagePivotSelectionChangedAsync(index);
        }

        private async Task ShowPacketFormAsync()
        {
            MessageState messageState = MessageState.None;
            if (!LoadMessage)
            {
                _packetMessage = null;
            }
            else
            {
                _packetMessage = FormsPage.PacketMessage;
                messageState = FormsPage.PacketMessage.MessageState;
            }

            _packetAddressForm = new SendFormDataControl
            {
                FormPacketMessage = _packetMessage
            };

            string practiceSubject = PacketSettingsViewModel.Instance.DefaultSubject;

            string pivotItemName = _pivotItem.Name;
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }

            _packetForm.FormPacketMessage = _packetMessage;
            _packetForm.FormatTextBoxes();
            if (LoadMessage && _packetForm.FormPacketMessage.MessageState == MessageState.ResendSameID)
            {
                MessageNo = _packetForm.FormPacketMessage.MessageNumber;
            }
            else
            {
                MessageNo = Utilities.GetMessageNumberPacket();
            }
            OriginMsgNo = MessageNo;

            //formHeaderControl.ViewModelBase.OriginMsgNo

            // Populate the form View
            StackPanel stackPanel = ((ScrollViewer)_pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                if (LoadMessage)
                {
                    // Show existing message
                    stackPanel.Children.Insert(0, _packetAddressForm);
                    stackPanel.Children.Insert(1, _packetForm);

                    if (_packetMessage.MessageState == MessageState.ResendSameID || _packetMessage.MessageState == MessageState.ResendNewID)
                    {
                        //(_packetForm.ViewModelBase as MessageFormControlViewModel).MessageReceivedTime = DateTime.Now;     // Is inserted in the Created time field
                        (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageCreatedTime = DateTime.Now;
                    }

                    switch (_packetMessage.MessageOrigin)
                    {
                        case MessageOriginHelper.MessageOrigin.Received:
                            (_packetForm.ViewModelBase as MessageFormControlViewModel).InBoxHeaderVisibility = true;
                            break;
                        case MessageOriginHelper.MessageOrigin.Sent:
                            (_packetForm.ViewModelBase as MessageFormControlViewModel).SentHeaderVisibility = true;
                            break;
                        default:
                            (_packetForm.ViewModelBase as MessageFormControlViewModel).NewHeaderVisibility = true;
                            break;
                    }
                }
                else
                {
                    // Insert Pivot for message type
                    _simpleMessagePivot = new SimpleMessagePivot();

                    stackPanel.Children.Insert(0, _simpleMessagePivot);
                    stackPanel.Children.Insert(1, _packetAddressForm);
                    stackPanel.Children.Insert(2, _packetForm);

                    (_packetForm.ViewModelBase as MessageFormControlViewModel).NewHeaderVisibility = true;
                    //(_packetForm.ViewModelBase as MessageFormControlViewModel).MessageReceivedTime = DateTime.Now;     // Is inserted in the Created time field
                    (_packetForm.ViewModelBase as MessageFormControlViewModel).MessageCreatedTime = DateTime.Now;

                    _simpleMessagePivot.EventSimpleMsgSubjectChanged += SimpleMessage_SubjectChange;
                    _simpleMessagePivot.EventMessageChanged += FormControl_MessageChanged;
                }

                // Moved to SimpleMessagePivot control
                //_packetAddressForm.MessageSubject = $"{MessageNo}_R_";
                //if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                //{
                //    _packetAddressForm.MessageSubject += practiceSubject;
                //    //_packetForm.MessageBody = PacketSettingsViewModel>.Instance.DefaultMessage;
                //}
                //(_packetForm.ViewModelBase as MessageFormControlViewModel).MessageReceivedTime = DateTime.Now;
            }
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                SendFormDataControlViewModel.Instance.MessageSubject = _packetForm.CreateSubject();

                if (PacketSettingsViewModel.Instance.IsDrillTraffic && !LoadMessage && pivotItemName != "SimpleMessage")
                {
                    _packetForm.AppendDrillTraffic();
                }
            }

            if (!LoadMessage)
            {
                DateTime now = DateTime.Now;
                MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                //_packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
                //HandlingOrder = null;
                OperatorName = IdentityViewModel.Instance.UserName;
                OperatorCallsign = IdentityViewModel.Instance.UserCallsign;
                if (IdentityViewModel.Instance.UseTacticalCallsign)
                {
                    _packetForm.ViewModelBase.TacticalCallsign = IdentityViewModel.Instance.TacticalCallsign;
                }

                if (!string.IsNullOrEmpty(SendFormDataControlViewModel.Instance.MessageTo))
                {
                    if (SendFormDataControlViewModel.Instance.MessageTo.Contains("PKTMON")
                            || SendFormDataControlViewModel.Instance.MessageTo.Contains("PKTTUE"))
                    {
                        MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";

                        _packetForm.SetPracticeField(practiceSubject);
                    }
                }
            }
            else
            {
                FillFormFromPacketMessage();
                LoadMessage = false;
            }
            // Moved here in case state is edit message. The form needs to be filled first otherwise the subject is incomplete
            _packetForm.EventSubjectChanged += FormControl_SubjectChange;
            if (_packetForm.FormHeaderControl != null)
            {
                _packetForm.FormHeaderControl.EventSubjectChanged += FormControl_SubjectChange;
                _packetForm.FormHeaderControl.EventMsgTimeChanged += FormControl_MsgTimeChanged;

                //_packetForm.FormHeaderControl.ViewModelBase.HandlingOrder = "priority";
            }
        }

        public async void FormsPagePivotSelectionChangedAsync(int selectedIndex)
        {
            if (selectedIndex < 0)
                return;

            _pivotItem = FormsPage.FormsPagePivot.Items[selectedIndex] as PivotItem;
            if (_pivotItem is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }

            await ShowPacketFormAsync();
        }

        //private static string ValidateSubject(string subject)
        //{
        //    if (string.IsNullOrEmpty(subject))
        //        return subject;

        //    try
        //    {
        //        return Regex.Replace(subject, @"[^\w\.@-\\%/\-\ ,()]", "~",
        //                             RegexOptions.Singleline, TimeSpan.FromSeconds(1.0));
        //    }
        //    // If we timeout when replacing invalid characters, 
        //    // we should return Empty.
        //    catch (RegexMatchTimeoutException)
        //    {
        //        return string.Empty;
        //    }
        //}

        //private string CreateValidatedSubject()
        //{
        //    return ValidateSubject(_packetForm.CreateSubject());
        //}

        private void CreatePacketMessage(MessageState messageState = MessageState.None)
        {
            _packetMessage = new PacketMessage()
            {
                FormControlType = PacketForm.FormControlType,
                BBSName = SendFormDataControlViewModel.Instance.MessageBBS,
                TNCName = SendFormDataControlViewModel.Instance.MessageTNC,
                FormFieldArray = PacketForm.CreateFormFieldsInXML(),
                FormProvider = PacketForm.FormProvider,
                PacFormName = PacketForm.FormControlName,
                PacFormType = PacketForm.PacFormType,
                MessageFrom = SendFormDataControlViewModel.Instance.MessageFrom,
                MessageTo = SendFormDataControlViewModel.Instance.MessageTo,
                CreateTime = DateTime.Now,
                MessageState = messageState,
                HandlingOrder = HandlingOrder,
                MessageNumber = PacketForm.ViewModelBase.MessageNo,
        };
            //_packetMessage.HandlingOrder = HandlingOrder;

            //_packetMessage.MessageNumber = PacketForm.ViewModelBase.MessageNo;

            UserAddressArray.Instance.AddAddressAsync(_packetMessage.MessageTo);
            //string subject = ValidateSubject(_packetForm.CreateSubject());  // TODO use CreateSubject
            string subject = _packetForm.CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            _packetMessage.Subject = subject ?? SendFormDataControlViewModel.Instance.MessageSubject;
            if (!_packetMessage.CreateFileName())
            {
                throw new Exception();
            }
        }


        private ICommand _ViewOutpostDataCommand;
        public ICommand ViewOutpostDataCommand => _ViewOutpostDataCommand ?? (_ViewOutpostDataCommand = new RelayCommand(ViewOutpostData));

        public void ViewOutpostData()
        {
            AppBarViewOutpostData();
        }

        public async void AppBarViewOutpostData()
        {
            PacketMessage packetMessage = new PacketMessage()
            {
                FormFieldArray = PacketForm.CreateFormFieldsInXML(),
                FormProvider = PacketForm.FormProvider,
                PacFormName = PacketForm.FormControlName,
                PacFormType = PacketForm.PacFormType,
                MessageNumber = PacketForm.ViewModelBase.MessageNo,
                CreateTime = DateTime.Now,
            };
            if (PacketForm.FormHeaderControl == null)
            {
                packetMessage.HandlingOrder = PacketForm.ViewModelBase.HandlingOrder;
            }
            else
            {
                packetMessage.HandlingOrder = PacketForm.FormHeaderControl.ViewModelBase.HandlingOrder;
            }

            DateTime now = DateTime.Now;
            var operatorDateField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
            if (operatorDateField != null)
            {
                operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{(now.Year):d4}";
            }
            var operatorTimeField = packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
            if (operatorTimeField != null)
                operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

            string subject = _packetForm.CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            packetMessage.Subject = (subject ?? SendFormDataControlViewModel.Instance.MessageSubject);
            packetMessage.MessageBody = PacketForm.CreateOutpostData(ref packetMessage);

            string messagebodyForViewing = packetMessage.MessageBody;
            messagebodyForViewing = messagebodyForViewing.TrimEnd('\n');

            // Dispaly "\n" as \\n
            if (packetMessage.PacFormName != "SimpleMessage")
            {                
                string messageBodyStart = messagebodyForViewing.Substring(0, messagebodyForViewing.IndexOf("]\n"));
                messagebodyForViewing = messagebodyForViewing.Substring(messagebodyForViewing.IndexOf("]\n"));
                //messagebodyForViewing = messagebodyForViewing.Replace("]\n", "backslashNL");
                //messagebodyForViewing = messagebodyForViewing.Replace("\n", "\\n");
                //messagebodyForViewing = messagebodyForViewing.Replace("backslashNL", "]\n");
                messagebodyForViewing = messageBodyStart + messagebodyForViewing;
            }
            else
            {

            }

            string messageTextForViewing = $"{packetMessage.Subject}\r\n\r\n{messagebodyForViewing}";
            string messageText = $"{packetMessage.Subject}\r\n\r\n{packetMessage.MessageBody}";
            bool copy = await ContentDialogs.ShowDualButtonMessageDialogAsync(messageTextForViewing, "Copy", "Close", "Outpost Message");
            if (copy)
            {
                DataPackage dataPackage = new DataPackage
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(messageText);
                Clipboard.SetContent(dataPackage);
            }
        }

        //Send message
        private ICommand _SendReceiveCommand;
        public ICommand SendReceiveCommand => _SendReceiveCommand ?? (_SendReceiveCommand = new RelayCommand(SendReceive));

        public async void SendReceive()
        {
            string validationResult = PacketForm.ValidateForm();
            validationResult = PacketAddressForm.ValidateForm(validationResult);
            if (!string.IsNullOrEmpty(validationResult))
            {
                validationResult += "\n\nAdd the missing information and press \"Send\" to continue.";
                await ContentDialogs.ShowSingleButtonContentDialogAsync(validationResult, "Close", "Missing input fields");
                return;
            }

            // append "Drill Traffic" if requested. Moved to message creation.
            //if (PacketSettingsViewModel.Instance.IsDrillTraffic)
            //{
            //    PacketForm.AppendDrillTraffic();
            //}

            CreatePacketMessage();

            DateTime now = DateTime.Now;
            var operatorDateField = _packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorDate").FirstOrDefault();
            if (operatorDateField != null)
            {
                operatorDateField.ControlContent = $"{now.Month:d2}/{now.Day:d2}/{(now.Year):d4}";
            }
            var operatorTimeField = _packetMessage.FormFieldArray.Where(formField => formField.ControlName == "operatorTime").FirstOrDefault();
            if (operatorTimeField != null)
                operatorTimeField.ControlContent = $"{now.Hour:d2}:{now.Minute:d2}";

            //Utilities.MarkMessageNumberAsUsed();
            //_packetMessage.MessageState = MessageState.None;
            _packetMessage.MessageOrigin = MessageOriginHelper.MessageOrigin.Sent;
            if (_packetMessage.PacFormName == "SimpleMessage")
            {
                // Copy messageBody to facilitate printing for long messages. Convert to Rich Text Format
                string message = _packetMessage.FormFieldArray[0].ControlContent.Replace("\\n", "\r\n");
                Paragraph paragraph = new Paragraph();
                Run run = new Run
                {
                    Text = message
                };

                // Add the Run to the Paragraph, the Paragraph to the RichTextBlock.
                paragraph.Inlines.Add(run);
                FormField formField = new FormField()
                {
                    ControlName = "richTextMessageBody",
                    //ControlContent = _packetMessage.FormFieldArray[0].ControlContent,
                    ControlContent = run.Text,
                };
                _packetMessage.FormFieldArray[1] = formField;
            }

            //string subject = CreateSubject();
            //_packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            //_packetMessage.MessageBody = _packetForm.CreateOutpostData(ref _packetMessage);

            _packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

            CommunicationsService communicationsService = new CommunicationsService();
            //Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            await communicationsService.BBSConnectAsync2();

            // Create an empty form
            InitializeFormControlAsync();
        }

        private ICommand _PrintFormCommand;
        public ICommand PrintFormCommand => _PrintFormCommand ?? (_PrintFormCommand = new RelayCommand(PrintForm));

        public void PrintForm()
        {
            _packetForm.PrintForm();
        }

        private ICommand _ClearFormCommand;
        public ICommand ClearFormCommand => _ClearFormCommand ?? (_ClearFormCommand = new RelayCommand(ClearForm));

        public void ClearForm()
        {
            InitializeFormControlAsync();
        }

        private ICommand _SaveFormCommand;
        public ICommand SaveFormCommand => _SaveFormCommand ?? (_SaveFormCommand = new RelayCommand(SaveForm));

        public void SaveForm()
        {
            if (_packetMessage != null)     // Not a new message
            {
                // if the message state was locked it means it was previously sent or received.
                // We must assign a new message number. Also unlock the message
                if (_packetMessage.MessageState == MessageState.Locked)
                {
                    MessageNo = Utilities.GetMessageNumberPacket(true);
                    _packetMessage.MessageNumber = MessageNo;
                    _packetMessage.MessageState = MessageState.Edit;
                }
                _packetMessage.FormFieldArray = _packetForm.CreateFormFieldsInXML();       // Update fields
            }
            else
            {
                CreatePacketMessage(); 
            }

            _packetMessage.Save(SharedData.DraftMessagesFolder.Path);
            Utilities.MarkMessageNumberAsUsed();
        }

    }
}
