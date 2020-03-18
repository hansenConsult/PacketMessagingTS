using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace MessageFormControl
{
    public class PrintHelper
    {
        /// <summary>
        /// Occurs when a print was successful.
        /// </summary>
        public event Action OnPrintSucceeded;

        /// <summary>
        /// Occurs when a print fails.
        /// </summary>
        public event Action OnPrintFailed;

        /// <summary
        /// Occurs when a print is canceled by the user.
        /// </summary>
        public event Action OnPrintCanceled;

        /// <summary>
        /// Occurs after print preview pages are generated.
        /// </summary>
        /// <remarks>
        /// You can use this event to tweak the final rendering by adding/moving controls in the page.
        /// </remarks>
        public event Action<List<FrameworkElement>> OnPreviewPagesCreated;

        /// <summary>
        /// The percent of app's margin width, content is set at .... of the area's width
        /// </summary>
        protected double ApplicationContentMarginLeft = 0.03;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        protected double ApplicationContentMarginTop = 0.03;
        
        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        protected PrintDocument _printDocument;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        protected IPrintDocumentSource _printDocumentSource;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<FrameworkElement> _printPreviewPages;

        /// <summary>
        /// A hidden canvas used to hold pages we wish to print.
        /// </summary>
        private Canvas _printCanvas;

        private Panel _canvasContainer;
        private string _printTaskName;

        // Event callback which is called after print preview pages are generated.
        //protected event EventHandler PreviewPagesCreated;

        private bool _directPrint = false;

        /// <summary>
        /// The list of elements to print.
        /// </summary>
        private List<FrameworkElement> _elementsToPrint;

        /// <summary>
        /// The options for the print dialog.
        /// </summary>
        private Microsoft.Toolkit.Uwp.Helpers.PrintHelperOptions _printHelperOptions;

        /// <summary>
        /// The default options for the print dialog.
        /// </summary>
        private Microsoft.Toolkit.Uwp.Helpers.PrintHelperOptions _defaultPrintHelperOptions;

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        protected FrameworkElement firstPage;

        /// <summary>
        ///  A reference back to the scenario page used to access XAML elements on the scenario page
        /// </summary>
        //protected FrameworkElement scenarioPage;

        /// <summary>
        ///  A hidden canvas used to hold pages we wish to print
        /// </summary>
        //protected Canvas PrintCanvas
        //{
        //    get
        //    {
        //        return scenarioPage.FindName("container") as Canvas;
        //    }
        //}
        private Panel PrintCanvas;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenarioPage">The scenario page constructing us</param>
        public PrintHelper(Panel canvasContainer, Microsoft.Toolkit.Uwp.Helpers.PrintHelperOptions defaultPrintHelperOptions = null)
        {
            if (canvasContainer == null)
            {
                throw new ArgumentNullException();
            }

            //this.scenarioPage = scenarioPage;
            PrintCanvas = canvasContainer;

            _printPreviewPages = new List<FrameworkElement>();
            _printCanvas = new Canvas();
            _printCanvas.Opacity = 0;

            _canvasContainer = canvasContainer;
            _canvasContainer.RequestedTheme = ElementTheme.Light;

            _elementsToPrint = new List<FrameworkElement>();

            _defaultPrintHelperOptions = defaultPrintHelperOptions ?? new Microsoft.Toolkit.Uwp.Helpers.PrintHelperOptions();

            RegisterForPrinting();

            // Initialize print content for this scenario, just one page to print with 
            //Page page = new Page();
            //page.Content = _elementsToPrint[0];
            //PreparePrintContent(page);
        }

        /// <summary>
        /// Adds an element to the list of elements to print.
        /// </summary>
        /// <param name="element">Framework element to print.</param>
        /// <remarks>The element cannot have a parent; It must not be included in any visual tree.</remarks>
        public void AddFrameworkElementToPrint(FrameworkElement element)
        {
            if (element.Parent != null)
            {
                throw new ArgumentException("Printable elements cannot have a parent.");
            }

            _elementsToPrint.Add(element);
        }

        /// <summary>
        /// Removes an element from the list of elements to print.
        /// </summary>
        /// <param name="element">Framework element to remove</param>
        public void RemoveFrameworkElementToPrint(FrameworkElement element)
        {
            _elementsToPrint.Remove(element);
        }

        /// <summary>
        /// Removes all elements from the list of elements to print.
        /// </summary>
        public void ClearListOfPrintableFrameworkElements()
        {
            _elementsToPrint.Clear();
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public virtual void RegisterForPrinting()
        {
            _printDocument = new PrintDocument();

            _printDocumentSource = _printDocument.DocumentSource;
            _printDocument.Paginate += CreatePrintPreviewPages;
            _printDocument.GetPreviewPage += GetPrintPreviewPage;
            _printDocument.AddPages += AddPrintPages;

            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public virtual void UnregisterForPrinting()
        {
            if (_printDocument == null)
            {
                return;
            }

            _printDocument.Paginate -= CreatePrintPreviewPages;
            _printDocument.GetPreviewPage -= GetPrintPreviewPage;
            _printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            // Mover to DetachCanvas()
            //PrintManager printMan = PrintManager.GetForCurrentView();
            //printMan.PrintTaskRequested -= PrintTaskRequested;

            PrintCanvas.Children.Clear();
        }

        public async Task ShowPrintUIAsync(string printTaskName)
        {
            // Catch and print out any errors reported
            //try
            //{
                // Launch print process
                _printTaskName = printTaskName;
                await PrintManager.ShowPrintUIAsync();
            //}
            //catch (Exception e)
            //{
            //    MainPage.Current.NotifyUser("Error printing: " + e.Message + ", hr=" + e.HResult, NotifyType.ErrorMessage);
            //}
        }

        /// <summary>
        /// Method that will generate print content for the scenario
        /// For scenarios 1-4: it will create the first page from which content will flow
        /// Scenario 5 uses a different approach
        /// </summary>
        /// <param name="page">The page to print</param>
        public virtual void PreparePrintContent(Page page)
        {
            if (firstPage == null)
            {
                firstPage = page;
                //StackPanel header = (StackPanel)firstPage.FindName("Header");
                //header.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(firstPage);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();

            //_printCanvas.Children.Add(firstPage);
            //_printCanvas.InvalidateMeasure();
            //_printCanvas.UpdateLayout();
        }

        private async Task DetachCanvas()
        {
            if (!_directPrint)
            {
                await Microsoft.Toolkit.Uwp.Helpers.DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    _canvasContainer.Children.Remove(_printCanvas);
                    _printCanvas.Children.Clear();
                });
            }

            //_stateBags.Clear();

            //// Clear the cache of preview pages
            //await ClearPageCache();

            // Remove the handler for printing initialization.
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("C# Printing SDK Sample", sourceRequested =>
            {
                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        _canvasContainer.RequestedTheme = ElementTheme.Default;
                        await DetachCanvas();

                        switch (args.Completion)
                        {
                            case PrintTaskCompletion.Failed:
                            {
                                OnPrintFailed?.Invoke();
                                break;
                            }
                            case PrintTaskCompletion.Canceled:
                            {
                                OnPrintCanceled?.Invoke();
                                break;
                            }
                            case PrintTaskCompletion.Submitted:
                            {
                                OnPrintSucceeded?.Invoke();
                                break;
                            }
                        }
                    });
                };

                sourceRequested.SetSource(_printDocumentSource);
            });
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected virtual async void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            // Get the PrintTaskOptions
            PrintTaskOptions printingOptions = e.PrintTaskOptions;

            // Get the page description to determine how big the page is
            PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

            if (_directPrint)
            {
                _canvasContainer.RequestedTheme = ElementTheme.Light;
                foreach (FrameworkElement element in this._canvasContainer.Children)
                {
                    _printPreviewPages.Add(element);
                }
            }
            else
            {
                // Attach the canvas
                if (!_canvasContainer.Children.Contains(_printCanvas))
                {
                    _canvasContainer.Children.Add(_printCanvas);
                }

                _canvasContainer.RequestedTheme = ElementTheme.Light;

                // Clear the cache of preview pages
                await ClearPageCache();

                lock (_printPreviewPages)
                {

                    // Clear the cache of preview pages
                    //_printPreviewPages.Clear();

                    // Clear the print canvas of preview pages
                    //PrintCanvas.Children.Clear();
                    _printCanvas.Children.Clear();

                    // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
                    RichTextBlockOverflow lastRTBOOnPage;

                    // We know there is at least one page to be printed. passing null as the first parameter to
                    // AddOnePrintPreviewPage tells the function to add the first page.
                    lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);

                    // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
                    // page has extra content
                    while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible)
                    {
                        lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
                    }

                    OnPreviewPagesCreated?.Invoke(_printPreviewPages);

                    PrintDocument printDoc = (PrintDocument)sender;

                    // Report the number of preview pages created
                    printDoc.SetPreviewPageCount(_printPreviewPages.Count, PreviewPageCountType.Intermediate);
                }
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;
            printDoc.SetPreviewPage(e.PageNumber, _printPreviewPages[e.PageNumber - 1]);
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < _printPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                _printDocument.AddPage(_printPreviewPages[i]);
            }
            
            PrintDocument printDoc = (PrintDocument)sender;
            
            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        protected virtual RichTextBlockOverflow AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // The link container for text overflowing in this page
            RichTextBlockOverflow textLink;

            // Check if this is the first page ( no previous RichTextBlockOverflow)
            if (lastRTBOAdded == null)
            {
                // If this is the first page add the specific scenario content
                page = firstPage;

                // Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
                //StackPanel footer = (StackPanel)page.FindName("Footer");
                //footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                // Flow content (text) from previous pages
                page = new ContinuationPage(lastRTBOAdded);
            }

            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;
            
            Grid printableArea = (Grid)page.FindName("printPage1");

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = firstPage.Width - marginWidth;
            printableArea.Height = firstPage.Height - marginHeight;

            // Add the (newley created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(page);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();

            //_printCanvas.Children.Add(page);
            //_printCanvas.InvalidateMeasure();
            //_printCanvas.UpdateLayout();

            // Find the last text container and see if the content is overflowing
            textLink = (RichTextBlockOverflow)page.FindName("ContinuationPageLinkedContainer");

            // Check if this is the last page
            if (!textLink.HasOverflowContent && textLink.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                //StackPanel footer = (StackPanel)page.FindName("Footer");
                //footer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                PrintCanvas.UpdateLayout();
                _printCanvas.UpdateLayout();
            }

            // Add the page to the page preview collection
            _printPreviewPages.Add(page);

            return textLink;
        }

        private Task ClearPageCache()
        {
            return Microsoft.Toolkit.Uwp.Helpers.DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                if (!_directPrint)
                {
                    foreach (Page page in _printPreviewPages)
                    {
                        page.Content = null;
                    }
                }

                _printPreviewPages.Clear();
            });
        }

    }
}
