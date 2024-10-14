using System;
using System.Collections.Generic;

using FormControlBaseClass;

using Microsoft.UI.Xaml.Controls;

using SharedCode;
using SharedCode.Helpers.PrintHelpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using FormControlBaseMvvmNameSpace;
using PacketMessagingTS.Core.Helpers;
using SharedCode.Models;
using FormUserControl;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ICS213PackItFormControl
{
    [FormControl(
        FormControlName = "form-ics213",
        FormControlMenuName = "ICS-213 Message",
        FormControlType = FormControlAttribute.FormType.CountyForm
        )
    ]

    public partial class ICS213PackItControl : FormControlBase
    {
        private readonly ICS213PackItControlViewModel ViewModel = new ICS213PackItControlViewModel();
        private readonly double _messageBoxHeight;

        public ICS213PackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            ViewModel.PIF = "2.2";
            receivedOrSent.SelectedIndex = 1;
            ViewModel.HowReceivedSent = otherRecvdType;
            otherText.Text = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;

            _messageBoxHeight = message.Height;

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override string PacFormType => "ICS213";

        public override void AppendDrillTraffic()
        {
            message.Text += DrillTraffic;
        }

        public override void SetPracticeField(string practiceField)
        {
            handlingOrder.SelectedIndex = 2;
            ViewModelBase.HandlingOrder = "Routine";
            if (string.IsNullOrEmpty(practiceField))
            {
                practiceField = "";
            }
            subject.Text = practiceField;
            UpdateFormFieldsRequiredColors();       // TODO check this. Subject is red unless called.
        }

        public override Panel DirectPrintContainer => directPrintContainer;

        public override Panel CanvasContainer => container;

        public override List<Panel> PrintPanels
        {
            get
            {
                _scrollViewer = FindVisualChild<ScrollViewer>(message);
                if (!(_scrollViewer is null))
                {
                    double schollViewerHeight = _scrollViewer.ExtentHeight;
                    if (_messageBoxHeight < _scrollViewer.ExtentHeight)
                    {
                        List<Panel> printPages = new List<Panel>();
                        int pageCount = Math.Min((int)(schollViewerHeight / _messageBoxHeight) + 1, 2);
                        for (int i = 0; i < pageCount; i++)
                        {
                            if (i == 0)
                            {
                                _scrollViewer.ChangeView(null, 0, null, true);
                                printPages.Add(printPage1);
                            }
                            else
                            {
                                Grid grid = new Grid();
                                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                                // Header
                                TextBlock header = new TextBlock { Text = $"ICS 213 Message, message number: {ViewModelBase.MessageNo}", Margin = new Thickness(0, 0, 0, 20) };
                                Grid.SetRow(header, 0);
                                grid.Children.Add(header);

                                // Main content
                                TextBox messageBox = new TextBox()
                                {
                                    BorderThickness = new Thickness(0, 0, 0, 0),
                                    AcceptsReturn = true,
                                    TextWrapping = TextWrapping.Wrap,
                                    Text = message.Text,
                                };
                                Grid.SetRow(messageBox, 1);
                                grid.Children.Add(messageBox);

                                printPages.Add(grid);
                            }
                        }
                        return printPages;
                    }
                }
                return new List<Panel> { printPage1 };
            }
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
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl)
                            {
                                BaseBorderColor = textBox.IsReadOnly ? textBox.Background : textBox.BorderBrush
                            };
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

                    case RadioButtons radioButtons:
                        {
                            FormControl formControl = new FormControl((FrameworkElement)control, formUserControl);
                            _formControlsList.Add(formControl);

                            if (radioButtons.Name == "reply")
                            {
                                if ((FindName("replyBy") as TextBox) != null)
                                {
                                    formControl = new FormControl(FindName("replyBy") as TextBox, formUserControl);
                                    _formControlsList.Add(formControl);
                                }
                            }
                            else if (radioButtons.Name == "howRecevedSent")
                            {
                                if ((FindName("otherText") as TextBox) != null)
                                {
                                    formControl = new FormControl(FindName("otherText") as TextBox, formUserControl);
                                    _formControlsList.Add(formControl);
                                }
                            }
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
                    default:
                        break;
                }

            }
        }

        public override string CreateSubject()
		{
            return $"{messageNo.Text}_{ViewModelBase.HandlingOrder?.ToUpper()[0]}_ICS213_{subject.Text}";
        }

        protected override string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            string[] comboBoxDataSet = comboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);

            return comboBoxDataSet[0];
        }

        private void Severity_SelectionChanged(object sender, SelectionChangedEventArgs e) => Subject_Changed(sender, null);

        protected virtual void TextBox_ReplyTimeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FormControl formControl = !string.IsNullOrEmpty(textBox.Name)
                    ? _formControlsList.Find(x => textBox.Name == x.InputControl.Name)
                    : _formControlsList.Find(x => GetTagIndex(x.InputControl) == GetTagIndex(textBox));

                if (formControl == null)
                {
                    return;
                }

                CheckTimeFormat(formControl);
            }
        }

        //public override void FillFormFromFormFields(FormField[] formFields)
        //{
        //    bool found1 = false;
        //    foreach (FormField formField in formFields)
        //    {
        //        FrameworkElement control = GetFrameworkElement(formField);

        //        if (control is null || string.IsNullOrEmpty(formField.ControlContent))
        //            continue;

        //        if (control is TextBox textBox)
        //        {
        //            switch (control.Name)
        //            {
        //                case "subject":
        //                    Subject = formField.ControlContent;
        //                    found1 = true;
        //                    break;
        //                case null:
        //                    continue;
        //            }
        //        }
        //        if (found1)
        //            break;
        //    }
        //    base.FillFormFromFormFields(formFields);
        //}


        //private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //    {
        //        DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //        if (child != null && child is T)
        //            return (T)child;
        //        else
        //        {
        //            T childOfChild = FindVisualChild<T>(child);
        //            if (childOfChild != null)
        //                return childOfChild;
        //        }
        //    }
        //    return null;
        //}

        //private void TextBoxICSPosition_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //    sender.Text = args.SelectedItem as string;
        //}

        //private void TextBoxFromICSPosition_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        //Set the ItemsSource to be your filtered dataset
        //        //sender.ItemsSource = null;
        //        _ICSPositionFiltered = new List<string>();
        //        foreach (string s in ICSPosition)
        //        {
        //            string lowerS = s.ToLower();
        //            if (string.IsNullOrEmpty(sender.Text) || lowerS.StartsWith(sender.Text.ToLower()))
        //            {
        //                _ICSPositionFiltered.Add(s);
        //            }
        //        }
        //        sender.ItemsSource = _ICSPositionFiltered;
        //    }
        //}

    }
}
