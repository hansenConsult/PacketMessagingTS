﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using FormControlBasicsNamespace;

using FormUserControl;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Helpers.PrintHelpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Documents;
using Windows.Storage;
using System.Linq;

namespace FormControlBaseClass
{
    //   // This is for deciding at runtime which form is supported by an assembly
    //   [AttributeUsage(AttributeTargets.Class)]
    //public class FormControlAttribute : Attribute
    //{
    //	public enum FormType
    //	{
    //		None,
    //		CountyForm,
    //		CityForm,
    //		HospitalForm,
    //           TestForm,
    //	};

    //       // Form file name
    //	public string FormControlName { get; set; }    // 

    //       // Form type (County, Hospital etc.)
    //	public FormType FormControlType { get; set; }

    //       // Menu text
    //	public string FormControlMenuName { get; set; }    // 
    //}

 //   public sealed class FormEventArgs : EventArgs
	//{
	//	//public FormEventArgs() { }

	//	//public FormEventArgs(string tacticalCallsign)
	//	//{
	//	//	TacticalCallsign = tacticalCallsign;
	//	//}

	//	//public string TacticalCallsign
	//	//{ get; set; }

	//	public string SubjectLine
	//	{ get; set; }
	//}

    public abstract partial class FormControlBase : FormControlBasics
    {
        protected List<string> _outpostData;

        protected ScrollViewer _scrollViewer;

        readonly protected List<ComboBoxPackItItem> Hospitals = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem("El Camino Hospital Los Gatos"),
            new ComboBoxPackItItem("El Camino Hospital Mountain View"),
            new ComboBoxPackItItem("Good Samaritan Hospital"),
            new ComboBoxPackItItem("Kaiser Gan Jose Medical Center"),
            new ComboBoxPackItItem("Kaiser Santa Clara Hospital"),
            new ComboBoxPackItItem("Lucile Packard Children's Hospital"),
            new ComboBoxPackItItem("O'Connor Hospital"),
            new ComboBoxPackItItem("Palo Alto Veterans Hospital"),
            new ComboBoxPackItItem("Regional San Jose Medical Center"),
            new ComboBoxPackItItem("Saint Loise Regional Hospital"),
            new ComboBoxPackItItem("Stanford Hospital"),
            new ComboBoxPackItItem("Stanford School of Medicine"),
            new ComboBoxPackItItem("Valley Medical Center"),
        };

        protected List<ComboBoxPackItItem> Priority = new List<ComboBoxPackItItem>
        {
            new ComboBoxPackItItem("Now", "Now", 0),
            new ComboBoxPackItItem("High (0-4 hrs.)", "High", 1),
            new ComboBoxPackItItem("Medium (5-12 hrs.)", "Medium", 2),
            new ComboBoxPackItItem("Low (12+ hrs.)", "Low", 3),
        };

        private static Dictionary<string, object> _properties = new Dictionary<string, object>();
        static Dictionary<string, bool> _propertyFirstTime = new Dictionary<string, bool>();

        public static string DrillTraffic = "\rDrill Traffic";

        protected PrintHelper _printHelper;
        protected List<Panel> _printPanels;


