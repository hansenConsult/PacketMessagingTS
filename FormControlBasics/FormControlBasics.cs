using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        //// <provider, index> "PacForm" or "PacItForm"
        //// xx|yy,required,text
        //protected Dictionary<string, string> _tagIndexByFormProvider;
        ////protected string[] _formProviders = new string[] { "PacForm", "PacItForm" };
        //protected int _providerIndix = 0;

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
            set => Set(ref _pif, value);
        }


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

        protected void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
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
                        formControl.BaseBorderColor = WhiteBrush;
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
                else if (control is AutoSuggestBox)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    formControl.BaseBorderColor = TextBoxBorderBrush;
                    _formControlsList.Add(formControl);
                }
                else if (control is RadioButton)
                {
                    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                    _formControlsList.Add(formControl);

                    _radioButtonsList.Add((RadioButton)control);
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
        protected string FormatTelephoneNumber(string phoneNumber)
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

        public bool IsFieldRequired(FrameworkElement control)
        {
            string tag = (control.Tag as string)?.ToLower();
            if (!string.IsNullOrEmpty(tag) && tag.Contains("conditionallyrequired"))
                return false;
            else if (!string.IsNullOrEmpty(tag) && tag.Contains("required"))
                return true;
            else
                return false;
        }

        protected virtual void AutoSuggestBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is AutoSuggestBox textBox && textBox.Name == formControl.InputControl.Name)
                {
                    if (IsFieldRequired(sender as Control) && string.IsNullOrEmpty(textBox.Text))
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
                if (IsFieldRequired(sender as TextBox) && !match)
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
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is TextBox textBox && textBox.Name == formControl.InputControl.Name)
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
                            string phonePattern = @"\b\d{3}[-]?\d{3}[-]?\d{4}\s*[xX]?\d*\b";
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
                    string datePattern;
                    if (date.Length == 8)
                    {
                        date = date.Insert(2, "/");
                        date = date.Insert(5, "/");
                    }
                    datePattern = @"^(0[1-9]|1[012])[/](0[1-9]|[12][0-9]|3[01])[/](19|20)\d\d$";//(0[1-9]|1[012])/(0[1-9]|1[0-9]|2[0-9]|3[01])/[1-2][0-9][0-9][0-9]
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
                    if (e.AddedItems[0] is ComboBoxPackItItem comboBoxPackItItem)
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
                    if (IsFieldRequired(comboBox) && comboBox.SelectedIndex < 0)
                    {
                        comboBox.BorderBrush = formControl.RequiredBorderBrush;
                        comboBox.BorderThickness = new Thickness(2);
                    }
                    else
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
                        toggleButtonGroup.CheckedControlName = radioButton.Name;
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
                    }
                    break;
                }
            }
            EventHandler<FormEventArgs> OnSubjectChange = EventSubjectChanged;
            FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = MessageNo };
            OnSubjectChange?.Invoke(this, formEventArgs);
        }

        //protected virtual void TextBoxFromICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //    sender.Text = args.SelectedItem as string;
        //}

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
