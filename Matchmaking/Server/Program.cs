using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        public Program()
        {

        }

        public static void Main(String[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            SocketServer server = new SocketServer(IPAddress.Loopback, 26950);
            server.Listen();
            await Task.Delay(-1);
        }
    }
}
