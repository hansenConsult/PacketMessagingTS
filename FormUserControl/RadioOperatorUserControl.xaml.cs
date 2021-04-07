using System;
using System.Collections.Generic;
using System.Linq;

using FormControlBaseMvvmNameSpace;

//using FormControlBasicsNamespace;

using SharedCode;
using SharedCode.Models;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;



// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FormUserControl
{
    public sealed partial class RadioOperatorUserControl : FormControlBaseMvvm
    {
        public RadioOperatorUserControlViewModel ViewModel = RadioOperatorUserControlViewModel.Instance;


        public RadioOperatorUserControl()
        {
            InitializeComponent();

            ScanControls(radioOperatorOnly);
        }

        //public List<FormControl> FormControlsList => _formControlsList;

        public DependencyObject Panel => radioOperatorOnly;

        public override FormControlBaseMvvm RootPanel => rootPanel;

    }
}
