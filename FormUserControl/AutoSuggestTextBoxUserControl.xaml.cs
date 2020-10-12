using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using FormControlBasicsNamespace;
using SharedCode;
using SharedCode.Helpers;

using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public class ListItemData
    {
        public string Text { get; set; }
        public ICommand Command { get; set; }
    }

    public sealed partial class AutoSuggestTextBoxUserControl : FormControlBasics
    {
        readonly ObservableCollection<ListItemData> collection = new ObservableCollection<ListItemData>();

        readonly StandardUICommand _deleteCommand;
        string _matchedText = "";


        private List<string> _PreviousTexts;
        public List<string> PreviousTexts
        {
            get
            {
                if (_PreviousTexts is null)
                {
                    _PreviousTexts = new List<string>();
                }
                return _PreviousTexts;
            }
            set => _PreviousTexts = value;
        }

        public DependencyObject Panel => autoSuggestTextBoxUserControl;

        public override FormControlBasics RootPanel => rootPanel;


        public AutoSuggestTextBoxUserControl()
        {
            InitializeComponent();

            ScanControls(autoSuggestTextBoxUserControl);

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                _deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
                _deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;
            }
        }

        private void DeleteCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter != null)
            {
                foreach (var i in collection)
                {
                    if (i.Text == (args.Parameter as string))
                    {
                        bool success = PreviousTexts.Remove(i.Text);
                        collection.Remove(i);
                        return;
                    }
                }
            }
        }

        private void ListViewSwipeContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
            }
        }

        private void ListViewSwipeContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

        private async void AutoSuggestTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            string name = (sender as AutoSuggestBox).Name;
            PreviousTexts = await ApplicationData.Current.LocalFolder.ReadAsync<List<string>>(name);

        }

        private async void AutoSuggestTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                collection.Clear();
                foreach (string text in PreviousTexts)
                {
                    string lowerCaseS = text.ToLower();
                    //if (string.IsNullOrEmpty(sender.Text) || lowerCaseS.StartsWith(sender.Text.ToLower()))
                    if (lowerCaseS.StartsWith(sender.Text.ToLower()))
                    {
                        collection.Add(new ListItemData { Text = text, Command = _deleteCommand });
                    }
                }

                bool matchFound = false;
                for (int i = 0; i < PreviousTexts.Count; i++)
                {
                    if (sender.Text.ToLower().Contains(PreviousTexts[i].ToLower()))
                    {
                        PreviousTexts[i] = sender.Text;
                        _matchedText = sender.Text;
                        matchFound = true;
                        break;
                    }
                }
                if (matchFound && !string.IsNullOrEmpty(sender.Text))
                {
                    await ApplicationData.Current.LocalFolder.SaveAsync(sender.Name, PreviousTexts);
                }
                else if (!matchFound && !string.IsNullOrEmpty(sender.Text))
                {
                    PreviousTexts.Add(sender.Text);
                    _matchedText = sender.Text;
                }

                sender.ItemsSource = collection;
            }
            AutoSuggestBox_TextChanged(sender, null);
        }

        private void AutoSuggestTextBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //Set sender.Text.You can use args.SelectedItem to build your text string.
            sender.Text = (args.SelectedItem as ListItemData).Text;
            if (_matchedText != sender.Text)
            {
                PreviousTexts.Remove(_matchedText);
            }
        }

    }
}
