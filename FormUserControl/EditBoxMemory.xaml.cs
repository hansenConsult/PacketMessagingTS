using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using FormControlBasicsNamespace;

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


    public sealed partial class EditBoxMemory : FormControlBasics
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

        //private string _ChosenText;
        //public string ChosenText
        //{
        //    get => _ChosenText;
        //    set => Set(ref _ChosenText, value);
        //}

        public EditBoxMemory()
        {
            InitializeComponent();

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                _deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
                _deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;

                //DeleteFlyoutItem.Command = _deleteCommand;
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
                //collection.Clear();
                //foreach (string s in PreviousTexts)
                //{
                //    if (!string.IsNullOrEmpty(s))
                //    {
                //        collection.Add(new ListItemData { Text = s, Command = _deleteCommand });
                //    }
                //}
            }
            //int selectedIndex = autoSuggestMemory.Items.;
            //if (ListViewRight.SelectedIndex != -1)
            //{
            //    PreviousTexts.RemoveAt(ListViewRight.SelectedIndex);
            //    collection.RemoveAt(ListViewRight.SelectedIndex);
            //}
        }

        //private void ListView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    var listView = (ListView)sender;
        //    listView.ItemsSource = collection;
        //}

        //private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //if (ListViewRight.SelectedIndex != -1)
        //    //{
        //    //    var item = collection[ListViewRight.SelectedIndex];
        //    //    Text = PreviousTexts[ListViewRight.SelectedIndex];
        //    //}
        //}

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

        private async void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {
            PreviousTexts = await ApplicationData.Current.LocalFolder.ReadAsync<List<string>>(Name);

            //if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            //{
            //    //DeleteFlyoutItem.Command = _deleteCommand;

            //    foreach (string s in PreviousTexts)
            //    {
            //        collection.Add(new ListItemData { Text = s, Command = _deleteCommand });
            //    }
            //}
            //else
            //{
            //    foreach (string s in PreviousTexts)
            //    {
            //        collection.Add(new ListItemData { Text = s, Command = null });
            //    }
            //}
            //(sender as AutoSuggestBox).ItemsSource = collection;
        }

        //private void ListViewRight_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        //{
        //    MenuFlyout flyout = new MenuFlyout();
        //    ListItemData data = (ListItemData)args.Item;
        //    MenuFlyoutItem item = new MenuFlyoutItem() { Command = data.Command };
        //    flyout.Opened += delegate (object element, object e) {
        //        MenuFlyout flyoutElement = element as MenuFlyout;
        //        ListViewItem elementToHighlight = flyoutElement.Target as ListViewItem;
        //        elementToHighlight.IsSelected = true;
        //    };
        //    flyout.Items.Add(item);
        //    args.ItemContainer.ContextFlyout = flyout;
        //}

        private async void AutoSuggestBoxMemory_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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
                    await ApplicationData.Current.LocalFolder.SaveAsync(Name, PreviousTexts);
                }
                else if (!matchFound && !string.IsNullOrEmpty(sender.Text))
                {
                    PreviousTexts.Add(sender.Text);
                    _matchedText = sender.Text;
                }

                sender.ItemsSource = collection;
            }
        }

        private void TextBoxMemory_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
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
