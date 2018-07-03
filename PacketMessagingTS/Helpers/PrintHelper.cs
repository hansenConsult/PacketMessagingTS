﻿using FormControlBaseClass;
using MetroLog;
using PacketMessagingTS.Views;
using SharedCode;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace PacketMessagingTS.Helpers
{
    public class PrintHelper
    {
        private static ILogger log = LogManagerFactory.DefaultLogManager.GetLogger<PrintHelper>();
        private static LogHelper _logHelper = new LogHelper(log);

        /// <summary>
        /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
        /// </summary>
        protected double _ApplicationContentMarginLeft = 0.075;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of that area's height
        /// </summary>
        protected double _ApplicationContentMarginTop = 0.03;
        
        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        protected PrintDocument _printDocument;     // Required

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        protected IPrintDocumentSource _printDocumentSource;    // Required

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<UIElement> _printPreviewPages;

        // Event callback which is called after print preview pages are generated.  Photos scenario uses this to do filtering of preview pages
        protected event EventHandler PreviewPagesCreated;

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        protected FrameworkElement _firstPage;

        /// <summary>
        ///  A reference back to the scenario page used to access XAML elements on the scenario page
        /// </summary>
        protected Page _scenarioPage;

        /// <summary>
        ///  A hidden canvas used to hold pages we wish to print
        /// </summary>
        protected Canvas PrintCanvas
        {
            get
            {
                return ((FormsPage)_scenarioPage).PacketForm.FindName("PrintCanvas") as Canvas;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenarioPage">The scenario page constructing us</param>
        public PrintHelper(FormsPage scenarioPage)
        {
            _scenarioPage = scenarioPage;
            _printPreviewPages = new List<UIElement>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenarioPage">The scenario page constructing us</param>
        public PrintHelper(Page scenarioPage)
        {
            _scenarioPage = scenarioPage;
            _printPreviewPages = new List<UIElement>();
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
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= PrintTaskRequested;

            PrintCanvas?.Children.Clear();
        }

        public async Task ShowPrintUIAsync()
        {
            if (PrintManager.IsSupported())
            {
                // Catch and print out any errors reported
                try
                {
                    await PrintManager.ShowPrintUIAsync();
                }
                catch (Exception e)
                {
                    _logHelper.Log(LogLevel.Error, "Error printing: " + e.Message + ", hr=" + e.HResult);
                    await Utilities.ShowMessageDialogAsync("Error printing" + e.Message);
                }
            }
            else
            {
                await Utilities.ShowMessageDialogAsync("Printing is not supported on this device");
            }
        }

        /// <summary>
        /// Method that will generate print content for the scenario
        /// For scenarios 1-4: it will create the first page from which content will flow
        /// Scenario 5 uses a different approach
        /// </summary>
        /// <param name="page">The page to print</param>
        public virtual void PreparePrintContent(FormsPage page)
        {
            if (_firstPage == null)
            {
                _firstPage = page;
            }

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            //PrintCanvas.Children.Add(_firstPage);
            //PrintCanvas.InvalidateMeasure();
            //PrintCanvas.UpdateLayout();
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)   // Required 1.called
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("Print Form", sourceRequestedArgs =>
            {
                IList<string> displayedOptions = printTask.Options.DisplayedOptions;

                // Choose the printer options to be shown.
                // The order in which the options are appended determines the order in which they appear in the UI
                displayedOptions.Clear();
                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies);
                displayedOptions.Add(StandardPrintTaskOptions.ColorMode);

                // Preset the default value of the printer option
                printTask.Options.MediaSize = PrintMediaSize.NorthAmerica9x11;
                printTask.Options.ColorMode = PrintColorMode.Monochrome;                

                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        await _scenarioPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            _logHelper.Log(LogLevel.Error, "Failed to print.");
                            await Utilities.ShowMessageDialogAsync("Failed to print.");
                        });
                    }
                };

                sourceRequestedArgs.SetSource(_printDocumentSource);
            });
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)  // Required  2. called
        {
            lock (_printPreviewPages)
            {
                // Clear the cache of preview pages
                _printPreviewPages.Clear();

                // Clear the print canvas of preview pages
                //PrintCanvas?.Children.Clear();

                // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
                //RichTextBlockOverflow lastRTBOOnPage;

                // Get the PrintTaskOptions
                PrintTaskOptions printingOptions = ((PrintTaskOptions)e.PrintTaskOptions);

                // Get the page description to determine how big the page is
                PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

                // We know there is at least one page to be printed. passing null as the first parameter to
                // AddOnePrintPreviewPage tells the function to add the first page.
                //lastRTBOOnPage = AddOnePrintPreviewPage(null, pageDescription);
                AddOnePrintPreviewPage(null, pageDescription);

                // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
                // page has extra content
                //while (lastRTBOOnPage.HasOverflowContent && lastRTBOOnPage.Visibility == Windows.UI.Xaml.Visibility.Visible)
                //{
                //    lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
                //}

                if (PreviewPagesCreated != null)
                {
                    PreviewPagesCreated.Invoke(_printPreviewPages, null);
                }

                PrintDocument printDoc = (PrintDocument)sender;

                // Report the number of preview pages created
                printDoc.SetPreviewPageCount(_printPreviewPages.Count, PreviewPageCountType.Intermediate);
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)    // Required
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
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)    // Required
        {
            // Loop over all of the preview pages and add each one to  add each page to be printed
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
        protected virtual void AddOnePrintPreviewPage(RichTextBlockOverflow lastRTBOAdded, PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page;

            // The link container for text overflowing in this page
            //RichTextBlockOverflow textLink;

            // Check if this is the first page ( no previous RichTextBlockOverflow)
            //if (lastRTBOAdded == null)
            {
                // If this is the first page add the specific scenario content
                page = _firstPage;

                // Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
                //StackPanel footer = (StackPanel)page.FindName("Footer");
                //footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            //else
            //{
            //    // Flow content (text) from previous pages
            //    page = new ContinuationPage(lastRTBOAdded);
            //}

            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            StackPanel printableArea = ((FormsPage)page).PacketForm.FindName("PrintableArea") as StackPanel;

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * _ApplicationContentMarginLeft * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * _ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            printableArea.Width = _firstPage.Width - marginWidth;
            printableArea.Height = _firstPage.Height - marginHeight;

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            //PrintCanvas.Children.Add(page);
            //PrintCanvas.InvalidateMeasure();
            //PrintCanvas.UpdateLayout();

            // Find the last text container and see if the content is overflowing
            //textLink = (RichTextBlockOverflow)page.FindName("ContinuationPageLinkedContainer");

            // Check if this is the last page
            //if (!textLink.HasOverflowContent && textLink.Visibility == Windows.UI.Xaml.Visibility.Visible)
            //{
            //    StackPanel footer = (StackPanel)page.FindName("Footer");
            //    footer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //    PrintCanvas.UpdateLayout();
            //}

            // Add the page to the page preview collection
            _printPreviewPages.Add(printableArea);

            //return textLink;
        }
    }
}
