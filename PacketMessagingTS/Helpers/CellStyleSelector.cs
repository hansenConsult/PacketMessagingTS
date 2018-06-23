using FormControlBaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PacketMessagingTS.Helpers
{
    public class CellStyleSelector : StyleSelector
    {
        public Style MessageOpened { get; set; }
        public Style MessageUnopened { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            var cell = (item as DataGridCellInfo);
            var packetMessage = cell.Item as PacketMessage;

            if (packetMessage.MessageOpened)
            {
                return this.MessageOpened;
            }

            return this.MessageUnopened;
        }

    }
}
