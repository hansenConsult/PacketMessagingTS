using System;
using System.Linq;
using System.Reflection;

using FormControlBaseClass;

using MetroLog;

using PacketMessagingTS.Core.Helpers;
using PacketMessagingTS.ViewModels;

using SharedCode;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;
using static SharedCode.Helpers.MessageOriginHelper;

namespace PacketMessagingTS.Helpers
{
    public class Base0FormsPage : Page
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<Base0FormsPage>();
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
                _packetForm.ViewModelBase.ReceivedOrSent = "Receiver";
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
                _packetForm.ViewModelBase.ReceivedOrSent = "Sender";

            }
            else if (_packetMessage.MessageOrigin == MessageOrigin.New)
            {
                _packetForm.MessageSentTime = null;
                _packetForm.MessageReceivedTime = _packetMessage.CreateTime;
                OriginMsgNo = _packetMessage.MessageNumber;
            }
        }

        public static FormControlBase CreateFormControlInstance(string formControlName)
        {
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
                        foreach (CustomAttributeData customAttribute in attrib.CustomAttributes.Where(customAttribute => customAttribute.GetType() == typeof(CustomAttributeData)))
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

                                //var formControlType = namedArguments[0].TypedValue.Value as string;
                                //if (formControlType == controlName)
                                //{
                                //    foundType = classType;
                                //    break;
                                //}
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
