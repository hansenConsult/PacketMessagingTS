﻿using System;
using System.Collections.Generic;
using System.Text;

using FormControlBaseClass;

using FormControlBaseMvvmNameSpace;

using FormControlBasicsNamespace;

using PacketMessagingTS.Core.Helpers;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

namespace MessageFormControl
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    [FormControl(
        FormControlName = "SimpleMessage",
        FormControlMenuName = "Simple Message",
        FormControlType = FormControlAttribute.FormType.None
        )
    ]

    public partial class MessageControl : FormControlBase
    {

        //MessageFormControlViewModel ViewModel = MessageFormControlViewModel.Instance;
        readonly MessageFormControlViewModel ViewModel = new MessageFormControlViewModel();

        //private HeaderVisibility _visibleHeader = HeaderVisibility.None;
        //private HeaderVisibility _previousVisibleHeader = HeaderVisibility.None;

        private new PrintHelper _printHelper;


        public MessageControl()
        {
            InitializeComponent();

            ScanControls(PrintableArea);

            GetFormDataFromAttribute(GetType());

            ViewModelBase = ViewModel;

            ViewModel.MessageFormControl = this;

            UpdateFormFieldsRequiredColors();
        }


        //private bool inBoxHeaderVisibility;
        //public bool InBoxHeaderVisibility
        //{
        //    get => inBoxHeaderVisibility;
        //    set
        //    {
        //        SetProperty(ref inBoxHeaderVisibility, value);
        //        if (value)
        //        {
        //            _visibleHeader = HeaderVisibility.InboxHeader;
        //            FixedContentVisibility = true;
        //            SentHeaderVisibility = false;
        //            NewHeaderVisibility = false;
        //            PrintHeaderVisibility = false;
        //        }
        //    }
        //}

        //private bool sentHeaderVisibility;
        //public bool SentHeaderVisibility
        //{
        //    get => sentHeaderVisibility;
        //    set
        //    {
        //        SetProperty(ref sentHeaderVisibility, value);
        //        if (value)
        //        {
        //            _visibleHeader = HeaderVisibility.SentHeader;
        //            FixedContentVisibility = true;
        //            InBoxHeaderVisibility = false;
        //            NewHeaderVisibility = false;
        //            PrintHeaderVisibility = false;
        //        }
        //    }
        //}

        //private bool newHeaderVisibility;
        //public bool NewHeaderVisibility
        //{
        //    get => newHeaderVisibility;
        //    set
        //    {
        //        SetProperty(ref newHeaderVisibility, value);
        //        if (value)
        //        {
        //            _visibleHeader = HeaderVisibility.NewHeader;
        //            FixedContentVisibility = false;
        //            InBoxHeaderVisibility = false;
        //            SentHeaderVisibility = false;
        //            PrintHeaderVisibility = false;
        //        }
        //    }
        //}

        //private bool printHeaderVisibility;
        //public bool PrintHeaderVisibility
        //{
        //    get => printHeaderVisibility;
        //    set
        //    {
        //        SetProperty(ref printHeaderVisibility, value);
        //        if (value)
        //        {
        //            _visibleHeader = HeaderVisibility.PrintHeader;
        //            FixedContentVisibility = true;
        //            NewHeaderVisibility = false;
        //            InBoxHeaderVisibility = false;
        //            SentHeaderVisibility = false;
        //        }
        //    }
        //}

        //private bool fixedContentVisibility;
        //public bool FixedContentVisibility
        //{
        //    get => fixedContentVisibility;
        //    set 
        //    {
        //        SetProperty(ref fixedContentVisibility, value);
        //    }
        //}

        //private void SetHeaderVisibility()
        //{
        //    switch (_visibleHeader)
        //    {
        //        case HeaderVisibility.None:
        //            break;
        //        case HeaderVisibility.InboxHeader:
        //            InBoxHeaderVisibility = true;
        //            break;
        //        case HeaderVisibility.SentHeader:
        //            SentHeaderVisibility = true;
        //            break;
        //        case HeaderVisibility.NewHeader:
        //            NewHeaderVisibility = true;
        //            break;
        //        case HeaderVisibility.PrintHeader:
        //            PrintHeaderVisibility = true;
        //            break;
        //        //case HeaderVisibility.FixedContent:
        //        //    FixedContentVisibility = true;
        //        //    break;
        //    }
        //}

        //private string _messageBody;
        //public override string MessageBody
        //{
        //    get => _messageBody;
        //    set => SetProperty(ref _messageBody, value);
        //}

        //public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.None;

        public override FormControlBaseMvvm RootPanel => rootPanel;

        public override FormProviders FormProvider => FormProviders.PacForm;

        //public override string GetPacFormName() => "SimpleMessage";

        public override string PacFormType => "SimpleMessage";

        public override void AppendDrillTraffic()
        {
            //messageBody.Text += DrillTraffic;
            ViewModel.MessageBody += DrillTraffic;
        }

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        public override List<Panel> PrintPanels
        {
            get
            {
                ViewModel._previousVisibleHeader = ViewModel._visibleHeader;
                ViewModel._visibleHeader = MessageFormControlViewModel.HeaderVisibility.PrintHeader;
                ViewModel.SetHeaderVisibility();

                List<Panel> printPages = new List<Panel>();

                Panel printPage1 = DirectPrintContainer.FindName("printableArea") as Panel;
                //printPages.Add(printableArea);
                printPages.Add(printPage1);

                return printPages;
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

            //Page page = new Page();
            //page.Content = printableArea;
            //_printHelper.PreparePrintContent(page);

            for (int i = 0; i < _printPanels.Count; i++)
            {
                DirectPrintContainer.Children.Remove(_printPanels[i]);
            }

            for (int i = 0; i < _printPanels.Count; i++)
            {
                _printHelper.AddFrameworkElementToPrint(_printPanels[i]);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            _printHelper.OnPreviewPagesCreated += PrintHelper_OnPreviewPagesCreated;

            await _printHelper.ShowPrintUIAsync(" ");
        }

        protected override void PrintHelper_OnPreviewPagesCreated(List<FrameworkElement> FrameworkElementList)
        {
            for (int i = 0; i < FrameworkElementList.Count; i++)
            {
                if (FrameworkElementList[i].FindName("footer") is TextBlock footer)
                {
                    footer.Text = $"Page {i + 1} of {FrameworkElementList.Count}";
                }
            }       
        }

        protected override void ReleasePrintHelper()
        {
            // Must use local _printHelper
            _printHelper.Dispose();

            for (int i = 0; i < _printPanels.Count; i++)
            {
                if (_printPanels[i] != null && !DirectPrintContainer.Children.Contains(_printPanels[i]))
                {
                    if (_printPanels[i].FindName("footer") is TextBlock footer)
                    {
                        _printPanels[i].Children.Remove(footer);
                    }

                    DirectPrintContainer.Children.Add(_printPanels[i]);
                }
            }

            ViewModel._visibleHeader = ViewModel._previousVisibleHeader;
            ViewModel.SetHeaderVisibility();
        }

        protected override void CreateOutpostDataFromFormFields(ref PacketMessage packetMessage, ref List<string> outpostData)
        {
            foreach (FormField formField in packetMessage.FormFieldArray)
            {
                if (formField.ControlContent is null || formField.ControlContent.Length == 0)
                    continue;

                switch (formField.ControlName)
                {
                    case "messageBody":
                        //string filteredMsg = formField.ControlContent.Replace("\r\n", "\r");
                        string filteredMsg = formField.ControlContent.Replace("\\n", "\r");
                        outpostData.Add($"{filteredMsg}");
                        break;
                }
            }
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>();

            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override FormField[] ConvertFromOutpost(string msgNumber, ref string[] msgLines, FormProviders formProvider)
        {
            StringBuilder sb = new StringBuilder();
            // Skip to start of message
            int i = 0;
            for (; i < msgLines.Length; i++)
            {
                if (msgLines[i].StartsWith("Subject:"))
                {
                    i++;
                    break;
                }
            }
            // Message
            for (; i < msgLines.Length; i++)
            {
				string convertedLine = ConvertLineTabsToSpaces(msgLines[i], 8);

				//sb.AppendLine(msgLines[i]);
				sb.AppendLine(convertedLine);
			}
            // Use RichTextBlock
            Paragraph paragraph = new Paragraph();
            Run run = new Run
            {
                Text = sb.ToString()
            };

            // Add the Run to the Paragraph, the Paragraph to the RichTextBlock.
            paragraph.Inlines.Add(run);
            richTextMessageBody.Blocks.Add(paragraph);
            //string messageBody = sb.ToString();

            FormField[] formFields = CreateEmptyFormFieldsArray();
            foreach (FormField formField in formFields)
            {
                switch (formField.ControlName)
                {
                    case "richTextMessageBody":
                        formField.ControlContent = run.Text;
                        break;
                        //case "messageBody":
                        //    formField.ControlContent = messageBody;
                        //    break;
                }
            }
            return formFields;
        }

		public override string CreateSubject() => null;

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "messageBody":
                            //ViewModel.MessageBody = formField.ControlContent;
                            ViewModel.MessageBody = formField.ControlContent.Replace("\\n", "\n");
                            //string msgBody = formField.ControlContent.Replace("\\n", "\n");
                            //ViewModel.MessageBody = "1234567890\r123456";
                            found1 = true;
                            break;
                        case null:
                            continue;
                    }
                }
                if (found1)
                    break;
            }
            base.FillFormFromFormFields(formFields);

            //UpdateFormFieldsRequiredColors();
        }

        public override void MessageChanged(string message)
        {
            ViewModel.MessageBody = message ?? "";

            if (string.IsNullOrEmpty(message))
                return;

            Paragraph paragraph = new Paragraph();
            Run run = new Run
            {
                Text = message
            };

            // Add the Run to the Paragraph, the Paragraph to the RichTextBlock.
            paragraph.Inlines.Add(run);
            richTextMessageBody.Blocks.Add(paragraph);
        }

    }
}