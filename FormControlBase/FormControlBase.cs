﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using FormControlBasicsNamespace;

using FormUserControl;

using Microsoft.UI.Xaml.Controls;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Helpers.PrintHelpers;
using SharedCode.Models;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;
using Windows.Storage;
using System.Linq;
using PacketMessagingTS.Core.Helpers;
using MetroLog;

namespace FormControlBaseClass
{

    public abstract partial class FormControlBase : FormControlBasics
    {
        protected static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<FormControlBase>();
        private static readonly LogHelper _logHelper = new LogHelper(log);

        protected List<string> _outpostData;

        protected ScrollViewer _scrollViewer;

        protected readonly List<ComboBoxItem> Municipalities = new List<ComboBoxItem>
        {
                new ComboBoxItem() {Content = "Campbell"},
                new ComboBoxItem() {Content = "Cupertino"},
                new ComboBoxItem() {Content = "Gilroy"},
                new ComboBoxItem() {Content = "Los Altos"},
                new ComboBoxItem() {Content = "Los Altos Hills"},
                new ComboBoxItem() {Content = "Los Gatos"},
                new ComboBoxItem() {Content = "Milpitas"},
                new ComboBoxItem() {Content = "Monte Sereno"},
                new ComboBoxItem() {Content = "Morgan Hill"},
                new ComboBoxItem() {Content = "Mountain View"},
                new ComboBoxItem() {Content = "Palo Alto"},
                new ComboBoxItem() {Content = "San Jose"},
                new ComboBoxItem() {Content = "Santa Clara"},
                new ComboBoxItem() {Content = "Saratoga"},
                new ComboBoxItem() {Content = "Sunnyvale"},
        };

