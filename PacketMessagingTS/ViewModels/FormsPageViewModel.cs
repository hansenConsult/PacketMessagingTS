//*********************************************************
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

using MetroLog;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services.CommunicationsService;

using SharedCode;
using SharedCode.Helpers;

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.ViewModels
{
    public class FormsViewModel : BaseViewModel
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<BaseFormsPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        //public List<FormControlAttributes> _formControlAttributeList;
        protected PivotItem _pivotItem;
        protected PacketMessage _packetMessage;
        protected SendFormDataControl _packetAddressForm;
        protected FormControlBase _packetForm;
        protected SimpleMessagePivot _simpleMessagePivot;

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

        public string MsgTime
        {
            get
            {
                if (_packetForm.FormHeaderControl != null)
                    return _packetForm.FormHeaderControl.MsgTime;
                else
                    return _packetForm.MsgTime;
            }
            set
            {
                if (_packetForm.FormHeaderControl != null)
                    _packetForm.FormHeaderControl.MsgTime = value;
                else
                    _packetForm.MsgTime = value;
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

        public override bool IsAppBarSendEnabled
        {
            get => isAppBarSendEnabled;
            set => SetProperty(ref isAppBarSendEnabled, value);
        }
        
        public BaseFormsPage FormsPage
        { get; set; }

        private bool _loadMessage;
        public bool LoadMessage
        {
            get => _loadMessage;
            set => _loadMessage = value;
        }

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

        public static FormControlBase CreateFormControlInstance(string formControlName, MessageState messageState)
        {
            //_logHelper.Log(LogLevel.Info, $"Control Name: {formControlName}");
            FormControlBase formControl = null;
            //IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
            //if (files is null)
            //    return null;

            Type foundType = null;
            //foreach (var file in files.Where(file => file.FileType == ".dll" && file.Name.Contains("FormControl.dll")))
            foreach (Assembly assembly in SharedData.Assemblies)
            {
                try
                {
                    //Assembly assembly = Assembly.Load(new AssemblyName(file.DisplayName));
                    foreach (Type classType in assembly.GetTypes())
                    {
                        var attrib = classType.GetTypeInfo();
                        //foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
                        foreach (CustomAttributeData customAttribute in attrib.CustomAttributes)
                        {
                            var namedArguments = customAttribute.NamedArguments;
                            if (namedArguments.Count == 3)
                            {
                                foreach (CustomAttributeNamedArgument arg in namedArguments)
                                {
                                    if (arg.MemberName == "FormControlName")
                                    {
                                        if (formControlName == arg.TypedValue.Value as string)
                                        {
                                            foundType = classType;
                                            break;
                                        }
                                    }
                                }

                                //    var formControlType = namedArguments[0].TypedValue.Value as string;
                                //    if (formControlType == formControlName)
                                //    {
                                //        foundType = classType;
                                //        break;
                                //    }
                                //}
                            }
                            if (foundType != null)
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logHelper.Log(LogLevel.Info, $"Exception: {ex.Message}");
                }
                if (foundType != null)
                    break;
            }

            if (foundType != null)
            {
                try
                {
                    formControl = (FormControlBase)Activator.CreateInstance(foundType);
                    //object[] parameters = new object[] { messageState };
                    //formControl = (FormControlBase)Activator.CreateInstance(foundType, parameters);
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Info, $"Exception: {e.Message}");
                }
            }
            return formControl;
        }

        private async Task InitializeFormControlAsync()
        {
            _packetMessage = null;
            //_pivotItem = FormsPage.FormsPagePivot.Items[FormsPagePivotSelectedIndex] as PivotItem;
            string formControlName = _pivotItem.Name;

            //string practiceSubject = Singleton<PacketSettingsViewModel>.Instance.DefaultSubject;

            _packetAddressForm = new SendFormDataControl();
            _packetForm = CreateFormControlInstance(formControlName, MessageState.None); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");

                return;
            }

            _packetForm.UpdateFormFieldsRequiredColors();

            MessageNo = Utilities.GetMessageNumberPacket();
            OriginMsgNo = MessageNo;

            DateTime now = DateTime.Now;
            MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
            //_packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
            OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
            OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
            if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
            {
                _packetForm.TacticalCallsign = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
            }

            StackPanel stackPanel = ((ScrollViewer)_pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (formControlName == "SimpleMessage")
            {
                // Insert Pivot for message type
                _simpleMessagePivot = new SimpleMessagePivot();

                stackPanel.Children.Insert(0, _simpleMessagePivot);
                stackPanel.Children.Insert(1, _packetAddressForm);
                stackPanel.Children.Insert(2, _packetForm);

                _simpleMessagePivot.EventSimpleMsgSubjectChanged += SimpleMessage_SubjectChange;
                _simpleMessagePivot.EventMessageChanged += FormControl_MessageChanged;

                _packetAddressForm.MessageSubject = $"{MessageNo}_R_";
                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {
                    _packetAddressForm.MessageSubject += Singleton<PacketSettingsViewModel>.Instance.DefaultSubject;
                }

                (_packetForm as MessageControl).NewHeaderVisibility = true;
                _packetForm.MessageReceivedTime = DateTime.Now;
            }
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetForm.EventSubjectChanged += FormControl_SubjectChange;
                if (_packetForm.FormHeaderControl != null)
                {
                    _packetForm.FormHeaderControl.EventSubjectChanged += FormControl_SubjectChange;
                    _packetForm.FormHeaderControl.EventMsgTimeChanged += FormControl_MsgTimeChanged;
                }

                _packetAddressForm.MessageSubject = $"{MessageNo}";
            }
            IsAppBarSendEnabled = true;
        }

        public void FillFormFromPacketMessage()
        {
            //_packetForm.FormProvider = _packetMessage.FormProvider;
            _packetAddressForm.MessageBBS = _packetMessage.BBSName;
            _packetAddressForm.MessageTNC = _packetMessage.TNCName;
            if (_packetMessage.MessageState == MessageState.Locked)
            {
                _packetForm.LockForm();
                _packetAddressForm.LockForm();
            }
            _packetForm.FillFormFromFormFields(_packetMessage.FormFieldArray);
            _packetAddressForm.MessageFrom = _packetMessage.MessageFrom;
            _packetAddressForm.MessageTo = _packetMessage.MessageTo;
            _packetAddressForm.MessageSubject = _packetMessage.Subject;

            //string opcall = _packetForm.OperatorCallsign;//test
            // Special handling for SimpleMessage
            //_packetForm.MessageNo = _packetMessage.MessageNumber;
            MessageNo = _packetMessage.MessageNumber;
            _packetForm.MessageReceivedTime = _packetMessage.ReceivedTime;
            if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.Received)
            {
                _packetForm.MessageSentTime = _packetMessage.JNOSDate;
                _packetForm.ReceivedOrSent = "Receiver";
                if (_packetForm.FormProvider == Core.Helpers.FormProvidersHelper.FormProviders.PacItForm && _packetForm.PacFormType == "ICS213")
                {
                    MessageNo = _packetMessage.MessageNumber;
                    _packetForm.SenderMsgNo = _packetMessage.SenderMessageNumber;
                }
                else
                {
                    DestinationMsgNo = _packetMessage.MessageNumber;
                    OriginMsgNo = _packetMessage.SenderMessageNumber;
                }
            }
            else if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.Sent)
            {
                _packetForm.MessageSentTime = _packetMessage.SentTime;
                DestinationMsgNo = _packetMessage.ReceiverMessageNumber;
                OriginMsgNo = _packetMessage.MessageNumber;
                _packetForm.ReceivedOrSent = "Sender";
            }
            else if (_packetMessage.MessageOrigin == MessageOriginHelper.MessageOrigin.New)
            {
                _packetForm.MessageSentTime = null;
                _packetForm.MessageReceivedTime = _packetMessage.CreateTime;
                OriginMsgNo = _packetMessage.MessageNumber;
            }
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

        void SimpleMessage_SubjectChange(object sender, FormEventArgs e)
        {
            _packetAddressForm.MessageSubject = $"{MessageNo}_R_{e?.SubjectLine}";
            
            if (_packetMessage != null)
            {
                _packetMessage.Subject = _packetAddressForm.MessageSubject;
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

        public async void FormsPagePivotSelectionChangedAsync(int selectedIndex)
        {
            MessageState messageState = MessageState.None;
            if (!_loadMessage)
            {
                _packetMessage = null;
                
            }
            else
            {
                _packetMessage = FormsPage.PacketMessage;
                messageState = FormsPage.PacketMessage.MessageState;
            }

            _packetAddressForm = new SendFormDataControl();
            _packetAddressForm.FormPacketMessage = _packetMessage;

            string practiceSubject = Singleton<PacketSettingsViewModel>.Instance.DefaultSubject;

            _pivotItem = FormsPage.FormsPagePivot.Items[selectedIndex] as PivotItem;
            string pivotItemName = _pivotItem.Name;
            _packetForm = CreateFormControlInstance(pivotItemName, messageState); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }

            _packetForm.FormPacketMessage = _packetMessage;
            _packetForm.FormatTextBoxes();
            MessageNo = Utilities.GetMessageNumberPacket();
            OriginMsgNo = MessageNo;

            StackPanel stackPanel = ((ScrollViewer)_pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                if (!_loadMessage)
                {
                    // Insert Pivot for message type
                    _simpleMessagePivot = new SimpleMessagePivot();

                    stackPanel.Children.Insert(0, _simpleMessagePivot);
                    stackPanel.Children.Insert(1, _packetAddressForm);
                    stackPanel.Children.Insert(2, _packetForm);

                    (_packetForm as MessageControl).NewHeaderVisibility = true;

                    _simpleMessagePivot.EventSimpleMsgSubjectChanged += SimpleMessage_SubjectChange;
                    _simpleMessagePivot.EventMessageChanged += FormControl_MessageChanged;
                }
                else
                {
                    // Show existing message
                    stackPanel.Children.Insert(0, _packetAddressForm);
                    stackPanel.Children.Insert(1, _packetForm);

                    switch (_packetMessage.MessageOrigin)
                    {
                        case MessageOriginHelper.MessageOrigin.Received:
                            (_packetForm as MessageControl).InBoxHeaderVisibility = true;
                            break;
                        case MessageOriginHelper.MessageOrigin.Sent:
                            (_packetForm as MessageControl).SentHeaderVisibility = true;
                            break;
                        default:
                            (_packetForm as MessageControl).NewHeaderVisibility = true;
                            break;
                    }
                }

                // Moved to SimpleMessagePivot control
                //_packetAddressForm.MessageSubject = $"{MessageNo}_R_";
                //if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                //{
                //    _packetAddressForm.MessageSubject += practiceSubject;
                //    //_packetForm.MessageBody = Singleton<PacketSettingsViewModel>.Instance.DefaultMessage;
                //}
                _packetForm.MessageReceivedTime = DateTime.Now;
            }
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            }

            if (!_loadMessage)
            {
                // Moved to end of function to allow for state Edit without updated subject 
                //_packetForm.EventSubjectChanged += FormControl_SubjectChange;
                //if (_packetForm.FormHeaderControl != null)
                //{
                //    _packetForm.FormHeaderControl.EventSubjectChanged += FormControl_SubjectChange;
                //    _packetForm.FormHeaderControl.EventMsgTimeChanged += FormControl_MsgTimeChanged;
                //}

                DateTime now = DateTime.Now;
                MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                //_packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
                OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
                OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
                if (Singleton<IdentityViewModel>.Instance.UseTacticalCallsign)
                {
                    _packetForm.TacticalCallsign = Singleton<IdentityViewModel>.Instance.TacticalCallsign;
                }

                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {
                    HandlingOrder = "routine";
                    MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
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
                IsAppBarSendEnabled = true;
            }
            else
            {
                FillFormFromPacketMessage();
                IsAppBarSendEnabled = !(_packetMessage.MessageState == MessageState.Locked);

                _loadMessage = false;
            }
            // Moved here in case state is edit message. The form needs to be filled first otherwise the subject is incomplete
            _packetForm.EventSubjectChanged += FormControl_SubjectChange;
            if (_packetForm.FormHeaderControl != null)
            {
                _packetForm.FormHeaderControl.EventSubjectChanged += FormControl_SubjectChange;
                _packetForm.FormHeaderControl.EventMsgTimeChanged += FormControl_MsgTimeChanged;
            }
        }

        private static string ValidateSubject(string subject)
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

        //private string CreateValidatedSubject()
        //{
        //    return ValidateSubject(_packetForm.CreateSubject());
        //}

        //private void CreatePacketMessage(MessageState messageState = MessageState.Locked, FormProvidersHelper.FormProviders formProvider = FormProvidersHelper.FormProviders.PacItForm)
        private void CreatePacketMessage(MessageState messageState = MessageState.None)
        {
            _packetMessage = new PacketMessage()
            {
                FormControlType = PacketForm.FormControlType,
                BBSName = PacketAddressForm.MessageBBS,
                TNCName = PacketAddressForm.MessageTNC,
                FormFieldArray = PacketForm.CreateFormFieldsInXML(),
                FormProvider = PacketForm.FormProvider,
                PacFormName = PacketForm.GetPacFormName(),
                PacFormType = PacketForm.PacFormType,
                MessageFrom = PacketAddressForm.MessageFrom,
                MessageTo = PacketAddressForm.MessageTo,
                CreateTime = DateTime.Now,
                MessageState = messageState,
            };

            _packetMessage.MessageNumber = PacketForm.MessageNo;

            UserAddressArray.Instance.AddAddressAsync(_packetMessage.MessageTo);
            //string subject = ValidateSubject(_packetForm.CreateSubject());  // TODO use CreateSubject
            string subject = _packetForm.CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            _packetMessage.Subject = subject ?? PacketAddressForm.MessageSubject;
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
                PacFormName = PacketForm.GetPacFormName(),
                PacFormType = PacketForm.PacFormType,
                MessageNumber = PacketForm.MessageNo,
                CreateTime = DateTime.Now,
            };
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
            packetMessage.Subject = (subject ?? PacketAddressForm.MessageSubject);
            packetMessage.MessageBody = PacketForm.CreateOutpostData(ref packetMessage);

            string messageText = $"{packetMessage.Subject}\r\n\r\n{packetMessage.MessageBody}";
            bool copy = await ContentDialogs.ShowDualButtonMessageDialogAsync(messageText, "Copy", "Close", "Outpost Message");
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

            // append "Drill Traffic" if requested
            if (Singleton<PacketSettingsViewModel>.Instance.IsDrillTraffic)
            {
                PacketForm.AppendDrillTraffic();
            }

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
                // Copy messageBody to facilitate printing for long messages
                FormField formField = new FormField()
                {
                    ControlName = "richTextMessageBody",
                    ControlContent = _packetMessage.FormFieldArray[0].ControlContent,
                };
                _packetMessage.FormFieldArray[1] = formField;
            }

            //string subject = CreateSubject();
            //_packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            //_packetMessage.MessageBody = _packetForm.CreateOutpostData(ref _packetMessage);

            _packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

            CommunicationsService communicationsService = new CommunicationsService();
            //Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            communicationsService.BBSConnectAsync2();

            // Create an empty form
            await InitializeFormControlAsync();
        }

        private ICommand _PrintFormCommand;
        public ICommand PrintFormCommand => _PrintFormCommand ?? (_PrintFormCommand = new RelayCommand(PrintForm));

        public void PrintForm()
        {
            _packetForm.PrintForm();
        }

        private ICommand _ClearFormCommand;
        public ICommand ClearFormCommand => _ClearFormCommand ?? (_ClearFormCommand = new RelayCommand(ClearForm));

        public async void ClearForm()
        {
            await InitializeFormControlAsync();
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
                    MessageNo = Utilities.GetMessageNumberPacket();
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