        protected T GetProperty<T>(ref T backingStore, [CallerMemberName]string propertyName = "")
        {
            if (_properties != null && _properties.ContainsKey(propertyName))
            {
                // Retrieve value from dictionary
                object o = _properties[propertyName];
                backingStore = (T)o;
                return (T)o;
            }
            else
                return backingStore;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, bool persist = false, bool forceUpdate = false,
                    [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            bool firstTime = false;
            if (_propertyFirstTime.ContainsKey(propertyName))
            {
                firstTime = _propertyFirstTime[propertyName];
            }
            else
            {
                firstTime = true;
            }
            _propertyFirstTime[propertyName] = false;
            //Do not update displayed value if not changed or not first time or not forced
            if (Equals(backingStore, value) && !firstTime && !forceUpdate)
            {
                return false;
            }
            //_propertyFirstTime[propertyName] = false;

            if (persist)
            {
                // store value
                _properties[propertyName] = value;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void UpdateFormFieldsRequiredColors(bool newForm = true)
        {            
            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;
                string name = control.Name;

                if (control is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text) && IsFieldRequired(textBox) && newForm)
                    {
                        textBox.BorderBrush = formControl.RequiredBorderBrush;
                        //textBox.BorderThickness = new Thickness(2);
                    }
                    else if (newForm)
                    {
                        textBox.BorderBrush = formControl.BaseBorderColor;
                        //textBox.BorderThickness = new Thickness(1);
                    }
                    else if (!newForm)
                    {
                        textBox.BorderBrush = new SolidColorBrush(Colors.White);
                        //textBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    if (string.IsNullOrEmpty(autoSuggestBox.Text) && IsFieldRequired(control) && newForm)
                    {
                        autoSuggestBox.BorderBrush = formControl.RequiredBorderBrush;
                        autoSuggestBox.BorderThickness = new Thickness(2);
                    }
                    else if (newForm)
                    {
                        autoSuggestBox.BorderBrush = formControl.BaseBorderColor;
                        autoSuggestBox.BorderThickness = new Thickness(1);
                    }
                    else if (!newForm)
                    {
                        autoSuggestBox.BorderBrush = new SolidColorBrush(Colors.White);
                        autoSuggestBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.SelectedIndex < 0 && IsFieldRequired(control) && newForm)
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                        comboBox.BorderThickness = new Thickness(2);
                    }
                    else if (newForm)
                    {
                        comboBox.BorderBrush = formControl.BaseBorderColor;
                        comboBox.BorderThickness = new Thickness(1);
                    }
                }
                else if (control is ToggleButtonGroup toggleButtonGroup)
                {
                    if (IsFieldRequired(control) && string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()) 
                        && newForm || (toggleButtonGroup.Name == "reportType" && !newForm))
                    {
                        toggleButtonGroup.ToggleButtonGroupBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        toggleButtonGroup.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }

        public override void UpdateStyles()
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.UserControl is FormHeaderUserControl formHeader)
                {
                    formHeader.FormPacketMessage = FormPacketMessage;
                    formHeader.UpdateStyles();
                }

            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty OperatorCallsignProperty =
        //	DependencyProperty.Register("OperatorCallsign", typeof(string), typeof(FormControlBase), null);

        public virtual string TacticalCallsign
        { get; set; }

        public virtual FormHeaderUserControl FormHeaderControl
        { get; set; }

        public virtual RadioOperatorUserControl RadioOperatorControl
        { get; set; }

        public virtual string SenderMsgNo
        { get; set; }

        public virtual string MessageBody
        { get; set; }

        //public virtual string ReceiverMsgNo
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

        public virtual string Action
        { get; set; }

        public virtual string Reply
        { get; set; }

        public virtual string Severity
        { get; set; }

        public virtual string ReceivedOrSent
        { get; set; }

        public virtual string HowReceivedSent
        { get; set; }

        private string _IncidentName;
        public virtual string IncidentName      // Required for setting Practice
        {
            get => _IncidentName;
            set => Set(ref _IncidentName, value);
        }

        public virtual string FacilityName      // Required for setting Practice
        { get; set; }

        // Implemented this way to facilitate synchronizing two name fields and required for setting Practice 
        private string _shelterName;
        public virtual string ShelterName
        {
            get => _shelterName;
            set => Set(ref _shelterName, value);
        }

        public virtual string Subject       // Required for setting Practice
        { get; set; }

        public virtual string ReportType
        { get; set; }

        private DateTime? messageReceivedTime = null;
        public virtual DateTime? MessageReceivedTime
        {
            get => messageReceivedTime;
            set => Set(ref messageReceivedTime, value);
        }

        private DateTime? messageSentTime = null;
        public virtual DateTime? MessageSentTime
        {
            get => messageSentTime;
            set => Set(ref messageSentTime, value);
        }

        public abstract FormControlAttribute.FormType FormControlType
        { get; }

        public abstract FormProviders FormProvider
        { get; }

        public abstract string PacFormName
		{ get; }

        public abstract string PacFormType
        { get; }
        
        public abstract string CreateSubject();

        public abstract void AppendDrillTraffic();

        public virtual string PackItFormVersion => "3.0";

        protected override void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
        {
            int count = VisualTreeHelper.GetChildrenCount(panelName);

            for (int i = 0; i < count; i++)
            {
                DependencyObject control = VisualTreeHelper.GetChild(panelName, i);

                if (control is StackPanel || control is Grid || control is Border || control is RelativePanel)
                {
                    ScanControls(control, formUserControl);
                }
                else if (control is TextBox textBox)
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
                }
                else if (control is ComboBox comboBox)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    formControl.BaseBorderColor = comboBox.BorderBrush;
                    _formControlsList.Add(formControl);
                }
                else if (control is CheckBox || control is ToggleButtonGroup || control is RichTextBlock)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    formControl.BaseBorderColor = TextBoxBorderBrush;
                    if (formControl.UserControl is AutoSuggestTextBoxUserControl)
                    {
                        autoSuggestBox.Name = formControl.UserControl.Name;
                        autoSuggestBox.Tag = formControl.UserControl.Tag;
                    }
                    _formControlsList.Add(formControl);
                }
                else if (control is RadioButton)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);

                    _radioButtonsList.Add((RadioButton)control);
                }
                else if (control is AutoSuggestTextBoxUserControl)
                {
                    ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                }
                else if (control is FormHeaderUserControl)
                {
                    ScanControls((control as FormHeaderUserControl).Panel, control as FrameworkElement);
                }
                else if (control is RadioOperatorUserControl)
                {
                    ScanControls((control as RadioOperatorUserControl).Panel, control as FrameworkElement);
                }
            }
        }

        public void LockForm(FormField[] formFields)
        {
            Messagestate = MessageState.Locked;
            FormFields = formFields;

            //TextBox
            RootPanel.Resources["TextControlBorderBrushPointerOver"] = RootPanel.Resources["ComboBoxFocusedBackgroundThemeBrush"];
            RootPanel.Resources["TextControlBorderBrushFocused"] = RootPanel.Resources["ComboBoxFocusedBackgroundThemeBrush"];
            // CheckBox
            RootPanel.Resources["CheckBoxForegroundCheckedDisabled"] = RootPanel.Resources["CheckBoxForegroundChecked"];
            RootPanel.Resources["CheckBoxBackgroundCheckedDisabled"] = RootPanel.Resources["CheckBoxBackgroundChecked"];
            RootPanel.Resources["CheckBoxBorderBrushCheckedDisabled"] = RootPanel.Resources["CheckBoxBorderBrushChecked"];
            RootPanel.Resources["CheckBoxCheckBackgroundStrokeCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundStrokeChecked"];
            RootPanel.Resources["CheckBoxCheckBackgroundFillCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundFillChecked"];
            RootPanel.Resources["CheckBoxCheckGlyphForegroundCheckedDisabled"] = RootPanel.Resources["CheckBoxCheckGlyphForegroundChecked"];

            RootPanel.Resources["CheckBoxForegroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxForegroundUnchecked"];
            RootPanel.Resources["CheckBoxBackgroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxBackgroundUnchecked"];
            RootPanel.Resources["CheckBoxBorderBrushUncheckedDisabled"] = RootPanel.Resources["CheckBoxBorderBrushUnchecked"];
            RootPanel.Resources["CheckBoxCheckBackgroundStrokeUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundStrokeUnchecked"];
            RootPanel.Resources["CheckBoxCheckBackgroundFillUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckBackgroundFillUnchecked"];
            RootPanel.Resources["CheckBoxCheckGlyphForegroundUncheckedDisabled"] = RootPanel.Resources["CheckBoxCheckGlyphForegroundUnchecked"];

            // RadioButton
            RootPanel.Resources["RadioButtonForegroundDisabled"] = RootPanel.Resources["RadioButtonForeground"];
            RootPanel.Resources["RadioButtonOuterEllipseFillDisabled"] = RootPanel.Resources["RadioButtonOuterEllipseFill"];
            RootPanel.Resources["RadioButtonOuterEllipseCheckedFillDisabled"] = RootPanel.Resources["RadioButtonOuterEllipseCheckedFill"];
            RootPanel.Resources["RadioButtonCheckGlyphFillDisabled"] = RootPanel.Resources["RadioButtonCheckGlyphFill"];


            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;

                if (control is TextBox textBox)
                {
                    textBox.IsReadOnly = true;
                    textBox.IsSpellCheckEnabled = false;
                    textBox.PlaceholderText = "";
                    //textBox.TextAlignment = TextAlignment.Left;
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    TextBox autoSuggestBoxAsTextBox = FindName($"{autoSuggestBox.Name}TextBox") as TextBox;
                    if (autoSuggestBoxAsTextBox != null)
                    {
                        autoSuggestBox.Visibility = Visibility.Collapsed;

                        autoSuggestBoxAsTextBox.Visibility = Visibility.Visible;
                        autoSuggestBoxAsTextBox.IsReadOnly = true;
                        autoSuggestBoxAsTextBox.IsSpellCheckEnabled = false;
                        autoSuggestBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
                        autoSuggestBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

                        FormField formField = formFields.FirstOrDefault(f => f.ControlName == autoSuggestBox.Name);
                        if (!string.IsNullOrEmpty(formField?.ControlContent))
                        {
                            autoSuggestBoxAsTextBox.Text = formField.ControlContent;
                        }
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    TextBox comboBoxAsTextBox = FindName($"{comboBox.Name}TextBox") as TextBox;
                    if (comboBoxAsTextBox != null)
                    {
                        comboBox.Visibility = Visibility.Collapsed;

                        comboBoxAsTextBox.Visibility = Visibility.Visible;
                        comboBoxAsTextBox.IsReadOnly = true;
                        comboBoxAsTextBox.IsSpellCheckEnabled = false;
                        comboBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
                        comboBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

                        FormField formField = formFields.FirstOrDefault(f => f.ControlName == comboBox.Name);
                        if (!string.IsNullOrEmpty(formField?.ControlContent))
                        {
                            comboBoxAsTextBox.Text = formField.ControlContent;
                        }
                    }
                }
                else if (control is RadioButton radioButton)
                {
                    radioButton.IsEnabled = false;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.IsEnabled = false;
                }
                if (formControl.UserControl is FormHeaderUserControl formHeader)
                {
                    formHeader.LockForm(formFields);
                }
                else if (formControl.UserControl is AutoSuggestTextBoxUserControl autosuggestTextBox)
                {
                    autosuggestTextBox.Messagestate = MessageState.Locked;
                    autosuggestTextBox.FormFields = formFields;
                }
                else if (formControl.UserControl is RadioOperatorUserControl userControl)
                {
                    userControl.Messagestate = MessageState.Locked;
                }
            }
        }

        public virtual string CreateOutpostData(ref PacketMessage packetMessage)
        {
            _outpostData = new List<string>
            {
                "!SCCoPIFO!",
                $"#T: {PacFormName}.html",
                $"#V: {PackItFormVersion}-{PIF}",
            };
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

                if (control is ToggleButtonGroup)
                {
                    string outpostValue = GetOutpostValue(id, ref msgLines);
                    if (!string.IsNullOrEmpty(outpostValue))
                    {
                        foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                        {
                            if (outpostValue == radioButton.Tag as string)
                            {
                                formField.ControlContent = radioButton.Name;
                            }
                            //if ((radioButton.Content as string).Contains(outpostValue))
                            //{
                            //    formField.ControlContent = radioButton.Name;
                            //}
                            //else if (radioButton.Name == outpostValue)
                            //{
                            //    formField.ControlContent = radioButton.Name;
                            //}
                        }
                    }
                }
                else if (control is CheckBox)
                {
                    formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "checked" ? "True" : "False");
                }
                else if (control is ComboBox comboBox)
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
                    if (control is ToggleButtonGroup)
                    {
                        foreach (RadioButton radioButton in ((ToggleButtonGroup)control).RadioButtonGroup)
                        {
                            string radioButtonIndex = GetTagIndex(radioButton);
                            if ((GetOutpostValue(radioButtonIndex, ref msgLines)?.ToLower()) == "true")
                            {
                                formField.ControlContent = radioButton.Name;
                            }
                        }
                    }
                    else if (control is CheckBox)
                    {
                        formField.ControlContent = (GetOutpostValue(id, ref msgLines) == "true" ? "True" : "False");
                    }
                    else if (control is ComboBox)
                    {
                        formField.ControlContent = ConvertComboBoxFromOutpost(id, ref msgLines);
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
                    string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (data.Length == 2)
                    {
                        if (data[1] == (-1).ToString() || string.IsNullOrEmpty(data[1]))
                        {
                            return $"{id}: [ }}0]";
                        }
                        else
                        {
                            return $"{id}: [{data[0]}}}{data[1]}]";
                        }
                    }
                    else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
                    {
                        return $"{id}: [ }}0]";
                    }
                    break;
                case FormProviders.PacItForm:
                    if (!string.IsNullOrEmpty(formField.ControlComboxContent?.Data))
                    {
                        return $"{id}: [{formField.ControlComboxContent.Data}]";
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

            if (control is TextBox)
            {
                if (formProvider == FormProviders.PacForm)
                {
                    if (((TextBox)control).AcceptsReturn)
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
            else if (control is RadioButton)
            {
                if (formProvider == FormProviders.PacForm)
                {

                    if (formField.ControlContent == "True")
                    {
                        return $"{id}: [true]";
                    }
                    else
                    {
                        return "";
                    }
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
            else if (control is ToggleButtonGroup toggleButtonGroup)
            {
                if (formProvider == FormProviders.PacItForm)
                {
                    return $"{id}: [{toggleButtonGroup.GetCheckedRadioButtonOutpostData(formField.ControlContent)}]";
                }
            }
            else if (control is ComboBox)
            {
                return CreateComboBoxOutpostDataString(formField, id);
            }
            return "";
        }

        protected string CreateOutpostMessageBody(List<string> outpostData)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in outpostData)
			{
				sb.Append(s + "\r");
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
                    //if (_formControlsList[i].UserControl == null)
                    //{
                        formField.ControlContent = textBox.Text;
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
                        formField.ControlContent = $"{comboBox.SelectedItem?.ToString()},{comboBox.SelectedIndex}";
                    }
                    else if (FormProvider == FormProviders.PacItForm)
                    {
                        if (comboBox.SelectedItem as ComboBoxPackItItem != null)
                        {
                            ComboBoxPackItItem comboBoxPackItItem = comboBox.SelectedItem as ComboBoxPackItItem;
                            comboBoxPackItItem.SelectedIndex = comboBox.SelectedIndex;
                            formField.ControlComboxContent = comboBoxPackItItem;
                            formField.ControlContent = comboBoxPackItItem?.Item;
                        }
                        else
                        {
                            formField.ControlContent = comboBox.SelectedItem?.ToString();
                        }
                    }
				}
                else if (_formControlsList[i].InputControl is ToggleButtonGroup toggleButtonGroup)
                {
					formField.ControlContent = toggleButtonGroup.GetRadioButtonCheckedState();
				}
                else if (_formControlsList[i].InputControl is CheckBox checkBox)
                {
					formField.ControlContent = checkBox.IsChecked.ToString();
				}
                else if (_formControlsList[i].InputControl is RadioButton radioButton)
                {
                    formField.ControlContent = radioButton.IsChecked.ToString();
                }
                formFields.SetValue(formField, i);
			}
			return formFields;
		}

        protected virtual void FillComboBoxFromFormFields(FormField formField, ComboBox comboBox)
        {
            var data = formField.ControlContent.Split(new char[] { ',' });
            if (data.Length == 2)
            {
                // This is a PacForm ComboBox
                int index = Convert.ToInt32(data[1]);
                if (index < 0 && comboBox.IsEditable)
                {
                    comboBox.Text = data[0];
                    //comboBox.SelectedIndex = index;
                    //comboBox.Visibility = Visibility.Visible;
                }
                else
                {
                    comboBox.SelectedIndex = index;
                    //comboBox.Text = data[0];
                }
            }
            else
            {
                // Received forms neds to have their ControlComboxContent updated
                var items = comboBox.Items;
                var selectedItem = comboBox.Items[0];
                var itemSource = comboBox.ItemsSource;

                if (formField.ControlComboxContent != null)
                {
                    comboBox.SelectedIndex = formField.ControlComboxContent.SelectedIndex;
                    //comboBox.SelectedValue = formField.ControlContent;

                }
                else
                {
                    comboBox.SelectedValue = formField.ControlContent;
                }
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
                    if (formControl.UserControl == null)
                    {
                        // Fields that use Binding requires special handling
                        switch (control.Name)
                        {
                            case "msgDate":
                                MsgDate = textBox.Text;
                                break;
                            case "operatorCallsign":
                                OperatorCallsign = textBox.Text;
                                break;
                            case "operatorName":
                                OperatorName = textBox.Text;
                                break;

                            //case "messageNo":
                            //    OriginMsgNo = textBox.Text;
                            //    break;
                            case null:
                                continue;
                        }
                    }
                    else
                    {
                        if (formControl.UserControl.GetType() == typeof(RadioOperatorUserControl))
                        {
                            RadioOperatorUserControl radioOperatorControl = formControl.UserControl as RadioOperatorUserControl;

                            //var formCtrl = radioOperatorControl.FormControlsList.Find(x => GetTagIndex(x.InputControl) == formField.ControlIndex);
                            //(formCtrl.InputControl as TextBox).Text = textBox.Text;

                            switch (control.Name)
                            {
                                case "operatorCallsign":
                                    radioOperatorControl.OperatorCallsign = textBox.Text;
                                    break;
                                case "operatorName":
                                    radioOperatorControl.OperatorName = textBox.Text;
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
                                    formHeaderControl.OriginMsgNo = textBox.Text;
                                    break;
                                case "destinationMsgNo":
                                    formHeaderControl.DestinationMsgNo = textBox.Text;
                                    break;
                                case "msgDate":
                                    formHeaderControl.MsgDate = textBox.Text;
                                    break;
                                case "msgTime":
                                    formHeaderControl.MsgTime = textBox.Text;
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
                    Run run = new Run();
                    run.Text = formField.ControlContent;
                    // Add the Run to the Paragraph, the Paragraph to the RichTextBlock.
                    paragraph.Inlines.Add(run);
                    richTextBlock.Blocks.Add(paragraph);
                }
                else if (control is AutoSuggestBox autoSuggsetBox)
                {
                    autoSuggsetBox.Text = formField.ControlContent;
                }
                else if (control is ComboBox comboBox)
                {
                    FillComboBoxFromFormFields(formField, comboBox);
                }
                else if (control is ToggleButtonGroup toggleButtonGroup)
                {
                    if (formControl.UserControl == null)
                    {
                        toggleButtonGroup.SetRadioButtonCheckedState(formField.ControlContent);
                    }
                    else if (formControl.UserControl.GetType() == typeof(FormHeaderUserControl))
                    {
                        FormHeaderUserControl formHeaderControl = formControl.UserControl as FormHeaderUserControl;
                        if (control.Name == "handlingOrder")
                            formHeaderControl.HandlingOrder = formField.ControlContent;
                    }
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.IsChecked = formField.ControlContent == "True";
                }
            }
            UpdateFormFieldsRequiredColors(false);
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

        public virtual void MessageChanged(string message)      // Must be public, not protected
        { }

        public virtual void MsgTimeChanged(string msgTime)      // Must be public, not protected
        { }

        protected virtual void Required_Checked(object sender, RoutedEventArgs e)
        {
            UpdateFormFieldsRequiredColors();
        }

        protected virtual void ReportType_Checked(object sender, RoutedEventArgs e)
        {
            bool complete = (bool)(sender as RadioButton).IsChecked && (sender as RadioButton).Name == "complete";
            UpdateRequiredFields(complete);
        }

        protected virtual void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (FormPacketMessage != null && FormPacketMessage.FormFieldArray != null && comboBox.ItemsSource is List<ComboBoxPackItItem>)
            {
                foreach (FormField formField in FormPacketMessage.FormFieldArray)
                {
                    //if (formField.ControlName == comboBox.Name && formField.ControlComboxContent.SelectedIndex < 0)
                    if (formField.ControlName == comboBox.Name)
                    {
                        //comboBox.SelectedValue = formField.ControlContent;
                        int index = 0;
                        foreach (ComboBoxPackItItem packItItem in comboBox.ItemsSource as List<ComboBoxPackItItem>)
                        {
                            if (packItItem.Data == formField.ControlContent)
                            {
                                packItItem.SelectedIndex = index;
                                comboBox.SelectedIndex = index;
                                if (Messagestate == MessageState.Locked)
                                {
                                    TextBox textBox = FindName($"{comboBox.Name}TextBlock")as TextBox;
                                    textBox.Background = packItItem.BackgroundBrush;
                                }
                                break;
                            }
                            index++;
                        }
                        break;
                    }
                }
            }
        }

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

        #region Print
        public abstract Panel CanvasContainer
        { get;  }

        public abstract Panel DirectPrintContainer
        { get;  }

        public abstract List<Panel> PrintPanels
        { get; }

        protected bool printFooterVisibility = false;
        public virtual bool PrintFooterVisibility
        {
            get => printFooterVisibility;
            set => Set(ref printFooterVisibility, value);
        }

        protected void AddFooter()
        {
            //PrintFooterVisibility = true;

            //for (int i = 0; i < _printPanels.Count; i++)
            //{
            //    TextBlock footer = _printPanels[i].FindName("footer") as TextBlock;
            //    if (footer != null)
            //        return;
            //}

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
                    string footerText = $"Page {i + 1} of {_printPanels.Count}, Message Number: {MessageNo}";
                    TextBlock footer = new TextBlock
                    {
                        //Name = "footer", Only one name "footer" allowed
                        Text = footerText,
                        Margin = new Thickness(0, 16, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    if (_printPanels[i].GetType() == typeof(Grid))
                    {
                        Grid grid = _printPanels[i] as Grid;
                        grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        Grid.SetRow(footer, grid.RowDefinitions.Count - 1);
                        grid.Children.Add(footer);

                    }
                    else
                    {
                        _printPanels[i].Children.Add(footer);
                    }
                }
            }
        }

        public virtual async void PrintForm()
        {
            if (CanvasContainer is null || DirectPrintContainer is null)
                return;

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
                        if (child is TextBlock textBlock && textBlock.Text.Contains($"Page {i + 1} of"))
                            _printPanels[i].Children.Remove(child);
                    }

                    DirectPrintContainer.Children.Add(_printPanels[i]);
                }
            }

            //for (int i = 0; i < _printPanels.Count; i++)
            //{
            //    if (_printPanels[i] != null && !DirectPrintContainer.Children.Contains(_printPanels[i]))
            //    {
            //        TextBlock footer = _printPanels[i].FindName("footer") as TextBlock;
            //        if (footer != null)
            //        {
            //            _printPanels[i].Children.Remove(footer);
            //        }

            //        DirectPrintContainer.Children.Add(_printPanels[i]);
            //    }
            //}
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


