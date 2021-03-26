using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using MetroLog;

using PacketMessagingTS.Core.Helpers;

using PacketMessagingTS.Helpers;
using PacketMessagingTS.Models;
using PacketMessagingTS.Services.CommunicationsService;
using PacketMessagingTS.ViewModels;

using SharedCode;
using SharedCode.Helpers;
using SharedCode.Helpers.PrintHelpers;

using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PacketMessagingTS.Views
{
    /*
    /// <summary>
    /// Photo size options
    /// </summary>
    public enum PhotoSize : byte
    {
        SizeFullPage,
        Size8x10
    }

    /// <summary>
    /// Photo scaling options
    /// </summary>
    public enum Scaling : byte
    {
        ShrinkToFit,
        Crop
    }

    /// <summary>
    /// Printable page description
    /// </summary>
    public class PageDescription : IEquatable<PageDescription>
    {
        public Windows.Foundation.Size Margin;
        public Windows.Foundation.Size PageSize;
        public Windows.Foundation.Size ViewablePageSize;
        public Windows.Foundation.Size PictureViewSize;
        public bool IsContentCropped;

        public bool Equals(PageDescription other)
        {
            // Detect if PageSize changed
            bool equal = (Math.Abs(PageSize.Width - other.PageSize.Width) < double.Epsilon) &&
                (Math.Abs(PageSize.Height - other.PageSize.Height) < double.Epsilon);

            // Detect if ViewablePageSize changed
            if (equal)
            {
                equal = (Math.Abs(ViewablePageSize.Width - other.ViewablePageSize.Width) < double.Epsilon) &&
                    (Math.Abs(ViewablePageSize.Height - other.ViewablePageSize.Height) < double.Epsilon);
            }

            // Detect if PictureViewSize changed
            if (equal)
            {
                equal = (Math.Abs(PictureViewSize.Width - other.PictureViewSize.Width) < double.Epsilon) &&
                    (Math.Abs(PictureViewSize.Height - other.PictureViewSize.Height) < double.Epsilon);
            }

            // Detect if cropping changed
            if (equal)
            {
                equal = IsContentCropped == other.IsContentCropped;
            }

            return equal;
        }
    }

    class ICS309PrintHelper : PrintHelper
    {
        /// <summary>
        /// The app's number of photos
        /// </summary>
        private const int NumberOfPhotos = 1;

        /// <summary>
        /// Constant for 96 DPI
        /// </summary>
        private const int DPI96 = 96;

        /// <summary>
        /// Current size settings for the image
        /// </summary>
        private PhotoSize photoSize;

        /// <summary>
        /// Current scale settings for the image
        /// </summary>
        private Scaling photoScale;

        /// <summary>
        /// A map of UIElements used to store the print preview pages.
        /// </summary>
        private Dictionary<int, UIElement> pageCollection = new Dictionary<int, UIElement>();

        /// <summary>
        /// Synchronization object used to sync access to pageCollection and the visual root(PrintingRoot).
        /// </summary>
        private static object printSync = new object();

        /// <summary>
        /// The current printer's page description used to create the content (size, margins, printable area)
        /// </summary>
        private PageDescription currentPageDescription;

        /// <summary>
        /// A request "number" used to describe a Paginate - GetPreviewPage session.
        /// It is used by GetPreviewPage to determine, before calling SetPreviewPage, if the page content is out of date.
        /// Flow:
        /// Paginate will increment the request count and all subsequent GetPreviewPage calls will store a local copy and verify it before calling SetPreviewPage.
        /// If another Paginate event is triggered while some GetPreviewPage workers are still executing asynchronously
        /// their results will be discarded(ignored) because their request number is expired (the photo page description changed).
        /// </summary>
        private long requestCount;

        public ICS309PrintHelper(Page scenarioPage) : base(scenarioPage)
        {
            photoSize = PhotoSize.SizeFullPage;
            photoScale = Scaling.ShrinkToFit;
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified
        /// by PrintTaskRequestedEventArgs->Request->Deadline.
        /// Therefore, we use this handler to only create the print task.
        /// The print settings customization can be done when the print document source is requested.
        /// </summary>
        /// <param name="sender">The print manager for which a print task request was made.</param>
        /// <param name="e">The print task request associated arguments.</param>
        protected override void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("ICS309", sourceRequestedArgs =>
            {
                PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);

                // Choose the printer options to be shown.
                // The order in which the options are appended determines the order in which they appear in the UI
                printDetailedOptions.DisplayedOptions.Clear();
                printDetailedOptions.DisplayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies);

                // Create a new list option.
                PrintCustomItemListOptionDetails photoSize = printDetailedOptions.CreateItemListOption("photoSize", "Photo Size");
                photoSize.AddItem("SizeFullPage", "Full Page");
                photoSize.AddItem("Size8x10", "8 x 10 in");

                // Add the custom option to the option list.
                printDetailedOptions.DisplayedOptions.Add("photoSize");

                PrintCustomItemListOptionDetails scaling = printDetailedOptions.CreateItemListOption("scaling", "Scaling");
                scaling.AddItem("ShrinkToFit", "Shrink To Fit");

                // Add the custom option to the option list.
                printDetailedOptions.DisplayedOptions.Add("scaling");

                // Set default orientation to landscape.
                printTask.Options.Orientation = PrintOrientation.Portrait;

                // Register for print task option changed notifications.
                //printDetailedOptions.OptionChanged += PrintDetailedOptionsOptionChanged;

                // Register for print task Completed notification.
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    await _scenarioPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        ClearPageCollection();

                        // Reset image options to default values.
                        this.photoScale = Scaling.ShrinkToFit;
                        this.photoSize = PhotoSize.SizeFullPage;

                        // Reset the current page description
                        currentPageDescription = null;

                        // Notify the user when the print operation fails.
                        if (args.Completion == PrintTaskCompletion.Failed)
                        {
                            _logHelper.Log(LogLevel.Error, "Failed to print.");
                            await Utilities.ShowSingleButtonContentDialogAsync("Failed to print.");
                        }
                    });
                };

                // Set the document source.
                sourceRequestedArgs.SetSource(_printDocumentSource);
            });
        }

        /// <summary>
        /// This is the event handler for Pagination.
        /// </summary>
        /// <param name="sender">The document for which pagination occurs.</param>
        /// <param name="e">The pagination event arguments containing the print options.</param>
        protected override void CreatePrintPreviewPages(object sender, Windows.UI.Xaml.Printing.PaginateEventArgs e)
        {

            PrintDocument printDoc = (PrintDocument)sender;

            // A new "session" starts with each paginate event.
            Interlocked.Increment(ref requestCount);

            PageDescription pageDescription = new PageDescription();

            // Get printer's page description.
            PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(e.PrintTaskOptions);
            PrintPageDescription printPageDescription = e.PrintTaskOptions.GetPageDescription(0);

            // Reset the error state
            printDetailedOptions.Options["photoSize"].ErrorText = string.Empty;

            // Compute the printing page description (page size & center printable area)
            pageDescription.PageSize = printPageDescription.PageSize;

            pageDescription.Margin.Width = Math.Max(
                printPageDescription.ImageableRect.Left,
                printPageDescription.ImageableRect.Right - printPageDescription.PageSize.Width);

            pageDescription.Margin.Height = Math.Max(
                printPageDescription.ImageableRect.Top,
                printPageDescription.ImageableRect.Bottom - printPageDescription.PageSize.Height);

            pageDescription.ViewablePageSize.Width = printPageDescription.PageSize.Width - pageDescription.Margin.Width * 2;
            pageDescription.ViewablePageSize.Height = printPageDescription.PageSize.Height - pageDescription.Margin.Height * 2;

            // Compute print photo area in pixels.
            switch (photoSize)
            {
                case PhotoSize.Size8x10:
                    pageDescription.PictureViewSize.Width = 8 * DPI96;
                    pageDescription.PictureViewSize.Height = 10 * DPI96;
                    break;
                case PhotoSize.SizeFullPage:
                    pageDescription.PictureViewSize.Width = pageDescription.ViewablePageSize.Width;
                    pageDescription.PictureViewSize.Height = pageDescription.ViewablePageSize.Height;
                    break;
            }

            // Try to maximize photo-size based on it's aspect-ratio
            if ((pageDescription.ViewablePageSize.Width > pageDescription.ViewablePageSize.Height) && (photoSize != PhotoSize.SizeFullPage))
            {
                var swap = pageDescription.PictureViewSize.Width;
                pageDescription.PictureViewSize.Width = pageDescription.PictureViewSize.Height;
                pageDescription.PictureViewSize.Height = swap;
            }

            pageDescription.IsContentCropped = false;

            // Recreate content only when :
            // - there is no current page description
            // - the current page description doesn't match the new one
            if (currentPageDescription is null || !currentPageDescription.Equals(pageDescription))
            {
                ClearPageCollection();

                if (pageDescription.PictureViewSize.Width > pageDescription.ViewablePageSize.Width ||
                    pageDescription.PictureViewSize.Height > pageDescription.ViewablePageSize.Height)
                {
                    printDetailedOptions.Options["photoSize"].ErrorText = "Photo doesn’t fit on the selected paper";

                    // Inform preview that it has only 1 page to show.
                    printDoc.SetPreviewPageCount(1, PreviewPageCountType.Intermediate);

                    // Add a custom "preview" unavailable page
                    lock (printSync)
                    {
                        pageCollection[0] = new PreviewUnavailable(pageDescription.PageSize, pageDescription.ViewablePageSize);
                    }
                }
                else
                {
                    // Inform preview that is has #NumberOfPhotos pages to show.
                    printDoc.SetPreviewPageCount(NumberOfPhotos, PreviewPageCountType.Intermediate);
                }

                currentPageDescription = pageDescription;
            }
        }

        /// <summary>
        /// Generates a page containing a photo.
        /// The image will be rotated if detected that there is a gain from that regarding size (try to maximize photo size).
        /// </summary>
        /// <param name="photoNumber">The photo number.</param>
        /// <param name="pageDescription">The description of the printer page.</param>
        /// <returns>A task that will return the page.</returns>
        private UIElement GeneratePage(int photoNumber, PageDescription pageDescription)
        {
            Canvas page = new Canvas
            {
                Width = pageDescription.PageSize.Width,
                Height = pageDescription.PageSize.Height
            };

            Canvas viewablePage = new Canvas()
            {
                Width = pageDescription.ViewablePageSize.Width,
                Height = pageDescription.ViewablePageSize.Height
            };

            viewablePage.SetValue(Canvas.LeftProperty, pageDescription.Margin.Width);
            viewablePage.SetValue(Canvas.TopProperty, pageDescription.Margin.Height);

            // The image "frame" which also acts as a view port
            Grid photoView = new Grid
            {
                Width = pageDescription.PictureViewSize.Width,
                Height = pageDescription.PictureViewSize.Height
            };

            // Center the frame.
            photoView.SetValue(Canvas.LeftProperty, (viewablePage.Width - photoView.Width) / 2);
            photoView.SetValue(Canvas.TopProperty, (viewablePage.Height - photoView.Height) / 2);

            //// Return an async task that will complete when the image is fully loaded.
            //WriteableBitmap bitmap = await LoadBitmapAsync(
            //    new Uri(string.Format("ms-appx:///Assets/photo{0}.jpg", photoNumber)),
            //    pageDescription.PageSize.Width > pageDescription.PageSize.Height);

            //if (bitmap != null)
            //{
            //    Image image = new Image
            //    {
            //        Source = bitmap,
            //        HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
            //        VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center
            //    };

            //    // Use the real image size when cropping or if the image is smaller then the target area (prevent a scale-up).
            //    if (photoScale == Scaling.Crop ||
            //        (bitmap.PixelWidth <= pageDescription.PictureViewSize.Width &&
            //        bitmap.PixelHeight <= pageDescription.PictureViewSize.Height))
            //    {
            //        image.Stretch = Stretch.None;
            //        image.Width = bitmap.PixelWidth;
            //        image.Height = bitmap.PixelHeight;
            //    }

            //    // Add the newly created image to the visual root.
            //    photoView.Children.Add(image);
            //    viewablePage.Children.Add(photoView);
            //    page.Children.Add(viewablePage);
            //}

            // Return the page with the image centered.
            return page;
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print page preview,
        /// in the form of an UIElement, to an instance of PrintDocument.
        /// PrintDocument subsequently converts the UIElement into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">The print document.</param>
        /// <param name="e">Arguments containing the requested page preview.</param>
        protected override void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            // Store a local copy of the request count to use later to determine if the computed page is out of date.
            // If the page preview is unavailable an async operation will generate the content.
            // When the operation completes there is a chance that a pagination request was already made therefore making this page obsolete.
            // If the page is obsolete throw away the result (don't call SetPreviewPage) since a new GetPrintPreviewPage will server that request.
            long requestNumber = 0;
            Interlocked.Exchange(ref requestNumber, requestCount);
            int pageNumber = e.PageNumber;

            UIElement page;
            bool pageReady = false;

            // Try to get the page if it was previously generated.
            lock (printSync)
            {
                pageReady = pageCollection.TryGetValue(pageNumber - 1, out page);
            }

            if (!pageReady)
            {
                // The page is not available yet.
                page = GeneratePage(pageNumber, currentPageDescription);

                // If the ticket changed discard the result since the content is outdated.
                if (Interlocked.CompareExchange(ref requestNumber, requestNumber, requestCount) != requestCount)
                {
                    return;
                }

                // Store the page in the list in case an invalidate happens but the content doesn't need to be regenerated.

                lock (printSync)
                {
                    pageCollection[pageNumber - 1] = page;

                    //// Add the newly created page to the printing root which is part of the visual tree and force it to go
                    //// through layout so that the linked containers correctly distribute the content inside them.
                    //PrintCanvas.Children.Add(page);
                    //PrintCanvas.InvalidateMeasure();
                    //PrintCanvas.UpdateLayout();
                }
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Send the page to preview.
            printDoc.SetPreviewPage(pageNumber, page);
        }

        /// <summary>
        /// Helper function that clears the page collection and also the pages attached to the "visual root".
        /// </summary>
        private void ClearPageCollection()
        {
            lock (printSync)
            {
                pageCollection.Clear();
            }
        }


    }
    */

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ToolsPage : Page
    {
        private static readonly ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
        private static readonly LogHelper _logHelper = new LogHelper(log);


        private ToolsViewModel _toolsViewModel = ToolsViewModel.Instance;
        private ICS309ViewModel Ics309ViewModel = ICS309ViewModel.Instance;

        StorageFile _selectedFile;
        PivotItem _currentPivotItem;

        private PrintHelper _printHelper;
        

        public ToolsPage()
        {
            this.InitializeComponent();
#if !DEBUG
            testReceive.Header = "";
#endif
            _toolsViewModel.ToolsPagePivot = toolsPagePivot;
            //_toolsViewModel.commLogEntryCollection = new ObservableCollection<CommLogEntry>();
            //ics309DataGrid.Columns[0].SortDirection = DataGridSortDirection.Descending;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PrintManager.IsSupported())
            {
                Ics309ViewModel.ICS309PrintButtonVisible = true;
            }
            else
            {
                // Remove the print button
                Ics309ViewModel.ICS309PrintButtonVisible = false;
            }

            // Printing-related event handlers will never be called if printing
            // is not supported, but it's okay to register for them anyway.

            // Initialize common helper class and register for printing
            //printHelper = new ICS309PrintHelper(this);
            //printHelper.RegisterForPrinting();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            base.OnNavigatedFrom(e);
        }

        public static object GetDynamicSortProperty(object item, string propName)
        {
            //Use reflection to get order type
            return item.GetType().GetProperty(propName).GetValue(item);
        }

        public static List<T> Sort_List<T>(List<T> data)
        {
            List<T> data_sorted = new List<T>();

            data_sorted = (from n in data
                           orderby GetDynamicSortProperty(n, "Time") ascending
                           select n).ToList();
            return data_sorted;
        }

        private async Task UpdateFileListAsync()
        {
            List<string> fileTypeFilter = new List<string>() { ".log" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the user's archive folder
            StorageFileQueryResult results = SharedData.MetroLogsFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();

            var observableCollection = new ObservableCollection<StorageFile>(files);
            LogFilesCollection.Source = observableCollection.OrderByDescending(f => f.Name);

            //logFilesComboBox.SelectedIndex = _selectedFileIndex;
        }

        private async void LogFilesComboBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedFile = (StorageFile)e.AddedItems[0];
                string temp = await FileIO.ReadTextAsync(_selectedFile);
                temp = temp.Replace("\r\r\n", "\r\n");
                temp = temp.Replace('\0', ' ');
                logFileTextBox.Text = temp;
            }
            catch (UnauthorizedAccessException)
            {
                StorageFile fileCopy;
                try
                {
                    // Delete any file copy that for some reason was not deleted
                    fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                    await fileCopy.DeleteAsync();
                }
                catch
                { }
                // Create a copy of an open log file because it can not be read directly
                await _selectedFile.CopyAsync(SharedData.MetroLogsFolder, _selectedFile.Name + "-Copy");
                fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                logFileTextBox.Text = await FileIO.ReadTextAsync(fileCopy);
                await fileCopy.DeleteAsync();
            }
            catch (COMException)
            {

            }
        }

        //packetMsg.MessageBody = "
        //Message #1 \r\n
        //Date: Mon, 01 Feb 2016 12:29:03 PST\r\n
        //From: kz6dm @w3xsc.ampr.org\r\n
        //To: kz6dm\r\n
        //Subject: 6DM-907P_O/R_ICS213_Check-in\r\n
        //!PACF!6DM-907P_O/R_ICS213_Check-in\r\n
        //# EOC MESSAGE FORM \r\n
        //# JS-ver. PR-4.1-3.1, 01/19/17,\r\n
        //# FORMFILENAME: Message.html\r\n
        //MsgNo: [6DM - 907P]\r\n
        //1a.: [02/01/2016]\r\n
        //1b.: [1219]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n7.: [Operations]\r\n9a.: [MTVEOC]\r\n8.: [Operations]\r\n9b.: [HOSECM]\r\n10.: [Check-in]\r\n12.: [\\nMonday Check-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: []\r\nOpTime: []\r\n# EOF\r\n";

        // read data
        //!PACF!6DM - 681P_O / R_ICS213_ghjhgj
        //# EOC MESSAGE FORM 
        //# JS-ver. PR-4.1-3.1, 01/19/17
        //# FORMFILENAME: Message.html
        //MsgNo: [6DM - 681P]
        //1a.: [03/01/18]
        //1b.: [1650]
        //4.: [OTHER]
        //5.: [ROUTINE]
        //9a.: [gfhjgfj]
        //9b.: [gfhjgfhj]
        //10.: [ghjhgj]
        //12.: [\nghjghj]
        //Rec-Sent: [sent]
        //Method: [Other]
        //Other: [Packet]
        //OpCall: [KZ6DM]
        //OpName: [Poul Hansen]
        //OpDate: [03/01/2018]
        //OpTime: [1652]
        //#EOF

        private async void ConvertToForm_Click(object sender, RoutedEventArgs e)
        {

            // Find type
            // do a navigate to forms with index and form id (maybe file name}
            bool success = DateTime.TryParse(messageReceivedTime.Text, out DateTime receivedTime);
            PacketMessage message = new PacketMessage()
            {
                ReceivedTime = success ? receivedTime : (DateTime?)null,
                MessageNumber = Utilities.GetMessageNumberPacket(),
                TNCName = "",
            };

            message.MessageBody = $"Date: {message.ReceivedTime}\r\n";
            message.MessageBody += $"From: {messageTo.Text}\r\n";
            message.MessageBody += $"To: {messageFrom.Text}\r\n";
            message.MessageBody += $"Subject: {messageSubject.Text}\r\n";
            message.MessageBody += PacFormText.Text;
            CommunicationsService communicationsService = new CommunicationsService();
            await communicationsService.CreatePacketMessageFromMessageAsync(message);
        }

        private async void ConvertFromBase64_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(PacFormText.Text))
                {
                    byte[] message = Convert.FromBase64String(PacFormText.Text);
                    string decodedString = Encoding.UTF8.GetString(message);
                    PacFormText.Text = decodedString;
                }
            }
            catch (FormatException fe)
            {
                await ContentDialogs.ShowSingleButtonContentDialogAsync(fe.Message);
            }
        }

        //private string FormatDateTime(DateTime dateTime)
        //{
        //    return $"{dateTime.Month:d2}/{dateTime.Day:d2}/{dateTime.Year - 2000:d2} {dateTime.Hour:d2}:{dateTime.Minute:d2}";
        //}

        private async void ToolsPagePivot_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            _currentPivotItem = (PivotItem)e.AddedItems[0];

            if (_currentPivotItem.Name == "logFile")
            {
                await UpdateFileListAsync();
            }
            else if (_currentPivotItem.Name == "testReceive")
            {
#if DEBUG
                await UpdateTestFileListAsync();
#endif
            }
            else if (_currentPivotItem.Name == "ics309")
            {
                Ics309ViewModel.Initialize();
                Ics309ViewModel.DateTimePrepared = DateTimeStrings.DateTimeString(DateTime.Now);
            }
        }

