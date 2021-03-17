using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace PacketMessagingTS.ViewModels
{
    //public class WebViewViewModel : BaseViewModel
    public class WebViewViewModel : ViewModelBase
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
                if (_navCompleted is null)
                {
                    _navCompleted = new RelayCommand<WebViewNavigationCompletedEventArgs>(NavCompletedAsync);
                }

                return _navCompleted;
            }
        }

        public List<string> InlList { get; set; }    // List of indexes
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
            {
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
                                    IdentityViewModel.Instance.UserCallsign ?? "",
                                    IdentityViewModel.Instance.UserName ?? ""};
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
                if (_navFailed is null)
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
                if (_retryCommand is null)
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
                if (_browserBackCommand is null)
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
                if (_browserForwardCommand is null)
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
                if (_refreshCommand is null)
                {
                    _refreshCommand = new RelayCommand(() => _webView?.Refresh());
                }

                return _refreshCommand;
            }
        }
        /* Edge test code
        public async Task<string> CreateSourceFormAsync(string pacForm, string msgNumber, string userCallsign, string userName)
        {
            // Parse for PacForms browser
            // Open form and insert message no, user callsign and User
            StorageFolder pacFormsLocation = await Package.Current.InstalledLocation.GetFolderAsync("PacForms");
            StorageFile pacFormFile = await pacFormsLocation.TryGetItemAsync(pacForm) as StorageFile;

            string sourceUrl = File.ReadAllText(pacFormFile.Path);

            int part1Index = sourceUrl.IndexOf("PART1 -->");    // Substitute text between PART1 and PART2
            string searchString = "</head>";
            string searchStringPart2 = "<!-- PART2 -->";
            part1Index = sourceUrl.IndexOf(searchString, part1Index);
            int part2Index = sourceUrl.IndexOf(searchStringPart2, part1Index);

            StringBuilder openEmptyScript = new StringBuilder();
            openEmptyScript.AppendLine("<script language = \"JavaScript\" type = \"text/javascript\" >");
            openEmptyScript.AppendLine("function fillvalue()");
            openEmptyScript.AppendLine("{");
            openEmptyScript.AppendLine($"    document.forms[0].msgno.value = \"{msgNumber}\";");
            openEmptyScript.AppendLine($"    document.forms[0].ocall.value = \"{userCallsign}\";");
            openEmptyScript.AppendLine($"    document.forms[0].oname.value = \"{userName}\";");
            openEmptyScript.AppendLine("}");
            openEmptyScript.AppendLine("</script>");
            openEmptyScript.AppendLine("</head>");
            openEmptyScript.AppendLine("<body text = \"#000000\" bgcolor=\"#FFFFFF\" onLoad=\"hide_message(); datetime(0); fillvalue(); custom();\">");
            //openEmptyScript.AppendLine("<body text = \"#000000\" bgcolor=\"#FFFFFF\" onLoad=\"hide_message(); datetime(0); custom();\">");

            sourceUrl = sourceUrl.Remove(part1Index, part2Index - part1Index);
            sourceUrl = sourceUrl.Insert(part1Index, openEmptyScript.ToString());

            return sourceUrl;
        }
        */
        private ICommand _openInBrowserCommand;

        public ICommand OpenInBrowserCommand
        {
            get
            {
                if (_openInBrowserCommand is null)
                {
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
