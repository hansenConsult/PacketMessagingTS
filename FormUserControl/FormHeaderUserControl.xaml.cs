using System;
using System.Collections.Generic;

using FormControlBasicsNamespace;

using ToggleButtonGroupControl;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    //public class ListItemData
    //{
    //    public string Text { get; set; }
    //    public ICommand Command { get; set; }
    //}


    public sealed partial class FormHeaderUserControl : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventMsgTimeChanged;

        //// For special A box
        //readonly ObservableCollection<ListItemData> collection = new ObservableCollection<ListItemData>();

        //readonly StandardUICommand _deleteCommand;
        //string _matchedText = "";


        //private List<string> _PreviousTexts;
        //public List<string> PreviousTexts
        //{
        //    get
        //    {
        //        if (_PreviousTexts is null)
        //        {
        //            _PreviousTexts = new List<string>();
        //        }
        //        return _PreviousTexts;
        //    }
        //    set => _PreviousTexts = value;
        //}

        ////  End for 

        public FormHeaderUserControl()
        {
            InitializeComponent();

            ScanControls(formHeaderUserControl);

            InitializeToggleButtonGroups();

            //_deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
            //_deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;
        }

        protected override void ScanControls(DependencyObject panelName, FrameworkElement formUserControl = null)
        {
            int count = VisualTreeHelper.GetChildrenCount(panelName);

            for (int i = 0; i < count; i++)
            {
                DependencyObject control = VisualTreeHelper.GetChild(panelName, i);

                switch (control)
                {
                    case StackPanel s:
                    case Grid g:
                    case Border b:
                    case RelativePanel r:
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
                            formControl.BaseBorderColor = textBox.BorderBrush;
                        }
                        _formControlsList.Add(formControl);
                        break;
                    case ComboBox comboBox:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        formControl.BaseBorderColor = comboBox.BorderBrush;
                        _formControlsList.Add(formControl);
                        break;
                    case CheckBox checkBox:
                    case ToggleButtonGroup toggleButton:
                    case RichTextBlock textBlock:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        _formControlsList.Add(formControl);
                        break;
                    case AutoSuggestBox autoSuggestBox:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        formControl.BaseBorderColor = TextBoxBorderBrush;
                        if (formControl.UserControl is AutoSuggestTextBoxUserControl)
                        {
                            autoSuggestBox.Name = formControl.UserControl.Name;
                            autoSuggestBox.Tag = formControl.UserControl.Tag;
                        }
                        _formControlsList.Add(formControl);
                        break;
                    case RadioButton radioButton:
                        formControl = new FormControl((FrameworkElement)control, formUserControl);
                        _formControlsList.Add(formControl);

                        _radioButtonsList.Add((RadioButton)control);
                        break;
                    case AutoSuggestTextBoxUserControl autoSuggestTextBox:
                        ScanControls((control as AutoSuggestTextBoxUserControl).Panel, control as FrameworkElement);
                        break;
                    case FormHeaderUserControl formHeader:
                        ScanControls((control as FormHeaderUserControl).Panel, control as FrameworkElement);
                        break;
                    case RadioOperatorUserControl radioOperator:
                        ScanControls((control as RadioOperatorUserControl).Panel, control as FrameworkElement);
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

        public List<FormControl> FormControlsList => _formControlsList;

        public DependencyObject Panel => formHeaderUserControl;

        private bool namePanel1Visibility = true;
        public bool NamePanel1Visibility
        { 
            get => namePanel1Visibility; 
            set => Set(ref namePanel1Visibility, value);
        }

        public bool NamePanel2Visibility => !NamePanel1Visibility;

        private string headerString1;
        public string HeaderString1
        {
            get => headerString1;
            set => Set(ref headerString1, value);
        }

        private string headerString2;
        public string HeaderString2
        {
            get => headerString2;
            set => Set(ref headerString2, $" {value}");
        }

        private string headerSubstring;
        public string HeaderSubstring
        {
            get => headerSubstring;
            set => Set(ref headerSubstring, value);
        }

        //private string headerPIF;
        //public string HeaderPIF
        //{
        //    get => $"PIF: {headerPIF}";
        //    set => Set(ref headerPIF, value);
        //}

        public void TextBox_MsgTimeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl;
                formControl = _formControlsList.Find(x => textBox.Name == x.InputControl.Name);

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

        //private void TextBoxLocation_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //    sender.Text = args.SelectedItem as string;
        //}

        //private void AutoSuggestBoxLocation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        //Set the ItemsSource to be your filtered dataset
        //        //sender.ItemsSource = null;
        //        _ICSLocationFiltered = new List<string>();
        //        foreach (string s in ICSPosition)
        //        {
        //            string lowerS = s.ToLower();
        //            if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
        //            {
        //                _ICSPositionFiltered.Add(s);
        //            }
        //        }
        //        sender.ItemsSource = _ICSLocationFiltered;
        //    }
        //    AutoSuggestBox_TextChanged(sender, null);
        //}

        //private void DeleteCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        //{
        //    if (args.Parameter != null)
        //    {
        //        foreach (var i in collection)
        //        {
        //            if (i.Text == (args.Parameter as string))
        //            {
        //                bool success = PreviousTexts.Remove(i.Text);
        //                collection.Remove(i);
        //                return;
        //            }
        //        }
        //    }
        //}

        //private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
        //    {
        //        VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
        //    }
        //}

        //private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        //}

        //private async void AutoSuggestBoxLocation_Loaded(object sender, RoutedEventArgs e)
        //{
        //    string name = (sender as AutoSuggestBox).Name;
        //    PreviousTexts = await ApplicationData.Current.LocalFolder.ReadAsync<List<string>>(name);

        //}

        //private async void AutoSuggestBoxLocation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        //Set the ItemsSource to be your filtered dataset
        //        collection.Clear();
        //        foreach (string text in PreviousTexts)
        //        {
        //            string lowerCaseS = text.ToLower();
        //            //if (string.IsNullOrEmpty(sender.Text) || lowerCaseS.StartsWith(sender.Text.ToLower()))
        //            if (lowerCaseS.StartsWith(sender.Text.ToLower()))
        //            {
        //                collection.Add(new ListItemData { Text = text, Command = _deleteCommand });
        //            }
        //        }

        //        bool matchFound = false;
        //        for (int i = 0; i < PreviousTexts.Count; i++)
        //        {
        //            if (sender.Text.ToLower().Contains(PreviousTexts[i].ToLower()))
        //            {
        //                PreviousTexts[i] = sender.Text;
        //                _matchedText = sender.Text;
        //                matchFound = true;
        //                break;
        //            }
        //        }
        //        if (matchFound && !string.IsNullOrEmpty(sender.Text))
        //        {
        //            await ApplicationData.Current.LocalFolder.SaveAsync(sender.Name, PreviousTexts);
        //        }
        //        else if (!matchFound && !string.IsNullOrEmpty(sender.Text))
        //        {
        //            PreviousTexts.Add(sender.Text);
        //            _matchedText = sender.Text;
        //        }

        //        sender.ItemsSource = collection;
        //    }
        //    AutoSuggestBox_TextChanged(sender, null);
        //}

        //private void AutoSuggestBoxLocation_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    //Set sender.Text.You can use args.SelectedItem to build your text string.
        //    sender.Text = (args.SelectedItem as ListItemData).Text;
        //    if (_matchedText != sender.Text)
        //    {
        //        PreviousTexts.Remove(_matchedText);
        //    }
        //}

    }
}