#region ICS309

        private async void AppBarPrintICS309_ClickAsync(object sender, RoutedEventArgs e)
        {
            DirectPrintContainer.Children.Remove(PrintableContent);

            _printHelper = new PrintHelper(Container);
            _printHelper.AddFrameworkElementToPrint(PrintableContent);

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            // Create a new PrintHelperOptions instance

            await _printHelper.ShowPrintUIAsync("ICS 309");
        }

        private void ReleasePrintHelper()
        {
            _printHelper.Dispose();

            if (!DirectPrintContainer.Children.Contains(PrintableContent))
            {
                DirectPrintContainer.Children.Add(PrintableContent);
            }
        }

        private void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();            
        }

        private void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();

            _logHelper.Log(LogLevel.Error, $"Print failed. {Ics309ViewModel.OperationalPeriod}");
        }

        private void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }


#endregion ICS309

#region TestScenarios
#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType">.txt or .log</param>
        /// <returns></returns>
        private async Task UpdateTestFileListAsync()
        {
            List<string> fileTypeFilter = new List<string>() { ".txt" };
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypeFilter);

            // Get the files in the user's archive folder
            StorageFolder storageFolder = SharedData.TestFilesFolder;
            StorageFileQueryResult results = storageFolder.CreateFileQueryWithOptions(queryOptions);
            // Iterate over the results
            IReadOnlyList<StorageFile> files = await results.GetFilesAsync();
            if (files.Count > 0)
            {
                var observableCollection = new ObservableCollection<StorageFile>(files);
                TestFilesCollection.Source = observableCollection.OrderByDescending(f => f.Name);

                //comboBoxTestFiles.SelectedIndex = _selectedTestFileIndex;
            }
        }
