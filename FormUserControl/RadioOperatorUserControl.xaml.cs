﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using FormControlBasicsNamespace;

using SharedCode.Models;

using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class RadioOperatorUserControl : FormControlBasics
    {
        public RadioOperatorUserControl()
        {
            InitializeComponent();

            ScanControls(radioOperatorOnly);
        }

        public List<FormControl> FormControlsList => _formControlsList;

        public DependencyObject Panel => radioOperatorOnly;

        public override FormControlBasics RootPanel => rootPanel;

    }
}
