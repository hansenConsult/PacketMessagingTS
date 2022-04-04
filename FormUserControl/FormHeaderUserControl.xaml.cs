using System;
using System.Collections.Generic;
using System.Linq;

using FormControlBaseMvvmNameSpace;

using FormControlBasicsNamespace;

using FormUserControl;

using Microsoft.UI.Xaml.Controls;

using SharedCode;
using SharedCode.Models;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public sealed partial class FormHeaderUserControl : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventMsgTimeChanged;

        public FormHeaderUserControlViewModel ViewModel = new FormHeaderUserControlViewModel();

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public List<ComboBoxItem> ToICSPositionComboBoxItems;

        public FormHeaderUserControl()
        {
            InitializeComponent();

            ScanControls(formHeaderUserControl);

            ViewModel.HandlingOrder = null;

            ViewModelBase = ViewModel;
            //UpdateFormFieldsRequiredColors();
        }

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
                        FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                        if (textBox.IsReadOnly)
                        {
                            formControl.BaseBorderColor = WhiteBrush;
                        }
                        else
                        {
                            textBox.BorderBrush = RootPanel.Resources["TextControlBorderBrush"] as Brush;
                            //textBox.BorderThickness = (Thickness)RootPanel.Resources["TextControlBorderThemeThickness"];
                            //textBox.BorderThickness = RootPanel.Resources["TextControlBorderThemeThickness"] as Thickness;
                            //formControl.BaseBorderColor = textBox.BorderBrush;
                            CornerRadius cornerRadius = textBox.CornerRadius;
                        }
                        _formControlsList.Add(formControl);
                        break;
                    case ComboBox comboBox:
                        formControl = new FormControl((FrameworkElement)control, formUserControl)
                        {
                            BaseBorderColor = comboBox.BorderBrush
                        };
                        _formControlsList.Add(formControl);
                        break;
                    case CheckBox _:
                    case RadioButtons _:
                    case RichTextBlock _:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        _formControlsList.Add(formControl);
                        break;
                    case AutoSuggestBox autoSuggestBox:
                        formControl = new FormControl((FrameworkElement)control, formUserControl)
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
                    case AutoSuggestTextBoxUserControl _:
                        ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                        break;
                }
            }
        }

        public new void LockForm()
        {
            //base.LockForm();

            foreach (FormControl formControl in _formControlsList)
            {
                FrameworkElement control = formControl.InputControl;

                if (control is TextBox textBox)
                {
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
                        if (!string.IsNullOrEmpty(formField?.ControlContent))
                        {
                            autoSuggestBoxAsTextBox.Text = formField.ControlContent;
                        }
                    }
                }
                else if (formControl.UserControl is AutoSuggestTextBoxUserControl autosuggestTextBox)
                {
                    autosuggestTextBox.FormPacketMessage = FormPacketMessage;
                }
            }
        }

        public void SelectToICSPositionAsComboBox(bool selectComboBox)
        {
            if (selectComboBox)
            {
                autoSuggestBoxToICSPosition.Visibility = Visibility.Collapsed;
                comboBoxToICSPosition.Visibility = Visibility.Visible;
            }
            else
            {
                autoSuggestBoxToICSPosition.Visibility = Visibility.Visible;
                comboBoxToICSPosition.Visibility = Visibility.Collapsed;
            }
        }

        public void SetHandlingOrder(int index)
        {
            if (index < 0 || index > 2)
            {
                Exception exception = new Exception("Wrong parameter");
            }
            handlingOrder.SelectedIndex = index;
        }

        public void SetToLocation(string tolocation)
        {
            toLocation.SetText(tolocation);
        }

        public void SetToICSPosition(string toicsPosition)
        {
            autoSuggestBoxToICSPosition.Text = toicsPosition;
        }

        public void SetToICSPosition(List<ComboBoxItem> toICSPositionComboBoxItems)
        {
            ToICSPositionComboBoxItems = toICSPositionComboBoxItems;
            SelectToICSPositionAsComboBox(true);
        }

        public DependencyObject Panel => formHeaderUserControl;

        //private bool namePanel1Visibility = true;
        //public bool NamePanel1Visibility
        //{ 
        //    get => namePanel1Visibility; 
        //    set => SetProperty(ref namePanel1Visibility, value);
        //}

        //public bool NamePanel2Visibility => !NamePanel1Visibility;

        //private string headerString1;
        //public string HeaderString1
        //{
        //    get => headerString1;
        //    set => SetProperty(ref headerString1, value);
        //}

        //private string headerString2;
        //public string HeaderString2
        //{
        //    get => headerString2;
        //    set => SetProperty(ref headerString2, $" {value}");
        //}

        //private string headerSubstring;
        //public string HeaderSubstring
        //{
        //    get => headerSubstring;
        //    set => SetProperty(ref headerSubstring, value);
        //}

        public void TextBox_MsgTimeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);

                if (formControl == null || !CheckTimeFormat(formControl))
                {
                    return;
                }

                // Create event time changed
                //EventHandler<FormEventArgs> OnMsgTimeChange = EventMsgTimeChanged;
                FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = textBox.Text };
                EventMsgTimeChanged?.Invoke(this, formEventArgs);
            }
        }

        //private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    RadioButtons radioButtons = sender as RadioButtons;
        //    int count = e.AddedItems.Count;
        //    var item = e.AddedItems[0];

        //    foreach (RadioButton radioButton in radioButtons.Items)
        //    {
        //        if (IsFieldRequired(radioButtons) && radioButtons.SelectedIndex == -1)
        //        {
        //            radioButton.Foreground = new SolidColorBrush(Colors.Red);
        //        }
        //        else
        //        {
        //            radioButton.Foreground = new SolidColorBrush(Colors.Black);
        //        }
        //    }
        //    if (radioButtons.Name == "handlingOrder")
        //    {
        //        Subject_Changed(sender, null);
        //    }
        //}

    }
}