        protected readonly List<ComboBoxItem> Hospitals = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = "El Camino Hospital Los Gatos" },
            new ComboBoxItem() { Content = "El Camino Hospital Mountain View" },
            new ComboBoxItem() { Content = "Good Samaritan Hospital" },
            new ComboBoxItem() { Content = "Kaiser Gan Jose Medical Center" },
            new ComboBoxItem() { Content = "Kaiser Santa Clara Hospital" },
            new ComboBoxItem() { Content = "Lucile Packard Children's Hospital" },
            new ComboBoxItem() { Content = "O'Connor Hospital" },
            new ComboBoxItem() { Content = "Palo Alto Veterans Hospital" },
            new ComboBoxItem() { Content = "Regional San Jose Medical Center" },
            new ComboBoxItem() { Content = "Saint Loise Regional Hospital" },
            new ComboBoxItem() { Content = "Stanford Hospital" },
            new ComboBoxItem() { Content = "Stanford School of Medicine" },
            new ComboBoxItem() { Content = "Valley Medical Center" },
        };

        protected List<ComboBoxItem> Priority = new List<ComboBoxItem>
        {
            new ComboBoxItem() { Content = "Now", Tag = "Now"},
            new ComboBoxItem() { Content = "High (0-4 hrs.)", Tag = "High" },
            new ComboBoxItem() { Content = "Medium (5-12 hrs.)", Tag = "Medium" },
            new ComboBoxItem() { Content = "Low (12+ hrs.)", Tag = "Low" },
        };

        protected readonly List<ComboBoxItem> ToICSPositionItems = new List<ComboBoxItem>
        {
            new ComboBoxItem() {Content = "EMS Unit"},
            new ComboBoxItem() {Content = "Medical Health Branch"},
            new ComboBoxItem() {Content = "Operations Section"},
        };

        //readonly protected List<ComboBoxItem> ToICSLocationItems = new List<ComboBoxItem>
        //{
        //    new ComboBoxItem() {Content = "PHDOC"},
        //    new ComboBoxItem() {Content = "County EOC"},
        //};


        public static string DrillTraffic = "\n**** This is drill traffic ****";

        protected PrintHelper _printHelper;
        protected List<Panel> _printPanels;


        public virtual void UpdateFormFieldsRequiredColors()
        {
            if (FormPacketMessage != null && FormPacketMessage.MessageState == MessageState.Locked)
            {
                return;
            }

            bool isReportTypeSelected = true;

            RadioButtons reportTypeRadioButtons = FindName("reportType") as RadioButtons;
            RadioButton radioBtn = reportTypeRadioButtons?.SelectedItem as RadioButton;
            string checkedReportType = radioBtn?.Name;
            if (reportTypeRadioButtons != null && string.IsNullOrEmpty(checkedReportType))
            {
                isReportTypeSelected = false;
            }

            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;

                if (control is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text) && IsFieldRequired(textBox) && isReportTypeSelected)
                    {
                        textBox.BorderBrush = formControl.RequiredBorderBrush;
                        textBox.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        textBox.BorderBrush = formControl.BaseBorderColor;
                        textBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    if (string.IsNullOrEmpty(autoSuggestBox.Text) && IsFieldRequired(control) && isReportTypeSelected)
                    {
                        autoSuggestBox.BorderBrush = formControl.RequiredBorderBrush;
                        autoSuggestBox.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        autoSuggestBox.BorderBrush = formControl.BaseBorderColor;
                        autoSuggestBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (((comboBox.SelectedIndex < 0 && IsFieldRequired(control) && !comboBox.IsEditable)
                        || (comboBox.SelectedIndex < 0 && IsFieldRequired(control) && comboBox.IsEditable && string.IsNullOrEmpty(comboBox.Text)))
                        && isReportTypeSelected)
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                        comboBox.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        comboBox.BorderBrush = formControl.BaseBorderColor;
                        comboBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is RadioButtons radioButtons)
                {
                    if (radioButtons.Name != "reportType" && isReportTypeSelected)
                    {
                        foreach (RadioButton radioButton in radioButtons.Items.Cast<RadioButton>())
                        {
                            if (IsFieldRequired(control) && radioButtons.SelectedIndex == -1)
                            {
                                radioButton.Foreground = formControl.RequiredBorderBrush;
                            }
                            else
                            {
                                radioButton.Foreground = new SolidColorBrush(Colors.Black);
                            }
                        }
                    }
                    else if (radioButtons.Name == "reportType" && !isReportTypeSelected)
                    {
                        foreach (RadioButton radioButton in radioButtons.Items)
                        {
                            radioButton.Foreground = formControl.RequiredBorderBrush;
                        }
                    }
                    else
                    {
                        foreach (RadioButton radioButton in radioButtons.Items.Cast<RadioButton>())
                        {
                            radioButton.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                }
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty OperatorCallsignProperty =
        //	DependencyProperty.Register("OperatorCallsign", typeof(string), typeof(FormControlBase), null);

        //public virtual UserControlViewModelBase ViewModelBase
        //{ get; set; }
    
        public virtual FormHeaderUserControl FormHeaderControl
        { get; set; }

        public virtual RadioOperatorUserControl RadioOperatorControl
        { get; set; }

        //public virtual string MessageBody
        //{ get; set; }

        //protected static string TimeCheck(string time)
        //{
        //    try
        //    {
        //        var filteredTime = time.Split(new char[] { ':' });
        //        if (filteredTime.Length == 1 && time.Length > 2)
        //        {
        //            string min = time.Substring(time.Length - 2);
        //            if (min.Length > 2 || Convert.ToInt32(min) > 59)
        //                return "";
        //            string hour = time.Substring(0, time.Length - 2);
        //            if (hour.Length > 2 || Convert.ToInt32(hour) > 23)
        //                return "";
        //            return $"{hour}:{min}";
        //        }
        //        else if (time.Length > 2 && time.Length < 6)
        //        {
        //            return time;
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        public virtual FormControlAttribute.FormType FormControlType
        { get; set; }

        public abstract FormProviders FormProvider
        { get; }

        public virtual string FormControlName
        { get; set; }

        public abstract string PacFormType
        { get; }

        
        public abstract string CreateSubject();

        public abstract void AppendDrillTraffic();

        protected override void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
        {
            int count = VisualTreeHelper.GetChildrenCount(panelName);

            for (int i = 0; i < count; i++)
            {
                DependencyObject control = VisualTreeHelper.GetChild(panelName, i);

                switch (control)
                {
                    case StackPanel _:
                    case Grid _:
                    case Border _:
                    case RelativePanel _:
                        ScanControls(control, formUserControl);
                        break;
                    case TextBox textBox:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                            if (textBox.IsReadOnly)
                            {
                                formControl.BaseBorderColor = textBox.Background;
                            }
                            else
                            {
                                formControl.BaseBorderColor = textBox.BorderBrush;
                            }
                            _formControlsList.Add(formControl);
                            break;
                        }

                    case ComboBox comboBox:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                            {
                                BaseBorderColor = comboBox.BorderBrush
                            };
                            _formControlsList.Add(formControl);
                            break;
                        }

                    case CheckBox _:
                    case RichTextBlock _:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                            _formControlsList.Add(formControl);
                            break;
                        }

                    case AutoSuggestBox autoSuggestBox:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                            {
                                BaseBorderColor = TextBoxBorderBrush
                            };
                            if (formControl.UserControl is AutoSuggestTextBoxUserControl)
                            {
                                autoSuggestBox.Name = formControl.UserControl.Name;
                                autoSuggestBox.Tag = formControl.UserControl.Tag;
                            }
                            _formControlsList.Add(formControl);
                            break;
                        }

                    case RadioButtons _:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                            _formControlsList.Add(formControl);
                            break;
                        }

                    case AutoSuggestTextBoxUserControl _:
                        ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                        break;
                    case FormHeaderUserControl _:
                        ScanControls((control as FormHeaderUserControl).Panel, control as FrameworkElement);
                        break;
                    case RadioOperatorUserControl _:
                        ScanControls((control as RadioOperatorUserControl).Panel, control as FrameworkElement);
                        break;
                }
            }
        }

        public override void LockForm()
        {
            //_logHelper.Log(LogLevel.Debug, "Enter LockForm in FormControlBase.cs");

            base.LockForm();

            bool FormHeaderLockFormCalled = false; 
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.UserControl == null || FormHeaderLockFormCalled)
                    continue;

                if (formControl.UserControl is FormHeaderUserControl formHeader)
                {
                    formHeader.FormPacketMessage = FormPacketMessage;
                    formHeader.LockForm();
                    FormHeaderLockFormCalled = true;
                }
                else if (formControl.UserControl is AutoSuggestTextBoxUserControl autosuggestTextBox)
                {
                    autosuggestTextBox.FormPacketMessage = FormPacketMessage;
                }
                else if (formControl.UserControl is RadioOperatorUserControl userControl)
                {
                    userControl.FormPacketMessage = FormPacketMessage;
                }
            }
        }

        public virtual void FormatTextBoxes()
        {
            // Used for customized formatting. For example in Municipal Status the numbers are moved from right to left in loscked state
        }

        public virtual string CreateOutpostData(ref PacketMessage packetMessage)
        {
            string line1 = "!SCCoPIFO!";
            switch (packetMessage.HandlingOrder)
            {
                case "priority":
                    //line1 = ""
                    break;
                
                case "immediate":
                    line1 = "!URG!!SCCoPIFO!";
                    break;
            }

            if (FormHeaderControl == null)
            {
                _outpostData = new List<string>
                {
                    {line1},
                    $"#T: {FormControlName}.html",
                    $"#V: {ViewModelBase.PackItFormVersion}-{ViewModelBase.PIF}",
                };
            }
            else 
            {
                _outpostData = new List<string>
                {
                    {line1},
                    $"#T: {FormControlName}.html",
                    $"#V: {ViewModelBase.PackItFormVersion}-{FormHeaderControl.ViewModelBase.PIF}",
                };
            }
            CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

            return CreateOutpostMessageBody(_outpostData);
        }


        protected virtual void CreateOutpostDataFromFormFields(ref PacketMessage packetMessage, ref List<string> outpostData)
        {
            if (packetMessage.FormFieldArray is null)
            {
                // This may happen if called from view Outpost DataDefault
                packetMessage.FormFieldArray = CreateFormFieldsInXML();
                FillFormFromFormFields(packetMessage.FormFieldArray);
            }
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                string data = CreateOutpostDataString(formField, packetMessage.FormProvider);
                if (string.IsNullOrEmpty(data))
                {
                    continue;
                }
                outpostData.Add(data);
            }
            switch (packetMessage.FormProvider)
            {
                case FormProviders.PacForm:
                    outpostData.Add("#EOF");
                    break;
                case FormProviders.PacItForm:
                    outpostData.Add("!/ADDON!");
                    break;
            }
        }

        protected virtual string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData?.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            
            return comboBoxDataSet?[0];
        }

        protected virtual FormField[] ConvertFromOutpostPackItForm(FormField[] formFields, ref string[] msgLines)
        {
            foreach (FormField formField in formFields)
            {
                (string id, FrameworkElement control) = GetTagIndex(formField);
                formField.ControlIndex = id;

                if (control is RadioButtons radioButtons)
                {
                    formField.ControlContent = GetOutpostValue(id, ref msgLines);
                }
                else if (control is CheckBox)
                {
                    formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "checked" ? "True" : "False");
                }
                else if (control is ComboBox)
                {
                    formField.ControlContent = ConvertComboBoxFromOutpost(id, ref msgLines);
                }
                else if (control is TextBox || control is AutoSuggestBox)
                {
                    //if (control.Name == "senderMsgNo")
                    //{
                    //    formField.ControlContent = senderMsgNo;
                    //}
                    //else
                    {
                        formField.ControlContent = GetOutpostValue(id, ref msgLines);
                    }
                }
            }
            return formFields;
        }

        public virtual FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines, FormProviders formProvider)
        {
            FormField[] formFields = CreateEmptyFormFieldsArray();

            // Populate Sender Message Number from received message number
            // Sender message number can be either in 1 or 3 in PACFORM
            if (formProvider == FormProviders.PacItForm)
            {
                formFields = ConvertFromOutpostPackItForm(formFields, ref msgLines);
            }
            else
            {
                // PACForm
                string senderMsgNo = "";
                    
                //if (!string.IsNullOrEmpty(GetOutpostValue("1", ref msgLines)))
                //{
                //    senderMsgNo = GetOutpostValue("1", ref msgLines);
                //}
                //else if (!string.IsNullOrEmpty(GetOutpostValue("3", ref msgLines)))
                //{
                //    senderMsgNo = GetOutpostValue("3", ref msgLines);
                //}
                foreach (FormField formField in formFields)
                {
                    (string id, FrameworkElement control) = GetTagIndex(formField);
                    formField.ControlIndex = id;
                    if (control is RadioButtons)
                    {
                        formField.ControlContent = GetOutpostValue(id, ref msgLines);
                    }
                    else if (control is CheckBox)
                    {
                        formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "true" ? "True" : "False");
                    }
                    else if (control is ComboBox)
                    {
                        //formField.ControlContent = ConvertComboBoxFromOutpost(id, ref msgLines);
                        formField.ControlContent = GetOutpostValue(id, ref msgLines);   // Modified to save the whole message
                        //string comboBoxData = GetOutpostValue(id, ref msgLines);
                    }
                    else if (control is TextBox || control is AutoSuggestBox)
                    {
                        if (control.Name == "senderMsgNo")
                        {
                            formField.ControlContent = senderMsgNo;
                        }
                        else
                        {
                            formField.ControlContent = GetOutpostValue(id, ref msgLines);
                            // Filter operator date and time
                            if (!string.IsNullOrEmpty(formField.ControlContent))
                            {
                                int index = formField.ControlContent.IndexOf("{odate");
                                if (index > 0)
                                {
                                    formField.ControlContent = formField.ControlContent.Substring(0, index);
                                }
                                index = formField.ControlContent.IndexOf("{otime");
                                if (index > 0)
                                {
                                    formField.ControlContent = formField.ControlContent.Substring(0, index);
                                }
                            }
                        }
                    }
                }
            }
            return formFields;
        }

        public FrameworkElement GetFrameworkElement(FormField formField)
        {
            if (formField is null)
                return (null);

            FormControl formControl;
            if (!string.IsNullOrEmpty(formField.ControlName))   // Use Name since index can be tha same for RadioButton and TextBox in ICS213
            {
                formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
            }
            else
            {
                formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == formField.ControlIndex);                
            }

            return formControl?.InputControl;
        }

        public (string id, FrameworkElement control) GetTagIndex(FormField formField)
        {
            if (formField is null)
                return ("", null);

            FrameworkElement control = null;
            try
            {
                // The control may not have a name, but the tag index is defined
                if (!string.IsNullOrEmpty(formField.ControlIndex))
                {
                    // Index is known, maybe no name
                    foreach (FormControl frmControl in _formControlsList)
                    {
                        string tag = frmControl.InputControl.Tag as string;
                        if (!string.IsNullOrEmpty(tag))
                        {
                            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tags[0] == formField.ControlIndex)
                            {
                                return (tags[0], frmControl.InputControl);
                            }
                        }
                    }
                }
                else
                {
                    // Name is known
                    FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                    control = formControl?.InputControl;

                    string tag = (string)control.Tag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!(tags[0].Contains("required") || tags[0].Contains("conditionallyrequired")))
                        {
                            return (tags[0], control);
                        }
                    }
                }
            }
            catch
            {
            }
            return ("", control);
        }


        //public string GetTagErrorMessage(FormField formField)
        //{
        //    //Control control = formField.InputControl;
        //    string name = _formControlsList[1].InputControl.Name;
        //    FormControl formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
        //    FrameworkElement control = formControl.InputControl;
        //    string tag = control.Tag as string;
        //    if (string.IsNullOrEmpty(tag))
        //        return "";

        //    string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    if (tags.Length == 2 && tags[0].Contains("required"))
        //    {
        //        return tags[1];
        //    }
        //    else if (tags.Length == 3)
        //    {
        //        return tags[2];
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        protected virtual string CreateComboBoxOutpostDataString(FormField formField, string id)
        {
            switch (FormProvider)
            {
                case FormProviders.PacForm:
                    string[] data = formField.ControlContent.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length == 2)
                    {
                        if (data[1] == (-1).ToString() || string.IsNullOrEmpty(data[1]))
                        {
                            return $"{id}: [}}0]";
                        }
                        else
                        {
                            int index = Convert.ToInt32(data[1]);
                            return $"{id}: [{data[0]}}}{index + 1}]";
                            //return $"{id}: [{data[0]}}}{data[1]}]";
                        }
                    }
                    else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
                    {
                        return $"{id}: [}}0]";
                    }
                    break;
                case FormProviders.PacItForm:
                    if (!string.IsNullOrEmpty(formField.ControlComboxContent?.PacketData))
                    {
                        return $"{id}: [{formField.ControlComboxContent.PacketData}]";
                    }
                    else if (formField.ControlComboxContent == null)
                    {
                        return $"{id}: [{formField.ControlContent}]";
                    }
                    else
                    {
                        string[] selection = formField.ControlContent.Split(new char[] { ' ' });
                        return $"{id}: [{selection[0]}]";
                    }
            }
            return "";
        }

        public string CreateOutpostDataString(FormField formField, FormProviders formProvider)
        {
            //(string id, FrameworkElement control) = GetTagIndex(formField);
            FrameworkElement control = GetFrameworkElement(formField);
            string id = formField.ControlIndex;
            if (string.IsNullOrEmpty(id))
                return "";
            if (control is TextBox textBox)
            {
                if (formProvider == FormProviders.PacForm)
                {
                    if (textBox.AcceptsReturn)
                    {
                        return $"{id}: [\\n{formField.ControlContent}]";
                    }
                    else
                    {
                        if (formField.ControlName == "operatorDate")
                        {
                            return $"{id}: [{formField.ControlContent}" + "{odate]";
                        }
                        else if (formField.ControlName == "operatorTime")
                        {
                            return $"{id}: [{formField.ControlContent}" + "{otime]";
                        }
                        else
                        {
                            return $"{id}: [{formField.ControlContent}]";
                        }
                    }
                }
                else if (formProvider == FormProviders.PacItForm)
                {
                    return $"{id}: [{formField.ControlContent}]";
                }
            }
            else if (control is AutoSuggestBox)
            {
                return $"{id}: [{formField.ControlContent}]";
            }
            else if (control is RadioButtons)
            {
                if (formProvider == FormProviders.PacItForm)
                {
                    return $"{id}: [{formField.ControlContent}]";
                }
                else if (formProvider == FormProviders.PacForm)
                {
                    return $"{id}: [{formField.ControlContent}]";
                }
            }
            else if (control is CheckBox)
            {
                if (formField.ControlContent == "True")
                {
                    if (formProvider == FormProviders.PacForm)
                    {
                        return $"{id}: [true]";
                    }
                    else if (formProvider == FormProviders.PacItForm)
                    {
                        return $"{id}: [checked]";
                    }
                }
                else
                {
                    return "";
                }
                        }
            else if (control is ComboBox)
            {
                return CreateComboBoxOutpostDataString(formField, id);
            }
            return "";
        }

        protected static string CreateOutpostMessageBody(List<string> outpostData)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in outpostData)
			{
				sb.Append(s + "\n");          // Added \n
			}
			string outpostDataMessage = sb.ToString();
			return outpostDataMessage;
		}

		public FormField[] CreateEmptyFormFieldsArray()
		{
			FormField[] formFields = new FormField[_formControlsList.Count];

            for (int i = 0; i < _formControlsList.Count; i++)
            {
                FormField formField = new FormField()
                {
                    ControlName = _formControlsList[i].InputControl.Name,
                    ControlContent = "",
                    ControlIndex = GetTagIndex(_formControlsList[i].InputControl),
                };
                formFields.SetValue(formField, i);

            }
            return formFields;
        }

        public FormField[] CreateFormFieldsInXML()
        {
            FormField[] formFields = new FormField[_formControlsList.Count];

        	for (int i = 0; i < _formControlsList.Count; i++)
			{
                FormField formField = new FormField()
                {
                    ControlName = _formControlsList[i].InputControl.Name,
                    ControlIndex = GetTagIndex(_formControlsList[i].InputControl),
                };

                if (_formControlsList[i].InputControl is TextBox textBox)
                {
                    //string noCRText = textBox.Text.Replace("\r", "\n");
                    string noCRText = textBox.Text.Replace("\r", "\\n");

                    formField.ControlContent = noCRText;
                    //if (_formControlsList[i].UserControl == null)
                    //{
                    //formField.ControlContent = textBox.Text;
                    //if (((TextBox)formFieldsList[i]).IsSpellCheckEnabled)
                    //{
                    //	for (int j = 0; j < ((TextBox)formFieldsList[i]).Text.Length; j++)
                    //	{
                    //		int spellingErrorIndex = ((TextBox)formFieldsList[i]).GetSpellingErrorStart(j);
                    //		if (spellingErrorIndex < 0)
                    //		{
                    //			continue;
                    //		}
                    //		else
                    //		{
                    //			int spellingErrorLength = ((TextBox)formFieldsList[i]).GetSpellingErrorLength(spellingErrorIndex);
                    //			string misSpelledWord = ((TextBox)formFieldsList[i]).Text.Substring(spellingErrorIndex, spellingErrorLength);
                    //			j += spellingErrorLength;
                    //			if (formField.MisSpells is null || formField.MisSpells.Length == 0)
                    //				formField.MisSpells = misSpelledWord;
                    //			else
                    //				formField.MisSpells += (", " + misSpelledWord);
                    //		}
                    //	}
                    //}
                    //}
                    //else if (_formControlsList[i].UserControl.GetType() == typeof(RadioOperatorUserControl))
                    //{
                    //    RadioOperatorUserControl radioOperatorControl = _formControlsList[i].UserControl as RadioOperatorUserControl;

                    //    var formCtrl = radioOperatorControl.FormControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(_formControlsList[i].InputControl));
                    //    if (formCtrl != null)
                    //    {
                    //        formField.ControlContent = (formCtrl.InputControl as TextBox).Text;
                    //    }
                    //}
                    //else if (_formControlsList[i].UserControl.GetType() == typeof(FormHeaderUserControl))
                    //{
                    //    FormHeaderUserControl formHeaderControl = _formControlsList[i].UserControl as FormHeaderUserControl;

                    //    var formCtrl = formHeaderControl.FormControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(_formControlsList[i].InputControl));
                    //    if (formCtrl != null)
                    //    {
                    //        formField.ControlContent = (formCtrl.InputControl as TextBox).Text;
                    //    }
                    //}
                }
                else if (_formControlsList[i].InputControl is AutoSuggestBox autoSuggestBox)
				{
					formField.ControlContent = autoSuggestBox.Text;
				}
				else if (_formControlsList[i].InputControl is ComboBox comboBox)
                {
                    if (FormProvider == FormProviders.PacForm)
                    {
                        // Note the Item Type must have a ToString() method
                        formField.ControlContent = $"{comboBox.SelectedItem?.ToString()}}}{comboBox.SelectedIndex}";
                    }
                    else if (FormProvider == FormProviders.PacItForm)
                    {
                        //if (comboBox.SelectedItem as ComboBoxPackItItem != null)
                        if (comboBox.SelectedItem is ComboBoxPackItItem)
                        {
                            ComboBoxPackItItem comboBoxPackItItem = comboBox.SelectedItem as ComboBoxPackItItem;
                            comboBoxPackItItem.SelectedIndex = comboBox.SelectedIndex;
                            formField.ControlComboxContent = comboBoxPackItItem;
                            formField.ControlContent = comboBoxPackItItem?.Item;
                        }
                        else if (comboBox.SelectedItem is ComboBoxItem)
                        {
                            ComboBoxItem comboBoxItem = comboBox.SelectedItem as ComboBoxItem;
                            if (comboBoxItem.Tag is null)
                            {
                                formField.ControlContent = comboBoxItem?.Content as string;
                            }
                            else
                            {
                                formField.ControlContent = comboBoxItem?.Tag as string;
                            }                            
                        }
                        else
                        {
                            formField.ControlContent = comboBox.SelectedItem?.ToString();
                        }
                    }
				}
                else if (_formControlsList[i].InputControl is RadioButtons radioButtons)
                {
                    if (FormProvider == FormProviders.PacItForm)
                    {
                        formField.ControlContent = (radioButtons.SelectedItem as RadioButton)?.Tag as string;
                    }
                    else if (FormProvider == FormProviders.PacForm)
                    {
                        RadioButton radioButton = radioButtons.SelectedItem as RadioButton;
                        formField.ControlIndex = GetTagIndex(radioButton);
                        formField.ControlContent = "true";
                    }
                }
                else if (_formControlsList[i].InputControl is CheckBox checkBox)
                {
					formField.ControlContent = checkBox.IsChecked.ToString();
				}
                formFields.SetValue(formField, i);
			}
			return formFields;
		}

        protected virtual void FillComboBoxFromFormFields(FormField formField, ComboBox comboBox)
        {
            if (comboBox.Items.Count == 0)
            {
                if (comboBox.IsEditable && formField.ControlContent.Length != 0)
                {
                    comboBox.Text = formField.ControlContent;
                }
                else
                {
                    return;     // ComboBox is not loaded
                }
            }

            if (FormPacketMessage.FormProvider == FormProviders.PacForm)
            {
                var data = formField.ControlContent.Split(new char[] { '}' });
                if (data.Length == 2)
                {
                    // This is a PacForm ComboBox
                    int index = Convert.ToInt32(data[1]);
                    //if (index < 0 && comboBox.IsEditable) 
                    if (!string.IsNullOrEmpty(data[0]) && comboBox.IsEditable)
                    {
                        comboBox.Text = data[0];
                        //comboBox.SelectedIndex = index;
                        //comboBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //List<TacticalCall> tacticalCalls = TacticalCallsigns.CreateMountainViewCERTList();
                        //int i = 0;
                        //for (; i < tacticalCalls.Count; i++)
                        //{
                        //    if (tacticalCalls[i].AgencyName == data[0])
                        //    {
                        //        break;
                        //    }
                        //}
                        //comboBox.SelectedIndex = i;
                        comboBox.SelectedIndex = index - 1; // Phil's index start with 1
                        //comboBox.Text = data[0]; // Only works if ComboBox is loaded
                    }
                }
            }
            else if (FormPacketMessage.FormProvider == FormProviders.PacItForm)
            {
                // Does not work for ComBoxItem. Use ComBox_Loaded

                // Received forms neds to have their ControlComboxContent updated
                var items = comboBox.Items;

                var selectedItem = comboBox.Items[0];
                var itemSource = comboBox.ItemsSource;

                if (formField.ControlComboxContent != null)
                {
                    // ComboBoxPackItItem control
                    comboBox.SelectedIndex = formField.ControlComboxContent.SelectedIndex;
                    //comboBox.SelectedValue = formField.ControlContent;

                }
                //    else
                //    {
                //        // Use ComboBox_Loaded

                //        // If locked see ComboBoc_Loaded
                //        // Check if Tag is used
                //        //var count = comboBox.Items.Count;
                //        bool tagFound = false;
                //        //foreach (ComboBoxItem comboBoxItem in comboBox.Items)
                //        //{
                //        //    if ((comboBoxItem.Tag as string) == formField.ControlContent)
                //        //    {
                //        //        tagFound = true;
                //        //        comboBox.SelectedItem = comboBoxItem;
                //        //        break;
                //        //    }
                //        //}
                //        //if (!tagFound)
                //        //{
                //        //comboBox.SelectedValue = formField.ControlContent;
                //        //}
                //    }
            }
        }

        public virtual void FillFormFromFormFields(FormField[] formFields)
		{
            foreach (FormField formField in formFields)
            {
                FormControl formControl;
                if (!string.IsNullOrEmpty(formField.ControlName))
                {
                    formControl = _formControlsList.Find(x => x.InputControl.Name == formField.ControlName);
                }
                else
                {
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == formField.ControlIndex);
                    continue;           // Name is needed
                }

                FrameworkElement control = formControl?.InputControl;

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    textBox.Text = formField.ControlContent;
                    //string controlContent = formField.ControlContent;
                    textBox.Text = textBox.Text.Replace("\\n", "\n");
                    //controlContent = controlContent.Replace("\\n", "\n");
                    if (formControl.UserControl == null)
                    {
                        // Fields that use Binding requires special handling
                        switch (control.Name)
                        {
                            case "msgDate":
                                ViewModelBase.MsgDate = textBox.Text;
                                break;
                            case "msgTime":
                                ViewModelBase.MsgTime = textBox.Text;
                                break;
                            case "operatorCallsign":
                                ViewModelBase.OperatorCallsign = textBox.Text;
                                break;
                            case "operatorName":
                                ViewModelBase.OperatorName = textBox.Text;
                                break;
                            case null:
                                continue;
                        }
                    }
                    else
                    {
                        if (formControl.UserControl.GetType() == typeof(RadioOperatorUserControl))
                        {
                            RadioOperatorUserControl radioOperatorControl = formControl.UserControl as RadioOperatorUserControl;

                            switch (control.Name)
                            {
                                case "operatorCallsign":
                                    radioOperatorControl.ViewModel.OperatorCallsign = textBox.Text;
                                    break;
                                case "operatorName":
                                    radioOperatorControl.ViewModel.OperatorName = textBox.Text;
                                    break;
                                case null:
                                    continue;
                            }
                        }
                        else if (formControl.UserControl.GetType() == typeof(FormHeaderUserControl))
                        {
                            FormHeaderUserControl formHeaderControl = formControl.UserControl as FormHeaderUserControl;

                            switch (control.Name)
                            {
                                case "messageNo":
                                    formHeaderControl.ViewModel.OriginMsgNo = textBox.Text;
                                    break;
                                case "destinationMsgNo":
                                    formHeaderControl.ViewModel.DestinationMsgNo = textBox.Text;
                                    break;
                                case "msgDate":
                                    formHeaderControl.ViewModel.MsgDate = textBox.Text;
                                    break;
                                case "msgTime":
                                    formHeaderControl.ViewModel.MsgTime = textBox.Text;
                                    break;
                                case null:
                                    continue;
                            }
                        }
                    }
                }
                else if (control is RichTextBlock richTextBlock)
                {
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run
                    {
                        Text = formField.ControlContent
                    };
                    // Add the Run to the Paragraph, the Paragraph to the RichTextBlock.
                    paragraph.Inlines.Add(run);
                    richTextBlock.Blocks.Add(paragraph);
                }
                else if (control is AutoSuggestBox autoSuggsetBox)
                {
                    autoSuggsetBox.Text = formField.ControlContent;
                }
                //else if (control is ComboBox comboBox && control.Visibility == Visibility.Visible)
                else if (control is ComboBox comboBox)
                {
                    if (formControl.UserControl == null)
                    {
                        FillComboBoxFromFormFields(formField, comboBox);
                    }
                    else if (formControl.UserControl.GetType() == typeof(FormHeaderUserControl))
                    {
                        FormHeaderUserControl formHeaderControl = formControl.UserControl as FormHeaderUserControl;
                        if (control.Name == "comboBoxToICSPosition" && formHeaderControl.ToICSPositionComboBoxItems != null)
                        {
                            int i = 0;
                            //bool found = false;
                            foreach (ComboBoxItem comboBoxItem in formHeaderControl.ToICSPositionComboBoxItems)
                            {
                                if (formField.ControlContent.ToLower() == ((string)comboBoxItem.Content).ToLower())
                                {
                                    //found = true;
                                    comboBox.SelectedIndex = i;
                                    break;
                                }
                                i++;
                            }
//                            if (!found && (control as ComboBox).IsEditable)
//                            {
//                                comboBox.Text = formField.ControlContent;                            
//                            }
                        }
                        else if (control.Name == "toLocationComboBox" && formHeaderControl.ToLocationComboBoxItems != null)
                        {
                            if (formHeaderControl.ToLocationComboBoxItems != null)
                            {
                                int i = 0;
                                foreach (ComboBoxItem comboBoxItem in formHeaderControl.ToLocationComboBoxItems)
                                {
                                    if (formField.ControlContent.ToLower() == ((string)comboBoxItem.Content).ToLower())
                                    {
                                        comboBox.SelectedIndex = i;
                                        break;
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                }
                else if (control is RadioButtons radioButtons)
                {
                    if (formControl.UserControl == null)
                    {
                        if (formControl.InputControl.Name != "receivedOrSent")
                        {
                            int i = 0;
                            foreach (RadioButton radioButton in radioButtons.Items.Cast<RadioButton>())
                            {
                                if (FormPacketMessage.FormProvider == FormProviders.PacItForm)
                                {
                                    if (formField.ControlContent.ToLower() == ((string)radioButton.Tag)?.ToLower())
                                    {
                                        radioButtons.SelectedIndex = i;
                                        break;
                                    }
                                }
                                if (FormPacketMessage.FormProvider == FormProviders.PacForm)
                                {
                                    if (formField.ControlIndex == radioButton.Tag as string && formField.ControlContent == "true")
                                    {
                                        radioButtons.SelectedIndex = i;
                                        break;
                                    }
                                }
                                i++;
                            }
                        }
                        else if (formControl.InputControl.Name == "receivedOrSent")
                        {
                            if (ViewModelBase.ReceivedOrSent == "Receiver")
                            {
                                radioButtons.SelectedIndex = 0;
                            }
                            else
                            {
                                radioButtons.SelectedIndex = 1;
                            }
                        }
                    }
                    else if (formControl.UserControl.GetType() == typeof(FormHeaderUserControl))
                    {
                        FormHeaderUserControl formHeaderControl = formControl.UserControl as FormHeaderUserControl;
                        if (control.Name == "handlingOrder")
                        {
                            formHeaderControl.ViewModelBase.HandlingOrder = formField.ControlContent;
                            int i = 0;
                            foreach (RadioButton radioButton in radioButtons.Items.Cast<RadioButton>())
                            {
                                if (formField.ControlContent.ToLower() == ((string)radioButton.Tag).ToLower())
                                {
                                    radioButtons.SelectedIndex = i;
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.IsChecked = formField.ControlContent == "True";
                }
                else if (control is AutoSuggestTextBoxUserControl autoSuggestTextBoxUserControl)
                {
                    if (formControl.UserControl.GetType() == typeof(FormHeaderUserControl))
                    {
                        FormHeaderUserControl formHeaderControl = formControl.UserControl as FormHeaderUserControl;
                        if (control.Name == "toLocation")
                        {
                            autoSuggestTextBoxUserControl.SetText(formField.ControlContent);
                        }
                    }
                }
            }
            UpdateFormFieldsRequiredColors();
        }

		public static string GetOutpostFieldValue(string field)
        {
            int startIndex = field.IndexOf('[');
            int endIndex = field.IndexOf(']');
            if (startIndex != -1 && endIndex != -1)
            {
                if (field.Substring(startIndex + 1, endIndex - startIndex - 1).StartsWith("\\n"))   // For PacForms
                {
                    return field.Substring(startIndex + 3, endIndex - startIndex - 3);
                }
                else
                {
                    return field.Substring(startIndex + 1, endIndex - startIndex - 1);
                }
            }
            else
            {
                return "";
            }
        }

        public static string GetOutpostValue(string msgLine)
        {
            int index = msgLine.IndexOf(':');
            if (index == -1)
            {
                return "";
            }
            return GetOutpostFieldValue(msgLine);
        }

        public static string GetOutpostValue(string fieldIdent, ref string[] msgLines)
		{
			for (int i = 4; i < msgLines.Length; i++)
			{
				int index = msgLines[i].IndexOf(':');
				if (index == -1)
				{
					continue;
				}
                if (fieldIdent == msgLines[i].Substring(0, index))
				{
					return GetOutpostFieldValue(msgLines[i]);
				}
			}
			return null;
		}

        protected static string ConvertTabsToSpaces(string text, int tabWidth)
		{
			StringBuilder sb = new StringBuilder();
			var lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			string[] convertedLines = new string[lines.Length];
			foreach (string line in lines)
			{
				string convertedLine = ConvertLineTabsToSpaces(line, tabWidth);
				sb.AppendLine(convertedLine);
			}
			return sb.ToString();
		}

		protected static string ConvertLineTabsToSpaces(string line, int tabWidth)
		{
			string convertedLine;
			StringBuilder sb = new StringBuilder();
			int j = 0;
			for (int i = 0; i < line.Length; i++, j++)
			{
				if (line[i] != '\t')
				{
					sb.Append(line[i]);
				}
				else
				{
					j--;
					int spaceCount = tabWidth - j % tabWidth;
					for (int k = 0; k < spaceCount; k++)
					{
						sb.Append(' ');
						j++;
					}
				}
			}
			convertedLine = sb.ToString();

			return convertedLine;
		}

        public virtual void SetPracticeField(string practiceField)  
        {
            if (practiceField is null)
            {
                practiceField = "";
                SetPracticeField(practiceField);
            }
        }

        protected virtual void UpdateRequiredFields(bool required)
        {
            if (!required)
            {
            }
            else
            {
            }
            UpdateFormFieldsRequiredColors();
        }

        protected static void CreateComboBoxList(List<ComboBoxItem> comboBoxList, List<ComboBoxItem> comboBoxRefList)
        {
            for (int i = 0; i < comboBoxRefList.Count; i++)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem()
                {
                    Content = comboBoxRefList[i].Content,
                    Background = comboBoxRefList[i].Background,
                    Tag = comboBoxRefList[i].Tag,
                };
                comboBoxList.Add(comboBoxItem);
            }
        }

        protected void GetFormDataFromAttribute(Type formType)
        {
            if (string.IsNullOrEmpty(FormControlName) 
                || FormControlType == FormControlAttribute.FormType.Undefined)
            {
                Assembly assembly = formType.Assembly;

                Type[] exportedTypes = assembly.GetExportedTypes();
                foreach (Type classType in exportedTypes)
                {
                    var attrib = classType.GetTypeInfo();
                    CustomAttributeData customAttribute = attrib.CustomAttributes.FirstOrDefault(attribute => attribute.AttributeType == typeof(FormControlAttribute));
                    if (customAttribute is null)
                    {
                        continue;
                    }
                    IList<CustomAttributeNamedArgument> namedArguments = customAttribute.NamedArguments;
                    string formControlMenuName;
                    bool formControlNameFound = false;
                    bool formControlTypeFound = false;
                    bool formControlMenuNameFound = false;
                    foreach (CustomAttributeNamedArgument arg in namedArguments)
                    {
                        switch (arg.MemberName)
                        {
                            case "FormControlName":
                                FormControlName = arg.TypedValue.Value as string;
                                formControlNameFound = true;
                                break;
                            case "FormControlType":
                                FormControlAttribute.FormType formControlType = (FormControlAttribute.FormType)Enum.Parse(typeof(FormControlAttribute.FormType), arg.TypedValue.Value.ToString());
                                FormControlType = formControlType;
                                formControlTypeFound = true;
                                break;
                            case "FormControlMenuName":
                                formControlMenuName = arg.TypedValue.Value as string;
                                formControlMenuNameFound = true;
                                break;
                        }
                        if (formControlNameFound && formControlTypeFound && formControlMenuNameFound)
                        {
                            return;
                        }
                    }
                    break;
                }
            }
        }

        public virtual void MessageChanged(string message)      // Must be public, not protected
        { }

        public virtual void MsgTimeChanged(string msgTime)      // Must be public, not protected
        { }

        //protected virtual void Required_Checked(object sender, RoutedEventArgs e)
        //{
        //    UpdateFormFieldsRequiredColors();
        //}

        //protected virtual void ReportType_Checked(object sender, RoutedEventArgs e)
        //{
        //    bool complete = (bool)(sender as RadioButton).IsChecked && (sender as RadioButton).Name == "complete";
        //    UpdateRequiredFields(complete);
        //}

        protected virtual void ReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] != null)
            {
                RadioButton radioButton = e.AddedItems[0] as RadioButton;
                bool complete = radioButton.Name == "complete";
                UpdateRequiredFields(complete);
            }
        }

        protected virtual void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormPacketMessage == null)
                return;

            ComboBox comboBox = sender as ComboBox;

            TextBox textBox = FindName($"{comboBox.Name}TextBox") as TextBox;

            if (FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxItem>)
            {
                FormField formField = FormPacketMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == comboBox.Name);
                if (!string.IsNullOrEmpty(formField.ControlContent))
                {
                    ComboBoxItem comboBoxItem = (comboBox.ItemsSource as List<ComboBoxItem>).FirstOrDefault
                            (c => (c.Tag as string) == formField.ControlContent 
                               || (c.Content as string) == formField.ControlContent);
                    if (FormPacketMessage.MessageState == MessageState.Locked)
                    {
                        if (comboBoxItem?.Background != null)
                        {
                            textBox.Background = comboBoxItem.Background;
                        }
                        //textBox.Text = comboBoxItem.Content as string;
                        textBox.Text = comboBox.Text;
                    }
                    else
                    {
                        comboBox.SelectedItem = comboBoxItem;
                    }
                }

            }
            else if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is string[])
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    if (formField.ControlName == comboBox.Name && !string.IsNullOrEmpty(formField.ControlContent))
                    {
                        FillComboBoxFromFormFields(formField, comboBox);
                        break;
                    }
                }
                UpdateFormFieldsRequiredColors();
            }
            else if (FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<TacticalCall>)
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    if (formField.ControlName == comboBox.Name && !string.IsNullOrEmpty(formField.ControlContent))
                    {
                        FillComboBoxFromFormFields(formField, comboBox);
                        break;
                    }
                }
                UpdateFormFieldsRequiredColors();
            }
            else if (FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxPackItItem>)
            {
                if (FormPacketMessage.MessageState != MessageState.Locked)
                    return;

                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    //if (formField.ControlName == comboBox.Name && formField.ControlComboxContent.SelectedIndex < 0)
                    if (formField.ControlName == comboBox.Name)
                    {
                        //comboBox.SelectedValue = formField.ControlContent;
                        int index = 0;
                        foreach (ComboBoxPackItItem packItItem in comboBox.ItemsSource as List<ComboBoxPackItItem>)
                        {
                            if (packItItem.PacketData == formField.ControlContent)
                            {
                                packItItem.SelectedIndex = index;
                                comboBox.SelectedIndex = index;
                                //if (FormPacketMessage.MessageState == MessageState.Locked)
                                //{
                                //    textBox = FindName($"{comboBox.Name}TextBox") as TextBox;
                                textBox.Background = packItItem.BackgroundBrush;
                                //}
                                break;
                            }
                            index++;
                        }
                        break;
                    }
                }
            }
        }

        //protected virtual void ComboBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ComboBox comboBox = sender as ComboBox;
        //    if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null 
        //        && comboBox.ItemsSource is List<ComboBoxPackItItem>
        //        && FormPacketMessage.MessageState == MessageState.Locked)
        //    {
        //        foreach (FormField formField in FormPacketMessage.FormFieldArray)
        //        {
        //            if (formField.ControlName == comboBox.Name && comboBox.SelectedIndex > -1)
        //            {
        //                int indx = comboBox.SelectedIndex;
        //                comboBox.SelectedValue = formField.ControlContent;
        //                int index = 0;
        //                foreach (ComboBoxPackItItem packItItem in comboBox.ItemsSource as List<ComboBoxPackItItem>)
        //                //ComboBoxPackItItem packItItem = (comboBox.ItemsSource as List<ComboBoxPackItItem>)[index];
        //                {
        //                    if (packItItem.PacketData == formField.ControlContent)
        //                    {
        //                        packItItem.SelectedIndex = index;
        //                        //comboBox.SelectedIndex = index;
        //                        //if (FormPacketMessage.MessageState == MessageState.Locked)
        //                        //{
        //                            TextBox textBox = FindName($"{comboBox.Name}TextBox") as TextBox;
        //                            textBox.Background = packItItem.BackgroundBrush;
        //                        //}
        //                        break;
        //                    }
        //                    index++;
        //                }
        //                break;
        //            }
        //        }
        //    }
        //}

        // This is for filtering non number keys for a number key only field
        //protected void TextBoxResource_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (!((e.Key >= VirtualKey.Number0 && e.Key <= VirtualKey.Number9) || (e.Key >= VirtualKey.NumberPad0 && e.Key <= VirtualKey.NumberPad9)
        //        || e.Key == VirtualKey.Delete || e.Key == VirtualKey.End || e.Key == VirtualKey.Left || e.Key == VirtualKey.Right
        //        || e.Key == VirtualKey.Back))
        //    {
        //        e.Handled = true;
        //    }
        //}

        protected static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        protected virtual void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RadioButtons radioButtons = sender as RadioButtons;

            foreach (RadioButton radioButton in radioButtons.Items)
            {
                if (IsFieldRequired(radioButtons) && radioButtons.SelectedIndex == -1)
                {
                    radioButton.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    radioButton.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

#region Print
        public abstract Panel CanvasContainer
        { get;  }

        public abstract Panel DirectPrintContainer
        { get;  }

        public abstract List<Panel> PrintPanels
        { get; }


        protected void AddFooter()
        {
            for (int i = 0; i < _printPanels.Count; i++)
            {
                bool footerFound = false;
                foreach (FrameworkElement child in _printPanels[i].Children)
                {
                    if (child is TextBlock textBlock && textBlock.Text.Contains($"page {i + 1} of"))
                    {
                        footerFound = true;
                        break;
                    }
                }

                if (!footerFound)
                {
                    TextBlock footerLeft = new TextBlock
                    {
                        Text = FormHeaderControl != null
                            ? FormHeaderControl.ViewModel.OriginMsgNo
                            : ViewModelBase.OriginMsgNo,
                        Margin = new Thickness(0, 16, 0, 0),
                    };

                    TextBlock footerRight = new TextBlock
                    {
                        Text = $"Page {i + 1} of {_printPanels.Count}",
                        Margin = new Thickness(0, 16, 0, 0),
                        TextAlignment = TextAlignment.Right,
                    };

                    Grid footerGrid = new Grid();
                    ColumnDefinition column0 = new ColumnDefinition();
                    footerGrid.ColumnDefinitions.Add(column0);
                    ColumnDefinition column1 = new ColumnDefinition();
                    footerGrid.ColumnDefinitions.Add(column1);
                    footerGrid.Children.Add(footerLeft);
                    footerGrid.Children.Add(footerRight);
                    Grid.SetColumn(footerLeft, 0);
                    Grid.SetColumn(footerRight, 1);

                    if (_printPanels[i].GetType() == typeof(Grid))
                    {
                        Grid grid = _printPanels[i] as Grid;
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        Grid.SetRow(footerGrid, grid.RowDefinitions.Count - 1);
                        grid.Children.Add(footerGrid);

                    }
                    else
                    {
                        _printPanels[i].Children.Add(footerGrid);
                    }
                }
            }
        }

        public virtual async void PrintForm()
        {
            if (CanvasContainer is null || DirectPrintContainer is null)
                return;

            var defaultPrintHelperOptions = new PrintHelperOptions();
            //defaultPrintHelperOptions.ExtendDisplayedOptions
            _printHelper = new PrintHelper(CanvasContainer);

            _printPanels = PrintPanels;

            if (_printPanels is null || _printPanels.Count == 0)
                return;

            for (int i = 0; i < _printPanels.Count; i++)
            {
                DirectPrintContainer.Children.Remove(_printPanels[i]);
            }

            AddFooter();

            for (int i = 0; i < _printPanels.Count; i++)
            {
                _printHelper.AddFrameworkElementToPrint(_printPanels[i]);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
            _printHelper.OnPreviewPagesCreated += PrintHelper_OnPreviewPagesCreated;

            await _printHelper.ShowPrintUIAsync(" ");
        }

        protected virtual void ReleasePrintHelper()
        {
            _printHelper.Dispose();

            for (int i = 0; i < _printPanels.Count; i++)
            {
                if (_printPanels[i] != null && !DirectPrintContainer.Children.Contains(_printPanels[i]))
                {
                    foreach (FrameworkElement child in _printPanels[i].Children)
                    {
                        if (child is Grid grid)
                        {
                            foreach (FrameworkElement gridChild in grid.Children)
                            {
                                if (gridChild is TextBlock textBlock && textBlock.Text.Contains($"Page {i + 1} of"))
                                {
                                    _printPanels[i].Children.Remove(grid);
                                }
                            }
                        }
                    }
                    DirectPrintContainer.Children.Add(_printPanels[i]);
                }
            }
        }

        protected virtual void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
        }

        protected virtual async void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();

            await ContentDialogs.ShowSingleButtonContentDialogAsync("Print failed");
            //logHelper.Log(LogLevel.Error, $"Print failed. {_ICS309ViewModel.OperationalPeriod}");
        }

        protected virtual void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }

        protected virtual async void PrintHelper_OnPreviewPagesCreated(List<FrameworkElement> FrameworkElementList)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("PrintTest.txt", CreationCollisionOption.ReplaceExisting);
            //string fileContent = await Json.StringifyAsync(content);

            //await FileIO.WriteTextAsync(file, FrameworkElementList[0].ToString());

        }

        #endregion Print

    }
}


