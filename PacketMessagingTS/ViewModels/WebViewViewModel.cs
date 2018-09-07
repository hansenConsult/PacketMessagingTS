using System;
using System.Windows.Input;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.ApplicationModel;

namespace PacketMessagingTS.ViewModels
{
    public class WebViewViewModel : BaseViewModel
    {
        // TODO WTS: Set the URI of the page to show by default   ms-appx
        //private const string DefaultUrl = "https://developer.microsoft.com/en-us/windows/apps";

        private const string DefaultUrl = "ms-appdata:///local/PacFORMS/XSC_ICS-213_Message_v070628.html";
        //private const string DefaultUrl = "ms-appdata:///local/PacFORMS/XSC_EOC-213RR_v1708.html";
        //private const string DefaultUrl = "ms-appdata:///local/PacFORMS/BedStatus.html";



        private Uri _source;
        public Uri Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private string _sourceUrl;
        public string SourceUrl
        {
            get => _sourceUrl;
            set
            {
                _sourceUrl = value;
                Source = new Uri(value);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (value)
                {
                    IsShowingFailedMessage = false;
                }

                SetProperty(ref _isLoading, value);
                IsLoadingVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _isLoadingVisibility;

        public Visibility IsLoadingVisibility
        {
            get { return _isLoadingVisibility; }
            set { SetProperty(ref _isLoadingVisibility, value); }
        }

        private bool _isShowingFailedMessage;

        public bool IsShowingFailedMessage
        {
            get
            {
                return _isShowingFailedMessage;
            }

            set
            {
                if (value)
                {
                    IsLoading = false;
                }

                SetProperty(ref _isShowingFailedMessage, value);
                FailedMesageVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _failedMesageVisibility;

        public Visibility FailedMesageVisibility
        {
            get { return _failedMesageVisibility; }
            set { SetProperty(ref _failedMesageVisibility, value); }
        }

        public bool PopulateEmptyForm
        { get; set; }

        private ICommand _navCompleted;
        public ICommand NavCompletedCommand
        {
            get
            {
                if (_navCompleted == null)
                {
                    _navCompleted = new RelayCommand<WebViewNavigationCompletedEventArgs>(NavCompletedAsync);
                }

                return _navCompleted;
            }
        }

        public List<string> InlList { get; set; }    // List of index
        public List<string> InrList { get; set; }   // List of values

        private string[] PrepareParameters(List<string> sourceList, int startIndex, int parameterCount)
        {
            string[] parameterArray = new string[parameterCount + 1];
            parameterArray[0] = startIndex.ToString();
            for (int i = startIndex, j = 1; i < parameterCount + startIndex; i++, j++)
            {
                parameterArray[j] = sourceList[i];
            }
            return parameterArray;
        }

        const int maxParameterCount = 5;
        private async Task SetPacFormDataArrayAsync(string functionName, List<string> parameters)
        {
            for (int i = 0; i < parameters.Count + maxParameterCount; i += maxParameterCount)
            {//35 - 10 * 5
                int parameterCount = Math.Min(maxParameterCount, parameters.Count - i);
                if (parameterCount <= 0)
                    break;

                string[] functionParameters = PrepareParameters(parameters, i, parameterCount);
                string jsFunctionName = functionName + $"{parameterCount}";
                await _webView.InvokeScriptAsync(jsFunctionName, functionParameters);   // LoadInrArray5
            }
        }

        private async void NavCompletedAsync(WebViewNavigationCompletedEventArgs e)
        {
            IsLoading = false;
            OnPropertyChanged(nameof(BrowserBackCommand));
            OnPropertyChanged(nameof(BrowserForwardCommand));
            if (PopulateEmptyForm)
            {
                string[] args = new string[] { Utilities.GetMessageNumberPacket(),
                                    Singleton<IdentityViewModel>.Instance.UserCallsign ?? "",
                                    Singleton<IdentityViewModel>.Instance.UserName ?? ""};
                await _webView.InvokeScriptAsync("SetMessageNumber", args);
            }
            else
            {
                await SetPacFormDataArrayAsync("LoadInlArray", InlList);
                await SetPacFormDataArrayAsync("LoadInrArray", InrList);
                string[] args = new string[] { "0" };
                await _webView.InvokeScriptAsync("PopulateForm", args);
            }
        }

        private ICommand _navFailed;

        public ICommand NavFailedCommand
        {
            get
            {
                if (_navFailed == null)
                {
                    _navFailed = new RelayCommand<WebViewNavigationFailedEventArgs>(NavFailed);
                }

                return _navFailed;
            }
        }

        private void NavFailed(WebViewNavigationFailedEventArgs e)
        {
            // Use `e.WebErrorStatus` to vary the displayed message based on the error reason
            IsShowingFailedMessage = true;
        }

        private ICommand _retryCommand;

        public ICommand RetryCommand
        {
            get
            {
                if (_retryCommand == null)
                {
                    _retryCommand = new RelayCommand(Retry);
                }

                return _retryCommand;
            }
        }

        private void Retry()
        {
            IsShowingFailedMessage = false;
            IsLoading = true;

            _webView?.Refresh();
        }

        private ICommand _browserBackCommand;

        public ICommand BrowserBackCommand
        {
            get
            {
                if (_browserBackCommand == null)
                {
                    _browserBackCommand = new RelayCommand(() => _webView?.GoBack(), () => _webView?.CanGoBack ?? false);
                }

                return _browserBackCommand;
            }
        }

        private ICommand _browserForwardCommand;
        public ICommand BrowserForwardCommand
        {
            get
            {
                if (_browserForwardCommand == null)
                {
                    _browserForwardCommand = new RelayCommand(() => _webView?.GoForward(), () => _webView?.CanGoForward ?? false);
                }

                return _browserForwardCommand;
            }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(() => _webView?.Refresh());
                }

                return _refreshCommand;
            }
        }

//<!-- PART1 -->
//</head>
//<body class="tcolorff" onLoad="hide_message(); datetime(0); custom();">
//<!-- PART2 -->
//<center>

//<!-- PART1 -->
//<script language = "JavaScript" type="text/javascript">
// // <!--
//inl=[3,5,8,11,18,20,21,23,24,25,26,31,33,41,48,49,50,51,52,53];
//inr=["6DM-123M","08/28/2018","true","true","0823","Operations","Operations}2","Planning","Planning}3","MTVEOC","Radio Room","Subject","\nMessage","true","true","Packet","KZ6DM","Poul Hansen","08/28/2018","0824"];
//  fromlocal = 0;
// function fillvalue()
//        {
//            var mssg, ms2;
//            thisurl = "XSC_ICS-213_Message_v070628.html";
//            outels();
//        }
//// -->
//</SCRIPT>
//</head>
//<body text = "#000000" bgcolor="#FFFFFF" onLoad="hide_message(); fillvalue()">
//<center>


        public async Task<string> CreateSourceFormAsync()
        {
            // Parse for PacForms browser
            // Open form and insert message no, user callsign and User
            //string sourceUrl = File.ReadAllText(SourceUrl);
            StorageFolder pacFormsLocation = await Package.Current.InstalledLocation.GetFolderAsync("PacForms");
            StorageFile pacForm = await pacFormsLocation.TryGetItemAsync("XSC_ICS-213_Message_v070628.html") as StorageFile;

            string sourceUrl = File.ReadAllText(pacForm.Path);

            int part1Index = sourceUrl.IndexOf("PART1 -->");    // Substitute text between PART1 and PART2
            string searchString = "</head>";
            string searchStringPart2 = "<!-- PART2 -->";
            part1Index = sourceUrl.IndexOf(searchString, part1Index);
            int part2Index = sourceUrl.IndexOf(searchStringPart2, part1Index);

            StringBuilder openEmptyScript = new StringBuilder();
            openEmptyScript.AppendLine("<script language = \"JavaScript\" type = \"text/javascript\" >");

            string msgNumber = Utilities.GetMessageNumberPacket();
            string userCallsign = Singleton<IdentityViewModel>.Instance.UserCallsign ?? "";
            string userName = Singleton<IdentityViewModel>.Instance.UserName ?? "";
            string qsValues = $"\"{msgNumber}\",\"{userCallsign}\",\"{userName}\"";

            openEmptyScript.AppendLine("qsParm = [0,1,2];");
            openEmptyScript.AppendLine($"qsVal = [{qsValues}];");
            openEmptyScript.AppendLine("qsN = 3;");
            openEmptyScript.AppendLine("function fillvalue()");
            openEmptyScript.AppendLine("{");
            openEmptyScript.AppendLine("    var mssg, ms2;");
            openEmptyScript.AppendLine("    thisurl = \"XSC_ICS - 213_Message_v070628.html\";");
            openEmptyScript.AppendLine("    assignq();");
            openEmptyScript.AppendLine("}");
            openEmptyScript.AppendLine("</SCRIPT>");
            openEmptyScript.AppendLine("</head>");
            openEmptyScript.AppendLine("<body text = \"#000000\" bgcolor=\"#FFFFFF\" onLoad=\"hide_message(); fillvalue()\">");
            sourceUrl = sourceUrl.Remove(part1Index, part2Index - part1Index);
            sourceUrl = sourceUrl.Insert(part1Index, openEmptyScript.ToString());

            return sourceUrl;
        }

        private ICommand _openInBrowserCommand;

        public ICommand OpenInBrowserCommand
        {
            get
            {
                if (_openInBrowserCommand == null)
                {
                    //Uri sourceForm = new Uri(CreateSourceForm());
                    _openInBrowserCommand = new RelayCommand(async () => await Windows.System.Launcher.LaunchUriAsync(Source));
                }

                return _openInBrowserCommand;
            }
        }

        private WebView _webView;

        public WebViewViewModel()
        {
            IsLoading = true;
            SourceUrl = DefaultUrl;
            Source = new Uri(DefaultUrl);
        }

        public void Initialize(WebView webView)
        {
            _webView = webView;
        }
    }
}