#endif

        private async void TestFilesComboBox_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedFile = (StorageFile)e.AddedItems[0];
                receivedMessage.Text = await FileIO.ReadTextAsync(_selectedFile);
            }
            catch (UnauthorizedAccessException)
            {
                StorageFile fileCopy;
                try
                {
                    // Delete any file copy that for some reason was not deleted
                    fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                    await fileCopy.DeleteAsync();
                }
                catch
                { }
                // Create a copy of an open log file because it can not be read directly
                await _selectedFile.CopyAsync(SharedData.MetroLogsFolder, _selectedFile.Name + "-Copy");
                fileCopy = await StorageFile.GetFileFromPathAsync(_selectedFile.Path + "-Copy");
                logFileTextBox.Text = await FileIO.ReadTextAsync(fileCopy);
                await fileCopy.DeleteAsync();
            }
            catch (COMException)
            {

            }
        }

        private void TestReceivedMessage_Click(object sender, RoutedEventArgs e)
        {
            bool TestWithoutConnecting = true;
            if (TestWithoutConnecting)
            {
                DateTime dateTime = DateTime.Now;
                int toIndex = receivedMessage.Text.IndexOf("To:");
                int lineEnd = receivedMessage.Text.IndexOf("\r", toIndex);
                string toLine = receivedMessage.Text.Substring(toIndex, lineEnd - toIndex);
                string[] toArray = toLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string area = "";
                if (toArray[1].ToLower().Contains("xscperm"))
                    area = "xscperm";
                else if (toArray[1].ToLower().Contains("xscevent"))
                    area = "xscevent";
                PacketMessage packetMsg = new PacketMessage()
                {
                    MessageBody = receivedMessage.Text,
                    ReceivedTime = dateTime,
                    BBSName = BBSDefinitions.Instance.BBSDataArray[2].Name,
                    TNCName = TNCDeviceArray.Instance.TNCDeviceList[1].Name,
                    MessageNumber = Helpers.Utilities.GetMessageNumberPacket(true),
                    Area = area,
                    MessageSize = receivedMessage.Text.Length,
                };

                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 24 Aug 2015 21:07:37 PDT\r\nFrom: kz6dm @w1xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM - 483_O/R_CityScan_Mountain View Emergency Declared: no\r\n\r\n!PACF!6DM - 483_O/R_CityScan_Mountain View Emergency Declared: no\r\n# CITY-SCAN UPDATE FLASH REPORT \r\n# JS-ver. PR-4.1-3.9, 01/11/15\r\n# FORMFILENAME: city-scan.html\r\nMsgNo: [6DM-483]\r\nD.: [OTHER]\r\nE.: [ROUTINE]\r\n1a.: [Mountain View]\r\n2.: [08/24/2015]\r\n3.: [2028]\r\n4.: [Poul Hansen]\r\n6.: [000-000-0000]\r\n9.: [no]\r\nOpDate: [08/24/2015]\r\nOpTime: [2030]\r\n#EOF\r\n";

                // Message
                //packetMsg.MessageSize = 30;
                //packetMsg.MessageBody = "Message #3 \r\nDate: Sun, 30 Aug 2015 12:42:30 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-185P:\r\nThis is a test message.\r\nNext line\r\n";

                //packetMsg.MessageSize = 30;
                //packetMsg.MessageBody = "Message #3 \r\nDate: Mon, 07 Sep 2015 20:40:57 PDT\r\nFrom: pktmon @w2xsc.ampr.org\r\nTo: k6taa @w3xsc\r\nCc: KI6SEP @W4XSC, KI6SEP@W3XSC, KZ6DM @W3XSC, WB6PVU@W3XSC, K6DLC @W2XSC,\r\n      KI6PUR@W3XSC, SNYEOC @W3XSC, AA6WK@W2XSC, KG6HI @W3XSC\r\nSubject: POV-5470: _O/R_SCCo ARES / RACES Packet Check-in Report For: Monday, September 07, 2015.Total = 18 call signs / 23 check - ins";

                //packetMsg.MessageBody = "Message #5 \r\nDate: Sun, 04 Oct 2015 14:47:44 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-571P_O/R_ICS213_Check-in 10/04/15-KZ6DM-Poul-Mountain View\r\n\r\n!PACF! 6DM-571P_O/R_ICS213_Check-in 10/04/15-KZ6DM-Poul-Mountain View\r\n# EOC MESSAGE FORM \r\n# JS-ver. PR-4.1-2.9, 01/11/15, \r\n# FORMFILENAME: Message.html \r\nMsgNo: [6DM-571P]\r\n1a.: [10/04/2015]\r\n1b.: [1444]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n6c.: [checked]\r\n7.: [Planning]\r\n9a.: [MTVEOC]\r\n8.: [Planning]\r\n9b.: [Radio]\r\n10.: [Check-in 10/04/15 - KZ6DM - Poul - Mountain View]\r\n12.: [\nCheck-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: [10/04/2015]\r\nOpTime: [1446]\r\n#EOF";
                //packetMsg.MessageBody = "Message #5 \r\nDate: Sun, 04 Oct 2015 14:47:44 PDT\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-571P_O/R_ICS213_Check-in 10/04/15 - KZ6DM - Poul - Mountain View\r\n\r\n!PACF!1234_O/P_ICS213_Damage Summary for Rex Manor subject 2\r\n# EOC MESSAGE FORM \r\n# JS-ver. MV/PR-4.2.4-2.18, 08/21/15\r\n# FORMFILENAME: CERT-DA-MTVUniversal-message.html\r\nMsgNo: [1234]\r\n1a.: [10/05/2015]\r\n1b.: [09:27]\r\n4.: [OTHER]\r\n5.: [PRIORITY]\r\n6a.: [No]\r\n6b.: [No]\r\n7.: [Planning]\r\n9a.: [Mountain View EOC]\r\n8.: [Planning]\r\n9b.: [Rex Manor]\r\n10.: [Damage Summary for Rex Manor subject 2]\r\n12.: [\n  F1  G2  W3  E4  C5  L6  Mod7  H8  I9  D10  T11 Mor12  A13  N14  O15  Nei16\nOther means other]\r\nRec-Sent: [Sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: [10/05/2015]\r\nOpTime: [09:29]\r\n# EOF";

                //packetMsg.MessageSize = 893;
                //packetMsg.MessageBody = "Message #1\r\nDate: Sun, 08 Nov 2015 13:44:51 PST\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-289P_O/R_EOC Logistics Req_Mountain View\r\n\r\rn!PACF!6DM-289P_O/R_EOC Logistics Req_Mountain View\r\n# JS:EOC Logistics Request (which4)\r\n# JS-ver. PR-4.1-2.5, 01/11/15\r\n# FORMFILENAME: EOCLogisticsRequest.html\r\n1: [6DM-289P]\r\n5: [true]\r\n8: [true]\r\n11: [true]\r\n13: [Check-in]\r\n14: [Poul Hansen]\r\n16: [Display List of Cities]\r\n17: [Mountain View]\r\n19: [Mountain View}10]\r\n\r\n28: [true]\r\n36: [\nCheck-in 11/08/15 - KZ6DM]\r\n37: [Jerry]\r\n39: [true]\r\n41: [true]\r\n42: [KZ6DM]\r\n43: [Poul Hansen]\r\n44: [11/08/2015{odate]\r\n45: [13:43{otime]\r\n# EOF";

                //packetMsg.MessageSize = 652;
                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 23 Nov 2015 19:02:04 PST\r\nFrom: pktmon @w2xsc.ampr.org\r\nTo: kz6dm @w3xsc.ampr.org\r\nSubject: DELIVERED:	6DM-648P_O/R_EOC Logistics Req_Mountain View\r\n\r\n!LMI!POV-5932P!DR!11/23/2015 6:59:22 PM\r\nYour Message\r\nTo: PKTMON\r\nSubject: 6DM-648P_O/R_EOC Logistics Req_Mountain View\r\nwas delivered on 11/23/2015 6:59:22 PM\r\nRecipient's Local Message ID: POV-5932P";

                //packetMsg.MessageBody = "Message #1 \r\nDate: Mon, 01 Feb 2016 12:29:03 PST\r\nFrom: kz6dm @w3xsc.ampr.org\r\nTo: kz6dm\r\nSubject: 6DM-907P_O/R_ICS213_Check-in\r\n!PACF!6DM-907P_O/R_ICS213_Check-in\r\n# EOC MESSAGE FORM \r\n# JS-ver. PR-4.1-3.1, 01/19/17,\r\n# FORMFILENAME: Message.html\r\nMsgNo: [6DM - 907P]\r\n1a.: [02/01/2016]\r\n1b.: [1219]\r\n4.: [OTHER]\r\n5.: [ROUTINE]\r\n7.: [Operations]\r\n9a.: [MTVEOC]\r\n8.: [Operations]\r\n9b.: [HOSECM]\r\n10.: [Check-in]\r\n12.: [\\nMonday Check-in]\r\nRec-Sent: [sent]\r\nMethod: [Other]\r\nOther: [Packet]\r\nOpCall: [KZ6DM]\r\nOpName: [Poul Hansen]\r\nOpDate: []\r\nOpTime: []\r\n# EOF\r\n";

                //				packetMsg.MessageSize = 4002;
                //				packetMsg.MessageBody = @"Message #4 
                //Date: Sun, 06 Mar 2016 18:00:48 PST
                //From: xsceoc@w1xsc.ampr.org
                //To: xscperm
                //Subject: SCCo Packet Tactical Calls v160306

                //# Primary Tactical Calls and BBSs for Santa Clara County Cities/Agencies
                //#=======================================================================
                //# Last revised:  06-Mar-2016 at 17:17 by Michael Fox, N6MEF
                //#
                //# IMPORTANT:  Post a copy of this file in your radio room and retain a copy on
                //# your packet computer. The suggested location is the Outpost Archive folder.
                //#
                //# BBS Naming/Addressing:
                //#  BBS Call Sign:    W#XSC  (where # is 1-6)     (Ex: w1xsc, w2xsc, ...)
                //#  AX.25 connect:    <BBScall>-1                 (Ex: connect w1xsc-1)
                //#  AMPRnet/Internet: <BBScall>.ampr.org          (Ex: w2xsc.ampr.org)	
                //#  BBS Network:      <BBScall>.#nca.ca.usa.noam  (Ex: w3xsc.#nca.ca.usa.noam)
                //#
                //# Usage:
                //# -- All users:  connect to the primary BBS for your city/agency.  If the
                //#    primary is down, connect to the secondary.
                //# -- Individuals:  connect using your FCC call sign
                //# -- Cities/Agencies:  connect using a tactical call assigned by your EC/CRO
                //#
                //# Each line in the table below contains:
                //# 1)  Primary tactical call sign for the agency (typically for the DOC/EOC)
                //# 2)  Agency name
                //# 3)  3-letter tactical prefix assigned to agency
                //# 4)  Primary BBS name (agency normally connects to and receives mail here)
                //# 5)  Secondary BBS name (agency connects here if primary BBS is down)
                //#
                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	-----------------------------	---	------	------
                //ARCEOC		American Red Cross		ARC	W1XSC	W4XSC
                //SCUCIC		Cal-Fire VIPs Santa Clara Unit	SCU	W2XSC	W1XSC
                //CBLEOC		Campbell, City of		CBL	W1XSC	W4XSC
                //CUPEOC		Cupertino, City of		CUP	W1XSC	W4XSC
                //GILEOC		Gilroy, City of			GIL	W2XSC	W1XSC
                //HOSDOC		Hospitals, all SCCo		HOS	W2XSC	W1XSC
                //LMPEOC		Loma Prieta Region		LMP	W2XSC	W1XSC
                //LOSEOC		Los Altos, City of		LOS	W3XSC	W1XSC
                //LAHEOC		Los Altos Hills, Town of	LAH	W3XSC	W1XSC
                //LGTEOC		Los Gatos, Town of		LGT	W1XSC	W4XSC
                //MLPEOC		Milpitas, City of		MLP	W4XSC	W1XSC
                //MSOEOC		Monte Sereno, City of		MSO	W1XSC	W4XSC
                //MRGEOC		Morgan Hill, City of		MRG	W2XSC	W1XSC
                //MTVEOC		Mountain View, City of		MTV	W3XSC	W1XSC
                //NAMEOC		NASA - Ames			NAM	W3XSC	W1XSC
                //PAFEOC		Palo Alto, City of		PAF	W3XSC	W1XSC
                //SJCEOC		San Jose, City of		SJC	W1XSC	W2XSC
                //SJWEOC		San Jose Water Co		SJW	W1XSC	W2XSC
                //SNCEOC		Santa Clara, City of		SNC	W1XSC	W2XSC
                //XSCEOC		Santa Clara County		XSC	W1XSC	W4XSC
                //VWDEOC		SC Valley Water District	VWD	W1XSC	W2XSC
                //SAREOC		Saratoga, City of		SAR	W1XSC	W4XSC
                //STUEOC		Stanford University		STU	W4XSC	W1XSC
                //SNYEOC		Sunnyvale, City of		SNY	W1XSC	W3XSC
                //#
                //# Each of the above agencies also has ten (10) numbered tactical call signs
                //# which begin with their assigned prefix and end with the numbers 001 - 010.
                //#
                //# Each of the above agencies may also have additional tactical call signs,
                //# defined by the EC/CRO, which begin with the agency's assigned prefix.
                //#
                //#
                //# Primary Tactical Calls for Other (non-SCCo) Agencies
                //#=====================================================
                //#
                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	--------------------------	---	------	------
                //COSEOC		CalEMA - Coastal Region		COS	W1XSC
                //XALEOC		Alameda County			XAL	W4XSC
                //XCCEOC		Contra Costa County		XCC	W1XSC
                //XMREOC		Marin County			XMR	W4XSC
                //XMYEOC		Monterey County			XMY	W2XSC
                //XBEEOC		San Benito County		XBE	W2XSC
                //XSFEOC		San Francisco County		XSF	W4XSC
                //XSMEOC		San Mateo County		XSM	W4XSC
                //XCZEOC		Santa Cruz County		XCZ	W2XSC
                //#
                //#
                //# Other Special Tactical Prefixes
                //#================================
                //#
                //#Tactical	Function			Pfx	Pri	Sec
                //#--------	--------------------------	---	------	------
                //PKT***		SCCo Packet Net Check-ins	PKT	W2XSC	W4XSC
                //#
                //#
                //#======== End Of Primary Tactical Calls ===================
                //		";

                ///*				packetMsg.MessageBody = @"Message #4 
                //Date: Sun, 06 Mar 2016 18:00:48 PST
                //From: xsceoc@w1xsc.ampr.org
                //To: xscperm
                //Subject: SCCo Packet Tactical Calls v160306

                //#Tactical	Agency Name			Pfx	Pri	Sec
                //#--------	-----------------------------	---	------	------
                //ARCEOC		American Red Cross		ARC	W1XSC	W4XSC
                //SCUCIC		Cal-Fire VIPs Santa Clara Unit	SCU	W2XSC	W1XSC
                //				";
                //*/

                //                packetMsg.MessageBody = @"Message #1 
                //Date: Wed, 20 Sep 2017 20:43:47 PDT
                //From: kz6dm @w3xsc.ampr.org
                //To: kz6dm
                //Subject: 6DM-264P_O/R_EOC213RR_Incident Name

                //!PACF! 6DM-264P_O/R_EOC213RR_Incident Name
                //# JS:EOC Resource Request (which4)
                //# JS-ver. PR-4.3-2.8, 09/15/17
                //# FORMFILENAME: XSC_EOC-213RR_v1706.html
                //1: [6DM-264P]
                //6: [true]
                //9: [true]
                //13: [\nIncident Name]
                //14: [09/25/2017]
                //15: [16:10]
                //16: [\nRequested by]
                //17: [\nPrepared by]
                //18: [\nApproved by]
                //19: [\n1]
                //20: [\nDescription]
                //21: [\nArrival]
                //25: [true]
                //26: [\n20]
                //27: [\nDelivery to]
                //28: [\nLocvation]
                //29: [\nSubstitutes]
                //32: [true]
                //33: [Propane]
                //34: [true]
                //39: [\nInstructions]
                //41: [true]
                //43: [true]
                //44: [KZ6DM]
                //45: [Poul Hansen]
                //46: [09/25/2017{odate]
                //47: [16:10{otime]
                //#EOF

                //";
                CommunicationsService communicationsService = new CommunicationsService();
                communicationsService._packetMessagesReceived.Add(packetMsg);
                communicationsService.ProcessReceivedMessagesAsync();
            }
            else
            {
                //BBSConnect();
            }

        }

        private void TestDeliveredMessage_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            PacketMessage packetMsg = new PacketMessage()
            {
                MessageBody = receivedMessage.Text,
                ReceivedTime = dateTime,
                BBSName = BBSDefinitions.Instance.BBSDataArray[2].Name,
                TNCName = TNCDeviceArray.Instance.TNCDeviceList[1].Name,
                MessageNumber = Helpers.Utilities.GetMessageNumberPacket(true),
                Area = "",
                MessageSize = receivedMessage.Text.Length,
            };

            List<PacketMessage> packetMessagesReceived = new List<PacketMessage>();
            BBSData bbs = PacketSettingsViewModel.Instance.BBSFromSelectedProfile;
            TNCDevice tncDevice = PacketSettingsViewModel.Instance.TNCFromSelectedProfile;
            //bbs?.ConnectName, ref tncDevice, bool forceReadBulletins, string areas, ref List<PacketMessage> packetMessagesToSend
            packetMessagesReceived.Add(packetMsg);
            TNCInterface tncInterface = new TNCInterface();
            string tncName = PacketSettingsViewModel.Instance.CurrentProfile.TNC;
            //TNCDevice tncDevice = TNCDeviceArray.Instance.TNCDeviceList.Where(tnc => tnc.Name == tncName).FirstOrDefault();
            string messageBBS = PacketSettingsViewModel.Instance.CurrentProfile.BBS;
            tncInterface.SendMessageReceipts(messageBBS, ref tncDevice, "", packetMessagesReceived);
            foreach (PacketMessage packetMessage in tncInterface.PacketMessagesSent)
            {
                packetMessage.Save(SharedData.SentMessagesFolder.Path);
            }
        }

        private async void AppBarButtonTest_SaveFileAsync(object sender, RoutedEventArgs e)
        {
#if DEBUG
            // Make sure the extension is .txt
            int index = textBoxTestFileName.Text.IndexOf('.');
            if (index < 0)
            {
                textBoxTestFileName.Text += ".txt";
            }
            else if (textBoxTestFileName.Text.Substring(index) != ".txt")
            {
                textBoxTestFileName.Text = textBoxTestFileName.Text.Substring(0, index) + ".txt";
            }
            textBoxTestFileName.Text = textBoxTestFileName.Text.Replace('/', '_');
            StorageFile file = await SharedData.TestFilesFolder.CreateFileAsync(textBoxTestFileName.Text, CreationCollisionOption.ReplaceExisting);
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, receivedMessage.Text);
            }
            else
            {
                return;
            }
