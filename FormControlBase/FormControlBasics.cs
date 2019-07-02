using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using SharedCode;

using ToggleButtonGroupControl;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FormControlBaseClass
{
    public class FormControlBasics : UserControl, INotifyPropertyChanged
    {
        public static SolidColorBrush _redBrush = new SolidColorBrush(Colors.Red);
        public static SolidColorBrush _whiteBrush = new SolidColorBrush(Colors.White);
        public static SolidColorBrush _blackBrush = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush _lightSalmonBrush = new SolidColorBrush(Colors.LightSalmon);

        protected List<FormControl> _formControlsList = new List<FormControl>();
        protected List<RadioButton> _radioButtonsList = new List<RadioButton>();

        protected string _validationResultMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        //// <provider, index> "PacForm" or "PacItForm"
        //// xx|yy,required,text
        //protected Dictionary<string, string> _tagIndexByFormProvider;
        ////protected string[] _formProviders = new string[] { "PacForm", "PacItForm" };
        //protected int _providerIndix = 0;

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

        public void ScanControls(DependencyObject panelName)
        {
            int count = VisualTreeHelper.GetChildrenCount(panelName);

            for (int i = 0; i < count; i++)
            {
                DependencyObject control = VisualTreeHelper.GetChild(panelName, i);

                if (control is StackPanel || control is Grid || control is Border || control is RelativePanel)
                {
                    ScanControls(control);
                }
                else if (control is TextBox || control is AutoSuggestBox || control is ComboBox
                                            || control is CheckBox || control is ToggleButtonGroup)
                {
                    FormControl formControl = new FormControl((Control)control);
                    _formControlsList.Add(formControl);
                }
                else if (control is RadioButton)
                {
                    FormControl formControl = new FormControl((Control)control);
                    _formControlsList.Add(formControl);

                    _radioButtonsList.Add((RadioButton)control);
                }
            }
        }

        protected void AddToErrorString(string errorText)
        {
            _validationResultMessage += ($"\n{errorText}");
        }

        public static string GetTagErrorMessage(Control control)
        {
            string tag = control.Tag as string;
            if (string.IsNullOrEmpty(tag))
                return "";

            string[] tags = tag.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tags.Length == 2 && tags[0] == "required")
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

        public virtual string ValidateForm(string errorText = "")
        {
            _validationResultMessage = errorText;
            foreach (FormControl formControl in _formControlsList)
            {
                Control control = formControl.InputControl;
                string tag = control.Tag as string;
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
                            AddToErrorString(GetTagErrorMessage(control));
                            control.BorderBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            control.BorderBrush = formControl.BaseBorderColor;
                        }
                    }
					else if (control is AutoSuggestBox autoSuggestBox)
					{
						if (autoSuggestBox.Text.Length == 0)
						{
							AddToErrorString(GetTagErrorMessage(control));
							control.BorderBrush = formControl.RequiredBorderBrush;
						}
						else
						{
							control.BorderBrush = formControl.BaseBorderColor;
						}
					}
					else if (control is ComboBox comboBox)
                    {
                        if (string.IsNullOrEmpty(comboBox.SelectionBoxItem?.ToString()))
                        {
                            AddToErrorString(GetTagErrorMessage(control));
                            control.BorderBrush = formControl.RequiredBorderBrush;
                        }
                        else
                        {
                            control.BorderBrush = formControl.BaseBorderColor;
                        }
                    }
                    else if (control is ToggleButtonGroup toggleButtonGroup)
                    {
                        if (!toggleButtonGroup.Validate())
                        {
                            AddToErrorString(GetTagErrorMessage(control));
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

        public bool IsFieldRequired(Control control)
        {
            string tag = (control.Tag as string)?.ToLower();
            if (!string.IsNullOrEmpty(tag) && tag.Contains("conditionallyrequired"))
                return false;
            else if (!string.IsNullOrEmpty(tag) && tag.Contains("required"))
                return true;
            else
                return false;
        }

        protected virtual void TextBoxRequired_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is TextBox textBox && textBox.Name == formControl.InputControl.Name)
                {
                    if (string.IsNullOrEmpty(textBox.Text) && IsFieldRequired(sender as TextBox))
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

        protected virtual void ComboBoxRequired_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is ComboBox comboBox && comboBox.Name == formControl.InputControl.Name)
                {
                    if (comboBox.SelectedIndex < 0 && string.IsNullOrEmpty(comboBox.Text))
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
        }

        protected virtual void RadioButtonRequired_SelectionChanged(object sender, RoutedEventArgs e)
        {
            foreach (FormControl formControl in _formControlsList)
            {
                if (sender is RadioButton radioButton)
                {
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
                    }
                }
            }
        }

    }
}
