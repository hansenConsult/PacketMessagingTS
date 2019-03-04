﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using FormControlBaseClass;

using MessageFormControl;

using MetroLog;

using PacketMessagingTS.Controls;
using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.ViewModels;

using SharedCode;

using ToggleButtonGroupControl;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HospitalFormsPage : Page
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<HospitalFormsPage>();
        private static LogHelper _logHelper = new LogHelper(log);

        PacketMessage _packetMessage;
        bool _loadMessage = false;

        FormControlBase _packetForm;
        SendFormDataControl _packetAddressForm;
        //SimpleMessageHeader _headerControl;


        private PrintHelper printHelper;

        public FormControlBase PacketForm
        {
            get => _packetForm;
        }

        private List<FormControlAttributes> _formControlAttributeList;

        public HospitalFormsViewModel _hospitalFormsViewModel { get; } = Singleton<HospitalFormsViewModel>.Instance;

        public HospitalFormsPage()
        {
            InitializeComponent();

            _formControlAttributeList = new List<FormControlAttributes>();
            ScanFormAttributes();

            foreach (FormControlAttributes formControlAttribute in _formControlAttributeList)
            {
                if (string.IsNullOrEmpty(formControlAttribute.FormControlMenuName))
                {
                    continue;
                }
                PivotItem pivotItem = CreatePivotItem(formControlAttribute);
                hospitalFormsPagePivot.Items.Add(pivotItem);
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
            //_formControlAttributeList.AddRange(attributeListTypeNone);
            //_formControlAttributeList.AddRange(attributeListTypeCounty);
            //_formControlAttributeList.AddRange(attributeListTypeCity);
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
            // Special handling for SimpleMessage
            _packetForm.MessageNo = _packetMessage.MessageNumber;
            _packetForm.MessageReceivedTime = _packetMessage.ReceivedTime;
            // Below is a workaround for missing event when a field changes
            foreach (FormField formField in _packetMessage.FormFieldArray)
            {
                FormControl formControl = _packetForm.FormControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                if (formControl is null)
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
            // Initialize common helper class and register for printing
            printHelper = new PrintHelper(this);
            printHelper.RegisterForPrinting();

            if (e.Parameter is null)
            {
                // Open an empty form
                hospitalFormsPagePivot.SelectedIndex = _hospitalFormsViewModel.HospitalFormsPagePivotSelectedIndex;
                return;
            }

            // Open a form with content
            int index = 0;
            string packetMessagePath = e.Parameter as string;
            _packetMessage = PacketMessage.Open(packetMessagePath);
            _packetMessage.MessageOpened = true;
            string directory = Path.GetDirectoryName(packetMessagePath);
            _loadMessage = true;
            foreach (PivotItem pivotItem in hospitalFormsPagePivot.Items)
            {
                if (pivotItem.Name == _packetMessage.PacFormName) // If PacFormType is not set
                {
                    hospitalFormsPagePivot.SelectedIndex = index;
                    break;
                }
                index++;
            }
            // Show SimpleMessage header formatted by where the message came from
            _packetMessage.Save(directory);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (printHelper != null)
            {
                printHelper.UnregisterForPrinting();
            }
            _hospitalFormsViewModel.HospitalFormsPagePivotSelectedIndex = hospitalFormsPagePivot.SelectedIndex;

            base.OnNavigatedFrom(e);
        }

        private async Task InitializeFormControlAsync()
        {
            PivotItem pivotItem = hospitalFormsPagePivot.SelectedItem as PivotItem;
            string pivotItemName = pivotItem.Name;

            _packetAddressForm = new SendFormDataControl();
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                MessageDialog messageDialog = new MessageDialog(content: "Failed to find packet form.", title: "Packet Messaging Error");
                await messageDialog.ShowAsync();
                return;
            }

            _packetMessage = new PacketMessage();
            _packetForm.MessageNo = Utilities.GetMessageNumberPacket();

            DateTime now = DateTime.Now;
            _packetForm.MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
            _packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
            _packetForm.OperatorDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
            _packetForm.OperatorTime = $"{now.Hour:d2}:{now.Minute:d2}";
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

        // TODO insert InitializeFormControlAsync, maybe
        private async void HospitalFormsPagePivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _packetAddressForm = new SendFormDataControl(_loadMessage);

            PivotItem pivotItem = (PivotItem)((Pivot)sender).SelectedItem;
            string pivotItemName = pivotItem.Name;
            _packetForm = CreateFormControlInstance(pivotItemName); // Should be PacketFormName, since there may be multiple files with same name
            if (_packetForm is null)
            {
                //MessageDialog messageDialog = new MessageDialog(content: "Failed to find packet form.", title: "Packet Messaging Error");
                //await messageDialog.ShowAsync();
                await Utilities.ShowSingleButtonContentDialogAsync("Failed to find packet form.", "Close", "Packet Messaging Error");
                return;
            }

            _packetForm.MessageNo = Utilities.GetMessageNumberPacket();

            StackPanel stackPanel = ((ScrollViewer)pivotItem.Content).Content as StackPanel;
            stackPanel.Margin = new Thickness(0, 0, 12, 0);

            string practiceSubject = Singleton<PacketSettingsViewModel>.Instance.DefaultSubject;

            stackPanel.Children.Clear();
            if (pivotItemName == "SimpleMessage")
            {
                stackPanel.Children.Insert(0, _packetAddressForm);
                stackPanel.Children.Insert(1, _packetForm);

                _packetAddressForm.MessageSubject = $"{_packetForm.MessageNo}_O/R_";
                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {
                    _packetAddressForm.MessageSubject += practiceSubject;
                }
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
                _packetMessage = new PacketMessage();

                _packetForm.EventSubjectChanged += FormControl_SubjectChange;

                DateTime now = DateTime.Now;
                _packetForm.MsgDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                _packetForm.MsgTime = $"{now.Hour:d2}:{now.Minute:d2}";
                _packetForm.OperatorDate = $"{now.Month:d2}/{now.Day:d2}/{now.Year:d4}";
                _packetForm.OperatorTime = $"{now.Hour:d2}:{now.Minute:d2}";
                _packetForm.OperatorName = Singleton<IdentityViewModel>.Instance.UserName;
                _packetForm.OperatorCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign;

                _packetMessage.FormFieldArray = _packetForm.CreateFormFieldsInXML();
                if (_packetAddressForm.MessageTo.Contains("PKTMON") || _packetAddressForm.MessageTo.Contains("PKTTUE"))
                {
                    _packetForm.Severity = "other";
                    _packetForm.HandlingOrder = "routine";

                    foreach (FormField formField in _packetMessage.FormFieldArray)
                    {
                        FormControl formControl = _packetForm.FormControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                        if (formControl is null)
                            continue;

                        Control control = formControl?.InputControl;
                        bool controlFound = false;
                        switch (control.Name)
                        {
                            case "subject":
                                (control as TextBox).Text = practiceSubject;
                                controlFound = true;
                                break;
                            case "incidentName":
                                (control as TextBox).Text = practiceSubject;
                                controlFound = true;
                                break;
                        }
                        if (controlFound)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                FillFormFromPacketMessage();
                _loadMessage = false;
            }

            //_hospitalFormsViewModel.FormsPagePivotSelectedIndex = FormsPagePivot.SelectedIndex;

            // Initialize print content for this scenario
            //if (_packetForm.GetType() == typeof(ICS213Control))
            {
                //ContinuationPage continuationPage = new ContinuationPage(this);
                printHelper?.PreparePrintContent(this);
            }
            _hospitalFormsViewModel.HospitalFormsPagePivotSelectedIndex = ((Pivot)sender).SelectedIndex;
        }

        private async void AppBarViewOutpostData_ClickAsync(object sender, RoutedEventArgs e)
        {
            CreatePacketMessage();

            if (string.IsNullOrEmpty(_packetMessage.MessageBody))
            {
                _packetMessage.MessageBody = _packetForm.CreateOutpostData(ref _packetMessage);
            }

            TextBlock messageBody = new TextBlock()
            {
                Text = _packetMessage.MessageBody,
            };
            ScrollViewer.SetVerticalScrollBarVisibility(messageBody, ScrollBarVisibility.Visible); // Does not work

            //ContentDialog outpostDataDialog = new ContentDialog()
            //{
            //    Title = "Outpost Message",
            //    Content = messageBody,
            //    CloseButtonText = "Close",
            //    IsPrimaryButtonEnabled = true,
            //    PrimaryButtonText = "Copy",
            //};
            //ContentDialogResult result = await outpostDataDialog.ShowAsync();
            bool result = await Utilities.ShowDualButtonMessageDialogAsync(_packetMessage.MessageBody, "Copy", "Close", "Outpost Message");
            //if (result == ContentDialogResult.Primary)      // Copy also copies the invisible part
            if (result)      // Copy also copies the invisible part
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
                    //validationResult += "\n\nAdd the missing information and press \"Send\" to continue.";
                    //ContentDialog contentDialog = new ContentDialog
                    //{
                    //    Title = "Missing input fields",
                    //    Content = validationResult,
                    //    CloseButtonText = "Close"
                    //};
                    //ContentDialogResult result = await contentDialog.ShowAsync();
                await Utilities.ShowSingleButtonContentDialogAsync(validationResult, "Close", "Missing input fields");
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
                await printHelper.ShowPrintUIAsync();
            }

    }
}