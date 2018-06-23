﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;
using ToggleButtonGroupControl;
using FormControlBaseClass;
using MetroLog;
using SharedCode;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
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

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FormsPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<FormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);


        PacketMessage _packetMessage;
        bool _loadMessage = false;

        FormControlBase _packetForm;
        SendFormDataControl _packetAddressForm;

        private List<FormControlAttributes> _formControlAttributeList;


        public FormsPage()
        {
            this.InitializeComponent();

            _formControlAttributeList = new List<FormControlAttributes>();
            ScanFormAttributes();

            foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
            {
                PivotItem pivotItem = CreatePivotItem(formControlAttribute);
                MyPivot.Items.Add(pivotItem);
            }
        }

        private PivotItem CreatePivotItem(FormControlAttributes formControlAttributes)
        {
            PivotItem pivotItem = new PivotItem();
            pivotItem.Name = formControlAttributes.FormControlName;
            pivotItem.Header = formControlAttributes.FormControlMenuName;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Margin = new Thickness(0, 12, -12, 0);
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.Height = double.NaN;

            StackPanel stackpanel = new StackPanel();
            stackpanel.Name = pivotItem.Name + "Panel";
            scrollViewer.Content = stackpanel;

            pivotItem.Content = scrollViewer;

            return pivotItem;
        }

        private void ScanFormAttributes()
        {
            IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
            if (files == null)
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
            List<FormControlAttributes> attributeListTypeNone = new List<FormControlAttributes>();
            List<FormControlAttributes> attributeListTypeCounty = new List<FormControlAttributes>();
            List<FormControlAttributes> attributeListTypeCity = new List<FormControlAttributes>();
            List<FormControlAttributes> attributeListTypeHospital = new List<FormControlAttributes>();
            // Sort by menu type
            foreach (FormControlAttributes formControlAttributes in _formControlAttributeList)
            {
                if (formControlAttributes.FormControlType == FormControlAttribute.FormType.None)
                {
                    attributeListTypeNone.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CountyForm)
                {
                    attributeListTypeCounty.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.CityForm)
                {
                    attributeListTypeCity.Add(formControlAttributes);
                }
                else if (formControlAttributes.FormControlType == FormControlAttribute.FormType.HospitalForm)
                {
                    attributeListTypeHospital.Add(formControlAttributes);
                }
            }
            _formControlAttributeList.Clear();
            _formControlAttributeList.AddRange(attributeListTypeNone);
            _formControlAttributeList.AddRange(attributeListTypeCounty);
            _formControlAttributeList.AddRange(attributeListTypeCity);
            _formControlAttributeList.AddRange(attributeListTypeHospital);
        }

        private void CreatePacketMessage()
        {
            _packetMessage = new PacketMessage()
            {
                BBSName = _packetAddressForm.MessageBBS,
                TNCName = _packetAddressForm.MessageTNC,
                FormFieldArray = _packetForm.CreateFormFieldsInXML(),
                PacFormName = _packetForm.PacFormName,
                PacFormType = _packetForm.PacFormType,
                MessageFrom = _packetAddressForm.MessageFrom,
                MessageTo = _packetAddressForm.MessageTo,
                MessageNumber = _packetForm.MessageNo
            };
            AddressBook.Instance.AddAddressAsync(_packetMessage.MessageTo);
            string subject = _packetForm.CreateSubject();
            // subject is "null" for Simple Message, otherwise use the form generated subject line
            _packetMessage.Subject = (subject ?? _packetAddressForm.MessageSubject);
            _packetMessage.CreateFileName();
        }

        public void FillFormFromPacketMessage()
        {
            _packetAddressForm.MessageBBS = _packetMessage.BBSName;
            _packetAddressForm.MessageTNC = _packetMessage.TNCName;
            _packetForm.FillFormFromFormFields(_packetMessage.FormFieldArray);
            _packetAddressForm.MessageFrom = _packetMessage.MessageFrom;
            _packetAddressForm.MessageTo = _packetMessage.MessageTo;
            _packetAddressForm.MessageSubject = _packetMessage.Subject;


            //string opcall = _packetForm.OperatorCallsign;//test

            // Below is a workaround for missing event when a field changes
            foreach (FormField formField in _packetMessage.FormFieldArray)
            {
                FormControl formControl = _packetForm.FormControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                if (formControl == null)
                    continue;

                Control control = formControl?.InputControl;
                switch (control.Name)
                {
                    case "severity":
                        _packetForm.Severity = (control as ToggleButtonGroup).CheckedControlName;
                        break;
                    case "handlingOrder":
                        _packetForm.HandlingOrder = (control as ToggleButtonGroup).CheckedControlName;
                        break;
                    case "msgDate":
                        _packetForm.MsgDate = (control as TextBox).Text;
                        break;
                    case "msgTime":
                        _packetForm.MsgTime = (control as TextBox).Text;
                        break;
                    case "operatorCallsign":
                        _packetForm.OperatorCallsign = (control as TextBox).Text;
                        break;
                    case "operatorName":
                        _packetForm.OperatorName = (control as TextBox).Text;
                        break;
                    case "operatorDate":
                        _packetForm.OperatorDate = (control as TextBox).Text;
                        break;
                    case "operatorTime":
                        _packetForm.OperatorTime = (control as TextBox).Text;
                        break;
                    case null:
                        continue;
                }
            }
        }

        // The form control is created based on the control name,
        // since there may be several versions of the same type
        public static FormControlBase CreateFormControlInstance(string controlName)
        {
            FormControlBase formControl = null;
            IReadOnlyList<StorageFile> files = SharedData.FilesInInstalledLocation;
            if (files == null)
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
            if (e.Parameter == null)
                return;

            int index = 0;
            string packetMessagePath = e.Parameter as string;
            _packetMessage = PacketMessage.Open(packetMessagePath);
            _packetMessage.MessageOpened = true;
            string directory = Path.GetDirectoryName(packetMessagePath);
            _packetMessage.Save(directory);
            _loadMessage = true;
            foreach (PivotItem pivotItem in MyPivot.Items)
            {
                if (pivotItem.Name == _packetMessage.PacFormName) // If PacFormType is not set
                {
                    MyPivot.SelectedIndex = index;
                    break;
                }
                index++;
            }
        }

        private async Task InitializeFormControlAsync()
        {
            PivotItem pivotItem = MyPivot.SelectedItem as PivotItem;
            string pivotItemName = pivotItem.Name;

            _packetAddressForm = new SendFormDataControl();
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm == null)
            {
                MessageDialog messageDialog = new MessageDialog(content: "Failed to find packet form.", title: "Packet Messaging Error");
                await messageDialog.ShowAsync();
                return;
            }

            _packetMessage = new PacketMessage();
            _packetForm.MessageNo = Utilities.GetMessageNumberPacket();

            DateTime now = DateTime.Now;
            _packetForm.MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year - 2000:d2}";
            _packetForm.MsgTime = $"{now.Hour:d2}{now.Minute:d2}";
            _packetForm.OperatorDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year - 2000:d2}";
            _packetForm.OperatorTime = $"{now.Hour:d2}{now.Minute:d2}";
            _packetForm.OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
            _packetForm.OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;

            StackPanel stackPanel = ((ScrollViewer)pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                stackPanel.Children.Insert(0, _packetAddressForm);
                stackPanel.Children.Insert(1, _packetForm);

                _packetAddressForm.MessageSubject = $"{_packetForm.MessageNo}_O/R_<subject>";
            }
            //else if (pivotItemName == "Message")
            //{
            //    Form213Panel.Children.Clear();
            //    Form213Panel.Children.Insert(0, _packetForm);
            //    Form213Panel.Children.Insert(1, _packetAddressForm);
            //    _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            //}
            //else if (pivotItemName != "Message")
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            }
        }

        void FormControl_SubjectChange(object sender, FormEventArgs e)
        {
            if (e?.SubjectLine?.Length > 0)
            {
                if (_packetMessage != null)
                {
                    _packetMessage.Subject = _packetForm.CreateSubject();
                    _packetAddressForm.MessageSubject = _packetMessage.Subject;
                }
            }
        }

        // TODO insert InitializeFormControlAsync
        private async void MyPivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _packetAddressForm = new SendFormDataControl();
            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            string pivotItemName = pivotItem.Name;
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm == null)
            {
                MessageDialog messageDialog = new MessageDialog(content: "Failed to find packet form.", title: "Packet Messaging Error");
                await messageDialog.ShowAsync();
                return;
            }

            _packetForm.MessageNo = Utilities.GetMessageNumberPacket();
            //if (!_loadMessage)
            //{
            //    _packetMessage = new PacketMessage();
            //}
            //_packetForm.MessageNo = Utilities.GetMessageNumberPacket();

            StackPanel stackPanel = ((ScrollViewer)pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            DateTime now = DateTime.Now;
            string messageData = $"{now.Month:d2}/{now.Day:d2}/{now.Year - 2000:d2}";

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                stackPanel.Children.Insert(0, _packetAddressForm);
                stackPanel.Children.Insert(1, _packetForm);

                _packetAddressForm.MessageSubject = $"{_packetForm.MessageNo}_O/R_<subject>";
            }
            //else if (pivotItemName == "Message")
            //{
            //    Form213Panel.Children.Clear();
            //    Form213Panel.Children.Insert(0, _packetForm);
            //    Form213Panel.Children.Insert(1, _packetAddressForm);
            //    _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            //}
            //else if (pivotItemName != "Message")
            else
            {
                stackPanel.Children.Insert(0, _packetForm);
                stackPanel.Children.Insert(1, _packetAddressForm);

                _packetAddressForm.MessageSubject = _packetForm.CreateSubject();
            }

            if (!_loadMessage)
            {
                _packetMessage = new PacketMessage();

                _packetForm.EventSubjectChanged += FormControl_SubjectChange;

                _packetForm.MsgDate = messageData;
                _packetForm.MsgTime = $"{now.Hour:d2}{now.Minute:d2}";
                _packetForm.OperatorDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year - 2000:d2}";
                _packetForm.OperatorTime = $"{now.Hour:d2}{now.Minute:d2}";
                _packetForm.OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
                _packetForm.OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;
            }
            else 
            {
                FillFormFromPacketMessage();
                _loadMessage = false;
            }
        }

        private async void AppBarViewOutpostData_ClickAsync(object sender, RoutedEventArgs e)
        {
            CreatePacketMessage();

            if (string.IsNullOrEmpty(_packetMessage.MessageBody))
            {
                _packetMessage.MessageBody = _packetForm.CreateOutpostData(ref _packetMessage);
            }

            ContentDialog outpostDataDialog = new ContentDialog()
            {
                Title = "Outpost Message",
                Content = _packetMessage.MessageBody,
                CloseButtonText = "Cancel",
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Copy",
            };
            ContentDialogResult result = await outpostDataDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(_packetMessage.MessageBody);
                Clipboard.SetContent(dataPackage);
            }
        }

        private async void AppBarSave_ClickAsync(object sender, RoutedEventArgs e)
        {
            CreatePacketMessage();
            DateTime dateTime = DateTime.Now;
            _packetMessage.CreateTime = DateTime.Now;

            _packetMessage.Save(SharedData.DraftMessagesFolder.Path);
            Utilities.MarkMessageNumberAsUsed();

            // Initialize to an empty form
            await InitializeFormControlAsync();
        }

        private async void AppBarSend_ClickAsync(object sender, RoutedEventArgs e)
        {
            string validationResult = _packetForm.ValidateForm();
            validationResult = _packetAddressForm.ValidateForm(validationResult);
            if (!string.IsNullOrEmpty(validationResult))
            {
                //validationResult = "Please fill out the areas in red." + validationResult;
                validationResult += "\n\nAdd the missing information and press \"Send\" to continue.";
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "Missing input fields",
                    Content = validationResult,
                    CloseButtonText = "Close"
                };
                ContentDialogResult result = await contentDialog.ShowAsync();
                return;
            }

            CreatePacketMessage();
            _packetMessage.CreateTime = DateTime.Now;
            Utilities.MarkMessageNumberAsUsed();

            _packetMessage.Save(SharedData.UnsentMessagesFolder.Path);

            Services.CommunicationsService.CommunicationsService communicationsService = Services.CommunicationsService.CommunicationsService.CreateInstance();
            communicationsService.BBSConnectAsync2();

            // Create an empty form
            await InitializeFormControlAsync();
        }

        private async void AppBarPrint_ClickAsync(object sender, RoutedEventArgs e)
        {

        }

    }
}