#endif
        }
#endregion
        private async void AppBarButtonTest_OpenFileAsync(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (await SharedData.TestFilesFolder.TryGetItemAsync(textBoxTestFileName.Text) is StorageFile file)
            {
                receivedMessage.Text = await FileIO.ReadTextAsync(file);
            }
            else
            {
                return;
            }
#endif
        }

        private async void AppBarButton_OpenFileAsync(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync(textBoxTestFileName.Text);
            if (file != null)
            {
                receivedMessage.Text = await FileIO.ReadTextAsync(file);
            }
            else
            {
                return;
            }
        }

        private async void AppBarButton_DeleteLogFileAsync()
        {
                StorageFile deleteFile = logFilesComboBox.SelectedItem as StorageFile;

                await deleteFile.DeleteAsync();

                await UpdateFileListAsync();
        }

        private async void AppBarButton_DeleteFileAsync(object sender, RoutedEventArgs e)
        {
            StorageFile fileToDelete;
            switch (_currentPivotItem.Name)
            {
#if DEBUG
                case "testReceive":
                    fileToDelete = comboBoxTestFiles.SelectedItem as StorageFile;
                    await fileToDelete.DeleteAsync();
                    await UpdateTestFileListAsync();
                    break;
#endif
                case "logFile":
                    fileToDelete = logFilesComboBox.SelectedItem as StorageFile;
                    await fileToDelete.DeleteAsync();
                    await UpdateFileListAsync();
                    break;
            }
        }

    }

}
