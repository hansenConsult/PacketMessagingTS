using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FormControlBaseClass
{
	public class FormControl
	{
        public Brush BaseBorderColor
		{ get; set; }

        public Brush RequiredBorderBrush { get; } = new SolidColorBrush(Colors.Red);

        public Control InputControl
		{ get; private set; }

		public FormControl()
		{
			
		}

		public FormControl(Control control)
		{
			InputControl = control;
			BaseBorderColor = control.BorderBrush;
		}
	}
}
