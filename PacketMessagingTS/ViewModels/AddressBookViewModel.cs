using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PacketMessagingTS.Helpers;

namespace PacketMessagingTS.ViewModels
{
    public class AddressBookViewModel : ViewModelBase
    {
        public static AddressBookViewModel Instance { get; } = new AddressBookViewModel();

        public AddressBookViewModel()
        {

        }

    }
}
