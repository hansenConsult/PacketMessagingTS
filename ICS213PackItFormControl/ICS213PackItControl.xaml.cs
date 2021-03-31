using System;
using System.Collections.Generic;

using FormControlBaseClass;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Helpers.PrintHelpers;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

using Windows.UI.Xaml.Controls;
//using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml;
using FormControlBasicsNamespace;
using FormControlBaseMvvmNameSpace;

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
        public ICS213PackItControlViewModel ViewModel = ICS213PackItControlViewModel.Instance;
        
        

        double _messageBoxHeight;

        public ICS213PackItControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            ViewModel.ReceivedOrSent = "sent";
            ViewModel.HowReceivedSent = "otherRecvdType";
            otherText.Text = "Packet";
            autoSuggestBoxToICSPosition.ItemsSource = ICSPosition;
            autoSuggestBoxFromICSPosition.ItemsSource = ICSPosition;

            _messageBoxHeight = message.Height;

            ViewModelBase = ViewModel;

            UpdateFormFieldsRequiredColors();
        }

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacItForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.CountyForm;

        public override string GetPacFormName() => "form-ics213";	// Used in CreateFileName() 

        public override string PacFormType => "ICS213";

        public override string MessageNo
        {
            get => base.MessageNo;
            set
            {
                base.MessageNo = value;
                OriginMsgNo = value;
            }
        }

        public override void AppendDrillTraffic()
        {
            message.Text += DrillTraffic;
        }

        public override void SetPracticeField(string practiceField)
        {
            ViewModelBase.HandlingOrder = "Routine";
            subject.Text = practiceField;
            UpdateFormFieldsRequiredColors();       // TODO check this
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
                                var grid = new Grid();
                                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                                //grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                                //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                                // Header
                                TextBlock header = new TextBlock { Text = $"ICS 213 Message, message number: {MessageNo}", Margin = new Thickness(0, 0, 0, 20) };
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

        public override async void PrintForm()
        {
            if (CanvasContainer is null || DirectPrintContainer is null)
                return;

            _printPanels = PrintPanels;
            if (_printPanels is null || _printPanels.Count == 0)
                return;

            _printHelper = new PrintHelper(CanvasContainer);

            DirectPrintContainer.Children.Remove(_printPanels[0]);

            AddFooter();

            for (int i = 0; i < _printPanels.Count; i++)
            {
                _printHelper.AddFrameworkElementToPrint(_printPanels[i]);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            await _printHelper.ShowPrintUIAsync("  ");
        }

        protected override void ReleasePrintHelper()
        {
            _printHelper.Dispose();

            if (_printPanels[0] != null && !DirectPrintContainer.Children.Contains(_printPanels[0]))
            {
                foreach (FrameworkElement child in _printPanels[0].Children)
                {
                    if (child is TextBlock textBlock && textBlock.Text.Contains($"page 1 of"))
                        _printPanels[0].Children.Remove(child);
                }

                DirectPrintContainer.Children.Add(_printPanels[0]);
            }
        }

        public override string CreateSubject()
		{
            return $"{messageNo.Text}_{ViewModelBase.HandlingOrder?.ToUpper()[0]}_ICS213_{subject.Text}";
        }

        protected override string ConvertComboBoxFromOutpost(string id, ref string[] msgLines)
        {
            string comboBoxData = GetOutpostValue(id, ref msgLines);
            var comboBoxDataSet = comboBoxData.Split(new char[] { '}' }, StringSplitOptions.RemoveEmptyEntries);
            //formField.ControlContent = comboBoxDataSet[0];

            return comboBoxDataSet[0];
        }

        //protected override string CreateComboBoxOutpostDataString(FormField formField, string id)
        //{
        //    string[] data = formField.ControlContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    switch (FormProvider)
        //    {
        //        case FormProviders.PacForm:
        //            if (data.Length == 2)
        //            {
        //                if (data[1] == (-1).ToString())
        //                {
        //                    return $"{id}: [ }}0]";
        //                }
        //                else
        //                {
        //                    if (formField.ControlName == "comboBoxToICSPosition" || formField.ControlName == "comboBoxFromICSPosition")
        //                    {
        //                        int index = Convert.ToInt32(data[1]);
        //                        return $"{id}: [{data[0]}}}{(index + 1).ToString()}]";
        //                    }
        //                    else
        //                    {
        //                        return $"{id}: [{data[0]}}}{data[1]}]";
        //                    }
        //                }
        //            }
        //            else if (data[0] == "-1" || string.IsNullOrEmpty(data[0]))
        //            {
        //                return $"{id}: [ }}0]";
        //            }
        //            break;
        //        case FormProviders.PacItForm:
        //            break;
        //    }
        //    return "";
        //}

        //public override string CreateOutpostData(ref PacketMessage packetMessage)
        //{
        //    _outpostData = new List<string>()
        //    {
        //        "!SCCoPIFO!",
        //        "#T: form-ics213.html",
        //        $"#V: {PackItFormVersion}-{PIF}",
        //    };
        //    CreateOutpostDataFromFormFields(ref packetMessage, ref _outpostData);

        //    return CreateOutpostMessageBody(_outpostData);
        //}

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

        //private void ICSPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as ComboBox).Name == "comboBoxToICSPosition")
        //    {
        //        textBoxToICSPosition.Text = comboBoxToICSPosition.Text;
        //    }
        //    else if ((sender as ComboBox).Name == "comboBoxFromICSPosition")
        //    {
        //        textBoxFromICSPosition.Text = comboBoxFromICSPosition.Text;
        //    }
        //    ComboBoxRequired_SelectionChanged(sender, e);
        //}

    }
}
