using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using SharedCode;

namespace PacketMessagingTS.Helpers
{
    public class RowStyleSelector : StyleSelector
    {
        public Style MessageOpened { get; set; }
        public Style MessageUnopened { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            //var cell = (item as DataGridCellInfo);
            //var packetMessage = cell.Item as PacketMessage;
            //PacketMessage packetMessage = item as PacketMessage;

            //if (packetMessage.MessageOpened)
            //{
            //    //return MessageOpened;
            //    return null;
            //}

            //return MessageUnopened;
            return null;
        }

    }
}
