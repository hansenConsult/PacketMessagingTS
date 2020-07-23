using System;
using System.Collections.Generic;

using FormControlBasicsNamespace;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public sealed partial class FormHeaderUserControl : FormControlBasics
    {
        public event EventHandler<FormEventArgs> EventMsgTimeChanged;

        public FormHeaderUserControl()
        {
            this.InitializeComponent();

            ScanControls(formHeaderUserControl);

            InitializeToggleButtonGroups();
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


    }
}
