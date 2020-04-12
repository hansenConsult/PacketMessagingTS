using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace FormControlBasicsNamespace
{
	public class FormControl
	{
        public Brush BaseBorderColor
		{ get; set; }

        public Brush RequiredBorderBrush { get; } = FormControlBasics.RedBrush;

        public FrameworkElement InputControl
		{ get; private set; }

		public FrameworkElement UserControl
		{ get; private set; }

		public FormControl(FrameworkElement control, FrameworkElement userControl = null)
		{
			UserControl = userControl;

			InputControl = control;
			if (control.GetType() == typeof(Control))
			{
				BaseBorderColor = (control as Control).BorderBrush;
			}
		}
	}
}
