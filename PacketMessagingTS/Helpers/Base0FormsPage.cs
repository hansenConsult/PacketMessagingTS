﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FormControlBaseClass;
using MetroLog;
using Microsoft.Toolkit.Helpers;
using PacketMessagingTS.ViewModels;
using SharedCode;
using SharedCode.Helpers;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using static SharedCode.Helpers.MessageOriginHelper;

namespace PacketMessagingTS.Helpers
{
    public class Base0FormsPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<Base0FormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        protected PacketMessage _packetMessage;
        protected FormControlBase _packetForm;

        //public class FormControlAttributes
        //{
        //    public string FormControlName
        //    { get; private set; }

        //    public string FormControlMenuName
        //    { get; private set; }

        //    public FormControlAttribute.FormType FormControlType
        //    { get; private set; }

        //    public StorageFile FormControlFileName
        //    { get; set; }

        //    public FormControlAttributes(string formControlType, string formControlMenuName, FormControlAttribute.FormType formType, StorageFile formControlFileName)
        //    {
        //        FormControlName = formControlType;
        //        FormControlMenuName = formControlMenuName;
        //        FormControlType = formType;
        //        FormControlFileName = formControlFileName;
        //    }
        //}

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        public void FillFormFromPacketMessage()
        {
            _packetForm.FillFormFromFormFields(_packetMessage.FormFieldArray);

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
                    DestinationMsgNo = _packetMessage.MessageNumber;
                    OriginMsgNo = _packetMessage.SenderMessageNumber;
                }
            }
            else if (_packetMessage.MessageOrigin == MessageOrigin.Sent)
            {
                _packetForm.MessageSentTime = _packetMessage.SentTime;
                DestinationMsgNo = _packetMessage.ReceiverMessageNumber;
                OriginMsgNo = _packetMessage.MessageNumber;
                _packetForm.ReceivedOrSent = "Sender";

            }
            else if (_packetMessage.MessageOrigin == MessageOrigin.New)
            {
                _packetForm.MessageSentTime = null;
                _packetForm.MessageReceivedTime = _packetMessage.CreateTime;
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
                    _logHelper.Log(LogLevel.Error, $"Error in CreateFormControlInstance{ex.Message}");
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

    }
}
