using System.Collections.Generic;

using FormControlBasicsNamespace;

using SharedCode.Models;

using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public sealed partial class RadioOperatorUserControl : FormControlBasics
    {
        public RadioOperatorUserControl()
        {
            InitializeComponent();

            ScanControls(radioOperatorOnly);
        }

        //public List<FormControl> FormControlsList => _formControlsList;

        public DependencyObject Panel => radioOperatorOnly;

        public override FormControlBasics RootPanel => rootPanel;

    }
}
