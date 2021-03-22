using System;
using System.Collections.Generic;

using FormControlBaseClass;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using SharedCode;
using SharedCode.Helpers;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static PacketMessagingTS.Core.Helpers.FormProvidersHelper;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PublicNoticeFormControl
{
    [FormControl(
        FormControlName = "PublicNotice",
        FormControlMenuName = "Public Notice",
        FormControlType = FormControlAttribute.FormType.TestForm
        )
    ]

    public sealed partial class PublicNoticeControl : FormControlBase
    {
        public PublicNoticeControl()
        {
            this.InitializeComponent();

            ScanControls(PrintableArea);

            InitializeToggleButtonGroups();

            UpdateFormFieldsRequiredColors();

            _printPanel = printPage1;

            PageVisibility = true;
            NoticeVisibility = false;
        }

        public override FormProviders FormProvider => FormProviders.PacForm;

        public override FormControlAttribute.FormType FormControlType => FormControlAttribute.FormType.TestForm;

        public override string GetPacFormName() => "PublicNotice";

        public override string PacFormType => "PublicNoticeForm";

        public override Panel CanvasContainer => container;

        public override Panel DirectPrintContainer => directPrintContainer;

        private Panel _printPanel;
        public override List<Panel> PrintPanels => new List<Panel> { _printPanel };

        public override void AppendDrillTraffic()
        {
            notice.Text += DrillTraffic;
        }

        private bool _pageVisibility = true;
        public bool PageVisibility
        {
            get => _pageVisibility;
            set => SetProperty(ref _pageVisibility, value);
        }

        private bool _noticeVisibility = false;
        public bool NoticeVisibility
        {
            get => _noticeVisibility;
            set => SetProperty(ref _noticeVisibility, value);
        }

        private string _noticeType;
        public string NoticeType
        {
            get => _noticeType;
            set => SetProperty(ref _noticeType, value);
        }

        private int _typeFontSize = 50;
        public int TypeFontSize
        {
            get => _typeFontSize;
            set => SetProperty(ref _typeFontSize, value);
        }

        private string _topic;
        public string Topic
        {
            get => _topic;
            set => SetProperty(ref _topic, value);
        }

        private int _topicFontSize = 35;
        public int TopicFontSize
        {
            get => _topicFontSize;
            set => SetProperty(ref _topicFontSize, value);
        }

        private string _issuedBy;
        public string IssuedBy
        {
            get => _issuedBy;
            set => SetProperty(ref _issuedBy, value);
        }

        private string _effectiveDate;
        public string EffectiveDate
        {
            get => _effectiveDate;
            set => SetProperty(ref _effectiveDate, value);
        }

        private string _expires;
        public string Expires
        {
            get => _expires;
            set => SetProperty(ref _expires, value);
        }

        private string _notice;
        public string Notice
        {
            get => _notice;
            set => SetProperty(ref _notice, value);
        }

        private string _signed;
        public string Signed
        {
            get => _signed;
            set => SetProperty(ref _signed, value);
        }

        public override string CreateOutpostData(ref PacketMessage packetMessage)
        {
            List<string> outpostData = new List<string>()
            {
                "!PACF! " + packetMessage.Subject,
                "# JS:Public Notice (which4) ",
                "# JS-ver. PR-4.7-1.2, 02/27/20",
                "# FORMFILENAME: PublicNotice.html"
            };
            CreateOutpostDataFromFormFields(ref packetMessage, ref outpostData);

            return CreateOutpostMessageBody(outpostData);
        }

        public override string CreateSubject()
        {
            return ($"{messageNo.Text}_{HandlingOrder?.ToUpper()[0]}_PubNotice_");
        }

        private void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            PageVisibility = false;
            NoticeVisibility = true;

            _printPanel = noticePage;
        }

        public override void FillFormFromFormFields(FormField[] formFields)
        {
            bool found1 = false, found2 = false;
            foreach (FormField formField in formFields)
            {
                FrameworkElement control = GetFrameworkElement(formField);

                if (control is null || string.IsNullOrEmpty(formField.ControlContent))
                    continue;

                if (control is TextBox textBox)
                {
                    switch (control.Name)
                    {
                        case "typeFontSize":
                            TypeFontSize = Convert.ToInt32(formField.ControlContent);
                            found1 = true;
                            break;
                        case "topic":
                            Topic = formField.ControlContent;
                            break;
                        case "topicFontSize":
                            TopicFontSize = Convert.ToInt32(formField.ControlContent);
                            break;
                        case "issuedBy":
                            IssuedBy = formField.ControlContent;
                            break;
                        case "effectiveDate":
                            EffectiveDate = formField.ControlContent;
                            break;
                        case "expires":
                            Expires = formField.ControlContent;
                            break;
                        case "notice":
                            Notice = formField.ControlContent;
                            break;
                        case "signed":
                            Signed = formField.ControlContent;
                            break;
                        case null:
                            continue;
                    }
                }
                if (found1 && found2)
                    break;
            }
            base.FillFormFromFormFields(formFields);

            UpdateFormFieldsRequiredColors();
        }

        //public override async void PrintForm()
        //{
        //    if (CanvasContainer is null || DirectPrintContainer is null)
        //        return;

        //    _printPanels = PrintPanels;
        //    if (_printPanels is null || _printPanels.Count == 0)
        //        return;

        //    _printHelper = new PrintHelper(CanvasContainer);

        //    DirectPrintContainer.Children.Remove(_printPanels[0]);

        //    AddFooter();

        //    for (int i = 0; i < _printPanels.Count; i++)
        //    {
        //        _printHelper.AddFrameworkElementToPrint(_printPanels[i]);
        //    }

        //    _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
        //    _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
        //    _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

        //    await _printHelper.ShowPrintUIAsync("  ");
        //}

        private void ComboBoxNoticeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                NoticeType = (string)(e.AddedItems[0] as ComboBoxItem).Content;
            }
        }

    }
}
