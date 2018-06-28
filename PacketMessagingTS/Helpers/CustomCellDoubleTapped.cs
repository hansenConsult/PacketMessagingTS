using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.ViewModels;
using SharedCode;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace PacketMessagingTS.Helpers
{
    class CustomCellDoubleTapped : DataGridCommand
    {
        public CustomCellDoubleTapped()
        {
            this.Id = CommandId.CellDoubleTap;
        }

        public override bool CanExecute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            // put your custom logic here
            return true;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as DataGridCellInfo;
            Singleton<MainViewModel>.Instance.OpenMessageFromDoubleClick(context.Item as PacketMessage);
        }

    }
}
