using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Constants
    {
        public const int TICKS_PER_SECOND = 128;
        public const float MS_PER_TICK = 1000f / TICKS_PER_SECOND;

        public const int WAITING_ROOM_ID = 0;
        public const int ROOM_MINIMUM_CLIENTS = 2;
    }
}
