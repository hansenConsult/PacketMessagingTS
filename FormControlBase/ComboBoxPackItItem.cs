using Windows.UI.Xaml.Media;


namespace FormControlBaseClass
{

    public abstract partial class FormControlBase
    {
        public class ComboBoxPackItItem
        {
            public string Item
            { get; set; }

            public string Data
            { get; set; }

            public Brush BackgroundColor
            { get; set; }

            public ComboBoxPackItItem()
            { }

            public ComboBoxPackItItem(string item, string data)
            {
                Item = item;
                Data = data;
            }

            public ComboBoxPackItItem(string item, string data, Brush backgroundColor)
            {
                Item = item;
                Data = data;
                BackgroundColor = backgroundColor;
            }
        }


    }
}


