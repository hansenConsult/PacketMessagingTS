﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketMessagingTS.Core.Helpers
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
