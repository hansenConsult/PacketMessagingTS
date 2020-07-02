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
                EventHandler<FormEventArgs> OnMsgTimeChange = EventMsgTimeChanged;
                FormEventArgs formEventArgs = new FormEventArgs() { SubjectLine = textBox.Text };
                OnMsgTimeChange?.Invoke(this, formEventArgs);
            }
        }

    }
}
