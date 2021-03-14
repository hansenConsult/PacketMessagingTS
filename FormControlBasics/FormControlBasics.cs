using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using SharedCode;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FormControlBasicsNamespace
{
    public sealed class FormEventArgs : EventArgs
    {
        //public FormEventArgs() { }

        //public FormEventArgs(string tacticalCallsign)
        //{
        //	TacticalCallsign = tacticalCallsign;
        //}

        //public string TacticalCallsign
        //{ get; set; }

        public string SubjectLine
        { get; set; }
    }


    public partial class FormControlBasics : UserControl, INotifyPropertyChanged
    {
        public event EventHandler<FormEventArgs> EventSubjectChanged;

        public static SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush WhiteBrush = new SolidColorBrush(Colors.White);
        public static SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush TextBoxBorderBrush = new SolidColorBrush(Color.FromArgb(66, 0, 0, 0));
        public static SolidColorBrush LightSalmonBrush = new SolidColorBrush(Colors.LightSalmon);
        public static SolidColorBrush LightGreenBrush = new SolidColorBrush(Colors.LightGreen);
        public static SolidColorBrush PinkBrush = new SolidColorBrush(Colors.Pink);
        public static SolidColorBrush LightGrayBrush = new SolidColorBrush(Colors.LightGray);
        public static SolidColorBrush GainsboroBrush = new SolidColorBrush(Colors.Gainsboro);
        public static SolidColorBrush YellowBrush = new SolidColorBrush(Colors.Yellow);
        public static SolidColorBrush OrangeBrush = new SolidColorBrush(Colors.Orange);

        protected string[] ICSPosition = new string[] {
                "Incident Commander",
                "Operations",
                "Planning",
                "Logistics",
                "Finance",
                "Public Info. Officer",
                "Liaison Officer",
                "Safety Officer"
        };

        protected List<FormControl> _formControlsList = new List<FormControl>();
        protected List<RadioButton> _radioButtonsList = new List<RadioButton>();

        protected List<string> _ICSPositionFiltered = new List<string>();

        protected string _validationResultMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public PacketMessage FormPacketMessage
        { get; set; }

        //public FormField[] FormFields
        //{ get; set; }

        public virtual FormControlBasics RootPanel
        { get; set; }

        public virtual string MessageNo
        { get; set; }

        public virtual string DestinationMsgNo
        { get; set; }

        public virtual string OriginMsgNo
        { get; set; }

        protected string _msgDate;
        public virtual string MsgDate
        { get; set; }

        public virtual string MsgTime
        { get; set; }

        public virtual string HandlingOrder
        { get; set; }

        public virtual string OperatorName
        { get; set; }

        public virtual string OperatorCallsign
        { get; set; }

        protected string _pif = "2.1";
        public virtual string PIF
        {
            get => _pif;
            set => Set(ref _pif, value);
        }

        //private string headerPIF;
        public string HeaderPIF
        {
            get => $"PIF: {_pif}";
            //set => Set(ref _pif, value);
        }

        //public MessageState Messagestate
        //{
        //    get;
        //    set;
        //}

        //public virtual void UpdateStyles()
        //{
        //}

        //public FormControlBasics()
        //{
        //    //Messagestate = MessageState.None;
        //}

        protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName) =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
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
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                    {
                        //if (textBox.IsReadOnly)
                        //{
                        //    formControl.BaseBorderColor = WhiteBrush;
                        //}
                        //else
                        //{
                        BaseBorderColor = textBox.BorderBrush
                    };
                    //}
                    _formControlsList.Add(formControl);
                }
                else if (control is ComboBox comboBox)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                    {
                        BaseBorderColor = comboBox.BorderBrush
                    };
                    _formControlsList.Add(formControl);
                }
                else if (control is CheckBox || control is ToggleButtonGroup || control is RichTextBlock)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);
                }
                else if (control is AutoSuggestBox)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                    {
                        BaseBorderColor = TextBoxBorderBrush
                    };
                    _formControlsList.Add(formControl);
                }
                else if (control is RadioButton)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);

                    _radioButtonsList.Add((RadioButton)control);
                }
                else if (control is ToggleSwitch)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);
                }
            }
        }

        public virtual void LockForm()
        {
            if (FormPacketMessage.MessageState != MessageState.Locked)
                return;

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

            //ToggleSwitch
            RootPanel.Resources["ToggleSwitchContainerBackgroundDisabled"] = RootPanel.Resources["ToggleSwitchContainerBackground"];
            RootPanel.Resources["ToggleSwitchContentForegroundDisabled"] = RootPanel.Resources["ToggleSwitchContentForeground"];
            RootPanel.Resources["ToggleSwitchFillOffDisabled"] = RootPanel.Resources["ToggleSwitchFillOff"];
            RootPanel.Resources["ToggleSwitchFillOnDisabled"] = RootPanel.Resources["ToggleSwitchFillOn"];
            RootPanel.Resources["ToggleSwitchKnobFillOffDisabled"] = RootPanel.Resources["ToggleSwitchKnobFillOff"];
            RootPanel.Resources["ToggleSwitchKnobFillOnDisabled"] = RootPanel.Resources["ToggleSwitchKnobFillOn"];


            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;

                if (control is TextBox textBox)
                {
                    textBox.IsReadOnly = true;
                    textBox.IsSpellCheckEnabled = false;
                    textBox.PlaceholderText = "";
                    textBox.BorderBrush = WhiteBrush;
                }
                else if (control is AutoSuggestBox autoSuggestBox)
                {
                    if (FindName($"{autoSuggestBox.Name}TextBox") is TextBox autoSuggestBoxAsTextBox)
                    {
                        autoSuggestBox.Visibility = Visibility.Collapsed;

                        autoSuggestBoxAsTextBox.Visibility = Visibility.Visible;
                        autoSuggestBoxAsTextBox.IsReadOnly = true;
                        autoSuggestBoxAsTextBox.IsSpellCheckEnabled = false;
                        autoSuggestBoxAsTextBox.PlaceholderText = "";
                        autoSuggestBoxAsTextBox.BorderBrush = WhiteBrush;
                        autoSuggestBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
                        autoSuggestBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

                        FormField formField = FormPacketMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == autoSuggestBox.Name);
                        if (formField != null && formField.ControlContent != null)
                        {
                            autoSuggestBoxAsTextBox.Text = formField.ControlContent;
                        }
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (FindName($"{comboBox.Name}TextBox") is TextBox comboBoxAsTextBox)
                    {
                        comboBox.Visibility = Visibility.Collapsed;

                        comboBoxAsTextBox.Visibility = Visibility.Visible;
                        comboBoxAsTextBox.IsReadOnly = true;
                        comboBoxAsTextBox.IsSpellCheckEnabled = false;
                        comboBoxAsTextBox.VerticalAlignment = VerticalAlignment.Center;
                        comboBoxAsTextBox.HorizontalAlignment = HorizontalAlignment.Left;

                        FormField formField = FormPacketMessage.FormFieldArray.FirstOrDefault(f => f.ControlName == comboBox.Name);
                        if (formField != null && formField.ControlContent != null)
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
                else if (control is ToggleSwitch toggleSwitch)
                {
                    toggleSwitch.IsEnabled = false;
                }
            }
        }

        protected void AddToErrorString(string errorText)
        {
            _validationResultMessage += ($"\n{errorText}");
        }

        public static string GetTagIndex(FrameworkElement control)
        {
            try
            {
                string tag = (string)control.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(new char[] { ',' });
                    if (!tags[0].Contains("required"))
                    {
                        return tags[0];
                    }
                }
            }
            catch
            {
            }
            return "";
        }

        public static string GetTagErrorMessage(Control control)
        {
            string tag = control.Tag as string;
            if (string.IsNullOrEmpty(tag))
                return "";

            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tags.Length == 2 && (tags[0] == "required" || tags[0] == "conditionallyrequired"))
            {
                return tags[1];
            }
            else if (tags.Length == 3)
            {
                return tags[2];
            }
            else
            {
                return "";
            }
        }

        public void InitializeToggleButtonGroups()
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup)
                {
                    toggleButtonGroup.Initialize(_radioButtonsList, formControl.InputControl.Name);
                }
            }
        }

        public virtual string ValidateForm(string errorText = "")
        {
            _validationResultMessage = errorText;
            foreach (FormControl formControl in _formControlsList)
            {
                // Validation is only done on Control's that are not RichTextBlock
                Control control = formControl.InputControl as Control;
                string tag = control?.Tag as string;
                if (!string.IsNullOrEmpty(tag) && control.IsEnabled && tag.Contains("conditionallyrequired"))
                {
                    control.BorderBrush = formControl.BaseBorderColor;
                    continue;
                }

                if (!string.IsNullOrEmpty(tag) && control.IsEnabled && control.Visibility == Visibility.Visible && tag.Contains("required"))
                {
                    if (control is TextBox textBox)
                    {
                        if (textBox.Text.Length == 0)
                        {
                            AddToErrorString(GetTagErrorMessage(textBox));
                            textBox.BorderBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            textBox.BorderBrush = formControl.BaseBorderColor;
                        }
                    }
					else if (control is AutoSuggestBox autoSuggestBox)
					{
						if (autoSuggestBox.Text.Length == 0)
						{
							AddToErrorString(GetTagErrorMessage(autoSuggestBox));
                            autoSuggestBox.BorderBrush = formControl.RequiredBorderBrush;
						}
						else
						{
                            autoSuggestBox.BorderBrush = formControl.BaseBorderColor;
						}
					}
					else if (control is ComboBox comboBox)
                    {
                        if (string.IsNullOrEmpty(comboBox.SelectionBoxItem?.ToString()))
                        {
                            AddToErrorString(GetTagErrorMessage(comboBox));
                            comboBox.BorderBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            comboBox.BorderBrush = formControl.BaseBorderColor;
                        }
                    }
                    else if (control is ToggleButtonGroup toggleButtonGroup)
                    {
                        if (!toggleButtonGroup.Validate())
                        {
                            AddToErrorString(GetTagErrorMessage(toggleButtonGroup));
                        }
                    }
                }
            }
            return _validationResultMessage;
        }

        // Formats a 10 digit number to (123) 456-7890
        protected static string FormatTelephoneNumber(string phoneNumber)
        {
            char[] phoneNumberArray = phoneNumber.ToCharArray();
            char[] digitArray = new char[16];
            int j = 0;
            for (int i = 0; i < phoneNumberArray.Length; i++)
            {
                if (char.IsDigit(phoneNumberArray[i]))
                {
                    digitArray[j++] = phoneNumberArray[i];
                }
            }
            if (j == 10)
            {
                return $"({digitArray[0]}{digitArray[1]}{digitArray[2]}) {digitArray[3]}{digitArray[4]}{digitArray[5]}-{digitArray[6]}{digitArray[7]}{digitArray[8]}{digitArray[9]}";
            }
            else
            {
                return phoneNumber;
            }
        }

        protected virtual void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string phoneNumberString = (sender as TextBox).Text;
            string formattedPhoneNumber = FormatTelephoneNumber(phoneNumberString);
            if (formattedPhoneNumber != phoneNumberString)
            {
                (sender as TextBox).Text = formattedPhoneNumber;
            }
        }

        public static bool IsFieldRequired(FrameworkElement control)
        {
            string tag = (control.Tag as string)?.ToLower();
            if (!string.IsNullOrEmpty(tag))
            {                
                if (tag.Contains("conditionallyrequired"))
                    return false;
                else if (tag.Contains("required"))
                    return true;
            }
            return false;
        }

        protected virtual void AutoSuggestBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            FormControl formControl = _formControlsList.FirstOrDefault(
                                        control => control.InputControl.Name == autoSuggestBox.Name);
            if (formControl != null)
            {
                if (IsFieldRequired(sender as Control) && string.IsNullOrEmpty(autoSuggestBox.Text))
                {
                    //autoSuggestBox.BorderThickness = new Thickness(2);
                    autoSuggestBox.BorderBrush = formControl.RequiredBorderBrush;
                }
                else
                {
                    //autoSuggestBox.BorderThickness = new Thickness(1);
                    autoSuggestBox.BorderBrush = formControl.BaseBorderColor;
                }
            }
        }

        protected virtual void TextBox_IntegerChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl;
                if (!string.IsNullOrEmpty(textBox.Name))
                    formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);
                else
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(textBox));

                string pattern = @"\b[0-9]+\b";
                bool match = Regex.IsMatch(textBox.Text, pattern);
                if ((IsFieldRequired(textBox) && (string.IsNullOrEmpty(textBox.Text) || !match))
                    || (!IsFieldRequired(textBox) && !string.IsNullOrEmpty(textBox.Text) && !match))
                {
                    textBox.BorderThickness = new Thickness(2);
                    textBox.BorderBrush = formControl.RequiredBorderBrush;
                }
                else
                {
                    textBox.BorderThickness = new Thickness(1);
                    textBox.BorderBrush = formControl.BaseBorderColor;
                }
            }
        }

        protected virtual void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            FormControl formControl = _formControlsList.FirstOrDefault(
                            control => control.InputControl.Name == textBox.Name);

            if (IsFieldRequired(textBox) && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BorderThickness = new Thickness(2);
                textBox.BorderBrush = formControl.RequiredBorderBrush;
            }
            else
            {
                textBox.BorderThickness = new Thickness(1);
                textBox.BorderBrush = formControl.BaseBorderColor;
            }
        }

        protected virtual void TextBox_PhoneChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                foreach (FormControl formControl in _formControlsList)
                {
                    if (textBox.Name == formControl.InputControl.Name)
                    {
                        string phoneNumber = textBox.Text;
                        bool match = false;
                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            string phonePattern = @"\b\d*\s*[ -]?\d{3}[-]?\d{3}[-]?\d{4}\s*[xX]?\d*\b";
                            match = Regex.IsMatch(phoneNumber, phonePattern);
                        }

                        //if (!match || (IsFieldRequired(textBox) && !match))
                        if (match && IsFieldRequired(textBox) || !IsFieldRequired(textBox))
                        {
                            textBox.BorderThickness = new Thickness(1);
                            textBox.BorderBrush = formControl.BaseBorderColor;
                        }
                        else
                        {
                            textBox.BorderThickness = new Thickness(2);
                            textBox.BorderBrush = formControl.RequiredBorderBrush;
                        }
                        break;
                    }
                }
            }
        }

        protected virtual void TextBox_DateChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl;
                if (!string.IsNullOrEmpty(textBox.Name))
                    formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);
                else
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(textBox));

                string date = textBox.Text.Trim();
                bool match = false;
                if (!string.IsNullOrEmpty(date))
                {
                    string datePattern = @"^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/](19|20)\d\d$";//(0[1-9]|1[012])/(0[1-9]|1[0-9]|2[0-9]|3[01])/[1-2][0-9][0-9][0-9]
                    match = Regex.IsMatch(date, datePattern);
                }
                if (match && IsFieldRequired(textBox) || !IsFieldRequired(textBox))
                {
                    textBox.Text = date;
                    textBox.BorderThickness = new Thickness(1);
                    textBox.BorderBrush = formControl.BaseBorderColor;
                }
                else
                {
                    textBox.BorderThickness = new Thickness(2);
                    textBox.BorderBrush = formControl.RequiredBorderBrush;
                }
            }
        }

        public bool CheckTimeFormat(FormControl formControl)
        {
            TextBox textBox = formControl.InputControl as TextBox;
            string time = textBox.Text;
            bool match = false;
            if (!string.IsNullOrEmpty(time))
            {
                string timePattern = @"^((0[0-9]|1[0-9]|2[0-3]):?[0-5][0-9])|24:?00$";
                match = Regex.IsMatch(time, timePattern);
            }

            if (match && IsFieldRequired(textBox) || !IsFieldRequired(textBox))
            {
                textBox.BorderThickness = new Thickness(1);
                textBox.BorderBrush = formControl.BaseBorderColor;
                if (!string.IsNullOrEmpty(time) && time.Length == 4 && time[2] != ':')
                {
                    textBox.Text = time.Insert(2, ":");
                }
                return true;
            }
            else
            {
                textBox.BorderThickness = new Thickness(2);
                textBox.BorderBrush = formControl.RequiredBorderBrush;
                return false;
            }
        }

        protected virtual void TextBox_TimeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl;
                if (!string.IsNullOrEmpty(textBox.Name))
                    formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);
                else
                    formControl = _formControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(textBox));

                CheckTimeFormat(formControl);
            }
        }

        protected virtual void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e is null)
                return;

            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is ComboBox comboBox && comboBox.Name == formControl.InputControl.Name)
                {
                    if (e.AddedItems.Count == 0)
                    {
                        break;
                    }
                    string selection;
                    if (e.AddedItems[0] is ComboBoxItem comboBoxItem)
                    {
                        selection = comboBoxItem.Content as string;
                        comboBox.Background = comboBoxItem.Background;
                        comboBox.Foreground = comboBoxItem.Foreground;
                    }
                    else if (e.AddedItems[0] is ComboBoxPackItItem comboBoxPackItItem)
                    {
                        selection = comboBoxPackItItem.Item;
                        comboBoxPackItItem.SelectedIndex = comboBox.SelectedIndex;
                        comboBox.Background = comboBoxPackItItem.BackgroundBrush;
                        comboBox.Foreground = comboBoxPackItItem.ForegroundBrush;
                    }
                    else
                    {
                        //comboBox.SelectedValue = e.AddedItems[0].ToString();
                        selection = e.AddedItems[0].ToString();
                    }
                    if (string.IsNullOrEmpty(selection))
                    {
                        comboBox.SelectedIndex = -1;
                    }
                    //if (IsFieldRequired(comboBox) && comboBox.SelectedIndex < 0)
                    if ((IsFieldRequired(comboBox) && comboBox.SelectedIndex < 0 && !comboBox.IsEditable) ||
                        (IsFieldRequired(comboBox) && string.IsNullOrEmpty(selection) && comboBox.IsEditable))
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                        comboBox.BorderThickness = new Thickness(2);
                    }
                    else //if (Messagestate != MessageState.Locked)
                    {
                        comboBox.BorderBrush = formControl.BaseBorderColor;
                        comboBox.BorderThickness = new Thickness(1);
                    }
                    break;
                }
            }
        }

        protected virtual void RadioButton_SelectionChanged(object sender, RoutedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is RadioButton radioButton)
                {
                    if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup && toggleButtonGroup.Name == radioButton.GroupName)
                    {
                        toggleButtonGroup.CheckedControlName = radioButton.Name;
                        if (IsFieldRequired(toggleButtonGroup) && string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()))
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
        }

        protected void Subject_Changed(object sender, RoutedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is RadioButton radioButton)
                {
                    //if (radioButton.Name == "emergency") No longer allowed
                    //{
                    //    HandlingOrder = "immediate";
                    //}
                    if (formControl.InputControl is ToggleButtonGroup toggleButtonGroup && toggleButtonGroup.Name == radioButton.GroupName)
                    {
                        //toggleButtonGroup.CheckedControlName = radioButton.Name;
                        if (toggleButtonGroup.Name == "handlingOrder")
                        {
                            HandlingOrder = radioButton.Name;
                        }
                        if (string.IsNullOrEmpty(toggleButtonGroup.GetRadioButtonCheckedState()))
                        {
                            toggleButtonGroup.ToggleButtonGroupBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            toggleButtonGroup.ToggleButtonGroupBrush = new SolidColorBrush(Colors.Black);
                        }
                        break;
                    }
                }
                else if (sender is TextBox textBox && textBox.Name == formControl.InputControl.Name)
                {
                    if (IsFieldRequired(sender as TextBox) && string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.BorderThickness = new Thickness(2);
                        textBox.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        textBox.BorderThickness = new Thickness(1);
                        textBox.BorderBrush = formControl.BaseBorderColor;
                    }
                    break;
                }
                else if (sender is ComboBox comboBox && comboBox.Name == formControl.InputControl.Name)
                {
                    if (comboBox.SelectedIndex < 0)
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                    }
                    else
                    {
                        comboBox.BorderBrush = formControl.BaseBorderColor;
                        //comboBox.BorderThickness = new Thickness(1);
                    }
                    break;
                }
            }
            EventHandler<FormEventArgs> OnSubjectChange = EventSubjectChanged;
            FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = MessageNo };
            OnSubjectChange?.Invoke(this, formEventArgs);
        }

        protected virtual void TextBoxICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = args.SelectedItem as string;
        }

        protected virtual void AutoSuggestBoxICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = null;
                _ICSPositionFiltered = new List<string>();
                foreach (string s in ICSPosition)
                {
                    string lowerS = s.ToLower();
                    if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
                    {
                        _ICSPositionFiltered.Add(s);
                    }
                }
                sender.ItemsSource = _ICSPositionFiltered;
            }
            AutoSuggestBox_TextChanged(sender, null);
        }

    }
}
