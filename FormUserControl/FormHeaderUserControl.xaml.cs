using System;
using System.Linq;

using FormControlBaseMvvmNameSpace;

using FormControlBasicsNamespace;

using SharedCode;
using SharedCode.Models;

using ToggleButtonGroupControl;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public sealed partial class FormHeaderUserControl : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventMsgTimeChanged;


        public override FormControlBaseMvvm RootPanel => rootPanel;
     

        public FormHeaderUserControl()
        {
            InitializeComponent();

            ScanControls(formHeaderUserControl);

            InitializeToggleButtonGroups();

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
                    case ToggleButtonGroup _:
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
                    case RadioButton _:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        _formControlsList.Add(formControl);

                        _radioButtonsList.Add((RadioButton)control);
                        break;
                    case AutoSuggestTextBoxUserControl _:
                        ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                        break;
                }

                //if (control is StackPanel || control is Grid || control is Border || control is RelativePanel)
                //{
                //    ScanControls(control, formUserControl);
                //}
                //else if (control is TextBox textBox)
                //{
                //    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                //    if (textBox.IsReadOnly)
                //    {
                //        formControl.BaseBorderColor = WhiteBrush;
                //    }
                //    else
                //    {
                //        formControl.BaseBorderColor = textBox.BorderBrush;
                //    }
                //    _formControlsList.Add(formControl);
                //}
                //else if (control is ComboBox comboBox)
                //{
                //    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                //    formControl.BaseBorderColor = comboBox.BorderBrush;
                //    _formControlsList.Add(formControl);
                //}
                //else if (control is CheckBox || control is ToggleButtonGroup || control is RichTextBlock)
                //{
                //    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                //    _formControlsList.Add(formControl);
                //}
                //else if (control is AutoSuggestBox)
                //{
                //    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                //    formControl.BaseBorderColor = TextBoxBorderBrush;
                //    _formControlsList.Add(formControl);
                //}
                //else if (control is RadioButton)
                //{
                //    FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                //    _formControlsList.Add(formControl);

                //    _radioButtonsList.Add((RadioButton)control);
                //}
                //else if (control is AutoSuggestTextBoxUserControl)
                //{
                //    ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                //}
                //else if (control is FormHeaderUserControl)
                //{
                //    ScanControls((control as FormHeaderUserControl).Panel, control as FrameworkElement);
                //}
                //else if (control is RadioOperatorUserControl)
                //{
                //    ScanControls((control as RadioOperatorUserControl).Panel, control as FrameworkElement);
                //}
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

        public DependencyObject Panel => formHeaderUserControl;

        private bool namePanel1Visibility = true;
        public bool NamePanel1Visibility
        { 
            get => namePanel1Visibility; 
            set => SetProperty(ref namePanel1Visibility, value);
        }

        public bool NamePanel2Visibility => !NamePanel1Visibility;

        private string headerString1;
        public string HeaderString1
        {
            get => headerString1;
            set => SetProperty(ref headerString1, value);
        }

        private string headerString2;
        public string HeaderString2
        {
            get => headerString2;
            set => SetProperty(ref headerString2, $" {value}");
        }

        private string headerSubstring;
        public string HeaderSubstring
        {
            get => headerSubstring;
            set => SetProperty(ref headerSubstring, value);
        }

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

    }
}
