using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using FormControlBasicsNamespace;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Media.Capture.Frames;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
        ObservableCollection<ListItemData> collection = new ObservableCollection<ListItemData>();

        StandardUICommand _deleteCommand;

        private List<string> _PreviousTexts = new List<string>() { "MTVEOC", "XSCEOC" };
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
        }

        private string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                Set(ref _Text, value);
                if (!PreviousTexts.Contains(_Text))
                {
                    PreviousTexts.Add(_Text);
                    collection.Add(new ListItemData { Text = _Text, Command = _deleteCommand });
                }
            }
        }

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
                        collection.Remove(i);
                        PreviousTexts.Remove(i.Text);
                        return;
                    }
                }
            }
            if (ListViewRight.SelectedIndex != -1)
            {
                PreviousTexts.RemoveAt(ListViewRight.SelectedIndex);
                collection.RemoveAt(ListViewRight.SelectedIndex);
            }
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var listView = (ListView)sender;
            listView.ItemsSource = collection;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewRight.SelectedIndex != -1)
            {
                var item = collection[ListViewRight.SelectedIndex];
                Text = PreviousTexts[ListViewRight.SelectedIndex];
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

        private void ControlExample_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                //var deleteCommand = new StandardUICommand(StandardUICommandKind.Delete);
                //deleteCommand.ExecuteRequested += DeleteCommand_ExecuteRequested;

                //DeleteFlyoutItem.Command = _deleteCommand;

                foreach (string s in PreviousTexts)
                {
                    collection.Add(new ListItemData { Text = s, Command = _deleteCommand });
                }
            }
            else
            {
                foreach (string s in PreviousTexts)
                {
                    collection.Add(new ListItemData { Text = s, Command = null });
                }
            }
        }

        private void ListViewRight_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            MenuFlyout flyout = new MenuFlyout();
            ListItemData data = (ListItemData)args.Item;
            MenuFlyoutItem item = new MenuFlyoutItem() { Command = data.Command };
            flyout.Opened += delegate (object element, object e) {
                MenuFlyout flyoutElement = element as MenuFlyout;
                ListViewItem elementToHighlight = flyoutElement.Target as ListViewItem;
                elementToHighlight.IsSelected = true;
            };
            flyout.Items.Add(item);
            args.ItemContainer.ContextFlyout = flyout;
        }
    }
}
