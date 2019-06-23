using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Helpers
{
    public class MessageOriginHelper
    {
        public enum MessageOrigin
        {
            Archived,
            Deleted,
            Draft,
            Received,
            Sent,
            Unsent,
            New
        }

    }
}
