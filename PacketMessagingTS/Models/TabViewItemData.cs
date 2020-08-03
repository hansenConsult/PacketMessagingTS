using System.Runtime.CompilerServices;

namespace PacketMessagingTS.Models
{
    public class TabViewItemData
    {
        private int index;
        public int Index
        {
            get;
            set;
        }


        public string Header
        {
            get;
            set;
        }

        public string Folder
        { get; set; }

        public object Content { get; set; }
    }
}
