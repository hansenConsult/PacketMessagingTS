using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketMessagingTS.Models
{
    class Drafts
    {
        public DateTime CreateTime { get; set; }

        public string Subject { get; set; }

        public string MessageNumber { get; set; }

        public string MessageTo { get; set; }

        public string MessageFrom { get; set; }

        public string BBS { get; set; }
    }
}
